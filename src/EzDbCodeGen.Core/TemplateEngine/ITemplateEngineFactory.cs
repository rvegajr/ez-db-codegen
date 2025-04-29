namespace EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents a factory for creating template engines.
/// </summary>
public interface ITemplateEngineFactory
{
    /// <summary>
    /// Creates a template engine of the specified type.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <returns>A template engine.</returns>
    ITemplateEngine CreateTemplateEngine(string engineType);
    
    /// <summary>
    /// Creates a template engine with the specified options.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <param name="options">The template engine options.</param>
    /// <returns>A template engine.</returns>
    ITemplateEngine CreateTemplateEngine(string engineType, TemplateEngineOptions options);
    
    /// <summary>
    /// Gets all available template engine types.
    /// </summary>
    /// <returns>A list of available template engine types.</returns>
    IReadOnlyList<string> GetAvailableEngineTypes();
    
    /// <summary>
    /// Registers a template engine factory method.
    /// </summary>
    /// <param name="engineType">The type of template engine.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterTemplateEngineFactory(string engineType, Func<TemplateEngineOptions, ITemplateEngine> factoryMethod);
}
