using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.Interfaces.Utilities;

namespace EzDbCodeGen.Core.Utilities
{
    /// <summary>
    /// Implementation of the IStringUtility interface providing string manipulation utilities.
    /// </summary>
    public class StringUtility : IStringUtility
    {
        // Dictionary for irregular plurals that aren't handled by standard rules
        private static readonly Dictionary<string, string> _irregularPlurals = new(StringComparer.OrdinalIgnoreCase)
        {
            { "child", "children" },
            { "person", "people" },
            { "man", "men" },
            { "woman", "women" },
            { "tooth", "teeth" },
            { "foot", "feet" },
            { "mouse", "mice" },
            { "goose", "geese" },
            { "datum", "data" },
            { "criterion", "criteria" },
            { "analysis", "analyses" },
            { "ox", "oxen" },
            { "vertex", "vertices" },
            { "index", "indices" },
            { "matrix", "matrices" },
            { "quiz", "quizzes" }
        };

        // Dictionary for singular forms of irregular plurals
        private static readonly Dictionary<string, string> _irregularSingulars;

        // Words that are both singular and plural (no change)
        private static readonly HashSet<string> _uncountable = new(StringComparer.OrdinalIgnoreCase)
        {
            "equipment", "information", "rice", "money", "species", "series", 
            "fish", "sheep", "deer", "aircraft", "feedback", "data"
        };

        // Static constructor to initialize the singular dictionary
        static StringUtility()
        {
            _irregularSingulars = _irregularPlurals.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        /// <inheritdoc/>
        public string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Split the input into words
            var words = SplitIntoWords(input);

            // Convert each word to title case and join them
            var result = string.Join("", words.Select(word => 
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word.ToLower())
            ));

            return result;
        }

        /// <inheritdoc/>
        public string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // First convert to PascalCase
            string pascalCase = ToPascalCase(input);

            // Then convert the first character to lowercase
            if (pascalCase.Length > 0)
            {
                return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
            }

            return pascalCase;
        }

        /// <inheritdoc/>
        public string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Split the input into words
            var words = SplitIntoWords(input);

            // Join words with underscore
            return string.Join("_", words.Select(word => word.ToLower()));
        }

        /// <inheritdoc/>
        public string ToKebabCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Split the input into words
            var words = SplitIntoWords(input);

            // Join words with hyphen
            return string.Join("-", words.Select(word => word.ToLower()));
        }

        /// <inheritdoc/>
        public string ToSingular(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Check for uncountable words
            if (_uncountable.Contains(input))
            {
                return input;
            }

            // Check for irregular plurals
            if (_irregularSingulars.TryGetValue(input, out string? irregular))
            {
                return irregular;
            }

            // Apply standard rules
            if (input.EndsWith("ies", StringComparison.OrdinalIgnoreCase))
            {
                // babies -> baby
                return input.Substring(0, input.Length - 3) + "y";
            }
            else if (input.EndsWith("es", StringComparison.OrdinalIgnoreCase))
            {
                // boxes -> box, dishes -> dish
                if (input.EndsWith("sses", StringComparison.OrdinalIgnoreCase) ||
                    input.EndsWith("shes", StringComparison.OrdinalIgnoreCase) ||
                    input.EndsWith("ches", StringComparison.OrdinalIgnoreCase) ||
                    input.EndsWith("xes", StringComparison.OrdinalIgnoreCase))
                {
                    return input.Substring(0, input.Length - 2);
                }
                else
                {
                    // Otherwise just remove the 's'
                    return input.Substring(0, input.Length - 1);
                }
            }
            else if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) && !input.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
            {
                // cats -> cat, but not bass -> bas
                return input.Substring(0, input.Length - 1);
            }

            // No change needed
            return input;
        }

        /// <inheritdoc/>
        public string ToPlural(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Check for uncountable words
            if (_uncountable.Contains(input))
            {
                return input;
            }

            // Check for irregular plurals
            if (_irregularPlurals.TryGetValue(input, out string? irregular))
            {
                return irregular;
            }

            // Apply standard rules

            // Words ending in "y" preceded by a consonant
            if (input.EndsWith("y", StringComparison.OrdinalIgnoreCase) && 
                input.Length > 1 && 
                !IsVowel(input[input.Length - 2]))
            {
                // city -> cities
                return input.Substring(0, input.Length - 1) + "ies";
            }
            // Words ending in s, x, z, ch, sh
            else if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                     input.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                     input.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                     input.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                     input.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                // box -> boxes, dish -> dishes
                return input + "es";
            }
            // Words ending in "o" preceded by a consonant
            else if (input.EndsWith("o", StringComparison.OrdinalIgnoreCase) && 
                     input.Length > 1 && 
                     !IsVowel(input[input.Length - 2]))
            {
                // hero -> heroes
                return input + "es";
            }
            else
            {
                // Regular plural: add "s"
                return input + "s";
            }
        }

        /// <inheritdoc/>
        public bool MatchesWildcard(string input, string pattern, bool ignoreCase = true)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            // Convert the wildcard pattern to a regex pattern
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return Regex.IsMatch(input, regexPattern, options);
        }

        /// <inheritdoc/>
        public string[] SplitIntoWords(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Array.Empty<string>();
            }

            // Handle snake_case and kebab-case
            input = input.Replace('_', ' ').Replace('-', ' ');

            // Handle camelCase and PascalCase by inserting spaces before capital letters
            var result = new StringBuilder(input.Length * 2);
            result.Append(input[0]);
            
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && 
                    (char.IsLower(input[i - 1]) || 
                     (i < input.Length - 1 && char.IsLower(input[i + 1]))))
                {
                    result.Append(' ');
                }
                result.Append(input[i]);
            }

            // Split by whitespace and remove empty entries
            return result.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <inheritdoc/>
        public string Join(IEnumerable<string> strings, string separator)
        {
            if (strings == null)
            {
                throw new ArgumentNullException(nameof(strings));
            }

            return string.Join(separator ?? string.Empty, strings);
        }

        /// <inheritdoc/>
        public string RemoveWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return Regex.Replace(input, @"\s+", string.Empty);
        }

        /// <inheritdoc/>
        public string DatabaseToCSharpName(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                return databaseName;
            }

            return ToPascalCase(databaseName);
        }

        /// <inheritdoc/>
        public string EscapeForCSharp(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        /// <inheritdoc/>
        public string GetValidCSharpIdentifier(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "_";
            }

            // Replace invalid characters with underscore
            var result = new StringBuilder();
            
            // First character must be a letter or underscore
            if (!char.IsLetter(input[0]) && input[0] != '_')
            {
                result.Append('_');
            }
            
            result.Append(input[0]);
            
            // Other characters can be letters, digits, or underscores
            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsLetterOrDigit(input[i]) || input[i] == '_')
                {
                    result.Append(input[i]);
                }
                else
                {
                    result.Append('_');
                }
            }

            // Check if the result is a C# keyword
            string identifier = result.ToString();
            if (IsCSharpKeyword(identifier))
            {
                identifier = "@" + identifier;
            }

            return identifier;
        }

        /// <inheritdoc/>
        public string GetValidNamespace(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "DefaultNamespace";
            }

            // Split the input into parts by dots
            string[] parts = input.Split('.');
            
            // Process each part and join with dots
            return string.Join(".", parts.Select(part => GetValidCSharpIdentifier(part)));
        }

        /// <summary>
        /// Determines if a character is a vowel.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is a vowel; otherwise, false.</returns>
        private static bool IsVowel(char c)
        {
            char lower = char.ToLowerInvariant(c);
            return lower == 'a' || lower == 'e' || lower == 'i' || lower == 'o' || lower == 'u';
        }

        /// <summary>
        /// Determines if a string is a C# keyword.
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns>True if the word is a C# keyword; otherwise, false.</returns>
        private static bool IsCSharpKeyword(string word)
        {
            string[] keywords = 
            {
                "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", 
                "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", 
                "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", 
                "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", 
                "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", 
                "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", 
                "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", 
                "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "async", "await", 
                "by", "descending", "dynamic", "equals", "from", "get", "global", "group", "into", "join", "let", 
                "nameof", "on", "orderby", "partial", "remove", "select", "set", "value", "var", "when", "where", "yield"
            };

            return Array.IndexOf(keywords, word.ToLowerInvariant()) >= 0;
        }
    }
}
