namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a database view.
/// </summary>
public interface IView
{
    /// <summary>
    /// Gets the name of the view.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the schema of the view.
    /// </summary>
    string Schema { get; }
    
    /// <summary>
    /// Gets the columns in the view.
    /// </summary>
    IReadOnlyCollection<IViewColumn> Columns { get; }
    
    /// <summary>
    /// Gets the definition of the view.
    /// </summary>
    string? Definition { get; }
    
    /// <summary>
    /// Gets a value indicating whether the view is indexed.
    /// </summary>
    bool IsIndexed { get; }
}
