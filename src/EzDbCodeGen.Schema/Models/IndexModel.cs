using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents an index in a database table.
    /// </summary>
    public class IndexModel : IIndex
    {
        private readonly List<IIndexColumn> _columns = new List<IIndexColumn>();

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the table that the index belongs to.
        /// </summary>
        public ITable Table { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the index is unique.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is clustered.
        /// </summary>
        public bool IsClustered { get; set; }

        /// <summary>
        /// Gets or sets the type of the index.
        /// </summary>
        public string IndexType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the filter expression for the index, if any.
        /// </summary>
        public string? Filter { get; set; }

        /// <summary>
        /// Gets the collection of columns in the index.
        /// </summary>
        public IReadOnlyList<IIndexColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether this is a composite index (multiple columns).
        /// </summary>
        public bool IsComposite => Columns.Count > 1;

        /// <summary>
        /// Adds a column to the index.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IIndexColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            _columns.Add(column);
        }

        // Legacy collection accessor
        
        /// <summary>
        /// Gets the mutable collection of columns in the index.
        /// </summary>
        public IList<IIndexColumn> MutableColumns => _columns;
    }
}