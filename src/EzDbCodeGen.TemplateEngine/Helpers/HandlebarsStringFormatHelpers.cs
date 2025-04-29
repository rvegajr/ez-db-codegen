using System;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides string formatting helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsStringFormatHelpers : IStringFormatHelpers, IHelperRegistration
    {
        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register camelCase helper
            templateEngine.RegisterHelper("camelCase", (context) => {
                if (context == null) return string.Empty;
                return ToCamelCase(context.ToString());
            });

            // Register PascalCase helper
            templateEngine.RegisterHelper("pascalCase", (context) => {
                if (context == null) return string.Empty;
                return ToPascalCase(context.ToString());
            });

            // Register snake_case helper
            templateEngine.RegisterHelper("snakeCase", (context) => {
                if (context == null) return string.Empty;
                return ToSnakeCase(context.ToString());
            });

            // Register kebab-case helper
            templateEngine.RegisterHelper("kebabCase", (context) => {
                if (context == null) return string.Empty;
                return ToKebabCase(context.ToString());
            });

            // Register trim helper
            templateEngine.RegisterHelper("trim", (context) => {
                if (context == null) return string.Empty;
                return context.ToString().Trim();
            });

            // Register lowercase helper
            templateEngine.RegisterHelper("lowercase", (context) => {
                if (context == null) return string.Empty;
                return context.ToString().ToLowerInvariant();
            });

            // Register uppercase helper
            templateEngine.RegisterHelper("uppercase", (context) => {
                if (context == null) return string.Empty;
                return context.ToString().ToUpperInvariant();
            });

            // Register indent helper
            templateEngine.RegisterHelper("indent", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                if (!int.TryParse(arguments[1]?.ToString(), out var indentLevel))
                {
                    indentLevel = 1;
                }
                
                var indentation = new string(' ', indentLevel * 4);
                return Indent(text, indentation);
            });

            // Register tab helper
            templateEngine.RegisterHelper("tab", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                if (!int.TryParse(arguments[1]?.ToString(), out var tabCount))
                {
                    tabCount = 1;
                }
                
                var indentation = new string('\t', tabCount);
                return Indent(text, indentation);
            });

            // Register string format helper
            templateEngine.RegisterHelper("format", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var format = arguments[0]?.ToString() ?? string.Empty;
                var args = new object[arguments.Length - 1];
                
                for (int i = 1; i < arguments.Length; i++)
                {
                    args[i - 1] = arguments[i];
                }
                
                try
                {
                    return string.Format(format, args);
                }
                catch (Exception)
                {
                    return format;
                }
            });
        }

        private string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            
            // First convert to pascal case, then lowercase the first character
            var pascalCase = ToPascalCase(input);
            return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
        }

        private string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            
            // Replace non-alphanumeric characters with spaces
            var normalized = Regex.Replace(input, @"[^\w]", " ");
            
            // Split by spaces and uppercase the first character of each word
            var words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            
            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    sb.Append(char.ToUpperInvariant(word[0]));
                    if (word.Length > 1)
                    {
                        sb.Append(word.Substring(1).ToLowerInvariant());
                    }
                }
            }
            
            return sb.ToString();
        }

        private string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            
            // Insert underscore before capital letters and lowercase everything
            var result = Regex.Replace(input, @"([a-z])([A-Z])", "$1_$2");
            
            // Replace non-alphanumeric characters with underscores
            result = Regex.Replace(result, @"[^\w]", "_");
            
            // Remove consecutive underscores and lowercase everything
            result = Regex.Replace(result, @"_{2,}", "_");
            
            return result.ToLowerInvariant();
        }

        private string ToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            
            // Convert to snake case first, then replace underscores with hyphens
            return ToSnakeCase(input).Replace('_', '-');
        }

        private string Indent(string text, string indentation)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            
            foreach (var line in lines)
            {
                sb.AppendLine(indentation + line);
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}
