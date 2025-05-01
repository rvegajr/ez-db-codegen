using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a column in a database view.
/// </summary>
public class ViewColumn : IViewColumn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewColumn"/> class.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="dataType">The data type of the column.</param>
    /// <param name="view">The view that the column belongs to.</param>
    /// <param name="ordinalPosition">The ordinal position of the column in the view.</param>
    /// <param name="isNullable">A value indicating whether the column allows null values.</param>
    public ViewColumn(string name, string dataType, IView view, int ordinalPosition, bool isNullable = true)
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
        View = view ?? throw new ArgumentNullException(nameof(view));
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
    public IView View { get; }

    /// <summary>
    /// Gets the fully qualified name of the view column (schema.view.column).
    /// </summary>
    /// <returns>The fully qualified name of the view column.</returns>
    public string GetFullName()
    {
        return $"{View.Schema}.{View.Name}.{Name}";
    }

    /// <summary>
    /// Maps the view column's database type to a type in the specified language.
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
    /// Gets a string representation of the view column.
    /// </summary>
    /// <returns>A string representation of the view column.</returns>
    public override string ToString()
    {
        return $"{Name} ({DataType}{(IsNullable ? " NULL" : " NOT NULL")})";
    }
}