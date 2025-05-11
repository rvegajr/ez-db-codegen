# Schema Comparison Tool

This is a standalone tool for comparing SQL Server schema discovery performance between Entity Framework Core and direct SQL queries.

## Overview

The Schema Comparison Tool allows you to:

1. Connect to a SQL Server database
2. Analyze the database schema using both EF Core and direct SQL queries
3. Compare the results and performance of both approaches
4. Output the results in either text or JSON format

## Usage

```bash
dotnet run -- --connection "<connection-string>" [--database <database-name>] [--format <json|text>]
```

### Parameters

- `--connection`: Required. The connection string to your SQL Server database.
- `--database`: Optional. The name of the database to analyze. If provided, it will be added to the connection string.
- `--format`: Optional. The output format, either "text" (default) or "json".

### Example

```bash
dotnet run -- --connection "Server=localhost;User ID=sa;Password=APADemo123!;TrustServerCertificate=True" --database AdventureWorks
```

## Output

The tool will output:
- Number of tables discovered by each method
- Number of columns discovered by each method
- Number of relationships discovered by each method
- Time taken by each method
- Performance comparison between the two methods

## Building the Tool

```bash
dotnet build
```

## Running the Tool

```bash
dotnet run -- --help
```

## Notes

- This tool is a standalone alternative to the schema comparison functionality in the EZ-DB-CodeGen CLI.
- It was created to avoid the circular dependency issues in the main project.
- The tool uses the same core functionality as the original implementation.

## Requirements

- .NET 8.0 SDK
- Microsoft.EntityFrameworkCore.SqlServer package
- Microsoft.Extensions.Logging.Console package
