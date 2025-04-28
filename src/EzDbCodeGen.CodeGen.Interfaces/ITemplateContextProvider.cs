namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for providing additional context data to templates.
/// </summary>
public interface ITemplateContextProvider
{
    /// <summary>
    /// Gets the name of the context provider.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the context provider.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets context data for the template rendering process.
    /// </summary>
    /// <param name="templateName">The name of the template being rendered.</param>
    /// <param name="model">The model being rendered.</param>
    /// <returns>A dictionary of context data to add to the rendering context.</returns>
    IDictionary<string, object> GetContext(string templateName, object? model);
    
    /// <summary>
    /// Gets the order in which this provider should be applied relative to other providers.
    /// Lower numbers are applied first.
    /// </summary>
    int Order { get; }
    
    /// <summary>
    /// Initializes the context provider with configuration parameters.
    /// </summary>
    /// <param name="configuration">The configuration parameters.</param>
    void Initialize(IDictionary<string, object> configuration);
    
    /// <summary>
    /// Determines whether this provider should provide context for the specified template.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if this provider should provide context for the specified template; otherwise, false.</returns>
    bool ShouldProvideContextFor(string templateName);
    
    /// <summary>
    /// Determines whether this provider should provide context for the specified model.
    /// </summary>
    /// <param name="model">The model to check.</param>
    /// <returns>True if this provider should provide context for the specified model; otherwise, false.</returns>
    bool ShouldProvideContextFor(object? model);
}
