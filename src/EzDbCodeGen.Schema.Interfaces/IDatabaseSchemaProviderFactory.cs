using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Represents a factory for creating database schema providers.
/// </summary>
public interface IDatabaseSchemaProviderFactory
{
    /// <summary>
    /// Creates a database schema provider for a specific database provider and connection string.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>A database schema provider.</returns>
    IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString);
    
    /// <summary>
    /// Creates a database schema provider for a specific database provider and connection string with the specified options.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="options">The schema provider options.</param>
    /// <returns>A database schema provider.</returns>
    IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString, SchemaProviderOptions options);
    
    /// <summary>
    /// Registers a database schema provider factory method.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterProviderFactory(string providerName, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider> factoryMethod);
    
    /// <summary>
    /// Gets all available database provider names.
    /// </summary>
    /// <returns>A list of available database provider names.</returns>
    IReadOnlyList<string> GetAvailableProviders();
    
    /// <summary>
    /// Checks if a provider is registered for a specific database provider.
    /// </summary>
    /// <param name="providerName">The name of the database provider.</param>
    /// <returns>True if a provider is registered for the database provider, false otherwise.</returns>
    bool IsProviderRegistered(string providerName);
}
