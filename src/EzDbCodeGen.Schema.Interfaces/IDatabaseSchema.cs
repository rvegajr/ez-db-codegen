namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a database schema with tables, views, stored procedures, and functions.
/// </summary>
public interface IDatabaseSchema
{
    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    string Name { get; }
    
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
}
