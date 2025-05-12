using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.Interfaces.TemplateEngine.Helpers;
using EzDbCodeGen.Interfaces.Utilities;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Core.TemplateEngine.Helpers
{
    /// <summary>
    /// Implementation of the ICodeFormatHelper interface for formatting code in templates.
    /// This helps generate superior, well-formatted code.
    /// </summary>
    public class CodeFormatHelper : ICodeFormatHelper
    {
        private readonly ILogger<CodeFormatHelper> _logger;
        private readonly IStringUtility _stringUtility;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFormatHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="stringUtility">The string utility to use.</param>
        public CodeFormatHelper(
            ILogger<CodeFormatHelper> logger,
            IStringUtility stringUtility)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stringUtility = stringUtility ?? throw new ArgumentNullException(nameof(stringUtility));
        }

        /// <inheritdoc/>
        public string ToPascalCase(string input)
        {
            return _stringUtility.ToPascalCase(input);
        }

        /// <inheritdoc/>
        public string ToCamelCase(string input)
        {
            return _stringUtility.ToCamelCase(input);
        }

        /// <inheritdoc/>
        public string ToSnakeCase(string input)
        {
            return _stringUtility.ToSnakeCase(input);
        }

        /// <inheritdoc/>
        public string ToKebabCase(string input)
        {
            return _stringUtility.ToKebabCase(input);
        }

        /// <inheritdoc/>
        public string Pluralize(string input)
        {
            return _stringUtility.ToPlural(input);
        }

        /// <inheritdoc/>
        public string Singularize(string input)
        {
            return _stringUtility.ToSingular(input);
        }

        /// <inheritdoc/>
        public string Indent(string input, int spaces = 4)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string indentation = new string(' ', spaces);
            
            // Split by lines, add indentation, and rejoin
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            for (int i = 0; i < lines.Length; i++)
            {
                // Skip empty lines
                if (!string.IsNullOrEmpty(lines[i]))
                {
                    lines[i] = indentation + lines[i];
                }
            }
            
            return string.Join(Environment.NewLine, lines);
        }

        /// <inheritdoc/>
        public string FormatComment(string input, string language = "csharp")
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Normalize the language
            language = language.ToLowerInvariant();

            // Format the comment based on the language
            switch (language)
            {
                case "csharp":
                case "cs":
                    return FormatCSharpComment(input);
                case "typescript":
                case "javascript":
                case "js":
                case "ts":
                    return FormatJavaScriptComment(input);
                case "java":
                    return FormatJavaComment(input);
                case "python":
                case "py":
                    return FormatPythonComment(input);
                case "sql":
                    return FormatSqlComment(input);
                default:
                    _logger.LogWarning($"Unsupported language for comment formatting: {language}. Using C# format.");
                    return FormatCSharpComment(input);
            }
        }

        /// <inheritdoc/>
        public string EscapeString(string input, string language = "csharp")
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Normalize the language
            language = language.ToLowerInvariant();

            // Escape the string based on the language
            switch (language)
            {
                case "csharp":
                case "cs":
                    return input
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
                case "typescript":
                case "javascript":
                case "js":
                case "ts":
                    return input
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
                case "java":
                    return input
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
                case "python":
                case "py":
                    return input
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
                case "sql":
                    return input.Replace("'", "''");
                default:
                    _logger.LogWarning($"Unsupported language for string escaping: {language}. Using C# format.");
                    return input
                        .Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\r", "\\r")
                        .Replace("\n", "\\n")
                        .Replace("\t", "\\t");
            }
        }

        /// <inheritdoc/>
        public string ConvertTabs(string input, int tabSize = 4)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Replace tabs with spaces
            return input.Replace("\t", new string(' ', tabSize));
        }

        /// <inheritdoc/>
        public string FormatIdentifierForCode(string input, string language = "csharp")
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Normalize the language
            language = language.ToLowerInvariant();

            // Format the identifier based on the language
            switch (language)
            {
                case "csharp":
                case "cs":
                    return _stringUtility.GetValidCSharpIdentifier(input);
                case "typescript":
                case "javascript":
                case "js":
                case "ts":
                    // JavaScript/TypeScript identifiers follow similar rules to C#
                    var identifier = Regex.Replace(input, @"[^\w$]", "_");
                    if (!Regex.IsMatch(identifier, @"^[a-zA-Z_$]"))
                    {
                        identifier = "_" + identifier;
                    }
                    return identifier;
                case "java":
                    // Java identifiers follow similar rules to C#
                    return _stringUtility.GetValidCSharpIdentifier(input);
                case "python":
                case "py":
                    // Python identifiers are similar but convention is snake_case
                    var pythonIdentifier = Regex.Replace(input, @"[^\w]", "_");
                    if (!Regex.IsMatch(pythonIdentifier, @"^[a-zA-Z_]"))
                    {
                        pythonIdentifier = "_" + pythonIdentifier;
                    }
                    return pythonIdentifier.ToLower();
                case "sql":
                    // SQL identifiers vary by database but we'll use a safe approach
                    return $"[{input.Replace("]", "]]")}]";
                default:
                    _logger.LogWarning($"Unsupported language for identifier formatting: {language}. Using C# format.");
                    return _stringUtility.GetValidCSharpIdentifier(input);
            }
        }

        /// <summary>
        /// Formats a comment in C# style.
        /// </summary>
        /// <param name="input">The comment text to format.</param>
        /// <returns>A formatted C# comment.</returns>
        private string FormatCSharpComment(string input)
        {
            // Split into lines
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            // If there's a single line, use // comment
            if (lines.Length == 1)
            {
                return "// " + lines[0];
            }
            
            // Otherwise use /* */ comment
            var result = new StringBuilder();
            result.AppendLine("/*");
            
            foreach (var line in lines)
            {
                result.AppendLine(" * " + line);
            }
            
            result.Append(" */");
            return result.ToString();
        }

        /// <summary>
        /// Formats a comment in JavaScript/TypeScript style.
        /// </summary>
        /// <param name="input">The comment text to format.</param>
        /// <returns>A formatted JavaScript/TypeScript comment.</returns>
        private string FormatJavaScriptComment(string input)
        {
            // JavaScript/TypeScript comments are similar to C#
            return FormatCSharpComment(input);
        }

        /// <summary>
        /// Formats a comment in Java style.
        /// </summary>
        /// <param name="input">The comment text to format.</param>
        /// <returns>A formatted Java comment.</returns>
        private string FormatJavaComment(string input)
        {
            // Java comments are similar to C#
            return FormatCSharpComment(input);
        }

        /// <summary>
        /// Formats a comment in Python style.
        /// </summary>
        /// <param name="input">The comment text to format.</param>
        /// <returns>A formatted Python comment.</returns>
        private string FormatPythonComment(string input)
        {
            // Split into lines
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            // If there's a single line, use # comment
            if (lines.Length == 1)
            {
                return "# " + lines[0];
            }
            
            // Otherwise use multiple # comments
            var result = new StringBuilder();
            
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    result.AppendLine("#");
                }
                else
                {
                    result.AppendLine("# " + line);
                }
            }
            
            // Remove the last newline
            if (result.Length > 0)
            {
                result.Length -= Environment.NewLine.Length;
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Formats a comment in SQL style.
        /// </summary>
        /// <param name="input">The comment text to format.</param>
        /// <returns>A formatted SQL comment.</returns>
        private string FormatSqlComment(string input)
        {
            // Split into lines
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            // If there's a single line, use -- comment
            if (lines.Length == 1)
            {
                return "-- " + lines[0];
            }
            
            // Otherwise use /* */ comment
            var result = new StringBuilder();
            result.AppendLine("/*");
            
            foreach (var line in lines)
            {
                result.AppendLine(" * " + line);
            }
            
            result.Append(" */");
            return result.ToString();
        }
    }
}
