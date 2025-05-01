using System.Collections.Generic;
using System.Collections.ObjectModel;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database schema model.
    /// </summary>
    public class DatabaseSchemaModel : IDatabaseSchema
    {
        private readonly List<ITable> _tables = new();
        private readonly List<IView> _views = new();
        private readonly List<IStoredProcedure> _storedProcedures = new();
        private readonly List<IFunction> _functions = new();
        private readonly List<IRelationship> _relationships = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaModel"/> class.
        /// </summary>
        public DatabaseSchemaModel()
        {
            Name = string.Empty;
            DatabaseName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the name of the schema.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets the collection of tables in the schema.
        /// </summary>
        public IReadOnlyCollection<ITable> Tables => _tables.AsReadOnly();

        /// <summary>
        /// Gets the collection of views in the schema.
        /// </summary>
        public IReadOnlyCollection<IView> Views => _views.AsReadOnly();

        /// <summary>
        /// Gets the collection of stored procedures in the schema.
        /// </summary>
        public IReadOnlyCollection<IStoredProcedure> StoredProcedures => _storedProcedures.AsReadOnly();

        /// <summary>
        /// Gets the collection of functions in the schema.
        /// </summary>
        public IReadOnlyCollection<IFunction> Functions => _functions.AsReadOnly();

        /// <summary>
        /// Gets the collection of relationships in the schema.
        /// </summary>
        public IReadOnlyCollection<IRelationship> Relationships => _relationships.AsReadOnly();

        /// <summary>
        /// Gets the mutable collection of tables in the schema.
        /// This is for internal use only and should not be exposed through the public API.
        /// </summary>
        internal List<ITable> MutableTables => _tables;

        /// <summary>
        /// Gets the mutable collection of views in the schema.
        /// This is for internal use only and should not be exposed through the public API.
        /// </summary>
        internal List<IView> MutableViews => _views;

        /// <summary>
        /// Gets the mutable collection of stored procedures in the schema.
        /// This is for internal use only and should not be exposed through the public API.
        /// </summary>
        internal List<IStoredProcedure> MutableStoredProcedures => _storedProcedures;

        /// <summary>
        /// Gets the mutable collection of functions in the schema.
        /// This is for internal use only and should not be exposed through the public API.
        /// </summary>
        internal List<IFunction> MutableFunctions => _functions;

        /// <summary>
        /// Gets the mutable collection of relationships in the schema.
        /// This is for internal use only and should not be exposed through the public API.
        /// </summary>
        internal List<IRelationship> MutableRelationships => _relationships;

        /// <summary>
        /// Adds a table to the schema.
        /// </summary>
        /// <param name="table">The table to add.</param>
        public void AddTable(ITable table)
        {
            _tables.Add(table);
        }

        /// <summary>
        /// Adds a view to the schema.
        /// </summary>
        /// <param name="view">The view to add.</param>
        public void AddView(IView view)
        {
            _views.Add(view);
        }

        /// <summary>
        /// Adds a stored procedure to the schema.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure to add.</param>
        public void AddStoredProcedure(IStoredProcedure storedProcedure)
        {
            _storedProcedures.Add(storedProcedure);
        }

        /// <summary>
        /// Adds a function to the schema.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(IFunction function)
        {
            _functions.Add(function);
        }

        /// <summary>
        /// Adds a relationship to the schema.
        /// </summary>
        /// <param name="relationship">The relationship to add.</param>
        public void AddRelationship(IRelationship relationship)
        {
            _relationships.Add(relationship);
        }
    }
}