# SQL Server Schema Provider Enhancements

## Overview

The SQL Server schema provider has been enhanced to support:

1. **Views**: Database views are now extracted, including column metadata and properties.
2. **Stored Procedures**: Stored procedures are now extracted, including parameters and result sets.
3. **Functions**: Both scalar and table-valued functions are now extracted, including parameters, return types, and result sets for table-valued functions.

## Implemented Changes

The following components have been updated:

### Core Interfaces

New interfaces have been added:
- `IView` and `IViewColumn` for database views
- `IStoredProcedure` for stored procedures
- `IFunction` for database functions
- `IParameter` for parameters in stored procedures and functions
- `IResultColumn` for result columns in stored procedures and table-valued functions

### Model Classes

New model classes have been implemented to support these objects:
- `ViewModel` and `ViewColumnModel`
- `StoredProcedureModel` and `ParameterModel`
- `FunctionModel` and `ResultColumnModel`

### Schema Extraction

The `SqlServerSchemaProvider` has been enhanced with new SQL queries to extract:
- Views from `sys.views`
- Stored procedures from `sys.procedures`
- Functions from `sys.objects` filtered by type
- Parameters from `sys.parameters`
- Result sets using `sp_describe_first_result_set`

### CLI Commands

The `schema-info` command now supports extracting and displaying:
- Tables, views, stored procedures, and functions
- Filtering by object type (`--object-type` parameter)
- Both text and JSON output formats

### Code Generation Templates

Templates have been provided for generating code for:
- Views (`ViewModel.hbs`)
- Stored procedures (`StoredProcedureWrapper.hbs`)
- Functions (`FunctionWrapper.hbs`)

## Testing the Enhancements

### Using the Standalone Tester

We've created a standalone tester that can verify the schema provider enhancements:

```bash
cd /Users/rickyvega/Dev/Noctusoft/ez-db-codegen
dotnet run --project ./SchemaProviderTester/SchemaProviderTester.csproj -- "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;"
```

You can filter by object type by adding a parameter:

```bash
# Only show views
dotnet run --project ./SchemaProviderTester/SchemaProviderTester.csproj -- "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" views

# Only show stored procedures
dotnet run --project ./SchemaProviderTester/SchemaProviderTester.csproj -- "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" procs

# Only show functions
dotnet run --project ./SchemaProviderTester/SchemaProviderTester.csproj -- "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" functions
```

### Using the CLI (When Build Issues Are Resolved)

Once the build issues are resolved, you can use the CLI to test these enhancements:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info --connection "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" --object-type all
```

The `--object-type` parameter can be set to:
- `tables` - Shows only tables
- `views` - Shows only views
- `procs` - Shows only stored procedures
- `functions` - Shows only functions
- `all` - Shows all objects (default)

You can also use the `--format json` parameter to get the output in JSON format:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info --connection "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" --object-type views --format json
```

## Code Generation (Once Build Issues Are Resolved)

Once the build issues are resolved, you can generate code for views, stored procedures, and functions:

```bash
# Generate code for views
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj generate --connection "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" --template-dir templates/views --output-dir output/Models/Views --namespace YourApp.Models --object-type views

# Generate code for stored procedures
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj generate --connection "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" --template-dir templates/procedures --output-dir output/Data/Procedures --namespace YourApp.Data --object-type procs

# Generate code for functions
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj generate --connection "Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;" --template-dir templates/functions --output-dir output/Data/Functions --namespace YourApp.Data --object-type functions
```

## Next Steps

1. Address build issues in the main project to fully integrate the enhancements
2. Add unit tests for the new functionality
3. Update documentation to reflect the new capabilities
4. Enhance template helpers to better support views, stored procedures, and functions

## Technical Notes for Future Development

The schema provider now supports a more comprehensive database model that includes:

- Tables and their relationships
- Views and their columns
- Stored procedures with parameters and result sets
- Functions (both scalar and table-valued) with parameters and result sets

This enhanced model provides a complete picture of a database schema for more comprehensive code generation capabilities.
