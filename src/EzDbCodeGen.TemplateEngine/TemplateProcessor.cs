using System;
using System.IO;
using System.Collections.Generic;
using EzDbCodeGen.Core.Interfaces.Logging;
using EzDbCodeGen.TemplateEngine.Filters;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.Filters;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine;

/// <summary>
/// Implements a template processor using a specified template engine.
/// </summary>
public class TemplateProcessor : ITemplateProcessor
{
    private readonly ICodeGenerationLogger _logger;
    private readonly CompositeTemplateFilter _filter = new();
    
    /// <summary>
    /// Gets the template engine used by this processor.
    /// </summary>
    public ITemplateEngine TemplateEngine { get; }
    
    /// <summary>
    /// Gets or sets the base path for resolving relative template paths.
    /// </summary>
    public string BasePath { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateProcessor"/> class.
    /// </summary>
    /// <param name="templateEngine">The template engine to use.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="basePath">The base path for resolving relative template paths.</param>
    public TemplateProcessor(
        ITemplateEngine templateEngine, 
        ICodeGenerationLogger logger, 
        string basePath = null)
    {
        TemplateEngine = templateEngine ?? throw new ArgumentNullException(nameof(templateEngine));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        BasePath = basePath ?? Directory.GetCurrentDirectory();
        
        // Add default filters
        AddOutputFilter(OutputTemplateFilters.LineEndingsFilter(LineEndingType.PlatformDefault));
        AddOutputFilter(OutputTemplateFilters.RemoveTrailingWhitespaceFilter());
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
        
        _filter.AddFilter(filter);
        _logger.LogDebug($"Added filter: {filter.GetType().Name}");
    }
    
    /// <summary>
    /// Adds an output filter to the processor.
    /// </summary>
    /// <param name="filter">The output filter to add.</param>
    public void AddOutputFilter(OutputTemplateFilter filter)
    {
        if (filter == null)
        {
            throw new ArgumentNullException(nameof(filter));
        }
        
        _filter.AddOutputFilter(filter);
        _logger.LogDebug($"Added output filter: {filter.Name}");
    }

    /// <inheritdoc/>
    public string ProcessTemplate(string templatePath, object dataModel)
    {
        if (string.IsNullOrEmpty(templatePath))
        {
            throw new ArgumentNullException(nameof(templatePath));
        }

        try
        {
            _logger.LogDebug($"Processing template file: {templatePath}");
            
            // Resolve the template path
            string fullPath = ResolveTemplatePath(templatePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Template file not found: {fullPath}", fullPath);
            }
            
            // Read the template content
            string templateContent = File.ReadAllText(fullPath);
            
            // Process the template content
            string result = ProcessTemplateContent(templateContent, dataModel);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing template {templatePath}: {ex.Message}");
            throw new TemplateProcessingException($"Error processing template {templatePath}", ex);
        }
    }

    /// <inheritdoc/>
    public string ProcessTemplateContent(string templateContent, object dataModel)
    {
        if (string.IsNullOrEmpty(templateContent))
        {
            throw new ArgumentNullException(nameof(templateContent));
        }

        try
        {
            _logger.LogDebug("Processing template content");
            
            // Apply input filters
            var filteredTemplate = templateContent;
            
            // Check if the template has a layout
            string layoutPath = null;
            
            // Compile and execute the template
            var compiledTemplate = TemplateEngine.Compile(filteredTemplate);
            string result = TemplateEngine.Execute(compiledTemplate, dataModel);
            
            // Apply output filters
            result = _filter.ApplyOutputFilters(result, "template");
            
            // Process layout if specified
            if (!string.IsNullOrEmpty(layoutPath))
            {
                result = ProcessLayout(layoutPath, dataModel, result);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing template content: {ex.Message}");
            throw new TemplateProcessingException("Error processing template content", ex);
        }
    }

    /// <inheritdoc/>
    public string ProcessLayout(string layoutPath, object dataModel, string bodyContent)
    {
        if (string.IsNullOrEmpty(layoutPath))
        {
            throw new ArgumentNullException(nameof(layoutPath));
        }

        try
        {
            _logger.LogDebug($"Processing layout: {layoutPath}");
            
            // Resolve the layout path
            string fullPath = ResolveTemplatePath(layoutPath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Layout file not found: {fullPath}", fullPath);
            }
            
            // Read the layout content
            string layoutContent = File.ReadAllText(fullPath);
            
            // Create a layout data model that includes the original model and the body content
            var layoutModel = new LayoutDataModel(dataModel, bodyContent);
            
            // Compile and execute the layout template
            var compiledTemplate = TemplateEngine.Compile(layoutContent);
            string result = TemplateEngine.Execute(compiledTemplate, layoutModel);
            
            // Apply output filters
            result = _filter.ApplyOutputFilters(result, "layout");
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing layout {layoutPath}: {ex.Message}");
            throw new TemplateProcessingException($"Error processing layout {layoutPath}", ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterHelper(string name, Delegate helper)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (helper == null)
        {
            throw new ArgumentNullException(nameof(helper));
        }

        try
        {
            _logger.LogDebug($"Registering helper: {name}");
            TemplateEngine.RegisterHelper(name, (Action<HandlebarsDotNet.EncodedTextWriter, HandlebarsDotNet.Context, HandlebarsDotNet.Arguments>)helper);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering helper {name}: {ex.Message}");
            throw new TemplateProcessingException($"Error registering helper {name}", ex);
        }
    }
    
    /// <inheritdoc/>
    public void RegisterBlockHelper(string name, Delegate blockHelper)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (blockHelper == null)
        {
            throw new ArgumentNullException(nameof(blockHelper));
        }

        try
        {
            _logger.LogDebug($"Registering block helper: {name}");
            TemplateEngine.RegisterBlockHelper(name, (Action<HandlebarsDotNet.EncodedTextWriter, HandlebarsDotNet.BlockHelperOptions, HandlebarsDotNet.Context, HandlebarsDotNet.Arguments>)blockHelper);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering block helper {name}: {ex.Message}");
            throw new TemplateProcessingException($"Error registering block helper {name}", ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterPartial(string name, string partialContent)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrEmpty(partialContent))
        {
            throw new ArgumentNullException(nameof(partialContent));
        }

        try
        {
            _logger.LogDebug($"Registering partial: {name}");
            TemplateEngine.RegisterPartial(name, partialContent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering partial {name}: {ex.Message}");
            throw new TemplateProcessingException($"Error registering partial {name}", ex);
        }
    }
    
    private string ResolveTemplatePath(string templatePath)
    {
        if (Path.IsPathRooted(templatePath))
        {
            return templatePath;
        }
        
        return Path.Combine(BasePath, templatePath);
    }
    
    /// <summary>
    /// Data model for layout templates.
    /// </summary>
    private class LayoutDataModel
    {
        /// <summary>
        /// Gets the original data model.
        /// </summary>
        public object Model { get; }
        
        /// <summary>
        /// Gets the body content to insert into the layout.
        /// </summary>
        public string Body { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDataModel"/> class.
        /// </summary>
        /// <param name="model">The original data model.</param>
        /// <param name="bodyContent">The body content to insert into the layout.</param>
        public LayoutDataModel(object model, string bodyContent)
        {
            Model = model;
            Body = bodyContent;
        }
    }
    
    /// <inheritdoc/>
    public string Process(string templateContent, object dataModel)
    {
        return ProcessTemplateContent(templateContent, dataModel);
    }
    
    /// <inheritdoc/>
    public string ProcessFile(string templatePath, object dataModel)
    {
        return ProcessTemplate(templatePath, dataModel);
    }
    
    /// <inheritdoc/>
    public string ProcessFile(string templatePath, string outputPath, object dataModel)
    {
        var result = ProcessTemplate(templatePath, dataModel);
        
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
    public System.Collections.Generic.IDictionary<string, string> ProcessFiles(System.Collections.Generic.IEnumerable<string> templatePaths, object dataModel)
    {
        if (templatePaths == null)
        {
            throw new ArgumentNullException(nameof(templatePaths));
        }
        
        var results = new System.Collections.Generic.Dictionary<string, string>();
        
        foreach (var templatePath in templatePaths)
        {
            var result = ProcessTemplate(templatePath, dataModel);
            results[templatePath] = result;
        }
        
        return results;
    }
    
    /// <inheritdoc/>
    public System.Collections.Generic.IDictionary<string, string> ProcessFiles(System.Collections.Generic.IEnumerable<string> templatePaths, string outputDirectory, object dataModel)
    {
        if (templatePaths == null)
        {
            throw new ArgumentNullException(nameof(templatePaths));
        }
        
        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new ArgumentNullException(nameof(outputDirectory));
        }
        
        var results = new System.Collections.Generic.Dictionary<string, string>();
        
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
}
