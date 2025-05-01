# EzDbCodeGen Templates

This directory contains Handlebars templates for generating C# code from database schemas. These templates are designed to produce high-quality, maintainable code that follows best practices.

## Available Templates

### CSharp

- **EntityModel.hbs**: Generates entity classes with:
  - Data annotations and Fluent API configuration in the same file
  - Interface definitions for each entity
  - DTO classes without navigation properties
  - Conversion methods between entities and DTOs
  - Self-referencing relationship handling
  - Many-to-many relationship support via junction tables
  - Equality and ToString implementations

- **DbContext.hbs**: Generates an Entity Framework DbContext with:
  - DbSet properties for all entities
  - Fluent API configuration for relationships
  - Connection string configuration
  - Support for dependency injection

- **Repository.hbs**: Generates repository classes with:
  - Interface definitions
  - CRUD operations
  - DTO support
  - Async methods
  - Error handling

- **ServiceCollectionExtensions.hbs**: Generates extension methods for dependency injection:
  - DbContext registration
  - Repository registration
  - Connection string configuration

- **RepositoryTests.hbs**: Generates unit tests for repositories:
  - In-memory database setup
  - Test data generation
  - Tests for all CRUD operations

## Template Options

The templates support the following options:

```json
{
  "Namespace": "YourNamespace",
  "ContextName": "YourDbContext",
  "ConnectionStringName": "DefaultConnection",
  "UseFluentApi": true,
  "GenerateNavigationProperties": true,
  "GenerateEquality": true,
  "GenerateToString": true,
  "UseJsonSerialization": true,
  "UseNewtonsoft": false,
  "EnableSensitiveDataLogging": false,
  "EnableDetailedErrors": true,
  "IncludeNavigationProperties": true,
  "CommandTimeout": 30
}
```

## Advantages Over EF Core Power Tools

These templates offer several advantages over EF Core Power Tools:

1. **Advanced Relationship Detection**:
   - Junction table detection for many-to-many relationships
   - Self-referencing relationship handling
   - Inheritance pattern detection (TPH/TPT)

2. **Complete Solution**:
   - Entity classes with interfaces
   - DTO classes without navigation properties
   - Repository pattern implementation
   - Dependency injection setup
   - Unit tests

3. **Customization**:
   - All code in one place (entity + configuration)
   - Consistent naming conventions
   - Detailed documentation comments
   - Customizable templates

4. **Best Practices**:
   - Async/await throughout
   - Proper error handling
   - Dependency injection
   - Repository pattern
   - Unit testing

## Usage

To use these templates with EzDbCodeGen:

1. Configure your database connection
2. Set template options
3. Run the code generation command

Example:

```csharp
var options = new CodeGenerationOptions
{
    Namespace = "YourNamespace",
    ContextName = "YourDbContext",
    ConnectionStringName = "DefaultConnection",
    UseFluentApi = true,
    GenerateNavigationProperties = true,
    GenerateEquality = true,
    GenerateToString = true
};

var generator = new CodeGenerator(options);
await generator.GenerateAsync(schema, "path/to/output");
```

## Extending Templates

You can extend these templates by:

1. Creating new Handlebars helpers
2. Modifying existing templates
3. Creating new templates based on existing ones
