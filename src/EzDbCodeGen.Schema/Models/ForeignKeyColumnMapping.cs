using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a mapping between a column in a table and a referenced column in another table.
    /// </summary>
    public class ForeignKeyColumnMapping : IForeignKeyColumnMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyColumnMapping"/> class.
        /// </summary>
        public ForeignKeyColumnMapping()
        {
            SourceColumn = null!;
            ReferencedColumn = null!;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignKeyColumnMapping"/> class.
        /// </summary>
        /// <param name="sourceColumn">The source column.</param>
        /// <param name="referencedColumn">The referenced column.</param>
        public ForeignKeyColumnMapping(IColumn sourceColumn, IColumn referencedColumn)
        {
            SourceColumn = sourceColumn;
            ReferencedColumn = referencedColumn;
        }

        /// <summary>
        /// Gets or sets the source column.
        /// </summary>
        public IColumn SourceColumn { get; set; }

        /// <summary>
        /// Gets or sets the referenced column.
        /// </summary>
        public IColumn ReferencedColumn { get; set; }
    }
}
