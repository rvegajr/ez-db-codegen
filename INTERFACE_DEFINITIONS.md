# EzDbCodeGen NG Core Interface Definitions

These interface definitions provide a starting point for implementation. They encapsulate the core functionality required for each component.

## Schema Analysis Interfaces

```csharp
namespace EzDbCodeGen.Schema
{
    /// <summary>
    /// Represents a database schema provider that extracts schema information from a specific database type.
    /// </summary>
    public interface IDatabaseSchemaProvider
    {
        /// <summary>
        /// Gets the database schema asynchronously using the provided connection string and options.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        /// <param name="options">Options for schema extraction.</param>
        /// <returns>A database schema.</returns>
        Task<IDatabaseSchema> GetSchemaAsync(string connectionString, SchemaProviderOptions options);
        
        /// <summary>
        /// Tests the connection to the database.
        /// </summary>
        /// <param name="connectionString">The connection string to test.</param>
        /// <returns>True if the connection is successful, false otherwise.</returns>
        Task<bool> TestConnectionAsync(string connectionString);
        
        /// <summary>
        /// Gets the provider type.
        /// </summary>
        string ProviderType { get; }
    }

    /// <summary>
    /// Represents a database schema with tables, views, stored procedures, and functions.
    /// </summary>
    public interface IDatabaseSchema
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the tables in the database.
        /// </summary>
        IReadOnlyCollection<ITable> Tables { get; }
        
        /// <summary>
        /// Gets the views in the database.
        /// </summary>
        IReadOnlyCollection<IView> Views { get; }
        
        /// <summary>
        /// Gets the stored procedures in the database.
        /// </summary>
        IReadOnlyCollection<IStoredProcedure> StoredProcedures { get; }
        
        /// <summary>
        /// Gets the functions in the database.
        /// </summary>
        IReadOnlyCollection<IFunction> Functions { get; }
    }

    /// <summary>
    /// Represents a database table with columns, keys, and constraints.
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the schema of the table.
        /// </summary>
        string Schema { get; }
        
        /// <summary>
        /// Gets the columns in the table.
        /// </summary>
        IReadOnlyCollection<IColumn> Columns { get; }
        
        /// <summary>
        /// Gets the primary key of the table.
        /// </summary>
        IKey? PrimaryKey { get; }
        
        /// <summary>
        /// Gets the foreign keys in the table.
        /// </summary>
        IReadOnlyCollection<IForeignKey> ForeignKeys { get; }
        
        /// <summary>
        /// Gets the indexes in the table.
        /// </summary>
        IReadOnlyCollection<IIndex> Indexes { get; }
        
        /// <summary>
        /// Gets the unique constraints in the table.
        /// </summary>
        IReadOnlyCollection<IUniqueConstraint> UniqueConstraints { get; }
        
        /// <summary>
        /// Gets a value indicating whether the table is a temporal table.
        /// </summary>
        bool IsTemporal { get; }
        
        /// <summary>
        /// Gets the history table name if this is a temporal table.
        /// </summary>
        string? HistoryTableName { get; }
    }

    /// <summary>
    /// Represents a column in a database table.
    /// </summary>
    public interface IColumn
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the ordinal position of the column in the table.
        /// </summary>
        int OrdinalPosition { get; }
        
        /// <summary>
        /// Gets the data type of the column.
        /// </summary>
        string DataType { get; }
        
        /// <summary>
        /// Gets a value indicating whether the column allows null values.
        /// </summary>
        bool IsNullable { get; }
        
        /// <summary>
        /// Gets the maximum length of the column.
        /// </summary>
        int? MaxLength { get; }
        
        /// <summary>
        /// Gets the precision of the column.
        /// </summary>
        int? Precision { get; }
        
        /// <summary>
        /// Gets the scale of the column.
        /// </summary>
        int? Scale { get; }
        
        /// <summary>
        /// Gets a value indicating whether the column is an identity column.
        /// </summary>
        bool IsIdentity { get; }
        
        /// <summary>
        /// Gets a value indicating whether the column is computed.
        /// </summary>
        bool IsComputed { get; }
        
        /// <summary>
        /// Gets the default value of the column.
        /// </summary>
        string? DefaultValue { get; }
        
        /// <summary>
        /// Gets the table that the column belongs to.
        /// </summary>
        ITable Table { get; }
    }

    /// <summary>
    /// Represents a relationship detector that analyzes a database schema for relationships.
    /// </summary>
    public interface IRelationshipDetector
    {
        /// <summary>
        /// Detects relationships in the database schema.
        /// </summary>
        /// <param name="schema">The database schema to analyze.</param>
        /// <param name="options">Options for relationship detection.</param>
        /// <returns>A collection of relationships.</returns>
        IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
    }

    /// <summary>
    /// Represents a relationship between database tables.
    /// </summary>
    public interface IRelationship
    {
        /// <summary>
        /// Gets the source table of the relationship.
        /// </summary>
        ITable SourceTable { get; }
        
        /// <summary>
        /// Gets the target table of the relationship.
        /// </summary>
        ITable TargetTable { get; }
        
        /// <summary>
        /// Gets the type of the relationship.
        /// </summary>
        RelationshipType Type { get; }
        
        /// <summary>
        /// Gets the foreign key that defines the relationship.
        /// </summary>
        IForeignKey? ForeignKey { get; }
        
        /// <summary>
        /// Gets the join table for many-to-many relationships.
        /// </summary>
        ITable? JoinTable { get; }
        
        /// <summary>
        /// Gets the source navigation property name.
        /// </summary>
        string SourceNavigationPropertyName { get; }
        
        /// <summary>
        /// Gets the target navigation property name.
        /// </summary>
        string TargetNavigationPropertyName { get; }
        
        /// <summary>
        /// Gets a value indicating whether the source navigation property is a collection.
        /// </summary>
        bool IsSourceCollection { get; }
        
        /// <summary>
        /// Gets a value indicating whether the target navigation property is a collection.
        /// </summary>
        bool IsTargetCollection { get; }
    }
}
```

## Type Mapping Interfaces

```csharp
namespace EzDbCodeGen.TypeMapping
{
    /// <summary>
    /// Represents a type mapper that maps database types to programming language types.
    /// </summary>
    public interface ITypeMapper
    {
        /// <summary>
        /// Maps a database column to a programming language type.
        /// </summary>
        /// <param name="column">The column to map.</param>
        /// <param name="options">Options for type mapping.</param>
        /// <returns>The mapped type name.</returns>
        string MapType(IColumn column, TypeMappingOptions options);
        
        /// <summary>
        /// Gets the default value for a database column in the target language.
        /// </summary>
        /// <param name="column">The column to get the default value for.</param>
        /// <param name="options">Options for type mapping.</param>
        /// <returns>The default value.</returns>
        string GetDefaultValue(IColumn column, TypeMappingOptions options);
        
        /// <summary>
        /// Determines whether a column should be nullable in the target language.
        /// </summary>
        /// <param name="column">The column to check.</param>
        /// <param name="options">Options for type mapping.</param>
        /// <returns>True if the column should be nullable, false otherwise.</returns>
        bool IsNullable(IColumn column, TypeMappingOptions options);
        
        /// <summary>
        /// Gets the target language for the type mapper.
        /// </summary>
        string Language { get; }
    }

    /// <summary>
    /// Represents a provider of type maps for different programming languages.
    /// </summary>
    public interface ITypeMapProvider
    {
        /// <summary>
        /// Gets a type map for a specific language.
        /// </summary>
        /// <param name="language">The target language.</param>
        /// <returns>A type map for the language.</returns>
        ITypeMap GetTypeMap(string language);
        
        /// <summary>
        /// Registers a type map for a specific language.
        /// </summary>
        /// <param name="language">The target language.</param>
        /// <param name="typeMap">The type map to register.</param>
        void RegisterTypeMap(string language, ITypeMap typeMap);
    }

    /// <summary>
    /// Represents a type map that maps database types to programming language types.
    /// </summary>
    public interface ITypeMap
    {
        /// <summary>
        /// Maps a database type to a programming language type.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The programming language type.</returns>
        string MapDatabaseType(string databaseType, int? maxLength, int? precision, int? scale);
        
        /// <summary>
        /// Gets the nullability syntax for a type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="isNullable">Whether the type is nullable.</param>
        /// <returns>The nullability syntax.</returns>
        string GetNullabilitySyntax(string typeName, bool isNullable);
        
        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="isNullable">Whether the type is nullable.</param>
        /// <returns>The default value.</returns>
        string GetDefaultValue(string typeName, bool isNullable);
        
        /// <summary>
        /// Gets the language of the type map.
        /// </summary>
        string Language { get; }
    }
}
```

## Template Engine Interfaces

```csharp
namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Represents a template engine that compiles and renders templates.
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Registers a helper function with the template engine.
        /// </summary>
        /// <param name="name">The name of the helper.</param>
        /// <param name="helper">The helper function.</param>
        void RegisterHelper(string name, Delegate helper);
        
        /// <summary>
        /// Registers a block helper with the template engine.
        /// </summary>
        /// <param name="name">The name of the helper.</param>
        /// <param name="helper">The helper function.</param>
        void RegisterBlockHelper(string name, Delegate helper);
        
        /// <summary>
        /// Registers a partial template with the template engine.
        /// </summary>
        /// <param name="name">The name of the partial.</param>
        /// <param name="template">The partial template.</param>
        void RegisterPartial(string name, string template);
        
        /// <summary>
        /// Compiles and renders a template with the provided data.
        /// </summary>
        /// <param name="template">The template to compile.</param>
        /// <param name="data">The data to render the template with.</param>
        /// <returns>The rendered template.</returns>
        string Compile(string template, object data);
    }

    /// <summary>
    /// Represents a template processor that processes templates with schema data.
    /// </summary>
    public interface ITemplateProcessor
    {
        /// <summary>
        /// Processes a template with the provided data.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="data">The data to process the template with.</param>
        /// <returns>A dictionary of file paths and content.</returns>
        Task<IDictionary<string, string>> ProcessAsync(string template, object data);
        
        /// <summary>
        /// Processes a template file with the provided data.
        /// </summary>
        /// <param name="templatePath">The path to the template file.</param>
        /// <param name="data">The data to process the template with.</param>
        /// <returns>A dictionary of file paths and content.</returns>
        Task<IDictionary<string, string>> ProcessFileAsync(string templatePath, object data);
    }

    /// <summary>
    /// Represents a template processor that performs differential processing.
    /// </summary>
    public interface IDifferentialTemplateProcessor : ITemplateProcessor
    {
        /// <summary>
        /// Processes a template with new and old data, generating only changed output.
        /// </summary>
        /// <param name="template">The template to process.</param>
        /// <param name="newData">The new data.</param>
        /// <param name="oldData">The old data.</param>
        /// <returns>A dictionary of file paths and content.</returns>
        Task<IDictionary<string, string>> ProcessIncrementalAsync(string template, object newData, object oldData);
    }
}
```

## Code Generation Interfaces

```csharp
namespace EzDbCodeGen.CodeGeneration
{
    /// <summary>
    /// Represents a code generator that generates code from a database schema.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Generates code from a database schema.
        /// </summary>
        /// <param name="options">Options for code generation.</param>
        /// <returns>A result containing generated files and statistics.</returns>
        Task<CodeGenerationResult> GenerateAsync(CodeGenerationOptions options);
    }

    /// <summary>
    /// Represents a pipeline for code generation.
    /// </summary>
    public interface ICodeGenerationPipeline
    {
        /// <summary>
        /// Configures the pipeline with a schema analyzer.
        /// </summary>
        /// <param name="analyzer">The schema analyzer to use.</param>
        /// <returns>The pipeline for chaining.</returns>
        ICodeGenerationPipeline WithSchemaAnalyzer(ISchemaAnalyzer analyzer);
        
        /// <summary>
        /// Configures the pipeline with a template processor.
        /// </summary>
        /// <param name="processor">The template processor to use.</param>
        /// <returns>The pipeline for chaining.</returns>
        ICodeGenerationPipeline WithTemplateProcessor(ITemplateProcessor processor);
        
        /// <summary>
        /// Configures the pipeline with a type mapper.
        /// </summary>
        /// <param name="mapper">The type mapper to use.</param>
        /// <returns>The pipeline for chaining.</returns>
        ICodeGenerationPipeline WithTypeMapper(ITypeMapper mapper);
        
        /// <summary>
        /// Executes the pipeline with the provided options.
        /// </summary>
        /// <param name="options">Options for code generation.</param>
        /// <returns>A result containing generated files and statistics.</returns>
        Task<CodeGenerationResult> ExecuteAsync(CodeGenerationOptions options);
    }

    /// <summary>
    /// Represents an output provider that writes generated code to a destination.
    /// </summary>
    public interface IOutputProvider
    {
        /// <summary>
        /// Writes content to a file path.
        /// </summary>
        /// <param name="path">The path to write to.</param>
        /// <param name="content">The content to write.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task WriteAsync(string path, string content);
        
        /// <summary>
        /// Gets existing paths in a base path.
        /// </summary>
        /// <param name="basePath">The base path to search in.</param>
        /// <returns>A collection of existing paths.</returns>
        Task<IEnumerable<string>> GetExistingPathsAsync(string basePath);
        
        /// <summary>
        /// Deletes a file at the specified path.
        /// </summary>
        /// <param name="path">The path to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteAsync(string path);
    }
}
```
