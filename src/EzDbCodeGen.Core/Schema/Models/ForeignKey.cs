using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IForeignKey interface representing a foreign key constraint in a database.
    /// </summary>
    public class ForeignKey : IForeignKey
    {
        private readonly List<(IColumn SourceColumn, IColumn ReferencedColumn)> _columnPairs = new();
        private readonly ITable _sourceTable;
        private readonly ITable _referencedTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKey"/> class.
        /// </summary>
        /// <param name="name">The name of the foreign key.</param>
        /// <param name="sourceTable">The table that contains the foreign key.</param>
        /// <param name="referencedTable">The table that the foreign key references.</param>
        public ForeignKey(string name, ITable sourceTable, ITable referencedTable)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _sourceTable = sourceTable ?? throw new ArgumentNullException(nameof(sourceTable));
            _referencedTable = referencedTable ?? throw new ArgumentNullException(nameof(referencedTable));
            OnUpdateAction = ReferentialAction.NoAction;
            OnDeleteAction = ReferentialAction.NoAction;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public ITable SourceTable => _sourceTable;

        /// <inheritdoc/>
        public ITable ReferencedTable => _referencedTable;

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> SourceColumns => _columnPairs.Select(p => p.SourceColumn).ToList().AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> ReferencedColumns => _columnPairs.Select(p => p.ReferencedColumn).ToList().AsReadOnly();

        /// <inheritdoc/>
        public ReferentialAction OnUpdateAction { get; set; }

        /// <inheritdoc/>
        public ReferentialAction OnDeleteAction { get; set; }

        /// <summary>
        /// Adds a column pair to the foreign key.
        /// </summary>
        /// <param name="sourceColumn">The source column in the foreign key.</param>
        /// <param name="referencedColumn">The referenced column in the foreign key.</param>
        public void AddColumnPair(IColumn sourceColumn, IColumn referencedColumn)
        {
            if (sourceColumn == null)
            {
                throw new ArgumentNullException(nameof(sourceColumn));
            }

            if (referencedColumn == null)
            {
                throw new ArgumentNullException(nameof(referencedColumn));
            }

            if (sourceColumn.Table != SourceTable)
            {
                throw new ArgumentException($"Source column '{sourceColumn.Name}' does not belong to the source table '{SourceTable.FullName}'.", nameof(sourceColumn));
            }

            if (referencedColumn.Table != ReferencedTable)
            {
                throw new ArgumentException($"Referenced column '{referencedColumn.Name}' does not belong to the referenced table '{ReferencedTable.FullName}'.", nameof(referencedColumn));
            }

            if (_columnPairs.Any(p => p.SourceColumn == sourceColumn))
            {
                throw new InvalidOperationException($"Source column '{sourceColumn.Name}' is already part of the foreign key '{Name}'.");
            }

            _columnPairs.Add((sourceColumn, referencedColumn));
        }

        /// <summary>
        /// Determines whether the foreign key represents a self-reference (i.e., the source table and referenced table are the same).
        /// </summary>
        /// <returns>True if the foreign key is a self-reference; otherwise, false.</returns>
        public bool IsSelfReference()
        {
            return SourceTable == ReferencedTable;
        }

        /// <summary>
        /// Determines whether the foreign key is a simple reference (i.e., it has only one column pair).
        /// </summary>
        /// <returns>True if the foreign key is a simple reference; otherwise, false.</returns>
        public bool IsSimpleReference()
        {
            return _columnPairs.Count == 1;
        }

        /// <summary>
        /// Determines whether the foreign key is part of the primary key of the source table.
        /// </summary>
        /// <returns>True if the foreign key is part of the primary key; otherwise, false.</returns>
        public bool IsPartOfPrimaryKey()
        {
            return SourceColumns.All(c => c.IsPrimaryKey);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string columnPairs = string.Join(", ", _columnPairs.Select(p => $"{p.SourceColumn.Name} -> {p.ReferencedColumn.Name}"));
            return $"{Name}: {SourceTable.FullName} -> {ReferencedTable.FullName} ({columnPairs})";
        }
    }
}
