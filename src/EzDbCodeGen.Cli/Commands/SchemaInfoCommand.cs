using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// The schema-info command is responsible for displaying information about a database schema.
    /// </summary>
    public class SchemaInfoCommand : ICommand
    {
        private readonly IDatabaseSchemaProviderFactory _schemaProviderFactory;
        private readonly IRelationshipDetectorFactory _relationshipDetectorFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaInfoCommand"/> class.
        /// </summary>
        /// <param name="schemaProviderFactory">The schema provider factory to use.</param>
        /// <param name="relationshipDetectorFactory">The relationship detector factory to use.</param>
        /// <param name="logger">The logger to use.</param>
        public SchemaInfoCommand(
            IDatabaseSchemaProviderFactory schemaProviderFactory,
            IRelationshipDetectorFactory relationshipDetectorFactory,
            ILogger logger)
        {
            _schemaProviderFactory = schemaProviderFactory ?? throw new ArgumentNullException(nameof(schemaProviderFactory));
            _relationshipDetectorFactory = relationshipDetectorFactory ?? throw new ArgumentNullException(nameof(relationshipDetectorFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Name => "schema-info";

        /// <inheritdoc/>
        public string Description => "Display information about a database schema";

        /// <inheritdoc/>
        public string Usage => "schema-info --connection <connection-string> [--provider <provider-name>] [--table <table-name>] [--schema <schema-name>] [--format <json|text>] [--details <true|false>] [--detect-relationships <true|false>] [--object-type <tables|views|procs|functions|all>]";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            try
            {
                // Extract options
                options.TryGetValue("connection", out var connectionString);
                options.TryGetValue("provider", out var providerName);
                options.TryGetValue("table", out var tableName);
                options.TryGetValue("schema", out var schemaName);
                options.TryGetValue("format", out var format);
                options.TryGetValue("object-type", out var objectType);
                
                bool showDetails = ParseBoolOption(options, "details", false);
                bool detectRelationships = ParseBoolOption(options, "detect-relationships", true);
                
                // Set default values
                providerName ??= "SqlServer";
                format ??= "text";
                objectType ??= "all";
                
                _logger.Info($"Retrieving schema information from {providerName} database...");
                _logger.Info($"Connection: {MaskConnectionString(connectionString)}");
                
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    _logger.Info($"Filtering by table: {tableName}");
                }
                
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    _logger.Info($"Filtering by schema: {schemaName}");
                }
                
                // Create connection info dictionary
                var connectionInfo = new Dictionary<string, string>
                {
                    ["ConnectionString"] = connectionString
                };

                // Create schema provider
                var schemaProvider = _schemaProviderFactory.CreateSchemaProvider(providerName, connectionInfo);
                
                // Extract schema
                var schema = await schemaProvider.GetSchemaAsync();
                
                // Detect relationships if requested
                if (detectRelationships)
                {
                    _logger.Info("Detecting relationships...");
                    var relationshipDetector = _relationshipDetectorFactory.CreateRelationshipDetector();
                    var relationships = await relationshipDetector.DetectRelationshipsAsync(schema);
                    _logger.Info($"Detected {relationships.Count} relationships.");
                }
                
                // Filter tables
                var tables = schema.Tables;
                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    tables = tables.Where(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    tables = tables.Where(t => t.Schema.Equals(schemaName, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                // Display schema information
                if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    DisplaySchemaAsJson(tables, schema.Views, schema.StoredProcedures, schema.Functions, objectType, showDetails);
                }
                else
                {
                    DisplaySchemaAsText(tables, schema.Views, schema.StoredProcedures, schema.Functions, objectType, showDetails);
                }
                
                _logger.Info("Schema information retrieval completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving schema information: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public bool ValidateOptions(IReadOnlyDictionary<string, string> options)
        {
            var requiredOptions = GetRequiredOptions();
            
            foreach (var requiredOption in requiredOptions)
            {
                if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <inheritdoc/>
        public IList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options)
        {
            var errors = new List<string>();
            var requiredOptions = GetRequiredOptions();
            
            foreach (var requiredOption in requiredOptions)
            {
                if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                {
                    errors.Add($"Option '{requiredOption}' is required.");
                }
            }
            
            return errors;
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetOptions()
        {
            return new Dictionary<string, string>
            {
                ["connection"] = "Database connection string",
                ["provider"] = "Database provider (default: SqlServer)",
                ["table"] = "Filter by table name",
                ["schema"] = "Filter by schema name",
                ["format"] = "Output format: text or json (default: text)",
                ["details"] = "Show detailed information (default: false)",
                ["detect-relationships"] = "Detect relationships (default: true)",
                ["object-type"] = "Filter by object type: tables, views, procs, functions, or all (default: all)"
            };
        }

        /// <inheritdoc/>
        public IList<string> GetRequiredOptions()
        {
            return new List<string> { "connection" };
        }

        private void DisplaySchemaAsText(IList<ITable> tables, IList<IView> views, IList<IStoredProcedure> storedProcedures, IList<IFunction> functions, string objectType, bool showDetails)
        {
            _logger.Info("Schema information:");
            _logger.Info("==================");
            
            if (objectType.Equals("tables", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var table in tables)
                {
                    _logger.Info($"Table: {table.Schema}.{table.Name}");
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(table.Description))
                    {
                        _logger.Info($"Description: {table.Description}");
                    }
                    
                    _logger.Info("Columns:");
                    
                    foreach (var column in table.Columns)
                    {
                        var nullableStr = column.IsNullable ? "NULL" : "NOT NULL";
                        var pkStr = column.IsPrimaryKey ? " (PK)" : "";
                        var fkStr = column.IsForeignKey ? " (FK)" : "";
                        
                        _logger.Info($"  - {column.Name} ({column.DataType}) {nullableStr}{pkStr}{fkStr}");
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(column.Description))
                            {
                                _logger.Info($"    Description: {column.Description}");
                            }
                            
                            if (column.IsForeignKey)
                            {
                                var fk = table.ForeignKeys.FirstOrDefault(fk => fk.Columns.Any(c => c.Name == column.Name));
                                if (fk != null)
                                {
                                    _logger.Info($"    References: {fk.ReferencedTable.Schema}.{fk.ReferencedTable.Name}.{fk.ReferencedColumns.FirstOrDefault()?.Name}");
                                }
                            }
                        }
                    }
                    
                    if (showDetails)
                    {
                        if (table.PrimaryKey != null && table.PrimaryKey.Columns.Count > 0)
                        {
                            _logger.Info("Primary Key:");
                            _logger.Info($"  {string.Join(", ", table.PrimaryKey.Columns.Select(c => c.Name))}");
                        }
                        
                        if (table.ForeignKeys.Count > 0)
                        {
                            _logger.Info("Foreign Keys:");
                            foreach (var fk in table.ForeignKeys)
                            {
                                _logger.Info($"  {fk.Name}: {string.Join(", ", fk.Columns.Select(c => c.Name))} -> {fk.ReferencedTable.Schema}.{fk.ReferencedTable.Name}.{string.Join(", ", fk.ReferencedColumns.Select(c => c.Name))}");
                            }
                        }
                        
                        if (table.Indexes.Count > 0)
                        {
                            _logger.Info("Indexes:");
                            foreach (var idx in table.Indexes)
                            {
                                var uniqueStr = idx.IsUnique ? " (UNIQUE)" : "";
                                _logger.Info($"  {idx.Name}{uniqueStr}: {string.Join(", ", idx.Columns.Select(c => c.Name))}");
                            }
                        }
                    }
                    
                    _logger.Info("------------------");
                }
            }
            
            if (objectType.Equals("views", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var view in views)
                {
                    _logger.Info($"View: {view.Schema}.{view.Name}");
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(view.Description))
                    {
                        _logger.Info($"Description: {view.Description}");
                    }
                    
                    _logger.Info("Columns:");
                    
                    foreach (var column in view.Columns)
                    {
                        _logger.Info($"  - {column.Name} ({column.DataType})");
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(column.Description))
                            {
                                _logger.Info($"    Description: {column.Description}");
                            }
                        }
                    }
                    
                    _logger.Info("------------------");
                }
            }
            
            if (objectType.Equals("procs", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var proc in storedProcedures)
                {
                    _logger.Info($"StoredProcedure: {proc.Schema}.{proc.Name}");
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(proc.Description))
                    {
                        _logger.Info($"Description: {proc.Description}");
                    }
                    
                    _logger.Info("Parameters:");
                    
                    foreach (var param in proc.Parameters)
                    {
                        _logger.Info($"  - {param.Name} ({param.DataType})");
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(param.Description))
                            {
                                _logger.Info($"    Description: {param.Description}");
                            }
                        }
                    }
                    
                    _logger.Info("------------------");
                }
            }
            
            if (objectType.Equals("functions", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var func in functions)
                {
                    _logger.Info($"Function: {func.Schema}.{func.Name}");
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(func.Description))
                    {
                        _logger.Info($"Description: {func.Description}");
                    }
                    
                    _logger.Info("Parameters:");
                    
                    foreach (var param in func.Parameters)
                    {
                        _logger.Info($"  - {param.Name} ({param.DataType})");
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(param.Description))
                            {
                                _logger.Info($"    Description: {param.Description}");
                            }
                        }
                    }
                    
                    _logger.Info("------------------");
                }
            }
        }

        private void DisplaySchemaAsJson(IList<ITable> tables, IList<IView> views, IList<IStoredProcedure> storedProcedures, IList<IFunction> functions, string objectType, bool showDetails)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            var result = new List<object>();
            
            if (objectType.Equals("tables", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var table in tables)
                {
                    var tableInfo = new Dictionary<string, object>
                    {
                        ["name"] = table.Name,
                        ["schema"] = table.Schema
                    };
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(table.Description))
                    {
                        tableInfo["description"] = table.Description;
                    }
                    
                    var columns = new List<object>();
                    foreach (var column in table.Columns)
                    {
                        var columnInfo = new Dictionary<string, object>
                        {
                            ["name"] = column.Name,
                            ["dataType"] = column.DataType,
                            ["isNullable"] = column.IsNullable,
                            ["isPrimaryKey"] = column.IsPrimaryKey,
                            ["isForeignKey"] = column.IsForeignKey
                        };
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(column.Description))
                            {
                                columnInfo["description"] = column.Description;
                            }
                            
                            if (column.IsForeignKey)
                            {
                                var fk = table.ForeignKeys.FirstOrDefault(fk => fk.Columns.Any(c => c.Name == column.Name));
                                if (fk != null)
                                {
                                    columnInfo["references"] = new Dictionary<string, string>
                                    {
                                        ["schema"] = fk.ReferencedTable.Schema,
                                        ["table"] = fk.ReferencedTable.Name,
                                        ["column"] = fk.ReferencedColumns.FirstOrDefault()?.Name
                                    };
                                }
                            }
                        }
                        
                        columns.Add(columnInfo);
                    }
                    
                    tableInfo["columns"] = columns;
                    
                    if (showDetails)
                    {
                        if (table.PrimaryKey != null && table.PrimaryKey.Columns.Count > 0)
                        {
                            tableInfo["primaryKey"] = table.PrimaryKey.Columns.Select(c => c.Name).ToList();
                        }
                        
                        if (table.ForeignKeys.Count > 0)
                        {
                            var foreignKeys = new List<object>();
                            foreach (var fk in table.ForeignKeys)
                            {
                                foreignKeys.Add(new Dictionary<string, object>
                                {
                                    ["name"] = fk.Name,
                                    ["columns"] = fk.Columns.Select(c => c.Name).ToList(),
                                    ["referencedTable"] = new Dictionary<string, object>
                                    {
                                        ["schema"] = fk.ReferencedTable.Schema,
                                        ["name"] = fk.ReferencedTable.Name,
                                        ["columns"] = fk.ReferencedColumns.Select(c => c.Name).ToList()
                                    }
                                });
                            }
                            
                            tableInfo["foreignKeys"] = foreignKeys;
                        }
                        
                        if (table.Indexes.Count > 0)
                        {
                            var indexes = new List<object>();
                            foreach (var idx in table.Indexes)
                            {
                                indexes.Add(new Dictionary<string, object>
                                {
                                    ["name"] = idx.Name,
                                    ["isUnique"] = idx.IsUnique,
                                    ["columns"] = idx.Columns.Select(c => c.Name).ToList()
                                });
                            }
                            
                            tableInfo["indexes"] = indexes;
                        }
                    }
                    
                    result.Add(tableInfo);
                }
            }
            
            if (objectType.Equals("views", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var view in views)
                {
                    var viewInfo = new Dictionary<string, object>
                    {
                        ["name"] = view.Name,
                        ["schema"] = view.Schema
                    };
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(view.Description))
                    {
                        viewInfo["description"] = view.Description;
                    }
                    
                    var columns = new List<object>();
                    foreach (var column in view.Columns)
                    {
                        var columnInfo = new Dictionary<string, object>
                        {
                            ["name"] = column.Name,
                            ["dataType"] = column.DataType
                        };
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(column.Description))
                            {
                                columnInfo["description"] = column.Description;
                            }
                        }
                        
                        columns.Add(columnInfo);
                    }
                    
                    viewInfo["columns"] = columns;
                    
                    result.Add(viewInfo);
                }
            }
            
            if (objectType.Equals("procs", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var proc in storedProcedures)
                {
                    var procInfo = new Dictionary<string, object>
                    {
                        ["name"] = proc.Name,
                        ["schema"] = proc.Schema
                    };
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(proc.Description))
                    {
                        procInfo["description"] = proc.Description;
                    }
                    
                    var parameters = new List<object>();
                    foreach (var param in proc.Parameters)
                    {
                        var paramInfo = new Dictionary<string, object>
                        {
                            ["name"] = param.Name,
                            ["dataType"] = param.DataType
                        };
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(param.Description))
                            {
                                paramInfo["description"] = param.Description;
                            }
                        }
                        
                        parameters.Add(paramInfo);
                    }
                    
                    procInfo["parameters"] = parameters;
                    
                    result.Add(procInfo);
                }
            }
            
            if (objectType.Equals("functions", StringComparison.OrdinalIgnoreCase) || objectType.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var func in functions)
                {
                    var funcInfo = new Dictionary<string, object>
                    {
                        ["name"] = func.Name,
                        ["schema"] = func.Schema
                    };
                    
                    if (showDetails && !string.IsNullOrWhiteSpace(func.Description))
                    {
                        funcInfo["description"] = func.Description;
                    }
                    
                    var parameters = new List<object>();
                    foreach (var param in func.Parameters)
                    {
                        var paramInfo = new Dictionary<string, object>
                        {
                            ["name"] = param.Name,
                            ["dataType"] = param.DataType
                        };
                        
                        if (showDetails)
                        {
                            if (!string.IsNullOrWhiteSpace(param.Description))
                            {
                                paramInfo["description"] = param.Description;
                            }
                        }
                        
                        parameters.Add(paramInfo);
                    }
                    
                    funcInfo["parameters"] = parameters;
                    
                    result.Add(funcInfo);
                }
            }
            
            _logger.Info("Schema information in JSON format:");
            _logger.Info(JsonSerializer.Serialize(result, options));
        }

        private bool ParseBoolOption(IReadOnlyDictionary<string, string> options, string key, bool defaultValue)
        {
            if (options.TryGetValue(key, out var value))
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }
                
                if (value.Equals("yes", StringComparison.OrdinalIgnoreCase) || 
                    value.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                
                if (value.Equals("no", StringComparison.OrdinalIgnoreCase) || 
                    value.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("0", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            
            return defaultValue;
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
}
