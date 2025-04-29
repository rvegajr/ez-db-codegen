using System.Collections.Generic;

namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a comparison between two unique constraints.
/// </summary>
public interface IUniqueConstraintDiff
{
    /// <summary>
    /// Gets the original unique constraint.
    /// </summary>
    IUniqueConstraint Original { get; }
    
    /// <summary>
    /// Gets the new unique constraint.
    /// </summary>
    IUniqueConstraint New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the unique constraint has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the unique constraint.
    /// </summary>
    IReadOnlyCollection<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the unique constraint.
    /// </summary>
    IReadOnlyCollection<IColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the clustering of the unique constraint has changed.
    /// </summary>
    bool IsClusteredChanged { get; }
}
