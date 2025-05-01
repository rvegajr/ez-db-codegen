# EzDbCodeGen NG Architecture

## System Overview

EzDbCodeGen NG is organized into distinct layers with clear responsibilities:

```
┌───────────────────┐     ┌───────────────────┐     ┌───────────────────┐
│                   │     │                   │     │                   │
│  Schema Analysis  │────▶│  Type Mapping     │────▶│  Template Engine  │
│                   │     │                   │     │                   │
└───────────────────┘     └───────────────────┘     └───────────────────┘
          │                        │                         │
          │                        │                         │
          ▼                        ▼                         ▼
┌──────────────────────────────────────────────────────────────────────┐
│                                                                      │
│                      Code Generation Pipeline                        │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
                                   │
                                   │
                                   ▼
┌──────────────────────────────────────────────────────────────────────┐
│                                                                      │
│                       CLI Interface Layer                            │
│                                                                      │
└──────────────────────────────────────────────────────────────────────┘
```

## Core Components

### 1. Schema Analysis Engine

The Schema Analysis Engine extracts database schema information and detects relationships:

- **Database Schema Provider**: Connects to databases and extracts schema metadata
- **Relationship Detector**: Analyzes schema for relationship patterns
- **Navigation Property Namer**: Generates intuitive navigation property names

### 2. Type Mapping System

The Type Mapping System handles conversion of database types to programming language types:

- **Type Map Provider**: Maintains mappings for each target language
- **Nullability Handler**: Determines and applies proper nullability syntax
- **Precision/Scale Handler**: Handles numeric precision and scale

### 3. Template Engine

The Template Engine processes Handlebars templates with schema data:

- **Template Engine**: Integrates with Handlebars.Net
- **Helper Registry**: Manages and registers template helpers
- **Layout Template Processor**: Handles layout templates with regions

### 4. Code Generation Pipeline

The Code Generation Pipeline orchestrates the end-to-end generation process:

- **Code Generator**: Orchestrates schema analysis, template processing, and output
- **Differential Generator**: Selectively regenerates changed entities
- **Output Provider**: Writes generated code to different destinations

### 5. CLI Interface

The CLI Interface provides command-line access to the system:

- **Command Handler**: Processes CLI commands
- **Configuration Manager**: Handles config file loading/saving
- **Progress Reporter**: Reports generation progress

## Key Interfaces

### Schema Analysis

```csharp
public interface IDatabaseSchemaProvider
{
    Task<IDatabaseSchema> GetSchemaAsync(string connectionString, SchemaProviderOptions options);
    Task<bool> TestConnectionAsync(string connectionString);
}

public interface IDatabaseSchema
{
    string Name { get; }
    IReadOnlyCollection<ITable> Tables { get; }
    IReadOnlyCollection<IView> Views { get; }
    IReadOnlyCollection<IStoredProcedure> StoredProcedures { get; }
    IReadOnlyCollection<IFunction> Functions { get; }
}

public interface IRelationshipDetector
{
    IEnumerable<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
}
```

### Type Mapping

```csharp
public interface ITypeMapper
{
    string MapType(IColumn column, TypeMappingOptions options);
    string GetDefaultValue(IColumn column, TypeMappingOptions options);
    bool IsNullable(IColumn column, TypeMappingOptions options);
}

public interface ITypeMapProvider
{
    ITypeMap GetTypeMap(string language);
    void RegisterTypeMap(string language, ITypeMap typeMap);
}
```

### Template Engine

```csharp
public interface ITemplateEngine
{
    void RegisterHelper(string name, Delegate helper);
    void RegisterBlockHelper(string name, Delegate helper);
    void RegisterPartial(string name, string template);
    string Compile(string template, object data);
}

public interface ITemplateProcessor
{
    Task<IDictionary<string, string>> ProcessAsync(string template, object data);
    Task<IDictionary<string, string>> ProcessFileAsync(string templatePath, object data);
}
```

### Code Generation

```csharp
public interface ICodeGenerator
{
    Task<CodeGenerationResult> GenerateAsync(CodeGenerationOptions options);
}

public interface ICodeGenerationPipeline
{
    ICodeGenerationPipeline WithSchemaAnalyzer(ISchemaAnalyzer analyzer);
    ICodeGenerationPipeline WithTemplateProcessor(ITemplateProcessor processor);
    ICodeGenerationPipeline WithTypeMapper(ITypeMapper mapper);
    Task<CodeGenerationResult> ExecuteAsync(CodeGenerationOptions options);
}
```

## Data Flow

1. **Schema Extraction**: Database schema is extracted via provider
2. **Relationship Detection**: Relationships are detected and categorized
3. **Type Mapping**: Database types are mapped to language types
4. **Template Processing**: Templates are processed with schema data
5. **Code Generation**: Generated code is written to output

## Extension Points

- **Database Providers**: Add support for additional database types
- **Type Mappers**: Add support for additional programming languages
- **Template Helpers**: Add custom template helpers
- **Output Providers**: Add support for additional output destinations

## Performance Considerations

- Template compilation caching
- Incremental schema processing
- Differential code generation
- Parallel processing where applicable

This architecture provides a clean separation of concerns with clear responsibilities and extension points.
