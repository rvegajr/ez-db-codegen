using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Utilities
{
    /// <summary>
    /// Defines utilities for string operations.
    /// </summary>
    public interface IStringUtility
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
        /// Determines if a string matches a wildcard pattern.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="pattern">The wildcard pattern.</param>
        /// <param name="ignoreCase">Whether to ignore case.</param>
        /// <returns>True if the input matches the pattern, false otherwise.</returns>
        bool MatchesWildcard(string input, string pattern, bool ignoreCase = true);

        /// <summary>
        /// Splits a string into words.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>An array of words.</returns>
        string[] SplitIntoWords(string input);

        /// <summary>
        /// Joins an array of strings with a separator.
        /// </summary>
        /// <param name="strings">The strings to join.</param>
        /// <param name="separator">The separator to use.</param>
        /// <returns>The joined string.</returns>
        string Join(IEnumerable<string> strings, string separator);

        /// <summary>
        /// Removes all whitespace from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The string without whitespace.</returns>
        string RemoveWhitespace(string input);

        /// <summary>
        /// Converts a database name to a C# name.
        /// </summary>
        /// <param name="databaseName">The database name.</param>
        /// <returns>The C# name.</returns>
        string DatabaseToCSharpName(string databaseName);

        /// <summary>
        /// Escapes a string for use in C# code.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The escaped string.</returns>
        string EscapeForCSharp(string input);

        /// <summary>
        /// Gets a valid C# identifier from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A valid C# identifier.</returns>
        string GetValidCSharpIdentifier(string input);

        /// <summary>
        /// Gets a valid namespace from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>A valid namespace.</returns>
        string GetValidNamespace(string input);
    }
}
