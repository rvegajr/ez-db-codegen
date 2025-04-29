namespace EzDbCodeGen.Core.TemplateEngine.Helpers;

/// <summary>
/// Represents a collection of string formatting helpers for templates.
/// Implementation of the FormatEz helper mentioned in the architectural specifications.
/// </summary>
public interface IStringFormatHelpers
{
    /// <summary>
    /// Converts a string to camelCase format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The camelCase formatted string.</returns>
    string ToCamelCase(string input);
    
    /// <summary>
    /// Converts a string to PascalCase format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The PascalCase formatted string.</returns>
    string ToPascalCase(string input);
    
    /// <summary>
    /// Converts a string to snake_case format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The snake_case formatted string.</returns>
    string ToSnakeCase(string input);
    
    /// <summary>
    /// Converts a string to kebab-case format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The kebab-case formatted string.</returns>
    string ToKebabCase(string input);
    
    /// <summary>
    /// Converts a string to CONSTANT_CASE format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The CONSTANT_CASE formatted string.</returns>
    string ToConstantCase(string input);
    
    /// <summary>
    /// Pluralizes a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The pluralized string.</returns>
    string Pluralize(string input);
    
    /// <summary>
    /// Singularizes a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The singularized string.</returns>
    string Singularize(string input);
    
    /// <summary>
    /// Humanizes a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The humanized string.</returns>
    string Humanize(string input);
    
    /// <summary>
    /// Indents a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="spaces">The number of spaces to indent.</param>
    /// <returns>The indented string.</returns>
    string Indent(string input, int spaces);
    
    /// <summary>
    /// Indents each line in a string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="spaces">The number of spaces to indent.</param>
    /// <returns>The indented string.</returns>
    string IndentLines(string input, int spaces);
}
