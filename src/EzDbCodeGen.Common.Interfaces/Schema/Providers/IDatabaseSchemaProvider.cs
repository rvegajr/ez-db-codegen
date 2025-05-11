using System.Threading.Tasks;

namespace EzDbCodeGen.Common.Interfaces.Schema.Providers;

/// <summary>
/// Defines the interface for a database schema provider.
/// </summary>
public interface IDatabaseSchemaProvider
{
    /// <summary>
    /// Gets the schema of a database.
    /// </summary>
    /// <param name="connectionString">The connection string to the database.</param>
    /// <returns>The database schema.</returns>
    Task<IDatabaseSchema> GetSchemaAsync(string connectionString);
}
