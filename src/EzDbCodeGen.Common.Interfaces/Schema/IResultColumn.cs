namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a result column of a stored procedure or function.
/// </summary>
public interface IResultColumn
{
    /// <summary>
    /// Gets the name of the result column.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the data type of the result column.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the result column is nullable.
    /// </summary>
    bool IsNullable { get; }
}
