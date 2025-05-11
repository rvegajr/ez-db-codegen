using System;
using System.IO;
using System.Text;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;

#nullable enable

namespace EzDbCodeGen.TemplateEngine.Helpers
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
            templateEngine.RegisterHelper("xmldoc", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var text = parameters[0]?.ToString() ?? string.Empty;
                writer.Write(GenerateXmlDocComment(text));
            });
            
            templateEngine.RegisterHelper("xmldocProperty", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateXmlPropertyComment(name, description));
            });
            
            templateEngine.RegisterHelper("xmldocClass", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateXmlClassComment(name, description));
            });
            
            templateEngine.RegisterHelper("xmldocMethod", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = parameters.Length >= 3 ? (parameters[2]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateXmlMethodComment(name, description, returnDesc));
            });
            
            // JSDoc documentation comment helpers
            templateEngine.RegisterHelper("jsdoc", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var text = parameters[0]?.ToString() ?? string.Empty;
                writer.Write(GenerateJsDocComment(text));
            });
            
            templateEngine.RegisterHelper("jsdocProperty", (writer, context, parameters) => {
                if (parameters.Length < 2) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var type = parameters[1]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 3 ? (parameters[2]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateJsDocPropertyComment(name, type, description));
            });
            
            templateEngine.RegisterHelper("jsdocClass", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateJsDocClassComment(name, description));
            });
            
            templateEngine.RegisterHelper("jsdocMethod", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnType = parameters.Length >= 3 ? (parameters[2]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = parameters.Length >= 4 ? (parameters[3]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GenerateJsDocMethodComment(name, description, returnType, returnDesc));
            });
            
            // Python docstring helpers
            templateEngine.RegisterHelper("pydoc", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var text = parameters[0]?.ToString() ?? string.Empty;
                writer.Write(GeneratePythonDocstring(text));
            });
            
            templateEngine.RegisterHelper("pydocClass", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GeneratePythonClassDocstring(name, description));
            });
            
            templateEngine.RegisterHelper("pydocMethod", (writer, context, parameters) => {
                if (parameters.Length < 1) return;
                
                var name = parameters[0]?.ToString() ?? string.Empty;
                var description = parameters.Length >= 2 ? (parameters[1]?.ToString() ?? string.Empty) : string.Empty;
                var returnType = parameters.Length >= 3 ? (parameters[2]?.ToString() ?? string.Empty) : string.Empty;
                var returnDesc = parameters.Length >= 4 ? (parameters[3]?.ToString() ?? string.Empty) : string.Empty;
                
                writer.Write(GeneratePythonMethodDocstring(name, description, returnType, returnDesc));
            });
        }

        /// <summary>
        /// Generates an XML documentation comment.
        /// </summary>
        /// <param name="text">The text to include in the comment.</param>
        /// <returns>The generated XML documentation comment.</returns>
        public string GenerateXmlDocComment(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {text}");
            sb.AppendLine("/// </summary>");
            
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Generates an XML documentation comment for a property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="description">The property description.</param>
        /// <returns>The generated XML documentation comment.</returns>
        public string GenerateXmlPropertyComment(string name, string description)
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

        /// <summary>
        /// Generates an XML documentation comment for a class.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <param name="description">The class description.</param>
        /// <returns>The generated XML documentation comment.</returns>
        public string GenerateXmlClassComment(string name, string description)
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

        /// <summary>
        /// Generates an XML documentation comment for a method.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="description">The method description.</param>
        /// <param name="returnDesc">The return value description.</param>
        /// <returns>The generated XML documentation comment.</returns>
        public string GenerateXmlMethodComment(string name, string description, string returnDesc)
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

        /// <summary>
        /// Generates a JSDoc comment.
        /// </summary>
        /// <param name="text">The text to include in the comment.</param>
        /// <returns>The generated JSDoc comment.</returns>
        public string GenerateJsDocComment(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            sb.AppendLine($" * {text}");
            sb.AppendLine(" */");
            
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Generates a JSDoc comment for a property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="description">The property description.</param>
        /// <returns>The generated JSDoc comment.</returns>
        public string GenerateJsDocPropertyComment(string name, string type, string description)
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

        /// <summary>
        /// Generates a JSDoc comment for a class.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <param name="description">The class description.</param>
        /// <returns>The generated JSDoc comment.</returns>
        public string GenerateJsDocClassComment(string name, string description)
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

        /// <summary>
        /// Generates a JSDoc comment for a method.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="description">The method description.</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="returnDesc">The return value description.</param>
        /// <returns>The generated JSDoc comment.</returns>
        public string GenerateJsDocMethodComment(string name, string description, string returnType, string returnDesc)
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

        /// <summary>
        /// Generates a Python docstring.
        /// </summary>
        /// <param name="text">The text to include in the docstring.</param>
        /// <returns>The generated Python docstring.</returns>
        public string GeneratePythonDocstring(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            
            var sb = new StringBuilder();
            sb.AppendLine("\"\"\"");
            sb.AppendLine(text);
            sb.AppendLine("\"\"\"");
            
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Generates a Python docstring for a class.
        /// </summary>
        /// <param name="name">The class name.</param>
        /// <param name="description">The class description.</param>
        /// <returns>The generated Python docstring.</returns>
        public string GeneratePythonClassDocstring(string name, string description)
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

        /// <summary>
        /// Generates a Python docstring for a method.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="description">The method description.</param>
        /// <param name="returnType">The return type.</param>
        /// <param name="returnDesc">The return value description.</param>
        /// <returns>The generated Python docstring.</returns>
        public string GeneratePythonMethodDocstring(string name, string description, string returnType, string returnDesc)
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
