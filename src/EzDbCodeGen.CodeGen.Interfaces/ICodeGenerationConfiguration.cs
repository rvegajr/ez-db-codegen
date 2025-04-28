namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for configuring the code generation process.
/// </summary>
public interface ICodeGenerationConfiguration
{
    /// <summary>
    /// Gets or sets the output directory for generated files.
    /// </summary>
    string OutputDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets the template directory.
    /// </summary>
    string TemplateDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets the template engine type to use.
    /// </summary>
    TemplateEngineType TemplateEngineType { get; set; }
    
    /// <summary>
    /// Gets or sets the template engine options.
    /// </summary>
    TemplateEngineOptions TemplateEngineOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the naming convention to use.
    /// </summary>
    NamingConvention NamingConvention { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files.
    /// </summary>
    bool OverwriteExisting { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to include schema information in the generated code.
    /// </summary>
    bool IncludeSchemaInfo { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate separate files for each entity.
    /// </summary>
    bool GenerateSeparateFiles { get; set; }
    
    /// <summary>
    /// Gets or sets the file extension for generated code files.
    /// </summary>
    string FileExtension { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of template names to include in generation.
    /// </summary>
    ICollection<string> IncludeTemplates { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of template names to exclude from generation.
    /// </summary>
    ICollection<string> ExcludeTemplates { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of entity names to include in generation.
    /// </summary>
    ICollection<string> IncludeEntities { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of entity names to exclude from generation.
    /// </summary>
    ICollection<string> ExcludeEntities { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of schema names to include in generation.
    /// </summary>
    ICollection<string> IncludeSchemas { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of schema names to exclude from generation.
    /// </summary>
    ICollection<string> ExcludeSchemas { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of global parameters to pass to all templates.
    /// </summary>
    IDictionary<string, object> GlobalParameters { get; set; }
    
    /// <summary>
    /// Gets or sets the default namespace for generated code.
    /// </summary>
    string DefaultNamespace { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to clean the output directory before generation.
    /// </summary>
    bool CleanOutputDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate documentation.
    /// </summary>
    bool GenerateDocumentation { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to log detailed generation information.
    /// </summary>
    bool VerboseLogging { get; set; }
    
    /// <summary>
    /// Gets or sets a collection of metadata that can be used during generation.
    /// </summary>
    IDictionary<string, object> Metadata { get; set; }
    
    /// <summary>
    /// Gets or sets the schema provider options for database access.
    /// </summary>
    SchemaProviderOptions SchemaProviderOptions { get; set; }
    
    /// <summary>
    /// Creates a deep clone of this configuration.
    /// </summary>
    /// <returns>A new instance of the configuration with the same values.</returns>
    ICodeGenerationConfiguration Clone();
    
    /// <summary>
    /// Validates the configuration and returns any validation errors.
    /// </summary>
    /// <returns>A collection of validation errors, or an empty collection if the configuration is valid.</returns>
    IReadOnlyCollection<string> Validate();
}
