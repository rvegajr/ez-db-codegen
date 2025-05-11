using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;

/// <summary>
/// Represents a collection of layout helpers for templates.
/// Implementation of the LayoutEz helper mentioned in the architectural specifications.
/// </summary>
public interface ILayoutHelpers : IHelperRegistration
{
    /// <summary>
    /// Defines a section in a template.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <param name="content">The content of the section.</param>
    /// <returns>The section content.</returns>
    string DefineSection(string name, string content);
    
    /// <summary>
    /// Renders a section in a template.
    /// </summary>
    /// <param name="name">The name of the section.</param>
    /// <returns>The rendered section content.</returns>
    string RenderSection(string name);
    
    /// <summary>
    /// Renders the body content of a layout.
    /// </summary>
    /// <returns>The rendered body content.</returns>
    string RenderBody();
}
