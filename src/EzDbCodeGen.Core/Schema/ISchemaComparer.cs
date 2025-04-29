namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a comparer that detects differences between database schemas.
/// </summary>
public interface ISchemaComparer
{
    /// <summary>
    /// Compares two database schemas and returns the differences.
    /// </summary>
    /// <param name="sourceSchema">The source schema.</param>
    /// <param name="targetSchema">The target schema.</param>
    /// <returns>A schema difference object.</returns>
    ISchemaDiff Compare(IDatabaseSchema sourceSchema, IDatabaseSchema targetSchema);
    
    /// <summary>
    /// Compares two database schemas with the specified options and returns the differences.
    /// </summary>
    /// <param name="sourceSchema">The source schema.</param>
    /// <param name="targetSchema">The target schema.</param>
    /// <param name="options">The comparison options.</param>
    /// <returns>A schema difference object.</returns>
    ISchemaDiff Compare(IDatabaseSchema sourceSchema, IDatabaseSchema targetSchema, SchemaComparisonOptions options);
    
    /// <summary>
    /// Compares two tables and returns the differences.
    /// </summary>
    /// <param name="sourceTable">The source table.</param>
    /// <param name="targetTable">The target table.</param>
    /// <returns>A table difference object.</returns>
    ITableDiff CompareTable(ITable sourceTable, ITable targetTable);
    
    /// <summary>
    /// Compares two columns and returns the differences.
    /// </summary>
    /// <param name="sourceColumn">The source column.</param>
    /// <param name="targetColumn">The target column.</param>
    /// <returns>A column difference object.</returns>
    IColumnDiff CompareColumn(IColumn sourceColumn, IColumn targetColumn);
    
    /// <summary>
    /// Gets the relationship differences.
    /// </summary>
    /// <param name="source">The source schema.</param>
    /// <param name="target">The target schema.</param>
    /// <returns>The relationship differences.</returns>
    IReadOnlyCollection<IRelationshipDiff> CompareRelationships(ISchemaModel source, ISchemaModel target);
    
    /// <summary>
    /// Gets the comparison options.
    /// </summary>
    SchemaComparisonOptions Options { get; }
}
