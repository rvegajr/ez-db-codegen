using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IStoredProcedure interface representing a stored procedure in a database.
    /// </summary>
    public class StoredProcedure : IStoredProcedure
    {
        private readonly List<IParameter> _parameters = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="schema">The schema name of the stored procedure.</param>
        public StoredProcedure(string name, string schema)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
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
        public string ReturnType { get; set; } = "void";

        /// <inheritdoc/>
        public string Definition { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IReadOnlyCollection<IParameter> Parameters => _parameters.AsReadOnly();

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
        /// Adds a parameter to the stored procedure.
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
                throw new InvalidOperationException($"Parameter '{parameter.Name}' already exists in the stored procedure '{FullName}'.");
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
            return $"{FullName}({parameters})";
        }
    }
}
