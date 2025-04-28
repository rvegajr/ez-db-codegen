namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a column in the result set of a stored procedure or a table-valued function.
/// </summary>
public interface IResultColumn
{
    /// <summary>
    /// Gets the name of the result column.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the ordinal position of the result column.
    /// </summary>
    int OrdinalPosition { get; }
    
    /// <summary>
    /// Gets the data type of the result column.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the result column allows null values.
    /// </summary>
    bool IsNullable { get; }
    
    /// <summary>
    /// Gets the maximum length of the result column.
    /// </summary>
    int? MaxLength { get; }
    
    /// <summary>
    /// Gets the precision of the result column.
    /// </summary>
    int? Precision { get; }
    
    /// <summary>
    /// Gets the scale of the result column.
    /// </summary>
    int? Scale { get; }
}
