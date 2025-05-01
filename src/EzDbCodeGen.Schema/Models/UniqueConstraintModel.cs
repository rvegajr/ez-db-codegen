using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a unique constraint in a database table.
    /// </summary>
    public class UniqueConstraintModel : IUniqueConstraint
    {
        private readonly List<IColumn> _columns = new List<IColumn>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueConstraintModel"/> class.
        /// </summary>
        public UniqueConstraintModel()
        {
            Name = string.Empty;
            Table = null!;
        }

        /// <summary>
        /// Gets or sets the name of the unique constraint.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the collection of columns that make up the unique constraint.
        /// </summary>
        public IReadOnlyList<IColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets or sets the table that contains this unique constraint.
        /// </summary>
        public ITable Table { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this unique constraint is clustered.
        /// </summary>
        public bool IsClustered { get; set; }

        /// <summary>
        /// Gets a value indicating whether this is a composite unique constraint (multiple columns).
        /// </summary>
        public bool IsComposite => Columns.Count > 1;

        /// <summary>
        /// Gets the mutable collection of columns that make up the unique constraint.
        /// </summary>
        public IList<IColumn> MutableColumns => _columns;

        /// <summary>
        /// Adds a column to the unique constraint.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            _columns.Add(column);
        }
    }
}