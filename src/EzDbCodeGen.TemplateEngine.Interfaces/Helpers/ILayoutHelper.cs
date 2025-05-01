namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that handle template layouts.
/// </summary>
public interface ILayoutHelper : IHandlebarsHelper
{
    /// <summary>
    /// Applies a layout template to the content.
    /// </summary>
    /// <param name="content">The content to wrap in a layout.</param>
    /// <param name="layoutName">The name of the layout template.</param>
    /// <returns>The content wrapped in the layout.</returns>
    string ApplyLayout(string content, string layoutName);
}
