namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines a factory for creating template engines.
/// </summary>
public interface ITemplateEngineFactory
{
    /// <summary>
    /// Creates a template engine of the specified type.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <returns>A template engine instance.</returns>
    ITemplateEngine CreateEngine(TemplateEngineType engineType);
    
    /// <summary>
    /// Creates a template engine of the specified type with configuration options.
    /// </summary>
    /// <param name="engineType">The type of template engine to create.</param>
    /// <param name="options">Configuration options for the template engine.</param>
    /// <returns>A template engine instance.</returns>
    ITemplateEngine CreateEngine(TemplateEngineType engineType, TemplateEngineOptions options);
    
    /// <summary>
    /// Registers a template engine factory method.
    /// </summary>
    /// <param name="engineType">The type of template engine.</param>
    /// <param name="factoryMethod">The factory method that creates the template engine.</param>
    void RegisterEngineFactory(TemplateEngineType engineType, Func<TemplateEngineOptions, ITemplateEngine> factoryMethod);
    
    /// <summary>
    /// Gets all available template engine types.
    /// </summary>
    /// <returns>A list of available template engine types.</returns>
    IReadOnlyList<TemplateEngineType> GetAvailableEngines();
    
    /// <summary>
    /// Checks if a template engine type is registered.
    /// </summary>
    /// <param name="engineType">The type of template engine.</param>
    /// <returns>True if the template engine type is registered; otherwise, false.</returns>
    bool IsEngineRegistered(TemplateEngineType engineType);
}
