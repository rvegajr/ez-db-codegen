using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Extensions;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents an index in a database table.
/// </summary>
public class Index : IIndex
{
    private readonly List<IIndexColumn> _columns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class.
    /// </summary>
    /// <param name="name">The name of the index.</param>
    /// <param name="table">The table that the index belongs to.</param>
    /// <param name="isUnique">A value indicating whether the index is unique.</param>
    /// <param name="isClustered">A value indicating whether the index is clustered.</param>
    /// <param name="filter">The filter expression for the index, if any.</param>
    public Index(string name, ITable table, bool isUnique = false, bool isClustered = false, string? filter = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Table = table ?? throw new ArgumentNullException(nameof(table));
        IsUnique = isUnique;
        IsClustered = isClustered;
        Filter = filter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Index"/> class with specified columns.
    /// </summary>
    /// <param name="name">The name of the index.</param>
    /// <param name="table">The table that the index belongs to.</param>
    /// <param name="columns">The columns that make up the index.</param>
    /// <param name="isUnique">A value indicating whether the index is unique.</param>
    /// <param name="isClustered">A value indicating whether the index is clustered.</param>
    /// <param name="filter">The filter expression for the index, if any.</param>
    public Index(
        string name,
        ITable table,
        IEnumerable<IIndexColumn> columns,
        bool isUnique = false,
        bool isClustered = false,
        string? filter = null)
        : this(name, table, isUnique, isClustered, filter)
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
    public IReadOnlyList<IIndexColumn> Columns => _columns.AsReadOnly();

    /// <inheritdoc/>
    public bool IsUnique { get; }

    /// <inheritdoc/>
    public bool IsClustered { get; }

    /// <inheritdoc/>
    public string? Filter { get; }

    /// <summary>
    /// Gets a value indicating whether this is a composite index (contains multiple columns).
    /// </summary>
    public bool IsCompositeIndex => Columns.Count > 1;

    /// <summary>
    /// Adds a column to the index.
    /// </summary>
    /// <param name="column">The column to add.</param>
    public void AddColumn(IIndexColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        // Ensure the column belongs to the same table as the index
        if (column.Column.Table != Table)
        {
            throw new ArgumentException($"Column '{column.Column.Name}' does not belong to table '{Table.Name}'.",
                nameof(column));
        }

        // Check if the column is already part of the index
        if (_columns.Any(c => string.Equals(c.Column.Name, column.Column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Column '{column.Column.Name}' is already part of the index.", nameof(column));
        }

        _columns.Add(column);

        // Ensure columns are ordered by ordinal position
        _columns.Sort((a, b) => a.OrdinalPosition.CompareTo(b.OrdinalPosition));
    }

    /// <summary>
    /// Adds a column to the index.
    /// </summary>
    /// <param name="column">The column to add.</param>
    /// <param name="isDescending">A value indicating whether the column is in descending order.</param>
    public void AddColumn(IColumn column, bool isDescending = false)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        // Create a new IndexColumn and add it
        var indexColumn = new IndexColumn(column, _columns.Count, isDescending);
        AddColumn(indexColumn);
    }

    /// <summary>
    /// Adds multiple columns to the index.
    /// </summary>
    /// <param name="columns">The columns to add.</param>
    public void AddColumns(IEnumerable<IIndexColumn> columns)
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
    /// Adds multiple columns to the index.
    /// </summary>
    /// <param name="columns">The columns to add.</param>
    /// <param name="isDescending">A value indicating whether the columns are in descending order.</param>
    public void AddColumns(IEnumerable<IColumn> columns, bool isDescending = false)
    {
        if (columns == null)
        {
            throw new ArgumentNullException(nameof(columns));
        }

        foreach (var column in columns)
        {
            AddColumn(column, isDescending);
        }
    }

    /// <summary>
    /// Creates an index column from a regular column.
    /// </summary>
    /// <param name="column">The column to convert.</param>
    /// <param name="ordinalPosition">The ordinal position in the index.</param>
    /// <param name="isDescending">Whether the column is in descending order.</param>
    /// <returns>A new index column.</returns>
    public static IIndexColumn CreateIndexColumn(IColumn column, int ordinalPosition, bool isDescending = false)
    {
        return new IndexColumn(column, ordinalPosition, isDescending);
    }

    /// <summary>
    /// Gets the covered columns as a comma-separated list.
    /// </summary>
    /// <returns>A comma-separated list of column names.</returns>
    public string GetColumnList()
    {
        return string.Join(", ", Columns.Select(c => $"{c.Column.Name}{(c.IsDescending ? " DESC" : "")}"));
    }

    /// <summary>
    /// Gets a string representation of the index.
    /// </summary>
    /// <returns>A string representation of the index.</returns>
    public override string ToString()
    {
        var uniqueStr = IsUnique ? "UNIQUE " : "";
        var clusteredStr = IsClustered ? "CLUSTERED " : "NONCLUSTERED ";
        var columns = GetColumnList();
        var filterStr = !string.IsNullOrEmpty(Filter) ? $" WHERE {Filter}" : "";

        return $"{uniqueStr}{clusteredStr}INDEX {Name} ON {Table.GetFullName()}({columns}){filterStr}";
    }
}