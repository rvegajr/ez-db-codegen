using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using EzDbCodeGen.Core.Interfaces;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.TemplateEngine.Testing
{
    /// <summary>
    /// Helper methods for Handlebars templates in tests
    /// </summary>
    internal static class HandlebarsHelpers
    {
        /// <summary>
        /// A simple test helper for Handlebars templates
        /// </summary>
        public static void TestHelper(EncodedTextWriter writer, Context context, Arguments args)
        {
            writer.WriteSafeString("test");
        }
    }

    /// <summary>
    /// A test harness for testing templates against database schemas.
    /// </summary>
    public class TemplateTestHarness
    {
        private readonly EzDbCodeGen.TemplateEngine.Interfaces.ITemplateProcessor _templateProcessor;
        private readonly EzDbCodeGen.CodeGen.Interfaces.ISchemaModelAdapter _schemaModelAdapter;
        private readonly Microsoft.Extensions.Logging.ILogger<TemplateTestHarness>? _logger;
        private readonly EzDbCodeGen.TemplateEngine.Interfaces.ITemplateProcessorFactory _templateProcessorFactory;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateTestHarness"/> class.
        /// </summary>
        /// <param name="templateProcessor">The template processor to use.</param>
        /// <param name="schemaModelAdapter">The schema model adapter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="templateProcessorFactory">The template processor factory.</param>
        public TemplateTestHarness(
            EzDbCodeGen.TemplateEngine.Interfaces.ITemplateProcessor templateProcessor,
            EzDbCodeGen.CodeGen.Interfaces.ISchemaModelAdapter schemaModelAdapter,
            Microsoft.Extensions.Logging.ILogger<TemplateTestHarness>? logger = null,
            EzDbCodeGen.TemplateEngine.Interfaces.ITemplateProcessorFactory templateProcessorFactory = null)
        {
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _schemaModelAdapter = schemaModelAdapter ?? throw new ArgumentNullException(nameof(schemaModelAdapter));
            _logger = logger;
            _templateProcessorFactory = templateProcessorFactory;
        }
        
        /// <summary>
        /// Tests a template against a database schema.
        /// </summary>
        /// <param name="templatePath">The path to the template.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="options">The test options.</param>
        /// <returns>A result with the output and any errors.</returns>
        public async Task<TemplateTestResult> TestTemplateAsync(
            string templatePath,
            EzDbCodeGen.Schema.Interfaces.IDatabaseSchema schema,
            TemplateTestOptions options)
        {
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                options = new TemplateTestOptions();
            }

            _logger?.LogInformation("Testing template {TemplatePath} against database {DatabaseName}", 
                templatePath, schema.DatabaseName);

            try
            {
                // Create a result object
                var result = new TemplateTestResult
                {
                    TemplatePath = templatePath,
                    DatabaseName = schema.DatabaseName,
                    Success = true,
                    TestTime = DateTime.UtcNow
                };
                
                // Convert schema to template model
                _logger?.LogDebug("Converting schema to template model");
                var model = await _schemaModelAdapter.ConvertSchemaToTemplateModelAsync(schema);
                
                // Add options to the model
                model["Options"] = new Dictionary<string, object>
                {
                    ["TargetLanguage"] = options.TargetLanguage,
                    ["IncludeComments"] = options.IncludeComments,
                    ["DetectRelationships"] = options.DetectRelationships,
                    ["GenerateNavigationProperties"] = options.GenerateNavigationProperties
                };
                
                // Process the template with the schema model
                _logger?.LogDebug("Processing template {TemplatePath}", templatePath);
                
                // If testing a specific table
                if (!string.IsNullOrEmpty(options.TargetTable))
                {
                    var table = schema.Tables.FirstOrDefault(t => t.Name == options.TargetTable);
                    if (table == null)
                    {
                        result.Success = false;
                        result.Errors.Add($"Table '{options.TargetTable}' not found in schema");
                        return result;
                    }
                    
                    _logger?.LogDebug("Testing template against table {TableName}", table.Name);
                    
                    var tableModel = await _schemaModelAdapter.ConvertTableToTemplateModelAsync(table);
                    tableModel["Schema"] = model; // Include full schema
                    tableModel["Options"] = model["Options"];
                    
                    try
                    {
                        var processor = _templateProcessorFactory.CreateProcessor(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType.Handlebars);
                        var output = await Task.FromResult(processor.ProcessFile(templatePath, tableModel));
                        result.Output = output;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Errors.Add($"Error processing template for table '{table.Name}': {ex.Message}");
                        _logger?.LogError(ex, "Error processing template for table {TableName}", table.Name);
                    }
                }
                // If testing a specific view
                else if (!string.IsNullOrEmpty(options.TargetView))
                {
                    var view = schema.Views.FirstOrDefault(v => v.Name == options.TargetView);
                    if (view == null)
                    {
                        result.Success = false;
                        result.Errors.Add($"View '{options.TargetView}' not found in schema");
                        return result;
                    }
                    
                    _logger?.LogDebug("Testing template against view {ViewName}", view.Name);
                    
                    var viewModel = await _schemaModelAdapter.ConvertViewToTemplateModelAsync(view);
                    viewModel["Schema"] = model; // Include full schema
                    viewModel["Options"] = model["Options"];
                    
                    try
                    {
                        var processor = _templateProcessorFactory.CreateProcessor(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType.Handlebars);
                        var output = await Task.FromResult(processor.ProcessFile(templatePath, viewModel));
                        result.Output = output;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Errors.Add($"Error processing template for view '{view.Name}': {ex.Message}");
                        _logger?.LogError(ex, "Error processing template for view {ViewName}", view.Name);
                    }
                }
                // Otherwise test with the full schema
                else
                {
                    try
                    {
                        var processor = _templateProcessorFactory.CreateProcessor(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType.Handlebars);
                        var output = await Task.FromResult(processor.ProcessFile(templatePath, model));
                        result.Output = output;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Errors.Add($"Error processing template: {ex.Message}");
                        _logger?.LogError(ex, "Error processing template");
                    }
                }
                
                // Run additional validators if specified
                if (options.ValidateOutput)
                {
                    _logger?.LogDebug("Validating template output");
                    
                    // Basic validation checks
                    if (string.IsNullOrWhiteSpace(result.Output))
                    {
                        result.Warnings.Add("Template output is empty");
                    }
                    
                    if (options.CheckForPlaceholders)
                    {
                        ValidatePlaceholders(result);
                    }
                    
                    if (options.CheckSyntaxForLanguage)
                    {
                        ValidateSyntaxForLanguage(result, options.TargetLanguage);
                    }
                }
                
                // Log success or failure
                if (result.Success)
                {
                    _logger?.LogInformation("Template test successful for {TemplatePath}", templatePath);
                }
                else
                {
                    _logger?.LogWarning("Template test failed for {TemplatePath} with {ErrorCount} errors",
                        templatePath, result.Errors.Count);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error testing template {TemplatePath}: {ErrorMessage}", 
                    templatePath, ex.Message);
                
                return new TemplateTestResult
                {
                    TemplatePath = templatePath,
                    DatabaseName = schema.DatabaseName,
                    Success = false,
                    TestTime = DateTime.UtcNow,
                    Errors = { ex.Message }
                };
            }
        }
        
        /// <summary>
        /// Tests a template string against a database schema.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="options">The test options.</param>
        /// <returns>A result with the output and any errors.</returns>
        public async Task<TemplateTestResult> TestTemplateStringAsync(
            string templateContent,
            EzDbCodeGen.Schema.Interfaces.IDatabaseSchema schema,
            TemplateTestOptions options)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                options = new TemplateTestOptions();
            }

            // Create a temporary file with the template content
            var tempFilePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFilePath, templateContent);
            
            try
            {
                // Test the template file
                return await TestTemplateAsync(tempFilePath, schema, options);
            }
            finally
            {
                // Clean up the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        
        /// <summary>
        /// Tests multiple templates against a database schema.
        /// </summary>
        /// <param name="templateDirectory">The directory containing templates.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="options">The test options.</param>
        /// <returns>A collection of test results.</returns>
        public async Task<IReadOnlyList<TemplateTestResult>> TestTemplatesInDirectoryAsync(
            string templateDirectory,
            EzDbCodeGen.Schema.Interfaces.IDatabaseSchema schema,
            TemplateTestOptions options)
        {
            if (string.IsNullOrEmpty(templateDirectory))
            {
                throw new ArgumentException("Template directory cannot be null or empty.", nameof(templateDirectory));
            }

            if (!Directory.Exists(templateDirectory))
            {
                throw new DirectoryNotFoundException($"Template directory not found: {templateDirectory}");
            }

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                options = new TemplateTestOptions();
            }

            _logger?.LogInformation("Testing templates in directory {TemplateDirectory} against database {DatabaseName}", 
                templateDirectory, schema.DatabaseName);

            var results = new List<TemplateTestResult>();
            
            // Get all template files
            var templateFiles = Directory.GetFiles(templateDirectory, options.TemplateFilePattern, SearchOption.AllDirectories);
            _logger?.LogDebug("Found {Count} template files to test", templateFiles.Length);
            
            // Test each template
            foreach (var templateFile in templateFiles)
            {
                var result = await TestTemplateAsync(templateFile, schema, options);
                results.Add(result);
            }
            
            _logger?.LogInformation("Completed testing {Count} templates with {SuccessCount} successes and {FailureCount} failures",
                results.Count, results.Count(r => r.Success), results.Count(r => !r.Success));
            
            return results;
        }
        
        private void ValidatePlaceholders(TemplateTestResult result)
        {
            // Check for common placeholder patterns that might indicate incomplete templates
            if (result.Output.Contains("{{") && result.Output.Contains("}}"))
            {
                result.Warnings.Add("Output contains possible unprocessed template placeholders");
            }
            
            if (result.Output.Contains("[[") && result.Output.Contains("]]"))
            {
                result.Warnings.Add("Output contains possible custom placeholder markers");
            }
            
            if (result.Output.Contains("TODO") || result.Output.Contains("FIXME") || 
                result.Output.Contains("PLACEHOLDER") || result.Output.Contains("NOT IMPLEMENTED"))
            {
                result.Warnings.Add("Output contains TODO or placeholder comments");
            }
        }
        
        private void ValidateSyntaxForLanguage(TemplateTestResult result, string targetLanguage)
        {
            if (string.IsNullOrEmpty(targetLanguage))
            {
                return;
            }
            
            // Very basic syntax checks based on language
            switch (targetLanguage.ToLower())
            {
                case "csharp":
                    if (result.Output.Contains("{") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Possible unbalanced braces in C# output");
                    }
                    
                    if (result.Output.Contains("class ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Class definition appears incomplete in C# output");
                    }
                    
                    if (result.Output.Contains("namespace ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Namespace definition appears incomplete in C# output");
                    }
                    
                    if (result.Output.Contains("using ") && !result.Output.Contains(";"))
                    {
                        result.Warnings.Add("Using statement may be missing semicolon in C# output");
                    }
                    break;
                    
                case "typescript":
                case "javascript":
                    if (result.Output.Contains("{") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Possible unbalanced braces in TypeScript/JavaScript output");
                    }
                    
                    if (result.Output.Contains("class ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Class definition appears incomplete in TypeScript/JavaScript output");
                    }
                    
                    if (result.Output.Contains("interface ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Interface definition appears incomplete in TypeScript output");
                    }
                    
                    if (result.Output.Contains("import ") && !result.Output.Contains(";"))
                    {
                        result.Warnings.Add("Import statement may be missing semicolon in TypeScript/JavaScript output");
                    }
                    break;
                    
                case "java":
                    if (result.Output.Contains("{") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Possible unbalanced braces in Java output");
                    }
                    
                    if (result.Output.Contains("class ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Class definition appears incomplete in Java output");
                    }
                    
                    if (result.Output.Contains("interface ") && !result.Output.Contains("}"))
                    {
                        result.Warnings.Add("Interface definition appears incomplete in Java output");
                    }
                    
                    if (result.Output.Contains("package ") && !result.Output.Contains(";"))
                    {
                        result.Warnings.Add("Package statement may be missing semicolon in Java output");
                    }
                    
                    if (result.Output.Contains("import ") && !result.Output.Contains(";"))
                    {
                        result.Warnings.Add("Import statement may be missing semicolon in Java output");
                    }
                    break;
                    
                case "python":
                    if (result.Output.Contains("class ") && !result.Output.Contains(":"))
                    {
                        result.Warnings.Add("Class definition may be missing colon in Python output");
                    }
                    
                    if (result.Output.Contains("def ") && !result.Output.Contains(":"))
                    {
                        result.Warnings.Add("Function definition may be missing colon in Python output");
                    }
                    break;
            }
        }
    }
}
