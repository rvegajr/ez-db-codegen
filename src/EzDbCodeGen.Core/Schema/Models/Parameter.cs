using System;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IParameter interface representing a parameter for a stored procedure or function.
    /// </summary>
    public class Parameter : IParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dataType">The data type of the parameter.</param>
        public Parameter(string name, string dataType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string DataType { get; }

        /// <inheritdoc/>
        public bool IsOutput { get; set; }

        /// <inheritdoc/>
        public bool IsNullable { get; set; }

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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string direction = IsOutput ? "OUT" : "IN";
            return $"{Name} {direction} {DataType}";
        }
    }
}
