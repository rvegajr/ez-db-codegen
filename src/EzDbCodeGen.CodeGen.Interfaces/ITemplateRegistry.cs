namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for a registry that manages template metadata and relationships.
/// </summary>
public interface ITemplateRegistry
{
    /// <summary>
    /// Registers a template with its metadata.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="metadata">The metadata for the template.</param>
    void RegisterTemplate(string templateName, TemplateMetadata metadata);
    
    /// <summary>
    /// Gets template metadata by name.
    /// </summary>
    /// <param name="templateName">The name of the template to get metadata for.</param>
    /// <returns>The template metadata, or null if no template with the specified name exists.</returns>
    TemplateMetadata? GetTemplateMetadata(string templateName);
    
    /// <summary>
    /// Determines whether a template with the specified name is registered.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if a template with the specified name is registered; otherwise, false.</returns>
    bool IsTemplateRegistered(string templateName);
    
    /// <summary>
    /// Gets all registered template names.
    /// </summary>
    /// <returns>A collection of all registered template names.</returns>
    IReadOnlyCollection<string> GetAllTemplateNames();
    
    /// <summary>
    /// Gets all registered templates with their metadata.
    /// </summary>
    /// <returns>A dictionary mapping template names to their metadata.</returns>
    IReadOnlyDictionary<string, TemplateMetadata> GetAllTemplates();
    
    /// <summary>
    /// Removes a template from the registry.
    /// </summary>
    /// <param name="templateName">The name of the template to remove.</param>
    /// <returns>True if the template was removed; otherwise, false.</returns>
    bool RemoveTemplate(string templateName);
    
    /// <summary>
    /// Gets templates that match a specified category.
    /// </summary>
    /// <param name="category">The category to match.</param>
    /// <returns>A collection of template names in the specified category.</returns>
    IReadOnlyCollection<string> GetTemplatesByCategory(string category);
    
    /// <summary>
    /// Gets templates that match a specified tag.
    /// </summary>
    /// <param name="tag">The tag to match.</param>
    /// <returns>A collection of template names with the specified tag.</returns>
    IReadOnlyCollection<string> GetTemplatesByTag(string tag);
    
    /// <summary>
    /// Gets templates that are suitable for a specific model type.
    /// </summary>
    /// <param name="modelType">The model type to match.</param>
    /// <returns>A collection of template names suitable for the specified model type.</returns>
    IReadOnlyCollection<string> GetTemplatesForModelType(string modelType);
    
    /// <summary>
    /// Gets templates that generate a specific target language.
    /// </summary>
    /// <param name="targetLanguage">The target language to match.</param>
    /// <returns>A collection of template names that generate the specified target language.</returns>
    IReadOnlyCollection<string> GetTemplatesByTargetLanguage(string targetLanguage);
    
    /// <summary>
    /// Gets templates that are partials.
    /// </summary>
    /// <returns>A collection of template names that are partials.</returns>
    IReadOnlyCollection<string> GetPartialTemplates();
    
    /// <summary>
    /// Gets templates that are layouts.
    /// </summary>
    /// <returns>A collection of template names that are layouts.</returns>
    IReadOnlyCollection<string> GetLayoutTemplates();
    
    /// <summary>
    /// Updates the metadata for a template.
    /// </summary>
    /// <param name="templateName">The name of the template to update.</param>
    /// <param name="metadata">The new metadata for the template.</param>
    /// <returns>True if the template metadata was updated; otherwise, false.</returns>
    bool UpdateTemplateMetadata(string templateName, TemplateMetadata metadata);
    
    /// <summary>
    /// Gets all template categories.
    /// </summary>
    /// <returns>A collection of all template categories.</returns>
    IReadOnlyCollection<string> GetAllCategories();
    
    /// <summary>
    /// Gets all template tags.
    /// </summary>
    /// <returns>A collection of all template tags.</returns>
    IReadOnlyCollection<string> GetAllTags();
}
