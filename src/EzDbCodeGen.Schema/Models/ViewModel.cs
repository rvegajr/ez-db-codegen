using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database view in the schema.
    /// </summary>
    public class ViewModel : IView
    {
        private readonly List<IViewColumn> _columns = new List<IViewColumn>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            Name = string.Empty;
            Schema = string.Empty;
            Definition = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the view.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema of the view.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets the columns in the view.
        /// </summary>
        public IReadOnlyCollection<IViewColumn> Columns => _columns.AsReadOnly();

        /// <summary>
        /// Gets or sets the definition of the view.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the view is indexed.
        /// </summary>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Gets the fully qualified name of the view, including the schema.
        /// </summary>
        public string FullName => string.IsNullOrEmpty(Schema) ? Name : $"{Schema}.{Name}";

        /// <summary>
        /// Adds a column to the view.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(IViewColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            _columns.Add(column);
        }
    }
}