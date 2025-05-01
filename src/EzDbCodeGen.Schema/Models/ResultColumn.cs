using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a column in the result set of a stored procedure or a table-valued function.
/// </summary>
public class ResultColumn : IResultColumn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultColumn"/> class.
    /// </summary>
    /// <param name="name">The name of the result column.</param>
    /// <param name="dataType">The data type of the result column.</param>
    /// <param name="ordinalPosition">The ordinal position of the result column.</param>
    /// <param name="isNullable">A value indicating whether the result column allows null values.</param>
    public ResultColumn(string name, string dataType, int ordinalPosition, bool isNullable = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Result column name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(dataType))
        {
            throw new ArgumentException("Result column data type cannot be null or empty.", nameof(dataType));
        }

        Name = name;
        DataType = dataType;
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

    /// <summary>
    /// Creates a clone of this result column.
    /// </summary>
    /// <returns>A new result column instance with the same values.</returns>
    public ResultColumn Clone()
    {
        var clone = new ResultColumn(Name, DataType, OrdinalPosition, IsNullable)
        {
            MaxLength = MaxLength,
            Precision = Precision,
            Scale = Scale
        };

        return clone;
    }

    /// <summary>
    /// Maps the result column's database type to a type in the specified language.
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
    /// Gets a suggested property name for this result column when generating code.
    /// </summary>
    /// <returns>A suggested property name.</returns>
    public string GetSuggestedPropertyName()
    {
        // Basic implementation - could be enhanced with more sophisticated naming conventions
        if (string.IsNullOrEmpty(Name))
        {
            return "Column" + OrdinalPosition;
        }

        // Convert from snake_case or other formats to PascalCase
        string propertyName = Name;

        // Handle special characters and spaces
        propertyName = propertyName.Replace("_", " ").Replace("-", " ");

        // Title case the words
        var words = propertyName.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            }
        }

        // Join the words together
        propertyName = string.Join("", words);

        // Ensure it starts with a letter
        if (propertyName.Length > 0 && !char.IsLetter(propertyName[0]))
        {
            propertyName = "P" + propertyName;
        }

        return propertyName;
    }

    /// <summary>
    /// Gets a string representation of the result column.
    /// </summary>
    /// <returns>A string representation of the result column.</returns>
    public override string ToString()
    {
        var nullableStr = IsNullable ? " NULL" : " NOT NULL";
        return $"{Name} {DataType}{nullableStr}";
    }
}