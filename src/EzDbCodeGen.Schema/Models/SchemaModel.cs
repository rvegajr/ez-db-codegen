using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema.Models
{
    /// <summary>
    /// Represents a database schema model containing tables, views, and other schema elements.
    /// </summary>
    public class SchemaModel : ISchemaModel, IDatabaseSchema
    {
        private readonly List<IRelationship> _relationships = new List<IRelationship>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaModel"/> class.
        /// </summary>
        public SchemaModel()
        {
            DatabaseName = string.Empty;
            Name = string.Empty;
        }
        
        /// <summary>
        /// Gets the collection of tables in the schema.
        /// </summary>
        public IList<ITable> Tables { get; } = new List<ITable>();

        /// <summary>
        /// Gets the collection of views in the schema.
        /// </summary>
        public IList<IView> Views { get; } = new List<IView>();

        /// <summary>
        /// Gets the collection of stored procedures in the schema.
        /// </summary>
        public IList<IStoredProcedure> StoredProcedures { get; } = new List<IStoredProcedure>();

        /// <summary>
        /// Gets the collection of functions in the schema.
        /// </summary>
        public IList<IFunction> Functions { get; } = new List<IFunction>();

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the version of the schema.
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets the timestamp when the schema was extracted.
        /// </summary>
        public DateTime ExtractionTimestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the provider that extracted this schema.
        /// </summary>
        public string Provider { get; set; } = "SqlServer";
        
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the tables in the database as a read-only collection.
        /// </summary>
        IReadOnlyCollection<ITable> IDatabaseSchema.Tables => Tables.AsReadOnly();
        
        /// <summary>
        /// Gets the views in the database as a read-only collection.
        /// </summary>
        IReadOnlyCollection<IView> IDatabaseSchema.Views => Views.AsReadOnly();
        
        /// <summary>
        /// Gets the stored procedures in the database as a read-only collection.
        /// </summary>
        IReadOnlyCollection<IStoredProcedure> IDatabaseSchema.StoredProcedures => StoredProcedures.AsReadOnly();
        
        /// <summary>
        /// Gets the functions in the database as a read-only collection.
        /// </summary>
        IReadOnlyCollection<IFunction> IDatabaseSchema.Functions => Functions.AsReadOnly();
        
        /// <summary>
        /// Gets the relationships in the database as a read-only collection.
        /// </summary>
        public IReadOnlyCollection<IRelationship> Relationships => _relationships.AsReadOnly();
        
        /// <summary>
        /// Adds a table to the database schema.
        /// </summary>
        /// <param name="table">The table to add.</param>
        public void AddTable(ITable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
                
            Tables.Add(table);
        }
        
        /// <summary>
        /// Adds a view to the database schema.
        /// </summary>
        /// <param name="view">The view to add.</param>
        public void AddView(IView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
                
            Views.Add(view);
        }
        
        /// <summary>
        /// Adds a stored procedure to the database schema.
        /// </summary>
        /// <param name="storedProcedure">The stored procedure to add.</param>
        public void AddStoredProcedure(IStoredProcedure storedProcedure)
        {
            if (storedProcedure == null)
                throw new ArgumentNullException(nameof(storedProcedure));
                
            StoredProcedures.Add(storedProcedure);
        }
        
        /// <summary>
        /// Adds a function to the database schema.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(IFunction function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
                
            Functions.Add(function);
        }
        
        /// <summary>
        /// Adds a relationship to the database schema.
        /// </summary>
        /// <param name="relationship">The relationship to add.</param>
        public void AddRelationship(IRelationship relationship)
        {
            if (relationship == null)
                throw new ArgumentNullException(nameof(relationship));
                
            _relationships.Add(relationship);
        }
    }
}