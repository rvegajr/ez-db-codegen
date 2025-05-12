using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EzDbCodeGen.Interfaces.TemplateEngine;
using EzDbCodeGen.Interfaces.Utilities;
using HandlebarsNet = HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Interfaces.TemplateEngine.Helpers;

namespace EzDbCodeGen.Core.TemplateEngine
{
    /// <summary>
    /// Implementation of the ITemplateProcessor interface for processing Handlebars templates.
    /// This template processor offers significantly more powerful template processing capabilities than EF Core Power Tools.
    /// </summary>
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly HandlebarsNet.IHandlebars _handlebars;
        private readonly IFileUtility _fileUtility;
        private readonly ILogger<TemplateProcessor> _logger;
        private readonly Dictionary<string, HandlebarsNet.HandlebarsTemplate<object, object>> _compiledTemplates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessor"/> class.
        /// </summary>
        /// <param name="fileUtility">The file utility to use.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="schemaHelper">The schema helper to register.</param>
        /// <param name="relationshipHelper">The relationship helper to register.</param>
        /// <param name="codeFormatHelper">The code format helper to register.</param>
        public TemplateProcessor(
            IFileUtility fileUtility,
            ILogger<TemplateProcessor> logger,
            ISchemaHelper schemaHelper = null,
            IRelationshipHelper relationshipHelper = null,
            ICodeFormatHelper codeFormatHelper = null)
        {
            _fileUtility = fileUtility ?? throw new ArgumentNullException(nameof(fileUtility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Initialize Handlebars
            _handlebars = HandlebarsNet.Handlebars.Create(new HandlebarsNet.HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = true,
                NoEscape = true
            });

            // Register built-in helpers
            RegisterBuiltInHelpers();

            // Register provided helpers
            if (schemaHelper != null)
            {
                RegisterSchemaHelper(schemaHelper);
            }

            if (relationshipHelper != null)
            {
                RegisterRelationshipHelper(relationshipHelper);
            }

            if (codeFormatHelper != null)
            {
                RegisterCodeFormatHelper(codeFormatHelper);
            }
        }

        /// <inheritdoc/>
        public string ProcessTemplate(string templateContent, object model)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            // Compile the template or get from cache
            var template = GetOrCompileTemplate(templateContent);

            // Process the template
            return template(model);
        }

        /// <inheritdoc/>
        public string ProcessTemplateFile(string templatePath, object model)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
            }

            // Read the template file
            string templateContent = _fileUtility.ReadAllText(templatePath);

            // Process the template
            return ProcessTemplate(templateContent, model);
        }

        /// <inheritdoc/>
        public Task<string> ProcessTemplateAsync(string templateContent, object model)
        {
            // Handlebars doesn't have async processing, so we'll just call the synchronous method
            return Task.FromResult(ProcessTemplate(templateContent, model));
        }

        /// <inheritdoc/>
        public async Task<string> ProcessTemplateFileAsync(string templatePath, object model)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
            }

            // Read the template file
            string templateContent = await _fileUtility.ReadAllTextAsync(templatePath);

            // Process the template
            return ProcessTemplate(templateContent, model);
        }

        /// <inheritdoc/>
        public void RegisterHelper(string name, Delegate helperFunction)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Helper name cannot be null or empty.", nameof(name));
            }

            if (helperFunction == null)
            {
                throw new ArgumentNullException(nameof(helperFunction));
            }

            _handlebars.RegisterHelper(name, helperFunction);
        }

        /// <inheritdoc/>
        public void RegisterBlockHelper(string name, Delegate blockHelperFunction)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Block helper name cannot be null or empty.", nameof(name));
            }

            if (blockHelperFunction == null)
            {
                throw new ArgumentNullException(nameof(blockHelperFunction));
            }

            _handlebars.RegisterHelper(name, blockHelperFunction);
        }

        /// <inheritdoc/>
        public void RegisterPartial(string name, string partialContent)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Partial name cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(partialContent))
            {
                throw new ArgumentException("Partial content cannot be null or empty.", nameof(partialContent));
            }

            _handlebars.RegisterTemplate(name, partialContent);
        }

        /// <inheritdoc/>
        public void RegisterPartialFile(string name, string partialPath)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Partial name cannot be null or empty.", nameof(name));
            }

            if (string.IsNullOrEmpty(partialPath))
            {
                throw new ArgumentException("Partial path cannot be null or empty.", nameof(partialPath));
            }

            string partialContent = _fileUtility.ReadAllText(partialPath);
            RegisterPartial(name, partialContent);
        }

        /// <summary>
        /// Gets a compiled template from the cache or compiles and caches a new one.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <returns>The compiled template.</returns>
        private HandlebarsNet.HandlebarsTemplate<object, object> GetOrCompileTemplate(string templateContent)
        {
            // Use a hash of the template content as the key
            string key = ComputeHash(templateContent);

            if (!_compiledTemplates.TryGetValue(key, out var template))
            {
                template = _handlebars.Compile(templateContent);
                _compiledTemplates[key] = template;
            }

            return template;
        }

        /// <summary>
        /// Computes a hash of the template content for caching.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <returns>A hash of the template content.</returns>
        private string ComputeHash(string templateContent)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(templateContent);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Registers built-in Handlebars helpers.
        /// </summary>
        private void RegisterBuiltInHelpers()
        {
            // String manipulation helpers
            RegisterHelper("lowercase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return arguments[0].ToString().ToLowerInvariant();
            }));

            RegisterHelper("uppercase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return arguments[0].ToString().ToUpperInvariant();
            }));

            RegisterHelper("capitalize", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                string value = arguments[0].ToString();
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }
                return char.ToUpperInvariant(value[0]) + value.Substring(1);
            }));

            // Comparison helpers
            RegisterHelper("eq", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return false;
                }
                return Equals(arguments[0], arguments[1]);
            }));

            RegisterHelper("neq", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return true;
                }
                return !Equals(arguments[0], arguments[1]);
            }));

            RegisterHelper("lt", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return false;
                }
                return Compare(arguments[0], arguments[1]) < 0;
            }));

            RegisterHelper("lte", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return false;
                }
                return Compare(arguments[0], arguments[1]) <= 0;
            }));

            RegisterHelper("gt", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return false;
                }
                return Compare(arguments[0], arguments[1]) > 0;
            }));

            RegisterHelper("gte", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return false;
                }
                return Compare(arguments[0], arguments[1]) >= 0;
            }));

            // Logic helpers
            RegisterHelper("and", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                foreach (var arg in arguments)
                {
                    if (arg == null || arg.Equals(false))
                    {
                        return false;
                    }
                }
                return true;
            }));

            RegisterHelper("or", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                foreach (var arg in arguments)
                {
                    if (arg != null && !arg.Equals(false))
                    {
                        return true;
                    }
                }
                return false;
            }));

            RegisterHelper("not", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0)
                {
                    return true;
                }
                var arg = arguments[0];
                return arg == null || arg.Equals(false);
            }));

            // Math helpers
            RegisterHelper("add", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return 0;
                }
                if (TryConvertToDouble(arguments[0], out double a) && TryConvertToDouble(arguments[1], out double b))
                {
                    return a + b;
                }
                return 0;
            }));

            RegisterHelper("subtract", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return 0;
                }
                if (TryConvertToDouble(arguments[0], out double a) && TryConvertToDouble(arguments[1], out double b))
                {
                    return a - b;
                }
                return 0;
            }));

            RegisterHelper("multiply", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return 0;
                }
                if (TryConvertToDouble(arguments[0], out double a) && TryConvertToDouble(arguments[1], out double b))
                {
                    return a * b;
                }
                return 0;
            }));

            RegisterHelper("divide", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return 0;
                }
                if (TryConvertToDouble(arguments[0], out double a) && TryConvertToDouble(arguments[1], out double b) && b != 0)
                {
                    return a / b;
                }
                return 0;
            }));

            // Array helpers
            RegisterHelper("first", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                if (arguments[0] is System.Collections.IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        return item;
                    }
                }
                return null;
            }));

            RegisterHelper("last", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                if (arguments[0] is System.Collections.IEnumerable enumerable)
                {
                    object last = null;
                    foreach (var item in enumerable)
                    {
                        last = item;
                    }
                    return last;
                }
                return null;
            }));

            RegisterHelper("count", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return 0;
                }
                if (arguments[0] is System.Collections.ICollection collection)
                {
                    return collection.Count;
                }
                if (arguments[0] is System.Collections.IEnumerable enumerable)
                {
                    int count = 0;
                    foreach (var item in enumerable)
                    {
                        count++;
                    }
                    return count;
                }
                return 0;
            }));

            // Format helpers
            RegisterHelper("format", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return string.Empty;
                }
                try
                {
                    string format = arguments[1].ToString();
                    return string.Format(format, arguments[0]);
                }
                catch
                {
                    return arguments[0]?.ToString() ?? string.Empty;
                }
            }));

            RegisterHelper("join", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2)
                {
                    return string.Empty;
                }
                if (arguments[0] is System.Collections.IEnumerable enumerable)
                {
                    string separator = arguments[1].ToString();
                    var items = new List<string>();
                    foreach (var item in enumerable)
                    {
                        items.Add(item?.ToString() ?? string.Empty);
                    }
                    return string.Join(separator, items);
                }
                return arguments[0]?.ToString() ?? string.Empty;
            }));

            // Block helpers
            RegisterBlockHelper("each", (HandlebarsNet.HandlebarsBlockHelper)((context, options, blockTemplate, inverseTemplate) =>
            {
                if (context == null)
                {
                    return string.Empty;
                }

                var result = string.Empty;
                if (context is System.Collections.IEnumerable enumerable)
                {
                    var enumerator = enumerable.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        int index = 0;
                        do
                        {
                            options["index"] = index;
                            options["first"] = index == 0;
                            options["last"] = !enumerator.MoveNext();
                            result += blockTemplate(enumerator.Current, options);
                            index++;
                        } while (enumerator.MoveNext());
                    }
                    else
                    {
                        result = inverseTemplate(context, options);
                    }
                }
                else
                {
                    result = blockTemplate(context, options);
                }

                return result;
            }));

            RegisterBlockHelper("with", (HandlebarsNet.HandlebarsBlockHelper)((context, options, blockTemplate, inverseTemplate) =>
            {
                if (context == null)
                {
                    return inverseTemplate(context, options);
                }
                return blockTemplate(context, options);
            }));

            RegisterBlockHelper("if", (HandlebarsNet.HandlebarsBlockHelper)((context, options, blockTemplate, inverseTemplate) =>
            {
                if (IsTruthy(context))
                {
                    return blockTemplate(context, options);
                }
                return inverseTemplate(context, options);
            }));

            RegisterBlockHelper("unless", (HandlebarsNet.HandlebarsBlockHelper)((context, options, blockTemplate, inverseTemplate) =>
            {
                if (!IsTruthy(context))
                {
                    return blockTemplate(context, options);
                }
                return inverseTemplate(context, options);
            }));
        }

        /// <summary>
        /// Registers schema helper functions.
        /// </summary>
        /// <param name="schemaHelper">The schema helper.</param>
        private void RegisterSchemaHelper(ISchemaHelper schemaHelper)
        {
            // Register schema helpers
            RegisterHelper("getPrimaryKeyColumns", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                return schemaHelper.GetPrimaryKeyColumns(arguments[0]);
            }));

            RegisterHelper("getNonPrimaryKeyColumns", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                return schemaHelper.GetNonPrimaryKeyColumns(arguments[0]);
            }));

            RegisterHelper("getForeignKeyColumns", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                return schemaHelper.GetForeignKeyColumns(arguments[0]);
            }));

            RegisterHelper("getNonForeignKeyColumns", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                return schemaHelper.GetNonForeignKeyColumns(arguments[0]);
            }));

            RegisterHelper("getColumnDbType", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return schemaHelper.GetColumnDbType(arguments[0]);
            }));

            RegisterHelper("getColumnClrType", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                string language = arguments.Length > 1 ? arguments[1]?.ToString() : "csharp";
                return schemaHelper.GetColumnClrType(arguments[0], language);
            }));
        }

        /// <summary>
        /// Registers relationship helper functions.
        /// </summary>
        /// <param name="relationshipHelper">The relationship helper.</param>
        private void RegisterRelationshipHelper(IRelationshipHelper relationshipHelper)
        {
            // Register relationship helpers
            RegisterHelper("getRelationships", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                
                if (arguments.Length > 1 && arguments[1] != null)
                {
                    return relationshipHelper.GetRelationships(arguments[0], arguments[1].ToString());
                }
                
                return relationshipHelper.GetRelationships(arguments[0]);
            }));

            RegisterHelper("getNavigationPropertyName", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2 || arguments[0] == null || arguments[1] == null)
                {
                    return string.Empty;
                }
                return relationshipHelper.GetNavigationPropertyName(arguments[0], arguments[1]);
            }));

            RegisterHelper("isCollectionNavigationProperty", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length < 2 || arguments[0] == null || arguments[1] == null)
                {
                    return false;
                }
                return relationshipHelper.IsCollectionNavigationProperty(arguments[0], arguments[1]);
            }));

            RegisterHelper("getPayloadProperties", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return null;
                }
                return relationshipHelper.GetPayloadProperties(arguments[0]);
            }));
        }

        /// <summary>
        /// Registers code format helper functions.
        /// </summary>
        /// <param name="codeFormatHelper">The code format helper.</param>
        private void RegisterCodeFormatHelper(ICodeFormatHelper codeFormatHelper)
        {
            // Register code format helpers
            RegisterHelper("toPascalCase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.ToPascalCase(arguments[0].ToString());
            }));

            RegisterHelper("toCamelCase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.ToCamelCase(arguments[0].ToString());
            }));

            RegisterHelper("toSnakeCase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.ToSnakeCase(arguments[0].ToString());
            }));

            RegisterHelper("toKebabCase", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.ToKebabCase(arguments[0].ToString());
            }));

            RegisterHelper("pluralize", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.Pluralize(arguments[0].ToString());
            }));

            RegisterHelper("singularize", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                return codeFormatHelper.Singularize(arguments[0].ToString());
            }));

            RegisterHelper("indent", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                
                int spaces = 4;
                if (arguments.Length > 1 && TryConvertToInt(arguments[1], out int parsedSpaces))
                {
                    spaces = parsedSpaces;
                }
                
                return codeFormatHelper.Indent(arguments[0].ToString(), spaces);
            }));

            RegisterHelper("formatComment", (HandlebarsNet.HandlebarsHelper)((context, arguments) =>
            {
                if (arguments.Length == 0 || arguments[0] == null)
                {
                    return string.Empty;
                }
                
                string language = "csharp";
                if (arguments.Length > 1 && arguments[1] != null)
                {
                    language = arguments[1].ToString();
                }
                
                return codeFormatHelper.FormatComment(arguments[0].ToString(), language);
            }));
        }

        /// <summary>
        /// Determines if an object is "truthy".
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>True if the object is "truthy"; otherwise, false.</returns>
        private bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool b)
            {
                return b;
            }

            if (value is string s)
            {
                return !string.IsNullOrEmpty(s);
            }

            if (value is System.Collections.ICollection collection)
            {
                return collection.Count > 0;
            }

            if (value is System.Collections.IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                return enumerator.MoveNext();
            }

            if (value is int i)
            {
                return i != 0;
            }

            if (value is double d)
            {
                return d != 0;
            }

            if (value is decimal dec)
            {
                return dec != 0;
            }

            return true;
        }

        /// <summary>
        /// Tries to convert an object to a double.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="result">The converted double value.</param>
        /// <returns>True if the conversion succeeded; otherwise, false.</returns>
        private bool TryConvertToDouble(object value, out double result)
        {
            result = 0;
            if (value == null)
            {
                return false;
            }

            if (value is double d)
            {
                result = d;
                return true;
            }

            if (value is int i)
            {
                result = i;
                return true;
            }

            if (value is decimal dec)
            {
                result = (double)dec;
                return true;
            }

            if (value is string s)
            {
                return double.TryParse(s, out result);
            }

            return false;
        }

        /// <summary>
        /// Tries to convert an object to an integer.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="result">The converted integer value.</param>
        /// <returns>True if the conversion succeeded; otherwise, false.</returns>
        private bool TryConvertToInt(object value, out int result)
        {
            result = 0;
            if (value == null)
            {
                return false;
            }

            if (value is int i)
            {
                result = i;
                return true;
            }

            if (value is double d)
            {
                result = (int)d;
                return true;
            }

            if (value is decimal dec)
            {
                result = (int)dec;
                return true;
            }

            if (value is string s)
            {
                return int.TryParse(s, out result);
            }

            return false;
        }

        /// <summary>
        /// Compares two objects.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        private int Compare(object a, object b)
        {
            if (a == null && b == null)
            {
                return 0;
            }

            if (a == null)
            {
                return -1;
            }

            if (b == null)
            {
                return 1;
            }

            if (a is IComparable comparableA)
            {
                return comparableA.CompareTo(b);
            }

            if (TryConvertToDouble(a, out double aDouble) && TryConvertToDouble(b, out double bDouble))
            {
                return aDouble.CompareTo(bDouble);
            }

            return a.ToString().CompareTo(b.ToString());
        }
    }
}
