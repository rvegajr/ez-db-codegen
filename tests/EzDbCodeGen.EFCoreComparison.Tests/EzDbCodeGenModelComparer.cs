using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Core;
using EzDbCodeGen.Core.Interfaces;
using EzDbCodeGen.Core.Config;
using EzDbCodeGen.Core.Extenders;
using EzDbCodeGen.TypeMapping;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Compares EzDbCodeGen models with database schemas
    /// </summary>
    public class EzDbCodeGenModelComparer
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;

        public EzDbCodeGenModelComparer(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
        }

        public EzDbCodeGenModelComparer(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<EzDbCodeGenModelComparer>();
        }

        /// <summary>
        /// Analyzes a database schema using EzDbCodeGen
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="databaseName">The name of the database</param>
        /// <returns>The database metadata</returns>
        public DatabaseMetadata AnalyzeDatabase(string connectionString, string databaseName)
        {
            _logger.LogInformation($"Analyzing database schema for {databaseName} using EzDbCodeGen...");

            var stopwatch = Stopwatch.StartNew();
            
            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = connectionString,
                DatabaseType = "SqlServer",
                Name = databaseName
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = metadataProvider.GetDatabaseMetadata();
            
            stopwatch.Stop();
            _logger.LogInformation($"EzDbCodeGen schema analysis completed in {stopwatch.ElapsedMilliseconds}ms");
            
            return metadata;
        }

        /// <summary>
        /// Analyzes relationships in a database using EzDbCodeGen
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="databaseName">The name of the database</param>
        /// <returns>A list of relationships detected in the database</returns>
        public List<RelationshipInfo> AnalyzeEzDbCodeGenRelationships(string connectionString, string databaseName)
        {
            _logger.LogInformation($"Analyzing relationships in {databaseName} using EzDbCodeGen...");

            var metadata = AnalyzeDatabase(connectionString, databaseName);
            var relationships = new List<RelationshipInfo>();

            foreach (var table in metadata.Tables)
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    relationships.Add(new RelationshipInfo
                    {
                        SourceTable = table.Name,
                        TargetTable = foreignKey.ReferencedTableName,
                        SourceProperty = $"{foreignKey.ReferencedTableName}Reference",
                        IsCollection = false,
                        RelationshipType = "ManyToOne" // Default assumption
                    });

                    // Add the reverse relationship (collection)
                    relationships.Add(new RelationshipInfo
                    {
                        SourceTable = foreignKey.ReferencedTableName,
                        TargetTable = table.Name,
                        SourceProperty = $"{table.Name}Collection",
                        IsCollection = true,
                        RelationshipType = "OneToMany" // Default assumption
                    });
                }
            }

            return relationships;
        }

        /// <summary>
        /// Analyzes column types in a database using EzDbCodeGen
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="databaseName">The name of the database</param>
        /// <returns>A dictionary mapping table.column to its type</returns>
        public Dictionary<string, string> AnalyzeEzDbCodeGenColumnTypes(string connectionString, string databaseName)
        {
            _logger.LogInformation($"Analyzing column types in {databaseName} using EzDbCodeGen...");

            var metadata = AnalyzeDatabase(connectionString, databaseName);
            var columnTypes = new Dictionary<string, string>();

            // Create a type mapper to map SQL types to .NET types
            var typeMapper = new SqlServerTypeMapper();

            foreach (var table in metadata.Tables)
            {
                foreach (var column in table.Columns)
                {
                    var key = $"{table.Name}.{column.Name}";
                    var dotNetType = typeMapper.MapFrom(column.DataType, column.IsNullable);
                    columnTypes[key] = dotNetType;
                }
            }

            return columnTypes;
        }

        /// <summary>
        /// Performs a performance test of EzDbCodeGen schema discovery
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="databaseName">The name of the database</param>
        /// <returns>Performance metrics</returns>
        public PerformanceMetrics MeasureEzDbCodeGenPerformance(string connectionString, string databaseName)
        {
            _logger.LogInformation($"Measuring EzDbCodeGen performance for {databaseName}...");

            var metrics = new PerformanceMetrics
            {
                DatabaseName = databaseName
            };

            var stopwatch = Stopwatch.StartNew();
            
            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = connectionString,
                DatabaseType = "SqlServer",
                Name = databaseName
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Measure schema discovery time
            stopwatch.Restart();
            var metadata = metadataProvider.GetDatabaseMetadata();
            stopwatch.Stop();
            metrics.SchemaDiscoveryTime = stopwatch.ElapsedMilliseconds;
            
            // Count tables and columns
            metrics.TableCount = metadata.Tables.Count;
            metrics.ColumnCount = metadata.Tables.Sum(t => t.Columns.Count);
            metrics.RelationshipCount = metadata.Tables.Sum(t => t.ForeignKeys.Count);
            
            return metrics;
        }

        /// <summary>
        /// Gets all relationships from the database schema using EzDbCodeGen
        /// </summary>
        public async Task<List<Relationship>> GetAllRelationshipsAsync(string[] schemas)
        {
            _logger.LogInformation("Getting all relationships using EzDbCodeGen...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            var relationships = new List<Relationship>();
            
            // Process all tables in the requested schemas
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    // Find the referenced table
                    var referencedTable = metadata.Tables.FirstOrDefault(t => 
                        t.Name == foreignKey.ReferencedTableName);
                    
                    if (referencedTable != null)
                    {
                        var relationship = new Relationship
                        {
                            Name = foreignKey.Name,
                            PrimaryKeyTableSchema = referencedTable.Schema,
                            PrimaryKeyTableName = referencedTable.Name,
                            PrimaryKeyColumnNames = foreignKey.ReferencedColumnNames.ToList(),
                            ForeignKeyTableSchema = table.Schema,
                            ForeignKeyTableName = table.Name,
                            ForeignKeyColumnNames = foreignKey.ColumnNames.ToList(),
                            IsSelfReferencing = table.Name == referencedTable.Name
                        };
                        
                        relationships.Add(relationship);
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Gets many-to-many relationships from the database schema using EzDbCodeGen
        /// </summary>
        public async Task<List<ManyToManyRelationship>> GetManyToManyRelationshipsAsync(string[] schemas)
        {
            _logger.LogInformation("Getting many-to-many relationships using EzDbCodeGen...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            var relationships = new List<ManyToManyRelationship>();
            
            // Find potential junction tables (tables with exactly 2 foreign keys)
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                // Check if this table has exactly 2 foreign keys
                if (table.ForeignKeys.Count == 2)
                {
                    var fk1 = table.ForeignKeys[0];
                    var fk2 = table.ForeignKeys[1];
                    
                    // Find the referenced tables
                    var leftTable = metadata.Tables.FirstOrDefault(t => t.Name == fk1.ReferencedTableName);
                    var rightTable = metadata.Tables.FirstOrDefault(t => t.Name == fk2.ReferencedTableName);
                    
                    if (leftTable != null && rightTable != null)
                    {
                        // Determine payload columns (columns that are not part of foreign keys)
                        var fkColumns = new HashSet<string>();
                        fkColumns.UnionWith(fk1.ColumnNames);
                        fkColumns.UnionWith(fk2.ColumnNames);
                        
                        var payloadColumns = table.Columns
                            .Where(c => !fkColumns.Contains(c.Name) && c.Name != "ID" && !c.Name.EndsWith("ID"))
                            .Select(c => c.Name)
                            .ToList();
                        
                        var relationship = new ManyToManyRelationship
                        {
                            Name = $"{leftTable.Name}To{rightTable.Name}",
                            LeftTableSchema = leftTable.Schema,
                            LeftTableName = leftTable.Name,
                            LeftColumnNames = fk1.ReferencedColumnNames.ToList(),
                            RightTableSchema = rightTable.Schema,
                            RightTableName = rightTable.Name,
                            RightColumnNames = fk2.ReferencedColumnNames.ToList(),
                            JoinTableSchema = table.Schema,
                            JoinTableName = table.Name,
                            LeftJoinColumnNames = fk1.ColumnNames.ToList(),
                            RightJoinColumnNames = fk2.ColumnNames.ToList(),
                            HasPayloadColumns = payloadColumns.Count > 0,
                            PayloadColumnNames = payloadColumns
                        };
                        
                        relationships.Add(relationship);
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Gets inheritance relationships from the database schema using EzDbCodeGen
        /// </summary>
        public async Task<List<InheritanceRelationship>> GetInheritanceRelationshipsAsync(string[] schemas)
        {
            _logger.LogInformation("Getting inheritance relationships using EzDbCodeGen...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            var relationships = new List<InheritanceRelationship>();
            
            // Look for potential TPT (Table-Per-Type) relationships
            // These are typically identified by tables that have a 1:1 relationship
            // where the primary key of one table is also a foreign key to another table
            foreach (var derivedTable in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                // Check if this table has a primary key that is also a foreign key
                var pkColumns = derivedTable.PrimaryKey?.ColumnNames.ToHashSet() ?? new HashSet<string>();
                
                foreach (var foreignKey in derivedTable.ForeignKeys)
                {
                    // Check if the foreign key columns match the primary key columns exactly
                    if (foreignKey.ColumnNames.All(c => pkColumns.Contains(c)) && 
                        foreignKey.ColumnNames.Count == pkColumns.Count)
                    {
                        // This is likely a TPT relationship
                        var baseTable = metadata.Tables.FirstOrDefault(t => 
                            t.Name == foreignKey.ReferencedTableName);
                        
                        if (baseTable != null)
                        {
                            var relationship = new InheritanceRelationship
                            {
                                BaseTableSchema = baseTable.Schema,
                                BaseTableName = baseTable.Name,
                                DerivedTableSchema = derivedTable.Schema,
                                DerivedTableName = derivedTable.Name,
                                InheritanceType = "TPT", // Table-Per-Type
                                DiscriminatorColumn = null
                            };
                            
                            relationships.Add(relationship);
                        }
                    }
                }
            }
            
            // Look for potential TPH (Table-Per-Hierarchy) relationships
            // These are typically identified by tables with a discriminator column
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                // Check for common discriminator column names
                var potentialDiscriminatorColumns = new[] { "Discriminator", "Type", "EntityType", "RecordType" };
                
                foreach (var column in table.Columns)
                {
                    if (potentialDiscriminatorColumns.Contains(column.Name))
                    {
                        // This is likely a TPH relationship with this table as the base
                        var relationship = new InheritanceRelationship
                        {
                            BaseTableSchema = table.Schema,
                            BaseTableName = table.Name,
                            DerivedTableSchema = table.Schema, // Same schema for TPH
                            DerivedTableName = $"{table.Name}Derived", // Placeholder for derived entity
                            InheritanceType = "TPH", // Table-Per-Hierarchy
                            DiscriminatorColumn = column.Name
                        };
                        
                        relationships.Add(relationship);
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Gets edge case relationships from the database schema using EzDbCodeGen
        /// </summary>
        public async Task<List<Relationship>> GetEdgeCaseRelationshipsAsync(string[] schemas)
        {
            _logger.LogInformation("Getting edge case relationships using EzDbCodeGen...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            var relationships = new List<Relationship>();
            
            // Find self-referencing relationships
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    if (foreignKey.ReferencedTableName == table.Name)
                    {
                        // This is a self-referencing relationship
                        var relationship = new Relationship
                        {
                            Name = foreignKey.Name,
                            PrimaryKeyTableSchema = table.Schema,
                            PrimaryKeyTableName = table.Name,
                            PrimaryKeyColumnNames = foreignKey.ReferencedColumnNames.ToList(),
                            ForeignKeyTableSchema = table.Schema,
                            ForeignKeyTableName = table.Name,
                            ForeignKeyColumnNames = foreignKey.ColumnNames.ToList(),
                            IsSelfReferencing = true
                        };
                        
                        relationships.Add(relationship);
                    }
                }
            }
            
            // Find tables with multiple foreign keys to the same table
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                // Group foreign keys by referenced table
                var fkGroups = table.ForeignKeys
                    .GroupBy(fk => fk.ReferencedTableName)
                    .Where(g => g.Count() > 1);
                
                foreach (var group in fkGroups)
                {
                    var referencedTable = metadata.Tables.FirstOrDefault(t => t.Name == group.Key);
                    
                    if (referencedTable != null)
                    {
                        foreach (var foreignKey in group)
                        {
                            var relationship = new Relationship
                            {
                                Name = foreignKey.Name,
                                PrimaryKeyTableSchema = referencedTable.Schema,
                                PrimaryKeyTableName = referencedTable.Name,
                                PrimaryKeyColumnNames = foreignKey.ReferencedColumnNames.ToList(),
                                ForeignKeyTableSchema = table.Schema,
                                ForeignKeyTableName = table.Name,
                                ForeignKeyColumnNames = foreignKey.ColumnNames.ToList(),
                                IsSelfReferencing = false
                            };
                            
                            relationships.Add(relationship);
                        }
                    }
                }
            }
            
            return relationships;
        }

        /// <summary>
        /// Gets navigation properties from the database schema using EzDbCodeGen
        /// </summary>
        public async Task<List<NavigationProperty>> GetNavigationPropertiesAsync(string[] schemas)
        {
            _logger.LogInformation("Getting navigation properties using EzDbCodeGen...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            var navigationProperties = new List<NavigationProperty>();
            
            // Process all tables in the requested schemas
            foreach (var table in metadata.Tables.Where(t => schemas.Contains(t.Schema)))
            {
                // Generate reference navigation properties (many-to-one)
                foreach (var foreignKey in table.ForeignKeys)
                {
                    var referencedTable = metadata.Tables.FirstOrDefault(t => 
                        t.Name == foreignKey.ReferencedTableName);
                    
                    if (referencedTable != null)
                    {
                        // Generate a semantic name for the navigation property
                        string navigationName = GetSemanticNavigationName(foreignKey, referencedTable.Name, false);
                        
                        var navigationProperty = new NavigationProperty
                        {
                            Name = navigationName,
                            SourceTableName = table.Name,
                            TargetTableName = referencedTable.Name,
                            IsCollection = false
                        };
                        
                        navigationProperties.Add(navigationProperty);
                        
                        // Generate the reverse navigation property (collection)
                        string collectionName = GetSemanticNavigationName(foreignKey, table.Name, true);
                        
                        var collectionProperty = new NavigationProperty
                        {
                            Name = collectionName,
                            SourceTableName = referencedTable.Name,
                            TargetTableName = table.Name,
                            IsCollection = true
                        };
                        
                        navigationProperties.Add(collectionProperty);
                    }
                }
            }
            
            return navigationProperties;
        }

        /// <summary>
        /// Generates a semantic name for a navigation property
        /// </summary>
        private string GetSemanticNavigationName(ForeignKeyInfo foreignKey, string targetTableName, bool isCollection)
        {
            // Start with the target table name
            string name = targetTableName;
            
            // If this is a collection, pluralize the name
            if (isCollection)
            {
                // Simple pluralization rules
                if (name.EndsWith("y") && !name.EndsWith("ay") && !name.EndsWith("ey") && !name.EndsWith("oy") && !name.EndsWith("uy"))
                    name = name.Substring(0, name.Length - 1) + "ies";
                else if (name.EndsWith("s") || name.EndsWith("x") || name.EndsWith("z") || name.EndsWith("ch") || name.EndsWith("sh"))
                    name += "es";
                else
                    name += "s";
            }
            
            // If the foreign key column has a prefix that matches the target table name, use it
            // For example, if the column is CustomerID and the target table is Customer, use Customer
            foreach (var column in foreignKey.ColumnNames)
            {
                if (column.EndsWith("ID") || column.EndsWith("Id"))
                {
                    string prefix = column.Substring(0, column.Length - 2);
                    
                    if (prefix.Equals(targetTableName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (isCollection)
                        {
                            // Simple pluralization rules
                            if (prefix.EndsWith("y") && !prefix.EndsWith("ay") && !prefix.EndsWith("ey") && !prefix.EndsWith("oy") && !prefix.EndsWith("uy"))
                                return prefix.Substring(0, prefix.Length - 1) + "ies";
                            else if (prefix.EndsWith("s") || prefix.EndsWith("x") || prefix.EndsWith("z") || prefix.EndsWith("ch") || prefix.EndsWith("sh"))
                                return prefix + "es";
                            else
                                return prefix + "s";
                        }
                        else
                        {
                            return prefix;
                        }
                    }
                }
            }
            
            return name;
        }

        /// <summary>
        /// Simulates EzDbCodeGen code generation for performance comparison
        /// </summary>
        public async Task<bool> SimulateEzDbCodeGenGenerationAsync(IDatabaseSchema schema)
        {
            _logger.LogInformation("Simulating EzDbCodeGen code generation...");

            // Create a database configuration
            var dbConfig = new DatabaseConfig
            {
                ConnectionString = _connectionString,
                DatabaseType = "SqlServer",
                Name = "WideWorldImporters"
            };

            // Create a metadata provider
            var metadataProvider = new DatabaseMetadataProviderFactory().Create(dbConfig);
            
            // Get the database metadata
            var metadata = await Task.Run(() => metadataProvider.GetDatabaseMetadata());
            
            // Simulate code generation by processing each table
            var stopwatch = Stopwatch.StartNew();
            
            foreach (var table in metadata.Tables)
            {
                // Simulate generating an entity class
                var entityClass = new StringBuilder();
                entityClass.AppendLine($"public class {table.Name}");
                entityClass.AppendLine("{");
                
                // Add properties for each column
                foreach (var column in table.Columns)
                {
                    var typeMapper = new SqlServerTypeMapper();
                    var dotNetType = typeMapper.MapFrom(column.DataType, column.IsNullable);
                    entityClass.AppendLine($"    public {dotNetType} {column.Name} {{ get; set; }}");
                }
                
                // Add navigation properties
                foreach (var foreignKey in table.ForeignKeys)
                {
                    var referencedTable = metadata.Tables.FirstOrDefault(t => 
                        t.Name == foreignKey.ReferencedTableName);
                    
                    if (referencedTable != null)
                    {
                        // Reference navigation property
                        entityClass.AppendLine($"    public {referencedTable.Name} {referencedTable.Name} {{ get; set; }}");
                    }
                }
                
                // Find tables that reference this table
                foreach (var otherTable in metadata.Tables)
                {
                    foreach (var foreignKey in otherTable.ForeignKeys)
                    {
                        if (foreignKey.ReferencedTableName == table.Name)
                        {
                            // Collection navigation property
                            entityClass.AppendLine($"    public ICollection<{otherTable.Name}> {otherTable.Name}s {{ get; set; }}");
                        }
                    }
                }
                
                entityClass.AppendLine("}");
                
                // Simulate writing the file
                // In a real implementation, this would write to a file
                var entityClassString = entityClass.ToString();
            }
            
            stopwatch.Stop();
            _logger.LogInformation($"EzDbCodeGen code generation completed in {stopwatch.ElapsedMilliseconds}ms");
            
            return true;
        }
    }

    /// <summary>
    /// Represents performance metrics for comparison
    /// </summary>
    public class PerformanceMetrics
    {
        public string DatabaseName { get; set; }
        public long SchemaDiscoveryTime { get; set; }
        public int TableCount { get; set; }
        public int ColumnCount { get; set; }
        public int RelationshipCount { get; set; }

        public override string ToString()
        {
            return $"Database: {DatabaseName}, " +
                   $"Schema Discovery: {SchemaDiscoveryTime}ms, " +
                   $"Tables: {TableCount}, " +
                   $"Columns: {ColumnCount}, " +
                   $"Relationships: {RelationshipCount}";
        }
    }
}
