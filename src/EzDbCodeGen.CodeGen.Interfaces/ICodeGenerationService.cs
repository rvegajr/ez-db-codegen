namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for a service that orchestrates the code generation process.
/// </summary>
public interface ICodeGenerationService
{
    /// <summary>
    /// Gets or sets the code generation configuration.
    /// </summary>
    ICodeGenerationConfiguration Configuration { get; set; }
    
    /// <summary>
    /// Gets or sets the template processor to use for generation.
    /// </summary>
    ITemplateProcessor TemplateProcessor { get; set; }
    
    /// <summary>
    /// Gets or sets the file output manager to use for writing generated files.
    /// </summary>
    IFileOutputManager FileOutputManager { get; set; }
    
    /// <summary>
    /// Gets or sets the database schema provider to use for accessing the database schema.
    /// </summary>
    IDatabaseSchemaProvider? SchemaProvider { get; set; }
    
    /// <summary>
    /// Generates code based on a database schema.
    /// </summary>
    /// <param name="schema">The database schema to generate code for.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateFromSchema(IDatabaseSchema schema);
    
    /// <summary>
    /// Generates code based on a database schema loaded from a provider.
    /// </summary>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateFromDatabaseProvider();
    
    /// <summary>
    /// Generates code based on a database schema loaded from a file.
    /// </summary>
    /// <param name="schemaFilePath">The path to the schema file.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateFromSchemaFile(string schemaFilePath);
    
    /// <summary>
    /// Generates code for a specific entity in the database schema.
    /// </summary>
    /// <param name="schema">The database schema containing the entity.</param>
    /// <param name="entityName">The name of the entity to generate code for.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateForEntity(IDatabaseSchema schema, string entityName);
    
    /// <summary>
    /// Generates code for a specific template using the provided model.
    /// </summary>
    /// <param name="templateName">The name of the template to use.</param>
    /// <param name="model">The model to use for generation.</param>
    /// <param name="outputPath">The path to write the output to, relative to the output directory.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateFromTemplate(string templateName, object model, string outputPath);
    
    /// <summary>
    /// Generates code for multiple templates using the provided model.
    /// </summary>
    /// <param name="templateNames">The names of the templates to use.</param>
    /// <param name="model">The model to use for generation.</param>
    /// <param name="outputPathFormat">The format string for output paths, where {0} is replaced with the template name.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateFromTemplates(IEnumerable<string> templateNames, object model, string outputPathFormat);
    
    /// <summary>
    /// Registers a template filter to be used during generation.
    /// </summary>
    /// <param name="filter">The filter to register.</param>
    void RegisterFilter(ITemplateFilter filter);
    
    /// <summary>
    /// Removes a template filter.
    /// </summary>
    /// <param name="filterName">The name of the filter to remove.</param>
    void RemoveFilter(string filterName);
    
    /// <summary>
    /// Clears all registered template filters.
    /// </summary>
    void ClearFilters();
    
    /// <summary>
    /// Gets all registered template filters.
    /// </summary>
    /// <returns>A collection of registered template filters.</returns>
    IReadOnlyCollection<ITemplateFilter> GetFilters();
    
    /// <summary>
    /// Validates the code generation configuration.
    /// </summary>
    /// <returns>A collection of validation errors, or an empty collection if the configuration is valid.</returns>
    IReadOnlyCollection<string> ValidateConfiguration();
}
