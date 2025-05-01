using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EzDbCodeGen.Schema.Interfaces;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Providers
{
    /// <summary>
    /// Factory for creating database schema providers.
    /// </summary>
    public class DatabaseSchemaProviderFactory : IDatabaseSchemaProviderFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<string, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider>> _providerFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSchemaProviderFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public DatabaseSchemaProviderFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _providerFactories = new Dictionary<string, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider>>(StringComparer.OrdinalIgnoreCase);
            
            // Register the default providers
            RegisterDefaultProviders();
        }

        /// <inheritdoc/>
        public IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString)
        {
            return CreateProvider(providerName, connectionString, new SchemaProviderOptions());
        }

        /// <inheritdoc/>
        public IDatabaseSchemaProvider CreateProvider(string providerName, string connectionString, SchemaProviderOptions options)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Provider name cannot be null or empty.", nameof(providerName));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!_providerFactories.TryGetValue(providerName, out var factory))
            {
                throw new ArgumentException($"Unsupported provider: {providerName}. Available providers: {string.Join(", ", _providerFactories.Keys)}", nameof(providerName));
            }

            return factory(connectionString, options);
        }

        /// <inheritdoc/>
        public void RegisterProviderFactory(string providerName, Func<string, SchemaProviderOptions, IDatabaseSchemaProvider> factoryMethod)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Provider name cannot be null or empty.", nameof(providerName));
            }

            if (factoryMethod == null)
            {
                throw new ArgumentNullException(nameof(factoryMethod));
            }

            _providerFactories[providerName] = factoryMethod;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetAvailableProviders()
        {
            return new List<string>(_providerFactories.Keys).AsReadOnly();
        }

        /// <inheritdoc/>
        public bool IsProviderRegistered(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return false;
            }

            return _providerFactories.ContainsKey(providerName);
        }

        private void RegisterDefaultProviders()
        {
            // Register SQL Server provider
            RegisterProviderFactory("SqlServer", (connectionString, options) =>
            {
                var logger = _loggerFactory.CreateLogger<SqlServerSchemaProvider>();
                var provider = new SqlServerSchemaProvider(logger);
                
                // Configure the provider with the connection string
                var settings = new Dictionary<string, string>
                {
                    ["ConnectionString"] = connectionString
                };
                
                provider.Configure(settings);
                return provider;
            });
            
            // Additional providers can be registered here as they are implemented
            // RegisterProviderFactory("PostgreSQL", (connectionString, options) => new PostgreSQLSchemaProvider(_loggerFactory.CreateLogger<PostgreSQLSchemaProvider>(), connectionString));
            // RegisterProviderFactory("MySQL", (connectionString, options) => new MySQLSchemaProvider(_loggerFactory.CreateLogger<MySQLSchemaProvider>(), connectionString));
            // RegisterProviderFactory("Oracle", (connectionString, options) => new OracleSchemaProvider(_loggerFactory.CreateLogger<OracleSchemaProvider>(), connectionString));
        }
    }
}
