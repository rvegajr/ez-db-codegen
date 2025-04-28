namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents differences between two database tables.
/// </summary>
public interface ITableDiff
{
    /// <summary>
    /// Gets the source table.
    /// </summary>
    ITable Source { get; }
    
    /// <summary>
    /// Gets the target table.
    /// </summary>
    ITable Target { get; }
    
    /// <summary>
    /// Gets the columns that were added to the target table.
    /// </summary>
    IReadOnlyList<IColumn> AddedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were removed from the source table.
    /// </summary>
    IReadOnlyList<IColumn> RemovedColumns { get; }
    
    /// <summary>
    /// Gets the columns that were modified between the source and target tables.
    /// </summary>
    IReadOnlyList<IColumnDiff> ModifiedColumns { get; }
    
    /// <summary>
    /// Gets the primary key diff, if the primary key has changed.
    /// </summary>
    IKeyDiff PrimaryKeyDiff { get; }
    
    /// <summary>
    /// Gets the foreign key diffs.
    /// </summary>
    IReadOnlyCollection<IForeignKeyDiff> ForeignKeyDiffs { get; }
    
    /// <summary>
    /// Gets the index diffs.
    /// </summary>
    IReadOnlyCollection<IIndexDiff> IndexDiffs { get; }
    
    /// <summary>
    /// Gets the unique constraint diffs.
    /// </summary>
    IReadOnlyCollection<IUniqueConstraintDiff> UniqueConstraintDiffs { get; }
    
    /// <summary>
    /// Gets a value indicating whether the table name has changed.
    /// </summary>
    bool NameChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the table schema has changed.
    /// </summary>
    bool SchemaChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether there are any differences between the tables.
    /// </summary>
    bool HasDifferences { get; }
    
    /// <summary>
    /// Gets a summary of the differences.
    /// </summary>
    /// <returns>A summary of the differences.</returns>
    string GetSummary();
    
    /// <summary>
    /// Gets a detailed report of the differences.
    /// </summary>
    /// <returns>A detailed report of the differences.</returns>
    string GetDetailedReport();
}
