namespace EzDbCodeGen.TemplateEngine.Testing;

/// <summary>
/// Options for template testing.
/// </summary>
public class TemplateTestOptions
{
    /// <summary>
    /// Gets or sets the target table to test against. If specified, will use only this table's model for testing.
    /// </summary>
    public string? TargetTable { get; set; }
    
    /// <summary>
    /// Gets or sets the target view to test against. If specified, will use only this view's model for testing.
    /// </summary>
    public string? TargetView { get; set; }
    
    /// <summary>
    /// Gets or sets the target language for code generation.
    /// </summary>
    public string TargetLanguage { get; set; } = "CSharp";
    
    /// <summary>
    /// Gets or sets a value indicating whether to validate the output.
    /// </summary>
    public bool ValidateOutput { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to check for unprocessed placeholders in the output.
    /// </summary>
    public bool CheckForPlaceholders { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to check for basic syntax errors in the target language.
    /// </summary>
    public bool CheckSyntaxForLanguage { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include comments in the generated code.
    /// </summary>
    public bool IncludeComments { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect and include relationship information.
    /// </summary>
    public bool DetectRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate navigation properties for relationships.
    /// </summary>
    public bool GenerateNavigationProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the file pattern for finding template files in a directory.
    /// </summary>
    public string TemplateFilePattern { get; set; } = "*.hbs";
}
