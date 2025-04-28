namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a cache of compiled templates.
/// </summary>
public interface ITemplateCache
{
    /// <summary>
    /// Gets a compiled template from the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to retrieve.</param>
    /// <returns>The compiled template, or null if the template is not in the cache.</returns>
    ICompiledTemplate? GetTemplate(string templateName);
    
    /// <summary>
    /// Adds a compiled template to the cache.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="compiledTemplate">The compiled template to cache.</param>
    void AddTemplate(string templateName, ICompiledTemplate compiledTemplate);
    
    /// <summary>
    /// Removes a template from the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to remove.</param>
    /// <returns>True if the template was removed; otherwise, false.</returns>
    bool RemoveTemplate(string templateName);
    
    /// <summary>
    /// Determines whether a template is in the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template is in the cache; otherwise, false.</returns>
    bool ContainsTemplate(string templateName);
    
    /// <summary>
    /// Gets all template names in the cache.
    /// </summary>
    /// <returns>A collection of all template names in the cache.</returns>
    IReadOnlyCollection<string> GetAllTemplateNames();
    
    /// <summary>
    /// Gets the number of templates in the cache.
    /// </summary>
    /// <returns>The number of templates in the cache.</returns>
    int Count { get; }
    
    /// <summary>
    /// Clears all templates from the cache.
    /// </summary>
    void Clear();
    
    /// <summary>
    /// Updates a compiled template in the cache.
    /// </summary>
    /// <param name="templateName">The name of the template to update.</param>
    /// <param name="compiledTemplate">The new compiled template.</param>
    /// <returns>True if the template was updated; otherwise, false.</returns>
    bool UpdateTemplate(string templateName, ICompiledTemplate compiledTemplate);
    
    /// <summary>
    /// Gets the last time a template was added or updated in the cache.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>The last modified time, or null if the template is not in the cache.</returns>
    DateTime? GetLastModifiedTime(string templateName);
    
    /// <summary>
    /// Removes templates that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against template names.</param>
    /// <returns>The number of templates removed.</returns>
    int RemoveTemplates(string pattern);
}
