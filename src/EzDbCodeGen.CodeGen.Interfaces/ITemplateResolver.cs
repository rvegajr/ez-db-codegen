namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for resolving template paths and content.
/// </summary>
public interface ITemplateResolver
{
    /// <summary>
    /// Resolves a template name to its content.
    /// </summary>
    /// <param name="templateName">The name of the template to resolve.</param>
    /// <returns>The template content, or null if the template could not be resolved.</returns>
    string? ResolveTemplate(string templateName);
    
    /// <summary>
    /// Resolves a template name to its file path.
    /// </summary>
    /// <param name="templateName">The name of the template to resolve.</param>
    /// <returns>The file path of the template, or null if the template could not be resolved.</returns>
    string? ResolveTemplatePath(string templateName);
    
    /// <summary>
    /// Determines whether a template with the specified name exists.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template exists; otherwise, false.</returns>
    bool TemplateExists(string templateName);
    
    /// <summary>
    /// Adds a base path to search for templates.
    /// </summary>
    /// <param name="basePath">The base path to add.</param>
    void AddBasePath(string basePath);
    
    /// <summary>
    /// Gets all base paths configured for this resolver.
    /// </summary>
    /// <returns>A collection of base paths.</returns>
    IReadOnlyCollection<string> GetBasePaths();
    
    /// <summary>
    /// Gets all template file extensions that this resolver recognizes.
    /// </summary>
    /// <returns>A collection of file extensions.</returns>
    IReadOnlyCollection<string> GetTemplateExtensions();
    
    /// <summary>
    /// Adds a template file extension that this resolver should recognize.
    /// </summary>
    /// <param name="extension">The file extension to add, including the leading period (e.g., ".hbs").</param>
    void AddTemplateExtension(string extension);
    
    /// <summary>
    /// Resolves a partial template name to its content.
    /// </summary>
    /// <param name="partialName">The name of the partial template to resolve.</param>
    /// <returns>The partial template content, or null if the partial template could not be resolved.</returns>
    string? ResolvePartial(string partialName);
    
    /// <summary>
    /// Resolves a layout template name to its content.
    /// </summary>
    /// <param name="layoutName">The name of the layout template to resolve.</param>
    /// <returns>The layout template content, or null if the layout template could not be resolved.</returns>
    string? ResolveLayout(string layoutName);
    
    /// <summary>
    /// Gets all templates in the configured base paths.
    /// </summary>
    /// <returns>A collection of template names.</returns>
    IReadOnlyCollection<string> GetAllTemplates();
    
    /// <summary>
    /// Gets templates that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against template names.</param>
    /// <returns>A collection of matching template names.</returns>
    IReadOnlyCollection<string> FindTemplates(string pattern);
}
