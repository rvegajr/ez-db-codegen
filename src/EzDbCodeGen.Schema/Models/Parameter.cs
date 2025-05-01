using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a parameter in a stored procedure or function.
/// </summary>
public class Parameter : IParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parameter"/> class.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="dataType">The data type of the parameter.</param>
    /// <param name="ordinalPosition">The ordinal position of the parameter.</param>
    /// <param name="isOutput">A value indicating whether the parameter is an output parameter.</param>
    /// <param name="isNullable">A value indicating whether the parameter allows null values.</param>
    public Parameter(string name, string dataType, int ordinalPosition, bool isOutput = false, bool isNullable = true)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Parameter name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(dataType))
        {
            throw new ArgumentException("Parameter data type cannot be null or empty.", nameof(dataType));
        }

        Name = name;
        DataType = dataType;
        OrdinalPosition = ordinalPosition;
        IsOutput = isOutput;
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
    public bool IsOutput { get; }

    /// <inheritdoc/>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets a value indicating whether the parameter is an input parameter.
    /// </summary>
    public bool IsInput => true; // In SQL Server, parameters can be both input and output

    /// <inheritdoc/>
    public ParameterDirection Direction => IsOutput ? (IsInput ? ParameterDirection.InputOutput : ParameterDirection.Output) : ParameterDirection.Input;

    /// <summary>
    /// Gets the parameter direction as a string (IN, OUT, INOUT).
    /// </summary>
    public string DirectionString => IsOutput ? (IsInput ? "INOUT" : "OUT") : "IN";

    /// <summary>
    /// Creates a clone of this parameter.
    /// </summary>
    /// <returns>A new parameter instance with the same values.</returns>
    public Parameter Clone()
    {
        var clone = new Parameter(Name, DataType, OrdinalPosition, IsOutput, IsNullable)
        {
            MaxLength = MaxLength,
            Precision = Precision,
            Scale = Scale,
            DefaultValue = DefaultValue
        };

        return clone;
    }

    /// <summary>
    /// Maps the parameter's database type to a type in the specified language.
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
    /// Gets a string representation of the parameter.
    /// </summary>
    /// <returns>A string representation of the parameter.</returns>
    public override string ToString()
    {
        var directionStr = IsOutput ? " OUTPUT" : "";
        var nullableStr = IsNullable ? " NULL" : " NOT NULL";
        return $"{Name} {DataType}{nullableStr}{directionStr}";
    }
}