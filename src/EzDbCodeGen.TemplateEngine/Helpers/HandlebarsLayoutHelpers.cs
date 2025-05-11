using System;
using System.Collections.Generic;
using System.Text;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;

#nullable enable

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides layout helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsLayoutHelpers : ILayoutHelpers, IHelperRegistration
    {
        private readonly Dictionary<string, string> _sections = new();
        private readonly List<string> _sectionOrder = new();

        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register section helper
            templateEngine.RegisterHelper("section", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2)
                {
                    return;
                }
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                var content = arguments[1]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName))
                {
                    return;
                }
                
                // Store the section content
                _sections[sectionName] = content;
                
                // If this is a new section, add it to the order list
                if (!_sectionOrder.Contains(sectionName))
                {
                    _sectionOrder.Add(sectionName);
                }
            });

            // Register render section helper
            templateEngine.RegisterHelper("renderSection", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1)
                {
                    return;
                }
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName))
                {
                    return;
                }
                
                // Check if the section exists
                if (!_sections.TryGetValue(sectionName, out var content))
                {
                    return;
                }
                
                writer.Write(content);
            });

            // Register layout helper
            templateEngine.RegisterHelper("layout", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 2)
                {
                    return;
                }
                
                var layoutName = arguments[0]?.ToString() ?? string.Empty;
                var bodyContent = arguments[1]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(layoutName))
                {
                    return;
                }
                
                // Store the body content
                _sections["body"] = bodyContent;
                
                // Add body to the section order if it's not already there
                if (!_sectionOrder.Contains("body"))
                {
                    _sectionOrder.Add("body");
                }
                
                // The layout will be processed later, when the full template is processed
                writer.Write($"LAYOUT:{layoutName}");
            });

            // Register render body helper
            templateEngine.RegisterHelper("renderBody", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                // Check if the body section exists
                if (_sections.TryGetValue("body", out var content))
                {
                    writer.Write(content);
                }
            });

            // Register clear sections helper
            templateEngine.RegisterHelper("clearSections", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                _sections.Clear();
                _sectionOrder.Clear();
            });

            // Register render all sections helper
            templateEngine.RegisterHelper("renderAllSections", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                // Render sections in the order they were defined
                foreach (var sectionName in _sectionOrder)
                {
                    if (_sections.TryGetValue(sectionName, out var content))
                    {
                        writer.Write(content);
                    }
                }
            });

            // Register has section helper
            templateEngine.RegisterHelper("hasSection", (EncodedTextWriter writer, Context context, Arguments arguments) => {
                if (arguments.Length < 1)
                {
                    writer.Write("false");
                    return;
                }
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName))
                {
                    writer.Write("false");
                    return;
                }
                
                writer.Write(_sections.ContainsKey(sectionName) ? "true" : "false");
            });
        }
        
        /// <inheritdoc/>
        public string DefineSection(string name, string content)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            
            // Store the section content
            _sections[name] = content;
            
            // If this is a new section, add it to the order list
            if (!_sectionOrder.Contains(name))
            {
                _sectionOrder.Add(name);
            }
            
            return content;
        }
        
        /// <inheritdoc/>
        public string RenderSection(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            
            // Check if the section exists
            if (!_sections.TryGetValue(name, out var content))
            {
                return string.Empty;
            }
            
            return content;
        }
        
        /// <inheritdoc/>
        public string RenderBody()
        {
            // Check if the body section exists
            if (_sections.TryGetValue("body", out var content))
            {
                return content;
            }
            
            return string.Empty;
        }
    }
}
