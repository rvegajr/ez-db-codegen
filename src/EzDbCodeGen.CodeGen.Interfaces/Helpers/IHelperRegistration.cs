namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for registering and managing template helpers.
/// </summary>
public interface IHelperRegistration
{
    /// <summary>
    /// Registers a collection of template helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    /// <param name="helpers">The collection of helpers to register.</param>
    void RegisterHelpers(ITemplateEngine templateEngine, IEnumerable<ITemplateHelper> helpers);
    
    /// <summary>
    /// Registers a specific template helper with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helper with.</param>
    /// <param name="helper">The helper to register.</param>
    void RegisterHelper(ITemplateEngine templateEngine, ITemplateHelper helper);
    
    /// <summary>
    /// Registers all format helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterFormatHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all type conversion helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterTypeConversionHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all documentation helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterDocumentationHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all layout helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterLayoutHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all code format helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterCodeFormatHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all relationship helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterRelationshipHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Registers all available helpers with a template engine.
    /// </summary>
    /// <param name="templateEngine">The template engine to register the helpers with.</param>
    void RegisterAllHelpers(ITemplateEngine templateEngine);
    
    /// <summary>
    /// Gets all registered helpers.
    /// </summary>
    /// <returns>A collection of all registered helpers.</returns>
    IReadOnlyCollection<ITemplateHelper> GetAllHelpers();
}
