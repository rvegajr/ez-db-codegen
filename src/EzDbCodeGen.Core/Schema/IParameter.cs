namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a parameter in a stored procedure or function.
/// </summary>
public interface IParameter
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the ordinal position of the parameter.
    /// </summary>
    int OrdinalPosition { get; }
    
    /// <summary>
    /// Gets the data type of the parameter.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the parameter allows null values.
    /// </summary>
    bool IsNullable { get; }
    
    /// <summary>
    /// Gets the maximum length of the parameter.
    /// </summary>
    int? MaxLength { get; }
    
    /// <summary>
    /// Gets the precision of the parameter.
    /// </summary>
    int? Precision { get; }
    
    /// <summary>
    /// Gets the scale of the parameter.
    /// </summary>
    int? Scale { get; }
    
    /// <summary>
    /// Gets a value indicating whether the parameter is an output parameter.
    /// </summary>
    bool IsOutput { get; }
    
    /// <summary>
    /// Gets the default value of the parameter, if any.
    /// </summary>
    string? DefaultValue { get; }
}
