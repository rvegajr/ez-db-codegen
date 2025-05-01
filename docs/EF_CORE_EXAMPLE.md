# Entity Framework Core Code Generation Example

This guide demonstrates how to use EzDbCodeGen to generate Entity Framework Core model classes from a SQL Server database.

## Prerequisites

- A SQL Server database with tables, relationships, and constraints
- .NET SDK 8.0 or higher
- EzDbCodeGen CLI tool

## Step 1: Inspect Your Database Schema

First, let's inspect the database schema to understand what we're working with:

```bash
dotnet run schema-info --connection "Server=yourserver;Database=yourdatabase;User ID=username;Password=password;" --format json --details true > schema.json
```

This command extracts your database schema and saves it to a JSON file for reference.

## Step 2: Create a Configuration File

Create a file named `ezdbcodegen.json` with the following content:

```json
{
  "ConnectionString": "Server=yourserver;Database=yourdatabase;User ID=username;Password=password;",
  "Provider": "SqlServer",
  "TemplateDirectory": "templates/efcore",
  "OutputDirectory": "output/Models",
  "Namespace": "YourApp.Models",
  "ClassNameFormat": "{TableName}",
  "PropertyNameFormat": "{ColumnName}",
  "Options": {
    "UseDataAnnotations": true,
    "GenerateNavigationProperties": true,
    "PluralizeCollectionNavigationProperties": true
  }
}
```

## Step 3: Create a Template (if not using the built-in one)

You can use the built-in Entity Framework Core template or create your own. A custom template might look like this:

```handlebars
// templates/efcore/EntityModel.hbs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace {{namespace}}.Models
{
    /// <summary>
    /// {{#if table.Description}}{{table.Description}}{{else}}Represents the {{table.Name}} entity{{/if}}
    /// </summary>
    [Table("{{table.Name}}", Schema = "{{table.Schema}}")]
    public partial class {{className}}
    {
        {{#if hasCollectionProperties}}
        public {{className}}()
        {
            {{#each collectionProperties}}
            {{propertyName}} = new HashSet<{{typeName}}>();
            {{/each}}
        }
        {{/if}}

        {{#each columns}}
        {{#if isPrimaryKey}}[Key]{{/if}}
        {{#if isIdentity}}[DatabaseGenerated(DatabaseGeneratedOption.Identity)]{{/if}}
        public {{typeName}} {{propertyName}} { get; set; }{{#if isNullable}}?{{/if}}
        
        {{/each}}

        {{#each navigationProperties}}
        public {{#if isCollection}}ICollection<{{/if}}{{typeName}}{{#if isCollection}}>{{/if}} {{propertyName}} { get; set; }{{#if isNullable}}?{{/if}}
        {{/each}}
    }
}
```

## Step 4: Generate Code

Run the generate command to create your model classes:

```bash
dotnet run generate --config ezdbcodegen.json
```

## Step 5: Inspect Generated Code

Check the output directory (`output/Models` in this example) to see the generated code. For each table in your database, you should have a corresponding C# class with properties for all columns and navigation properties for relationships.

## Example Generated Class

Here's an example of what a generated class might look like for a `Customer` table with related `Order` entities:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YourApp.Models
{
    /// <summary>
    /// Represents customers who place orders
    /// </summary>
    [Table("Customer", Schema = "dbo")]
    [Index(nameof(Email), IsUnique = true)]
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(255)]
        public string Email { get; set; }
        
        [Column("DateCreated")]
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public ICollection<Order> Orders { get; set; }
    }
}
```

## Step 6: Use the Generated Models in Your Application

Add the generated models to your Entity Framework Core DbContext:

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    // Add other entities as needed
}
```

## Advanced: Customizing the Generation Process

You can customize the code generation in several ways:

1. **Modify the templates**: Update the Handlebars templates to match your coding style or add custom attributes

2. **Create a mapping file**: To override default type mappings for specific columns

3. **Filter tables and columns**: Use configuration settings to include or exclude specific database objects

4. **Post-processing**: Apply custom transformations to the generated code

## Summary

EzDbCodeGen makes it easy to generate Entity Framework Core model classes directly from your database schema, saving you from manual coding and ensuring your models accurately reflect your database structure, including relationships between tables.
