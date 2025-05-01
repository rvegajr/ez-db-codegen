using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a user-defined function in a database.
/// </summary>
public class Function : IFunction
{
    private readonly List<IParameter> _parameters = new();
    private readonly List<IResultColumn>? _resultColumns;

    /// <summary>
    /// Initializes a new instance of the <see cref="Function"/> class.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="returnType">The return type of the function.</param>
    /// <param name="schema">The schema of the function.</param>
    /// <param name="isTableValued">A value indicating whether the function is a table-valued function.</param>
    /// <param name="definition">The definition of the function.</param>
    public Function(string name, string returnType, string schema = "dbo", bool isTableValued = false,
        string? definition = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Function name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(returnType))
        {
            throw new ArgumentException("Function return type cannot be null or empty.", nameof(returnType));
        }

        Name = name;
        ReturnType = returnType;
        Schema = schema ?? "dbo";
        IsTableValued = isTableValued;
        Definition = definition;

        if (isTableValued)
        {
            _resultColumns = new List<IResultColumn>();
        }
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Schema { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IParameter> Parameters => _parameters.AsReadOnly();

    /// <inheritdoc/>
    public string ReturnType { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IResultColumn>? ResultColumns => _resultColumns?.AsReadOnly();

    /// <inheritdoc/>
    public bool IsTableValued { get; }

    /// <inheritdoc/>
    public string? Definition { get; }

    /// <summary>
    /// Gets a value indicating whether the function is a scalar function.
    /// </summary>
    public bool IsScalar => !IsTableValued;

    /// <summary>
    /// Adds a parameter to the function.
    /// </summary>
    /// <param name="parameter">The parameter to add.</param>
    public void AddParameter(IParameter parameter)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(nameof(parameter));
        }

        if (_parameters.Any(p => string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Parameter with name '{parameter.Name}' already exists in the function.",
                nameof(parameter));
        }

        _parameters.Add(parameter);
    }

    /// <summary>
    /// Adds multiple parameters to the function.
    /// </summary>
    /// <param name="parameters">The parameters to add.</param>
    public void AddParameters(IEnumerable<IParameter> parameters)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }

    /// <summary>
    /// Creates a new parameter and adds it to the function.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="dataType">The data type of the parameter.</param>
    /// <param name="isNullable">A value indicating whether the parameter allows null values.</param>
    /// <returns>The created parameter.</returns>
    public IParameter CreateParameter(string name, string dataType, bool isNullable = true)
    {
        // Note: Function parameters are never output parameters
        var parameter = new Parameter(name, dataType, _parameters.Count, isOutput: false, isNullable);
        AddParameter(parameter);
        return parameter;
    }

    /// <summary>
    /// Adds a result column to the function.
    /// </summary>
    /// <param name="resultColumn">The result column to add.</param>
    public void AddResultColumn(IResultColumn resultColumn)
    {
        if (resultColumn == null)
        {
            throw new ArgumentNullException(nameof(resultColumn));
        }

        if (_resultColumns == null)
        {
            throw new InvalidOperationException("Cannot add result columns to a scalar function.");
        }

        if (_resultColumns.Any(c => string.Equals(c.Name, resultColumn.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                $"Result column with name '{resultColumn.Name}' already exists in the function.", nameof(resultColumn));
        }

        _resultColumns.Add(resultColumn);
    }

    /// <summary>
    /// Adds multiple result columns to the function.
    /// </summary>
    /// <param name="resultColumns">The result columns to add.</param>
    public void AddResultColumns(IEnumerable<IResultColumn> resultColumns)
    {
        if (resultColumns == null)
        {
            throw new ArgumentNullException(nameof(resultColumns));
        }

        foreach (var resultColumn in resultColumns)
        {
            AddResultColumn(resultColumn);
        }
    }

    /// <summary>
    /// Creates a new result column and adds it to the function.
    /// </summary>
    /// <param name="name">The name of the result column.</param>
    /// <param name="dataType">The data type of the result column.</param>
    /// <param name="isNullable">A value indicating whether the result column allows null values.</param>
    /// <returns>The created result column.</returns>
    public IResultColumn CreateResultColumn(string name, string dataType, bool isNullable = true)
    {
        if (_resultColumns == null)
        {
            throw new InvalidOperationException("Cannot add result columns to a scalar function.");
        }

        var resultColumn = new ResultColumn(name, dataType, _resultColumns.Count, isNullable);
        AddResultColumn(resultColumn);
        return resultColumn;
    }

    /// <summary>
    /// Gets the fully qualified name of the function (schema.name).
    /// </summary>
    /// <returns>The fully qualified name of the function.</returns>
    public string GetFullName()
    {
        return $"{Schema}.{Name}";
    }

    /// <summary>
    /// Maps the return type of the function to a type in the specified language.
    /// This method is designed to work with the DataTypeMap system.
    /// </summary>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <param name="includeNullability">Whether to include nullability in the type declaration.</param>
    /// <returns>The mapped type name in the target language.</returns>
    public string MapReturnTypeToLanguage(string targetLanguage, bool includeNullability = true)
    {
        if (IsTableValued)
        {
            // For table-valued functions, return an appropriate collection type
            return GetTableValuedReturnType(targetLanguage);
        }

        // This stub method would normally integrate with the DataTypeMap system
        // In a full implementation, it would use the DataTypeMapFactory to get the appropriate mapper

        // Example usage would be:
        // var dataTypeMap = dataTypeMapFactory.CreateTypeMap("SqlServer");
        // return dataTypeMap.MapType(ReturnType, targetLanguage, null, null, null, includeNullability);

        // For now we'll return a simple string indicating what would happen
        return $"{ReturnType} mapped to {targetLanguage}";
    }

    /// <summary>
    /// Gets the appropriate collection type for a table-valued function.
    /// </summary>
    /// <param name="targetLanguage">The target programming language.</param>
    /// <returns>The appropriate collection type for the target language.</returns>
    private string GetTableValuedReturnType(string targetLanguage)
    {
        string resultType = GetSuggestedResultClassName();

        switch (targetLanguage.ToLowerInvariant())
        {
            case "csharp":
                return $"IEnumerable<{resultType}>";

            case "typescript":
                return $"{resultType}[]";

            case "java":
                return $"List<{resultType}>";

            case "python":
                return $"List[{resultType}]";

            default:
                return $"Collection of {resultType}";
        }
    }

    /// <summary>
    /// Generates a suggested method name for invoking this function.
    /// </summary>
    /// <param name="removePrefix">Prefix to remove from the function name.</param>
    /// <param name="removeSuffix">Suffix to remove from the function name.</param>
    /// <returns>A suggested method name.</returns>
    public string GetSuggestedMethodName(string? removePrefix = null, string? removeSuffix = null)
    {
        string methodName = Name;

        // Remove prefix if specified and function name starts with it
        if (!string.IsNullOrEmpty(removePrefix) &&
            methodName.StartsWith(removePrefix, StringComparison.OrdinalIgnoreCase))
        {
            methodName = methodName.Substring(removePrefix.Length);
        }

        // Remove suffix if specified and function name ends with it
        if (!string.IsNullOrEmpty(removeSuffix) &&
            methodName.EndsWith(removeSuffix, StringComparison.OrdinalIgnoreCase))
        {
            methodName = methodName.Substring(0, methodName.Length - removeSuffix.Length);
        }

        // Convert to camelCase for method names (first character lowercase)
        if (methodName.Length > 0)
        {
            // First convert to PascalCase
            methodName = ToPascalCase(methodName);

            // Then convert first character to lowercase
            methodName = char.ToLowerInvariant(methodName[0]) + methodName.Substring(1);
        }

        return methodName;
    }

    /// <summary>
    /// Generates a suggested class name for the result set of this function.
    /// </summary>
    /// <returns>A suggested class name for the result set.</returns>
    public string GetSuggestedResultClassName()
    {
        // Base the result class name on the function name
        string baseName = Name;

        // Convert to PascalCase
        baseName = ToPascalCase(baseName);

        return $"{baseName}Result";
    }

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The input string converted to PascalCase.</returns>
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Convert from snake_case or other formats to PascalCase
        input = input.Replace("_", " ").Replace("-", " ");

        // Title case the words
        var words = input.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            }
        }

        // Join the words together
        return string.Join("", words);
    }

    /// <summary>
    /// Gets a string representation of the function.
    /// </summary>
    /// <returns>A string representation of the function.</returns>
    public override string ToString()
    {
        var paramList = string.Join(", ", _parameters.Select(p => p.ToString()));
        return $"{GetFullName()}({paramList}) RETURNS {(IsTableValued ? "TABLE" : ReturnType)}";
    }
}