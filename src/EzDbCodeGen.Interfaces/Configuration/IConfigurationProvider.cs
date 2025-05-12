using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Configuration
{
    /// <summary>
    /// Defines a provider for application configuration.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets a configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="defaultValue">The default value to return if the key is not found.</param>
        /// <returns>The configuration value.</returns>
        T GetValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// Sets a configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the configuration value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The configuration value.</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// Gets all configuration keys.
        /// </summary>
        /// <returns>A collection of all configuration keys.</returns>
        IEnumerable<string> GetAllKeys();

        /// <summary>
        /// Gets a section of the configuration as a dictionary.
        /// </summary>
        /// <param name="sectionKey">The section key.</param>
        /// <returns>A dictionary containing the configuration section.</returns>
        IDictionary<string, object> GetSection(string sectionKey);

        /// <summary>
        /// Gets a typed configuration section.
        /// </summary>
        /// <typeparam name="T">The type of the configuration section.</typeparam>
        /// <param name="sectionKey">The section key.</param>
        /// <returns>The typed configuration section.</returns>
        T GetSection<T>(string sectionKey) where T : class, new();

        /// <summary>
        /// Sets a configuration section.
        /// </summary>
        /// <typeparam name="T">The type of the configuration section.</typeparam>
        /// <param name="sectionKey">The section key.</param>
        /// <param name="section">The configuration section.</param>
        void SetSection<T>(string sectionKey, T section) where T : class;

        /// <summary>
        /// Determines if a configuration key exists.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Loads configuration from a file.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        void LoadFromFile(string path);

        /// <summary>
        /// Asynchronously loads configuration from a file.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task LoadFromFileAsync(string path);

        /// <summary>
        /// Saves configuration to a file.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        void SaveToFile(string path);

        /// <summary>
        /// Asynchronously saves configuration to a file.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveToFileAsync(string path);

        /// <summary>
        /// Clears all configuration values.
        /// </summary>
        void Clear();
    }
}
