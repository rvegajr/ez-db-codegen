using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;

#nullable enable

namespace EzDbCodeGen.TemplateEngine.Helpers
{
    /// <summary>
    /// Handlebars helpers for generating test values and other testing utilities.
    /// </summary>
    public class HandlebarsTestHelpers : ITestHelpers, IHelperRegistration
    {
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Registers all test helpers with the template engine.
        /// </summary>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            if (templateEngine == null)
            {
                throw new ArgumentNullException(nameof(templateEngine));
            }
            
            // Register the getTestValue helper
            templateEngine.RegisterHelper("getTestValue", (writer, context, parameters) =>
            {
                if (parameters.Length < 1)
                {
                    throw new HandlebarsException("getTestValue helper requires at least one parameter: type");
                }
                
                string type = parameters[0]?.ToString();
                int seed = parameters.Length > 1 && parameters[1] != null ? Convert.ToInt32(parameters[1]) : 1;
                
                string testValue = GenerateTestValue(type, seed);
                writer.Write(testValue);
            });
            
            // Register the pluralize helper
            templateEngine.RegisterHelper("pluralize", (writer, context, parameters) =>
            {
                if (parameters.Length < 1)
                {
                    throw new HandlebarsException("pluralize helper requires one parameter: word");
                }
                
                string word = parameters[0]?.ToString();
                string pluralized = Pluralize(word);
                writer.Write(pluralized);
            });
            
            // Register the navigationPropertyName helper
            templateEngine.RegisterHelper("navigationPropertyName", (writer, context, parameters) =>
            {
                if (parameters.Length < 2)
                {
                    throw new HandlebarsException("navigationPropertyName helper requires at least two parameters: tableName and isCollection");
                }
                
                string tableName = parameters[0]?.ToString();
                bool isCollection = Convert.ToBoolean(parameters[1]);
                string suffix = parameters.Length > 2 ? parameters[2]?.ToString() : null;
                
                string propertyName = GenerateNavigationPropertyName(tableName, isCollection, suffix);
                writer.Write(propertyName);
            });
        }
        
        /// <summary>
        /// Generates a test value for the specified C# type.
        /// </summary>
        /// <param name="type">The C# type to generate a test value for.</param>
        /// <param name="seed">A seed value to generate unique test values.</param>
        /// <returns>A string representation of a test value.</returns>
        public string GenerateTestValue(string type, int seed)
        {
            if (string.IsNullOrEmpty(type))
            {
                return "null";
            }
            
            bool isNullable = type.EndsWith("?");
            string baseType = isNullable ? type.Substring(0, type.Length - 1) : type;
            
            // For nullable types with seed 0, return null
            if (isNullable && seed % 5 == 0)
            {
                return "null";
            }
            
            switch (baseType.ToLowerInvariant())
            {
                case "string":
                    return $"\"TestValue{seed}\"";
                    
                case "int":
                case "int32":
                    return (seed * 100).ToString();
                    
                case "long":
                case "int64":
                    return (seed * 10000L).ToString() + "L";
                    
                case "decimal":
                    return (seed * 100.5m).ToString() + "m";
                    
                case "double":
                    return (seed * 100.5).ToString() + "d";
                    
                case "float":
                    return (seed * 100.5f).ToString() + "f";
                    
                case "bool":
                case "boolean":
                    return (seed % 2 == 0).ToString().ToLowerInvariant();
                    
                case "datetime":
                    return $"DateTime.Parse(\"{DateTime.Now.AddDays(seed).ToString("yyyy-MM-dd")}\")";
                    
                case "datetimeoffset":
                    return $"DateTimeOffset.Parse(\"{DateTimeOffset.Now.AddDays(seed).ToString("yyyy-MM-dd")}\")";
                    
                case "timespan":
                    return $"TimeSpan.FromHours({seed})";
                    
                case "guid":
                    return $"Guid.Parse(\"{new Guid(seed, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)seed)}\")";
                    
                case "byte[]":
                    return $"new byte[] {{ {string.Join(", ", Enumerable.Range(0, 5).Select(i => ((i + seed) % 256).ToString()))} }}";
                    
                default:
                    // For unknown types, return null
                    return "null";
            }
        }
        
        /// <summary>
        /// Pluralizes an English word.
        /// </summary>
        /// <param name="word">The word to pluralize.</param>
        /// <returns>The pluralized word.</returns>
        public string Pluralize(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }
            
            // Simple pluralization rules
            if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                char secondToLast = word.Length > 1 ? word[word.Length - 2] : '\0';
                if (!"aeiou".Contains(char.ToLowerInvariant(secondToLast)))
                {
                    return word.Substring(0, word.Length - 1) + "ies";
                }
            }
            
            if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return word + "es";
            }
            
            // Special cases
            if (word.Equals("Person", StringComparison.OrdinalIgnoreCase))
            {
                return "People";
            }
            
            if (word.Equals("Child", StringComparison.OrdinalIgnoreCase))
            {
                return "Children";
            }
            
            if (word.Equals("Foot", StringComparison.OrdinalIgnoreCase))
            {
                return "Feet";
            }
            
            if (word.Equals("Tooth", StringComparison.OrdinalIgnoreCase))
            {
                return "Teeth";
            }
            
            if (word.Equals("Goose", StringComparison.OrdinalIgnoreCase))
            {
                return "Geese";
            }
            
            // Default: add 's'
            return word + "s";
        }
        
        /// <summary>
        /// Generates a navigation property name based on the table name.
        /// </summary>
        /// <param name="tableName">The name of the related table.</param>
        /// <param name="isCollection">Whether the navigation property is a collection.</param>
        /// <param name="suffix">Optional suffix to add to the property name.</param>
        /// <returns>A properly formatted navigation property name.</returns>
        public string GenerateNavigationPropertyName(string tableName, bool isCollection, string? suffix = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return string.Empty;
            }

            var stringHelpers = new HandlebarsStringFormatHelpers();
            var propertyName = stringHelpers.ToPascalCase(tableName);
            if (isCollection)
            {
                propertyName = stringHelpers.Pluralize(propertyName);
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                propertyName += suffix;
            }

            return propertyName;
        }
    }
}
