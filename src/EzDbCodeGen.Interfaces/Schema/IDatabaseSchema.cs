using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a database schema containing tables, views, stored procedures, and functions.
    /// </summary>
    public interface IDatabaseSchema
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Gets the server name hosting the database.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        string DatabaseVersion { get; }

        /// <summary>
        /// Gets the collection of tables in the database.
        /// </summary>
        IReadOnlyCollection<ITable> Tables { get; }

        /// <summary>
        /// Gets the collection of views in the database.
        /// </summary>
        IReadOnlyCollection<ITable> Views { get; }

        /// <summary>
        /// Gets the collection of stored procedures in the database.
        /// </summary>
        IReadOnlyCollection<IStoredProcedure> StoredProcedures { get; }

        /// <summary>
        /// Gets the collection of functions in the database.
        /// </summary>
        IReadOnlyCollection<IFunction> Functions { get; }
        
        /// <summary>
        /// Gets the collection of relationships in the database.
        /// </summary>
        IReadOnlyCollection<IRelationship> Relationships { get; }

        /// <summary>
        /// Gets a table by its name.
        /// </summary>
        /// <param name="tableName">The name of the table to get.</param>
        /// <param name="schemaName">The schema name of the table to get. If null, the default schema is used.</param>
        /// <returns>The table with the specified name, or null if no such table exists.</returns>
        ITable GetTable(string tableName, string schemaName = null);

        /// <summary>
        /// Gets a view by its name.
        /// </summary>
        /// <param name="viewName">The name of the view to get.</param>
        /// <param name="schemaName">The schema name of the view to get. If null, the default schema is used.</param>
        /// <returns>The view with the specified name, or null if no such view exists.</returns>
        ITable GetView(string viewName, string schemaName = null);

        /// <summary>
        /// Gets a stored procedure by its name.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to get.</param>
        /// <param name="schemaName">The schema name of the stored procedure to get. If null, the default schema is used.</param>
        /// <returns>The stored procedure with the specified name, or null if no such stored procedure exists.</returns>
        IStoredProcedure GetStoredProcedure(string procedureName, string schemaName = null);

        /// <summary>
        /// Gets a function by its name.
        /// </summary>
        /// <param name="functionName">The name of the function to get.</param>
        /// <param name="schemaName">The schema name of the function to get. If null, the default schema is used.</param>
        /// <returns>The function with the specified name, or null if no such function exists.</returns>
        IFunction GetFunction(string functionName, string schemaName = null);
    }
}
