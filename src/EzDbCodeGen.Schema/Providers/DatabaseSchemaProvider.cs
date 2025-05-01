using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Providers;

/// <summary>
/// Base abstract class for database schema providers.
/// </summary>
public abstract class DatabaseSchemaProvider : IDatabaseSchemaProvider
{
    /// <summary>
    /// Gets the logger.
    /// </summary>
    protected readonly ILogger Logger;
    
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    protected string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseSchemaProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    protected DatabaseSchemaProvider(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public abstract string ProviderType { get; }

    /// <inheritdoc/>
    public abstract Task<IDatabaseSchema> GetSchemaAsync(string connectionString, SchemaProviderOptions options);

    /// <inheritdoc/>
    public abstract Task<bool> TestConnectionAsync(string connectionString);
    
    /// <summary>
    /// Configures the provider with the specified settings.
    /// </summary>
    /// <param name="settings">The settings to configure the provider with.</param>
    public virtual void Configure(IDictionary<string, string> settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }
        
        if (settings.TryGetValue("ConnectionString", out var connectionString))
        {
            ConnectionString = connectionString;
        }
        
        Logger.LogInformation("Provider {ProviderType} configured with {SettingsCount} settings", ProviderType, settings.Count);
    }

    /// <summary>
    /// Validates the connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the connection string is null or empty.</exception>
    protected void ValidateConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }
    }

    /// <summary>
    /// Validates the schema provider options.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    /// <returns>The validated options, or default options if the input was null.</returns>
    protected SchemaProviderOptions ValidateOptions(SchemaProviderOptions options)
    {
        return options ?? SchemaProviderOptions.Default;
    }

    /// <summary>
    /// Logs detailed information about the schema provider options.
    /// </summary>
    /// <param name="options">The options to log.</param>
    protected void LogOptions(SchemaProviderOptions options)
    {
        if (options == null)
        {
            Logger.LogWarning("Schema provider options are null. Using defaults.");
            return;
        }

        Logger.LogInformation("Schema provider options:");
        Logger.LogInformation($"- Include system objects: {options.IncludeSystemObjects}");
        Logger.LogInformation($"- Include views: {options.IncludeViews}");
        Logger.LogInformation($"- Include stored procedures: {options.IncludeStoredProcedures}");
        Logger.LogInformation($"- Include functions: {options.IncludeFunctions}");
        Logger.LogInformation($"- Include table columns: {options.IncludeTableColumns}");
        Logger.LogInformation($"- Include primary keys: {options.IncludePrimaryKeys}");
        Logger.LogInformation($"- Include foreign keys: {options.IncludeForeignKeys}");
        Logger.LogInformation($"- Include indexes: {options.IncludeIndexes}");
        Logger.LogInformation($"- Include unique constraints: {options.IncludeUniqueConstraints}");
        Logger.LogInformation($"- Command timeout: {options.CommandTimeout} seconds");

        if (options.IncludeSchemas != null && options.IncludeSchemas.Count > 0)
        {
            Logger.LogInformation($"- Include schemas: {string.Join(", ", options.IncludeSchemas)}");
        }

        if (options.ExcludeSchemas != null && options.ExcludeSchemas.Count > 0)
        {
            Logger.LogInformation($"- Exclude schemas: {string.Join(", ", options.ExcludeSchemas)}");
        }

        if (options.IncludeTables != null && options.IncludeTables.Count > 0)
        {
            Logger.LogInformation($"- Include tables: {string.Join(", ", options.IncludeTables)}");
        }

        if (options.ExcludeTables != null && options.ExcludeTables.Count > 0)
        {
            Logger.LogInformation($"- Exclude tables: {string.Join(", ", options.ExcludeTables)}");
        }
    }
}
