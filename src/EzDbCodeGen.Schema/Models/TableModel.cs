using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database table in the schema.
    /// </summary>
    public class TableModel : ITable
    {
        private readonly List<IColumn> _columns = new List<IColumn>();
        private readonly List<IForeignKey> _foreignKeys = new List<IForeignKey>();
        private readonly List<IIndex> _indexes = new List<IIndex>();
        private readonly List<IUniqueConstraint> _uniqueConstraints = new List<IUniqueConstraint>();

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the schema of the table.
        /// </summary>
        public string Schema { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the table.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets the collection of columns in the table.
        /// </summary>
        public IReadOnlyCollection<IColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets or sets the primary key of the table.
        /// </summary>
        public IKey? PrimaryKey { get; set; }

        /// <summary>
        /// Gets the collection of foreign keys in the table.
        /// </summary>
        public IReadOnlyCollection<IForeignKey> ForeignKeys => _foreignKeys.AsReadOnly();

        /// <summary>
        /// Gets the collection of indexes in the table.
        /// </summary>
        public IReadOnlyCollection<IIndex> Indexes => _indexes.AsReadOnly();

        /// <summary>
        /// Gets the collection of unique constraints in the table.
        /// </summary>
        public IReadOnlyCollection<IUniqueConstraint> UniqueConstraints => _uniqueConstraints.AsReadOnly();

        /// <summary>
        /// Gets the fully qualified name of the table, including the schema.
        /// </summary>
        public string FullName => string.IsNullOrEmpty(Schema) ? Name : $"{Schema}.{Name}";

        /// <summary>
        /// Gets a value indicating whether the table is a junction table.
        /// </summary>
        public bool IsJunctionTable { get; set; }

        /// <summary>
        /// Gets a value indicating whether the table is a view.
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// Gets a value indicating whether the table is memory-optimized.
        /// </summary>
        public bool IsMemoryOptimized { get; set; }

        /// <summary>
        /// Gets a value indicating whether the table is a temporal table.
        /// </summary>
        public bool IsTemporal => TemporalType > 0;

        /// <summary>
        /// Gets or sets the temporal type of the table.
        /// </summary>
        public int TemporalType { get; set; }

        /// <summary>
        /// Gets or sets the history table name if this is a temporal table.
        /// </summary>
        public string? HistoryTableName { get; set; }

        // Legacy collection accessors

        /// <summary>
        /// Gets the mutable collection of columns in the table.
        /// </summary>
        public IList<IColumn> MutableColumns => _columns;

        /// <summary>
        /// Gets the mutable collection of foreign keys in the table.
        /// </summary>
        public IList<IForeignKey> MutableForeignKeys => _foreignKeys;

        /// <summary>
        /// Gets the mutable collection of indexes in the table.
        /// </summary>
        public IList<IIndex> MutableIndexes => _indexes;

        /// <summary>
        /// Gets the mutable collection of unique constraints in the table.
        /// </summary>
        public IList<IUniqueConstraint> MutableUniqueConstraints => _uniqueConstraints;
    }
}