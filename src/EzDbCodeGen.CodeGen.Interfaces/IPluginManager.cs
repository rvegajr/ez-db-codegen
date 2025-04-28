namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for managing plugins in the code generation system.
/// </summary>
public interface IPluginManager
{
    /// <summary>
    /// Loads a plugin from a file.
    /// </summary>
    /// <param name="pluginPath">The path to the plugin file.</param>
    /// <returns>The loaded plugin, or null if the plugin could not be loaded.</returns>
    ITemplatePlugin? LoadPlugin(string pluginPath);
    
    /// <summary>
    /// Loads all plugins from a directory.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing plugins.</param>
    /// <param name="recursive">Whether to search subdirectories.</param>
    /// <returns>A collection of loaded plugins.</returns>
    IReadOnlyCollection<ITemplatePlugin> LoadPluginsFromDirectory(string directoryPath, bool recursive = false);
    
    /// <summary>
    /// Registers a plugin with the manager.
    /// </summary>
    /// <param name="plugin">The plugin to register.</param>
    void RegisterPlugin(ITemplatePlugin plugin);
    
    /// <summary>
    /// Unregisters a plugin from the manager.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to unregister.</param>
    /// <returns>True if the plugin was unregistered; otherwise, false.</returns>
    bool UnregisterPlugin(string pluginName);
    
    /// <summary>
    /// Gets a plugin by its name.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to retrieve.</param>
    /// <returns>The plugin, or null if no plugin with the specified name exists.</returns>
    ITemplatePlugin? GetPlugin(string pluginName);
    
    /// <summary>
    /// Gets all registered plugins.
    /// </summary>
    /// <returns>A collection of all registered plugins.</returns>
    IReadOnlyCollection<ITemplatePlugin> GetAllPlugins();
    
    /// <summary>
    /// Initializes all registered plugins with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to initialize the plugins with.</param>
    void InitializePlugins(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Disables a plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to disable.</param>
    /// <returns>True if the plugin was disabled; otherwise, false.</returns>
    bool DisablePlugin(string pluginName);
    
    /// <summary>
    /// Enables a plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to enable.</param>
    /// <returns>True if the plugin was enabled; otherwise, false.</returns>
    bool EnablePlugin(string pluginName);
    
    /// <summary>
    /// Gets plugin metadata.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to get metadata for.</param>
    /// <returns>A dictionary containing the plugin metadata, or null if the plugin does not exist.</returns>
    IDictionary<string, object>? GetPluginMetadata(string pluginName);
    
    /// <summary>
    /// Checks if a plugin is compatible with the current system.
    /// </summary>
    /// <param name="plugin">The plugin to check.</param>
    /// <returns>True if the plugin is compatible; otherwise, false.</returns>
    bool IsPluginCompatible(ITemplatePlugin plugin);
}
