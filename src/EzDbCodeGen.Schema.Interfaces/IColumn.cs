namespace EzDbCodeGen.Schema.Interfaces;

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
    /// Gets the ordinal position of the column in the table.
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
    /// Gets a value indicating whether the column is an identity column.
    /// </summary>
    bool IsIdentity { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is computed.
    /// </summary>
    bool IsComputed { get; }
    
    /// <summary>
    /// Gets the default value of the column.
    /// </summary>
    string? DefaultValue { get; }
    
    /// <summary>
    /// Gets the computed column expression.
    /// </summary>
    string? ComputedColumnExpression { get; }
    
    /// <summary>
    /// Gets the collation of the column.
    /// </summary>
    string? Collation { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of the primary key.
    /// </summary>
    bool IsPartOfPrimaryKey { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of a unique constraint.
    /// </summary>
    bool IsPartOfUniqueConstraint { get; }
    
    /// <summary>
    /// Gets a value indicating whether the column is part of a foreign key.
    /// </summary>
    bool IsPartOfForeignKey { get; }
    
    /// <summary>
    /// Gets the table that the column belongs to.
    /// </summary>
    ITable Table { get; }
}
