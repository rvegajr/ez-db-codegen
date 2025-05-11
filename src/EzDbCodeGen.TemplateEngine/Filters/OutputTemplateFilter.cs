using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EzDbCodeGen.TemplateEngine.Interfaces.Filters;

namespace EzDbCodeGen.TemplateEngine.Filters;

/// <summary>
/// A template filter that applies transformations to the generated output.
/// </summary>
public class OutputTemplateFilter : ITemplateProcessingFilter
{
    private readonly Func<string, string> _transformFunc;
    private readonly Predicate<string> _templateMatcher;
    
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Gets a description of the filter.
    /// </summary>
    public string Description { get; }
    
    /// <summary>
    /// Gets the order in which this filter should be applied relative to other filters.
    /// </summary>
    public int Order { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTemplateFilter"/> class.
    /// </summary>
    /// <param name="name">The name of the filter.</param>
    /// <param name="description">A description of the filter.</param>
    /// <param name="transformFunc">A function that transforms the generated output.</param>
    /// <param name="templateMatcher">A predicate that determines which templates this filter applies to.</param>
    /// <param name="order">The order in which this filter should be applied relative to other filters.</param>
    public OutputTemplateFilter(
        string name,
        string description,
        Func<string, string> transformFunc,
        Predicate<string> templateMatcher = null,
        int order = 0)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        _transformFunc = transformFunc ?? throw new ArgumentNullException(nameof(transformFunc));
        _templateMatcher = templateMatcher ?? (_ => true); // Default to match all templates
        Order = order;
    }
    
    /// <summary>
    /// Applies the filter to the generated output.
    /// </summary>
    /// <param name="output">The generated output to filter.</param>
    /// <returns>The filtered output.</returns>
    public string ApplyTransform(string output)
    {
        if (string.IsNullOrEmpty(output))
        {
            return output;
        }
        
        return _transformFunc(output);
    }
    
    /// <summary>
    /// Determines if this filter should be applied to the specified template.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <returns>True if this filter should be applied to the template; otherwise, false.</returns>
    public bool ShouldApplyTo(string templateName)
    {
        if (string.IsNullOrEmpty(templateName))
        {
            return false;
        }
        
        return _templateMatcher(templateName);
    }

    /// <inheritdoc/>
    public string Filter(string input)
    {
        return ApplyTransform(input);
    }

    /// <inheritdoc/>
    public bool ShouldProcessTemplate(string templateName, object? model)
    {
        // This filter doesn't prevent template processing, it only transforms the output
        return true;
    }

    /// <inheritdoc/>
    public bool ShouldProcessModel(object? model, string templateName)
    {
        // This filter doesn't prevent model processing, it only transforms the output
        return true;
    }

    /// <inheritdoc/>
    public bool ShouldIncludeProperty(string propertyName, object? model, string templateName)
    {
        // This filter doesn't filter properties, it only transforms the output
        return true;
    }
}

/// <summary>
/// Factory for creating common output template filters.
/// </summary>
public static class OutputTemplateFilters
{
    /// <summary>
    /// Creates a filter that normalizes line endings.
    /// </summary>
    /// <param name="lineEnding">The type of line ending to use.</param>
    /// <param name="order">The order in which this filter should be applied.</param>
    /// <returns>A filter that normalizes line endings.</returns>
    public static OutputTemplateFilter LineEndingsFilter(LineEndingType lineEnding, int order = 0)
    {
        string description = $"Normalizes line endings to {lineEnding}";
        
        Func<string, string> transformFunc = content =>
        {
            // First normalize to \n
            content = content.Replace("\r\n", "\n").Replace("\r", "\n");
            
            // Then convert to the desired line ending
            return lineEnding switch
            {
                LineEndingType.Windows => content.Replace("\n", "\r\n"),
                LineEndingType.Unix => content,
                LineEndingType.Mac => content.Replace("\n", "\r"),
                LineEndingType.PlatformDefault => content.Replace("\n", Environment.NewLine),
                _ => content // KeepAsIs or unknown
            };
        };
        
        return new OutputTemplateFilter("LineEndings", description, transformFunc, null, order);
    }
    
    /// <summary>
    /// Creates a filter that removes trailing whitespace from each line.
    /// </summary>
    /// <param name="order">The order in which this filter should be applied.</param>
    /// <returns>A filter that removes trailing whitespace.</returns>
    public static OutputTemplateFilter RemoveTrailingWhitespaceFilter(int order = 10)
    {
        string description = "Removes trailing whitespace from each line";
        
        Func<string, string> transformFunc = content =>
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            
            // Replace trailing whitespace on each line
            return Regex.Replace(content, @"[ \t]+$", "", RegexOptions.Multiline);
        };
        
        return new OutputTemplateFilter("RemoveTrailingWhitespace", description, transformFunc, null, order);
    }
    
    /// <summary>
    /// Creates a filter that removes empty lines.
    /// </summary>
    /// <param name="order">The order in which this filter should be applied.</param>
    /// <returns>A filter that removes empty lines.</returns>
    public static OutputTemplateFilter RemoveEmptyLinesFilter(int order = 20)
    {
        string description = "Removes empty lines";
        
        Func<string, string> transformFunc = content =>
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            
            // Replace consecutive empty lines with a single empty line
            return Regex.Replace(content, @"(\r?\n){3,}", Environment.NewLine + Environment.NewLine);
        };
        
        return new OutputTemplateFilter("RemoveEmptyLines", description, transformFunc, null, order);
    }
    
    /// <summary>
    /// Creates a filter that ensures the output ends with a single newline.
    /// </summary>
    /// <param name="order">The order in which this filter should be applied.</param>
    /// <returns>A filter that ensures the output ends with a single newline.</returns>
    public static OutputTemplateFilter EnsureTrailingNewlineFilter(int order = 30)
    {
        string description = "Ensures the output ends with a single newline";
        
        Func<string, string> transformFunc = content =>
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }
            
            // Remove all trailing newlines
            content = Regex.Replace(content, @"(\r?\n)+$", "");
            
            // Add a single newline
            return content + Environment.NewLine;
        };
        
        return new OutputTemplateFilter("EnsureTrailingNewline", description, transformFunc, null, order);
    }
    
    /// <summary>
    /// Creates a filter that formats the code according to a specific language.
    /// This is a placeholder and should be replaced with actual code formatting logic.
    /// </summary>
    /// <param name="language">The language to format (e.g., "csharp", "typescript", "sql").</param>
    /// <param name="order">The order in which this filter should be applied.</param>
    /// <returns>A filter that formats the code.</returns>
    public static OutputTemplateFilter CodeFormattingFilter(string language, int order = 100)
    {
        string description = $"Formats {language} code";
        
        Func<string, string> transformFunc = content =>
        {
            // This is a placeholder for code formatting logic
            // In a real implementation, this would call a language-specific formatter
            return content;
        };
        
        // Only apply to files with specific extensions based on the language
        Predicate<string> templateMatcher = templateName =>
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return false;
            }
            
            return language.ToLowerInvariant() switch
            {
                "csharp" => templateName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                           templateName.EndsWith(".csharp", StringComparison.OrdinalIgnoreCase),
                "typescript" => templateName.EndsWith(".ts", StringComparison.OrdinalIgnoreCase),
                "javascript" => templateName.EndsWith(".js", StringComparison.OrdinalIgnoreCase),
                "sql" => templateName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase),
                "html" => templateName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                         templateName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase),
                "css" => templateName.EndsWith(".css", StringComparison.OrdinalIgnoreCase),
                "xml" => templateName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase),
                "json" => templateName.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
                "markdown" => templateName.EndsWith(".md", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        };
        
        return new OutputTemplateFilter($"CodeFormatting_{language}", description, transformFunc, templateMatcher, order);
    }
}

/// <summary>
/// Enum that defines the different types of line endings.
/// </summary>
public enum LineEndingType
{
    /// <summary>
    /// Keep the line endings as they are in the template.
    /// </summary>
    KeepAsIs,
    
    /// <summary>
    /// Use Windows-style line endings (CR+LF, \r\n).
    /// </summary>
    Windows,
    
    /// <summary>
    /// Use Unix-style line endings (LF, \n).
    /// </summary>
    Unix,
    
    /// <summary>
    /// Use Mac-style line endings (CR, \r).
    /// </summary>
    Mac,
    
    /// <summary>
    /// Use the platform-default line endings (Environment.NewLine).
    /// </summary>
    PlatformDefault
}
