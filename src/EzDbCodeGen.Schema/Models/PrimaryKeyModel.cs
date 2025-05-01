using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a primary key constraint in a database table.
    /// </summary>
    public class PrimaryKeyModel : IKey
    {
        private readonly List<IColumn> _columns = new List<IColumn>();

        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the collection of columns that make up the primary key.
        /// </summary>
        public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets or sets the table that the primary key belongs to.
        /// </summary>
        public ITable Table { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the primary key is clustered.
        /// </summary>
        public bool IsClustered { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a composite primary key (multiple columns).
        /// </summary>
        public bool IsComposite => Columns.Count > 1;

        /// <summary>
        /// Adds a column to the primary key.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            _columns.Add(column);
        }

        // Legacy collection accessor
        
        /// <summary>
        /// Gets the mutable collection of columns in the primary key.
        /// </summary>
        public IList<IColumn> MutableColumns => _columns;
    }
}