namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for managing template dependencies and inclusions.
/// </summary>
public interface ITemplateDependencyManager
{
    /// <summary>
    /// Registers a dependency between templates.
    /// </summary>
    /// <param name="templateName">The name of the template that has dependencies.</param>
    /// <param name="dependsOn">The name of the template that is depended on.</param>
    void RegisterDependency(string templateName, string dependsOn);
    
    /// <summary>
    /// Registers multiple dependencies for a template.
    /// </summary>
    /// <param name="templateName">The name of the template that has dependencies.</param>
    /// <param name="dependencies">The names of the templates that are depended on.</param>
    void RegisterDependencies(string templateName, IEnumerable<string> dependencies);
    
    /// <summary>
    /// Gets all dependencies for a template.
    /// </summary>
    /// <param name="templateName">The name of the template to get dependencies for.</param>
    /// <returns>A collection of template names that the specified template depends on.</returns>
    IReadOnlyCollection<string> GetDependencies(string templateName);
    
    /// <summary>
    /// Gets all dependent templates for a template.
    /// </summary>
    /// <param name="templateName">The name of the template to get dependents for.</param>
    /// <returns>A collection of template names that depend on the specified template.</returns>
    IReadOnlyCollection<string> GetDependents(string templateName);
    
    /// <summary>
    /// Determines whether a template has dependencies.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template has dependencies; otherwise, false.</returns>
    bool HasDependencies(string templateName);
    
    /// <summary>
    /// Determines whether a template has dependents.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template has dependents; otherwise, false.</returns>
    bool HasDependents(string templateName);
    
    /// <summary>
    /// Gets a sorted list of templates in dependency order.
    /// </summary>
    /// <param name="templates">The templates to sort.</param>
    /// <returns>A collection of template names sorted in dependency order.</returns>
    IReadOnlyCollection<string> GetTemplatesInDependencyOrder(IEnumerable<string> templates);
    
    /// <summary>
    /// Registers an inclusion for a template.
    /// </summary>
    /// <param name="templateName">The name of the template that includes another template.</param>
    /// <param name="includeName">The name of the template to include.</param>
    void RegisterInclusion(string templateName, string includeName);
    
    /// <summary>
    /// Gets all inclusions for a template.
    /// </summary>
    /// <param name="templateName">The name of the template to get inclusions for.</param>
    /// <returns>A collection of template names that are included by the specified template.</returns>
    IReadOnlyCollection<string> GetInclusions(string templateName);
    
    /// <summary>
    /// Determines whether a template is included by any other templates.
    /// </summary>
    /// <param name="templateName">The name of the template to check.</param>
    /// <returns>True if the template is included by any other templates; otherwise, false.</returns>
    bool IsIncluded(string templateName);
    
    /// <summary>
    /// Determines whether a circular dependency exists in the template dependencies.
    /// </summary>
    /// <returns>True if a circular dependency exists; otherwise, false.</returns>
    bool HasCircularDependencies();
    
    /// <summary>
    /// Gets any circular dependencies that exist in the template dependencies.
    /// </summary>
    /// <returns>A collection of template name cycles representing circular dependencies.</returns>
    IReadOnlyCollection<IReadOnlyCollection<string>> GetCircularDependencies();
    
    /// <summary>
    /// Clears all registered dependencies and inclusions.
    /// </summary>
    void Clear();
}
