namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two foreign keys.
/// </summary>
public interface IForeignKeyDiff
{
    /// <summary>
    /// Gets the source foreign key.
    /// </summary>
    IForeignKey? Source { get; }

    /// <summary>
    /// Gets the target foreign key.
    /// </summary>
    IForeignKey? Target { get; }

    /// <summary>
    /// Gets the original foreign key.
    /// </summary>
    IForeignKey? Original { get; }
    
    /// <summary>
    /// Gets the new foreign key.
    /// </summary>
    IForeignKey? New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the foreign key has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the name has changed.
    /// </summary>
    bool NameChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the delete behavior has changed.
    /// </summary>
    bool DeleteBehaviorChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the update behavior has changed.
    /// </summary>
    bool UpdateBehaviorChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the foreign key.
    /// </summary>
    IReadOnlyCollection<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the foreign key.
    /// </summary>
    IReadOnlyCollection<IColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets the column mappings that were added to the foreign key.
    /// </summary>
    IReadOnlyCollection<IForeignKeyColumnMapping> AddedMappings { get; }
    
    /// <summary>
    /// Gets the column mappings that were removed from the foreign key.
    /// </summary>
    IReadOnlyCollection<IForeignKeyColumnMapping> RemovedMappings { get; }
    
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
