namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents options for code generation.
/// </summary>
public class CodeGenerationOptions
{
    /// <summary>
    /// Gets or sets the namespace to use for generated code.
    /// </summary>
    public string Namespace { get; set; } = "Generated";

    /// <summary>
    /// Gets or sets the layout template path to use for all generated files.
    /// </summary>
    public string? LayoutTemplatePath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include timestamps in generated files.
    /// </summary>
    public bool IncludeTimestamps { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to generate files for empty objects.
    /// </summary>
    public bool GenerateEmptyObjects { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files.
    /// </summary>
    public bool OverwriteExistingFiles { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to include schema information in generated files.
    /// </summary>
    public bool IncludeSchemaInformation { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to include relationships in generated files.
    /// </summary>
    public bool IncludeRelationships { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to detect inheritance relationships.
    /// </summary>
    public bool DetectInheritance { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to track file dependencies.
    /// </summary>
    public bool TrackDependencies { get; set; } = true;

    /// <summary>
    /// Gets or sets a list of tables to exclude from code generation.
    /// </summary>
    public ICollection<string> ExcludedTables { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a list of schemas to exclude from code generation.
    /// </summary>
    public ICollection<string> ExcludedSchemas { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a list of file extensions to generate.
    /// </summary>
    public ICollection<string> FileExtensions { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets additional configuration parameters for code generation.
    /// </summary>
    public IDictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the naming convention for class names.
    /// </summary>
    public NamingConvention ClassNamingConvention { get; set; } = NamingConvention.PascalCase;

    /// <summary>
    /// Gets or sets the naming convention for property names.
    /// </summary>
    public NamingConvention PropertyNamingConvention { get; set; } = NamingConvention.PascalCase;

    /// <summary>
    /// Gets or sets the naming convention for field names.
    /// </summary>
    public NamingConvention FieldNamingConvention { get; set; } = NamingConvention.CamelCase;
}
