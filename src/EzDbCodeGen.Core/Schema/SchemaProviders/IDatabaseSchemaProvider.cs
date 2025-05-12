using System.Threading.Tasks;
using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Core.Schema.SchemaProviders
{
    /// <summary>
    /// Defines a provider for loading database schemas.
    /// </summary>
    public interface IDatabaseSchemaProvider
    {
        /// <summary>
        /// Gets the database provider type supported by this provider.
        /// </summary>
        DatabaseProvider ProviderType { get; }

        /// <summary>
        /// Loads a database schema from the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="options">The schema provider options.</param>
        /// <returns>The loaded database schema.</returns>
        IDatabaseSchema LoadSchema(string connectionString, SchemaProviderOptions options = null);

        /// <summary>
        /// Asynchronously loads a database schema from the specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to use.</param>
        /// <param name="options">The schema provider options.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded database schema.</returns>
        Task<IDatabaseSchema> LoadSchemaAsync(string connectionString, SchemaProviderOptions options = null);
    }
}
