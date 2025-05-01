using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a stored procedure in a database.
/// </summary>
public class StoredProcedure : IStoredProcedure
{
    private readonly List<IParameter> _parameters = new();
    private readonly List<IResultColumn>? _resultColumns;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="schema">The schema of the stored procedure.</param>
    /// <param name="definition">The definition of the stored procedure.</param>
    /// <param name="hasResultSet">A value indicating whether the stored procedure returns a result set.</param>
    public StoredProcedure(string name, string schema = "dbo", string? definition = null, bool hasResultSet = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(name));
        }

        Name = name;
        Schema = schema ?? "dbo";
        Definition = definition;

        if (hasResultSet)
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
    public IReadOnlyCollection<IResultColumn>? ResultColumns => _resultColumns?.AsReadOnly();

    /// <inheritdoc/>
    public string? Definition { get; }

    /// <summary>
    /// Gets a value indicating whether the stored procedure returns a result set.
    /// </summary>
    public bool HasResultSet => _resultColumns != null;

    /// <summary>
    /// Gets the input parameters of the stored procedure.
    /// </summary>
    public IEnumerable<IParameter> InputParameters => _parameters.Where(p => !p.IsOutput);

    /// <summary>
    /// Gets the output parameters of the stored procedure.
    /// </summary>
    public IEnumerable<IParameter> OutputParameters => _parameters.Where(p => p.IsOutput);

    /// <summary>
    /// Adds a parameter to the stored procedure.
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
            throw new ArgumentException(
                $"Parameter with name '{parameter.Name}' already exists in the stored procedure.", nameof(parameter));
        }

        _parameters.Add(parameter);
    }

    /// <summary>
    /// Adds multiple parameters to the stored procedure.
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
    /// Creates a new parameter and adds it to the stored procedure.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="dataType">The data type of the parameter.</param>
    /// <param name="isOutput">A value indicating whether the parameter is an output parameter.</param>
    /// <param name="isNullable">A value indicating whether the parameter allows null values.</param>
    /// <returns>The created parameter.</returns>
    public IParameter CreateParameter(string name, string dataType, bool isOutput = false, bool isNullable = true)
    {
        var parameter = new Parameter(name, dataType, _parameters.Count, isOutput, isNullable);
        AddParameter(parameter);
        return parameter;
    }

    /// <summary>
    /// Adds a result column to the stored procedure.
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
            throw new InvalidOperationException(
                "Cannot add result columns to a stored procedure that does not return a result set.");
        }

        if (_resultColumns.Any(c => string.Equals(c.Name, resultColumn.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                $"Result column with name '{resultColumn.Name}' already exists in the stored procedure.",
                nameof(resultColumn));
        }

        _resultColumns.Add(resultColumn);
    }

    /// <summary>
    /// Adds multiple result columns to the stored procedure.
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
    /// Creates a new result column and adds it to the stored procedure.
    /// </summary>
    /// <param name="name">The name of the result column.</param>
    /// <param name="dataType">The data type of the result column.</param>
    /// <param name="isNullable">A value indicating whether the result column allows null values.</param>
    /// <returns>The created result column.</returns>
    public IResultColumn CreateResultColumn(string name, string dataType, bool isNullable = true)
    {
        if (_resultColumns == null)
        {
            throw new InvalidOperationException(
                "Cannot add result columns to a stored procedure that does not return a result set.");
        }

        var resultColumn = new ResultColumn(name, dataType, _resultColumns.Count, isNullable);
        AddResultColumn(resultColumn);
        return resultColumn;
    }

    /// <summary>
    /// Gets the fully qualified name of the stored procedure (schema.name).
    /// </summary>
    /// <returns>The fully qualified name of the stored procedure.</returns>
    public string GetFullName()
    {
        return $"{Schema}.{Name}";
    }

    /// <summary>
    /// Generates a suggested method name for invoking this stored procedure.
    /// </summary>
    /// <param name="removePrefix">Prefix to remove from the procedure name.</param>
    /// <param name="removeSuffix">Suffix to remove from the procedure name.</param>
    /// <returns>A suggested method name.</returns>
    public string GetSuggestedMethodName(string? removePrefix = null, string? removeSuffix = null)
    {
        string methodName = Name;

        // Remove prefix if specified and procedure name starts with it
        if (!string.IsNullOrEmpty(removePrefix) &&
            methodName.StartsWith(removePrefix, StringComparison.OrdinalIgnoreCase))
        {
            methodName = methodName.Substring(removePrefix.Length);
        }

        // Remove suffix if specified and procedure name ends with it
        if (!string.IsNullOrEmpty(removeSuffix) &&
            methodName.EndsWith(removeSuffix, StringComparison.OrdinalIgnoreCase))
        {
            methodName = methodName.Substring(0, methodName.Length - removeSuffix.Length);
        }

        // Convert from snake_case or other formats to PascalCase
        methodName = methodName.Replace("_", " ").Replace("-", " ");

        // Title case the words
        var words = methodName.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1);
            }
        }

        // Join the words together
        methodName = string.Join("", words);

        return methodName;
    }

    /// <summary>
    /// Generates a suggested class name for the result set of this stored procedure.
    /// </summary>
    /// <returns>A suggested class name for the result set.</returns>
    public string GetSuggestedResultClassName()
    {
        // Base the result class name on the stored procedure name
        string baseName = GetSuggestedMethodName();

        // Add a suffix to indicate it's a result
        return $"{baseName}Result";
    }

    /// <summary>
    /// Gets a string representation of the stored procedure.
    /// </summary>
    /// <returns>A string representation of the stored procedure.</returns>
    public override string ToString()
    {
        return GetFullName();
    }
}