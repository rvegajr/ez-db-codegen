using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Schema.Interfaces;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.CodeGen;

/// <summary>
/// A code generator that processes templates against database schemas.
/// </summary>
public class CodeGenerator : ICodeGenerator
{
    private readonly ISchemaModelAdapter _schemaModelAdapter;
    private readonly ILogger<CodeGenerator>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
    /// </summary>
    /// <param name="templateProcessor">The template processor to use.</param>
    /// <param name="schemaModelAdapter">The schema model adapter.</param>
    /// <param name="logger">The logger.</param>
    public CodeGenerator(
        ITemplateProcessor templateProcessor,
        ISchemaModelAdapter schemaModelAdapter,
        ILogger<CodeGenerator>? logger = null)
    {
        TemplateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
        _schemaModelAdapter = schemaModelAdapter ?? throw new ArgumentNullException(nameof(schemaModelAdapter));
        _logger = logger;
        BasePath = Directory.GetCurrentDirectory();
        OutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Output");
    }

    /// <inheritdoc/>
    public ITemplateProcessor TemplateProcessor { get; }

    /// <inheritdoc/>
    public string BasePath { get; set; }

    /// <inheritdoc/>
    public string OutputPath { get; set; }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GenerateCodeAsync(
        string templatePath, 
        IDatabaseSchema schema, 
        string outputPath, 
        CodeGenerationOptions options)
    {
        if (string.IsNullOrEmpty(templatePath))
        {
            throw new ArgumentException("Template path cannot be null or empty.", nameof(templatePath));
        }

        if (schema == null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        if (string.IsNullOrEmpty(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (options == null)
        {
            options = new CodeGenerationOptions();
        }

        _logger?.LogInformation("Generating code using template {TemplatePath} for database {DatabaseName}", 
            templatePath, schema.DatabaseName);

        try
        {
            // Set up output directory
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                _logger?.LogDebug("Created output directory: {OutputPath}", outputPath);
            }

            // Convert schema to template model
            _logger?.LogDebug("Converting schema to template model");
            var model = await _schemaModelAdapter.ConvertSchemaToTemplateModelAsync(schema);

            // Add options to the model
            model["Options"] = options;
            
            // Process the template
            _logger?.LogDebug("Processing template: {TemplatePath}", templatePath);
            
            var generatedFiles = new List<string>();
            
            if (options.GeneratePerTable)
            {
                // Generate code for each table
                var tables = schema.Tables;
                _logger?.LogDebug("Generating code for {Count} tables", tables.Count());
                
                foreach (var table in tables)
                {
                    var tableModel = await _schemaModelAdapter.ConvertTableToTemplateModelAsync(table);
                    tableModel["Schema"] = model; // Include the full schema as a property
                    tableModel["Options"] = options;
                    
                    var fileName = ResolveFileName(table.Name, options);
                    var outputFilePath = Path.Combine(outputPath, fileName);
                    
                    _logger?.LogDebug("Processing template for table {TableName}", table.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, tableModel);
                    
                    _logger?.LogDebug("Writing output to {OutputPath}", outputFilePath);
                    await File.WriteAllTextAsync(outputFilePath, result);
                    
                    generatedFiles.Add(outputFilePath);
                }
            }
            else if (options.GeneratePerView)
            {
                // Generate code for each view
                var views = schema.Views;
                _logger?.LogDebug("Generating code for {Count} views", views.Count());
                
                foreach (var view in views)
                {
                    var viewModel = await _schemaModelAdapter.ConvertViewToTemplateModelAsync(view);
                    viewModel["Schema"] = model; // Include the full schema as a property
                    viewModel["Options"] = options;
                    
                    var fileName = ResolveFileName(view.Name, options);
                    var outputFilePath = Path.Combine(outputPath, fileName);
                    
                    _logger?.LogDebug("Processing template for view {ViewName}", view.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, viewModel);
                    
                    _logger?.LogDebug("Writing output to {OutputPath}", outputFilePath);
                    await File.WriteAllTextAsync(outputFilePath, result);
                    
                    generatedFiles.Add(outputFilePath);
                }
            }
            else if (options.GeneratePerStoredProcedure)
            {
                // Generate code for each stored procedure
                var storedProcedures = schema.StoredProcedures;
                _logger?.LogDebug("Generating code for {Count} stored procedures", storedProcedures.Count());
                
                foreach (var storedProcedure in storedProcedures)
                {
                    var spModel = await _schemaModelAdapter.ConvertStoredProcedureToTemplateModelAsync(storedProcedure);
                    spModel["Schema"] = model; // Include the full schema as a property
                    spModel["Options"] = options;
                    
                    var fileName = ResolveFileName(storedProcedure.Name, options);
                    var outputFilePath = Path.Combine(outputPath, fileName);
                    
                    _logger?.LogDebug("Processing template for stored procedure {ProcedureName}", storedProcedure.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, spModel);
                    
                    _logger?.LogDebug("Writing output to {OutputPath}", outputFilePath);
                    await File.WriteAllTextAsync(outputFilePath, result);
                    
                    generatedFiles.Add(outputFilePath);
                }
            }
            else if (options.GeneratePerFunction)
            {
                // Generate code for each function
                var functions = schema.Functions;
                _logger?.LogDebug("Generating code for {Count} functions", functions.Count());
                
                foreach (var function in functions)
                {
                    var functionModel = await _schemaModelAdapter.ConvertFunctionToTemplateModelAsync(function);
                    functionModel["Schema"] = model; // Include the full schema as a property
                    functionModel["Options"] = options;
                    
                    var fileName = ResolveFileName(function.Name, options);
                    var outputFilePath = Path.Combine(outputPath, fileName);
                    
                    _logger?.LogDebug("Processing template for function {FunctionName}", function.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, functionModel);
                    
                    _logger?.LogDebug("Writing output to {OutputPath}", outputFilePath);
                    await File.WriteAllTextAsync(outputFilePath, result);
                    
                    generatedFiles.Add(outputFilePath);
                }
            }
            else
            {
                // Generate a single file for the entire schema
                var fileName = ResolveFileName(options.SingleFileName ?? "output", options);
                var outputFilePath = Path.Combine(outputPath, fileName);
                
                _logger?.LogDebug("Processing template for entire schema");
                
                string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, model);
                
                _logger?.LogDebug("Writing output to {OutputPath}", outputFilePath);
                await File.WriteAllTextAsync(outputFilePath, result);
                
                generatedFiles.Add(outputFilePath);
            }
            
            _logger?.LogInformation("Generated {Count} files", generatedFiles.Count);
            return generatedFiles;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating code: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GenerateCodeFromContentAsync(
        string templateContent, 
        IDatabaseSchema schema, 
        string outputPath, 
        CodeGenerationOptions options)
    {
        if (string.IsNullOrEmpty(templateContent))
        {
            throw new ArgumentException("Template content cannot be null or empty.", nameof(templateContent));
        }

        if (schema == null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        if (string.IsNullOrEmpty(outputPath))
        {
            throw new ArgumentException("Output path cannot be null or empty.", nameof(outputPath));
        }

        if (options == null)
        {
            options = new CodeGenerationOptions();
        }

        _logger?.LogInformation("Generating code using template content for database {DatabaseName}", schema.DatabaseName);

        try
        {
            // Create a temporary file with the template content
            var tempFile = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFile, templateContent);
            
            try
            {
                // Generate code using the temporary file
                return await GenerateCodeAsync(tempFile, schema, outputPath, options);
            }
            finally
            {
                // Clean up the temporary file
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating code from content: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, string>> PreviewCodeAsync(
        string templatePath, 
        IDatabaseSchema schema, 
        CodeGenerationOptions options)
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
            options = new CodeGenerationOptions();
        }

        _logger?.LogInformation("Previewing code using template {TemplatePath} for database {DatabaseName}", 
            templatePath, schema.DatabaseName);

        try
        {
            // Convert schema to template model
            _logger?.LogDebug("Converting schema to template model");
            var model = await _schemaModelAdapter.ConvertSchemaToTemplateModelAsync(schema);

            // Add options to the model
            model["Options"] = options;
            
            var preview = new Dictionary<string, string>();
            
            if (options.GeneratePerTable)
            {
                // Generate code for each table
                var tables = schema.Tables;
                _logger?.LogDebug("Previewing code for {Count} tables", tables.Count());
                
                foreach (var table in tables)
                {
                    var tableModel = await _schemaModelAdapter.ConvertTableToTemplateModelAsync(table);
                    tableModel["Schema"] = model; // Include the full schema as a property
                    tableModel["Options"] = options;
                    
                    var fileName = ResolveFileName(table.Name, options);
                    
                    _logger?.LogDebug("Processing template for table {TableName}", table.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, tableModel);
                    preview[fileName] = result;
                }
            }
            else if (options.GeneratePerView)
            {
                // Generate code for each view
                var views = schema.Views;
                _logger?.LogDebug("Previewing code for {Count} views", views.Count());
                
                foreach (var view in views)
                {
                    var viewModel = await _schemaModelAdapter.ConvertViewToTemplateModelAsync(view);
                    viewModel["Schema"] = model; // Include the full schema as a property
                    viewModel["Options"] = options;
                    
                    var fileName = ResolveFileName(view.Name, options);
                    
                    _logger?.LogDebug("Processing template for view {ViewName}", view.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, viewModel);
                    preview[fileName] = result;
                }
            }
            else if (options.GeneratePerStoredProcedure)
            {
                // Generate code for each stored procedure
                var storedProcedures = schema.StoredProcedures;
                _logger?.LogDebug("Previewing code for {Count} stored procedures", storedProcedures.Count());
                
                foreach (var storedProcedure in storedProcedures)
                {
                    var spModel = await _schemaModelAdapter.ConvertStoredProcedureToTemplateModelAsync(storedProcedure);
                    spModel["Schema"] = model; // Include the full schema as a property
                    spModel["Options"] = options;
                    
                    var fileName = ResolveFileName(storedProcedure.Name, options);
                    
                    _logger?.LogDebug("Processing template for stored procedure {ProcedureName}", storedProcedure.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, spModel);
                    preview[fileName] = result;
                }
            }
            else if (options.GeneratePerFunction)
            {
                // Generate code for each function
                var functions = schema.Functions;
                _logger?.LogDebug("Previewing code for {Count} functions", functions.Count());
                
                foreach (var function in functions)
                {
                    var functionModel = await _schemaModelAdapter.ConvertFunctionToTemplateModelAsync(function);
                    functionModel["Schema"] = model; // Include the full schema as a property
                    functionModel["Options"] = options;
                    
                    var fileName = ResolveFileName(function.Name, options);
                    
                    _logger?.LogDebug("Processing template for function {FunctionName}", function.Name);
                    
                    string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, functionModel);
                    preview[fileName] = result;
                }
            }
            else
            {
                // Generate a single file for the entire schema
                var fileName = ResolveFileName(options.SingleFileName ?? "output", options);
                
                _logger?.LogDebug("Processing template for entire schema");
                
                string result = await TemplateProcessor.ProcessTemplateFileAsync(templatePath, model);
                preview[fileName] = result;
            }
            
            _logger?.LogInformation("Generated preview for {Count} files", preview.Count);
            return preview;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error previewing code: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public void RegisterHelper(string name, Delegate helper)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Helper name cannot be null or empty.", nameof(name));
        }

        if (helper == null)
        {
            throw new ArgumentNullException(nameof(helper));
        }

        _logger?.LogDebug("Registering helper: {HelperName}", name);
        TemplateProcessor.RegisterHelper(name, helper);
    }

    /// <inheritdoc/>
    public void RegisterBlockHelper(string name, Delegate helper)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Block helper name cannot be null or empty.", nameof(name));
        }

        if (helper == null)
        {
            throw new ArgumentNullException(nameof(helper));
        }

        _logger?.LogDebug("Registering block helper: {HelperName}", name);
        TemplateProcessor.RegisterBlockHelper(name, helper);
    }
    
    /// <summary>
    /// Resolves the output file name based on the input name and options.
    /// </summary>
    /// <param name="baseName">The base name to use.</param>
    /// <param name="options">The code generation options.</param>
    /// <returns>The resolved file name.</returns>
    private string ResolveFileName(string baseName, CodeGenerationOptions options)
    {
        // Remove invalid characters
        string safeName = string.Join("", baseName.Split(Path.GetInvalidFileNameChars()));
        
        // Apply name transformation if specified
        if (!string.IsNullOrEmpty(options.FileNamePrefix))
        {
            safeName = options.FileNamePrefix + safeName;
        }
        
        if (!string.IsNullOrEmpty(options.FileNameSuffix))
        {
            safeName = safeName + options.FileNameSuffix;
        }
        
        // Add extension if not already present
        if (!string.IsNullOrEmpty(options.FileExtension))
        {
            string extension = options.FileExtension.StartsWith(".") 
                ? options.FileExtension 
                : "." + options.FileExtension;
                
            if (!safeName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                safeName += extension;
            }
        }
        
        return safeName;
    }
}
