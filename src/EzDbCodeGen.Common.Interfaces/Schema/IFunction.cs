using System.Collections.Generic;

namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a database function.
/// </summary>
public interface IFunction
{
    /// <summary>
    /// Gets the name of the function.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the schema of the function.
    /// </summary>
    string Schema { get; }
    
    /// <summary>
    /// Gets the parameters of the function.
    /// </summary>
    IReadOnlyCollection<IParameter> Parameters { get; }
    
    /// <summary>
    /// Gets the result columns of the function.
    /// </summary>
    IReadOnlyCollection<IResultColumn> ResultColumns { get; }
    
    /// <summary>
    /// Gets the return type of the function.
    /// </summary>
    string ReturnType { get; }
}
