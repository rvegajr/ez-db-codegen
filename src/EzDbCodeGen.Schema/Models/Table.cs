using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models;

/// <summary>
/// Represents a database table with columns, keys, and constraints.
/// </summary>
public class Table : ITable
{
    private readonly List<IColumn> _columns = new();
    private readonly List<IForeignKey> _foreignKeys = new();
    private readonly List<IIndex> _indexes = new();
    private readonly List<IUniqueConstraint> _uniqueConstraints = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Table"/> class.
    /// </summary>
    /// <param name="name">The name of the table.</param>
    /// <param name="schema">The schema of the table.</param>
    public Table(string name, string schema = "dbo")
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Table name cannot be null or empty.", nameof(name));
        }

        Name = name;
        Schema = schema ?? "dbo";
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Schema { get; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IColumn> Columns => _columns.AsReadOnly();

    /// <inheritdoc/>
    public IKey? PrimaryKey { get; private set; }

    /// <inheritdoc/>
    public IReadOnlyCollection<IForeignKey> ForeignKeys => _foreignKeys.AsReadOnly();

    /// <inheritdoc/>
    public IReadOnlyCollection<IIndex> Indexes => _indexes.AsReadOnly();

    /// <inheritdoc/>
    public IReadOnlyCollection<IUniqueConstraint> UniqueConstraints => _uniqueConstraints.AsReadOnly();

    /// <inheritdoc/>
    public bool IsTemporal { get; set; }

    /// <inheritdoc/>
    public string? HistoryTableName { get; set; }

    /// <inheritdoc/>
    public IList<IColumn> MutableColumns => _columns;

    /// <inheritdoc/>
    public IList<IForeignKey> MutableForeignKeys => _foreignKeys;

    /// <inheritdoc/>
    public IList<IIndex> MutableIndexes => _indexes;

    /// <inheritdoc/>
    public IList<IUniqueConstraint> MutableUniqueConstraints => _uniqueConstraints;

    /// <summary>
    /// Adds a column to the table.
    /// </summary>
    /// <param name="column">The column to add.</param>
    public void AddColumn(IColumn column)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (_columns.Any(c => string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException($"Column with name '{column.Name}' already exists in the table.",
                nameof(column));
        }

        _columns.Add(column);
    }

    /// <summary>
    /// Adds multiple columns to the table.
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
    /// Sets the primary key of the table.
    /// </summary>
    /// <param name="primaryKey">The primary key.</param>
    public void SetPrimaryKey(IKey primaryKey)
    {
        if (primaryKey == null)
        {
            throw new ArgumentNullException(nameof(primaryKey));
        }

        // Verify that all columns in the key exist in the table
        foreach (var keyColumn in primaryKey.Columns)
        {
            if (!_columns.Any(c => string.Equals(c.Name, keyColumn.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Column '{keyColumn.Name}' in primary key does not exist in the table.",
                    nameof(primaryKey));
            }
        }

        PrimaryKey = primaryKey;
    }

    /// <summary>
    /// Adds a foreign key to the table.
    /// </summary>
    /// <param name="foreignKey">The foreign key to add.</param>
    public void AddForeignKey(IForeignKey foreignKey)
    {
        if (foreignKey == null)
        {
            throw new ArgumentNullException(nameof(foreignKey));
        }

        // Verify that all columns in the foreign key exist in the table
        foreach (var keyColumn in foreignKey.Columns)
        {
            if (!_columns.Any(c => string.Equals(c.Name, keyColumn.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Column '{keyColumn.Name}' in foreign key does not exist in the table.",
                    nameof(foreignKey));
            }
        }

        _foreignKeys.Add(foreignKey);
    }

    /// <summary>
    /// Adds multiple foreign keys to the table.
    /// </summary>
    /// <param name="foreignKeys">The foreign keys to add.</param>
    public void AddForeignKeys(IEnumerable<IForeignKey> foreignKeys)
    {
        if (foreignKeys == null)
        {
            throw new ArgumentNullException(nameof(foreignKeys));
        }

        foreach (var foreignKey in foreignKeys)
        {
            AddForeignKey(foreignKey);
        }
    }

    /// <summary>
    /// Adds an index to the table.
    /// </summary>
    /// <param name="index">The index to add.</param>
    public void AddIndex(IIndex index)
    {
        if (index == null)
        {
            throw new ArgumentNullException(nameof(index));
        }

        // Verify that all columns in the index exist in the table
        foreach (var indexColumn in index.Columns)
        {
            if (!_columns.Any(c => string.Equals(c.Name, indexColumn.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Column '{indexColumn.Name}' in index does not exist in the table.",
                    nameof(index));
            }
        }

        _indexes.Add(index);
    }

    /// <summary>
    /// Adds multiple indexes to the table.
    /// </summary>
    /// <param name="indexes">The indexes to add.</param>
    public void AddIndexes(IEnumerable<IIndex> indexes)
    {
        if (indexes == null)
        {
            throw new ArgumentNullException(nameof(indexes));
        }

        foreach (var index in indexes)
        {
            AddIndex(index);
        }
    }

    /// <summary>
    /// Adds a unique constraint to the table.
    /// </summary>
    /// <param name="uniqueConstraint">The unique constraint to add.</param>
    public void AddUniqueConstraint(IUniqueConstraint uniqueConstraint)
    {
        if (uniqueConstraint == null)
        {
            throw new ArgumentNullException(nameof(uniqueConstraint));
        }

        // Verify that all columns in the unique constraint exist in the table
        foreach (var constraintColumn in uniqueConstraint.Columns)
        {
            if (!_columns.Any(c => string.Equals(c.Name, constraintColumn.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(
                    $"Column '{constraintColumn.Name}' in unique constraint does not exist in the table.",
                    nameof(uniqueConstraint));
            }
        }

        _uniqueConstraints.Add(uniqueConstraint);
    }

    /// <summary>
    /// Adds multiple unique constraints to the table.
    /// </summary>
    /// <param name="uniqueConstraints">The unique constraints to add.</param>
    public void AddUniqueConstraints(IEnumerable<IUniqueConstraint> uniqueConstraints)
    {
        if (uniqueConstraints == null)
        {
            throw new ArgumentNullException(nameof(uniqueConstraints));
        }

        foreach (var uniqueConstraint in uniqueConstraints)
        {
            AddUniqueConstraint(uniqueConstraint);
        }
    }

    /// <summary>
    /// Gets a column by name.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The column, or null if not found.</returns>
    public IColumn? GetColumn(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Column name cannot be null or empty.", nameof(name));
        }

        return _columns.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the fully qualified name of the table (schema.table).
    /// </summary>
    /// <returns>The fully qualified name of the table.</returns>
    public string GetFullName()
    {
        return $"{Schema}.{Name}";
    }

    /// <summary>
    /// Gets a string representation of the table.
    /// </summary>
    /// <returns>A string representation of the table.</returns>
    public override string ToString()
    {
        return GetFullName();
    }
}