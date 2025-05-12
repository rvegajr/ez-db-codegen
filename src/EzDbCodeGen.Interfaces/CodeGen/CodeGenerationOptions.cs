using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.CodeGen
{
    /// <summary>
    /// Defines options for code generation.
    /// </summary>
    public class CodeGenerationOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to generate a separate file for each table.
        /// </summary>
        public bool GeneratePerTable { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to generate a separate file for each view.
        /// </summary>
        public bool GeneratePerView { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to generate a separate file for each stored procedure.
        /// </summary>
        public bool GeneratePerStoredProcedure { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to generate a separate file for each function.
        /// </summary>
        public bool GeneratePerFunction { get; set; } = false;

        /// <summary>
        /// Gets or sets the file name to use when generating a single file for the entire schema.
        /// </summary>
        public string SingleFileName { get; set; } = "output";

        /// <summary>
        /// Gets or sets the prefix to add to generated file names.
        /// </summary>
        public string FileNamePrefix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the suffix to add to generated file names.
        /// </summary>
        public string FileNameSuffix { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file extension to use for generated files.
        /// </summary>
        public string FileExtension { get; set; } = ".cs";

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite existing files.
        /// </summary>
        public bool OverwriteExistingFiles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to include the database name in the generated code.
        /// </summary>
        public bool IncludeDatabaseName { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include schema names in the generated code.
        /// </summary>
        public bool IncludeSchemaNames { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to include descriptions in the generated code.
        /// </summary>
        public bool IncludeDescriptions { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to generate nullable reference types.
        /// </summary>
        public bool UseNullableReferenceTypes { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use records instead of classes.
        /// </summary>
        public bool UseRecords { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to generate fluent API configuration.
        /// </summary>
        public bool GenerateFluentApi { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to generate data annotations.
        /// </summary>
        public bool GenerateDataAnnotations { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use namespaces per schema.
        /// </summary>
        public bool UseNamespacesPerSchema { get; set; } = false;

        /// <summary>
        /// Gets or sets the root namespace for generated code.
        /// </summary>
        public string RootNamespace { get; set; } = "EzDbCodeGen.Generated";

        /// <summary>
        /// Gets or sets a value indicating whether to generate code in dry run mode (no files written).
        /// </summary>
        public bool DryRun { get; set; } = false;

        /// <summary>
        /// Gets or sets a collection of tables to include in code generation (if empty, all tables are included).
        /// </summary>
        public ICollection<string> IncludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of tables to exclude from code generation.
        /// </summary>
        public ICollection<string> ExcludeTables { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of schemas to include in code generation (if empty, all schemas are included).
        /// </summary>
        public ICollection<string> IncludeSchemas { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of schemas to exclude from code generation.
        /// </summary>
        public ICollection<string> ExcludeSchemas { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to generate XML documentation.
        /// </summary>
        public bool GenerateXmlDocumentation { get; set; } = true;

        /// <summary>
        /// Gets or sets additional custom properties for template rendering.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new instance of CodeGenerationOptions with default settings.
        /// </summary>
        /// <returns>A CodeGenerationOptions instance with default settings.</returns>
        public static CodeGenerationOptions Default() => new CodeGenerationOptions();

        /// <summary>
        /// Creates a new instance of CodeGenerationOptions optimized for Entity Framework Core.
        /// </summary>
        /// <returns>A CodeGenerationOptions instance optimized for Entity Framework Core.</returns>
        public static CodeGenerationOptions ForEFCore() => new CodeGenerationOptions
        {
            GeneratePerTable = true,
            IncludeDescriptions = true,
            UseNullableReferenceTypes = true,
            GenerateFluentApi = true,
            GenerateDataAnnotations = false,
            GenerateXmlDocumentation = true,
            FileExtension = ".cs",
            OverwriteExistingFiles = true
        };

        /// <summary>
        /// Creates a new instance of CodeGenerationOptions optimized for Dapper.
        /// </summary>
        /// <returns>A CodeGenerationOptions instance optimized for Dapper.</returns>
        public static CodeGenerationOptions ForDapper() => new CodeGenerationOptions
        {
            GeneratePerTable = true,
            IncludeDescriptions = true,
            UseNullableReferenceTypes = true,
            GenerateFluentApi = false,
            GenerateDataAnnotations = true,
            GenerateXmlDocumentation = true,
            FileExtension = ".cs",
            OverwriteExistingFiles = true
        };
    }
}
