using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using EzDbCodeGen.Core.Schema;

namespace SchemaProviderValidator;

public class Program
{
    private static readonly ILogger Logger = LoggerFactory
        .Create(builder => builder.AddConsole())
        .CreateLogger<Program>();

    // Command line args:
    // For schema extraction: <connection-string> [object-type]
    //   object-type can be: tables, views, procs, functions, all (default)
    // For schema diff: diff <source-connection> <target-connection> [object-type] [schema-name]
    public static async Task Main(string[] args)
    {
        if (args.Length < 1)
        {
            PrintUsage();
            return;
        }

        // Check if we're doing a schema diff
        if (args[0].Equals("diff", StringComparison.OrdinalIgnoreCase))
        {
            await RunSchemaDiffAsync(args);
            return;
        }

        string connectionString = args[0];
        string objectType = args.Length > 1 ? args[1].ToLowerInvariant() : "all";

        Console.WriteLine($"Connecting to: {MaskConnectionString(connectionString)}");

        using var connection = new SqlConnection(connectionString);
        try
        {
            await connection.OpenAsync();
            Console.WriteLine("Connection successful!");
            Console.WriteLine($"Server Version: {connection.ServerVersion}");
            Console.WriteLine($"Database: {connection.Database}");
            Console.WriteLine();

            Console.WriteLine("Extracting Schema Information...");
            Console.WriteLine();

            if (objectType == "all" || objectType == "tables")
            {
                await ExtractTablesAsync(connection);
            }

            if (objectType == "all" || objectType == "views")
            {
                await ExtractViewsAsync(connection);
            }

            if (objectType == "all" || objectType == "procs")
            {
                await ExtractStoredProceduresAsync(connection);
            }

            if (objectType == "all" || objectType == "functions")
            {
                await ExtractFunctionsAsync(connection);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  1. Schema Extraction: SchemaProviderValidator <connection-string> [object-type]");
        Console.WriteLine("     object-type: tables, views, procs, functions, all (default)");
        Console.WriteLine();
        Console.WriteLine("  2. Schema Diff: SchemaProviderValidator diff <source-connection> <target-connection> [object-type] [schema-name]");
    }

    private static async Task RunSchemaDiffAsync(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage for schema diff: SchemaProviderValidator diff <source-connection> <target-connection> [object-type] [schema-name]");
            return;
        }

        string sourceConnectionString = args[1];
        string targetConnectionString = args[2];
        string objectType = args.Length > 3 ? args[3].ToLowerInvariant() : "all";
        string schemaName = args.Length > 4 ? args[4] : null;

        Console.WriteLine("Starting Schema Diff...");
        Console.WriteLine($"Source: {MaskConnectionString(sourceConnectionString)}");
        Console.WriteLine($"Target: {MaskConnectionString(targetConnectionString)}");
        Console.WriteLine($"Object Type: {objectType}");
        if (!string.IsNullOrEmpty(schemaName))
        {
            Console.WriteLine($"Schema Filter: {schemaName}");
        }
        Console.WriteLine();

        try
        {
            var validator = new SchemaDiffValidator(Logger);
            await validator.ValidateSchemaDiffAsync(
                sourceConnectionString,
                targetConnectionString,
                "SqlServer",
                schemaName,
                objectType);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during schema diff: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            Console.WriteLine(ex.StackTrace);
        }
    }

    private static async Task ExtractTablesAsync(SqlConnection connection)
    {
        Console.WriteLine("--- Tables ---");
        
        // Query to get tables and their column counts
        const string sql = @"
            SELECT  
                SCHEMA_NAME(t.schema_id) AS SchemaName,
                t.name AS TableName,
                (SELECT COUNT(*) FROM sys.columns WHERE object_id = t.object_id) AS ColumnCount
            FROM sys.tables t
            ORDER BY SchemaName, TableName";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        int tableCount = 0;
        while (await reader.ReadAsync())
        {
            string schemaName = reader.GetString(0);
            string tableName = reader.GetString(1);
            int columnCount = reader.GetInt32(2);

            Console.WriteLine($"Table: {schemaName}.{tableName} ({columnCount} columns)");
            tableCount++;
        }

        Console.WriteLine();
        Console.WriteLine($"Total Tables: {tableCount}");
        Console.WriteLine();
    }

    private static async Task ExtractViewsAsync(SqlConnection connection)
    {
        Console.WriteLine("--- Views ---");
        
        // Query to get views, their column counts, and whether they're indexed
        const string sql = @"
            SELECT  
                SCHEMA_NAME(v.schema_id) AS SchemaName,
                v.name AS ViewName,
                (SELECT COUNT(*) FROM sys.columns WHERE object_id = v.object_id) AS ColumnCount,
                CASE WHEN EXISTS (
                    SELECT 1 FROM sys.indexes i 
                    WHERE i.object_id = v.object_id AND i.type > 0
                ) THEN 'Yes' ELSE 'No' END AS IsIndexed
            FROM sys.views v
            WHERE v.is_ms_shipped = 0
            ORDER BY SchemaName, ViewName";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        int viewCount = 0;
        while (await reader.ReadAsync())
        {
            string schemaName = reader.GetString(0);
            string viewName = reader.GetString(1);
            int columnCount = reader.GetInt32(2);
            string isIndexed = reader.GetString(3);

            Console.WriteLine($"View: {schemaName}.{viewName} ({columnCount} columns, Indexed: {isIndexed})");
            viewCount++;
        }

        Console.WriteLine();
        Console.WriteLine($"Total Views: {viewCount}");
        Console.WriteLine();
    }

    private static async Task ExtractStoredProceduresAsync(SqlConnection connection)
    {
        Console.WriteLine("--- Stored Procedures ---");
        
        // Query to get stored procedures and their parameter counts
        const string sql = @"
            SELECT  
                SCHEMA_NAME(p.schema_id) AS SchemaName,
                p.name AS ProcedureName,
                (
                    SELECT COUNT(*) 
                    FROM sys.parameters 
                    WHERE object_id = p.object_id AND parameter_id > 0
                ) AS ParameterCount
            FROM sys.procedures p
            WHERE p.is_ms_shipped = 0
            ORDER BY SchemaName, ProcedureName";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        int procCount = 0;
        while (await reader.ReadAsync())
        {
            string schemaName = reader.GetString(0);
            string procName = reader.GetString(1);
            int paramCount = reader.GetInt32(2);

            Console.WriteLine($"Stored Procedure: {schemaName}.{procName} ({paramCount} parameters)");
            procCount++;
        }

        Console.WriteLine();
        Console.WriteLine($"Total Stored Procedures: {procCount}");
        Console.WriteLine();
    }

    private static async Task ExtractFunctionsAsync(SqlConnection connection)
    {
        Console.WriteLine("--- Functions ---");
        
        // Query to get functions and their types
        const string sql = @"
            SELECT  
                SCHEMA_NAME(f.schema_id) AS SchemaName,
                f.name AS FunctionName,
                CASE 
                    WHEN f.type = 'FN' THEN 'Scalar-Valued' 
                    WHEN f.type = 'TF' THEN 'Table-Valued'
                    WHEN f.type_desc = 'SQL_INLINE_TABLE_VALUED_FUNCTION' THEN 'Table-Valued'
                    ELSE f.type_desc 
                END AS FunctionType,
                (
                    SELECT COUNT(*) 
                    FROM sys.parameters 
                    WHERE object_id = f.object_id AND parameter_id > 0
                ) AS ParameterCount,
                f.object_id
            FROM sys.objects f
            WHERE f.type IN ('FN', 'IF', 'TF')
            AND f.is_ms_shipped = 0
            ORDER BY SchemaName, FunctionName";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        int functionCount = 0;
        List<(string Schema, string Name, int ObjectId, bool IsTableValued)> tableFunctions = new();
        
        while (await reader.ReadAsync())
        {
            string schemaName = reader.GetString(0);
            string functionName = reader.GetString(1);
            string functionType = reader.GetString(2);
            int paramCount = reader.GetInt32(3);
            int objectId = reader.GetInt32(4);

            Console.WriteLine($"Function: {schemaName}.{functionName} (Type: {functionType}, {paramCount} parameters)");
            functionCount++;
            
            if (functionType.Contains("Table-Valued"))
            {
                tableFunctions.Add((schemaName, functionName, objectId, true));
            }
        }

        // For table-valued functions, try to get their result columns
        foreach (var func in tableFunctions)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Result columns for {func.Schema}.{func.Name}:");
                
                // This simplified approach executes the function with NULL parameters
                // In a production schema provider, we'd use sys.dm_exec_describe_first_result_set
                string columnSql = $@"
                    SELECT c.name, t.name
                    FROM sys.columns c
                    JOIN sys.types t ON c.user_type_id = t.user_type_id
                    WHERE c.object_id = OBJECT_ID('{func.Schema}.{func.Name}')
                    ORDER BY c.column_id";
                
                using var columnCommand = new SqlCommand(columnSql, connection);
                using var columnReader = await columnCommand.ExecuteReaderAsync();
                
                if (columnReader.HasRows)
                {
                    while (await columnReader.ReadAsync())
                    {
                        string columnName = columnReader.GetString(0);
                        string typeName = columnReader.GetString(1);
                        Console.WriteLine($"  - {columnName}: {typeName}");
                    }
                }
                else
                {
                    Console.WriteLine("  Error getting result columns: There is already an open DataReader associated with this Command which must be closed first.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error getting result columns: {ex.Message}");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Total Functions: {functionCount}");
    }

    private static string MaskConnectionString(string connectionString)
    {
        // Simple masking for any password in the connection string
        if (connectionString.Contains("Password="))
        {
            int start = connectionString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase) + 9;
            int end = connectionString.IndexOf(';', start);
            
            if (end == -1) // Password is the last part
                end = connectionString.Length;
                
            string password = connectionString.Substring(start, end - start);
            return connectionString.Replace(password, "********");
        }
        
        return connectionString;
    }
}
