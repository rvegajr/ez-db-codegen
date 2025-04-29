using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Analysis;

/// <summary>
/// Analyzes database relationships to detect complex patterns.
/// </summary>
public class RelationshipAnalyzer
{
    private readonly ILogger? _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipAnalyzer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public RelationshipAnalyzer(ILogger? logger = null)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Analyzes a database schema to detect complex relationships.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of detected relationships.</returns>
    public IEnumerable<RelationshipInfo> AnalyzeRelationships(IDatabaseSchema schema)
    {
        if (schema == null)
        {
            throw new ArgumentNullException(nameof(schema));
        }
        
        var relationships = new List<RelationshipInfo>();
        
        // Detect many-to-many relationships via junction tables
        var manyToManyRelationships = DetectManyToManyRelationships(schema);
        relationships.AddRange(manyToManyRelationships);
        
        // Detect one-to-one relationships
        var oneToOneRelationships = DetectOneToOneRelationships(schema);
        relationships.AddRange(oneToOneRelationships);
        
        // Detect self-referencing relationships
        var selfReferencingRelationships = DetectSelfReferencingRelationships(schema);
        relationships.AddRange(selfReferencingRelationships);
        
        // Detect inheritance patterns (TPH/TPT)
        var inheritanceRelationships = DetectInheritanceRelationships(schema);
        relationships.AddRange(inheritanceRelationships);
        
        return relationships;
    }
    
    /// <summary>
    /// Detects many-to-many relationships in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of many-to-many relationships.</returns>
    public IEnumerable<RelationshipInfo> DetectManyToManyRelationships(IDatabaseSchema schema)
    {
        var manyToManyRelationships = new List<RelationshipInfo>();
        
        // Look for junction tables (tables that have exactly two foreign keys)
        foreach (var table in schema.Tables)
        {
            // Skip tables with no foreign keys or with more than 2 foreign keys
            var foreignKeys = table.ForeignKeys.ToList();
            if (foreignKeys.Count != 2)
            {
                continue;
            }
            
            // Check if this might be a junction table
            if (IsJunctionTable(table, out var fk1, out var fk2))
            {
                _logger?.LogDebug("Found potential junction table: {TableName}", table.Name);
                
                // Create the many-to-many relationship
                var relationship = new RelationshipInfo
                {
                    Type = RelationshipType.ManyToMany,
                    SourceTable = fk1.ReferencedTable,
                    TargetTable = fk2.ReferencedTable,
                    JunctionTable = table,
                    SourceToJunctionForeignKey = fk1,
                    JunctionToTargetForeignKey = fk2,
                    Name = $"ManyToMany_{fk1.ReferencedTable.Name}_{fk2.ReferencedTable.Name}"
                };
                
                manyToManyRelationships.Add(relationship);
                _logger?.LogInformation("Detected many-to-many relationship between {SourceTable} and {TargetTable} via junction table {JunctionTable}",
                    fk1.ReferencedTable.Name, fk2.ReferencedTable.Name, table.Name);
            }
        }
        
        return manyToManyRelationships;
    }
    
    /// <summary>
    /// Determines whether a table is a junction table.
    /// </summary>
    /// <param name="table">The table to check.</param>
    /// <param name="sourceToJunctionFk">The foreign key from the source to the junction table.</param>
    /// <param name="junctionToTargetFk">The foreign key from the junction table to the target table.</param>
    /// <returns>True if the table is a junction table; otherwise, false.</returns>
    private bool IsJunctionTable(ITable table, out IForeignKey sourceToJunctionFk, out IForeignKey junctionToTargetFk)
    {
        sourceToJunctionFk = null;
        junctionToTargetFk = null;
        
        var foreignKeys = table.ForeignKeys.ToList();
        if (foreignKeys.Count != 2)
        {
            return false;
        }
        
        // Check if the table has only foreign key columns plus maybe some additional metadata columns
        var nonKeyColumns = table.Columns.Count - foreignKeys.Sum(fk => fk.Columns.Count);
        if (nonKeyColumns > 3) // Allow a few extra metadata columns (like CreatedDate, etc.)
        {
            _logger?.LogDebug("Table {TableName} has {Count} non-key columns, may not be a junction table", 
                table.Name, nonKeyColumns);
            return false;
        }
        
        // Check if the foreign key columns form the primary key (composite or not)
        if (table.PrimaryKey != null)
        {
            var primaryKeyColumns = table.PrimaryKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var foreignKeyColumns = foreignKeys.SelectMany(fk => fk.Columns).Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            // If primary key is not a subset of the foreign key columns, it's likely not a junction table
            if (!primaryKeyColumns.IsSubsetOf(foreignKeyColumns) && primaryKeyColumns.Count == foreignKeyColumns.Count)
            {
                _logger?.LogDebug("Table {TableName} primary key is not composed of foreign key columns, may not be a junction table", 
                    table.Name);
                return false;
            }
        }
        
        // If we get here, it's likely a junction table
        sourceToJunctionFk = foreignKeys[0];
        junctionToTargetFk = foreignKeys[1];
        return true;
    }
    
    /// <summary>
    /// Detects one-to-one relationships in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of one-to-one relationships.</returns>
    public IEnumerable<RelationshipInfo> DetectOneToOneRelationships(IDatabaseSchema schema)
    {
        var oneToOneRelationships = new List<RelationshipInfo>();
        
        foreach (var table in schema.Tables)
        {
            foreach (var foreignKey in table.ForeignKeys)
            {
                // Check if this foreign key forms a one-to-one relationship
                // A one-to-one relationship typically has a unique constraint or is a primary key 
                // on the foreign key columns
                if (IsOneToOneRelationship(foreignKey))
                {
                    var relationship = new RelationshipInfo
                    {
                        Type = RelationshipType.OneToOne,
                        SourceTable = foreignKey.Table,
                        TargetTable = foreignKey.ReferencedTable,
                        ForeignKey = foreignKey,
                        Name = $"OneToOne_{foreignKey.Table.Name}_{foreignKey.ReferencedTable.Name}"
                    };
                    
                    oneToOneRelationships.Add(relationship);
                    _logger?.LogInformation("Detected one-to-one relationship between {SourceTable} and {TargetTable}",
                        foreignKey.Table.Name, foreignKey.ReferencedTable.Name);
                }
            }
        }
        
        return oneToOneRelationships;
    }
    
    /// <summary>
    /// Determines whether a foreign key represents a one-to-one relationship.
    /// </summary>
    /// <param name="foreignKey">The foreign key to check.</param>
    /// <returns>True if the foreign key forms a one-to-one relationship; otherwise, false.</returns>
    private bool IsOneToOneRelationship(IForeignKey foreignKey)
    {
        // Check if the foreign key columns form the primary key
        if (foreignKey.Table.PrimaryKey != null)
        {
            var primaryKeyColumns = foreignKey.Table.PrimaryKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var foreignKeyColumns = foreignKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            if (primaryKeyColumns.SetEquals(foreignKeyColumns))
            {
                _logger?.LogDebug("Foreign key {ForeignKey} columns form the primary key of {TableName}, indicating a one-to-one relationship", 
                    foreignKey.Name, foreignKey.Table.Name);
                return true;
            }
        }
        
        // Check if the foreign key columns have a unique constraint
        foreach (var uniqueConstraint in foreignKey.Table.UniqueConstraints)
        {
            var uniqueColumns = uniqueConstraint.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var foreignKeyColumns = foreignKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            if (uniqueColumns.SetEquals(foreignKeyColumns))
            {
                _logger?.LogDebug("Foreign key {ForeignKey} columns have a unique constraint in {TableName}, indicating a one-to-one relationship", 
                    foreignKey.Name, foreignKey.Table.Name);
                return true;
            }
        }
        
        // Check if the foreign key columns have a unique index
        foreach (var index in foreignKey.Table.Indexes.Where(i => i.IsUnique))
        {
            var indexColumns = index.Columns.Select(c => c.Column.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var foreignKeyColumns = foreignKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            if (indexColumns.SetEquals(foreignKeyColumns))
            {
                _logger?.LogDebug("Foreign key {ForeignKey} columns have a unique index in {TableName}, indicating a one-to-one relationship", 
                    foreignKey.Name, foreignKey.Table.Name);
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Detects self-referencing relationships in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of self-referencing relationships.</returns>
    public IEnumerable<RelationshipInfo> DetectSelfReferencingRelationships(IDatabaseSchema schema)
    {
        var selfReferencingRelationships = new List<RelationshipInfo>();
        
        foreach (var table in schema.Tables)
        {
            foreach (var foreignKey in table.ForeignKeys)
            {
                // Check if this is a self-referencing relationship (foreign key references the same table)
                if (foreignKey.Table == foreignKey.ReferencedTable)
                {
                    var relationship = new RelationshipInfo
                    {
                        Type = RelationshipType.SelfReferencing,
                        SourceTable = foreignKey.Table,
                        TargetTable = foreignKey.ReferencedTable,
                        ForeignKey = foreignKey,
                        Name = $"SelfRef_{foreignKey.Table.Name}_{foreignKey.Name}"
                    };
                    
                    selfReferencingRelationships.Add(relationship);
                    _logger?.LogInformation("Detected self-referencing relationship in table {TableName} via foreign key {ForeignKey}",
                        foreignKey.Table.Name, foreignKey.Name);
                }
            }
        }
        
        return selfReferencingRelationships;
    }
    
    /// <summary>
    /// Detects inheritance relationships (TPH/TPT) in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of inheritance relationships.</returns>
    public IEnumerable<RelationshipInfo> DetectInheritanceRelationships(IDatabaseSchema schema)
    {
        var inheritanceRelationships = new List<RelationshipInfo>();
        
        // Detect Table-Per-Hierarchy (TPH) pattern - typically uses a discriminator column
        var tphRelationships = DetectTablePerHierarchyRelationships(schema);
        inheritanceRelationships.AddRange(tphRelationships);
        
        // Detect Table-Per-Type (TPT) pattern - typically uses foreign keys with same primary key values
        var tptRelationships = DetectTablePerTypeRelationships(schema);
        inheritanceRelationships.AddRange(tptRelationships);
        
        return inheritanceRelationships;
    }
    
    /// <summary>
    /// Detects Table-Per-Hierarchy (TPH) inheritance pattern in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of TPH inheritance relationships.</returns>
    private IEnumerable<RelationshipInfo> DetectTablePerHierarchyRelationships(IDatabaseSchema schema)
    {
        var tphRelationships = new List<RelationshipInfo>();
        
        // Look for tables with a discriminator column (common names: Discriminator, Type, Kind, etc.)
        foreach (var table in schema.Tables)
        {
            var discriminatorColumn = table.Columns.FirstOrDefault(c => 
                string.Equals(c.Name, "Discriminator", StringComparison.OrdinalIgnoreCase) || 
                string.Equals(c.Name, "Type", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Name, "Kind", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.Name, "EntityType", StringComparison.OrdinalIgnoreCase));
            
            if (discriminatorColumn != null)
            {
                _logger?.LogDebug("Found potential TPH table {TableName} with discriminator column {ColumnName}", 
                    table.Name, discriminatorColumn.Name);
                
                var relationship = new RelationshipInfo
                {
                    Type = RelationshipType.TablePerHierarchy,
                    SourceTable = table, // Base table
                    DiscriminatorColumn = discriminatorColumn,
                    Name = $"TPH_{table.Name}"
                };
                
                tphRelationships.Add(relationship);
                _logger?.LogInformation("Detected Table-Per-Hierarchy inheritance pattern in table {TableName} with discriminator column {ColumnName}",
                    table.Name, discriminatorColumn.Name);
            }
        }
        
        return tphRelationships;
    }
    
    /// <summary>
    /// Detects Table-Per-Type (TPT) inheritance pattern in the database schema.
    /// </summary>
    /// <param name="schema">The database schema to analyze.</param>
    /// <returns>A collection of TPT inheritance relationships.</returns>
    private IEnumerable<RelationshipInfo> DetectTablePerTypeRelationships(IDatabaseSchema schema)
    {
        var tptRelationships = new List<RelationshipInfo>();
        
        // Look for tables with foreign keys that reference another table's primary key
        // and where the foreign key is also the primary key of the child table
        foreach (var table in schema.Tables)
        {
            foreach (var foreignKey in table.ForeignKeys)
            {
                // TPT pattern typically has:
                // 1. The foreign key referencing another table's primary key
                // 2. The foreign key is the primary key of the child table
                // 3. The child table has additional columns representing subtype-specific properties
                if (IsTablePerTypeRelationship(foreignKey))
                {
                    var relationship = new RelationshipInfo
                    {
                        Type = RelationshipType.TablePerType,
                        SourceTable = foreignKey.ReferencedTable, // Base/parent table
                        TargetTable = foreignKey.Table, // Derived/child table
                        ForeignKey = foreignKey,
                        Name = $"TPT_{foreignKey.ReferencedTable.Name}_{foreignKey.Table.Name}"
                    };
                    
                    tptRelationships.Add(relationship);
                    _logger?.LogInformation("Detected Table-Per-Type inheritance pattern between base table {BaseTable} and derived table {DerivedTable}",
                        foreignKey.ReferencedTable.Name, foreignKey.Table.Name);
                }
            }
        }
        
        return tptRelationships;
    }
    
    /// <summary>
    /// Determines whether a foreign key represents a Table-Per-Type (TPT) relationship.
    /// </summary>
    /// <param name="foreignKey">The foreign key to check.</param>
    /// <returns>True if the foreign key forms a TPT relationship; otherwise, false.</returns>
    private bool IsTablePerTypeRelationship(IForeignKey foreignKey)
    {
        // Check if this foreign key references another table's primary key
        if (foreignKey.ReferencedTable.PrimaryKey == null)
        {
            return false;
        }
        
        // Check if the referenced columns match the primary key columns of the referenced table
        var primaryKeyColumns = foreignKey.ReferencedTable.PrimaryKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var referencedColumns = foreignKey.ReferencedColumns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        if (!primaryKeyColumns.SetEquals(referencedColumns))
        {
            return false;
        }
        
        // Check if the foreign key columns form the primary key of the child table
        if (foreignKey.Table.PrimaryKey == null)
        {
            return false;
        }
        
        var childPrimaryKeyColumns = foreignKey.Table.PrimaryKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var foreignKeyColumns = foreignKey.Columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        return childPrimaryKeyColumns.SetEquals(foreignKeyColumns);
    }
}

/// <summary>
/// Represents a detected relationship in the database schema.
/// </summary>
public class RelationshipInfo
{
    /// <summary>
    /// Gets or sets the type of the relationship.
    /// </summary>
    public RelationshipType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the relationship.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the source table of the relationship.
    /// </summary>
    public ITable SourceTable { get; set; }
    
    /// <summary>
    /// Gets or sets the target table of the relationship.
    /// </summary>
    public ITable TargetTable { get; set; }
    
    /// <summary>
    /// Gets or sets the junction table for many-to-many relationships.
    /// </summary>
    public ITable JunctionTable { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key for the relationship.
    /// </summary>
    public IForeignKey ForeignKey { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key from the source to the junction table (for many-to-many relationships).
    /// </summary>
    public IForeignKey SourceToJunctionForeignKey { get; set; }
    
    /// <summary>
    /// Gets or sets the foreign key from the junction table to the target table (for many-to-many relationships).
    /// </summary>
    public IForeignKey JunctionToTargetForeignKey { get; set; }
    
    /// <summary>
    /// Gets or sets the discriminator column for Table-Per-Hierarchy relationships.
    /// </summary>
    public IColumn DiscriminatorColumn { get; set; }
    
    /// <summary>
    /// Gets a string representation of the relationship.
    /// </summary>
    /// <returns>A string representation of the relationship.</returns>
    public override string ToString()
    {
        return $"{Type} relationship: {Name}";
    }
    
    /// <summary>
    /// Gets a descriptive string with details about the relationship.
    /// </summary>
    /// <returns>A descriptive string with details about the relationship.</returns>
    public string GetDescription()
    {
        switch (Type)
        {
            case RelationshipType.OneToMany:
                return $"One-to-many relationship from {SourceTable.GetFullName()} to {TargetTable.GetFullName()} via foreign key {ForeignKey.Name}";
                
            case RelationshipType.OneToOne:
                return $"One-to-one relationship between {SourceTable.GetFullName()} and {TargetTable.GetFullName()} via foreign key {ForeignKey.Name}";
                
            case RelationshipType.ManyToMany:
                return $"Many-to-many relationship between {SourceTable.GetFullName()} and {TargetTable.GetFullName()} via junction table {JunctionTable.GetFullName()}";
                
            case RelationshipType.SelfReferencing:
                return $"Self-referencing relationship in {SourceTable.GetFullName()} via foreign key {ForeignKey.Name}";
                
            case RelationshipType.TablePerHierarchy:
                return $"Table-Per-Hierarchy inheritance in {SourceTable.GetFullName()} with discriminator column {DiscriminatorColumn.Name}";
                
            case RelationshipType.TablePerType:
                return $"Table-Per-Type inheritance from base {SourceTable.GetFullName()} to derived {TargetTable.GetFullName()}";
                
            default:
                return $"Unknown relationship type: {Type}";
        }
    }
}
