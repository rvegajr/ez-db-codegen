namespace EzDbCodeGen.CodeGen.Interfaces;

using EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines an interface for generating documentation from database schemas and templates.
/// </summary>
public interface IDocumentationGenerator
{
    /// <summary>
    /// Gets or sets the code generation configuration to use for documentation generation.
    /// </summary>
    ICodeGenerationConfiguration Configuration { get; set; }
    
    /// <summary>
    /// Gets or sets the template processor to use for documentation generation.
    /// </summary>
    ITemplateProcessor TemplateProcessor { get; set; }
    
    /// <summary>
    /// Gets or sets the file output manager to use for writing generated documentation.
    /// </summary>
    IFileOutputManager FileOutputManager { get; set; }
    
    /// <summary>
    /// Generates schema documentation from a database schema.
    /// </summary>
    /// <param name="schema">The database schema to generate documentation for.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateSchemaDocumentation(IDatabaseSchema schema);
    
    /// <summary>
    /// Generates template documentation from template metadata.
    /// </summary>
    /// <param name="templateRepository">The template repository to generate documentation for.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateTemplateDocumentation(ITemplateRepository templateRepository);
    
    /// <summary>
    /// Generates API documentation from a model.
    /// </summary>
    /// <param name="model">The model to generate API documentation for.</param>
    /// <param name="outputDirectory">The directory to write the documentation to.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateApiDocumentation(object model, string outputDirectory);
    
    /// <summary>
    /// Generates relationship documentation from a database schema.
    /// </summary>
    /// <param name="schema">The database schema to generate relationship documentation for.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateRelationshipDocumentation(IDatabaseSchema schema);
    
    /// <summary>
    /// Generates documentation for a specific entity.
    /// </summary>
    /// <param name="entity">The entity to generate documentation for.</param>
    /// <param name="schema">The database schema containing the entity.</param>
    /// <returns>The generation result.</returns>
    IGenerationResult GenerateEntityDocumentation(object entity, IDatabaseSchema schema);
    
    /// <summary>
    /// Extracts documentation metadata from a database schema.
    /// </summary>
    /// <param name="schema">The database schema to extract metadata from.</param>
    /// <returns>A dictionary of metadata extracted from the schema.</returns>
    IDictionary<string, object> ExtractDocumentationMetadata(IDatabaseSchema schema);
    
    /// <summary>
    /// Gets all available documentation template names.
    /// </summary>
    /// <returns>A collection of available documentation template names.</returns>
    IReadOnlyCollection<string> GetDocumentationTemplateNames();
}
