namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a parameter of a stored procedure or function.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the data type of the parameter.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the parameter is an output parameter.
    /// </summary>
    bool IsOutput { get; }
    
    /// <summary>
    /// Gets a value indicating whether the parameter is nullable.
    /// </summary>
    bool IsNullable { get; }
}
