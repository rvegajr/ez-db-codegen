namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a mapping between a parent column and a child column in a foreign key relationship.
/// </summary>
public interface IForeignKeyColumnMapping
{
    /// <summary>
    /// Gets the parent column of the mapping.
    /// </summary>
    IColumn ParentColumn { get; }
    
    /// <summary>
    /// Gets the child column of the mapping.
    /// </summary>
    IColumn ChildColumn { get; }
}
