using System;
using System.Data;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Extensions;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Providers;

/// <summary>
/// Provides schema information from a SQL Server database.
/// </summary>
public class SqlServerSchemaProvider : DatabaseSchemaProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerSchemaProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public SqlServerSchemaProvider(ILogger logger) : base(logger)
    {
    }

    /// <inheritdoc/>
    public override string ProviderType => "SqlServer";

    /// <summary>
    /// Gets or sets the SQL Server connection options.
    /// </summary>
    protected SqlServerConnectionOptions ConnectionOptions { get; set; } = new SqlServerConnectionOptions();

    /// <inheritdoc/>
    public override void Configure(IDictionary<string, string> settings)
    {
        base.Configure(settings);

        // Extract SQL Server-specific settings
        if (settings.TryGetValue("CommandTimeout", out var commandTimeoutStr) && 
            int.TryParse(commandTimeoutStr, out var commandTimeout))
        {
            ConnectionOptions.CommandTimeout = commandTimeout;
        }

        if (settings.TryGetValue("ApplicationName", out var applicationName))
        {
            ConnectionOptions.ApplicationName = applicationName;
        }

        if (settings.TryGetValue("EnableMultipleActiveResultSets", out var marsStr) && 
            bool.TryParse(marsStr, out var mars))
        {
            ConnectionOptions.EnableMultipleActiveResultSets = mars;
        }

        Logger.LogDebug("SqlServerSchemaProvider configured with connection options: CommandTimeout={CommandTimeout}, ApplicationName={ApplicationName}, MARS={MARS}", 
            ConnectionOptions.CommandTimeout, 
            ConnectionOptions.ApplicationName, 
            ConnectionOptions.EnableMultipleActiveResultSets);
    }

    /// <inheritdoc/>
    public override async Task<IDatabaseSchema> GetSchemaAsync(string connectionString, SchemaProviderOptions options)
    {
        ValidateConnectionString(connectionString);
        options = ValidateOptions(options);
        LogOptions(options);

        Logger.LogInformation("Extracting schema from SQL Server database...");
        
        try
        {
            // Extract database name from connection string or use default
            string databaseName = ExtractDatabaseName(connectionString);
            
            // Create the schema
            var schema = new DatabaseSchema(databaseName);
            
            // Use connection to extract schema information
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                
                // Extract database objects based on the options
                if (options.IncludeTables == null || options.IncludeTables.Count > 0)
                {
                    await ExtractTablesAsync(connection, schema, options).ConfigureAwait(false);
                }
                
                if (options.IncludeViews)
                {
                    await ExtractViewsAsync(connection, schema, options).ConfigureAwait(false);
                }
                
                if (options.IncludeStoredProcedures)
                {
                    await ExtractStoredProceduresAsync(connection, schema, options).ConfigureAwait(false);
                }
                
                if (options.IncludeFunctions)
                {
                    await ExtractFunctionsAsync(connection, schema, options).ConfigureAwait(false);
                }
                
                // Extract relationships if tables are included
                if (options.IncludeTables == null || options.IncludeTables.Count > 0)
                {
                    if (options.IncludeForeignKeys)
                    {
                        await ExtractRelationshipsAsync(connection, schema, options).ConfigureAwait(false);
                    }
                }
                
                // Extract special data types and SQL Server-specific features
                await SqlServerSchemaProviderExtensions.ExtractSpecialDataTypesAsync(connection, (DatabaseSchema)schema, Logger).ConfigureAwait(false);
                await SqlServerSchemaProviderExtensions.ExtractJsonColumnsAsync(connection, (DatabaseSchema)schema, Logger).ConfigureAwait(false);
                await SqlServerSchemaProviderExtensions.ExtractTemporalTablesAsync(connection, (DatabaseSchema)schema, Logger).ConfigureAwait(false);
            }
            
            Logger.LogInformation("Schema extraction completed successfully.");
            return schema;
        }
        catch (Exception ex)
        {
            Logger.LogError("Error extracting schema from SQL Server database: {Message}", ex.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public override async Task<bool> TestConnectionAsync(string connectionString)
    {
        ValidateConnectionString(connectionString);
        
        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError("Error testing connection to SQL Server database: {Message}", ex.Message);
            return false;
        }
    }

    private string ExtractDatabaseName(string connectionString)
    {
        // Try to extract the database name from the connection string
        var builder = new SqlConnectionStringBuilder(connectionString);
        
        if (!string.IsNullOrEmpty(builder.InitialCatalog))
        {
            return builder.InitialCatalog;
        }
        
        if (!string.IsNullOrEmpty(builder.DataSource))
        {
            return builder.DataSource;
        }
        
        // Return a default database name if we couldn't extract one
        return "Database";
    }
    
    private async Task ExtractTablesAsync(SqlConnection connection, DatabaseSchema schema, SchemaProviderOptions options)
    {
        Logger.LogInformation("Extracting tables...");

        // Query to retrieve tables
        string tableQuery = @"
            SELECT 
                t.TABLE_SCHEMA as [Schema],
                t.TABLE_NAME as [Name],
                t.TABLE_TYPE as [TableType],
                CASE WHEN temporal.object_id IS NOT NULL THEN 1 ELSE 0 END as IsTemporal,
                ISNULL(history.name, '') as HistoryTableName
            FROM 
                INFORMATION_SCHEMA.TABLES t
                LEFT JOIN sys.tables temporal ON temporal.name = t.TABLE_NAME 
                    AND SCHEMA_NAME(temporal.schema_id) = t.TABLE_SCHEMA
                    AND temporal.temporal_type = 2 -- SYSTEM_VERSIONED_TEMPORAL_TABLE
                LEFT JOIN sys.tables history ON history.object_id = temporal.history_table_id
            WHERE 
                t.TABLE_TYPE = 'BASE TABLE'";

        // Add schema filter if specified
        if (options.IncludeSchemas != null && options.IncludeSchemas.Count > 0)
        {
            var schemaList = string.Join("','", options.IncludeSchemas);
            tableQuery += $" AND t.TABLE_SCHEMA IN ('{schemaList}')";
        }

        // Exclude system schemas if specified
        if (!options.IncludeSystemObjects)
        {
            tableQuery += " AND t.TABLE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA', 'db_owner', 'db_accessadmin', " +
                          "'db_securityadmin', 'db_ddladmin', 'db_backupoperator', 'db_datareader', 'db_datawriter', 'db_denydatareader', 'db_denydatawriter')";
        }

        using (var command = new SqlCommand(tableQuery, connection))
        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? tableName = reader["Name"]?.ToString();
                string? tableSchema = reader["Schema"]?.ToString();
                string? tableType = reader["TableType"]?.ToString();
                bool isTemporal = Convert.ToBoolean(reader["IsTemporal"]);
                string? historyTableName = reader["HistoryTableName"]?.ToString();

                if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(tableSchema))
                {
                    Logger.LogWarning("Skipping table with null or empty name or schema");
                    continue;
                }

                var table = new Table(tableName, tableSchema)
                {
                    IsTemporal = isTemporal,
                    HistoryTableName = isTemporal && !string.IsNullOrEmpty(historyTableName) ? historyTableName : null
                };
                schema.AddTable(table);
                Logger.LogDebug("Added table {Schema}.{Table}", tableSchema, tableName);
            }
        }

        // Now query for columns for each table
        foreach (var table in schema.Tables)
        {
            await ExtractColumnsAsync(connection, (Table)table, options).ConfigureAwait(false);
            await ExtractPrimaryKeyAsync(connection, (Table)table, options).ConfigureAwait(false);
            await ExtractIndexesAsync(connection, (Table)table, options).ConfigureAwait(false);
            await ExtractUniqueConstraintsAsync(connection, (Table)table, options).ConfigureAwait(false);
        }
    }

    private async Task ExtractColumnsAsync(SqlConnection connection, Table table, SchemaProviderOptions options)
    {
        Logger.LogDebug("Extracting columns for table {Schema}.{Table}", table.Schema, table.Name);

        string columnQuery = @"
            SELECT 
                c.COLUMN_NAME as [Name],
                c.ORDINAL_POSITION as OrdinalPosition,
                c.DATA_TYPE as DataType,
                CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END as IsNullable,
                CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN NULL ELSE CHARACTER_MAXIMUM_LENGTH END as MaxLength,
                c.NUMERIC_PRECISION as Precision,
                c.NUMERIC_SCALE as Scale,
                CASE WHEN COLUMNPROPERTY(OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME)), c.COLUMN_NAME, 'IsIdentity') = 1 THEN 1 ELSE 0 END as IsIdentity,
                CASE WHEN COLUMNPROPERTY(OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME)), c.COLUMN_NAME, 'IsComputed') = 1 THEN 1 ELSE 0 END as IsComputed,
                c.COLUMN_DEFAULT as DefaultValue
            FROM 
                INFORMATION_SCHEMA.COLUMNS c
            WHERE 
                c.TABLE_SCHEMA = @Schema AND c.TABLE_NAME = @TableName
            ORDER BY 
                c.ORDINAL_POSITION";

        using (var command = new SqlCommand(columnQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", table.Schema);
            command.Parameters.AddWithValue("@TableName", table.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? columnName = reader["Name"]?.ToString();
                    int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                    string? dataType = reader["DataType"]?.ToString();
                    bool isNullable = Convert.ToBoolean(reader["IsNullable"]);
                    bool isIdentity = Convert.ToBoolean(reader["IsIdentity"]);
                    bool isComputed = Convert.ToBoolean(reader["IsComputed"]);

                    // Skip if name or dataType is null
                    if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(dataType))
                    {
                        Logger.LogWarning($"Skipping column with missing name or data type at position {ordinalPosition}");
                        continue;
                    }

                    var column = new Column(columnName, dataType, table, ordinalPosition, isNullable)
                    {
                        MaxLength = reader["MaxLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxLength"]) : null,
                        Precision = reader["Precision"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Precision"]) : null,
                        Scale = reader["Scale"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Scale"]) : null,
                        IsIdentity = isIdentity,
                        IsComputed = isComputed,
                        DefaultValue = reader["DefaultValue"] != DBNull.Value ? reader["DefaultValue"].ToString() : null
                    };

                    table.AddColumn(column);
                    Logger.LogDebug("Added column {Column} to table {Schema}.{Table}", columnName, table.Schema, table.Name);
                }
            }
        }
    }

    private async Task ExtractPrimaryKeyAsync(SqlConnection connection, Table table, SchemaProviderOptions options)
    {
        Logger.LogDebug("Extracting primary key for table {Schema}.{Table}", table.Schema, table.Name);

        string pkQuery = @"
            SELECT 
                kcu.CONSTRAINT_NAME as [Name],
                kcu.COLUMN_NAME as ColumnName,
                CASE WHEN i.type_desc = 'CLUSTERED' THEN 1 ELSE 0 END as IsClustered
            FROM 
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA AND tc.TABLE_NAME = kcu.TABLE_NAME
                LEFT JOIN sys.indexes i ON i.name = tc.CONSTRAINT_NAME AND i.object_id = OBJECT_ID(QUOTENAME(tc.TABLE_SCHEMA) + '.' + QUOTENAME(tc.TABLE_NAME))
            WHERE 
                tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND tc.TABLE_SCHEMA = @Schema AND tc.TABLE_NAME = @TableName
            ORDER BY 
                kcu.ORDINAL_POSITION";

        using (var command = new SqlCommand(pkQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", table.Schema);
            command.Parameters.AddWithValue("@TableName", table.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                Key primaryKey = null;
                string? constraintName = null;
                bool isClustered = false;
                List<IColumn> keyColumns = new List<IColumn>();

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    constraintName = reader["Name"]?.ToString();
                    isClustered = Convert.ToBoolean(reader["IsClustered"]);
                    string? columnName = reader["ColumnName"]?.ToString();

                    if (string.IsNullOrEmpty(columnName))
                    {
                        Logger.LogWarning("Skipping primary key column with null or empty name");
                        continue;
                    }

                    var column = table.GetColumn(columnName);
                    if (column != null)
                    {
                        keyColumns.Add(column);
                    }
                }

                if (constraintName != null && keyColumns.Count > 0)
                {
                    primaryKey = new Key(constraintName, table, keyColumns, isClustered);
                    table.SetPrimaryKey(primaryKey);
                    Logger.LogDebug("Added primary key {Name} to table {Schema}.{Table}", constraintName, table.Schema, table.Name);
                }
            }
        }
    }

    private async Task ExtractIndexesAsync(SqlConnection connection, Table table, SchemaProviderOptions options)
    {
        if (!options.IncludeIndexes)
        {
            return;
        }

        Logger.LogDebug("Extracting indexes for table {Schema}.{Table}", table.Schema, table.Name);

        string indexQuery = @"
            SELECT 
                i.name as IndexName,
                c.name as ColumnName,
                ic.key_ordinal as OrdinalPosition,
                ic.is_descending_key as IsDescending,
                i.is_unique as IsUnique,
                i.type_desc as TypeDesc,
                i.filter_definition as FilterDefinition
            FROM 
                sys.indexes i
                JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                JOIN sys.tables t ON i.object_id = t.object_id
            WHERE 
                t.name = @TableName AND SCHEMA_NAME(t.schema_id) = @Schema
                AND i.is_primary_key = 0  -- Exclude primary keys as they are already handled
                AND i.is_unique_constraint = 0  -- Exclude unique constraints as they are handled separately
            ORDER BY 
                i.name, ic.key_ordinal";

        using (var command = new SqlCommand(indexQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", table.Schema);
            command.Parameters.AddWithValue("@TableName", table.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                Dictionary<string, EzDbCodeGen.Schema.Models.Index> indexes = new Dictionary<string, EzDbCodeGen.Schema.Models.Index>();

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? indexName = reader["IndexName"]?.ToString();
                    string? columnName = reader["ColumnName"]?.ToString();
                    int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                    bool isDescending = Convert.ToBoolean(reader["IsDescending"]);
                    bool isUnique = Convert.ToBoolean(reader["IsUnique"]);
                    string? typeDesc = reader["TypeDesc"]?.ToString();
                    bool isClustered = typeDesc == "CLUSTERED";
                    string? filterDefinition = reader["FilterDefinition"] != DBNull.Value ? reader["FilterDefinition"]?.ToString() : null;

                    if (string.IsNullOrEmpty(indexName) || string.IsNullOrEmpty(columnName))
                    {
                        Logger.LogWarning("Skipping index with null or empty name or column");
                        continue;
                    }

                    var column = table.GetColumn(columnName);
                    if (column == null)
                    {
                        continue;
                    }

                    if (!indexes.TryGetValue(indexName, out var index))
                    {
                        index = new EzDbCodeGen.Schema.Models.Index(indexName, table, isUnique, isClustered, filterDefinition);
                        indexes[indexName] = index;
                    }

                    var indexColumn = new EzDbCodeGen.Schema.Models.IndexColumn(column, ordinalPosition, isDescending);
                    index.AddColumn(indexColumn);
                }

                foreach (var index in indexes.Values)
                {
                    table.AddIndex(index);
                    Logger.LogDebug("Added index {Name} to table {Schema}.{Table}", index.Name, table.Schema, table.Name);
                }
            }
        }
    }

    private async Task ExtractUniqueConstraintsAsync(SqlConnection connection, Table table, SchemaProviderOptions options)
    {
        Logger.LogDebug("Extracting unique constraints for table {Schema}.{Table}", table.Schema, table.Name);

        string uniqueQuery = @"
            SELECT 
                tc.CONSTRAINT_NAME as ConstraintName,
                kcu.COLUMN_NAME as ColumnName,
                CASE WHEN i.type_desc = 'CLUSTERED' THEN 1 ELSE 0 END as IsClustered
            FROM 
                INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA AND tc.TABLE_NAME = kcu.TABLE_NAME
                LEFT JOIN sys.indexes i ON i.name = tc.CONSTRAINT_NAME AND i.object_id = OBJECT_ID(QUOTENAME(tc.TABLE_SCHEMA) + '.' + QUOTENAME(tc.TABLE_NAME))
            WHERE 
                tc.CONSTRAINT_TYPE = 'UNIQUE' AND tc.TABLE_SCHEMA = @Schema AND tc.TABLE_NAME = @TableName
            ORDER BY 
                tc.CONSTRAINT_NAME, kcu.ORDINAL_POSITION";

        using (var command = new SqlCommand(uniqueQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", table.Schema);
            command.Parameters.AddWithValue("@TableName", table.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                Dictionary<string, List<IColumn>> uniqueConstraints = new Dictionary<string, List<IColumn>>();
                Dictionary<string, bool> isClustered = new Dictionary<string, bool>();

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? constraintName = reader["ConstraintName"]?.ToString();
                    string? columnName = reader["ColumnName"]?.ToString();
                    bool clustered = Convert.ToBoolean(reader["IsClustered"]);

                    if (string.IsNullOrEmpty(constraintName) || string.IsNullOrEmpty(columnName))
                    {
                        Logger.LogWarning("Skipping unique constraint with null or empty name or column");
                        continue;
                    }

                    var column = table.GetColumn(columnName);
                    if (column == null)
                    {
                        continue;
                    }

                    if (!uniqueConstraints.TryGetValue(constraintName, out var columns))
                    {
                        columns = new List<IColumn>();
                        uniqueConstraints[constraintName] = columns;
                        isClustered[constraintName] = clustered;
                    }

                    columns.Add(column);
                }

                foreach (var kv in uniqueConstraints)
                {
                    var uniqueConstraint = new EzDbCodeGen.Schema.Models.UniqueConstraint(kv.Key, table, kv.Value, isClustered[kv.Key]);
                    table.AddUniqueConstraint(uniqueConstraint);
                    Logger.LogDebug("Added unique constraint {Name} to table {Schema}.{Table}", kv.Key, table.Schema, table.Name);
                }
            }
        }
    }
    
    private async Task ExtractViewsAsync(SqlConnection connection, DatabaseSchema schema, SchemaProviderOptions options)
    {
        if (!options.IncludeViews)
        {
            return;
        }

        Logger.LogInformation("Extracting views...");

        // Query to retrieve views
        string viewQuery = @"
            SELECT 
                v.TABLE_SCHEMA as [Schema],
                v.TABLE_NAME as [Name],
                CAST(CASE WHEN i.name IS NOT NULL THEN 1 ELSE 0 END AS bit) as IsIndexed,
                OBJECT_DEFINITION(OBJECT_ID(QUOTENAME(v.TABLE_SCHEMA) + '.' + QUOTENAME(v.TABLE_NAME))) as [Definition]
            FROM 
                INFORMATION_SCHEMA.VIEWS v
                LEFT JOIN sys.indexes i ON i.object_id = OBJECT_ID(QUOTENAME(v.TABLE_SCHEMA) + '.' + QUOTENAME(v.TABLE_NAME)) AND i.index_id > 0
            WHERE 
                1=1";

        // Add schema filter if specified
        if (options.IncludeSchemas != null && options.IncludeSchemas.Count > 0)
        {
            var schemaList = string.Join("','", options.IncludeSchemas);
            viewQuery += $" AND v.TABLE_SCHEMA IN ('{schemaList}')";
        }

        // Exclude system schemas if specified
        if (!options.IncludeSystemObjects)
        {
            viewQuery += " AND v.TABLE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA', 'db_owner', 'db_accessadmin', " +
                         "'db_securityadmin', 'db_ddladmin', 'db_backupoperator', 'db_datareader', 'db_datawriter', 'db_denydatareader', 'db_denydatawriter')";
        }

        using (var command = new SqlCommand(viewQuery, connection))
        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? viewName = reader["Name"]?.ToString();
                string? viewSchema = reader["Schema"]?.ToString();
                bool isIndexed = Convert.ToBoolean(reader["IsIndexed"]);
                string? definition = reader["Definition"] != DBNull.Value ? reader["Definition"]?.ToString() : null;

                if (string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(viewSchema))
                {
                    Logger.LogWarning("Skipping view with null or empty name or schema");
                    continue;
                }

                var view = new View(viewName, viewSchema, definition, isIndexed);
                schema.AddView(view);
                Logger.LogDebug("Added view {Schema}.{View}", viewSchema, viewName);
            }
        }

        // Now extract columns for each view
        foreach (var view in schema.Views)
        {
            await ExtractViewColumnsAsync(connection, (View)view, options).ConfigureAwait(false);
        }
    }

    private async Task ExtractViewColumnsAsync(SqlConnection connection, View view, SchemaProviderOptions options)
    {
        Logger.LogDebug("Extracting columns for view {Schema}.{View}", view.Schema, view.Name);

        string columnQuery = @"
            SELECT 
                c.COLUMN_NAME as [Name],
                c.ORDINAL_POSITION as OrdinalPosition,
                c.DATA_TYPE as DataType,
                CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END as IsNullable,
                CASE WHEN CHARACTER_MAXIMUM_LENGTH = -1 THEN NULL ELSE CHARACTER_MAXIMUM_LENGTH END as MaxLength,
                c.NUMERIC_PRECISION as Precision,
                c.NUMERIC_SCALE as Scale
            FROM 
                INFORMATION_SCHEMA.COLUMNS c
            WHERE 
                c.TABLE_SCHEMA = @Schema AND c.TABLE_NAME = @ViewName
            ORDER BY 
                c.ORDINAL_POSITION";

        using (var command = new SqlCommand(columnQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", view.Schema);
            command.Parameters.AddWithValue("@ViewName", view.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? columnName = reader["Name"]?.ToString();
                    int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                    string? dataType = reader["DataType"]?.ToString();
                    bool isNullable = Convert.ToBoolean(reader["IsNullable"]);

                    // Skip if name or dataType is null
                    if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(dataType))
                    {
                        Logger.LogWarning($"Skipping column with missing name or data type at position {ordinalPosition}");
                        continue;
                    }

                    var column = new ViewColumn(columnName, dataType, view, ordinalPosition, isNullable)
                    {
                        MaxLength = reader["MaxLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxLength"]) : null,
                        Precision = reader["Precision"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Precision"]) : null,
                        Scale = reader["Scale"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Scale"]) : null
                    };

                    view.AddColumn(column);
                    Logger.LogDebug("Added column {Column} to view {Schema}.{View}", columnName, view.Schema, view.Name);
                }
            }
        }
    }

    private async Task ExtractStoredProceduresAsync(SqlConnection connection, DatabaseSchema schema, SchemaProviderOptions options)
    {
        if (!options.IncludeStoredProcedures)
        {
            return;
        }

        Logger.LogInformation("Extracting stored procedures...");

        // Query to retrieve stored procedures
        string spQuery = @"
            SELECT 
                ROUTINE_SCHEMA as [Schema],
                ROUTINE_NAME as [Name],
                ROUTINE_DEFINITION as [Definition]
            FROM 
                INFORMATION_SCHEMA.ROUTINES
            WHERE 
                ROUTINE_TYPE = 'PROCEDURE'";

        // Add schema filter if specified
        if (options.IncludeSchemas != null && options.IncludeSchemas.Count > 0)
        {
            var schemaList = string.Join("','", options.IncludeSchemas);
            spQuery += $" AND ROUTINE_SCHEMA IN ('{schemaList}')";
        }

        // Exclude system schemas if specified
        if (!options.IncludeSystemObjects)
        {
            spQuery += " AND ROUTINE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA', 'db_owner', 'db_accessadmin', " +
                      "'db_securityadmin', 'db_ddladmin', 'db_backupoperator', 'db_datareader', 'db_datawriter', 'db_denydatareader', 'db_denydatawriter')";
        }

        using (var command = new SqlCommand(spQuery, connection))
        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? procName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : null;
                string? procSchema = reader["Schema"] != DBNull.Value ? reader["Schema"].ToString() : "dbo";
                string? definition = reader["Definition"] != DBNull.Value ? reader["Definition"].ToString() : null;

                // Validate required parameters
                if (string.IsNullOrEmpty(procName) || string.IsNullOrEmpty(procSchema))
                {
                    Logger.LogWarning("Skipping stored procedure with missing name or schema: {Schema}.{Name}", procSchema, procName ?? "Unknown");
                    continue;
                }

                // Determine if proc returns a result set (simplified approach - this is a complex topic)
                bool hasResultSet = definition != null && (definition.Contains("SELECT ") || definition.Contains("select "));

                var procedure = new StoredProcedure(procName, procSchema ?? "dbo", definition, hasResultSet);
                schema.AddStoredProcedure(procedure);
                Logger.LogDebug("Added stored procedure {Schema}.{Procedure}", procSchema, procName);
            }
        }

        // Extract parameters and result sets for each stored procedure
        foreach (var proc in schema.StoredProcedures)
        {
            await ExtractStoredProcedureParametersAsync(connection, (StoredProcedure)proc).ConfigureAwait(false);
            if (((StoredProcedure)proc).HasResultSet)
            {
                await ExtractStoredProcedureResultSetAsync(connection, (StoredProcedure)proc).ConfigureAwait(false);
            }
        }
    }

    private async Task ExtractStoredProcedureParametersAsync(SqlConnection connection, StoredProcedure procedure)
    {
        Logger.LogDebug("Extracting parameters for stored procedure {Schema}.{Procedure}", procedure.Schema, procedure.Name);

        string paramQuery = @"
            SELECT 
                p.name as [Name],
                p.parameter_id as OrdinalPosition,
                t.name as DataType,
                p.is_nullable as IsNullable,
                p.max_length as MaxLength,
                p.[precision] as Precision,
                p.scale as Scale,
                p.is_output as IsOutput,
                p.default_value as DefaultValue
            FROM 
                sys.procedures sp
                JOIN sys.parameters p ON sp.object_id = p.object_id
                JOIN sys.types t ON p.system_type_id = t.system_type_id AND p.user_type_id = t.user_type_id
            WHERE 
                sp.name = @ProcName AND SCHEMA_NAME(sp.schema_id) = @Schema
                AND p.parameter_id > 0 -- Filter out return value
            ORDER BY 
                p.parameter_id";

        using (var command = new SqlCommand(paramQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", procedure.Schema);
            command.Parameters.AddWithValue("@ProcName", procedure.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? paramName = reader["Name"]?.ToString();
                    int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                    string? dataType = reader["DataType"]?.ToString();
                    bool isNullable = Convert.ToBoolean(reader["IsNullable"]);
                    bool isOutput = Convert.ToBoolean(reader["IsOutput"]);

                    // Skip if name or dataType is null
                    if (string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(dataType))
                    {
                        Logger.LogWarning($"Skipping parameter with missing name or data type at position {ordinalPosition}");
                        continue;
                    }

                    // Remove the @ prefix if present
                    if (paramName.StartsWith("@"))
                    {
                        paramName = paramName.Substring(1);
                    }

                    var parameter = new Parameter(paramName, dataType, ordinalPosition, isOutput, isNullable)
                    {
                        MaxLength = reader["MaxLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxLength"]) : null,
                        Precision = reader["Precision"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Precision"]) : null,
                        Scale = reader["Scale"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Scale"]) : null,
                        DefaultValue = reader["DefaultValue"] != DBNull.Value ? reader["DefaultValue"].ToString() : null
                    };

                    procedure.AddParameter(parameter);
                    Logger.LogDebug("Added parameter {Parameter} to stored procedure {Schema}.{Procedure}", 
                        paramName, procedure.Schema, procedure.Name);
                }
            }
        }
    }

    private Task ExtractStoredProcedureResultSetAsync(SqlConnection connection, StoredProcedure procedure)
    {
        // This is a complex task that often requires executing the stored procedure with dummy parameters
        // or parsing the stored procedure definition to infer the result set structure
        // For simplicity, we'll log that this is not fully implemented yet
        
        Logger.LogInformation("Result set extraction for stored procedure {Schema}.{Procedure} is not fully implemented",
            procedure.Schema, procedure.Name);
        
        return Task.CompletedTask;
    }

    private async Task ExtractFunctionsAsync(SqlConnection connection, DatabaseSchema schema, SchemaProviderOptions options)
    {
        if (!options.IncludeFunctions)
        {
            return;
        }

        Logger.LogInformation("Extracting functions...");

        // Query to retrieve functions
        string funcQuery = @"
            SELECT 
                ROUTINE_SCHEMA as [Schema],
                ROUTINE_NAME as [Name],
                DATA_TYPE as ReturnType,
                CASE WHEN DATA_TYPE = 'TABLE' THEN 1 ELSE 0 END as IsTableValued,
                ROUTINE_DEFINITION as [Definition]
            FROM 
                INFORMATION_SCHEMA.ROUTINES
            WHERE 
                ROUTINE_TYPE = 'FUNCTION'";

        // Add schema filter if specified
        if (options.IncludeSchemas != null && options.IncludeSchemas.Count > 0)
        {
            var schemaList = string.Join("','", options.IncludeSchemas);
            funcQuery += $" AND ROUTINE_SCHEMA IN ('{schemaList}')";
        }

        // Exclude system schemas if specified
        if (!options.IncludeSystemObjects)
        {
            funcQuery += " AND ROUTINE_SCHEMA NOT IN ('sys', 'INFORMATION_SCHEMA', 'db_owner', 'db_accessadmin', " +
                         "'db_securityadmin', 'db_ddladmin', 'db_backupoperator', 'db_datareader', 'db_datawriter', 'db_denydatareader', 'db_denydatawriter')";
        }

        using (var command = new SqlCommand(funcQuery, connection))
        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? funcName = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : null;
                string? funcSchema = reader["Schema"] != DBNull.Value ? reader["Schema"].ToString() : "dbo";
                string? returnType = reader["ReturnType"] != DBNull.Value ? reader["ReturnType"].ToString() : "unknown";
                bool isTableValued = reader["IsTableValued"] != DBNull.Value && Convert.ToBoolean(reader["IsTableValued"]);
                string? definition = reader["Definition"] != DBNull.Value ? reader["Definition"].ToString() : null;

                // Validate required parameters
                if (string.IsNullOrEmpty(funcName) || string.IsNullOrEmpty(returnType))
                {
                    Logger.LogWarning("Skipping function with missing name or return type: {Schema}.{Name}", funcSchema, funcName ?? "Unknown");
                    continue;
                }

                var function = new Function(funcName, returnType, funcSchema ?? "dbo", isTableValued, definition);
                schema.AddFunction(function);
                Logger.LogDebug("Added function {Schema}.{Function}", funcSchema, funcName);
            }
        }

        // Extract parameters for each function
        foreach (var func in schema.Functions)
        {
            await ExtractFunctionParametersAsync(connection, (Function)func).ConfigureAwait(false);
            
            // For table-valued functions, extract the result columns
            if (((Function)func).IsTableValued)
            {
                await ExtractTableValuedFunctionColumnsAsync(connection, (Function)func).ConfigureAwait(false);
            }
        }
    }

    private async Task ExtractFunctionParametersAsync(SqlConnection connection, Function function)
    {
        Logger.LogDebug("Extracting parameters for function {Schema}.{Function}", function.Schema, function.Name);

        string paramQuery = @"
            SELECT 
                p.name as [Name],
                p.parameter_id as OrdinalPosition,
                t.name as DataType,
                p.is_nullable as IsNullable,
                p.max_length as MaxLength,
                p.[precision] as Precision,
                p.scale as Scale,
                p.default_value as DefaultValue
            FROM 
                sys.objects o
                JOIN sys.parameters p ON o.object_id = p.object_id
                JOIN sys.types t ON p.system_type_id = t.system_type_id AND p.user_type_id = t.user_type_id
            WHERE 
                o.name = @FuncName AND SCHEMA_NAME(o.schema_id) = @Schema
                AND p.parameter_id > 0 -- Filter out return value
            ORDER BY 
                p.parameter_id";

        using (var command = new SqlCommand(paramQuery, connection))
        {
            command.Parameters.AddWithValue("@Schema", function.Schema);
            command.Parameters.AddWithValue("@FuncName", function.Name);

            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    string? paramName = reader["Name"]?.ToString();
                    int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                    string? dataType = reader["DataType"]?.ToString();
                    bool isNullable = Convert.ToBoolean(reader["IsNullable"]);

                    // Skip if name or dataType is null
                    if (string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(dataType))
                    {
                        Logger.LogWarning($"Skipping parameter with missing name or data type at position {ordinalPosition}");
                        continue;
                    }

                    // Remove the @ prefix if present
                    if (paramName.StartsWith("@"))
                    {
                        paramName = paramName.Substring(1);
                    }

                    var parameter = new Parameter(paramName, dataType, ordinalPosition, false, isNullable)
                    {
                        MaxLength = reader["MaxLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxLength"]) : null,
                        Precision = reader["Precision"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Precision"]) : null,
                        Scale = reader["Scale"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Scale"]) : null,
                        DefaultValue = reader["DefaultValue"] != DBNull.Value ? reader["DefaultValue"].ToString() : null
                    };

                    function.AddParameter(parameter);
                    Logger.LogDebug("Added parameter {Parameter} to function {Schema}.{Function}", 
                        paramName, function.Schema, function.Name);
                }
            }
        }
    }

    private async Task ExtractTableValuedFunctionColumnsAsync(SqlConnection connection, Function function)
    {
        Logger.LogDebug("Extracting result columns for table-valued function {Schema}.{Function}", 
            function.Schema, function.Name);

        // This query attempts to extract the column information from a table-valued function
        // It requires executing the function with dummy parameters to get column metadata
        // For simplicity, we'll use a query against the system metadata if possible

        string columnQuery = @"
            SELECT 
                c.name as [Name],
                c.column_id as OrdinalPosition,
                t.name as DataType,
                c.is_nullable as IsNullable,
                c.max_length as MaxLength,
                c.[precision] as Precision,
                c.scale as Scale
            FROM 
                sys.columns c
                JOIN sys.types t ON c.system_type_id = t.system_type_id AND c.user_type_id = t.user_type_id
                JOIN sys.objects o ON c.object_id = o.object_id
            WHERE 
                o.name = @FuncName AND SCHEMA_NAME(o.schema_id) = @Schema
                AND o.type IN ('TF', 'IF', 'FT')
            ORDER BY 
                c.column_id";

        try
        {
            using (var command = new SqlCommand(columnQuery, connection))
            {
                command.Parameters.AddWithValue("@Schema", function.Schema);
                command.Parameters.AddWithValue("@FuncName", function.Name);

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        string? columnName = reader["Name"]?.ToString();
                        int ordinalPosition = Convert.ToInt32(reader["OrdinalPosition"]);
                        string? dataType = reader["DataType"]?.ToString();
                        bool isNullable = Convert.ToBoolean(reader["IsNullable"]);

                        // Skip if name or dataType is null
                        if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(dataType))
                        {
                            Logger.LogWarning($"Skipping result column with missing name or data type at position {ordinalPosition}");
                            continue;
                        }

                        var resultColumn = new ResultColumn(columnName, dataType, ordinalPosition, isNullable)
                        {
                            MaxLength = reader["MaxLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["MaxLength"]) : null,
                            Precision = reader["Precision"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Precision"]) : null,
                            Scale = reader["Scale"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Scale"]) : null
                        };

                        function.CreateResultColumn(columnName, dataType, isNullable);
                        Logger.LogDebug("Added result column {Column} to function {Schema}.{Function}", 
                            columnName, function.Schema, function.Name);
                    }
                }
            }
        }
        catch
        {
            // If the above approach fails, log the error but don't fail the entire schema extraction
            Logger.LogWarning("Failed to extract result columns for table-valued function {Schema}.{Function}. This may require directly executing the function to determine its schema.",
                function.Schema, function.Name);
        }
    }

    private async Task ExtractRelationshipsAsync(SqlConnection connection, DatabaseSchema schema, SchemaProviderOptions options)
    {
        if (!options.IncludeForeignKeys)
        {
            return;
        }

        Logger.LogInformation("Extracting relationships between tables...");

        string fkQuery = @"
            SELECT 
                fk.name as [Name],
                SCHEMA_NAME(pt.schema_id) as PrimarySchema,
                pt.name as PrimaryTable,
                pc.name as PrimaryColumn,
                SCHEMA_NAME(ft.schema_id) as ForeignSchema,
                ft.name as ForeignTable,
                fc.name as ForeignColumn,
                CASE WHEN fk.delete_referential_action = 1 THEN 'CASCADE'
                     WHEN fk.delete_referential_action = 2 THEN 'SET NULL'
                     WHEN fk.delete_referential_action = 3 THEN 'SET DEFAULT'
                     ELSE 'NO ACTION' END as DeleteAction,
                CASE WHEN fk.update_referential_action = 1 THEN 'CASCADE'
                     WHEN fk.update_referential_action = 2 THEN 'SET NULL'
                     WHEN fk.update_referential_action = 3 THEN 'SET DEFAULT'
                     ELSE 'NO ACTION' END as UpdateAction,
                fkc.constraint_column_id as OrdinalPosition
            FROM 
                sys.foreign_keys fk
                JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
                JOIN sys.tables ft ON fk.parent_object_id = ft.object_id
                JOIN sys.columns fc ON fkc.parent_object_id = fc.object_id AND fkc.parent_column_id = fc.column_id
                JOIN sys.tables pt ON fk.referenced_object_id = pt.object_id
                JOIN sys.columns pc ON fkc.referenced_object_id = pc.object_id AND fkc.referenced_column_id = pc.column_id
            ORDER BY 
                fk.name, fkc.constraint_column_id";

        using (var command = new SqlCommand(fkQuery, connection))
        using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
        {
            Dictionary<string, ForeignKey> foreignKeys = new Dictionary<string, ForeignKey>();
            
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                string? fkName = reader["Name"]?.ToString() ?? string.Empty;
                string? primarySchema = reader["PrimarySchema"]?.ToString() ?? string.Empty;
                string? primaryTable = reader["PrimaryTable"]?.ToString() ?? string.Empty;
                string? primaryColumn = reader["PrimaryColumn"]?.ToString() ?? string.Empty;
                string? foreignSchema = reader["ForeignSchema"]?.ToString() ?? string.Empty;
                string? foreignTable = reader["ForeignTable"]?.ToString() ?? string.Empty;
                string? foreignColumn = reader["ForeignColumn"]?.ToString() ?? string.Empty;
                string? deleteAction = reader["DeleteAction"]?.ToString() ?? string.Empty;
                string? updateAction = reader["UpdateAction"]?.ToString() ?? string.Empty;
                
                // Find the tables and columns
                var pkTable = schema.GetTable(primarySchema, primaryTable);
                var fkTable = schema.GetTable(foreignSchema, foreignTable);
                
                if (pkTable == null || fkTable == null)
                {
                    Logger.LogWarning("Could not find tables for foreign key {ForeignKey}", fkName);
                    continue;
                }
                
                var pkColumn = pkTable.GetColumn(primaryColumn);
                var fkColumn = fkTable.GetColumn(foreignColumn);
                
                if (pkColumn == null || fkColumn == null)
                {
                    Logger.LogWarning("Could not find columns for foreign key {ForeignKey}", fkName);
                    continue;
                }
                
                // Parse the referential actions
                ReferentialAction deleteRefAction = ParseReferentialAction(deleteAction);
                ReferentialAction updateRefAction = ParseReferentialAction(updateAction);
                
                // Create or update the foreign key
                if (!foreignKeys.TryGetValue(fkName, out var foreignKey))
                {
                    foreignKey = new ForeignKey(fkName, fkTable, pkTable, deleteRefAction, updateRefAction);
                    foreignKeys[fkName] = foreignKey;
                }
                
                // Add the column pair
                foreignKey.AddColumnPair(fkColumn, pkColumn);
            }
            
            // Add all foreign keys to their respective tables
            foreach (var foreignKey in foreignKeys.Values)
            {
                // Check if we have a complete foreign key
                if (foreignKey.Columns.Count > 0 && foreignKey.ReferencedColumns.Count > 0)
                {
                    ((Table)foreignKey.Table).AddForeignKey(foreignKey);
                    Logger.LogDebug("Added foreign key {ForeignKey} to table {Schema}.{Table}", 
                        foreignKey.Name, foreignKey.Table.Schema, foreignKey.Table.Name);
                }
            }
        }
    }
    
    private ReferentialAction ParseReferentialAction(string? action)
    {
        if (string.IsNullOrEmpty(action))
        {
            return ReferentialAction.NoAction;
        }
        
        switch (action.ToUpperInvariant())
        {
            case "CASCADE":
                return ReferentialAction.Cascade;
            case "SET NULL":
                return ReferentialAction.SetNull;
            case "SET DEFAULT":
                return ReferentialAction.SetDefault;
            case "RESTRICT":
                return ReferentialAction.Restrict;
            default:
                return ReferentialAction.NoAction;
        }
    }
}
