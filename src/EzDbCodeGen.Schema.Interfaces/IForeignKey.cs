namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a foreign key constraint in a database table.
/// </summary>
public interface IForeignKey
{
    /// <summary>
    /// Gets the name of the foreign key.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the table that the foreign key belongs to.
    /// </summary>
    ITable Table { get; }
    
    /// <summary>
    /// Gets the columns that make up the foreign key.
    /// </summary>
    IReadOnlyList<IColumn> Columns { get; }
    
    /// <summary>
    /// Gets the referenced table.
    /// </summary>
    ITable ReferencedTable { get; }
    
    /// <summary>
    /// Gets the referenced columns.
    /// </summary>
    IReadOnlyList<IColumn> ReferencedColumns { get; }
    
    /// <summary>
    /// Gets the delete action for the foreign key.
    /// </summary>
    ReferentialAction DeleteAction { get; }
    
    /// <summary>
    /// Gets the update action for the foreign key.
    /// </summary>
    ReferentialAction UpdateAction { get; }
}
