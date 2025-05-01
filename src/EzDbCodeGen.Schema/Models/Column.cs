using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a column in a database table.
/// </summary>
public class Column : IColumn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Column"/> class.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="dataType">The data type of the column.</param>
    /// <param name="table">The table that the column belongs to.</param>
    /// <param name="ordinalPosition">The ordinal position of the column in the table.</param>
    /// <param name="isNullable">A value indicating whether the column allows null values.</param>
    public Column(string name, string dataType, ITable table, int ordinalPosition, bool isNullable = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Column name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(dataType))
        {
            throw new ArgumentException("Column data type cannot be null or empty.", nameof(dataType));
        }

        Name = name;
        DataType = dataType;
        Table = table ?? throw new ArgumentNullException(nameof(table));
        OrdinalPosition = ordinalPosition;
        IsNullable = isNullable;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public int OrdinalPosition { get; }

    /// <inheritdoc/>
    public string DataType { get; }

    /// <inheritdoc/>
    public bool IsNullable { get; set; }

    /// <inheritdoc/>
    public int? MaxLength { get; set; }

    /// <inheritdoc/>
    public int? Precision { get; set; }

    /// <inheritdoc/>
    public int? Scale { get; set; }

    /// <inheritdoc/>
    public bool IsIdentity { get; set; }

    /// <inheritdoc/>
    public bool IsComputed { get; set; }

    /// <inheritdoc/>
    public string? DefaultValue { get; set; }
    
    /// <inheritdoc/>
    public string? ComputedColumnExpression { get; set; }
    
    /// <inheritdoc/>
    public string? Collation { get; set; }
    
    /// <inheritdoc/>
    public bool IsPartOfPrimaryKey { get; set; }
    
    /// <inheritdoc/>
    public bool IsPartOfUniqueConstraint { get; set; }
    
    /// <inheritdoc/>
    public bool IsPartOfForeignKey { get; set; }

    /// <inheritdoc/>
    public ITable Table { get; }

    /// <summary>
    /// Gets the fully qualified name of the column (schema.table.column).
    /// </summary>
    /// <returns>The fully qualified name of the column.</returns>
    public string GetFullName()
    {
        return $"{Table.Schema}.{Table.Name}.{Name}";
    }

    /// <summary>
    /// Clones the column with a new table reference.
    /// </summary>
    /// <param name="newTable">The new table reference.</param>
    /// <returns>A cloned column.</returns>
    public Column Clone(ITable newTable)
    {
        var clone = new Column(Name, DataType, newTable, OrdinalPosition, IsNullable)
        {
            MaxLength = MaxLength,
            Precision = Precision,
            Scale = Scale,
            IsIdentity = IsIdentity,
            IsComputed = IsComputed,
            DefaultValue = DefaultValue,
            ComputedColumnExpression = ComputedColumnExpression,
            Collation = Collation,
            IsPartOfPrimaryKey = IsPartOfPrimaryKey,
            IsPartOfUniqueConstraint = IsPartOfUniqueConstraint,
            IsPartOfForeignKey = IsPartOfForeignKey
        };

        return clone;
    }

    /// <summary>
    /// Maps the column's database type to a type in the specified language.
    /// This method is designed to work with the DataTypeMap system.
    /// </summary>
    /// <param name="targetLanguage">The target programming language (e.g., "csharp", "typescript", "java").</param>
    /// <param name="includeNullability">Whether to include nullability in the type declaration.</param>
    /// <returns>The mapped type name in the target language.</returns>
    public string MapTypeToLanguage(string targetLanguage, bool includeNullability = true)
    {
        // This stub method would normally integrate with the DataTypeMap system
        // In a full implementation, it would use the DataTypeMapFactory to get the appropriate mapper

        // Example usage would be:
        // var dataTypeMap = dataTypeMapFactory.CreateTypeMap("SqlServer");
        // return dataTypeMap.MapType(DataType, targetLanguage, Precision, Scale, MaxLength, IsNullable && includeNullability);

        // For now we'll return a simple string indicating what would happen
        return $"{DataType} mapped to {targetLanguage}";
    }

    /// <summary>
    /// Gets the default value for this column in the specified language.
    /// </summary>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <returns>The default value expression in the target language.</returns>
    public string GetDefaultValueInLanguage(string targetLanguage)
    {
        // This stub method would normally integrate with the DataTypeMap system
        // In a full implementation, it would use the DataTypeMapFactory to get the appropriate mapper

        // Example usage would be:
        // var dataTypeMap = dataTypeMapFactory.CreateTypeMap("SqlServer");
        // return dataTypeMap.GetDefaultValue(DataType, targetLanguage);

        // For now we'll return a simple string indicating what would happen
        return $"Default value for {DataType} in {targetLanguage}";
    }

    /// <summary>
    /// Gets a string representation of the column.
    /// </summary>
    /// <returns>A string representation of the column.</returns>
    public override string ToString()
    {
        return $"{Name} ({DataType}{(IsNullable ? " NULL" : " NOT NULL")})";
    }
}