using System;
using System.Collections.Generic;
using System.IO;
using EzDbCodeGen.Core.Interfaces.Logging;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.Filters;

namespace EzDbCodeGen.TemplateEngine;

/// <summary>
/// Implements a template processor that only processes templates that have changed since the last execution.
/// This helps improve performance by avoiding redundant processing and output operations.
/// </summary>
public class DifferentialTemplateProcessor : ITemplateProcessor
{
    private readonly ICodeGenerationLogger _logger;
    private readonly ITemplateProcessor _baseProcessor;
    private readonly Dictionary<string, string> _previousOutputs = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<ITemplateFilter> _filters = new();
    private readonly Dictionary<string, DateTime> _lastWriteTimes = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="DifferentialTemplateProcessor"/> class.
    /// </summary>
    /// <param name="baseProcessor">The base template processor to wrap.</param>
    /// <param name="logger">The logger to use.</param>
    public DifferentialTemplateProcessor(
        ITemplateProcessor baseProcessor,
        ICodeGenerationLogger logger)
    {
        _baseProcessor = baseProcessor ?? throw new ArgumentNullException(nameof(baseProcessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Adds a template filter to the processor.
    /// </summary>
    /// <param name="filter">The filter to add.</param>
    public void AddFilter(ITemplateFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }
        
        _filters.Add(filter);
        _logger.LogDebug($"Added template filter: {filter.GetType().Name}");
    }

    /// <inheritdoc/>
    public string Process(string templateContent, object dataModel)
    {
        if (string.IsNullOrEmpty(templateContent))
        {
            throw new ArgumentNullException(nameof(templateContent));
        }
        
        var templateKey = GetTemplateKey(templateContent.GetHashCode().ToString(), dataModel);
        
        try
        {
            // Check if we've processed this content before with the same model
            if (_previousOutputs.TryGetValue(templateKey, out var previousOutput))
            {
                _logger.LogDebug("Template content has not changed - reusing previous output");
                return previousOutput;
            }
            
            _logger.LogDebug("Processing new template content");
            
            // Process the template using the base processor
            var output = _baseProcessor.Process(templateContent, dataModel);
            
            // Apply any filters
            output = ApplyFilters(output, templateContent.GetHashCode().ToString());
            
            // Store the output for future comparison
            _previousOutputs[templateKey] = output;
            
            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in differential processing of template content: {ex.Message}");
            throw new TemplateProcessingException("Error in differential processing of template content", ex);
        }
    }

    /// <inheritdoc/>
    public string ProcessFile(string templatePath, object dataModel)
    {
        if (string.IsNullOrEmpty(templatePath))
        {
            throw new ArgumentNullException(nameof(templatePath));
        }
        
        var templateKey = GetTemplateKey(templatePath, dataModel);
        
        try
        {
            // First check if the template has changed
            if (!HasChanged(templatePath, dataModel))
            {
                _logger.LogDebug($"Template {templatePath} has not changed - reusing previous output");
                return _previousOutputs[templateKey];
            }
            
            _logger.LogDebug($"Processing changed template {templatePath}");
            
            // Process the template using the base processor
            var output = _baseProcessor.ProcessFile(templatePath, dataModel);
            
            // Apply any filters
            output = ApplyFilters(output, templatePath);
            
            // Store the output for future comparison
            _previousOutputs[templateKey] = output;
            
            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in differential processing of template {templatePath}: {ex.Message}");
            throw new TemplateProcessingException($"Error in differential processing of template {templatePath}", ex);
        }
    }

    /// <inheritdoc/>
    public string ProcessFile(string templatePath, string outputPath, object dataModel)
    {
        var result = ProcessFile(templatePath, dataModel);
        
        if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(outputPath))
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(outputPath, result);
            _logger.LogDebug($"Wrote output to: {outputPath}");
        }
        
        return result;
    }

    /// <inheritdoc/>
    public IDictionary<string, string> ProcessFiles(IEnumerable<string> templatePaths, object dataModel)
    {
        if (templatePaths == null)
        {
            throw new ArgumentNullException(nameof(templatePaths));
        }
        
        var results = new Dictionary<string, string>();
        
        foreach (var templatePath in templatePaths)
        {
            var result = ProcessFile(templatePath, dataModel);
            results[templatePath] = result;
        }
        
        return results;
    }
    
    /// <inheritdoc/>
    public IDictionary<string, string> ProcessFiles(IEnumerable<string> templatePaths, string outputDirectory, object dataModel)
    {
        if (templatePaths == null)
        {
            throw new ArgumentNullException(nameof(templatePaths));
        }
        
        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }
        
        var results = new Dictionary<string, string>();
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        
        foreach (var templatePath in templatePaths)
        {
            var fileName = Path.GetFileName(templatePath);
            var outputPath = Path.Combine(outputDirectory, fileName);
            
            var result = ProcessFile(templatePath, outputPath, dataModel);
            results[templatePath] = result;
        }
        
        return results;
    }
    
    /// <summary>
    /// Determines if a template has changed since it was last processed.
    /// </summary>
    /// <param name="templatePath">The path to the template file.</param>
    /// <param name="dataModel">The data model to use for template processing.</param>
    /// <returns>True if the template has changed; otherwise, false.</returns>
    public bool HasChanged(string templatePath, object dataModel)
    {
        var templateKey = GetTemplateKey(templatePath, dataModel);
        
        // If we've never processed this template before, it has changed
        if (!_previousOutputs.ContainsKey(templateKey))
        {
            return true;
        }
        
        // Check if the template file has been modified
        if (File.Exists(templatePath))
        {
            var lastWriteTime = File.GetLastWriteTime(templatePath);
            
            // Store the last write time in a dictionary to compare later
            if (!_lastWriteTimes.TryGetValue(templatePath, out var storedLastWriteTime) ||
                lastWriteTime > storedLastWriteTime)
            {
                _lastWriteTimes[templatePath] = lastWriteTime;
                return true;
            }
        }
        else
        {
            // Template file doesn't exist anymore, so it has changed
            return true;
        }
        
        return false;
    }
    
    private string ApplyFilters(string output, string templatePath)
    {
        foreach (var filter in _filters)
        {
            try
            {
                // Apply the filter
                output = filter.Filter(output);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error applying filter {filter.GetType().Name}: {ex.Message}");
            }
        }
        
        return output;
    }
    
    private string GetTemplateKey(string templateIdentifier, object dataModel)
    {
        // Create a key that uniquely identifies the template and the data model
        // This is used to detect if we've processed this exact combination before
        var modelHash = dataModel?.GetHashCode() ?? 0;
        return $"{templateIdentifier}:{modelHash}";
    }
}
