using System.Collections.Generic;

namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a comparison between two indexes.
/// </summary>
public interface IIndexDiff
{
    /// <summary>
    /// Gets the original index.
    /// </summary>
    IIndex Original { get; }
    
    /// <summary>
    /// Gets the new index.
    /// </summary>
    IIndex New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the index has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the index.
    /// </summary>
    IReadOnlyCollection<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the index.
    /// </summary>
    IReadOnlyCollection<IColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the uniqueness of the index has changed.
    /// </summary>
    bool IsUniqueChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the clustering of the index has changed.
    /// </summary>
    bool IsClusteredChanged { get; }
}
