using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a key constraint in a database table.
/// </summary>
public class Key : IKey
{
    private readonly List<IColumn> _columns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Key"/> class.
    /// </summary>
    /// <param name="name">The name of the key.</param>
    /// <param name="table">The table that the key belongs to.</param>
    /// <param name="isClustered">A value indicating whether the key is clustered.</param>
    public Key(string name, ITable table, bool isClustered = true)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Table = table ?? throw new ArgumentNullException(nameof(table));
        IsClustered = isClustered;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Key"/> class with specified columns.
    /// </summary>
    /// <param name="name">The name of the key.</param>
    /// <param name="table">The table that the key belongs to.</param>
    /// <param name="columns">The columns that make up the key.</param>
    /// <param name="isClustered">A value indicating whether the key is clustered.</param>
    public Key(string name, ITable table, IEnumerable<IColumn> columns, bool isClustered = true)
        : this(name, table, isClustered)
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

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

    /// <inheritdoc/>
    public ITable Table { get; }

    /// <inheritdoc/>
    public bool IsClustered { get; }

    /// <summary>
    /// Adds a column to the key.
    /// </summary>
    /// <param name="column">The column to add.</param>
    public void AddColumn(IColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        // Ensure the column belongs to the same table as the key
        if (column.Table != Table)
        {
            throw new ArgumentException($"Column '{column.Name}' does not belong to table '{Table.Name}'.",
                nameof(column));
        }

        // Check if the column is already part of the key
        if (_columns.Any(c => string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Column '{column.Name}' is already part of the key.", nameof(column));
        }

        _columns.Add(column);
    }

    /// <summary>
    /// Adds multiple columns to the key.
    /// </summary>
    /// <param name="columns">The columns to add.</param>
    public void AddColumns(IEnumerable<IColumn> columns)
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
    /// Gets a string representation of the key.
    /// </summary>
    /// <returns>A string representation of the key.</returns>
    public override string ToString()
    {
        var columnNames = string.Join(", ", Columns.Select(c => c.Name));
        return $"{Name} ({columnNames})";
    }

    /// <summary>
    /// Gets a value indicating whether this key is a composite key (contains multiple columns).
    /// </summary>
    public bool IsCompositeKey => Columns.Count > 1;

    /// <summary>
    /// Checks if this key uniquely identifies a row in the table.
    /// Primary keys always uniquely identify rows.
    /// </summary>
    /// <returns>True if this key uniquely identifies a row; otherwise, false.</returns>
    public bool IsUniqueIdentifier()
    {
        // In most cases, a Key represents a primary key which by definition uniquely identifies a row
        // This method could be overridden in derived classes if needed
        return true;
    }

    /// <summary>
    /// Checks if the specified columns match the columns in this key.
    /// </summary>
    /// <param name="columnNames">The column names to check.</param>
    /// <returns>True if the columns match; otherwise, false.</returns>
    public bool MatchesColumns(IEnumerable<string> columnNames)
    {
        if (columnNames == null)
        {
            return false;
        }

        var normalizedColumnNames = columnNames.Select(n => n.ToLowerInvariant()).OrderBy(n => n).ToArray();
        var keyColumnNames = Columns.Select(c => c.Name.ToLowerInvariant()).OrderBy(n => n).ToArray();

        return keyColumnNames.SequenceEqual(normalizedColumnNames);
    }
}