using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.SqlServer.Design.Internal;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// Command to compare EF Core schema discovery with direct SQL approach
    /// </summary>
    public class SchemaCompareCommand : ICommand
    {
        private readonly IDatabaseSchemaProviderFactory _schemaProviderFactory;
        private readonly IRelationshipDetectorFactory _relationshipDetectorFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaCompareCommand"/> class.
        /// </summary>
        /// <param name="schemaProviderFactory">The schema provider factory to use.</param>
        /// <param name="relationshipDetectorFactory">The relationship detector factory to use.</param>
        /// <param name="logger">The logger to use.</param>
        public SchemaCompareCommand(
            IDatabaseSchemaProviderFactory schemaProviderFactory,
            IRelationshipDetectorFactory relationshipDetectorFactory,
            ILogger logger)
        {
            _schemaProviderFactory = schemaProviderFactory ?? throw new ArgumentNullException(nameof(schemaProviderFactory));
            _relationshipDetectorFactory = relationshipDetectorFactory ?? throw new ArgumentNullException(nameof(relationshipDetectorFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Name => "schema-compare";

        /// <inheritdoc/>
        public string Description => "Compare EF Core schema discovery with direct SQL approach";

        /// <inheritdoc/>
        public string Usage => "schema-compare --connection <connection-string> [--database <database-name>] [--format <json|text>]";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            try
            {
                // Extract options
                options.TryGetValue("connection", out var connectionString);
                options.TryGetValue("database", out var databaseName);
                options.TryGetValue("format", out var format);
                
                // Set default values
                format ??= "text";
                
                _logger.Info($"Comparing schema discovery approaches for database: {databaseName ?? "default"}");
                _logger.Info($"Connection: {MaskConnectionString(connectionString)}");
                
                // Ensure we have a valid connection string
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    _logger.Error("Connection string is required");
                    return;
                }
                
                // If database name is specified, append it to the connection string
                if (!string.IsNullOrWhiteSpace(databaseName))
                {
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    builder.InitialCatalog = databaseName;
                    connectionString = builder.ConnectionString;
                }
                
                // Run the comparison
                await CompareSchemaDiscovery(connectionString, format);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error comparing schema discovery: {ex.Message}");
                _logger.Error(ex.ToString());
            }
        }

        /// <inheritdoc/>
        public bool ValidateOptions(IReadOnlyDictionary<string, string> options)
        {
            return GetValidationErrors(options).Count == 0;
        }

        /// <inheritdoc/>
        public IList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options)
        {
            var errors = new List<string>();
            
            if (!options.ContainsKey("connection"))
            {
                errors.Add("Connection string is required (--connection)");
            }
            
            return errors;
        }

        /// <inheritdoc/>
        public IList<CommandOption> GetOptions()
        {
            return new List<CommandOption>
            {
                new CommandOption("connection", "Connection string to the database", true),
                new CommandOption("database", "Database name (if not specified in connection string)", false),
                new CommandOption("format", "Output format (json or text)", false)
            };
        }

        /// <inheritdoc/>
        public IList<string> GetRequiredOptions()
        {
            return new List<string> { "connection" };
        }

        private async Task CompareSchemaDiscovery(string connectionString, string format)
        {
            // Measure EF Core schema discovery performance
            var efCoreStopwatch = Stopwatch.StartNew();
            int efCoreTableCount = 0;
            int efCoreColumnCount = 0;
            int efCoreRelationshipCount = 0;
            
            try
            {
                // Create a service provider with the necessary services for EF Core scaffolding
                var serviceProvider = CreateEfCoreServices();
                
                // Get the database model factory
                var databaseModelFactory = serviceProvider.GetRequiredService<IDatabaseModelFactory>();
                
                // Get the database model
                var databaseModel = databaseModelFactory.Create(connectionString, 
                    new DatabaseModelFactoryOptions());
                
                // Count tables, columns, and relationships
                efCoreTableCount = databaseModel.Tables.Count;
                efCoreColumnCount = databaseModel.Tables.Sum(t => t.Columns.Count);
                efCoreRelationshipCount = databaseModel.Tables.Sum(t => t.ForeignKeys.Count);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error measuring EF Core performance: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.Error($"Inner exception: {ex.InnerException.Message}");
                }
            }
            
            efCoreStopwatch.Stop();
            
            // Measure SQL Client schema discovery performance
            var sqlClientStopwatch = Stopwatch.StartNew();
            var sqlClientSchema = await AnalyzeDatabaseSchemaWithSqlClient(connectionString);
            sqlClientStopwatch.Stop();
            
            // Calculate performance difference
            var efCoreTime = efCoreStopwatch.ElapsedMilliseconds;
            var sqlClientTime = sqlClientStopwatch.ElapsedMilliseconds;
            var timeDifference = sqlClientTime - efCoreTime;
            var percentageDifference = efCoreTime > 0 ? (sqlClientTime * 100.0 / efCoreTime) : 0;
            
            // Output the results
            if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                DisplayComparisonAsJson(
                    efCoreTableCount, efCoreColumnCount, efCoreRelationshipCount, efCoreTime,
                    sqlClientSchema.Tables.Count, sqlClientSchema.Columns.Count, sqlClientSchema.Relationships.Count, sqlClientTime);
            }
            else
            {
                DisplayComparisonAsText(
                    efCoreTableCount, efCoreColumnCount, efCoreRelationshipCount, efCoreTime,
                    sqlClientSchema.Tables.Count, sqlClientSchema.Columns.Count, sqlClientSchema.Relationships.Count, sqlClientTime);
            }
        }

        private void DisplayComparisonAsText(
            int efCoreTableCount, int efCoreColumnCount, int efCoreRelationshipCount, long efCoreTime,
            int sqlClientTableCount, int sqlClientColumnCount, int sqlClientRelationshipCount, long sqlClientTime)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Schema Discovery Comparison ===");
            sb.AppendLine();
            
            sb.AppendLine("EF Core:");
            sb.AppendLine($"  Tables: {efCoreTableCount}");
            sb.AppendLine($"  Columns: {efCoreColumnCount}");
            sb.AppendLine($"  Relationships: {efCoreRelationshipCount}");
            sb.AppendLine($"  Time: {efCoreTime}ms");
            sb.AppendLine();
            
            sb.AppendLine("SQL Client:");
            sb.AppendLine($"  Tables: {sqlClientTableCount}");
            sb.AppendLine($"  Columns: {sqlClientColumnCount}");
            sb.AppendLine($"  Relationships: {sqlClientRelationshipCount}");
            sb.AppendLine($"  Time: {sqlClientTime}ms");
            sb.AppendLine();
            
            // Calculate performance difference
            var timeDifference = sqlClientTime - efCoreTime;
            var percentageDifference = efCoreTime > 0 ? (sqlClientTime * 100.0 / efCoreTime) : 0;
            
            sb.AppendLine("Performance Comparison:");
            if (timeDifference > 0)
            {
                sb.AppendLine($"  SQL Client is slower than EF Core by {timeDifference}ms");
                sb.AppendLine($"  SQL Client takes {percentageDifference:F2}% of the time that EF Core takes");
            }
            else if (timeDifference < 0)
            {
                sb.AppendLine($"  SQL Client is faster than EF Core by {Math.Abs(timeDifference)}ms");
                sb.AppendLine($"  SQL Client takes {percentageDifference:F2}% of the time that EF Core takes");
            }
            else
            {
                sb.AppendLine($"  SQL Client and EF Core have the same performance");
            }
            
            _logger.Info(sb.ToString());
        }

        private void DisplayComparisonAsJson(
            int efCoreTableCount, int efCoreColumnCount, int efCoreRelationshipCount, long efCoreTime,
            int sqlClientTableCount, int sqlClientColumnCount, int sqlClientRelationshipCount, long sqlClientTime)
        {
            var result = new
            {
                EfCore = new
                {
                    Tables = efCoreTableCount,
                    Columns = efCoreColumnCount,
                    Relationships = efCoreRelationshipCount,
                    Time = efCoreTime
                },
                SqlClient = new
                {
                    Tables = sqlClientTableCount,
                    Columns = sqlClientColumnCount,
                    Relationships = sqlClientRelationshipCount,
                    Time = sqlClientTime
                },
                Performance = new
                {
                    TimeDifference = sqlClientTime - efCoreTime,
                    PercentageDifference = efCoreTime > 0 ? (sqlClientTime * 100.0 / efCoreTime) : 0,
                    SqlClientIsFaster = sqlClientTime < efCoreTime
                }
            };
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            _logger.Info(JsonSerializer.Serialize(result, options));
        }

        private IServiceProvider CreateEfCoreServices()
        {
            // Create a service collection
            var serviceCollection = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .AddEntityFrameworkDesignTimeServices()
                .AddSingleton<ILoggerFactory, Microsoft.Extensions.Logging.LoggerFactory>()
                .AddLogging(builder => builder.AddConsole());

            // Add SQL Server design-time services
            new SqlServerDesignTimeServices().ConfigureDesignTimeServices(serviceCollection);
            
            return serviceCollection.BuildServiceProvider();
        }

        private async Task<DatabaseSchema> AnalyzeDatabaseSchemaWithSqlClient(string connectionString)
        {
            var dbSchema = new DatabaseSchema();
            var tablesList = new List<TableInfo>();
            var columnsList = new List<ColumnInfo>();
            var relationshipsList = new List<RelationshipInfo>();
            
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                
                // Get tables and views
                using (var command = new SqlCommand(@"
                    SELECT 
                        SCHEMA_NAME(schema_id) AS [Schema],
                        name AS [Name],
                        type_desc AS [Type]
                    FROM 
                        sys.objects
                    WHERE 
                        type_desc IN ('USER_TABLE', 'VIEW', 'SYSTEM_VIEW')
                        AND SCHEMA_NAME(schema_id) != 'sys'
                    ORDER BY 
                        SCHEMA_NAME(schema_id),
                        name", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var schemaName = reader.GetString(0);
                            var name = reader.GetString(1);
                            var type = reader.GetString(2);
                            
                            var tableInfo = new TableInfo
                            {
                                Schema = schemaName,
                                Name = name,
                                Type = type
                            };
                            
                            tablesList.Add(tableInfo);
                        }
                    }
                }
                
                // Get columns for all tables and views
                using (var command = new SqlCommand(@"
                    SELECT 
                        SCHEMA_NAME(o.schema_id) AS [Schema],
                        o.name AS [TableName],
                        c.name AS [ColumnName],
                        t.name AS [DataType],
                        c.is_nullable AS [IsNullable],
                        c.max_length AS [MaxLength],
                        c.precision AS [Precision],
                        c.scale AS [Scale],
                        CASE WHEN pk.column_id IS NOT NULL THEN 1 ELSE 0 END AS [IsPrimaryKey]
                    FROM 
                        sys.columns c
                    INNER JOIN 
                        sys.objects o ON c.object_id = o.object_id
                    INNER JOIN 
                        sys.types t ON c.user_type_id = t.user_type_id
                    LEFT JOIN 
                        (SELECT ic.column_id, ic.object_id
                         FROM sys.index_columns ic
                         INNER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                         WHERE i.is_primary_key = 1) pk 
                        ON c.column_id = pk.column_id AND c.object_id = pk.object_id
                    WHERE 
                        o.type_desc IN ('USER_TABLE', 'VIEW', 'SYSTEM_VIEW')
                        AND SCHEMA_NAME(o.schema_id) != 'sys'
                    ORDER BY 
                        [Schema], 
                        [TableName], 
                        c.column_id", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var schemaName = reader.GetString(0);
                            var tableName = reader.GetString(1);
                            var columnName = reader.GetString(2);
                            var dataType = reader.GetString(3);
                            var isNullable = reader.GetBoolean(4);
                            var maxLength = reader.GetInt16(5);
                            var precision = reader.GetByte(6);
                            var scale = reader.GetByte(7);
                            var isPrimaryKey = reader.GetInt32(8) == 1;
                            
                            var columnInfo = new ColumnInfo
                            {
                                Name = columnName,
                                DataType = dataType,
                                IsNullable = isNullable,
                                MaxLength = maxLength > 0 ? maxLength : null,
                                Precision = precision > 0 ? precision : null,
                                Scale = scale > 0 ? scale : null,
                                IsPrimaryKey = isPrimaryKey
                            };
                            
                            // Add column to the appropriate table
                            var table = tablesList.FirstOrDefault(t => t.Schema == schemaName && t.Name == tableName);
                            if (table != null)
                            {
                                table.Columns.Add(columnInfo);
                            }
                            
                            // Add to the flat list of columns
                            columnsList.Add(columnInfo);
                        }
                    }
                }
                
                // Get foreign keys
                using (var command = new SqlCommand(@"
                    SELECT 
                        fk.name AS FK_NAME,
                        OBJECT_SCHEMA_NAME(fk.parent_object_id) AS FK_SCHEMA,
                        OBJECT_NAME(fk.parent_object_id) AS FK_TABLE,
                        COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS FK_COLUMN,
                        OBJECT_SCHEMA_NAME(fk.referenced_object_id) AS PK_SCHEMA,
                        OBJECT_NAME(fk.referenced_object_id) AS PK_TABLE,
                        COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS PK_COLUMN
                    FROM 
                        sys.foreign_keys fk
                    INNER JOIN 
                        sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                    ORDER BY 
                        FK_SCHEMA, 
                        FK_TABLE", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fkName = reader.GetString(0);
                            var fkSchema = reader.GetString(1);
                            var fkTable = reader.GetString(2);
                            var fkColumn = reader.GetString(3);
                            var pkSchema = reader.GetString(4);
                            var pkTable = reader.GetString(5);
                            var pkColumn = reader.GetString(6);
                            
                            var foreignKeyInfo = new ForeignKeyInfo
                            {
                                Name = fkName,
                                ForeignKeyTable = $"{fkSchema}.{fkTable}",
                                ForeignKeyColumn = fkColumn,
                                PrimaryKeyTable = $"{pkSchema}.{pkTable}",
                                PrimaryKeyColumn = pkColumn
                            };
                            
                            dbSchema.ForeignKeys.Add(foreignKeyInfo);
                            
                            var relationshipInfo = new RelationshipInfo
                            {
                                ForeignKeyTable = $"{fkSchema}.{fkTable}",
                                ForeignKeyColumn = fkColumn,
                                PrimaryKeyTable = $"{pkSchema}.{pkTable}",
                                PrimaryKeyColumn = pkColumn
                            };
                            
                            relationshipsList.Add(relationshipInfo);
                        }
                    }
                }
            }
            
            // Set the tables, columns, and relationships lists
            dbSchema.Tables = tablesList;
            dbSchema.Columns = columnsList;
            dbSchema.Relationships = relationshipsList;
            
            return dbSchema;
        }

        private string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }
            
            // Simple masking for common password patterns in connection strings
            return connectionString
                .Replace("Password=", "Password=*****")
                .Replace("password=", "password=*****")
                .Replace("pwd=", "pwd=*****")
                .Replace("Pwd=", "Pwd=*****");
        }
    }

    // Schema classes to match the existing structure
    public class DatabaseSchema
    {
        public List<TableInfo> Tables { get; set; } = new List<TableInfo>();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
        public List<RelationshipInfo> Relationships { get; set; } = new List<RelationshipInfo>();
    }

    public class TableInfo
    {
        public string Schema { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
    }

    public class ColumnInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public bool IsPrimaryKey { get; set; }
    }

    public class ForeignKeyInfo
    {
        public string Name { get; set; } = string.Empty;
        public string ForeignKeyTable { get; set; } = string.Empty;
        public string ForeignKeyColumn { get; set; } = string.Empty;
        public string PrimaryKeyTable { get; set; } = string.Empty;
        public string PrimaryKeyColumn { get; set; } = string.Empty;
    }

    public class RelationshipInfo
    {
        public string ForeignKeyTable { get; set; } = string.Empty;
        public string ForeignKeyColumn { get; set; } = string.Empty;
        public string PrimaryKeyTable { get; set; } = string.Empty;
        public string PrimaryKeyColumn { get; set; } = string.Empty;
    }
}
