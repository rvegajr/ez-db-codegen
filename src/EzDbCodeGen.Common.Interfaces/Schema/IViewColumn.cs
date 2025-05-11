namespace EzDbCodeGen.Common.Interfaces.Schema;

/// <summary>
/// Represents a column in a database view.
/// </summary>
public interface IViewColumn
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
}
