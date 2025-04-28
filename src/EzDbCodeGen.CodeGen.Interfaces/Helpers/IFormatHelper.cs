namespace EzDbCodeGen.CodeGen.Interfaces.Helpers;

/// <summary>
/// Defines an interface for string formatting helpers that support case transformations, indentation, tabbing, and operation chaining.
/// </summary>
public interface IFormatHelper : ITemplateHelper
{
    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The camelCase version of the string.</returns>
    string ToCamelCase(string value);
    
    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The PascalCase version of the string.</returns>
    string ToPascalCase(string value);
    
    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The snake_case version of the string.</returns>
    string ToSnakeCase(string value);
    
    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The kebab-case version of the string.</returns>
    string ToKebabCase(string value);
    
    /// <summary>
    /// Indents each line of a string by the specified amount.
    /// </summary>
    /// <param name="value">The string to indent.</param>
    /// <param name="indentSize">The number of spaces to indent by.</param>
    /// <returns>The indented string.</returns>
    string Indent(string value, int indentSize);
    
    /// <summary>
    /// Adds tab indentation to each line of a string.
    /// </summary>
    /// <param name="value">The string to tab indent.</param>
    /// <param name="tabCount">The number of tabs to add.</param>
    /// <returns>The tab-indented string.</returns>
    string TabIndent(string value, int tabCount);
    
    /// <summary>
    /// Formats a string to a specified number of spaces per indentation level.
    /// </summary>
    /// <param name="value">The string to format.</param>
    /// <param name="spacesPerIndent">The number of spaces per indentation level.</param>
    /// <returns>The formatted string.</returns>
    string FormatIndentation(string value, int spacesPerIndent);
    
    /// <summary>
    /// Pluralizes a string according to a count.
    /// </summary>
    /// <param name="value">The string to potentially pluralize.</param>
    /// <param name="count">The count to check.</param>
    /// <returns>The original string or its plural form.</returns>
    string Pluralize(string value, int count);
    
    /// <summary>
    /// Singularizes a string.
    /// </summary>
    /// <param name="value">The string to singularize.</param>
    /// <returns>The singular form of the string.</returns>
    string Singularize(string value);
}
