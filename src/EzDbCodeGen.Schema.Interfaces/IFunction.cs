namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a user-defined function in a database.
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
    /// Gets the return type of the function.
    /// </summary>
    string ReturnType { get; }
    
    /// <summary>
    /// Gets the columns returned by the function, if it's a table-valued function.
    /// </summary>
    IReadOnlyCollection<IResultColumn>? ResultColumns { get; }
    
    /// <summary>
    /// Gets a value indicating whether the function is a table-valued function.
    /// </summary>
    bool IsTableValued { get; }
    
    /// <summary>
    /// Gets the definition of the function.
    /// </summary>
    string? Definition { get; }
}
