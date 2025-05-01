namespace EzDbCodeGen.TemplateEngine.Interfaces.Helpers;

/// <summary>
/// Interface for Handlebars helpers that format strings.
/// </summary>
public interface IStringFormatHelper : IHandlebarsHelper
{
    /// <summary>
    /// Formats a string according to the specified format.
    /// </summary>
    /// <param name="input">The input string to format.</param>
    /// <param name="format">The format to apply.</param>
    /// <returns>The formatted string.</returns>
    string FormatString(string input, string format);
}
