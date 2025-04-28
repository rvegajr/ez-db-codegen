namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for filtering templates during the code generation process.
/// </summary>
public interface ITemplateFilter
{
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the filter.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Determines whether a template should be processed based on its name and the model.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <param name="model">The model that would be used for rendering.</param>
    /// <returns>True if the template should be processed; otherwise, false.</returns>
    bool ShouldProcessTemplate(string templateName, object? model);
    
    /// <summary>
    /// Determines whether a model object should be processed with a specific template.
    /// </summary>
    /// <param name="model">The model to check.</param>
    /// <param name="templateName">The name of the template that would be used.</param>
    /// <returns>True if the model should be processed; otherwise, false.</returns>
    bool ShouldProcessModel(object? model, string templateName);
    
    /// <summary>
    /// Determines whether a property of a model should be included in the rendering process.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <param name="model">The model that contains the property.</param>
    /// <param name="templateName">The name of the template that would be used.</param>
    /// <returns>True if the property should be included; otherwise, false.</returns>
    bool ShouldIncludeProperty(string propertyName, object? model, string templateName);
    
    /// <summary>
    /// Gets the order in which this filter should be applied relative to other filters.
    /// Lower numbers are applied first.
    /// </summary>
    int Order { get; }
}
