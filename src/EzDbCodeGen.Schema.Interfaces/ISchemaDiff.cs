namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents differences between two database schemas.
/// </summary>
public interface ISchemaDiff
{
    /// <summary>
    /// Gets the source schema.
    /// </summary>
    IDatabaseSchema Source { get; }
    
    /// <summary>
    /// Gets the target schema.
    /// </summary>
    IDatabaseSchema Target { get; }
    
    /// <summary>
    /// Gets the tables that were added to the target schema.
    /// </summary>
    IReadOnlyList<ITable> AddedTables { get; }
    
    /// <summary>
    /// Gets the tables that were removed from the source schema.
    /// </summary>
    IReadOnlyList<ITable> RemovedTables { get; }
    
    /// <summary>
    /// Gets the tables that were modified between the source and target schemas.
    /// </summary>
    IReadOnlyList<ITableDiff> ModifiedTables { get; }
    
    /// <summary>
    /// Gets the relationships that were added, removed, or changed.
    /// </summary>
    IReadOnlyCollection<IRelationshipDiff> RelationshipDiffs { get; }
    
    /// <summary>
    /// Gets the views that were added, removed, or changed.
    /// </summary>
    IReadOnlyCollection<IViewDiff> ViewDiffs { get; }
    
    /// <summary>
    /// Gets the stored procedures that were added, removed, or changed.
    /// </summary>
    IReadOnlyCollection<IStoredProcedureDiff> StoredProcedureDiffs { get; }
    
    /// <summary>
    /// Gets the functions that were added, removed, or changed.
    /// </summary>
    IReadOnlyCollection<IFunctionDiff> FunctionDiffs { get; }
    
    /// <summary>
    /// Gets a value indicating whether there are any differences between the schemas.
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
