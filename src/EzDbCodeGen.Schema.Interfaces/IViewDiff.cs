namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a comparison between two database views.
/// </summary>
public interface IViewDiff
{
    /// <summary>
    /// Gets the original view.
    /// </summary>
    IView? Original { get; }
    
    /// <summary>
    /// Gets the new view.
    /// </summary>
    IView? New { get; }
    
    /// <summary>
    /// Gets a value indicating whether the view has changed.
    /// </summary>
    bool HasChanged { get; }
    
    /// <summary>
    /// Gets the columns that were added to the view.
    /// </summary>
    IReadOnlyCollection<IViewColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the view.
    /// </summary>
    IReadOnlyCollection<IViewColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the view definition has changed.
    /// </summary>
    bool DefinitionChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the view indexing has changed.
    /// </summary>
    bool IsIndexedChanged { get; }
}
