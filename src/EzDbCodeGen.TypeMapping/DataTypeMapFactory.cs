using EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

namespace EzDbCodeGen.TypeMapping;

/// <summary>
/// Factory for creating data type mappers based on database provider.
/// </summary>
public class DataTypeMapFactory : IDataTypeMapFactory
{
    private readonly Dictionary<string, Type> _registeredMappers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IDataTypeMap> _cachedMappers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="DataTypeMapFactory"/> class.
    /// </summary>
    public DataTypeMapFactory()
    {
        // Register default mappers
        RegisterDefaultMappers();
    }

    /// <summary>
    /// Creates a data type mapper for the specified database provider.
    /// </summary>
    /// <param name="databaseProvider">The database provider (e.g., "SqlServer", "PostgreSQL", "MySQL").</param>
    /// <returns>A data type mapper for the specified database provider.</returns>
    public IDataTypeMap CreateTypeMap(string databaseProvider)
    {
        if (string.IsNullOrEmpty(databaseProvider))
        {
            throw new ArgumentNullException(nameof(databaseProvider));
        }

        // Check if we have a cached instance
        if (_cachedMappers.TryGetValue(databaseProvider, out var cachedMapper))
        {
            return cachedMapper;
        }

        // Check if we have a registered type for this provider
        if (!_registeredMappers.TryGetValue(databaseProvider, out var mapperType))
        {
            throw new ArgumentException($"No type mapper registered for provider: {databaseProvider}", nameof(databaseProvider));
        }

        // Create a new instance
        var mapper = (IDataTypeMap)Activator.CreateInstance(mapperType)!;
        
        // Cache the instance
        _cachedMappers[databaseProvider] = mapper;
        
        return mapper;
    }

    /// <summary>
    /// Registers a custom data type mapper for a database provider.
    /// </summary>
    /// <param name="databaseProvider">The database provider.</param>
    /// <param name="dataTypeMap">The data type mapper to register.</param>
    public void RegisterTypeMap(string databaseProvider, IDataTypeMap dataTypeMap)
    {
        if (string.IsNullOrEmpty(databaseProvider))
        {
            throw new ArgumentNullException(nameof(databaseProvider));
        }

        if (dataTypeMap == null)
        {
            throw new ArgumentNullException(nameof(dataTypeMap));
        }

        _registeredMappers[databaseProvider] = dataTypeMap.GetType();
        _cachedMappers[databaseProvider] = dataTypeMap;
    }

    /// <summary>
    /// Gets all registered database providers.
    /// </summary>
    /// <returns>A collection of registered database providers.</returns>
    public IReadOnlyCollection<string> GetRegisteredProviders()
    {
        return _registeredMappers.Keys.ToList();
    }

    /// <summary>
    /// Creates a data type mapper from a configuration file.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    /// <returns>A data type mapper configured from the file.</returns>
    public IDataTypeMap CreateFromConfiguration(string configurationPath)
    {
        if (string.IsNullOrEmpty(configurationPath))
        {
            throw new ArgumentNullException(nameof(configurationPath));
        }

        if (!File.Exists(configurationPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configurationPath}", configurationPath);
        }

        // For now, we'll default to SQL Server and load the configuration
        var mapper = CreateTypeMap("SqlServer");
        mapper.LoadMappingsFromConfiguration(configurationPath);
        
        return mapper;
    }

    private void RegisterDefaultMappers()
    {
        // Register default mappers
        _registeredMappers["SqlServer"] = typeof(SqlServerDataTypeMap);
        
        // More providers can be added here
        // _registeredMappers["PostgreSQL"] = typeof(PostgreSQLDataTypeMap);
        // _registeredMappers["MySQL"] = typeof(MySQLDataTypeMap);
        // _registeredMappers["Oracle"] = typeof(OracleDataTypeMap);
    }
}
