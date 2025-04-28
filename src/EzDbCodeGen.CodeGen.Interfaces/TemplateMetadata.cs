namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents metadata extracted from a template.
/// </summary>
public class TemplateMetadata
{
    /// <summary>
    /// Gets or sets the name of the template.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the description of the template.
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the author of the template.
    /// </summary>
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the version of the template.
    /// </summary>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the date the template was created.
    /// </summary>
    public DateTime? Created { get; set; }
    
    /// <summary>
    /// Gets or sets the date the template was last modified.
    /// </summary>
    public DateTime? LastModified { get; set; }
    
    /// <summary>
    /// Gets or sets the category of the template.
    /// </summary>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the tags associated with the template.
    /// </summary>
    public ICollection<string> Tags { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the target output path for the template.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the expected model type for the template.
    /// </summary>
    public string ModelType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the required helpers for the template.
    /// </summary>
    public ICollection<string> RequiredHelpers { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the included partials in the template.
    /// </summary>
    public ICollection<string> IncludedPartials { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the model properties referenced in the template.
    /// </summary>
    public ICollection<string> ModelReferences { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the parameters required by the template.
    /// </summary>
    public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    
    /// <summary>
    /// Gets or sets the templates that this template depends on.
    /// </summary>
    public ICollection<string> Dependencies { get; set; } = new List<string>();
    
    /// <summary>
    /// Gets or sets the target language of the template output.
    /// </summary>
    public string TargetLanguage { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the template engine type required to process this template.
    /// </summary>
    public string TemplateEngineType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional metadata for the template.
    /// </summary>
    public IDictionary<string, object> AdditionalMetadata { get; set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// Gets or sets a value indicating whether the template is a partial template.
    /// </summary>
    public bool IsPartial { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the template is a layout template.
    /// </summary>
    public bool IsLayout { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the template generates multiple files.
    /// </summary>
    public bool GeneratesMultipleFiles { get; set; }
    
    /// <summary>
    /// Gets or sets examples of using the template.
    /// </summary>
    public ICollection<string> Examples { get; set; } = new List<string>();
}
