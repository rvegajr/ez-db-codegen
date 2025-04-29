namespace EzDbCodeGen.Core.CodeGeneration;

using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;

/// <summary>
/// Represents options for configuring a code generator.
/// </summary>
public class CodeGeneratorOptions
{
    /// <summary>
    /// Gets or sets the template engine to use.
    /// </summary>
    public ITemplateEngine? TemplateEngine { get; set; }
    
    /// <summary>
    /// Gets or sets the template processor to use.
    /// </summary>
    public ITemplateProcessor? TemplateProcessor { get; set; }
    
    /// <summary>
    /// Gets or sets the relationship detector to use.
    /// </summary>
    public IRelationshipDetector? RelationshipDetector { get; set; }
    
    /// <summary>
    /// Gets or sets the template engine options.
    /// </summary>
    public TemplateEngineOptions? TemplateEngineOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the template processor options.
    /// </summary>
    public TemplateProcessorOptions? TemplateProcessorOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the relationship detection options.
    /// </summary>
    public RelationshipDetectionOptions? RelationshipDetectionOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the code generation options.
    /// </summary>
    public CodeGenerationOptions? CodeGenerationOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the template engine type.
    /// </summary>
    public string TemplateEngineType { get; set; } = "Handlebars";
    
    /// <summary>
    /// Gets or sets the template processor type.
    /// </summary>
    public string TemplateProcessorType { get; set; } = "Standard";
    
    /// <summary>
    /// Gets or sets a value indicating whether to use the differential template processor.
    /// </summary>
    public bool UseDifferentialTemplateProcessor { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use filters.
    /// </summary>
    public bool UseFilters { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a collection of output filters.
    /// </summary>
    public IList<IOutputFilter> OutputFilters { get; set; } = new List<IOutputFilter>();
    
    /// <summary>
    /// Gets or sets a value indicating whether to register the schema as a global template variable.
    /// </summary>
    public bool RegisterSchemaAsGlobal { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a collection of template variables.
    /// </summary>
    public IDictionary<string, object> TemplateVariables { get; set; } = new Dictionary<string, object>();
}
