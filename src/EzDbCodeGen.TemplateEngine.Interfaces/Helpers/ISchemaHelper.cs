namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that work with database schema objects.
/// </summary>
public interface ISchemaHelper : IHandlebarsHelper
{
    /// <summary>
    /// Gets the schema object from the context.
    /// </summary>
    /// <param name="context">The template context.</param>
    /// <returns>The schema object.</returns>
    object GetSchemaObject(object context);
}
