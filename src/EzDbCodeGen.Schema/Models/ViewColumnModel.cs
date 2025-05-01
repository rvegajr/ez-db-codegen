using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a column in a database view.
    /// </summary>
    public class ViewColumnModel : IViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewColumnModel"/> class.
        /// </summary>
        public ViewColumnModel()
        {
            Name = string.Empty;
            DataType = string.Empty;
            Description = string.Empty;
            View = null!;
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the data type of the column.
        /// </summary>
        public string DataType { get; set; }

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
        /// Gets or sets the ordinal position of the column in the view.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the view that this column belongs to.
        /// </summary>
        public IView View { get; set; }

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

                    if (MaxLength.HasValue)
                    {
                        // For NCHAR and NVARCHAR, the length is stored as 2x the actual length
                        if (DataType.StartsWith("n", StringComparison.OrdinalIgnoreCase))
                            return $"{DataType}({MaxLength.Value / 2})";

                        return $"{DataType}({MaxLength.Value})";
                    }
                }

                // For decimal/numeric types, show precision and scale
                if ((DataType.Equals("decimal", StringComparison.OrdinalIgnoreCase) ||
                     DataType.Equals("numeric", StringComparison.OrdinalIgnoreCase)) &&
                    Precision.HasValue && Scale.HasValue)
                {
                    return $"{DataType}({Precision.Value},{Scale.Value})";
                }

                return DataType;
            }
        }
    }
}