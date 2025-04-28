namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a service that compiles templates.
/// </summary>
public interface ITemplateCompilationService
{
    /// <summary>
    /// Compiles a template from a string.
    /// </summary>
    /// <param name="templateContent">The template content to compile.</param>
    /// <returns>A compiled template.</returns>
    ICompiledTemplate CompileTemplate(string templateContent);
    
    /// <summary>
    /// Compiles a template from a file.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <returns>A compiled template.</returns>
    ICompiledTemplate CompileTemplateFromFile(string templatePath);
    
    /// <summary>
    /// Compiles a template with a specific name.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>A compiled template, or null if the template does not exist.</returns>
    ICompiledTemplate? CompileNamedTemplate(string templateName);
    
    /// <summary>
    /// Gets a cached compiled template if it exists.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>A cached compiled template, or null if not in the cache.</returns>
    ICompiledTemplate? GetCachedTemplate(string templateName);
    
    /// <summary>
    /// Adds a compiled template to the cache.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="compiledTemplate">The compiled template to cache.</param>
    void CacheTemplate(string templateName, ICompiledTemplate compiledTemplate);
    
    /// <summary>
    /// Removes a template from the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to remove.</param>
    /// <returns>True if the template was removed; otherwise, false.</returns>
    bool RemoveFromCache(string templateName);
    
    /// <summary>
    /// Clears the template cache.
    /// </summary>
    void ClearCache();
    
    /// <summary>
    /// Gets the number of templates in the cache.
    /// </summary>
    /// <returns>The number of templates in the cache.</returns>
    int GetCacheCount();
    
    /// <summary>
    /// Determines whether a template exists in the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template exists in the cache; otherwise, false.</returns>
    bool IsTemplateCached(string templateName);
}
