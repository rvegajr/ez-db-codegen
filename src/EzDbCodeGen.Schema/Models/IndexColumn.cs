using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a column in an index.
/// </summary>
public class IndexColumn : IIndexColumn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexColumn"/> class.
    /// </summary>
    /// <param name="column">The column that is part of the index.</param>
    /// <param name="ordinalPosition">The ordinal position of the column in the index.</param>
    /// <param name="isDescending">A value indicating whether the column is in descending order in the index.</param>
    public IndexColumn(IColumn column, int ordinalPosition, bool isDescending = false)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
        OrdinalPosition = ordinalPosition;
        IsDescending = isDescending;
    }

    /// <inheritdoc/>
    public IColumn Column { get; }

    /// <inheritdoc/>
    public int OrdinalPosition { get; }

    /// <inheritdoc/>
    public bool IsDescending { get; }

    /// <inheritdoc/>
    public string Name => Column.Name;

    /// <summary>
    /// Gets a string representation of the index column.
    /// </summary>
    /// <returns>A string representation of the index column.</returns>
    public override string ToString()
    {
        return $"{Column.Name} {(IsDescending ? "DESC" : "ASC")}";
    }
}