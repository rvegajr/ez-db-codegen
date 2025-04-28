namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for providing and managing code generation configurations.
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Loads a code generation configuration from a file.
    /// </summary>
    /// <param name="filePath">The path to the configuration file.</param>
    /// <returns>The loaded code generation configuration.</returns>
    ICodeGenerationConfiguration LoadConfiguration(string filePath);
    
    /// <summary>
    /// Saves a code generation configuration to a file.
    /// </summary>
    /// <param name="configuration">The configuration to save.</param>
    /// <param name="filePath">The path to save the configuration to.</param>
    void SaveConfiguration(ICodeGenerationConfiguration configuration, string filePath);
    
    /// <summary>
    /// Creates a new code generation configuration with default settings.
    /// </summary>
    /// <returns>A new code generation configuration.</returns>
    ICodeGenerationConfiguration CreateDefaultConfiguration();
    
    /// <summary>
    /// Gets the available configuration templates.
    /// </summary>
    /// <returns>A collection of available configuration template names.</returns>
    IReadOnlyCollection<string> GetAvailableTemplates();
    
    /// <summary>
    /// Creates a new code generation configuration from a template.
    /// </summary>
    /// <param name="templateName">The name of the template to use.</param>
    /// <returns>A new code generation configuration based on the template.</returns>
    ICodeGenerationConfiguration CreateFromTemplate(string templateName);
    
    /// <summary>
    /// Validates a code generation configuration.
    /// </summary>
    /// <param name="configuration">The configuration to validate.</param>
    /// <returns>A collection of validation errors, or an empty collection if the configuration is valid.</returns>
    IReadOnlyCollection<string> ValidateConfiguration(ICodeGenerationConfiguration configuration);
    
    /// <summary>
    /// Merges two configurations, with settings from the second configuration taking precedence.
    /// </summary>
    /// <param name="baseConfiguration">The base configuration.</param>
    /// <param name="overrideConfiguration">The configuration to override settings with.</param>
    /// <returns>A new merged configuration.</returns>
    ICodeGenerationConfiguration MergeConfigurations(ICodeGenerationConfiguration baseConfiguration, ICodeGenerationConfiguration overrideConfiguration);
    
    /// <summary>
    /// Gets the last used configuration.
    /// </summary>
    /// <returns>The last used configuration, or a default configuration if no configuration has been used.</returns>
    ICodeGenerationConfiguration GetLastUsedConfiguration();
    
    /// <summary>
    /// Sets the last used configuration.
    /// </summary>
    /// <param name="configuration">The configuration to set as the last used.</param>
    void SetLastUsedConfiguration(ICodeGenerationConfiguration configuration);
}
