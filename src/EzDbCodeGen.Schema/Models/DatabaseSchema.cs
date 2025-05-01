using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database schema with tables, views, stored procedures, functions, and relationships.
    /// </summary>
    public class DatabaseSchema : IDatabaseSchema
    {
        private readonly List<ITable> _tables = new();
        private readonly List<IView> _views = new();
        private readonly List<IStoredProcedure> _storedProcedures = new();
        private readonly List<IFunction> _functions = new();
        private readonly List<IRelationship> _relationships = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchema"/> class.
        /// </summary>
        /// <param name="name">The name of the database.</param>
        public DatabaseSchema(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DatabaseName = name;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string DatabaseName { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ITable> Tables => _tables.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IView> Views => _views.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IStoredProcedure> StoredProcedures => _storedProcedures.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IFunction> Functions => _functions.AsReadOnly();

        /// <inheritdoc/>
        public IReadOnlyCollection<IRelationship> Relationships => _relationships.AsReadOnly();

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

            _tables.Add(table);
        }

        /// <summary>
        /// Adds multiple tables to the database schema.
        /// </summary>
        /// <param name="tables">The tables to add.</param>
        public void AddTables(IEnumerable<ITable> tables)
        {
            if (tables == null)
            {
                throw new ArgumentNullException(nameof(tables));
            }

            _tables.AddRange(tables);
        }

        /// <summary>
        /// Adds a view to the database schema.
        /// </summary>
        /// <param name="view">The view to add.</param>
        public void AddView(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            _views.Add(view);
        }

        /// <summary>
        /// Adds multiple views to the database schema.
        /// </summary>
        /// <param name="views">The views to add.</param>
        public void AddViews(IEnumerable<IView> views)
        {
            if (views == null)
            {
                throw new ArgumentNullException(nameof(views));
            }

            _views.AddRange(views);
        }

        /// <summary>
        /// Adds a stored procedure to the database schema.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure to add.</param>
        public void AddStoredProcedure(IStoredProcedure storedProcedure)
        {
            if (storedProcedure == null)
            {
                throw new ArgumentNullException(nameof(storedProcedure));
            }

            _storedProcedures.Add(storedProcedure);
        }

        /// <summary>
        /// Adds multiple stored procedures to the database schema.
        /// </summary>
        /// <param name="storedProcedures">The stored procedures to add.</param>
        public void AddStoredProcedures(IEnumerable<IStoredProcedure> storedProcedures)
        {
            if (storedProcedures == null)
            {
                throw new ArgumentNullException(nameof(storedProcedures));
            }

            _storedProcedures.AddRange(storedProcedures);
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

            _functions.Add(function);
        }

        /// <summary>
        /// Adds multiple functions to the database schema.
        /// </summary>
        /// <param name="functions">The functions to add.</param>
        public void AddFunctions(IEnumerable<IFunction> functions)
        {
            if (functions == null)
            {
                throw new ArgumentNullException(nameof(functions));
            }

            _functions.AddRange(functions);
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

            _relationships.Add(relationship);
        }

        /// <summary>
        /// Adds multiple relationships to the database schema.
        /// </summary>
        /// <param name="relationships">The relationships to add.</param>
        public void AddRelationships(IEnumerable<IRelationship> relationships)
        {
            if (relationships == null)
            {
                throw new ArgumentNullException(nameof(relationships));
            }

            _relationships.AddRange(relationships);
        }

        /// <summary>
        /// Gets a table by name.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <returns>The table, or null if not found.</returns>
        public ITable GetTable(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(name));
            }

            return _tables.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a table by schema and name.
        /// </summary>
        /// <param name="schema">The schema of the table.</param>
        /// <param name="name">The name of the table.</param>
        /// <returns>The table, or null if not found.</returns>
        public ITable GetTable(string schema, string name)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Table name cannot be null or empty.", nameof(name));
            }

            return _tables.FirstOrDefault(t =>
                string.Equals(t.Schema, schema, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a view by name.
        /// </summary>
        /// <param name="name">The name of the view.</param>
        /// <returns>The view, or null if not found.</returns>
        public IView GetView(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("View name cannot be null or empty.", nameof(name));
            }

            return _views.FirstOrDefault(v => string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a view by schema and name.
        /// </summary>
        /// <param name="schema">The schema of the view.</param>
        /// <param name="name">The name of the view.</param>
        /// <returns>The view, or null if not found.</returns>
        public IView GetView(string schema, string name)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("View name cannot be null or empty.", nameof(name));
            }

            return _views.FirstOrDefault(v =>
                string.Equals(v.Schema, schema, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(v.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a stored procedure by name.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <returns>The stored procedure, or null if not found.</returns>
        public IStoredProcedure GetStoredProcedure(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(name));
            }

            return _storedProcedures.FirstOrDefault(sp => string.Equals(sp.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a stored procedure by schema and name.
        /// </summary>
        /// <param name="schema">The schema of the stored procedure.</param>
        /// <param name="name">The name of the stored procedure.</param>
        /// <returns>The stored procedure, or null if not found.</returns>
        public IStoredProcedure GetStoredProcedure(string schema, string name)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(name));
            }

            return _storedProcedures.FirstOrDefault(sp =>
                string.Equals(sp.Schema, schema, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(sp.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a function by name.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The function, or null if not found.</returns>
        public IFunction GetFunction(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Function name cannot be null or empty.", nameof(name));
            }

            return _functions.FirstOrDefault(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a function by schema and name.
        /// </summary>
        /// <param name="schema">The schema of the function.</param>
        /// <param name="name">The name of the function.</param>
        /// <returns>The function, or null if not found.</returns>
        public IFunction GetFunction(string schema, string name)
        {
            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema cannot be null or empty.", nameof(schema));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Function name cannot be null or empty.", nameof(name));
            }

            return _functions.FirstOrDefault(f =>
                string.Equals(f.Schema, schema, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a relationship by source and target tables.
        /// </summary>
        /// <param name="sourceTable">The source table of the relationship.</param>
        /// <param name="targetTable">The target table of the relationship.</param>
        /// <returns>The relationship, or null if not found.</returns>
        public IRelationship GetRelationship(ITable sourceTable, ITable targetTable)
        {
            if (sourceTable == null)
            {
                throw new ArgumentNullException(nameof(sourceTable));
            }

            if (targetTable == null)
            {
                throw new ArgumentNullException(nameof(targetTable));
            }

            return _relationships.FirstOrDefault(r =>
                r.SourceTable == sourceTable && r.TargetTable == targetTable);
        }
    }
}