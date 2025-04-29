using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.CodeGen;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema.Filters;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.TypeMapping;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Cli.Adapters;

/// <summary>
/// Adapter that implements the legacy Core.CodeGeneration interfaces using the new architecture.
/// </summary>
public class CodeGeneratorAdapter : Core.CodeGeneration.ICodeGenerator
{
    private readonly CodeGen.ICodeGenerator _codeGenerator;
    private readonly IDataTypeMap _dataTypeMap;
    private readonly ISchemaModelAdapter _schemaModelAdapter;
    private readonly ILogger<CodeGeneratorAdapter>? _logger;
    private readonly PatternSchemaFilter _schemaFilter;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGeneratorAdapter"/> class.
    /// </summary>
    /// <param name="codeGenerator">The new code generator to adapt.</param>
    /// <param name="dataTypeMap">The data type mapper.</param>
    /// <param name="schemaModelAdapter">The schema model adapter.</param>
    /// <param name="logger">The logger instance.</param>
    public CodeGeneratorAdapter(
        CodeGen.ICodeGenerator codeGenerator,
        IDataTypeMap dataTypeMap,
        ISchemaModelAdapter schemaModelAdapter,
        ILogger<CodeGeneratorAdapter>? logger = null)
    {
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _dataTypeMap = dataTypeMap ?? throw new ArgumentNullException(nameof(dataTypeMap));
        _schemaModelAdapter = schemaModelAdapter ?? throw new ArgumentNullException(nameof(schemaModelAdapter));
        _logger = logger;
        _schemaFilter = new PatternSchemaFilter("DefaultFilter", "Default schema filter", logger as ILogger);
    }
    
    /// <inheritdoc/>
    public async Task GenerateAsync(
        string templatePath,
        string outputPath,
        string namespaceName,
        string language,
        bool useDataAnnotations,
        bool useFluentApi,
        bool generateNavigationProperties)
    {
        try
        {
            _logger?.LogInformation("Generating code using adapter");
            
            // Convert options to the new format
            var options = new CodeGenerationOptions
            {
                TargetLanguage = language,
                DetectRelationships = true,
                GenerateNavigationProperties = generateNavigationProperties,
                UseFiltering = true,
                OverwriteExistingFiles = true,
                FileExtension = GetFileExtensionFromLanguage(language)
            };
            
            options.CustomProperties["Namespace"] = namespaceName;
            options.CustomProperties["UseDataAnnotations"] = useDataAnnotations;
            options.CustomProperties["UseFluentApi"] = useFluentApi;
            
            // Use our new code generator with the legacy options
            var schema = await GetCurrentSchemaAsync();
            
            var generatedFiles = await _codeGenerator.GenerateCodeAsync(
                templatePath, 
                schema, 
                outputPath, 
                options);
            
            _logger?.LogInformation("Generated {Count} files", generatedFiles.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error generating code: {ErrorMessage}", ex.Message);
            throw new CodeGenerationException("Error generating code", ex);
        }
    }
    
    /// <summary>
    /// Gets the file extension for the specified language.
    /// </summary>
    private string GetFileExtensionFromLanguage(string language)
    {
        return language?.ToLower() switch
        {
            "csharp" => ".cs",
            "typescript" => ".ts",
            "java" => ".java",
            "python" => ".py",
            _ => ".cs" // Default to C#
        };
    }
    
    /// <summary>
    /// Gets the current database schema.
    /// </summary>
    private async Task<IDatabaseSchema> GetCurrentSchemaAsync()
    {
        // We don't actually have access to the schema provider here,
        // so this is a placeholder that would be replaced with the actual schema
        // in a real implementation.
        throw new NotImplementedException(
            "This adapter requires the schema to be provided externally. " +
            "Use the SchemaProviderAdapter to get the schema first, then pass it to this adapter.");
    }
}

/// <summary>
/// Adapter factory for creating code generator adapters.
/// </summary>
public class CodeGeneratorAdapterFactory : Core.CodeGeneration.ICodeGeneratorFactory
{
    private readonly CodeGen.ICodeGenerator _codeGenerator;
    private readonly IDataTypeMap _dataTypeMap;
    private readonly ISchemaModelAdapter _schemaModelAdapter;
    private readonly ILoggerFactory? _loggerFactory;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGeneratorAdapterFactory"/> class.
    /// </summary>
    /// <param name="codeGenerator">The new code generator to adapt.</param>
    /// <param name="dataTypeMap">The data type mapper.</param>
    /// <param name="schemaModelAdapter">The schema model adapter.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public CodeGeneratorAdapterFactory(
        CodeGen.ICodeGenerator codeGenerator,
        IDataTypeMap dataTypeMap,
        ISchemaModelAdapter schemaModelAdapter,
        ILoggerFactory? loggerFactory = null)
    {
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _dataTypeMap = dataTypeMap ?? throw new ArgumentNullException(nameof(dataTypeMap));
        _schemaModelAdapter = schemaModelAdapter ?? throw new ArgumentNullException(nameof(schemaModelAdapter));
        _loggerFactory = loggerFactory;
    }
    
    /// <inheritdoc/>
    public Core.CodeGeneration.ICodeGenerator CreateCodeGenerator()
    {
        return new CodeGeneratorAdapter(
            _codeGenerator, 
            _dataTypeMap, 
            _schemaModelAdapter, 
            _loggerFactory?.CreateLogger<CodeGeneratorAdapter>());
    }
}

/// <summary>
/// Adapter that bridges the Core.Logging and Microsoft.Extensions.Logging interfaces.
/// </summary>
public class LoggerAdapter : Core.Logging.ILogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggerAdapter"/> class.
    /// </summary>
    /// <param name="logger">The Microsoft logger to adapt.</param>
    public LoggerAdapter(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc/>
    public void Debug(string message)
    {
        _logger.LogDebug(message);
    }
    
    /// <inheritdoc/>
    public void Info(string message)
    {
        _logger.LogInformation(message);
    }
    
    /// <inheritdoc/>
    public void Warning(string message)
    {
        _logger.LogWarning(message);
    }
    
    /// <inheritdoc/>
    public void Error(string message)
    {
        _logger.LogError(message);
    }
}

/// <summary>
/// Adapter for the relationship detector.
/// </summary>
public class RelationshipDetectorAdapter : Core.Schema.IRelationshipDetector
{
    private readonly Schema.Analysis.RelationshipAnalyzer _relationshipAnalyzer;
    private readonly ILogger<RelationshipDetectorAdapter>? _logger;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipDetectorAdapter"/> class.
    /// </summary>
    /// <param name="relationshipAnalyzer">The relationship analyzer to adapt.</param>
    /// <param name="logger">The logger instance.</param>
    public RelationshipDetectorAdapter(
        Schema.Analysis.RelationshipAnalyzer relationshipAnalyzer,
        ILogger<RelationshipDetectorAdapter>? logger = null)
    {
        _relationshipAnalyzer = relationshipAnalyzer ?? throw new ArgumentNullException(nameof(relationshipAnalyzer));
        _logger = logger;
    }
    
    /// <inheritdoc/>
    public Task<IEnumerable<Core.Schema.Relationship>> DetectRelationshipsAsync(Core.Schema.IDatabaseSchema schema)
    {
        // This would require adapting the schema object as well, which is a much larger task
        // For now, this is a placeholder
        throw new NotImplementedException(
            "This adapter requires a schema adapter, which is not implemented yet.");
    }
}

/// <summary>
/// Factory for creating relationship detector adapters.
/// </summary>
public class RelationshipDetectorAdapterFactory : Core.Schema.IRelationshipDetectorFactory
{
    private readonly Schema.Analysis.RelationshipAnalyzer _relationshipAnalyzer;
    private readonly ILoggerFactory? _loggerFactory;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipDetectorAdapterFactory"/> class.
    /// </summary>
    /// <param name="relationshipAnalyzer">The relationship analyzer to adapt.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public RelationshipDetectorAdapterFactory(
        Schema.Analysis.RelationshipAnalyzer relationshipAnalyzer,
        ILoggerFactory? loggerFactory = null)
    {
        _relationshipAnalyzer = relationshipAnalyzer ?? throw new ArgumentNullException(nameof(relationshipAnalyzer));
        _loggerFactory = loggerFactory;
    }
    
    /// <inheritdoc/>
    public Core.Schema.IRelationshipDetector CreateRelationshipDetector()
    {
        return new RelationshipDetectorAdapter(
            _relationshipAnalyzer,
            _loggerFactory?.CreateLogger<RelationshipDetectorAdapter>());
    }
}

/// <summary>
/// Exception thrown when code generation fails.
/// </summary>
public class CodeGenerationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenerationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CodeGenerationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
