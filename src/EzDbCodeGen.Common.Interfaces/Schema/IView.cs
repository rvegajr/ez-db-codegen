using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema;

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
}
