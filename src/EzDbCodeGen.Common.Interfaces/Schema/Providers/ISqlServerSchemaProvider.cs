using System.Threading.Tasks;

namespace EzDbCodeGen.Common.Interfaces.Schema.Providers;

/// <summary>
/// Provides methods for retrieving database schema information from SQL Server.
/// </summary>
public interface ISqlServerSchemaProvider : IDatabaseSchemaProvider
{
    /// <summary>
    /// Gets the schema of a SQL Server database.
    /// </summary>
    /// <param name="connectionString">The connection string to the database.</param>
    /// <param name="databaseName">The name of the database.</param>
    /// <returns>The database schema.</returns>
    Task<IDatabaseSchema> GetSchemaAsync(string connectionString, string databaseName);
}
