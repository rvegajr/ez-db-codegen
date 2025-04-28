namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents an index in a database table.
/// </summary>
public interface IIndex
{
    /// <summary>
    /// Gets the name of the index.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the table that the index belongs to.
    /// </summary>
    ITable Table { get; }
    
    /// <summary>
    /// Gets the columns that make up the index.
    /// </summary>
    IReadOnlyList<IIndexColumn> Columns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the index is unique.
    /// </summary>
    bool IsUnique { get; }
    
    /// <summary>
    /// Gets a value indicating whether the index is clustered.
    /// </summary>
    bool IsClustered { get; }
    
    /// <summary>
    /// Gets the filter expression for the index, if any.
    /// </summary>
    string? Filter { get; }
}
