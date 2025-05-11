using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a relationship between two tables in a database.
/// </summary>
public interface IRelationship
{
    /// <summary>
    /// Gets the name of the relationship.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the parent table of the relationship.
    /// </summary>
    ITable ParentTable { get; }
    
    /// <summary>
    /// Gets the child table of the relationship.
    /// </summary>
    ITable ChildTable { get; }
    
    /// <summary>
    /// Gets the column mappings of the relationship.
    /// </summary>
    IReadOnlyCollection<IForeignKeyColumnMapping> ColumnMappings { get; }
}
