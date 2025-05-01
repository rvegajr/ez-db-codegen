namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that generate documentation.
/// </summary>
public interface IDocumentationHelper : IHandlebarsHelper
{
    /// <summary>
    /// Generates documentation for the specified object.
    /// </summary>
    /// <param name="obj">The object to document.</param>
    /// <param name="format">The documentation format (e.g., XML, Markdown).</param>
    /// <returns>The generated documentation.</returns>
    string GenerateDocumentation(object obj, string format);
}
