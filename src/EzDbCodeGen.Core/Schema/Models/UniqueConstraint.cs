using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IUniqueConstraint interface representing a unique constraint in a database table.
    /// </summary>
    public class UniqueConstraint : IUniqueConstraint
    {
        private readonly List<IColumn> _columns = new();
        private readonly ITable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraint"/> class.
        /// </summary>
        /// <param name="name">The name of the unique constraint.</param>
        /// <param name="table">The table that contains the unique constraint.</param>
        public UniqueConstraint(string name, ITable table)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public ITable Table => _table;

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> Columns => _columns.AsReadOnly();

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

            if (column.Table != Table)
            {
                throw new ArgumentException($"Column '{column.Name}' does not belong to the table '{Table.FullName}'.", nameof(column));
            }

            if (_columns.Any(c => c == column))
            {
                throw new InvalidOperationException($"Column '{column.Name}' is already part of the unique constraint '{Name}'.");
            }

            _columns.Add(column);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string columnNames = string.Join(", ", _columns.Select(c => c.Name));
            return $"{Name} on {Table.FullName} ({columnNames})";
        }
    }
}
