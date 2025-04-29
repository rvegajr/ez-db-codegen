using System;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides code formatting helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsCodeFormatHelpers : ICodeFormatHelpers, IHelperRegistration
    {
        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register C# code format helpers
            templateEngine.RegisterHelper("csharpFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatCSharpCode(code);
            });
            
            templateEngine.RegisterHelper("csharpProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var hasGetter = arguments.Length < 4 || Convert.ToBoolean(arguments[3] ?? true);
                var hasSetter = arguments.Length < 5 || Convert.ToBoolean(arguments[4] ?? true);
                
                return GenerateCSharpProperty(type, name, accessModifier, hasGetter, hasSetter);
            });
            
            // Register SQL code format helpers
            templateEngine.RegisterHelper("sqlFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatSqlCode(code);
            });
            
            // Register TypeScript code format helpers
            templateEngine.RegisterHelper("typescriptFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatTypeScriptCode(code);
            });
            
            templateEngine.RegisterHelper("typescriptProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var isReadonly = arguments.Length >= 4 && Convert.ToBoolean(arguments[3] ?? false);
                var isOptional = arguments.Length >= 5 && Convert.ToBoolean(arguments[4] ?? false);
                
                return GenerateTypeScriptProperty(type, name, accessModifier, isReadonly, isOptional);
            });
            
            // Register JavaScript code format helpers
            templateEngine.RegisterHelper("javascriptFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatJavaScriptCode(code);
            });
            
            // Register HTML code format helpers
            templateEngine.RegisterHelper("htmlFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatHtmlCode(code);
            });
            
            // Register CSS code format helpers
            templateEngine.RegisterHelper("cssFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatCssCode(code);
            });
            
            // Register Java code format helpers
            templateEngine.RegisterHelper("javaFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatJavaCode(code);
            });
            
            templateEngine.RegisterHelper("javaProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "private") : "private";
                var generateGetter = arguments.Length < 4 || Convert.ToBoolean(arguments[3] ?? true);
                var generateSetter = arguments.Length < 5 || Convert.ToBoolean(arguments[4] ?? true);
                
                return GenerateJavaProperty(type, name, accessModifier, generateGetter, generateSetter);
            });
            
            // Register Python code format helpers
            templateEngine.RegisterHelper("pythonFormat", (context, options, arguments, blockParams) => {
                if (options == null) return string.Empty;
                
                var code = options.Fn(context);
                return FormatPythonCode(code);
            });
        }

        private string FormatCSharpCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple formatting rules for demonstration - in a real implementation,
            // you'd want to use a proper code formatter like Roslyn
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Adjust indent level based on braces
                if (trimmedLine.StartsWith("}") || trimmedLine.StartsWith(")"))
                {
                    indentLevel = Math.Max(0, indentLevel - 1);
                }
                
                // Add the line with the correct indentation
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    sb.AppendLine($"{new string(' ', indentLevel * 4)}{trimmedLine}");
                }
                else
                {
                    sb.AppendLine();
                }
                
                // Increase indent level for next line if this one opens a block
                if (trimmedLine.EndsWith("{") || trimmedLine.EndsWith("("))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateCSharpProperty(string type, string name, string accessModifier, bool hasGetter, bool hasSetter)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add the property declaration
            if (hasGetter && hasSetter)
            {
                sb.AppendLine($"{accessModifier} {type} {name} {{ get; set; }}");
            }
            else if (hasGetter)
            {
                sb.AppendLine($"{accessModifier} {type} {name} {{ get; }}");
            }
            else if (hasSetter)
            {
                sb.AppendLine($"{accessModifier} {type} {name} {{ set; }}");
            }
            else
            {
                sb.AppendLine($"{accessModifier} {type} {name};");
            }
            
            return sb.ToString().TrimEnd();
        }

        private string FormatSqlCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple SQL formatting - just capitalize keywords
            var keywords = new[] { 
                "SELECT", "FROM", "WHERE", "JOIN", "INNER JOIN", "LEFT JOIN", "RIGHT JOIN", 
                "ORDER BY", "GROUP BY", "HAVING", "INSERT", "UPDATE", "DELETE", "CREATE", "DROP", 
                "ALTER", "TABLE", "VIEW", "PROCEDURE", "FUNCTION", "INDEX", "TRIGGER", "AS", 
                "ON", "AND", "OR", "NOT", "IN", "BETWEEN", "LIKE", "IS NULL", "IS NOT NULL" 
            };
            
            var formattedCode = code;
            
            foreach (var keyword in keywords)
            {
                var pattern = $@"\b{keyword}\b";
                formattedCode = Regex.Replace(
                    formattedCode, 
                    pattern, 
                    keyword.ToUpperInvariant(), 
                    RegexOptions.IgnoreCase);
            }
            
            return formattedCode;
        }

        private string FormatTypeScriptCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharpCode(code); // Reuse the basic formatting logic
        }

        private string GenerateTypeScriptProperty(string type, string name, string accessModifier, bool isReadonly, bool isOptional)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add the property declaration
            var readonlyPrefix = isReadonly ? "readonly " : "";
            var optionalSuffix = isOptional ? "?" : "";
            
            sb.AppendLine($"{accessModifier} {readonlyPrefix}{name}{optionalSuffix}: {type};");
            
            return sb.ToString().TrimEnd();
        }

        private string FormatJavaScriptCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharpCode(code); // Reuse the basic formatting logic
        }

        private string FormatHtmlCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple HTML formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Decrease indent for closing tags
                if (trimmedLine.StartsWith("</"))
                {
                    indentLevel = Math.Max(0, indentLevel - 1);
                }
                
                // Add the line with the correct indentation
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    sb.AppendLine($"{new string(' ', indentLevel * 2)}{trimmedLine}");
                }
                else
                {
                    sb.AppendLine();
                }
                
                // Increase indent for opening tags that aren't self-closing
                if (trimmedLine.StartsWith("<") && !trimmedLine.StartsWith("</") && !trimmedLine.EndsWith("/>") && !trimmedLine.EndsWith("</"))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        private string FormatCssCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple CSS formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Adjust indent level based on braces
                if (trimmedLine.StartsWith("}"))
                {
                    indentLevel = Math.Max(0, indentLevel - 1);
                }
                
                // Add the line with the correct indentation
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    sb.AppendLine($"{new string(' ', indentLevel * 2)}{trimmedLine}");
                }
                else
                {
                    sb.AppendLine();
                }
                
                // Increase indent level for next line if this one opens a block
                if (trimmedLine.EndsWith("{"))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        private string FormatJavaCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharpCode(code); // Reuse the basic formatting logic
        }

        private string GenerateJavaProperty(string type, string name, string accessModifier, bool generateGetter, bool generateSetter)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add the field declaration
            sb.AppendLine($"{accessModifier} {type} {name};");
            
            // Add getter if requested
            if (generateGetter)
            {
                var getterName = $"get{char.ToUpperInvariant(name[0])}{name.Substring(1)}";
                sb.AppendLine();
                sb.AppendLine($"public {type} {getterName}() {{");
                sb.AppendLine($"    return {name};");
                sb.AppendLine("}");
            }
            
            // Add setter if requested
            if (generateSetter)
            {
                var setterName = $"set{char.ToUpperInvariant(name[0])}{name.Substring(1)}";
                sb.AppendLine();
                sb.AppendLine($"public void {setterName}({type} {name}) {{");
                sb.AppendLine($"    this.{name} = {name};");
                sb.AppendLine("}");
            }
            
            return sb.ToString().TrimEnd();
        }

        private string FormatPythonCode(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple Python formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Check for lines that decrease indentation
                if (indentLevel > 0 && !string.IsNullOrWhiteSpace(trimmedLine) && 
                    !trimmedLine.StartsWith("#") && // Skip comments
                    !trimmedLine.EndsWith(":") &&   // Skip lines ending with colon
                    (lines.Length > Array.IndexOf(lines, line) + 1) && // Ensure there's a next line
                    !string.IsNullOrWhiteSpace(lines[Array.IndexOf(lines, line) + 1]) && // Skip if next line is empty
                    !lines[Array.IndexOf(lines, line) + 1].StartsWith(" ") && // Check if next line is indented
                    !lines[Array.IndexOf(lines, line) + 1].StartsWith("\t"))
                {
                    indentLevel = 0; // Reset indentation if next line isn't indented
                }
                
                // Add the line with the correct indentation
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    sb.AppendLine($"{new string(' ', indentLevel * 4)}{trimmedLine}");
                }
                else
                {
                    sb.AppendLine();
                }
                
                // Increase indent level for next line if this one ends with a colon
                if (trimmedLine.EndsWith(":"))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}
