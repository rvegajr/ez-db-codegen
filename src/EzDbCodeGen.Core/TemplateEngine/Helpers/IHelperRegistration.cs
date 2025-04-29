namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

/// <summary>
/// Represents a registration system for template helpers.
/// </summary>
public interface IHelperRegistration
{
    /// <summary>
    /// Registers all helpers with a template engine.
    /// </summary>
    /// <param name="engine">The template engine to register helpers with.</param>
    void RegisterAllHelpers(ITemplateEngine engine);
    
    /// <summary>
    /// Registers helpers in a specific category with a template engine.
    /// </summary>
    /// <param name="engine">The template engine to register helpers with.</param>
    /// <param name="category">The category of helpers to register.</param>
    void RegisterCategoryHelpers(ITemplateEngine engine, string category);
    
    /// <summary>
    /// Gets all registered helpers.
    /// </summary>
    /// <returns>A dictionary of helper names and functions.</returns>
    IReadOnlyDictionary<string, Delegate> GetAllHelpers();
    
    /// <summary>
    /// Gets all registered helpers in a specific category.
    /// </summary>
    /// <param name="category">The category of helpers to get.</param>
    /// <returns>A dictionary of helper names and functions.</returns>
    IReadOnlyDictionary<string, Delegate> GetCategoryHelpers(string category);
    
    /// <summary>
    /// Registers a helper with the registration system.
    /// </summary>
    /// <param name="name">The name of the helper.</param>
    /// <param name="helper">The helper function.</param>
    /// <param name="category">The category of the helper.</param>
    void RegisterHelper(string name, Delegate helper, string category);
    
    /// <summary>
    /// Registers a block helper with the registration system.
    /// </summary>
    /// <param name="name">The name of the helper.</param>
    /// <param name="helper">The helper function.</param>
    /// <param name="category">The category of the helper.</param>
    void RegisterBlockHelper(string name, Delegate helper, string category);
}
