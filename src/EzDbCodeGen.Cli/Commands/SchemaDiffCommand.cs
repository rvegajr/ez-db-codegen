using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema.Diff;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// The schema-diff command is responsible for comparing two database schemas and displaying the differences.
    /// </summary>
    public class SchemaDiffCommand : ICommand
    {
        private readonly IDatabaseSchemaProviderFactory _schemaProviderFactory;
        private readonly IRelationshipDetectorFactory _relationshipDetectorFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaDiffCommand"/> class.
        /// </summary>
        /// <param name="schemaProviderFactory">The schema provider factory to use.</param>
        /// <param name="relationshipDetectorFactory">The relationship detector factory to use.</param>
        /// <param name="logger">The logger to use.</param>
        public SchemaDiffCommand(
            IDatabaseSchemaProviderFactory schemaProviderFactory,
            IRelationshipDetectorFactory relationshipDetectorFactory,
            ILogger logger)
        {
            _schemaProviderFactory = schemaProviderFactory ?? throw new ArgumentNullException(nameof(schemaProviderFactory));
            _relationshipDetectorFactory = relationshipDetectorFactory ?? throw new ArgumentNullException(nameof(relationshipDetectorFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Name => "schema-diff";

        /// <inheritdoc/>
        public string Description => "Compare two database schemas and display the differences";

        /// <inheritdoc/>
        public string Usage => "schema-diff --source-connection <connection-string> --target-connection <connection-string> [--source-provider <provider-name>] [--target-provider <provider-name>] [--schema <schema-name>] [--format <json|text>] [--details <true|false>] [--detect-relationships <true|false>] [--object-type <tables|views|procs|functions|all>]";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            try
            {
                // Extract options
                options.TryGetValue("source-connection", out var sourceConnectionString);
                options.TryGetValue("target-connection", out var targetConnectionString);
                options.TryGetValue("source-provider", out var sourceProviderName);
                options.TryGetValue("target-provider", out var targetProviderName);
                options.TryGetValue("schema", out var schemaName);
                options.TryGetValue("format", out var format);
                options.TryGetValue("object-type", out var objectType);
                
                bool showDetails = ParseBoolOption(options, "details", true);
                bool detectRelationships = ParseBoolOption(options, "detect-relationships", true);
                
                // Set default values
                sourceProviderName ??= "SqlServer";
                targetProviderName ??= sourceProviderName;
                format ??= "text";
                objectType ??= "all";
                
                _logger.Info($"Comparing schemas between source and target databases...");
                _logger.Info($"Source Connection: {MaskConnectionString(sourceConnectionString)}");
                _logger.Info($"Target Connection: {MaskConnectionString(targetConnectionString)}");
                
                if (!string.IsNullOrWhiteSpace(schemaName))
                {
                    _logger.Info($"Filtering by schema: {schemaName}");
                }
                
                // Get source schema
                var sourceSchema = await GetDatabaseSchemaAsync(
                    sourceConnectionString, 
                    sourceProviderName, 
                    schemaName, 
                    detectRelationships,
                    objectType);
                
                // Get target schema
                var targetSchema = await GetDatabaseSchemaAsync(
                    targetConnectionString, 
                    targetProviderName, 
                    schemaName, 
                    detectRelationships,
                    objectType);
                
                // Compare schemas
                var schemaDiff = new SchemaDiff(sourceSchema, targetSchema);
                
                // Display differences
                if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    DisplayDiffAsJson(schemaDiff, showDetails);
                }
                else
                {
                    DisplayDiffAsText(schemaDiff, showDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error comparing schemas: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
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
            
            if (!options.ContainsKey("source-connection"))
            {
                errors.Add("Source connection string is required.");
            }
            
            if (!options.ContainsKey("target-connection"))
            {
                errors.Add("Target connection string is required.");
            }
            
            return errors;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<IOptionMetadata> GetOptions()
        {
            return new List<IOptionMetadata>
            {
                new OptionMetadata("source-connection", "The connection string for the source database", true, null),
                new OptionMetadata("target-connection", "The connection string for the target database", true, null),
                new OptionMetadata("source-provider", "The database provider for the source database (default: SqlServer)", false, "SqlServer"),
                new OptionMetadata("target-provider", "The database provider for the target database (default: same as source-provider)", false, null),
                new OptionMetadata("schema", "Filter objects by schema name", false, null),
                new OptionMetadata("format", "Output format: json or text (default: text)", false, "text"),
                new OptionMetadata("details", "Show detailed information: true or false (default: true)", false, "true"),
                new OptionMetadata("detect-relationships", "Detect relationships between tables: true or false (default: true)", false, "true"),
                new OptionMetadata("object-type", "Type of objects to compare: tables, views, procs, functions, or all (default: all)", false, "all")
            };
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> GetRequiredOptions()
        {
            return new List<string> { "source-connection", "target-connection" };
        }

        private async Task<IDatabaseSchema> GetDatabaseSchemaAsync(
            string connectionString, 
            string providerName, 
            string schemaName, 
            bool detectRelationships,
            string objectType)
        {
            // Create connection info dictionary
            var connectionInfo = new Dictionary<string, string>
            {
                ["ConnectionString"] = connectionString
            };

            // Create schema provider
            var schemaProvider = _schemaProviderFactory.CreateSchemaProvider(providerName, connectionInfo);
            
            // Extract schema based on object types
            if (objectType.Equals("tables", StringComparison.OrdinalIgnoreCase))
            {
                return await schemaProvider.GetTablesAsync(schemaName);
            }
            else if (objectType.Equals("views", StringComparison.OrdinalIgnoreCase))
            {
                return await schemaProvider.GetViewsAsync(schemaName);
            }
            else if (objectType.Equals("procs", StringComparison.OrdinalIgnoreCase))
            {
                return await schemaProvider.GetStoredProceduresAsync(schemaName);
            }
            else if (objectType.Equals("functions", StringComparison.OrdinalIgnoreCase))
            {
                return await schemaProvider.GetFunctionsAsync(schemaName);
            }
            else
            {
                // Get full schema
                var schema = await schemaProvider.GetSchemaAsync(schemaName);
                
                // Detect relationships if requested
                if (detectRelationships && schema.Tables != null && schema.Tables.Count > 0)
                {
                    var relationshipDetector = _relationshipDetectorFactory.CreateRelationshipDetector();
                    var options = new RelationshipDetectionOptions
                    {
                        DetectOneToOne = true,
                        DetectOneToMany = true,
                        DetectManyToMany = true,
                        DetectSelfReferencing = true,
                        DetectInheritance = true,
                        UseBidirectionalNavigationProperties = true,
                        UseIntelligentNavigationPropertyNaming = true
                    };
                    
                    schema.Relationships = relationshipDetector.DetectRelationships(schema.Tables, options);
                }
                
                return schema;
            }
        }

        private void DisplayDiffAsText(ISchemaDiff schemaDiff, bool showDetails)
        {
            if (!schemaDiff.HasDifferences)
            {
                _logger.Info("No differences found between the schemas.");
                return;
            }
            
            if (showDetails)
            {
                _logger.Info(schemaDiff.GetDetailedReport());
            }
            else
            {
                _logger.Info(schemaDiff.GetSummary());
            }
        }

        private void DisplayDiffAsJson(ISchemaDiff schemaDiff, bool showDetails)
        {
            if (!schemaDiff.HasDifferences)
            {
                _logger.Info(JsonSerializer.Serialize(new { message = "No differences found between the schemas." }));
                return;
            }
            
            var result = new Dictionary<string, object>();
            
            // Added tables
            if (schemaDiff.AddedTables.Count > 0)
            {
                var tables = new List<object>();
                foreach (var table in schemaDiff.AddedTables)
                {
                    tables.Add(new
                    {
                        schema = table.Schema,
                        name = table.Name,
                        columns = table.Columns.Count
                    });
                }
                result["addedTables"] = tables;
            }
            
            // Removed tables
            if (schemaDiff.RemovedTables.Count > 0)
            {
                var tables = new List<object>();
                foreach (var table in schemaDiff.RemovedTables)
                {
                    tables.Add(new
                    {
                        schema = table.Schema,
                        name = table.Name,
                        columns = table.Columns.Count
                    });
                }
                result["removedTables"] = tables;
            }
            
            // Modified tables
            if (schemaDiff.ModifiedTables.Count > 0)
            {
                var tables = new List<object>();
                foreach (var tableDiff in schemaDiff.ModifiedTables)
                {
                    var diff = new Dictionary<string, object>
                    {
                        ["schema"] = tableDiff.Source.Schema,
                        ["name"] = tableDiff.Source.Name
                    };
                    
                    if (tableDiff.AddedColumns.Count > 0)
                    {
                        diff["addedColumns"] = tableDiff.AddedColumns.Select(c => new { name = c.Name, dataType = c.DataType }).ToList();
                    }
                    
                    if (tableDiff.RemovedColumns.Count > 0)
                    {
                        diff["removedColumns"] = tableDiff.RemovedColumns.Select(c => new { name = c.Name, dataType = c.DataType }).ToList();
                    }
                    
                    if (tableDiff.ModifiedColumns.Count > 0)
                    {
                        diff["modifiedColumns"] = tableDiff.ModifiedColumns.Select(c => new 
                        { 
                            name = c.Source.Name,
                            changes = new {
                                dataTypeChanged = c.DataTypeChanged,
                                nullabilityChanged = c.NullabilityChanged,
                                defaultValueChanged = c.DefaultValueChanged,
                                maximumLengthChanged = c.MaxLengthChanged,
                                precisionChanged = c.PrecisionChanged,
                                scaleChanged = c.ScaleChanged
                            }
                        }).ToList();
                    }
                    
                    tables.Add(diff);
                }
                result["modifiedTables"] = tables;
            }
            
            // View differences
            if (schemaDiff.ViewDiffs.Count > 0)
            {
                var views = new List<object>();
                foreach (var viewDiff in schemaDiff.ViewDiffs)
                {
                    if (viewDiff.Original == null)
                    {
                        views.Add(new 
                        {
                            type = "added",
                            schema = viewDiff.New.Schema,
                            name = viewDiff.New.Name,
                            columns = viewDiff.New.Columns.Count
                        });
                    }
                    else if (viewDiff.New == null)
                    {
                        views.Add(new 
                        {
                            type = "removed",
                            schema = viewDiff.Original.Schema,
                            name = viewDiff.Original.Name,
                            columns = viewDiff.Original.Columns.Count
                        });
                    }
                    else
                    {
                        var diff = new Dictionary<string, object>
                        {
                            ["type"] = "modified",
                            ["schema"] = viewDiff.Original.Schema,
                            ["name"] = viewDiff.Original.Name
                        };
                        
                        if (viewDiff.AddedColumns.Count > 0)
                        {
                            diff["addedColumns"] = viewDiff.AddedColumns.Select(c => new { name = c.Name, dataType = c.DataType }).ToList();
                        }
                        
                        if (viewDiff.RemovedColumns.Count > 0)
                        {
                            diff["removedColumns"] = viewDiff.RemovedColumns.Select(c => new { name = c.Name, dataType = c.DataType }).ToList();
                        }
                        
                        if (viewDiff.DefinitionChanged)
                        {
                            diff["definitionChanged"] = true;
                        }
                        
                        if (viewDiff.IsIndexedChanged)
                        {
                            diff["indexingChanged"] = true;
                        }
                        
                        views.Add(diff);
                    }
                }
                result["viewChanges"] = views;
            }
            
            // Stored procedure differences
            if (schemaDiff.StoredProcedureDiffs.Count > 0)
            {
                var procs = new List<object>();
                foreach (var procDiff in schemaDiff.StoredProcedureDiffs)
                {
                    if (procDiff.Original == null)
                    {
                        procs.Add(new 
                        {
                            type = "added",
                            schema = procDiff.New.Schema,
                            name = procDiff.New.Name
                        });
                    }
                    else if (procDiff.New == null)
                    {
                        procs.Add(new 
                        {
                            type = "removed",
                            schema = procDiff.Original.Schema,
                            name = procDiff.Original.Name
                        });
                    }
                    else
                    {
                        var diff = new Dictionary<string, object>
                        {
                            ["type"] = "modified",
                            ["schema"] = procDiff.Original.Schema,
                            ["name"] = procDiff.Original.Name
                        };
                        
                        if (procDiff.AddedParameters.Count > 0)
                        {
                            diff["addedParameters"] = procDiff.AddedParameters.Select(p => new { name = p.Name, dataType = p.DataType }).ToList();
                        }
                        
                        if (procDiff.RemovedParameters.Count > 0)
                        {
                            diff["removedParameters"] = procDiff.RemovedParameters.Select(p => new { name = p.Name, dataType = p.DataType }).ToList();
                        }
                        
                        if (procDiff.ChangedParameters.Count > 0)
                        {
                            diff["changedParameters"] = procDiff.ChangedParameters.Select(p => new { name = p.Name, dataType = p.DataType }).ToList();
                        }
                        
                        if (procDiff.DefinitionChanged)
                        {
                            diff["definitionChanged"] = true;
                        }
                        
                        procs.Add(diff);
                    }
                }
                result["storedProcedureChanges"] = procs;
            }
            
            // Function differences
            if (schemaDiff.FunctionDiffs.Count > 0)
            {
                var funcs = new List<object>();
                foreach (var funcDiff in schemaDiff.FunctionDiffs)
                {
                    if (funcDiff.Original == null)
                    {
                        funcs.Add(new 
                        {
                            type = "added",
                            schema = funcDiff.New.Schema,
                            name = funcDiff.New.Name,
                            isTableValued = funcDiff.New.IsTableValued
                        });
                    }
                    else if (funcDiff.New == null)
                    {
                        funcs.Add(new 
                        {
                            type = "removed",
                            schema = funcDiff.Original.Schema,
                            name = funcDiff.Original.Name,
                            isTableValued = funcDiff.Original.IsTableValued
                        });
                    }
                    else
                    {
                        var diff = new Dictionary<string, object>
                        {
                            ["type"] = "modified",
                            ["schema"] = funcDiff.Original.Schema,
                            ["name"] = funcDiff.Original.Name
                        };
                        
                        if (funcDiff.ReturnTypeChanged)
                        {
                            diff["returnTypeChanged"] = new
                            {
                                from = funcDiff.Original.ReturnType,
                                to = funcDiff.New.ReturnType
                            };
                        }
                        
                        if (funcDiff.IsTableValuedChanged)
                        {
                            diff["functionTypeChanged"] = new
                            {
                                from = funcDiff.Original.IsTableValued ? "Table-valued" : "Scalar",
                                to = funcDiff.New.IsTableValued ? "Table-valued" : "Scalar"
                            };
                        }
                        
                        if (funcDiff.AddedParameters.Count > 0)
                        {
                            diff["addedParameters"] = funcDiff.AddedParameters.Select(p => new { name = p.Name, dataType = p.DataType }).ToList();
                        }
                        
                        if (funcDiff.RemovedParameters.Count > 0)
                        {
                            diff["removedParameters"] = funcDiff.RemovedParameters.Select(p => new { name = p.Name, dataType = p.DataType }).ToList();
                        }
                        
                        if (funcDiff.DefinitionChanged)
                        {
                            diff["definitionChanged"] = true;
                        }
                        
                        funcs.Add(diff);
                    }
                }
                result["functionChanges"] = funcs;
            }
            
            var options = new JsonSerializerOptions { WriteIndented = true };
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
