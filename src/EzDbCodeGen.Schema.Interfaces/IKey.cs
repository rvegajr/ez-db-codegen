namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a key constraint in a database table.
/// </summary>
public interface IKey
{
    /// <summary>
    /// Gets the name of the key.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the columns that make up the key.
    /// </summary>
    IReadOnlyList<IColumn> Columns { get; }
    
    /// <summary>
    /// Gets the table that the key belongs to.
    /// </summary>
    ITable Table { get; }
    
    /// <summary>
    /// Gets a value indicating whether the key is clustered.
    /// </summary>
    bool IsClustered { get; }
}
