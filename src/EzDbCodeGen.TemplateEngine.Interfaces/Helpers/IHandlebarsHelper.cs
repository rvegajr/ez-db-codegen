namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Base interface for Handlebars helper implementations.
/// </summary>
public interface IHandlebarsHelper
{
    /// <summary>
    /// Gets the name of the helper.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers the helper with the Handlebars engine.
    /// </summary>
    /// <param name="handlebars">The Handlebars engine instance.</param>
    void Register(HandlebarsDotNet.IHandlebars handlebars);
}
