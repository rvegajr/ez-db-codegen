namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a factory for creating database schema providers.
    /// </summary>
    public interface IDatabaseSchemaProviderFactory
    {
        /// <summary>
        /// Gets a database schema provider for the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to get a provider for.</param>
        /// <returns>A database schema provider for the specified connection string, or null if no provider supports the connection string.</returns>
        IDatabaseSchemaProvider GetProvider(string connectionString);

        /// <summary>
        /// Gets a database schema provider by name.
        /// </summary>
        /// <param name="providerName">The name of the provider to get.</param>
        /// <returns>A database schema provider with the specified name, or null if no such provider exists.</returns>
        IDatabaseSchemaProvider GetProviderByName(string providerName);

        /// <summary>
        /// Registers a database schema provider with the factory.
        /// </summary>
        /// <param name="provider">The database schema provider to register.</param>
        void RegisterProvider(IDatabaseSchemaProvider provider);

        /// <summary>
        /// Gets all registered providers.
        /// </summary>
        /// <returns>A collection of all registered database schema providers.</returns>
        IReadOnlyCollection<IDatabaseSchemaProvider> GetAllProviders();
    }
}
