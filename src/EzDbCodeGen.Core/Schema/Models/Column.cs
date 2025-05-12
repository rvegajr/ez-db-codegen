using System;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IColumn interface representing a column in a database table or view.
    /// </summary>
    public class Column : IColumn
    {
        private readonly ITable _table;

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="dataType">The database type of the column.</param>
        /// <param name="table">The table that contains this column.</param>
        public Column(string name, string dataType, ITable table)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            _table = table ?? throw new ArgumentNullException(nameof(table));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string DataType { get; }

        /// <inheritdoc/>
        public bool IsNullable { get; set; }

        /// <inheritdoc/>
        public bool IsPrimaryKey { get; set; }

        /// <inheritdoc/>
        public bool IsIdentity { get; set; }

        /// <inheritdoc/>
        public bool IsComputed { get; set; }

        /// <inheritdoc/>
        public int OrdinalPosition { get; set; }

        /// <inheritdoc/>
        public int? MaxLength { get; set; }

        /// <inheritdoc/>
        public int? Precision { get; set; }

        /// <inheritdoc/>
        public int? Scale { get; set; }

        /// <inheritdoc/>
        public string DefaultValue { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public ITable Table => _table;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Table.Name}.{Name} ({DataType})";
        }

        /// <summary>
        /// Creates a new Column with the same properties as this one but a different table.
        /// </summary>
        /// <param name="table">The new table to associate with the column.</param>
        /// <returns>A new Column instance with properties copied from this one.</returns>
        public Column CloneWithTable(ITable table)
        {
            var clone = new Column(Name, DataType, table)
            {
                IsNullable = IsNullable,
                IsPrimaryKey = IsPrimaryKey,
                IsIdentity = IsIdentity,
                IsComputed = IsComputed,
                OrdinalPosition = OrdinalPosition,
                MaxLength = MaxLength,
                Precision = Precision,
                Scale = Scale,
                DefaultValue = DefaultValue,
                Description = Description
            };
            
            return clone;
        }
    }
}
