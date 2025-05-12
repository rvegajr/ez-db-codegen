namespace EzDbCodeGen.Interfaces.TemplateEngine.Helpers
{
    /// <summary>
    /// Defines helpers for code formatting in templates.
    /// </summary>
    public interface ICodeFormatHelper
    {
        /// <summary>
        /// Converts a string to PascalCase.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The PascalCase string.</returns>
        string ToPascalCase(string input);

        /// <summary>
        /// Converts a string to camelCase.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The camelCase string.</returns>
        string ToCamelCase(string input);

        /// <summary>
        /// Converts a string to snake_case.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The snake_case string.</returns>
        string ToSnakeCase(string input);

        /// <summary>
        /// Converts a string to kebab-case.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The kebab-case string.</returns>
        string ToKebabCase(string input);

        /// <summary>
        /// Gets the singular form of a word.
        /// </summary>
        /// <param name="input">The input word.</param>
        /// <returns>The singular form of the word.</returns>
        string ToSingular(string input);

        /// <summary>
        /// Gets the plural form of a word.
        /// </summary>
        /// <param name="input">The input word.</param>
        /// <returns>The plural form of the word.</returns>
        string ToPlural(string input);

        /// <summary>
        /// Escapes a string for C# code.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The escaped string.</returns>
        string EscapeCSharpString(string input);

        /// <summary>
        /// Formats XML documentation for C# code.
        /// </summary>
        /// <param name="description">The description to format.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <returns>The formatted XML documentation.</returns>
        string FormatXmlDocumentation(string description, int indentLevel = 1);

        /// <summary>
        /// Formats a code block with proper indentation.
        /// </summary>
        /// <param name="code">The code to format.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <returns>The formatted code block.</returns>
        string FormatCodeBlock(string code, int indentLevel = 1);

        /// <summary>
        /// Gets a comma-separated list of items.
        /// </summary>
        /// <param name="items">The items to join.</param>
        /// <returns>A comma-separated list of items.</returns>
        string JoinWithCommas(string[] items);

        /// <summary>
        /// Gets a C# string representation of an object.
        /// </summary>
        /// <param name="value">The value to represent.</param>
        /// <returns>The C# string representation of the value.</returns>
        string ToCSharpLiteral(object value);

        /// <summary>
        /// Gets the proper C# namespace for a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The C# namespace.</returns>
        string ToNamespace(string input);
    }
}
