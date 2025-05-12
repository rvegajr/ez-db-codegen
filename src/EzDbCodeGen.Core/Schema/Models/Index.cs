using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IIndex interface representing an index in a database table.
    /// </summary>
    public class Index : IIndex
    {
        private readonly List<IColumn> _columns = new();
        private readonly List<IColumn> _includedColumns = new();
        private readonly ITable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="Index"/> class.
        /// </summary>
        /// <param name="name">The name of the index.</param>
        /// <param name="table">The table that contains the index.</param>
        public Index(string name, ITable table)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public ITable Table => _table;

        /// <inheritdoc/>
        public bool IsUnique { get; set; }

        /// <inheritdoc/>
        public bool IsClustered { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> Columns => _columns.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> IncludedColumns => _includedColumns.AsReadOnly();

        /// <summary>
        /// Adds a column to the index.
        /// </summary>
        /// <param name="column">The column to add.</param>
        /// <param name="isIncluded">Whether the column is included but not part of the key.</param>
        public void AddColumn(IColumn column, bool isIncluded = false)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (column.Table != Table)
            {
                throw new ArgumentException($"Column '{column.Name}' does not belong to the table '{Table.FullName}'.", nameof(column));
            }

            if (isIncluded)
            {
                if (_includedColumns.Any(c => c == column))
                {
                    throw new InvalidOperationException($"Column '{column.Name}' is already part of the included columns of index '{Name}'.");
                }

                _includedColumns.Add(column);
            }
            else
            {
                if (_columns.Any(c => c == column))
                {
                    throw new InvalidOperationException($"Column '{column.Name}' is already part of the key columns of index '{Name}'.");
                }

                _columns.Add(column);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string uniqueStr = IsUnique ? "UNIQUE " : string.Empty;
            string clusteredStr = IsClustered ? "CLUSTERED " : "NONCLUSTERED ";
            string keyColumns = string.Join(", ", _columns.Select(c => c.Name));
            string includedStr = _includedColumns.Any() 
                ? $" INCLUDE ({string.Join(", ", _includedColumns.Select(c => c.Name))})" 
                : string.Empty;

            return $"{Name} on {Table.FullName}: {uniqueStr}{clusteredStr}({keyColumns}){includedStr}";
        }
    }
}
