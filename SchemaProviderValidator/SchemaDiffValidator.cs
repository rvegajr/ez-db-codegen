using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema;
using EzDbCodeGen.Schema.Diff;
using Microsoft.Extensions.Logging;

namespace SchemaProviderValidator
{
    /// <summary>
    /// A simple validator for testing the schema diff functionality.
    /// </summary>
    public class SchemaDiffValidator
    {
        private readonly ILogger _logger;
        private readonly IDatabaseSchemaProviderFactory _schemaProviderFactory;
        private readonly IRelationshipDetector _relationshipDetector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaDiffValidator"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public SchemaDiffValidator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schemaProviderFactory = new DatabaseSchemaProviderFactory(_logger);
            _relationshipDetector = new RelationshipDetector(_logger);
        }

        /// <summary>
        /// Validates the schema diff functionality by comparing schemas from two databases.
        /// </summary>
        /// <param name="sourceConnectionString">The connection string for the source database.</param>
        /// <param name="targetConnectionString">The connection string for the target database.</param>
        /// <param name="providerName">The database provider name (default: SqlServer).</param>
        /// <param name="schemaName">Optional schema name to filter by.</param>
        /// <param name="objectType">Type of objects to compare (tables, views, procs, functions, all).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ValidateSchemaDiffAsync(
            string sourceConnectionString, 
            string targetConnectionString, 
            string providerName = "SqlServer", 
            string schemaName = null,
            string objectType = "all")
        {
            _logger.LogInformation("Comparing schemas between source and target databases...");
            
            // Create connection info dictionaries
            var sourceConnectionInfo = new Dictionary<string, string>
            {
                ["ConnectionString"] = sourceConnectionString
            };
            
            var targetConnectionInfo = new Dictionary<string, string>
            {
                ["ConnectionString"] = targetConnectionString
            };
            
            // Get source schema
            var sourceSchema = await GetDatabaseSchemaAsync(
                sourceConnectionInfo, 
                providerName, 
                schemaName,
                objectType);
            
            // Get target schema
            var targetSchema = await GetDatabaseSchemaAsync(
                targetConnectionInfo, 
                providerName, 
                schemaName,
                objectType);
            
            // Compare schemas
            var schemaDiff = new SchemaDiff(sourceSchema, targetSchema);
            
            // Display results
            _logger.LogInformation("Schema Diff Results:");
            
            if (!schemaDiff.HasDifferences)
            {
                _logger.LogInformation("No differences found between the schemas.");
                return;
            }
            
            // Show detailed report
            _logger.LogInformation(schemaDiff.GetDetailedReport());
            
            // Show statistics
            _logger.LogInformation("\nDifference Statistics:");
            _logger.LogInformation($"- Added Tables: {schemaDiff.AddedTables.Count}");
            _logger.LogInformation($"- Removed Tables: {schemaDiff.RemovedTables.Count}");
            _logger.LogInformation($"- Modified Tables: {schemaDiff.ModifiedTables.Count}");
            _logger.LogInformation($"- View Changes: {schemaDiff.ViewDiffs.Count}");
            _logger.LogInformation($"- Stored Procedure Changes: {schemaDiff.StoredProcedureDiffs.Count}");
            _logger.LogInformation($"- Function Changes: {schemaDiff.FunctionDiffs.Count}");
            _logger.LogInformation($"- Relationship Changes: {schemaDiff.RelationshipDiffs.Count}");
        }

        private async Task<IDatabaseSchema> GetDatabaseSchemaAsync(
            Dictionary<string, string> connectionInfo, 
            string providerName, 
            string schemaName,
            string objectType)
        {
            // Create schema provider
            var schemaProvider = _schemaProviderFactory.CreateSchemaProvider(providerName, connectionInfo);
            
            // Extract schema based on object types
            if (objectType.Equals("tables", StringComparison.OrdinalIgnoreCase))
            {
                var schema = await schemaProvider.GetTablesAsync(schemaName);
                DetectRelationships(schema);
                return schema;
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
                DetectRelationships(schema);
                return schema;
            }
        }

        private void DetectRelationships(IDatabaseSchema schema)
        {
            if (schema.Tables != null && schema.Tables.Count > 0)
            {
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
                
                schema.Relationships = _relationshipDetector.DetectRelationships(schema.Tables, options);
                _logger.LogInformation($"Detected {schema.Relationships.Count} relationships");
            }
        }
    }
}
