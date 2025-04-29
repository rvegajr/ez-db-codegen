namespace EzDbCodeGen.Core.CodeGeneration;

using EzDbCodeGen.Core.TypeMapping;
using EzDbCodeGen.Core.Schema;

/// <summary>
/// Represents options for code generation.
/// </summary>
public class CodeGenerationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to generate one file per entity.
    /// </summary>
    public bool OneFilePerEntity { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the namespace for the generated code.
    /// </summary>
    public string Namespace { get; set; } = "Generated";
    
    /// <summary>
    /// Gets or sets the target language for code generation.
    /// </summary>
    public string Language { get; set; } = "CSharp";
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate model validation attributes.
    /// </summary>
    public bool GenerateModelValidation { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate data annotations for EF Core.
    /// </summary>
    public bool UseDataAnnotations { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate Fluent API configuration for EF Core.
    /// </summary>
    public bool UseFluentApi { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate comments.
    /// </summary>
    public bool GenerateComments { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate nullable reference types.
    /// </summary>
    public bool UseNullableReferenceTypes { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate XML documentation.
    /// </summary>
    public bool GenerateXmlDocumentation { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to apply code formatting.
    /// </summary>
    public bool ApplyCodeFormatting { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate navigation properties.
    /// </summary>
    public bool GenerateNavigationProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate constructors.
    /// </summary>
    public bool GenerateConstructors { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate equality members.
    /// </summary>
    public bool GenerateEqualityMembers { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate DbContext.
    /// </summary>
    public bool GenerateDbContext { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the file extension for generated files.
    /// </summary>
    public string FileExtension { get; set; } = ".cs";
    
    /// <summary>
    /// Gets or sets a value indicating whether to use Pascal case for table names.
    /// </summary>
    public bool UsePascalCaseForTableNames { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use Pascal case for column names.
    /// </summary>
    public bool UsePascalCaseForColumnNames { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to use Pascal case for navigation properties.
    /// </summary>
    public bool UsePascalCaseForNavigationProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to pluralize navigation properties for collections.
    /// </summary>
    public bool PluralizeNavigationCollectionProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate record types instead of classes.
    /// </summary>
    public bool UseRecordTypes { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include foreign key properties in the model.
    /// </summary>
    public bool IncludeForeignKeyProperties { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to include table schema in entity names.
    /// </summary>
    public bool IncludeSchemaInEntityNames { get; set; } = false;
    
    /// <summary>
    /// Gets or sets a value indicating whether to generate separate mapping configuration for EF Core.
    /// </summary>
    public bool GenerateSeparateMappingConfiguration { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether to skip code generation for junction tables.
    /// </summary>
    public bool SkipJunctionTables { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the type mapping options.
    /// </summary>
    public TypeMappingOptions? TypeMappingOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the relationship detection options.
    /// </summary>
    public RelationshipDetectionOptions? RelationshipDetectionOptions { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether to use partial classes.
    /// </summary>
    public bool UsePartialClasses { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a collection of template variables.
    /// </summary>
    public IDictionary<string, object> TemplateVariables { get; set; } = new Dictionary<string, object>();
}
