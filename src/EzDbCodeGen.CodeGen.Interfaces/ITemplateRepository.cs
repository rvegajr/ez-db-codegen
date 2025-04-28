namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a repository that manages template storage and retrieval.
/// </summary>
public interface ITemplateRepository
{
    /// <summary>
    /// Gets a template by its name.
    /// </summary>
    /// <param name="templateName">The name of the template to retrieve.</param>
    /// <returns>The template content, or null if the template does not exist.</returns>
    string? GetTemplate(string templateName);
    
    /// <summary>
    /// Determines whether a template with the specified name exists in the repository.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template exists; otherwise, false.</returns>
    bool TemplateExists(string templateName);
    
    /// <summary>
    /// Adds or updates a template in the repository.
    /// </summary>
    /// <param name="templateName">The name of the template to add or update.</param>
    /// <param name="templateContent">The content of the template.</param>
    void SaveTemplate(string templateName, string templateContent);
    
    /// <summary>
    /// Removes a template from the repository.
    /// </summary>
    /// <param name="templateName">The name of the template to remove.</param>
    /// <returns>True if the template was removed; otherwise, false.</returns>
    bool RemoveTemplate(string templateName);
    
    /// <summary>
    /// Gets all template names in the repository.
    /// </summary>
    /// <returns>A collection of template names.</returns>
    IReadOnlyCollection<string> GetAllTemplateNames();
    
    /// <summary>
    /// Gets template names that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against template names.</param>
    /// <returns>A collection of matching template names.</returns>
    IReadOnlyCollection<string> FindTemplates(string pattern);
    
    /// <summary>
    /// Gets the last modified time for a template.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>The last modified time, or null if the template does not exist.</returns>
    DateTime? GetLastModifiedTime(string templateName);
    
    /// <summary>
    /// Adds a base path to search for templates.
    /// </summary>
    /// <param name="basePath">The base path to add.</param>
    void AddBasePath(string basePath);
    
    /// <summary>
    /// Gets all base paths configured for this repository.
    /// </summary>
    /// <returns>A collection of base paths.</returns>
    IReadOnlyCollection<string> GetBasePaths();
}
