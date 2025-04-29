using System;
using System.Collections.Generic;
using System.Text;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

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
            templateEngine.RegisterHelper("section", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1 || options == null) return string.Empty;
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(sectionName)) return string.Empty;
                
                // Get the content of the section
                var content = options.Fn(context);
                
                // Store the section content
                _sections[sectionName] = content;
                
                // If this is a new section, add it to the order list
                if (!_sectionOrder.Contains(sectionName))
                {
                    _sectionOrder.Add(sectionName);
                }
                
                return string.Empty; // Sections don't output anything where they're defined
            });

            // Register render section helper
            templateEngine.RegisterHelper("renderSection", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(sectionName)) return string.Empty;
                
                // Check if the section exists
                if (!_sections.TryGetValue(sectionName, out var content))
                {
                    // Return default content if provided
                    return options?.Fn(context) ?? string.Empty;
                }
                
                return content;
            });

            // Register layout helper
            templateEngine.RegisterHelper("layout", (context, options, arguments, blockParams) => {
                if (arguments.Length < 1) return string.Empty;
                
                var layoutName = arguments[0]?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(layoutName)) return string.Empty;
                
                // Store the body content
                if (options != null)
                {
                    _sections["body"] = options.Fn(context);
                    
                    // Add body to the section order if it's not already there
                    if (!_sectionOrder.Contains("body"))
                    {
                        _sectionOrder.Add("body");
                    }
                }
                
                // The layout will be processed later, when the full template is processed
                return $"LAYOUT:{layoutName}";
            });

            // Register render body helper
            templateEngine.RegisterHelper("renderBody", (context) => {
                // Check if the body section exists
                if (_sections.TryGetValue("body", out var content))
                {
                    return content;
                }
                
                return string.Empty;
            });

            // Register clear sections helper
            templateEngine.RegisterHelper("clearSections", (context) => {
                _sections.Clear();
                _sectionOrder.Clear();
                return string.Empty;
            });

            // Register render all sections helper
            templateEngine.RegisterHelper("renderAllSections", (context) => {
                var sb = new StringBuilder();
                
                foreach (var sectionName in _sectionOrder)
                {
                    if (_sections.TryGetValue(sectionName, out var content))
                    {
                        sb.Append(content);
                    }
                }
                
                return sb.ToString();
            });

            // Register move section helper
            templateEngine.RegisterHelper("moveSection", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                var position = arguments[1]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName) || !_sectionOrder.Contains(sectionName)) return string.Empty;
                
                // Remove the section from its current position
                _sectionOrder.Remove(sectionName);
                
                // Place it at the requested position
                switch (position.ToLowerInvariant())
                {
                    case "first":
                        _sectionOrder.Insert(0, sectionName);
                        break;
                    
                    case "last":
                        _sectionOrder.Add(sectionName);
                        break;
                    
                    default:
                        // Try to parse as a numeric position
                        if (int.TryParse(position, out var pos))
                        {
                            // Clamp the position to a valid range
                            pos = Math.Max(0, Math.Min(pos, _sectionOrder.Count));
                            _sectionOrder.Insert(pos, sectionName);
                        }
                        else
                        {
                            // Default to adding at the end
                            _sectionOrder.Add(sectionName);
                        }
                        break;
                }
                
                return string.Empty;
            });

            // Register move section after helper
            templateEngine.RegisterHelper("moveSectionAfter", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                var targetSectionName = arguments[1]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName) || string.IsNullOrEmpty(targetSectionName) ||
                    !_sectionOrder.Contains(sectionName) || !_sectionOrder.Contains(targetSectionName)) 
                    return string.Empty;
                
                // Remove the section from its current position
                _sectionOrder.Remove(sectionName);
                
                // Find the target section position
                var targetPos = _sectionOrder.IndexOf(targetSectionName);
                
                // Insert after the target
                _sectionOrder.Insert(targetPos + 1, sectionName);
                
                return string.Empty;
            });

            // Register move section before helper
            templateEngine.RegisterHelper("moveSectionBefore", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var sectionName = arguments[0]?.ToString() ?? string.Empty;
                var targetSectionName = arguments[1]?.ToString() ?? string.Empty;
                
                if (string.IsNullOrEmpty(sectionName) || string.IsNullOrEmpty(targetSectionName) ||
                    !_sectionOrder.Contains(sectionName) || !_sectionOrder.Contains(targetSectionName)) 
                    return string.Empty;
                
                // Remove the section from its current position
                _sectionOrder.Remove(sectionName);
                
                // Find the target section position
                var targetPos = _sectionOrder.IndexOf(targetSectionName);
                
                // Insert before the target
                _sectionOrder.Insert(targetPos, sectionName);
                
                return string.Empty;
            });
        }
    }
}
