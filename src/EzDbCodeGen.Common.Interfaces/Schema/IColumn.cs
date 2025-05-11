namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a column in a database table.
/// </summary>
public interface IColumn
{
    /// <summary>
    /// Gets the name of the column.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the data type of the column.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is nullable.
    /// </summary>
    bool IsNullable { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is a primary key.
    /// </summary>
    bool IsPrimaryKey { get; }
}
