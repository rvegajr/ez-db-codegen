namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a column in an index.
/// </summary>
public interface IIndexColumn
{
    /// <summary>
    /// Gets the column that is part of the index.
    /// </summary>
    IColumn Column { get; }
    
    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the ordinal position of the column in the index.
    /// </summary>
    int OrdinalPosition { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is in descending order in the index.
    /// </summary>
    bool IsDescending { get; }
}
