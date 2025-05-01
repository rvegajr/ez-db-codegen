using System;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a parameter in a stored procedure or function.
    /// </summary>
    public class ParameterModel : IParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModel"/> class.
        /// </summary>
        public ParameterModel()
        {
            Name = string.Empty;
            DataType = string.Empty;
            DefaultValue = string.Empty;
            Description = string.Empty;
            Direction = ParameterDirection.Input;
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ordinal position of the parameter.
        /// </summary>
        public int OrdinalPosition { get; set; }

        /// <summary>
        /// Gets or sets the data type of the parameter.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter allows null values.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the parameter.
        /// </summary>
        public int? MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the precision of the parameter.
        /// </summary>
        public int? Precision { get; set; }

        /// <summary>
        /// Gets or sets the scale of the parameter.
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is an output parameter.
        /// </summary>
        public bool IsOutput { get; set; }

        /// <summary>
        /// Gets or sets the default value of the parameter, if any.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        /// <summary>
        /// Gets or sets the description of the parameter.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the user-friendly display of the parameter's type with details.
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