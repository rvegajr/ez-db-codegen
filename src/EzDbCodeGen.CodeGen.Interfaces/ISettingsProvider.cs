namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for providing and managing configuration settings.
/// </summary>
public interface ISettingsProvider
{
    /// <summary>
    /// Gets a setting value by its key.
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <returns>The setting value, or default(T) if the setting does not exist.</returns>
    T? GetSetting<T>(string key);
    
    /// <summary>
    /// Gets a setting value by its key with a default value.
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="defaultValue">The default value to return if the setting does not exist.</param>
    /// <returns>The setting value, or the default value if the setting does not exist.</returns>
    T GetSetting<T>(string key, T defaultValue);
    
    /// <summary>
    /// Sets a setting value.
    /// </summary>
    /// <typeparam name="T">The type of the setting value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="value">The value to set.</param>
    void SetSetting<T>(string key, T value);
    
    /// <summary>
    /// Removes a setting.
    /// </summary>
    /// <param name="key">The key of the setting to remove.</param>
    /// <returns>True if the setting was removed; otherwise, false.</returns>
    bool RemoveSetting(string key);
    
    /// <summary>
    /// Determines whether a setting with the specified key exists.
    /// </summary>
    /// <param name="key">The key of the setting to check.</param>
    /// <returns>True if the setting exists; otherwise, false.</returns>
    bool HasSetting(string key);
    
    /// <summary>
    /// Gets all settings.
    /// </summary>
    /// <returns>A dictionary containing all settings.</returns>
    IDictionary<string, object> GetAllSettings();
    
    /// <summary>
    /// Gets settings that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against setting keys.</param>
    /// <returns>A dictionary containing the matching settings.</returns>
    IDictionary<string, object> GetSettingsByPattern(string pattern);
    
    /// <summary>
    /// Loads settings from a configuration file.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    void LoadFromFile(string configurationPath);
    
    /// <summary>
    /// Saves settings to a configuration file.
    /// </summary>
    /// <param name="configurationPath">The path to the configuration file.</param>
    void SaveToFile(string configurationPath);
    
    /// <summary>
    /// Clears all settings.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Imports settings from a dictionary.
    /// </summary>
    /// <param name="settings">The settings to import.</param>
    /// <param name="overwrite">Whether to overwrite existing settings.</param>
    void ImportSettings(IDictionary<string, object> settings, bool overwrite = true);
    
    /// <summary>
    /// Creates a child settings provider with a prefix.
    /// </summary>
    /// <param name="prefix">The prefix for the child settings.</param>
    /// <returns>A new settings provider that accesses settings with the specified prefix.</returns>
    ISettingsProvider CreateChildProvider(string prefix);
}
