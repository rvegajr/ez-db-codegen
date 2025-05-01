namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that work with database relationships.
/// </summary>
public interface IRelationshipHelper : IHandlebarsHelper
{
    /// <summary>
    /// Gets the relationship object from the context.
    /// </summary>
    /// <param name="context">The template context.</param>
    /// <returns>The relationship object.</returns>
    object GetRelationshipObject(object context);
}
