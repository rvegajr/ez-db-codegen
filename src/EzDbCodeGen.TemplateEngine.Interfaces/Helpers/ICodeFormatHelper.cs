namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that format code.
/// </summary>
public interface ICodeFormatHelper : IHandlebarsHelper
{
    /// <summary>
    /// Formats the code according to the specified language.
    /// </summary>
    /// <param name="code">The code to format.</param>
    /// <param name="language">The programming language.</param>
    /// <returns>The formatted code.</returns>
    string FormatCode(string code, string language);
}
