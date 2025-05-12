using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.Models
{
    /// <summary>
    /// Implementation of the IDatabaseSchema interface representing a database schema.
    /// This is the container for all schema objects.
    /// </summary>
    public class DatabaseSchema : IDatabaseSchema
    {
        private readonly List<ITable> _tables = new();
        private readonly List<IStoredProcedure> _storedProcedures = new();
        private readonly List<IFunction> _functions = new();
        private readonly List<IRelationship> _relationships = new();
        private readonly Dictionary<string, object> _metadata = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchema"/> class.
        /// </summary>
        /// <param name="name">The name of the database schema.</param>
        /// <param name="provider">The database provider.</param>
        public DatabaseSchema(string name, DatabaseProvider provider)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Provider = provider;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public DatabaseProvider Provider { get; }

        /// <inheritdoc/>
        public string Server { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Database { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Schema { get; set; } = "dbo";

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public IReadOnlyCollection<ITable> Tables => _tables.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IStoredProcedure> StoredProcedures => _storedProcedures.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IFunction> Functions => _functions.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IRelationship> Relationships => _relationships.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, object> Metadata => _metadata;

        /// <inheritdoc/>
        public ITable? GetTable(string schema, string tableName)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
            }

            return _tables.FirstOrDefault(t => 
                string.Equals(t.Schema, schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(t.Name, tableName, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public IStoredProcedure? GetStoredProcedure(string schema, string procedureName)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(procedureName))
            {
                throw new ArgumentException("Procedure name cannot be null or empty.", nameof(procedureName));
            }

            return _storedProcedures.FirstOrDefault(p => 
                string.Equals(p.Schema, schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(p.Name, procedureName, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc/>
        public IFunction? GetFunction(string schema, string functionName)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(functionName))
            {
                throw new ArgumentException("Function name cannot be null or empty.", nameof(functionName));
            }

            return _functions.FirstOrDefault(f => 
                string.Equals(f.Schema, schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(f.Name, functionName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a table to the database schema.
        /// </summary>
        /// <param name="table">The table to add.</param>
        public void AddTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (_tables.Any(t => 
                string.Equals(t.Schema, table.Schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(t.Name, table.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Table '{table.FullName}' already exists in the database schema.");
            }

            _tables.Add(table);
        }

        /// <summary>
        /// Adds a stored procedure to the database schema.
        /// </summary>
        /// <param name="procedure">The stored procedure to add.</param>
        public void AddStoredProcedure(IStoredProcedure procedure)
        {
            if (procedure == null)
            {
                throw new ArgumentNullException(nameof(procedure));
            }

            if (_storedProcedures.Any(p => 
                string.Equals(p.Schema, procedure.Schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(p.Name, procedure.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Stored procedure '{procedure.FullName}' already exists in the database schema.");
            }

            _storedProcedures.Add(procedure);
        }

        /// <summary>
        /// Adds a function to the database schema.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(IFunction function)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            if (_functions.Any(f => 
                string.Equals(f.Schema, function.Schema, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(f.Name, function.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Function '{function.FullName}' already exists in the database schema.");
            }

            _functions.Add(function);
        }

        /// <summary>
        /// Adds a relationship to the database schema.
        /// </summary>
        /// <param name="relationship">The relationship to add.</param>
        public void AddRelationship(IRelationship relationship)
        {
            if (relationship == null)
            {
                throw new ArgumentNullException(nameof(relationship));
            }

            if (_relationships.Any(r => string.Equals(r.Name, relationship.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Relationship '{relationship.Name}' already exists in the database schema.");
            }

            _relationships.Add(relationship);
        }

        /// <summary>
        /// Sets a metadata value for the database schema.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public void SetMetadata(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Metadata key cannot be null or empty.", nameof(key));
            }

            _metadata[key] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{Name} ({Provider}) - {Tables.Count} tables, {StoredProcedures.Count} stored procedures, {Functions.Count} functions, {Relationships.Count} relationships";
        }
    }
}
