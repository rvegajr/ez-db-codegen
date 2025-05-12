using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Schema
{
    /// <summary>
    /// Defines a provider for loading database schemas.
    /// </summary>
    public interface IDatabaseSchemaProvider
    {
        /// <summary>
        /// Gets the name of the database provider.
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Gets a value indicating whether this provider supports this connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to check.</param>
        /// <returns>True if this provider supports the connection string, false otherwise.</returns>
        bool SupportsConnectionString(string connectionString);

        /// <summary>
        /// Loads a database schema from a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="options">The options for loading the schema.</param>
        /// <returns>The loaded database schema.</returns>
        IDatabaseSchema LoadSchema(string connectionString, SchemaProviderOptions options = null);

        /// <summary>
        /// Asynchronously loads a database schema from a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="options">The options for loading the schema.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded database schema.</returns>
        Task<IDatabaseSchema> LoadSchemaAsync(string connectionString, SchemaProviderOptions options = null);

        /// <summary>
        /// Tests a connection to the database.
        /// </summary>
        /// <param name="connectionString">The connection string to test.</param>
        /// <returns>True if the connection succeeded, false otherwise.</returns>
        bool TestConnection(string connectionString);

        /// <summary>
        /// Asynchronously tests a connection to the database.
        /// </summary>
        /// <param name="connectionString">The connection string to test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if the connection succeeded, false otherwise.</returns>
        Task<bool> TestConnectionAsync(string connectionString);
    }
}
