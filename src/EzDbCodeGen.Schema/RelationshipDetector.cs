using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;

namespace EzDbCodeGen.Schema
{
    /// <summary>
    /// Implements advanced relationship detection for database schemas.
    /// Detects one-to-many, one-to-one, many-to-many, and inheritance relationships.
    /// </summary>
    public class RelationshipDetector : IRelationshipDetector
    {
        private readonly ILogger _logger;
        private readonly List<IRelationship> _relationships = new List<IRelationship>();
        private IDatabaseSchema _schema = null!;

        /// <summary>
        /// Creates a new instance of the RelationshipDetector.
        /// </summary>
        /// <param name="logger">Logger for diagnostic messages</param>
        public RelationshipDetector(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the collection of detected relationships.
        /// </summary>
        public IReadOnlyList<IRelationship> Relationships => _relationships.AsReadOnly();

        /// <summary>
        /// Detects relationships in the given schema model.
        /// </summary>
        /// <param name="schema">The schema model to analyze</param>
        /// <param name="options">Options for relationship detection</param>
        /// <returns>A collection of detected relationships</returns>
        public IReadOnlyCollection<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options)
        {
            if (schema == null)
                throw new ArgumentNullException(nameof(schema));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _schema = schema;
            _relationships.Clear();

            _logger.LogInformation("Starting relationship detection...");

            // Step 1: Detect one-to-many relationships (based on foreign keys)
            if (options.DetectOneToManyRelationships)
            {
                DetectOneToManyRelationships();
                _logger.LogInformation($"Detected {_relationships.Count(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToMany)} one-to-many relationships");
            }

            // Step 2: Detect one-to-one relationships (one-to-many where FK is unique)
            if (options.DetectOneToOneRelationships)
            {
                DetectOneToOneRelationships();
                _logger.LogInformation($"Detected {_relationships.Count(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToOne)} one-to-one relationships");
            }

            // Step 3: Detect many-to-many relationships (using junction tables)
            if (options.DetectManyToManyRelationships)
            {
                DetectManyToManyRelationships();
                _logger.LogInformation($"Detected {_relationships.Count(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToMany)} many-to-many relationships");
            }

            // Step 4: Detect inheritance relationships
            if (options.DetectInheritanceRelationships)
            {
                DetectInheritanceRelationships();
                _logger.LogInformation($"Detected {_relationships.Count(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.Inheritance)} inheritance relationships");
            }

            // Step 5: Set IsJunctionTable flag on junction tables
            MarkJunctionTables();

            _logger.LogInformation($"Relationship detection complete. Total relationships: {_relationships.Count}");

            // Apply navigation property naming if enabled
            if (options.UseIntelligentNavigationNaming)
            {
                foreach (var relationship in _relationships)
                {
                    if (relationship is RelationshipModel model)
                    {
                        model.SourceNavigationPropertyName = GetNavigationPropertyName(model.TargetTable, model.IsSourceCollection);
                        model.TargetNavigationPropertyName = GetNavigationPropertyName(model.SourceTable, model.IsTargetCollection);
                    }
                }
            }

            return _relationships.AsReadOnly();
        }

        /// <summary>
        /// Detects relationships in the given schema model.
        /// </summary>
        /// <param name="schema">The schema model to analyze</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task<IReadOnlyCollection<IRelationship>> DetectRelationshipsAsync(IDatabaseSchema schema)
        {
            return await Task.Run(() => DetectRelationships(schema, new RelationshipDetectionOptions
            {
                DetectOneToManyRelationships = true,
                DetectOneToOneRelationships = true,
                DetectManyToManyRelationships = true,
                DetectInheritanceRelationships = true,
                UseIntelligentNavigationNaming = true
            }));
        }

        /// <summary>
        /// Detects one-to-many relationships based on foreign keys.
        /// </summary>
        private void DetectOneToManyRelationships()
        {
            _logger.LogInformation("Detecting one-to-many relationships...");

            foreach (var table in _schema.Tables)
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    // Skip self-referencing foreign keys for now
                    if (foreignKey.ReferencedTable == table)
                        continue;

                    var relationship = new RelationshipModel
                    {
                        SourceTable = foreignKey.ReferencedTable,
                        TargetTable = table,
                        Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToMany,
                        ForeignKey = foreignKey,
                        IsSourceCollection = true,
                        IsTargetCollection = false,
                        DeleteBehavior = foreignKey.DeleteAction
                    };

                    _relationships.Add(relationship);
                }
            }
        }

        /// <summary>
        /// Detects one-to-one relationships by examining existing one-to-many relationships
        /// where the foreign key columns form a unique constraint or are the primary key.
        /// </summary>
        private void DetectOneToOneRelationships()
        {
            _logger.LogInformation("Detecting one-to-one relationships...");
            
            // Find one-to-many relationships where the foreign key forms a unique constraint
            var oneToManyRelationships = _relationships
                .Where(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToMany)
                .ToList();
            
            foreach (var relationship in oneToManyRelationships)
            {
                if (relationship is RelationshipModel model && model.ForeignKey != null)
                {
                    bool isOneToOne = false;
                    var foreignKeyColumns = model.ForeignKey.Columns.Select(c => c.Name).ToList();
                    
                    // Check if foreign key columns form the primary key
                    if (model.SourceTable.PrimaryKey != null)
                    {
                        var primaryKeyColumns = model.SourceTable.PrimaryKey.Columns.Select(c => c.Name).ToList();
                        
                        if (foreignKeyColumns.Count == primaryKeyColumns.Count && 
                            foreignKeyColumns.All(f => primaryKeyColumns.Contains(f, StringComparer.OrdinalIgnoreCase)))
                        {
                            isOneToOne = true;
                        }
                    }
                    
                    // Check if foreign key columns form a unique constraint
                    if (!isOneToOne && model.SourceTable.UniqueConstraints != null)
                    {
                        foreach (var uniqueConstraint in model.SourceTable.UniqueConstraints)
                        {
                            var uniqueColumns = uniqueConstraint.Columns.Select(c => c.Name).ToList();
                            
                            if (foreignKeyColumns.Count == uniqueColumns.Count && 
                                foreignKeyColumns.All(f => uniqueColumns.Contains(f, StringComparer.OrdinalIgnoreCase)))
                            {
                                isOneToOne = true;
                                break;
                            }
                        }
                    }
                    
                    // Check if foreign key columns form a unique index
                    if (!isOneToOne && model.SourceTable.Indexes != null)
                    {
                        foreach (var index in model.SourceTable.Indexes.Where(i => i.IsUnique))
                        {
                            var indexColumns = index.Columns.Select(c => c.Name).ToList();
                            
                            if (foreignKeyColumns.Count == indexColumns.Count && 
                                foreignKeyColumns.All(f => indexColumns.Contains(f, StringComparer.OrdinalIgnoreCase)))
                            {
                                isOneToOne = true;
                                break;
                            }
                        }
                    }
                    
                    // Update relationship type if it's one-to-one
                    if (isOneToOne)
                    {
                        model.Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToOne;
                        model.IsTargetCollection = false; // In one-to-one relationships, neither side is a collection
                        model.TargetNavigationPropertyName = GetNavigationPropertyName(model.SourceTable, false);
                    }
                }
            }
        }

        /// <summary>
        /// Detects many-to-many relationships by identifying junction tables.
        /// A junction table typically has:
        /// - Exactly two foreign keys
        /// - The primary key consists of exactly these foreign key columns
        /// - Few or no other non-key columns (except metadata)
        /// </summary>
        private void DetectManyToManyRelationships()
        {
            _logger.LogInformation("Detecting many-to-many relationships...");
            
            foreach (var table in _schema.Tables)
            {
                // Candidate junction tables have exactly two foreign keys
                if (table.ForeignKeys.Count != 2)
                    continue;
                
                var foreignKeysList = table.ForeignKeys.ToList();
                var foreignKey1 = foreignKeysList[0];
                var foreignKey2 = foreignKeysList[1];
                
                // Check if the foreign keys reference different tables (not self-referencing)
                if (foreignKey1.ReferencedTable == foreignKey2.ReferencedTable)
                    continue;
                
                // Check if the primary key consists of exactly the foreign key columns
                if (table.PrimaryKey == null || 
                    table.PrimaryKey.Columns.Count != foreignKey1.Columns.Count + foreignKey2.Columns.Count)
                    continue;
                
                // Get all the columns involved in both foreign keys
                var fkColumnNames = new HashSet<string>(
                    foreignKey1.Columns.Select(c => c.Name)
                    .Concat(foreignKey2.Columns.Select(c => c.Name)),
                    StringComparer.OrdinalIgnoreCase);
                
                // Check if all primary key columns are part of a foreign key
                if (!table.PrimaryKey.Columns.All(c => fkColumnNames.Contains(c.Name)))
                    continue;
                
                // Find any payload properties (non-primary key columns)
                var payloadProperties = table.Columns
                    .Where(c => !table.PrimaryKey.Columns.Any(pk => pk.Name == c.Name))
                    .ToList();
                
                // Create the many-to-many relationship
                var relationship = new RelationshipModel
                {
                    SourceTable = foreignKey1.ReferencedTable,
                    TargetTable = foreignKey2.ReferencedTable,
                    Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToMany,
                    IsSelfReferencing = false,
                    JoinTable = table,
                    SourceForeignKey = foreignKey1,
                    TargetForeignKey = foreignKey2,
                    IsSourceCollection = true,
                    IsTargetCollection = true,
                    DeleteBehavior = ReferentialAction.Cascade // Default for junction tables
                };
                
                // Set navigation property names
                relationship.SourceNavigationPropertyName = GetNavigationPropertyName(relationship.TargetTable, true);
                relationship.TargetNavigationPropertyName = GetNavigationPropertyName(relationship.SourceTable, true);
                
                // Add payload properties
                foreach (var property in payloadProperties)
                {
                    relationship.PayloadPropertiesList.Add(property);
                }
                
                _relationships.Add(relationship);
            }
        }

        /// <summary>
        /// Detects inheritance relationships between tables.
        /// Inheritance is typically implemented as:
        /// - A foreign key that is also the primary key
        /// - The foreign key references another table's primary key
        /// - The column names match between parent and child table
        /// </summary>
        private void DetectInheritanceRelationships()
        {
            _logger.LogInformation("Detecting inheritance relationships...");
            
            // Get one-to-one relationships where the foreign key is the primary key
            var potentialInheritanceRelationships = _relationships
                .Where(r => r.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.OneToOne)
                .OfType<RelationshipModel>()
                .Where(r => r.ForeignKey != null && 
                           r.SourceTable.PrimaryKey != null && 
                           r.ForeignKey.Columns.Count == r.SourceTable.PrimaryKey.Columns.Count &&
                           r.ForeignKey.Columns.All(fc => 
                               r.SourceTable.PrimaryKey.Columns.Any(pc => pc.Name == fc.Name)))
                .ToList();
            
            foreach (var relationship in potentialInheritanceRelationships)
            {
                // Additional checks for inheritance relationships
                
                // Compare non-primary key columns between base and derived tables
                var baseTableNonPkColumns = relationship.TargetTable.PrimaryKey != null ? 
                    relationship.TargetTable.Columns
                        .Where(c => !relationship.TargetTable.PrimaryKey.Columns.Any(pk => pk.Name == c.Name))
                        .ToList() : 
                    relationship.TargetTable.Columns.ToList();
                
                var derivedTableNonPkColumns = relationship.SourceTable.PrimaryKey != null ?
                    relationship.SourceTable.Columns
                        .Where(c => !relationship.SourceTable.PrimaryKey.Columns.Any(pk => pk.Name == c.Name))
                        .ToList() :
                    relationship.SourceTable.Columns.ToList();
                
                var matchingColumnCount = baseTableNonPkColumns
                    .Count(bc => derivedTableNonPkColumns.Any(dc => dc.Name == bc.Name && dc.DataType == bc.DataType));
                
                // If there are matching columns, it's likely an inheritance relationship
                if (matchingColumnCount > 0 || baseTableNonPkColumns.Count == 0 || derivedTableNonPkColumns.Count == 0)
                {
                    relationship.Type = EzDbCodeGen.Schema.Interfaces.RelationshipType.Inheritance;
                    relationship.IsInheritance = true;
                    
                    // Identify TPH vs TPT inheritance
                    var inheritanceType = DetectInheritanceType(relationship);
                    relationship.InheritanceType = inheritanceType;
                    
                    // For TPH, try to identify the discriminator column
                    if (inheritanceType == Schema.Interfaces.InheritanceType.TablePerHierarchy)
                    {
                        // Look for discriminator column in base table (usually a string or int column with limited values)
                        var potentialDiscriminatorColumns = baseTableNonPkColumns
                            .Where(c => c.DataType == "nvarchar" || c.DataType == "varchar" || 
                                       c.DataType == "int" || c.DataType == "smallint" || 
                                       c.DataType == "tinyint" || c.DataType == "char" || 
                                       c.DataType == "nchar")
                            .ToList();
                        
                        // Heuristic: columns with specific names are likely discriminators
                        foreach (var column in potentialDiscriminatorColumns)
                        {
                            if (column.Name.IndexOf("discriminator", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                column.Name.IndexOf("type", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                column.Name.IndexOf("class", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                column.Name.IndexOf("category", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                relationship.DiscriminatorColumn = column;
                                
                                // Try to guess a discriminator value based on table name
                                relationship.DiscriminatorValue = relationship.SourceTable.Name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Detects the type of inheritance (TPH or TPT) between tables.
        /// </summary>
        private Schema.Interfaces.InheritanceType DetectInheritanceType(RelationshipModel relationship)
        {
            // Default to TPT (Table-per-Type) unless proven otherwise
            var inheritanceType = Schema.Interfaces.InheritanceType.TablePerType;
            
            // Check for signs of TPH (Table-per-Hierarchy):
            // 1. Base table has columns that seem to belong to derived tables
            // 2. Base table has a discriminator column
            
            // Look for columns in base table that seem specific to derived types
            var baseTableColumns = relationship.TargetTable.Columns
                .Select(c => c.Name.ToLowerInvariant())
                .ToList();
            
            // Check if derived table name or part of it appears in column names of base table
            var derivedTableNameParts = relationship.SourceTable.Name
                .Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.ToLowerInvariant())
                .Where(p => p.Length > 3) // Only consider meaningful name parts
                .ToList();
            
            foreach (var part in derivedTableNameParts)
            {
                if (baseTableColumns.Any(c => c.Contains(part)))
                {
                    inheritanceType = Schema.Interfaces.InheritanceType.TablePerHierarchy;
                    break;
                }
            }
            
            // Look for potential discriminator columns in base table
            if (inheritanceType != Schema.Interfaces.InheritanceType.TablePerHierarchy)
            {
                var discriminatorKeywords = new[] { "discriminator", "type", "class", "category", "kind" };
                
                if (baseTableColumns.Any(c => discriminatorKeywords.Any(k => c.Contains(k))))
                {
                    inheritanceType = Schema.Interfaces.InheritanceType.TablePerHierarchy;
                }
            }
            
            return inheritanceType;
        }

        /// <summary>
        /// Marks tables as junction tables if they participate in many-to-many relationships.
        /// </summary>
        private void MarkJunctionTables()
        {
            var junctionTables = new HashSet<ITable>();
            
            // Find all junction tables used in many-to-many relationships
            foreach (var relationship in _relationships)
            {
                if (relationship.Type == EzDbCodeGen.Schema.Interfaces.RelationshipType.ManyToMany && relationship is RelationshipModel model && model.JoinTable != null)
                {
                    junctionTables.Add(model.JoinTable);
                }
            }
            
            // Mark junction tables
            foreach (var table in junctionTables)
            {
                // Mark table as junction table if possible
                if (table is TableModel tableModel)
                {
                    tableModel.IsJunctionTable = true;
                }
            }
        }

        /// <summary>
        /// Generates a navigation property name for a relationship.
        /// </summary>
        /// <param name="table">The table to generate a name for</param>
        /// <param name="isCollection">Whether this is a collection navigation</param>
        /// <returns>A suitable navigation property name</returns>
        private string GetNavigationPropertyName(ITable table, bool isCollection)
        {
            if (table == null)
                return string.Empty;
            
            // Start with the table name
            string name = table.Name;
            
            // Remove common table prefixes
            if (name.StartsWith("tbl", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(3);
            else if (name.StartsWith("t_", StringComparison.OrdinalIgnoreCase))
                name = name.Substring(2);
            
            // Remove schema prefix if present
            if (name.Contains("."))
                name = name.Split('.').Last();
            
            // Remove underscores and capitalize each word
            if (name.Contains("_"))
            {
                var parts = name.Split('_');
                name = string.Join("", parts.Select(p => 
                    string.IsNullOrEmpty(p) ? "" : char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p.Substring(1) : "")));
            }
            else
            {
                // Just capitalize the first letter
                name = char.ToUpperInvariant(name[0]) + (name.Length > 1 ? name.Substring(1) : "");
            }
            
            // For collections, pluralize the name
            if (isCollection)
            {
                // Handle common English pluralization rules
                if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase) && 
                    !name.EndsWith("ay", StringComparison.OrdinalIgnoreCase) && 
                    !name.EndsWith("ey", StringComparison.OrdinalIgnoreCase) && 
                    !name.EndsWith("iy", StringComparison.OrdinalIgnoreCase) && 
                    !name.EndsWith("oy", StringComparison.OrdinalIgnoreCase) && 
                    !name.EndsWith("uy", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(0, name.Length - 1) + "ies";
                }
                else if (name.EndsWith("s", StringComparison.OrdinalIgnoreCase) || 
                         name.EndsWith("x", StringComparison.OrdinalIgnoreCase) || 
                         name.EndsWith("z", StringComparison.OrdinalIgnoreCase) || 
                         name.EndsWith("ch", StringComparison.OrdinalIgnoreCase) || 
                         name.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
                {
                    name += "es";
                }
                else
                {
                    name += "s";
                }
            }
            
            return name;
        }
    }

    /// <summary>
    /// Model class representing a detected relationship.
    /// </summary>
    public class RelationshipModel : IRelationship
    {
        /// <summary>
        /// Gets or sets the source table of the relationship.
        /// </summary>
        public ITable? SourceTable { get; set; }

        /// <summary>
        /// Gets or sets the target table of the relationship.
        /// </summary>
        public ITable? TargetTable { get; set; }

        /// <summary>
        /// Gets or sets the type of the relationship.
        /// </summary>
        public EzDbCodeGen.Schema.Interfaces.RelationshipType Type { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the relationship is self-referencing.
        /// </summary>
        public bool IsSelfReferencing { get; set; }

        /// <summary>
        /// Gets or sets the foreign key that defines this relationship.
        /// </summary>
        public IForeignKey? ForeignKey { get; set; }

        /// <summary>
        /// Gets or sets the source foreign key for a many-to-many relationship.
        /// </summary>
        public IForeignKey? SourceForeignKey { get; set; }

        /// <summary>
        /// Gets or sets the target foreign key for a many-to-many relationship.
        /// </summary>
        public IForeignKey? TargetForeignKey { get; set; }

        /// <summary>
        /// Gets or sets the junction table for a many-to-many relationship.
        /// </summary>
        public ITable? JoinTable { get; set; }

        /// <summary>
        /// Gets or sets the inheritance type for an inheritance relationship.
        /// </summary>
        public Schema.Interfaces.InheritanceType? InheritanceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an inheritance relationship.
        /// </summary>
        public bool IsInheritance { get; set; }

        /// <summary>
        /// Gets or sets the name of the navigation property in the source table.
        /// </summary>
        public string SourceNavigationPropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the navigation property in the target table.
        /// </summary>
        public string TargetNavigationPropertyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the source navigation property is a collection.
        /// </summary>
        public bool IsSourceCollection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the target navigation property is a collection.
        /// </summary>
        public bool IsTargetCollection { get; set; }

        /// <summary>
        /// Gets or sets the delete behavior for this relationship.
        /// </summary>
        public ReferentialAction DeleteBehavior { get; set; } = ReferentialAction.NoAction;

        /// <summary>
        /// Gets or sets the discriminator column for TPH inheritance relationships.
        /// </summary>
        public IColumn? DiscriminatorColumn { get; set; }

        /// <summary>
        /// Gets or sets the discriminator value for the derived table in TPH inheritance relationships.
        /// </summary>
        public string? DiscriminatorValue { get; set; }

        /// <summary>
        /// Gets the collection of properties in the source table that participate in the relationship.
        /// </summary>
        public IReadOnlyCollection<IColumn> SourceProperties => SourcePropertiesList.AsReadOnly();

        /// <summary>
        /// Gets the collection of properties in the target table that participate in the relationship.
        /// </summary>
        public IReadOnlyCollection<IColumn> TargetProperties => TargetPropertiesList.AsReadOnly();

        /// <summary>
        /// Gets the collection of payload properties for a many-to-many relationship.
        /// </summary>
        public IReadOnlyCollection<IColumn> PayloadProperties => PayloadPropertiesList.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether the relationship has payload properties.
        /// </summary>
        public bool HasPayload => PayloadPropertiesList.Count > 0;

        // Internal lists for property management
        internal List<IColumn> SourcePropertiesList { get; } = new List<IColumn>();
        internal List<IColumn> TargetPropertiesList { get; } = new List<IColumn>();
        internal List<IColumn> PayloadPropertiesList { get; } = new List<IColumn>();
    }
}
