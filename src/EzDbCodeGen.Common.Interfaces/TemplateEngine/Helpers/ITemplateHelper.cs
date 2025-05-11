using HandlebarsDotNet;

namespace EzDbCodeGen.Common.Interfaces.TemplateEngine.Helpers;

/// <summary>
/// Defines the base interface for all template helpers.
/// </summary>
public interface ITemplateHelper
{
    /// <summary>
    /// Registers the helpers with the specified Handlebars instance.
    /// </summary>
    /// <param name="handlebars">The Handlebars instance to register helpers with.</param>
    void RegisterHelpers(IHandlebars handlebars);
}
