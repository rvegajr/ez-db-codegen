# SQL Server Schema Provider

The SQL Server Schema Provider enables EzDbCodeGen to extract database schema information from Microsoft SQL Server databases. This component is designed to accurately extract tables, columns, primary keys, foreign keys, indexes, and other database objects, and to detect advanced relationships between them.

## Features

- **Comprehensive Schema Extraction**: Extracts full table definitions, including columns, data types, constraints, and relationships.
- **Advanced Relationship Detection**: Automatically detects different types of relationships:
  - One-to-Many: Standard foreign key relationships
  - One-to-One: Foreign keys with unique constraints
  - Many-to-Many: Via junction tables
  - Self-Referencing: Tables that reference themselves
  - Inheritance: TPH (Table-per-Hierarchy) and TPT (Table-per-Type) patterns

## Using the SQL Server Schema Provider

### From the Command Line

To extract schema information from a SQL Server database using the command line:

```bash
dotnet run schema-info --connection "Server=your-server;Database=your-db;User ID=your-user;Password=your-password;" --provider SqlServer
```

#### Command-Line Options

- `--connection`: Required. The connection string to your SQL Server database.
- `--provider`: Optional. Defaults to "SqlServer". Specifies the database provider to use.
- `--schema`: Optional. Filters tables by database schema (e.g., "dbo").
- `--table`: Optional. Filters by a specific table name.
- `--format`: Optional. Output format, either "text" or "json" (default: "text").
- `--details`: Optional. Show detailed information about tables (default: false).
- `--detect-relationships`: Optional. Enable or disable relationship detection (default: true).

### Programmatically

You can also use the SQL Server Schema Provider programmatically in your code:

```csharp
// Get the schema provider
var logger = new ConsoleLogger();
var schemaProvider = new SqlServerSchemaProvider(logger);

// Configure the connection
schemaProvider.Configure(new Dictionary<string, string>
{
    ["ConnectionString"] = "Server=your-server;Database=your-db;User ID=your-user;Password=your-password;"
});

// Extract the schema
var schema = await schemaProvider.GetSchemaAsync();

// Access schema information
foreach (var table in schema.Tables)
{
    Console.WriteLine($"Table: {table.Schema}.{table.Name}");
    
    foreach (var column in table.Columns)
    {
        Console.WriteLine($"  Column: {column.Name} ({column.DisplayType})");
    }
}
```

## Using the Relationship Detector

To detect relationships between tables:

```csharp
// Create a relationship detector
var relationshipDetector = new RelationshipDetector(logger);

// Detect relationships in the schema
var relationships = await relationshipDetector.DetectRelationshipsAsync(schema);

// Process detected relationships
foreach (var relationship in relationships)
{
    Console.WriteLine($"Relationship: {relationship.RelationshipType}");
    Console.WriteLine($"  From: {relationship.SourceTable.Name}");
    Console.WriteLine($"  To: {relationship.TargetTable.Name}");
    Console.WriteLine($"  Navigation Properties: {relationship.SourceNavigationProperty} -> {relationship.TargetNavigationProperty}");
}
```

## Connection String Format

The SQL Server connection string should follow this format:

```
Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;
```

For more options, see the [SQL Server connection string documentation](https://learn.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring).

## Best Practices

1. **Use Integrated Security When Possible**: For local development, Windows authentication is more secure:
   ```
   Server=myServerAddress;Database=myDataBase;Integrated Security=True;
   ```

2. **Limit Database User Permissions**: Create a database user with read-only permissions for schema extraction.

3. **Filter Large Schemas**: Use the `--schema` or `--table` options to extract only what you need when working with large databases.

4. **Review Detected Relationships**: While the relationship detector is sophisticated, always review the detected relationships for complex schemas.
