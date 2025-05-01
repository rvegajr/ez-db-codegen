namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a database schema with tables, views, stored procedures, functions, and relationships.
/// </summary>
public interface IDatabaseSchema
{
    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    string DatabaseName { get; set; }
    
    /// <summary>
    /// Gets the tables in the database.
    /// </summary>
    IReadOnlyCollection<ITable> Tables { get; }
    
    /// <summary>
    /// Gets the views in the database.
    /// </summary>
    IReadOnlyCollection<IView> Views { get; }
    
    /// <summary>
    /// Gets the stored procedures in the database.
    /// </summary>
    IReadOnlyCollection<IStoredProcedure> StoredProcedures { get; }
    
    /// <summary>
    /// Gets the functions in the database.
    /// </summary>
    IReadOnlyCollection<IFunction> Functions { get; }
    
    /// <summary>
    /// Gets the relationships in the database.
    /// </summary>
    IReadOnlyCollection<IRelationship> Relationships { get; }

    /// <summary>
    /// Adds a table to the database schema.
    /// </summary>
    /// <param name="table">The table to add.</param>
    void AddTable(ITable table);

    /// <summary>
    /// Adds a view to the database schema.
    /// </summary>
    /// <param name="view">The view to add.</param>
    void AddView(IView view);

    /// <summary>
    /// Adds a stored procedure to the database schema.
    /// </summary>
    /// <param name="storedProcedure">The stored procedure to add.</param>
    void AddStoredProcedure(IStoredProcedure storedProcedure);

    /// <summary>
    /// Adds a function to the database schema.
    /// </summary>
    /// <param name="function">The function to add.</param>
    void AddFunction(IFunction function);

    /// <summary>
    /// Adds a relationship to the database schema.
    /// </summary>
    /// <param name="relationship">The relationship to add.</param>
    void AddRelationship(IRelationship relationship);
}
