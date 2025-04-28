namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two keys.
/// </summary>
public interface IKeyDiff
{
    /// <summary>
    /// Gets the original key.
    /// </summary>
    IKey Original { get; }
    
    /// <summary>
    /// Gets the new key.
    /// </summary>
    IKey New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the key has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the key.
    /// </summary>
    IReadOnlyCollection<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the key.
    /// </summary>
    IReadOnlyCollection<IColumn> RemovedColumns { get; }
}
