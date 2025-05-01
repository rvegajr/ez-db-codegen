# EzDbCodeGen NG

A next-generation code generation tool for database schemas that vastly surpasses EF Core Power Tools in terms of relationship detection accuracy, template flexibility, and output quality.

## Engineer's Quick Start Guide

### 1. Start Here: Critical Documentation

Begin by reviewing these documents in order:

1. **[IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md)** - The primary roadmap for implementation with TDD approach
2. **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Overview of system architecture and component interactions
3. **[INTERFACE_DEFINITIONS.md](./INTERFACE_DEFINITIONS.md)** - Core interface definitions to implement

### 2. Implementation Approach

This project follows strict Test-Driven Development:

1. Write failing tests first for each component
2. Implement only enough code to make tests pass
3. Refactor while maintaining passing tests
4. Achieve high test coverage (>90%)

### 3. Project Structure

```
/Users/rickyvega/Dev/Noctusoft/ez-db-codegen/
├── src/                                  # Implementation code
│   ├── EzDbCodeGen.Core/                 # Core abstractions
│   ├── EzDbCodeGen.Schema/               # Schema extraction
│   ├── EzDbCodeGen.TypeMapping/          # Type mapping
│   ├── EzDbCodeGen.TemplateEngine/       # Template engine
│   ├── EzDbCodeGen.CodeGeneration/       # Generation pipeline
│   └── EzDbCodeGen.Cli/                  # CLI interface
├── tests/                                # Test projects
├── templates/                            # Template definitions
└── docs/                                 # Documentation
```

### 4. Critical Components to Implement First

1. Start with the Schema extraction component:
   - Implement `SqlServerSchemaProvider` first (before other providers)
   - Focus on comprehensive relationship detection

2. Implement the testing harness for templates:
   - Create a template test harness similar to this model:

```handlebars
{{!-- 
ModelTestHarness.hbs - Template for testing model generation
This generates:
1. A model class representation for validation
2. Test data about the model's properties and relationships
3. Debug visualization of the internal schema representation
--}}

// MODEL CLASS OUTPUT
namespace {{namespace}}.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// {{description}}
/// </summary>
[Table("{{tableName}}", Schema = "{{schemaName}}")]
public partial class {{className}}
{
    {{#if hasCollectionProperties}}
    // Collection initializers in constructor
    public {{className}}()
    {
        {{#each collectionProperties}}
        {{propertyName}} = new HashSet<{{typeName}}>();
        {{/each}}
    }
    {{/if}}

    // Properties, navigation properties, etc.
}

// TEST DATA OUTPUT - For validation
/*
{
  "TableMetadata": {
    "Name": "{{tableName}}",
    "Schema": "{{schemaName}}",
    // Additional metadata
  },
  // Properties, relationships, etc.
}
*/
```

### 5. Testing Requirements

- Write tests before implementation for all components
- Include tests for edge cases and error conditions
- Use mock databases for testing relationship detection
- Compare output with EF Core Power Tools to verify superior quality

### 6. Files to Update

- Create the solution and project structure first
- Implement interfaces in INTERFACE_DEFINITIONS.md
- Focus on SQL Server provider initially, but design for extensibility
- Prioritize components according to IMPLEMENTATION_CHECKLIST.md

## Goals for Superiority Over EF Core Power Tools

1. More accurate relationship detection
2. Better navigation property naming
3. Superior handling of inheritance relationships
4. More comprehensive type mapping
5. Differential code generation
6. Advanced template capabilities

By following this guide and the accompanying architectural documents, you will create a vastly superior code generation tool that outperforms EF Core Power Tools in all key metrics.
