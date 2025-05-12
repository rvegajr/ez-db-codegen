using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IFunction interface representing a database function.
    /// </summary>
    public class Function : IFunction
    {
        private readonly List<IParameter> _parameters = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Function"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="schema">The schema name of the function.</param>
        /// <param name="returnType">The return type of the function.</param>
        public Function(string name, string schema, string returnType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string Schema { get; }

        /// <inheritdoc/>
        public string FullName => $"{Schema}.{Name}";

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string ReturnType { get; }

        /// <inheritdoc/>
        public bool IsTableValued { get; set; }

        /// <inheritdoc/>
        public string Definition { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameter> Parameters => _parameters.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IColumn> ReturnColumns { get; set; } = new List<IColumn>().AsReadOnly();

        /// <inheritdoc/>
        public IParameter? GetParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentException("Parameter name cannot be null or empty.", nameof(parameterName));
            }

            return _parameters.FirstOrDefault(p => 
                string.Equals(p.Name, parameterName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a parameter to the function.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        public void AddParameter(IParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (_parameters.Any(p => string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Parameter '{parameter.Name}' already exists in the function '{FullName}'.");
            }

            _parameters.Add(parameter);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            string parameters = string.Join(", ", _parameters.Select(p => p.ToString()));
            string returnTypeStr = IsTableValued ? "TABLE" : ReturnType;
            return $"{FullName}({parameters}) RETURNS {returnTypeStr}";
        }
    }
}
