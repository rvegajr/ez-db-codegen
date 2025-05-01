using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a column in an index.
    /// </summary>
    public class IndexColumnModel : IIndexColumn
    {
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public IColumn Column { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ordinal position of the column in the index.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is sorted in descending order.
        /// </summary>
        public bool IsDescending { get; set; }

        /// <inheritdoc/>
        public string Name => Column?.Name ?? string.Empty;

        /// <summary>
        /// Gets the sort direction as a string.
        /// </summary>
        public string SortDirection => IsDescending ? "DESC" : "ASC";
    }
}