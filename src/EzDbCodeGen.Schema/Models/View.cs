using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a database view.
/// </summary>
public class View : IView
{
    private readonly List<IViewColumn> _columns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    /// <param name="name">The name of the view.</param>
    /// <param name="schema">The schema of the view.</param>
    /// <param name="definition">The definition of the view.</param>
    /// <param name="isIndexed">A value indicating whether the view is indexed.</param>
    public View(string name, string schema = "dbo", string? definition = null, bool isIndexed = false)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("View name cannot be null or empty.", nameof(name));
        }

        Name = name;
        Schema = schema ?? "dbo";
        Definition = definition;
        IsIndexed = isIndexed;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Schema { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IViewColumn> Columns => _columns.AsReadOnly();

    /// <inheritdoc/>
    public string? Definition { get; }

    /// <inheritdoc/>
    public bool IsIndexed { get; }

    /// <summary>
    /// Adds a column to the view.
    /// </summary>
    /// <param name="column">The column to add.</param>
    public void AddColumn(IViewColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (_columns.Any(c => string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Column with name '{column.Name}' already exists in the view.",
                nameof(column));
        }

        _columns.Add(column);
    }

    /// <summary>
    /// Adds multiple columns to the view.
    /// </summary>
    /// <param name="columns">The columns to add.</param>
    public void AddColumns(IEnumerable<IViewColumn> columns)
    {
        if (columns == null)
        {
            throw new ArgumentNullException(nameof(columns));
        }

        foreach (var column in columns)
        {
            AddColumn(column);
        }
    }

    /// <summary>
    /// Creates a new view column and adds it to the view.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <param name="dataType">The data type of the column.</param>
    /// <param name="ordinalPosition">The ordinal position of the column.</param>
    /// <param name="isNullable">A value indicating whether the column allows null values.</param>
    /// <returns>The created view column.</returns>
    public IViewColumn CreateColumn(string name, string dataType, int ordinalPosition, bool isNullable = true)
    {
        var column = new ViewColumn(name, dataType, this, ordinalPosition, isNullable);
        AddColumn(column);
        return column;
    }

    /// <summary>
    /// Gets a column by name.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The column, or null if not found.</returns>
    public IViewColumn? GetColumn(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Column name cannot be null or empty.", nameof(name));
        }

        return _columns.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the fully qualified name of the view (schema.view).
    /// </summary>
    /// <returns>The fully qualified name of the view.</returns>
    public string GetFullName()
    {
        return $"{Schema}.{Name}";
    }

    /// <summary>
    /// Generates a suggested entity name for the view.
    /// </summary>
    /// <param name="removePrefix">Prefix to remove from the view name.</param>
    /// <param name="removeSuffix">Suffix to remove from the view name.</param>
    /// <returns>A suggested entity name.</returns>
    public string GetSuggestedEntityName(string? removePrefix = null, string? removeSuffix = null)
    {
        string entityName = Name;

        // Remove prefix if specified and view name starts with it
        if (!string.IsNullOrEmpty(removePrefix) &&
            entityName.StartsWith(removePrefix, StringComparison.OrdinalIgnoreCase))
        {
            entityName = entityName.Substring(removePrefix.Length);
        }

        // Remove suffix if specified and view name ends with it
        if (!string.IsNullOrEmpty(removeSuffix) &&
            entityName.EndsWith(removeSuffix, StringComparison.OrdinalIgnoreCase))
        {
            entityName = entityName.Substring(0, entityName.Length - removeSuffix.Length);
        }

        // Ensure the entity name starts with a capital letter
        if (entityName.Length > 0)
        {
            entityName = char.ToUpperInvariant(entityName[0]) + entityName.Substring(1);
        }

        return entityName;
    }

    /// <summary>
    /// Gets a string representation of the view.
    /// </summary>
    /// <returns>A string representation of the view.</returns>
    public override string ToString()
    {
        return GetFullName();
    }
}