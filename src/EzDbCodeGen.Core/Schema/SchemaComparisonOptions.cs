namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents options for comparing database schemas.
/// </summary>
public class SchemaComparisonOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to ignore case when comparing names.
    /// </summary>
    public bool IgnoreCase { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to ignore whitespace when comparing definitions.
    /// </summary>
    public bool IgnoreWhitespace { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to ignore comments when comparing definitions.
    /// </summary>
    public bool IgnoreComments { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare table schemas.
    /// </summary>
    public bool CompareTableSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare view schemas.
    /// </summary>
    public bool CompareViewSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare stored procedure schemas.
    /// </summary>
    public bool CompareStoredProcedureSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare function schemas.
    /// </summary>
    public bool CompareFunctionSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare relationship schemas.
    /// </summary>
    public bool CompareRelationshipSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare index schemas.
    /// </summary>
    public bool CompareIndexSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare unique constraint schemas.
    /// </summary>
    public bool CompareUniqueConstraintSchemas { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column order.
    /// </summary>
    public bool CompareColumnOrder { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column data types.
    /// </summary>
    public bool CompareColumnDataTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column nullability.
    /// </summary>
    public bool CompareColumnNullability { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column default values.
    /// </summary>
    public bool CompareColumnDefaultValues { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column computed expressions.
    /// </summary>
    public bool CompareColumnComputedExpressions { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column identity specifications.
    /// </summary>
    public bool CompareColumnIdentitySpecifications { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column collations.
    /// </summary>
    public bool CompareColumnCollations { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare primary keys.
    /// </summary>
    public bool ComparePrimaryKeys { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare foreign keys.
    /// </summary>
    public bool CompareForeignKeys { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column precision and scale.
    /// </summary>
    public bool CompareColumnPrecisionAndScale { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare column max length.
    /// </summary>
    public bool CompareColumnMaxLength { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to compare extended properties.
    /// </summary>
    public bool CompareExtendedProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to treat added tables as modifications.
    /// </summary>
    public bool TreatAddedTablesAsModifications { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to treat removed tables as modifications.
    /// </summary>
    public bool TreatRemovedTablesAsModifications { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a list of tables to exclude from comparison.
    /// </summary>
    public IList<string> ExcludedTables { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets a list of schemas to exclude from comparison.
    /// </summary>
    public IList<string> ExcludedSchemas { get; set; } = new List<string>();
}
