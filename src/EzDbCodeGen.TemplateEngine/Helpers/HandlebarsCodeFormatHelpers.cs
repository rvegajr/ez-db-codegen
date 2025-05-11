using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;
using HandlebarsDotNet.IO;

#nullable enable

namespace EzDbCodeGen.TemplateEngine.Helpers
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
            templateEngine.RegisterBlockHelper("csharpFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatCSharp(code));
            });
            
            templateEngine.RegisterHelper("csharpProperty", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2) return;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var hasGetter = arguments.Length < 4 || Convert.ToBoolean(arguments[3] ?? true);
                var hasSetter = arguments.Length < 5 || Convert.ToBoolean(arguments[4] ?? true);
                
                writer.WriteSafeString(GenerateCSharpProperty(type, name, accessModifier, hasGetter, hasSetter));
            });
            
            // Register SQL code format helpers
            templateEngine.RegisterBlockHelper("sqlFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatSql(code));
            });
            
            // Register TypeScript code format helpers
            templateEngine.RegisterBlockHelper("typescriptFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatTypeScript(code));
            });
            
            templateEngine.RegisterHelper("typescriptProperty", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2) return;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var isReadonly = arguments.Length >= 4 && Convert.ToBoolean(arguments[3] ?? false);
                var isOptional = arguments.Length >= 5 && Convert.ToBoolean(arguments[4] ?? false);
                
                writer.WriteSafeString(GenerateTypeScriptProperty(type, name, accessModifier, isReadonly, isOptional));
            });
            
            // Register JavaScript code format helpers
            templateEngine.RegisterBlockHelper("javascriptFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatJavaScript(code));
            });
            
            // Register HTML code format helpers
            templateEngine.RegisterBlockHelper("htmlFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatHtml(code));
            });
            
            // Register CSS code format helpers
            templateEngine.RegisterBlockHelper("cssFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatCss(code));
            });
            
            // Register Java code format helpers
            templateEngine.RegisterBlockHelper("javaFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatJava(code));
            });
            
            templateEngine.RegisterHelper("javaProperty", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2) return;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var name = arguments[1]?.ToString() ?? string.Empty;
                
                var accessModifier = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? "public") : "public";
                var isFinal = arguments.Length >= 4 && Convert.ToBoolean(arguments[3] ?? false);
                var isStatic = arguments.Length >= 5 && Convert.ToBoolean(arguments[4] ?? false);
                
                writer.WriteSafeString(GenerateJavaProperty(type, name, accessModifier, isFinal, isStatic));
            });
            
            // Register Python code format helpers
            templateEngine.RegisterBlockHelper("pythonFormat", (EncodedTextWriter writer, BlockHelperOptions options, Context context, Arguments arguments) => {
                using var stringWriter = new EncodedTextWriter();
                options.Template(stringWriter, context);
                var code = stringWriter.ToString();
                writer.WriteSafeString(FormatPython(code));
            });
            
            // Register string escape helpers
            templateEngine.RegisterHelper("escapeCSharp", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1) return;
                
                var value = arguments[0]?.ToString() ?? string.Empty;
                writer.WriteSafeString(EscapeCSharpString(value));
            });
            
            templateEngine.RegisterHelper("escapeSql", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1) return;
                
                var value = arguments[0]?.ToString() ?? string.Empty;
                writer.WriteSafeString(EscapeSqlString(value));
            });
            
            templateEngine.RegisterHelper("escapeJs", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1) return;
                
                var value = arguments[0]?.ToString() ?? string.Empty;
                writer.WriteSafeString(EscapeJsString(value));
            });
            
            templateEngine.RegisterHelper("escapeHtml", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1) return;
                
                var value = arguments[0]?.ToString() ?? string.Empty;
                writer.WriteSafeString(EscapeHtmlString(value));
            });
            
            // Register code format by language helper
            templateEngine.RegisterHelper("formatCode", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2) return;
                
                var code = arguments[0]?.ToString() ?? string.Empty;
                var language = arguments[1]?.ToString() ?? string.Empty;
                
                writer.WriteSafeString(FormatCodeByLanguage(code, language));
            });
        }

        /// <inheritdoc/>
        public string FormatCSharp(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Adjust indent level based on closing braces at the start of the line
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
                
                // Adjust indent level based on opening braces at the end of the line
                if (trimmedLine.EndsWith("{") || trimmedLine.EndsWith("("))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        /// <inheritdoc/>
        public string GenerateCSharpProperty(string type, string name, string accessModifier = "public", bool hasGetter = true, bool hasSetter = true)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add XML documentation
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// Gets or sets the {name}.");
            sb.AppendLine("/// </summary>");
            
            // Build the property declaration
            sb.Append($"{accessModifier} {type} {name} {{ ");
            
            if (hasGetter)
            {
                sb.Append("get; ");
            }
            
            if (hasSetter)
            {
                sb.Append("set; ");
            }
            
            sb.Append("}");
            
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string FormatSql(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple SQL formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            // Keywords to uppercase
            var keywords = new[] { 
                "SELECT", "FROM", "WHERE", "JOIN", "LEFT", "RIGHT", "INNER", "OUTER", 
                "GROUP", "ORDER", "BY", "HAVING", "INSERT", "UPDATE", "DELETE", "CREATE", 
                "ALTER", "DROP", "TABLE", "VIEW", "PROCEDURE", "FUNCTION", "INDEX", "TRIGGER" 
            };
            
            foreach (var line in lines)
            {
                var formattedLine = line;
                
                // Uppercase SQL keywords
                foreach (var keyword in keywords)
                {
                    formattedLine = Regex.Replace(
                        formattedLine, 
                        $@"\b{keyword}\b", 
                        keyword, 
                        RegexOptions.IgnoreCase
                    );
                }
                
                sb.AppendLine(formattedLine);
            }
            
            return sb.ToString().TrimEnd();
        }

        /// <inheritdoc/>
        public string FormatTypeScript(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharp(code); // Reuse the basic formatting logic
        }

        /// <inheritdoc/>
        public string GenerateTypeScriptProperty(string type, string name, string accessModifier = "public", bool isReadonly = false, bool isOptional = false)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add property declaration
            sb.Append($"{accessModifier} ");
            
            if (isReadonly)
            {
                sb.Append("readonly ");
            }
            
            sb.Append(name);
            
            if (isOptional)
            {
                sb.Append("?");
            }
            
            sb.Append($": {type};");
            
            return sb.ToString();
        }

        /// <inheritdoc/>
        public string FormatJavaScript(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharp(code); // Reuse the basic formatting logic
        }

        /// <inheritdoc/>
        public string FormatHtml(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple HTML formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Check for closing tags that decrease indentation
                if (Regex.IsMatch(trimmedLine, @"^</[^>]+>"))
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
                
                // Increase indent level for next line if this one contains an opening tag without a closing tag
                if (Regex.IsMatch(trimmedLine, @"<[^/][^>]*>") && 
                    !Regex.IsMatch(trimmedLine, @"<[^/][^>]*/>") && 
                    !Regex.IsMatch(trimmedLine, @"<[^/][^>]*>[^<]*</[^>]+>"))
                {
                    indentLevel++;
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        /// <inheritdoc/>
        public string FormatCss(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // Simple CSS formatting
            var sb = new StringBuilder();
            var lines = code.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var indentLevel = 0;
            
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Check for closing braces that decrease indentation
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

        /// <inheritdoc/>
        public string FormatJava(string code)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            // For this demo, we'll just apply basic indentation
            return FormatCSharp(code); // Reuse the basic formatting logic
        }

        /// <inheritdoc/>
        public string GenerateJavaProperty(string type, string name, string accessModifier = "private", bool isFinal = false, bool isStatic = false)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            
            // Add the field declaration
            sb.AppendLine($"{accessModifier} {(isFinal ? "final " : "")}{(isStatic ? "static " : "")}{type} {name};");
            
            return sb.ToString().TrimEnd();
        }

        /// <inheritdoc/>
        public string FormatPython(string code)
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
        
        /// <inheritdoc/>
        public string EscapeCSharpString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
        
        /// <inheritdoc/>
        public string EscapeSqlString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            
            // In SQL, single quotes are escaped by doubling them
            return value.Replace("'", "''");
        }
        
        /// <inheritdoc/>
        public string EscapeJsString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            
            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("'", "\\'")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
        
        /// <inheritdoc/>
        public string EscapeHtmlString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }
        
        /// <inheritdoc/>
        public string FormatCodeByLanguage(string code, string language)
        {
            if (string.IsNullOrEmpty(code)) return string.Empty;
            
            switch (language.ToLowerInvariant())
            {
                case "csharp":
                case "c#":
                    return FormatCSharp(code);
                case "sql":
                    return FormatSql(code);
                case "typescript":
                case "ts":
                    return FormatTypeScript(code);
                case "javascript":
                case "js":
                    return FormatJavaScript(code);
                case "html":
                    return FormatHtml(code);
                case "css":
                    return FormatCss(code);
                case "java":
                    return FormatJava(code);
                case "python":
                case "py":
                    return FormatPython(code);
                default:
                    return code; // Return as-is if language not supported
            }
        }
        
        // Keep the old method names for backward compatibility with existing code
        public string FormatCSharpCode(string code) => FormatCSharp(code);
        public string FormatSqlCode(string code) => FormatSql(code);
        public string FormatTypeScriptCode(string code) => FormatTypeScript(code);
        public string FormatJavaScriptCode(string code) => FormatJavaScript(code);
        public string FormatHtmlCode(string code) => FormatHtml(code);
        public string FormatCssCode(string code) => FormatCss(code);
        public string FormatJavaCode(string code) => FormatJava(code);
        public string FormatPythonCode(string code) => FormatPython(code);
    }
}
