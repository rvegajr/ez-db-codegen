using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a user-defined function in a database schema.
    /// </summary>
    public class FunctionModel : IFunction
    {
        private readonly List<IParameter> _parameters = new List<IParameter>();
        private readonly List<IResultColumn> _resultColumns = new List<IResultColumn>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionModel"/> class.
        /// </summary>
        public FunctionModel()
        {
            Name = string.Empty;
            Schema = string.Empty;
            ReturnType = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the schema of the function.
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Gets the parameters of the function.
        /// </summary>
        public IReadOnlyCollection<IParameter> Parameters => _parameters.AsReadOnly();

        /// <summary>
        /// Gets or sets the return type of the function.
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// Gets the columns returned by the function, if it's a table-valued function.
        /// </summary>
        public IReadOnlyCollection<IResultColumn>? ResultColumns =>
            _resultColumns.Count > 0 ? _resultColumns.AsReadOnly() : null;

        /// <summary>
        /// Gets or sets a value indicating whether the function is a table-valued function.
        /// </summary>
        public bool IsTableValued { get; set; }

        /// <summary>
        /// Gets or sets the definition of the function.
        /// </summary>
        public string? Definition { get; set; }

        /// <summary>
        /// Gets the fully qualified name of the function, including the schema.
        /// </summary>
        public string FullName => string.IsNullOrEmpty(Schema) ? Name : $"{Schema}.{Name}";

        /// <summary>
        /// Adds a parameter to the function.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        public void AddParameter(IParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            _parameters.Add(parameter);
        }

        /// <summary>
        /// Adds a result column to the function.
        /// </summary>
        /// <param name="resultColumn">The result column to add.</param>
        public void AddResultColumn(IResultColumn resultColumn)
        {
            if (resultColumn == null)
                throw new ArgumentNullException(nameof(resultColumn));

            _resultColumns.Add(resultColumn);
        }
    }
}