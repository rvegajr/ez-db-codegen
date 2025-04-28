namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a database table with columns, keys, and constraints.
/// </summary>
public interface ITable
{
    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the schema of the table.
    /// </summary>
    string Schema { get; }
    
    /// <summary>
    /// Gets the columns in the table.
    /// </summary>
    IReadOnlyCollection<IColumn> Columns { get; }
    
    /// <summary>
    /// Gets the primary key of the table.
    /// </summary>
    IKey? PrimaryKey { get; }
    
    /// <summary>
    /// Gets the foreign keys in the table.
    /// </summary>
    IReadOnlyCollection<IForeignKey> ForeignKeys { get; }
    
    /// <summary>
    /// Gets the indexes in the table.
    /// </summary>
    IReadOnlyCollection<IIndex> Indexes { get; }
    
    /// <summary>
    /// Gets the unique constraints in the table.
    /// </summary>
    IReadOnlyCollection<IUniqueConstraint> UniqueConstraints { get; }
    
    /// <summary>
    /// Gets a value indicating whether the table is a temporal table.
    /// </summary>
    bool IsTemporal { get; }
    
    /// <summary>
    /// Gets the history table name if this is a temporal table.
    /// </summary>
    string? HistoryTableName { get; }
}
