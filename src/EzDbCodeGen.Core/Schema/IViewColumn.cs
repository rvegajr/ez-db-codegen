namespace EzDbCodeGen.Core.Schema;

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
    /// Gets the ordinal position of the column in the view.
    /// </summary>
    int OrdinalPosition { get; }
    
    /// <summary>
    /// Gets the data type of the column.
    /// </summary>
    string DataType { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column allows null values.
    /// </summary>
    bool IsNullable { get; }
    
    /// <summary>
    /// Gets the maximum length of the column.
    /// </summary>
    int? MaxLength { get; }
    
    /// <summary>
    /// Gets the precision of the column.
    /// </summary>
    int? Precision { get; }
    
    /// <summary>
    /// Gets the scale of the column.
    /// </summary>
    int? Scale { get; }
    
    /// <summary>
    /// Gets the view that the column belongs to.
    /// </summary>
    IView View { get; }
}
