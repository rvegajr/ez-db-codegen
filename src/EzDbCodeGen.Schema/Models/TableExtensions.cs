using System;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Extension methods for ITable interface.
    /// </summary>
    public static class TableExtensions
    {
        /// <summary>
        /// Gets a column by name from an ITable.
        /// </summary>
        /// <param name="table">The table to get the column from.</param>
        /// <param name="columnName">The name of the column to get.</param>
        /// <returns>The column, or null if not found.</returns>
        public static IColumn GetColumn(this ITable table, string columnName)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));
            }

            return table.Columns.FirstOrDefault(c => 
                string.Equals(c.Name, columnName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
