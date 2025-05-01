namespace EzDbCodeGen.Schema.Interfaces.Providers;

/// <summary>
/// Interface for SQL Server-specific schema provider functionality.
/// </summary>
public interface ISqlServerSchemaProvider : IDatabaseSchemaProvider
{
    /// <summary>
    /// Gets the SQL Server version information.
    /// </summary>
    /// <returns>The SQL Server version string.</returns>
    Task<string> GetServerVersionAsync();

    /// <summary>
    /// Gets a list of available databases on the SQL Server instance.
    /// </summary>
    /// <returns>A list of database names.</returns>
    Task<IReadOnlyList<string>> GetDatabasesAsync();
}
