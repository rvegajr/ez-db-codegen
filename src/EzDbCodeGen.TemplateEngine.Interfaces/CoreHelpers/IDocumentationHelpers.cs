namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of documentation helpers for templates.
/// Implementation of the DocEz helper mentioned in the architectural specifications.
/// </summary>
public interface IDocumentationHelpers : IHelperRegistration
{
    /// <summary>
    /// Generates XML documentation for a class.
    /// </summary>
    /// <param name="name">The class name.</param>
    /// <param name="description">The class description.</param>
    /// <returns>The XML documentation.</returns>
    string GenerateXmlClassComment(string name, string description);
    
    /// <summary>
    /// Generates XML documentation for a property.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="description">The property description.</param>
    /// <returns>The XML documentation.</returns>
    string GenerateXmlPropertyComment(string name, string description);
    
    /// <summary>
    /// Generates XML documentation for a method.
    /// </summary>
    /// <param name="name">The method name.</param>
    /// <param name="description">The method description.</param>
    /// <param name="returnDesc">The return value description.</param>
    /// <returns>The XML documentation.</returns>
    string GenerateXmlMethodComment(string name, string description, string returnDesc);
}
