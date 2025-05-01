using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Interfaces.Providers;
using EzDbCodeGen.Schema.Providers;

namespace EzDbCodeGen.Schema
{
    /// <summary>
    /// Factory for creating database schema providers.
    /// </summary>
    public class DatabaseSchemaProviderFactory : IDatabaseSchemaProviderFactory
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, Func<IDatabaseSchemaProvider>> _providerFactories;
        private readonly Dictionary<string, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider>> _configProviderFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaProviderFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public DatabaseSchemaProviderFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _providerFactories = new Dictionary<string, Func<IDatabaseSchemaProvider>>(StringComparer.OrdinalIgnoreCase);
            _configProviderFactories = new Dictionary<string, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider>>(StringComparer.OrdinalIgnoreCase);
            
            // Register standard providers
            RegisterStandardProviders();
        }

        /// <inheritdoc/>
        public IDatabaseSchemaProvider CreateSchemaProvider(string providerName, IDictionary<string, string>? connectionInfo = null)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be empty.", nameof(providerName));
            }

            if (!_providerFactories.TryGetValue(providerName, out var factory))
            {
                throw new KeyNotFoundException($"Database provider '{providerName}' not found. Available providers: {string.Join(", ", _providerFactories.Keys)}");
            }

            var provider = factory();
            
            // If connection info is provided, configure the provider
            if (connectionInfo != null && provider is IConfigurable configurable)
            {
                configurable.Configure(connectionInfo);
            }

            return provider;
        }

        /// <inheritdoc/>
        public IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString)
        {
            return CreateProvider(providerName, connectionString, new SchemaProviderOptions());
        }

        /// <inheritdoc/>
        public IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString, SchemaProviderOptions options)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be empty.", nameof(providerName));
            }

            if (_configProviderFactories.TryGetValue(providerName, out var factory))
            {
                return factory(connectionString, options);
            }
            
            // Fall back to basic provider if available
            if (_providerFactories.TryGetValue(providerName, out var basicFactory))
            {
                var provider = basicFactory();
                
                // If connection string is provided, configure the provider if possible
                if (!string.IsNullOrEmpty(connectionString) && provider is IDatabaseSchemaProvider schemaProvider)
                {
                    // Configure the provider with the connection string
                    var settings = new Dictionary<string, string>
                    {
                        { "ConnectionString", connectionString }
                    };
                    
                    if (provider is IConfigurable configurable)
                    {
                        configurable.Configure(settings);
                    }
                }
                
                return provider;
            }

            throw new KeyNotFoundException($"Database provider '{providerName}' not found. Available providers: {string.Join(", ", _providerFactories.Keys.Concat(_configProviderFactories.Keys).Distinct())}");
        }

        /// <inheritdoc/>
        public void RegisterProvider(string providerName, Func<IDatabaseSchemaProvider> factory)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be empty.", nameof(providerName));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _providerFactories[providerName] = factory;
            _logger.LogDebug("Registered database schema provider: {ProviderName}", providerName);
        }

        /// <inheritdoc/>
        public void RegisterProviderFactory(string providerName, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider> factory)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be empty.", nameof(providerName));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _configProviderFactories[providerName] = factory;
            _logger.LogDebug("Registered configurable database schema provider factory: {ProviderName}", providerName);
        }

        /// <inheritdoc/>
        public bool IsProviderRegistered(string providerName)
        {
            if (string.IsNullOrWhiteSpace(providerName))
            {
                return false;
            }

            return _providerFactories.ContainsKey(providerName) || _configProviderFactories.ContainsKey(providerName);
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetRegisteredProviders()
        {
            return _providerFactories.Keys.Concat(_configProviderFactories.Keys).Distinct().ToList().AsReadOnly();
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetAvailableProviders()
        {
            return GetRegisteredProviders();
        }

        private void RegisterStandardProviders()
        {
            // Register SQL Server provider
            RegisterProvider("SqlServer", () => new SqlServerSchemaProvider(_logger));
            
            // Register SQL Server provider with config
            RegisterProviderFactory("SqlServer", (connectionString, options) => 
            {
                var provider = new SqlServerSchemaProvider(_logger);
                var settings = new Dictionary<string, string> { { "ConnectionString", connectionString } };
                
                if (provider is IConfigurable configurable)
                {
                    configurable.Configure(settings);
                }
                
                return provider;
            });
            
            // Additional providers can be registered here
            // RegisterProvider("PostgreSQL", () => new PostgreSqlSchemaProvider(_logger));
            // RegisterProvider("MySQL", () => new MySqlSchemaProvider(_logger));
            // RegisterProvider("Oracle", () => new OracleSchemaProvider(_logger));
            // RegisterProvider("SQLite", () => new SQLiteSchemaProvider(_logger));
        }
    }

    /// <summary>
    /// Interface for configurable components.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Configures the component with the provided settings.
        /// </summary>
        /// <param name="settings">The settings to use for configuration.</param>
        void Configure(IDictionary<string, string> settings);
    }
}
