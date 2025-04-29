namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a unique constraint in a database table.
/// </summary>
public interface IUniqueConstraint
{
    /// <summary>
    /// Gets the name of the unique constraint.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the table that the unique constraint belongs to.
    /// </summary>
    ITable Table { get; }
    
    /// <summary>
    /// Gets the columns that make up the unique constraint.
    /// </summary>
    IReadOnlyList<IColumn> Columns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the unique constraint is clustered.
    /// </summary>
    bool IsClustered { get; }
}
