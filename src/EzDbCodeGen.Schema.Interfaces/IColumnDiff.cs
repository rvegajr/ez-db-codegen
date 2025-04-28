namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents differences between two database columns.
/// </summary>
public interface IColumnDiff
{
    /// <summary>
    /// Gets the source column.
    /// </summary>
    IColumn Source { get; }
    
    /// <summary>
    /// Gets the target column.
    /// </summary>
    IColumn Target { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column name has changed.
    /// </summary>
    bool NameChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column data type has changed.
    /// </summary>
    bool DataTypeChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column nullability has changed.
    /// </summary>
    bool NullabilityChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column default value has changed.
    /// </summary>
    bool DefaultValueChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column computed expression has changed.
    /// </summary>
    bool ComputedExpressionChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column identity specification has changed.
    /// </summary>
    bool IdentitySpecificationChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column collation has changed.
    /// </summary>
    bool CollationChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column precision has changed.
    /// </summary>
    bool PrecisionChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column scale has changed.
    /// </summary>
    bool ScaleChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column max length has changed.
    /// </summary>
    bool MaxLengthChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column ordinal position has changed.
    /// </summary>
    bool OrdinalPositionChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of a primary key has changed.
    /// </summary>
    bool IsPartOfPrimaryKeyChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of a unique constraint has changed.
    /// </summary>
    bool IsPartOfUniqueConstraintChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of a foreign key has changed.
    /// </summary>
    bool IsPartOfForeignKeyChanged { get; }
    
    /// <summary>
    /// Gets a value indicating whether there are any differences between the columns.
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
