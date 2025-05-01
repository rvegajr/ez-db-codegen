namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that handle type conversions.
/// </summary>
public interface ITypeConversionHelper : IHandlebarsHelper
{
    /// <summary>
    /// Converts a database type to a language-specific type.
    /// </summary>
    /// <param name="databaseType">The database type to convert.</param>
    /// <param name="language">The target programming language.</param>
    /// <returns>The converted type name.</returns>
    string ConvertType(string databaseType, string language);
}
