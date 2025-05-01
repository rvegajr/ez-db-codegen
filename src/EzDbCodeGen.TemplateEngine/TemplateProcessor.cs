using System;
using System.IO;
using System.Collections.Generic;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.TemplateEngine.Filters;

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
        AddFilter(OutputTemplateFilters.LineEndingsFilter(LineEndingType.PlatformDefault));
        AddFilter(OutputTemplateFilters.RemoveTrailingWhitespaceFilter());
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
        _logger.LogDebug($"Added filter: {filter.Name}");
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
            // Check if the template should be processed
            if (!_filter.ShouldProcessTemplate(templatePath, dataModel))
            {
                _logger.LogDebug($"Template {templatePath} was filtered out - skipping processing");
                return string.Empty;
            }
            
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
            
            // Apply output filters
            result = _filter.ApplyOutputFilters(result, templatePath);
            
            return result;
        }
        catch (Exception ex) when (!(ex is TemplateProcessingException))
        {
            _logger.LogError($"Error processing template file {templatePath}: {ex.Message}");
            throw new TemplateProcessingException($"Error processing template file {templatePath}", ex);
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
            
            // Compile and execute the template
            var compiledTemplate = TemplateEngine.Compile(templateContent);
            var result = TemplateEngine.Execute(compiledTemplate, dataModel);
            
            // Apply output filters (with null template name since this is raw content)
            result = _filter.ApplyOutputFilters(result, null);
            
            return result;
        }
        catch (Exception ex) when (!(ex is TemplateProcessingException))
        {
            _logger.LogError($"Error processing template content: {ex.Message}");
            throw new TemplateProcessingException("Error processing template content", ex);
        }
    }

    /// <inheritdoc/>
    public string ProcessLayout(string layoutTemplatePath, string bodyContent, object dataModel)
    {
        if (string.IsNullOrEmpty(layoutTemplatePath))
        {
            throw new ArgumentNullException(nameof(layoutTemplatePath));
        }

        try
        {
            // Check if the layout template should be processed
            if (!_filter.ShouldProcessTemplate(layoutTemplatePath, dataModel))
            {
                _logger.LogDebug($"Layout template {layoutTemplatePath} was filtered out - using body content directly");
                return bodyContent;
            }
            
            _logger.LogDebug($"Processing layout template: {layoutTemplatePath}");
            
            // Resolve the layout template path
            string fullPath = ResolveTemplatePath(layoutTemplatePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Layout template file not found: {fullPath}", fullPath);
            }
            
            // Read the layout template content
            string layoutContent = File.ReadAllText(fullPath);
            
            // Add the body content to the data model
            var layoutDataModel = new LayoutDataModel(dataModel, bodyContent);
            
            // Process the layout template with the body content
            string result = ProcessTemplateContent(layoutContent, layoutDataModel);
            
            // Apply output filters
            result = _filter.ApplyOutputFilters(result, layoutTemplatePath);
            
            return result;
        }
        catch (Exception ex) when (!(ex is TemplateProcessingException))
        {
            _logger.LogError($"Error processing layout template {layoutTemplatePath}: {ex.Message}");
            throw new TemplateProcessingException($"Error processing layout template {layoutTemplatePath}", ex);
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
            TemplateEngine.RegisterHelper(name, helper);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering helper {name}: {ex.Message}");
            throw new HelperRegistrationException($"Error registering helper {name}", ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterBlockHelper(string name, Delegate helper)
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
            _logger.LogDebug($"Registering block helper: {name}");
            TemplateEngine.RegisterBlockHelper(name, helper);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering block helper {name}: {ex.Message}");
            throw new HelperRegistrationException($"Error registering block helper {name}", ex);
        }
    }

    /// <inheritdoc/>
    public void RegisterPartial(string name, string templatePath)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrEmpty(templatePath))
        {
            throw new ArgumentNullException(nameof(templatePath));
        }

        try
        {
            _logger.LogDebug($"Registering partial: {name} from {templatePath}");
            
            // Resolve the template path
            string fullPath = ResolveTemplatePath(templatePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Partial template file not found: {fullPath}", fullPath);
            }
            
            // Read the partial template content
            string partialContent = File.ReadAllText(fullPath);
            
            // Register the partial with the template engine
            TemplateEngine.RegisterPartial(name, partialContent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error registering partial {name}: {ex.Message}");
            throw new PartialRegistrationException($"Error registering partial {name}", ex);
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
}

/// <summary>
/// Exception thrown when an error occurs during template processing.
/// </summary>
public class TemplateProcessingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateProcessingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TemplateProcessingException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateProcessingException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TemplateProcessingException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
