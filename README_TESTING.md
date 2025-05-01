# Testing the Enhanced SQL Server Schema Provider

This guide shows you how to test the enhanced SQL Server schema provider with a real database. The provider now supports extracting tables, views, stored procedures, and functions.

## Prerequisites

- SQL Server database (local or remote)
- SQL Server connection string
- .NET SDK 8.0 or higher

## Testing with the CLI

### 1. Build the Project

First, build the project:

```bash
dotnet build
```

### 2. Test Schema Extraction

Run the schema-info command to extract and display schema information from your database:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --details true
```

### 3. View Different Object Types

You can filter the output to show specific object types:

```bash
# View only tables
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --object-type tables

# View only views
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --object-type views

# View only stored procedures
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --object-type procs

# View only functions
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --object-type functions
```

### 4. Filter by Schema

You can filter objects by database schema:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --schema dbo
```

### 5. Output JSON Format

You can get the schema information in JSON format:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --format json > schema.json
```

## Testing with the AdventureWorks Database

If you don't have a database available, you can download and use the AdventureWorks sample database:

1. Download AdventureWorks from: https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure

2. Restore the database to your SQL Server instance

3. Run the schema extraction command:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj schema-info \
  --connection "Server=<your-server>;Database=AdventureWorks;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --details true
```

## Creating Templates for Views, Stored Procedures, and Functions

You can create Handlebars templates to generate code for these database objects. Here are some examples:

### View Template

Create a file at `templates/views/ViewModel.hbs`:

```handlebars
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {{namespace}}.Models.Views
{
    /// <summary>
    /// {{#if view.Description}}{{view.Description}}{{else}}View for {{view.Name}}{{/if}}
    /// </summary>
    [Table("{{view.Name}}", Schema = "{{view.Schema}}")]
    public partial class {{className}}
    {
        {{#each columns}}
        /// <summary>
        /// {{#if description}}{{description}}{{else}}Gets or sets the {{name}} property{{/if}}
        /// </summary>
        public {{typeName}} {{propertyName}} { get; set; }{{#if isNullable}}?{{/if}}
        
        {{/each}}
    }
}
```

### Stored Procedure Template

Create a file at `templates/procedures/StoredProcedureWrapper.hbs`:

```handlebars
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace {{namespace}}.Data.Procedures
{
    /// <summary>
    /// {{#if procedure.Description}}{{procedure.Description}}{{else}}Wrapper for {{procedure.Name}} stored procedure{{/if}}
    /// </summary>
    public class {{className}}
    {
        private readonly string _connectionString;

        public {{className}}(string connectionString)
        {
            _connectionString = connectionString;
        }

        {{#if resultColumns}}
        /// <summary>
        /// Executes the stored procedure and returns the results
        /// </summary>
        {{#if parameters}}
        /// <param name="parameters">Parameters for the stored procedure</param>
        {{/if}}
        public async Task<IEnumerable<{{resultClassName}}>> ExecuteAsync({{#each parameters}}{{typeName}} {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}})
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var parameters = new DynamicParameters();
                {{#each parameters}}
                parameters.Add("{{name}}", {{parameterName}}, {{direction}});
                {{/each}}
                
                return await connection.QueryAsync<{{resultClassName}}>(
                    "[{{procedure.Schema}}].[{{procedure.Name}}]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
        {{else}}
        /// <summary>
        /// Executes the stored procedure
        /// </summary>
        {{#if parameters}}
        /// <param name="parameters">Parameters for the stored procedure</param>
        {{/if}}
        public async Task ExecuteAsync({{#each parameters}}{{typeName}} {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}})
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var parameters = new DynamicParameters();
                {{#each parameters}}
                parameters.Add("{{name}}", {{parameterName}}, {{direction}});
                {{/each}}
                
                await connection.ExecuteAsync(
                    "[{{procedure.Schema}}].[{{procedure.Name}}]",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
        {{/if}}
    }

    {{#if resultColumns}}
    /// <summary>
    /// Result class for {{procedure.Name}}
    /// </summary>
    public class {{resultClassName}}
    {
        {{#each resultColumns}}
        /// <summary>
        /// {{#if description}}{{description}}{{else}}Gets or sets the {{name}} property{{/if}}
        /// </summary>
        public {{typeName}} {{propertyName}} { get; set; }{{#if isNullable}}?{{/if}}
        
        {{/each}}
    }
    {{/if}}
}
```

### Function Template

Create a file at `templates/functions/FunctionWrapper.hbs`:

```handlebars
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace {{namespace}}.Data.Functions
{
    /// <summary>
    /// {{#if function.Description}}{{function.Description}}{{else}}Wrapper for {{function.Name}} function{{/if}}
    /// </summary>
    public class {{className}}
    {
        private readonly string _connectionString;

        public {{className}}(string connectionString)
        {
            _connectionString = connectionString;
        }

        {{#if function.IsTableValued}}
        /// <summary>
        /// Executes the table-valued function and returns the results
        /// </summary>
        {{#if parameters}}
        /// <param name="parameters">Parameters for the function</param>
        {{/if}}
        public async Task<IEnumerable<{{resultClassName}}>> ExecuteAsync({{#each parameters}}{{typeName}} {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}})
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var sql = "SELECT * FROM [{{function.Schema}}].[{{function.Name}}]({{#each parameters}}@{{name}}{{#unless @last}}, {{/unless}}{{/each}})";
                
                return await connection.QueryAsync<{{resultClassName}}>(
                    sql,
                    new { {{#each parameters}}{{name}} = {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}} });
            }
        }
        {{else}}
        /// <summary>
        /// Executes the scalar function and returns the result
        /// </summary>
        {{#if parameters}}
        /// <param name="parameters">Parameters for the function</param>
        {{/if}}
        public async Task<{{returnTypeName}}> ExecuteAsync({{#each parameters}}{{typeName}} {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}})
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var sql = "SELECT [{{function.Schema}}].[{{function.Name}}]({{#each parameters}}@{{name}}{{#unless @last}}, {{/unless}}{{/each}})";
                
                return await connection.ExecuteScalarAsync<{{returnTypeName}}>(
                    sql,
                    new { {{#each parameters}}{{name}} = {{parameterName}}{{#unless @last}}, {{/unless}}{{/each}} });
            }
        }
        {{/if}}
    }

    {{#if function.IsTableValued}}
    /// <summary>
    /// Result class for {{function.Name}}
    /// </summary>
    public class {{resultClassName}}
    {
        {{#each resultColumns}}
        /// <summary>
        /// {{#if description}}{{description}}{{else}}Gets or sets the {{name}} property{{/if}}
        /// </summary>
        public {{typeName}} {{propertyName}} { get; set; }{{#if isNullable}}?{{/if}}
        
        {{/each}}
    }
    {{/if}}
}
```

## Generating Code

With these templates, you can generate code for views, stored procedures, and functions:

```bash
dotnet run --project src/EzDbCodeGen.Cli/EzDbCodeGen.Cli.csproj generate \
  --connection "Server=<your-server>;Database=<your-database>;User ID=<your-user>;Password=<your-password>;TrustServerCertificate=True;" \
  --template-dir templates/views \
  --output-dir output/Models/Views \
  --namespace YourApp.Models \
  --object-type views
```

Replace `--object-type views` with `--object-type procs` or `--object-type functions` to generate code for stored procedures or functions.
