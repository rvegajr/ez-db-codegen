using System;
using System.Collections.Generic;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database column in a table.
    /// </summary>
    public class ColumnModel : IColumn
    {
        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the data type of the column.
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum length of the column.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the precision of the column.
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// Gets or sets the scale of the column.
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is an identity column.
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is computed.
        /// </summary>
        public bool IsComputed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is part of the primary key.
        /// </summary>
        public bool IsPartOfPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is part of a foreign key.
        /// </summary>
        public bool IsPartOfForeignKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is part of a unique constraint.
        /// </summary>
        public bool IsPartOfUniqueConstraint { get; set; }

        /// <summary>
        /// Gets or sets the default value of the column.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the computed column expression.
        /// </summary>
        public string? ComputedColumnExpression { get; set; }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the collation of the column.
        /// </summary>
        public string? Collation { get; set; }

        /// <summary>
        /// Gets or sets the ordinal position of the column in the table.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Gets or sets the table that contains this column.
        /// </summary>
        public ITable Table { get; set; } = null!;

        /// <summary>
        /// Gets the user-friendly display of the column's type with details.
        /// </summary>
        public string DisplayType
        {
            get
            {
                if (string.IsNullOrEmpty(DataType))
                    return string.Empty;

                // For character types, show the length
                if (DataType.Contains("char") || DataType.Contains("binary"))
                {
                    if (MaxLength == -1)
                        return $"{DataType}(max)";

                    // For NCHAR and NVARCHAR, the length is stored as 2x the actual length
                    if (DataType.StartsWith("n", StringComparison.OrdinalIgnoreCase))
                        return $"{DataType}({MaxLength / 2})";

                    return $"{DataType}({MaxLength})";
                }

                // For decimal/numeric types, show precision and scale
                if (DataType.Equals("decimal", StringComparison.OrdinalIgnoreCase) ||
                    DataType.Equals("numeric", StringComparison.OrdinalIgnoreCase))
                {
                    return $"{DataType}({Precision},{Scale})";
                }

                return DataType;
            }
        }
    }
}