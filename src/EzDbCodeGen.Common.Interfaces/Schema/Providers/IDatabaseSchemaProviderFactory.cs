namespace EzDbCodeGen.Common.Interfaces.Schema.Providers;

/// <summary>
/// Defines the interface for a factory that creates database schema providers.
/// </summary>
public interface IDatabaseSchemaProviderFactory
{
    /// <summary>
    /// Creates a database schema provider for the specified database type.
    /// </summary>
    /// <param name="databaseType">The type of database to create a provider for.</param>
    /// <returns>The created database schema provider.</returns>
    IDatabaseSchemaProvider CreateProvider(string databaseType);
    
    /// <summary>
    /// Gets a SQL Server schema provider.
    /// </summary>
    /// <returns>A SQL Server schema provider.</returns>
    ISqlServerSchemaProvider GetSqlServerProvider();
}
