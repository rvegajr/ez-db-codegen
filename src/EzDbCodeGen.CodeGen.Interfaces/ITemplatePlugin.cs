namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a plugin that can extend the template engine functionality.
/// </summary>
public interface ITemplatePlugin
{
    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the plugin.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the version of the plugin.
    /// </summary>
    string Version { get; }
    
    /// <summary>
    /// Gets or sets the order in which this plugin should be initialized relative to other plugins.
    /// Lower numbers are initialized first.
    /// </summary>
    int InitializationOrder { get; set; }
    
    /// <summary>
    /// Initializes the plugin with the specified template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to initialize with.</param>
    void Initialize(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all the helpers provided by this plugin with the template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register helpers with.</param>
    void RegisterHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Performs plugin-specific pre-processing before template rendering.
    /// </summary>
    /// <param name="context">The rendering context.</param>
    /// <param name="templateName">The name of the template being rendered.</param>
    /// <param name="model">The model being rendered.</param>
    void BeforeRender(IRenderingContext context, string templateName, object? model);
    
    /// <summary>
    /// Performs plugin-specific post-processing after template rendering.
    /// </summary>
    /// <param name="context">The rendering context.</param>
    /// <param name="templateName">The name of the template that was rendered.</param>
    /// <param name="model">The model that was rendered.</param>
    /// <param name="result">The rendering result.</param>
    void AfterRender(IRenderingContext context, string templateName, object? model, string result);
}
