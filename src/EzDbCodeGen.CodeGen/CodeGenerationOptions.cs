using System.Collections.Generic;

namespace EzDbCodeGen.CodeGen;

/// <summary>
/// Options for code generation.
/// </summary>
public class CodeGenerationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to generate one file per table.
    /// </summary>
    public bool GeneratePerTable { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate one file per view.
    /// </summary>
    public bool GeneratePerView { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate one file per stored procedure.
    /// </summary>
    public bool GeneratePerStoredProcedure { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate one file per function.
    /// </summary>
    public bool GeneratePerFunction { get; set; }
    
    /// <summary>
    /// Gets or sets the file name to use when generating a single file for the entire schema.
    /// This is only used when not generating per-table, per-view, per-stored procedure, or per-function.
    /// </summary>
    public string? SingleFileName { get; set; }
    
    /// <summary>
    /// Gets or sets the prefix to add to generated file names.
    /// </summary>
    public string? FileNamePrefix { get; set; }
    
    /// <summary>
    /// Gets or sets the suffix to add to generated file names.
    /// </summary>
    public string? FileNameSuffix { get; set; }
    
    /// <summary>
    /// Gets or sets the file extension to use for generated files.
    /// </summary>
    public string? FileExtension { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to create subdirectories for generated files.
    /// </summary>
    public bool CreateSubdirectories { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing files.
    /// </summary>
    public bool OverwriteExistingFiles { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to clean the output directory before generating files.
    /// </summary>
    public bool CleanOutputDirectory { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate code only for filtered objects.
    /// </summary>
    public bool UseFiltering { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the name of the schema filter to use.
    /// </summary>
    public string? SchemaFilterName { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect and generate code for relationships.
    /// </summary>
    public bool DetectRelationships { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate navigation properties for relationships.
    /// </summary>
    public bool GenerateNavigationProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to detect and generate code for inheritance patterns.
    /// </summary>
    public bool DetectInheritance { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the target language for generated code.
    /// </summary>
    public string? TargetLanguage { get; set; } = "CSharp";
    
    /// <summary>
    /// Gets or sets additional custom properties to pass to templates.
    /// </summary>
    public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
    
    /// <summary>
    /// Gets or sets naming conventions for the generated code.
    /// </summary>
    public NamingConventions NamingConventions { get; set; } = new NamingConventions();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenerationOptions"/> class.
    /// </summary>
    public CodeGenerationOptions()
    {
        // Set default file extension based on target language
        FileExtension = ".cs";
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="CodeGenerationOptions"/> class for generating C# code.
    /// </summary>
    /// <returns>A new instance of the <see cref="CodeGenerationOptions"/> class for generating C# code.</returns>
    public static CodeGenerationOptions ForCSharp()
    {
        return new CodeGenerationOptions
        {
            TargetLanguage = "CSharp",
            FileExtension = ".cs",
            NamingConventions = NamingConventions.CSharp()
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="CodeGenerationOptions"/> class for generating TypeScript code.
    /// </summary>
    /// <returns>A new instance of the <see cref="CodeGenerationOptions"/> class for generating TypeScript code.</returns>
    public static CodeGenerationOptions ForTypeScript()
    {
        return new CodeGenerationOptions
        {
            TargetLanguage = "TypeScript",
            FileExtension = ".ts",
            NamingConventions = NamingConventions.TypeScript()
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="CodeGenerationOptions"/> class for generating Java code.
    /// </summary>
    /// <returns>A new instance of the <see cref="CodeGenerationOptions"/> class for generating Java code.</returns>
    public static CodeGenerationOptions ForJava()
    {
        return new CodeGenerationOptions
        {
            TargetLanguage = "Java",
            FileExtension = ".java",
            NamingConventions = NamingConventions.Java()
        };
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="CodeGenerationOptions"/> class for generating Python code.
    /// </summary>
    /// <returns>A new instance of the <see cref="CodeGenerationOptions"/> class for generating Python code.</returns>
    public static CodeGenerationOptions ForPython()
    {
        return new CodeGenerationOptions
        {
            TargetLanguage = "Python",
            FileExtension = ".py",
            NamingConventions = NamingConventions.Python()
        };
    }
}
