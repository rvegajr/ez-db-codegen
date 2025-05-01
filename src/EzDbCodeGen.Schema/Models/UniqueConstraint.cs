using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a unique constraint in a database table.
/// </summary>
public class UniqueConstraint : IUniqueConstraint
{
    private readonly List<IColumn> _columns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraint"/> class.
    /// </summary>
    /// <param name="name">The name of the unique constraint.</param>
    /// <param name="table">The table that the unique constraint belongs to.</param>
    /// <param name="isClustered">A value indicating whether the unique constraint is clustered.</param>
    public UniqueConstraint(string name, ITable table, bool isClustered = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Table = table ?? throw new ArgumentNullException(nameof(table));
        IsClustered = isClustered;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueConstraint"/> class with specified columns.
    /// </summary>
    /// <param name="name">The name of the unique constraint.</param>
    /// <param name="table">The table that the unique constraint belongs to.</param>
    /// <param name="columns">The columns that make up the unique constraint.</param>
    /// <param name="isClustered">A value indicating whether the unique constraint is clustered.</param>
    public UniqueConstraint(string name, ITable table, IEnumerable<IColumn> columns, bool isClustered = false)
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
    public ITable Table { get; }

    /// <inheritdoc/>
    public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

    /// <inheritdoc/>
    public IList<IColumn> MutableColumns => _columns;

    /// <inheritdoc/>
    public bool IsClustered { get; }

    /// <summary>
    /// Gets a value indicating whether this is a composite unique constraint (contains multiple columns).
    /// </summary>
    public bool IsCompositeConstraint => Columns.Count > 1;

    /// <summary>
    /// Adds a column to the unique constraint.
    /// </summary>
    /// <param name="column">The column to add.</param>
    public void AddColumn(IColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        // Ensure the column belongs to the same table as the unique constraint
        if (column.Table != Table)
        {
            throw new ArgumentException($"Column '{column.Name}' does not belong to table '{Table.Name}'.",
                nameof(column));
        }

        // Check if the column is already part of the unique constraint
        if (_columns.Any(c => string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Column '{column.Name}' is already part of the unique constraint.",
                nameof(column));
        }

        _columns.Add(column);
    }

    /// <summary>
    /// Adds multiple columns to the unique constraint.
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
    /// Checks if the specified column names match the columns in this unique constraint.
    /// </summary>
    /// <param name="columnNames">The column names to check.</param>
    /// <returns>True if the column names match; otherwise, false.</returns>
    public bool MatchesColumns(IEnumerable<string> columnNames)
    {
        if (columnNames == null)
        {
            return false;
        }

        var normalizedColumnNames = columnNames.Select(n => n.ToLowerInvariant()).OrderBy(n => n).ToArray();
        var constraintColumnNames = Columns.Select(c => c.Name.ToLowerInvariant()).OrderBy(n => n).ToArray();

        return constraintColumnNames.SequenceEqual(normalizedColumnNames);
    }

    /// <summary>
    /// Gets the column names as a comma-separated list.
    /// </summary>
    /// <returns>A comma-separated list of column names.</returns>
    public string GetColumnList()
    {
        return string.Join(", ", Columns.Select(c => c.Name));
    }

    /// <summary>
    /// Gets a string representation of the unique constraint.
    /// </summary>
    /// <returns>A string representation of the unique constraint.</returns>
    public override string ToString()
    {
        var clusteredStr = IsClustered ? "CLUSTERED " : "NONCLUSTERED ";
        var columns = GetColumnList();

        return $"CONSTRAINT {Name} {clusteredStr}UNIQUE ({columns})";
    }
}