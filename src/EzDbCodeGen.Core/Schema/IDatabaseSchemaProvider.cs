namespace EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents a database schema provider that extracts schema information from a specific database type.
/// </summary>
public interface IDatabaseSchemaProvider
{
    /// <summary>
    /// Gets the database schema asynchronously using the provided connection string and options.
    /// </summary>
    /// <param name="connectionString">The connection string to the database.</param>
    /// <param name="options">Options for schema extraction.</param>
    /// <returns>A database schema.</returns>
    Task<IDatabaseSchema> GetSchemaAsync(string connectionString, SchemaProviderOptions options);
    
    /// <summary>
    /// Tests the connection to the database.
    /// </summary>
    /// <param name="connectionString">The connection string to test.</param>
    /// <returns>True if the connection is successful, false otherwise.</returns>
    Task<bool> TestConnectionAsync(string connectionString);
    
    /// <summary>
    /// Gets the provider type.
    /// </summary>
    string ProviderType { get; }
}
