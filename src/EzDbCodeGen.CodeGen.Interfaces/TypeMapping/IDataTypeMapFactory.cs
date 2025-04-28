namespace EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

/// <summary>
/// Defines a factory interface for creating data type mappers.
/// </summary>
public interface IDataTypeMapFactory
{
    /// <summary>
    /// Creates a data type mapper for the specified database provider.
    /// </summary>
    /// <param name="databaseProvider">The database provider (e.g., "SqlServer", "PostgreSQL", "MySQL").</param>
    /// <returns>A data type mapper for the specified database provider.</returns>
    IDataTypeMap CreateTypeMap(string databaseProvider);
    
    /// <summary>
    /// Registers a custom data type mapper for a database provider.
    /// </summary>
    /// <param name="databaseProvider">The database provider.</param>
    /// <param name="dataTypeMap">The data type mapper to register.</param>
    void RegisterTypeMap(string databaseProvider, IDataTypeMap dataTypeMap);
    
    /// <summary>
    /// Gets all registered database providers.
    /// </summary>
    /// <returns>A collection of registered database providers.</returns>
    IReadOnlyCollection<string> GetRegisteredProviders();
    
    /// <summary>
    /// Creates a data type mapper from a configuration file.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    /// <returns>A data type mapper configured from the file.</returns>
    IDataTypeMap CreateFromConfiguration(string configurationPath);
}
