using System.Collections.Generic;

namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a comparison between two foreign keys.
/// </summary>
public interface IForeignKeyDiff
{
    /// <summary>
    /// Gets the original foreign key.
    /// </summary>
    IForeignKey Original { get; }
    
    /// <summary>
    /// Gets the new foreign key.
    /// </summary>
    IForeignKey New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the foreign key has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the foreign key.
    /// </summary>
    IReadOnlyCollection<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the foreign key.
    /// </summary>
    IReadOnlyCollection<IColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the referenced table has changed.
    /// </summary>
    bool ReferencedTableChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the referenced columns have changed.
    /// </summary>
    bool ReferencedColumnsChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the delete rule has changed.
    /// </summary>
    bool DeleteRuleChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the update rule has changed.
    /// </summary>
    bool UpdateRuleChanged { get; }
}
