using System;
using System.Text;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides documentation helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsDocumentationHelpers : IDocumentationHelpers, IHelperRegistration
    {
        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // XML documentation comment helpers
            templateEngine.RegisterHelper("xmldoc", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                return GenerateXmlDocComment(text);
            });
            
            templateEngine.RegisterHelper("xmldocProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateXmlPropertyComment(name, description);
            });
            
            templateEngine.RegisterHelper("xmldocClass", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateXmlClassComment(name, description);
            });
            
            templateEngine.RegisterHelper("xmldocMethod", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateXmlMethodComment(name, description, returnDesc);
            });
            
            // JSDoc documentation comment helpers
            templateEngine.RegisterHelper("jsdoc", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                return GenerateJsDocComment(text);
            });
            
            templateEngine.RegisterHelper("jsdocProperty", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var type = arguments[1]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateJsDocPropertyComment(name, type, description);
            });
            
            templateEngine.RegisterHelper("jsdocClass", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateJsDocClassComment(name, description);
            });
            
            templateEngine.RegisterHelper("jsdocMethod", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnType = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = arguments.Length >= 4 ? (arguments[3]?.ToString() ?? string.Empty) : string.Empty;
                
                return GenerateJsDocMethodComment(name, description, returnType, returnDesc);
            });
            
            // Python docstring helpers
            templateEngine.RegisterHelper("pydoc", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var text = arguments[0]?.ToString() ?? string.Empty;
                return GeneratePythonDocstring(text);
            });
            
            templateEngine.RegisterHelper("pydocClass", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                
                return GeneratePythonClassDocstring(name, description);
            });
            
            templateEngine.RegisterHelper("pydocMethod", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var name = arguments[0]?.ToString() ?? string.Empty;
                var description = arguments.Length >= 2 ? (arguments[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnType = arguments.Length >= 3 ? (arguments[2]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = arguments.Length >= 4 ? (arguments[3]?.ToString() ?? string.Empty) : string.Empty;
                
                return GeneratePythonMethodDocstring(name, description, returnType, returnDesc);
            });
        }

        private string GenerateXmlDocComment(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {text}");
            sb.AppendLine("/// </summary>");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateXmlPropertyComment(string name, string description)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"/// {description}");
            }
            else
            {
                sb.AppendLine($"/// Gets or sets the {name.ToLowerInvariant()}.");
            }
            
            sb.AppendLine("/// </summary>");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateXmlClassComment(string name, string description)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"/// {description}");
            }
            else
            {
                sb.AppendLine($"/// Represents a {name.ToLowerInvariant()}.");
            }
            
            sb.AppendLine("/// </summary>");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateXmlMethodComment(string name, string description, string returnDesc)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($"/// {description}");
            }
            else
            {
                sb.AppendLine($"/// Performs the {name.ToLowerInvariant()} operation.");
            }
            
            sb.AppendLine("/// </summary>");
            
            if (!string.IsNullOrEmpty(returnDesc))
            {
                sb.AppendLine("/// <returns>");
                sb.AppendLine($"/// {returnDesc}");
                sb.AppendLine("/// </returns>");
            }
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateJsDocComment(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            sb.AppendLine($" * {text}");
            sb.AppendLine(" */");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateJsDocPropertyComment(string name, string type, string description)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($" * {description}");
            }
            else
            {
                sb.AppendLine($" * The {name.ToLowerInvariant()} property.");
            }
            
            sb.AppendLine($" * @type {{{type}}}");
            sb.AppendLine(" */");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateJsDocClassComment(string name, string description)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($" * {description}");
            }
            else
            {
                sb.AppendLine($" * Represents a {name.ToLowerInvariant()}.");
            }
            
            sb.AppendLine($" * @class {name}");
            sb.AppendLine(" */");
            
            return sb.ToString().TrimEnd();
        }

        private string GenerateJsDocMethodComment(string name, string description, string returnType, string returnDesc)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine($" * {description}");
            }
            else
            {
                sb.AppendLine($" * Performs the {name.ToLowerInvariant()} operation.");
            }
            
            if (!string.IsNullOrEmpty(returnType))
            {
                if (!string.IsNullOrEmpty(returnDesc))
                {
                    sb.AppendLine($" * @returns {{{returnType}}} {returnDesc}");
                }
                else
                {
                    sb.AppendLine($" * @returns {{{returnType}}}");
                }
            }
            
            sb.AppendLine(" */");
            
            return sb.ToString().TrimEnd();
        }

        private string GeneratePythonDocstring(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("\"\"\"");
            sb.AppendLine(text);
            sb.AppendLine("\"\"\"");
            
            return sb.ToString().TrimEnd();
        }

        private string GeneratePythonClassDocstring(string name, string description)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("\"\"\"");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine(description);
            }
            else
            {
                sb.AppendLine($"Represents a {name.ToLowerInvariant()}.");
            }
            
            sb.AppendLine("\"\"\"");
            
            return sb.ToString().TrimEnd();
        }

        private string GeneratePythonMethodDocstring(string name, string description, string returnType, string returnDesc)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("\"\"\"");
            
            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendLine(description);
            }
            else
            {
                sb.AppendLine($"Performs the {name.ToLowerInvariant()} operation.");
            }
            
            if (!string.IsNullOrEmpty(returnType) || !string.IsNullOrEmpty(returnDesc))
            {
                sb.AppendLine();
                sb.Append("Returns");
                sb.AppendLine("-------");
                
                if (!string.IsNullOrEmpty(returnType))
                {
                    sb.Append(returnType);
                }
                
                if (!string.IsNullOrEmpty(returnDesc))
                {
                    if (!string.IsNullOrEmpty(returnType))
                    {
                        sb.Append(": ");
                    }
                    
                    sb.AppendLine(returnDesc);
                }
                else if (!string.IsNullOrEmpty(returnType))
                {
                    sb.AppendLine();
                }
            }
            
            sb.AppendLine("\"\"\"");
            
            return sb.ToString().TrimEnd();
        }
    }
}
