namespace EzDbCodeGen.Core.Configuration;

/// <summary>
/// Represents a section of configuration values.
/// </summary>
public interface IConfigurationSection
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
    /// Gets a child configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>A configuration section.</returns>
    IConfigurationSection GetSection(string sectionName);
    
    /// <summary>
    /// Gets all configuration keys in this section.
    /// </summary>
    /// <returns>A list of configuration keys.</returns>
    IReadOnlyList<string> GetAllKeys();
    
    /// <summary>
    /// Gets all child sections.
    /// </summary>
    /// <returns>A list of configuration sections.</returns>
    IReadOnlyList<IConfigurationSection> GetAllSections();
    
    /// <summary>
    /// Checks if a configuration key exists.
    /// </summary>
    /// <param name="key">The configuration key.</param>
    /// <returns>True if the key exists, false otherwise.</returns>
    bool ContainsKey(string key);
    
    /// <summary>
    /// Gets the name of the section.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the full path of the section.
    /// </summary>
    string Path { get; }
    
    /// <summary>
    /// Gets the parent section, or null if this is a root section.
    /// </summary>
    IConfigurationSection? Parent { get; }
    
    /// <summary>
    /// Gets or creates a child configuration section.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <returns>A configuration section.</returns>
    IConfigurationSection GetOrCreateSection(string sectionName);
    
    /// <summary>
    /// Binds the configuration section to an object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>The bound object.</returns>
    T Bind<T>() where T : new();
    
    /// <summary>
    /// Binds the configuration section to an existing object.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="instance">The object to bind to.</param>
    void Bind<T>(T instance) where T : class;
}
