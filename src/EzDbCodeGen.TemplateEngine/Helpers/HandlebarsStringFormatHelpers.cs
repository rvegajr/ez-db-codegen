using System;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;

#nullable enable

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
            templateEngine.RegisterHelper("camelCase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToCamelCase(value.ToString()));
            });

            // Register PascalCase helper
            templateEngine.RegisterHelper("pascalCase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToPascalCase(value.ToString()));
            });

            // Register snake_case helper
            templateEngine.RegisterHelper("snakeCase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToSnakeCase(value.ToString()));
            });

            // Register kebab-case helper
            templateEngine.RegisterHelper("kebabCase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToKebabCase(value.ToString()));
            });

            // Register trim helper
            templateEngine.RegisterHelper("trim", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(value.ToString().Trim());
            });

            // Register lowercase helper
            templateEngine.RegisterHelper("lowercase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToLowerCase(value.ToString()));
            });

            // Register uppercase helper
            templateEngine.RegisterHelper("uppercase", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                var value = arguments.Length > 0 ? arguments[0] : null;
                if (value == null) return;
                writer.Write(ToUpperCase(value.ToString()));
            });

            // Register indent helper
            templateEngine.RegisterBlockHelper("indent", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                if (arguments.Length < 2)
                {
                    return;
                }
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                if (!int.TryParse(arguments[1]?.ToString(), out var indentLevel))
                {
                    indentLevel = 1;
                }
                
                var indentation = new string(' ', indentLevel * 4);
                writer.Write(Indent(text, indentation));
            });

            // Register tab helper
            templateEngine.RegisterBlockHelper("tab", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                if (arguments.Length < 2)
                {
                    return;
                }
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                if (!int.TryParse(arguments[1]?.ToString(), out var tabCount))
                {
                    tabCount = 1;
                }
                
                var indentation = new string('\t', tabCount);
                writer.Write(Indent(text, indentation));
            });

            // Register string format helper
            templateEngine.RegisterBlockHelper("format", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                if (arguments.Length < 2)
                {
                    return;
                }
                
                var format = arguments[0]?.ToString() ?? string.Empty;
                var args = new object[arguments.Length - 1];
                
                for (int i = 1; i < arguments.Length; i++)
                {
                    args[i - 1] = arguments[i];
                }
                
                try
                {
                    writer.Write(string.Format(format, args));
                }
                catch (Exception)
                {
                    writer.Write(string.Empty);
                }
            });
        }

        public string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var result = words[0].ToLower();

            for (int i = 1; i < words.Length; i++)
            {
                result += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }

            return result;
        }

        public string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Empty;

            foreach (var word in words)
            {
                result += char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }

            return result;
        }

        public string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("_", words.Select(w => w.ToLower()));
        }

        public string ToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("-", words.Select(w => w.ToLower()));
        }

        public string ToConstantCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join("_", words.Select(w => w.ToUpper()));
        }

        public string Pluralize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Simple pluralization rules
            if (input.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                char secondToLast = input.Length > 1 ? input[input.Length - 2] : '\0';
                if (!"aeiou".Contains(char.ToLowerInvariant(secondToLast)))
                {
                    return input.Substring(0, input.Length - 1) + "ies";
                }
            }

            if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                input.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                input.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                input.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                input.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return input + "es";
            }

            return input + "s";
        }

        public string Singularize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (input.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(0, input.Length - 3) + "y";
            }

            if (input.EndsWith("es", StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(0, input.Length - 2);
            }

            if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(0, input.Length - 1);
            }

            return input;
        }

        public string Humanize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var words = input.Split(new[] { '_', ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Join(" ", words.Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));

            return result;
        }

        public string Indent(string input, int spaces)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var indentation = new string(' ', spaces);
            return string.Join("\n", input.Split('\n').Select(line => indentation + line));
        }

        public string IndentLines(string input, int spaces)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var indentation = new string(' ', spaces);
            return string.Join("\n", input.Split('\n').Select(line => indentation + line));
        }

        private static string ToUpperCase(string input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : input.ToUpper();
        }

        private static string ToLowerCase(string input)
        {
            return string.IsNullOrEmpty(input) ? string.Empty : input.ToLower();
        }

        private static string Indent(string text, string indentation)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var lines = text.Split('\n');
            return string.Join("\n", lines.Select(line => indentation + line));
        }
    }
}
