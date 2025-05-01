using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Providers;

namespace SchemaProviderTester
{
    /// <summary>
    /// Simple console application to test the enhanced SQL Server schema provider.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: dotnet run -- \"Server=your-server;Database=your-database;User ID=your-user;Password=your-password;TrustServerCertificate=True;\"");
                return;
            }

            string connectionString = args[0];
            string objectType = args.Length > 1 ? args[1] : "all";

            Console.WriteLine($"Connecting to: {MaskConnectionString(connectionString)}");
            
            try
            {
                // Setup logging
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Debug);
                });
                
                var logger = loggerFactory.CreateLogger<SqlServerSchemaProvider>();
                
                // Test connection
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connection successful!");
                    Console.WriteLine($"Server Version: {connection.ServerVersion}");
                    Console.WriteLine($"Database: {connection.Database}");
                    
                    // Now extract schema information using the enhanced provider
                    await ExtractDatabaseSchemaWithProvider(connectionString, objectType, logger);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }

        private static async Task ExtractDatabaseSchemaWithProvider(string connectionString, string objectType, ILogger<SqlServerSchemaProvider> logger)
        {
            Console.WriteLine("\nExtracting Schema Information using SqlServerSchemaProvider...");
            
            // Create the schema provider
            var provider = new SqlServerSchemaProvider(logger);
            
            // Configure options based on the requested object type
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = objectType == "system",
                IncludeViews = objectType == "all" || objectType == "views",
                IncludeStoredProcedures = objectType == "all" || objectType == "procs",
                IncludeFunctions = objectType == "all" || objectType == "functions",
                CommandTimeout = 60
            };
            
            // Apply table filter if specified
            if (objectType != "all" && objectType != "system" && objectType != "views" && 
                objectType != "procs" && objectType != "functions")
            {
                options.IncludeTables = new List<string> { objectType };
            }
            
            // Extract the schema
            var schema = await provider.GetSchemaAsync(connectionString, options);
            
            // Display schema information
            Console.WriteLine($"\nDatabase: {schema.Name}");
            Console.WriteLine($"Tables: {schema.Tables.Count}");
            Console.WriteLine($"Views: {schema.Views.Count}");
            Console.WriteLine($"Stored Procedures: {schema.StoredProcedures.Count}");
            Console.WriteLine($"Functions: {schema.Functions.Count}");
            
            // Display tables
            if (schema.Tables.Count > 0)
            {
                Console.WriteLine("\n--- Tables ---");
                foreach (var table in schema.Tables)
                {
                    Console.WriteLine($"Table: {table.Schema}.{table.Name} ({table.Columns.Count} columns)");
                    
                    // Display primary key
                    if (table.PrimaryKey != null)
                    {
                        var pkColumns = string.Join(", ", table.PrimaryKey.Columns.Select(c => c.Name));
                        Console.WriteLine($"  Primary Key: {table.PrimaryKey.Name} ({pkColumns})");
                    }
                    
                    // Display foreign keys
                    if (table.ForeignKeys.Any())
                    {
                        Console.WriteLine("  Foreign Keys:");
                        foreach (var fk in table.ForeignKeys)
                        {
                            Console.WriteLine($"    {fk.Name}: {table.Schema}.{table.Name} -> {fk.ReferencedTable.Schema}.{fk.ReferencedTable.Name}");
                        }
                    }
                    
                    // Display special columns (if any)
                    var specialColumns = table.Columns.Where(c => 
                        c.DataType == "xml" || 
                        c.DataType == "geography" || 
                        c.DataType == "geometry" ||
                        c.DataType == "hierarchyid" ||
                        (c.ExtendedProperties != null && c.ExtendedProperties.ContainsKey("PotentialJsonColumn")))
                        .ToList();
                    
                    if (specialColumns.Any())
                    {
                        Console.WriteLine("  Special Columns:");
                        foreach (var column in specialColumns)
                        {
                            Console.WriteLine($"    {column.Name} ({column.DataType})");
                            if (column.ExtendedProperties != null)
                            {
                                foreach (var prop in column.ExtendedProperties)
                                {
                                    Console.WriteLine($"      {prop.Key}: {prop.Value}");
                                }
                            }
                        }
                    }
                    
                    // Check if this is a temporal table
                    if (table.ExtendedProperties != null && table.ExtendedProperties.ContainsKey("IsTemporal"))
                    {
                        Console.WriteLine("  Temporal Table:");
                        Console.WriteLine($"    Period Start: {table.ExtendedProperties["PeriodStartColumn"]}");
                        Console.WriteLine($"    Period End: {table.ExtendedProperties["PeriodEndColumn"]}");
                        if (table.ExtendedProperties.ContainsKey("HistoryTable"))
                        {
                            Console.WriteLine($"    History Table: {table.ExtendedProperties["HistoryTable"]}");
                        }
                    }
                }
            }
            
            // Display views if requested
            if (options.IncludeViews && schema.Views.Count > 0)
            {
                Console.WriteLine("\n--- Views ---");
                foreach (var view in schema.Views)
                {
                    Console.WriteLine($"View: {view.Schema}.{view.Name} ({view.Columns.Count} columns)");
                }
            }
            
            // Display stored procedures if requested
            if (options.IncludeStoredProcedures && schema.StoredProcedures.Count > 0)
            {
                Console.WriteLine("\n--- Stored Procedures ---");
                foreach (var proc in schema.StoredProcedures)
                {
                    Console.WriteLine($"Procedure: {proc.Schema}.{proc.Name} ({proc.Parameters.Count} parameters)");
                }
            }
            
            // Display functions if requested
            if (options.IncludeFunctions && schema.Functions.Count > 0)
            {
                Console.WriteLine("\n--- Functions ---");
                foreach (var func in schema.Functions)
                {
                    Console.WriteLine($"Function: {func.Schema}.{func.Name} ({func.Parameters.Count} parameters)");
                    Console.WriteLine($"  Type: {func.FunctionType}");
                    
                    if (func.FunctionType == FunctionType.TableValued)
                    {
                        Console.WriteLine($"  Return Columns: {func.ReturnColumns.Count}");
                    }
                }
            }
        }

        private static string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }

            var parts = connectionString.Split(';');
            var maskedParts = new string[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part.StartsWith("Password=", StringComparison.OrdinalIgnoreCase))
                {
                    maskedParts[i] = "Password=********";
                }
                else
                {
                    maskedParts[i] = part;
                }
            }

            return string.Join(";", maskedParts);
        }
    }
}
