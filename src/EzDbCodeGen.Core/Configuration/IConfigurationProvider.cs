namespace EzDbCodeGen.Core.Configuration;

/// <summary>
/// Represents a provider of configuration values.
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Gets a configuration value.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <returns>The configuration value.</returns>
    T GetValue<T>(string key);
    
    /// <summary>
    /// Gets a configuration value, or a default value if the key is not found.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The configuration value, or the default value if the key is not found.</returns>
    T GetValueOrDefault<T>(string key, T defaultValue);
    
    /// <summary>
    /// Sets a configuration value.
    /// </summary>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <param name="key">The configuration key.</param>
    /// <param name="value">The configuration value.</param>
    void SetValue<T>(string key, T value);
    
    /// <summary>
    /// Gets a configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>A configuration section.</returns>
    IConfigurationSection GetSection(string sectionName);
    
    /// <summary>
    /// Gets all configuration keys.
    /// </summary>
    /// <returns>A list of configuration keys.</returns>
    IReadOnlyList<string> GetAllKeys();
    
    /// <summary>
    /// Checks if a configuration key exists.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    bool ContainsKey(string key);
    
    /// <summary>
    /// Removes a configuration key.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    void RemoveKey(string key);
    
    /// <summary>
    /// Loads configuration from a file.
    /// </summary>
    /// <param name="filePath">The path to the configuration file.</param>
    void LoadFromFile(string filePath);
    
    /// <summary>
    /// Saves configuration to a file.
    /// </summary>
    /// <param name="filePath">The path to the configuration file.</param>
    void SaveToFile(string filePath);
    
    /// <summary>
    /// Gets or creates a configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>A configuration section.</returns>
    IConfigurationSection GetOrCreateSection(string sectionName);
}
