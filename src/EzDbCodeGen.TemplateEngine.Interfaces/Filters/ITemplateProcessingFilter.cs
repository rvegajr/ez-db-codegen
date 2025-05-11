namespace EzDbCodeGen.TemplateEngine.Interfaces.Filters;

/// <summary>
/// Interface for filtering template processing.
/// </summary>
public interface ITemplateProcessingFilter : ITemplateFilter
{
    /// <summary>
    /// Determines if a template should be processed.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="model">The data model.</param>
    /// <returns>True if the template should be processed; otherwise, false.</returns>
    bool ShouldProcessTemplate(string templateName, object? model);
    
    /// <summary>
    /// Determines if a model should be processed.
    /// </summary>
    /// <param name="model">The data model.</param>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>True if the model should be processed; otherwise, false.</returns>
    bool ShouldProcessModel(object? model, string templateName);
    
    /// <summary>
    /// Determines if a property should be included.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="model">The data model.</param>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>True if the property should be included; otherwise, false.</returns>
    bool ShouldIncludeProperty(string propertyName, object? model, string templateName);
}
