using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core;
using EzDbCodeGen.Core.Configuration;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.EFCoreComparison.Tests.SuperiorityValidation
{
    public class PerformanceSuperiorityTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionString;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        public PerformanceSuperiorityTests(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            
            // Initialize comparers
            _efCoreComparer = new EFCoreModelComparer(_connectionString);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_connectionString);
        }

        [Fact]
        public async Task Should_Be_At_Least_50_Percent_Faster_Than_EFCore_For_Large_Schemas()
        {
            // Arrange
            var largeSchema = GenerateLargeSchema(tableCount: 100);
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            var serviceProvider = services.BuildServiceProvider();
            
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
            
            // Act - Measure EzDbCodeGen performance
            var ezDbCodeGenSw = Stopwatch.StartNew();
            var ezDbCodeGenMemoryBefore = GC.GetTotalMemory(true);
            
            codeGenerator.Generate(largeSchema, config);
            
            ezDbCodeGenSw.Stop();
            var ezDbCodeGenMemoryAfter = GC.GetTotalMemory(false);
            var ezDbCodeGenMemoryUsed = ezDbCodeGenMemoryAfter - ezDbCodeGenMemoryBefore;
            
            // Act - Measure EF Core performance (simulated)
            var efCoreSw = Stopwatch.StartNew();
            var efCoreMemoryBefore = GC.GetTotalMemory(true);
            
            // Simulate EF Core generation using our comparer
            await _efCoreComparer.SimulateEFCoreGenerationAsync(largeSchema);
            
            efCoreSw.Stop();
            var efCoreMemoryAfter = GC.GetTotalMemory(false);
            var efCoreMemoryUsed = efCoreMemoryAfter - efCoreMemoryBefore;
            
            // Log the results
            _output.WriteLine($"EF Core generation time: {efCoreSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"EzDbCodeGen generation time: {ezDbCodeGenSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"Speed improvement: {(double)efCoreSw.ElapsedMilliseconds / ezDbCodeGenSw.ElapsedMilliseconds:F2}x faster");
            
            _output.WriteLine($"EF Core memory used: {efCoreMemoryUsed / 1024 / 1024}MB");
            _output.WriteLine($"EzDbCodeGen memory used: {ezDbCodeGenMemoryUsed / 1024 / 1024}MB");
            _output.WriteLine($"Memory efficiency: {(double)efCoreMemoryUsed / ezDbCodeGenMemoryUsed:F2}x better");
            
            // Assert
            ezDbCodeGenSw.ElapsedMilliseconds.Should().BeLessThan(efCoreSw.ElapsedMilliseconds * 0.5,
                "because EzDbCodeGen should be at least 50% faster than EF Core for large schemas");
            
            ezDbCodeGenMemoryUsed.Should().BeLessThanOrEqualTo(efCoreMemoryUsed * 1.1,
                "because EzDbCodeGen should use less memory than EF Core (or at most 10% more)");
        }

        [Fact]
        public async Task Should_Scale_Better_With_Schema_Size()
        {
            // Arrange
            var smallSchema = GenerateLargeSchema(tableCount: 10);
            var mediumSchema = GenerateLargeSchema(tableCount: 50);
            var largeSchema = GenerateLargeSchema(tableCount: 100);
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            var serviceProvider = services.BuildServiceProvider();
            
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
            
            // Act - Measure EzDbCodeGen scaling
            var ezDbCodeGenSmallSw = Stopwatch.StartNew();
            codeGenerator.Generate(smallSchema, config);
            ezDbCodeGenSmallSw.Stop();
            
            var ezDbCodeGenMediumSw = Stopwatch.StartNew();
            codeGenerator.Generate(mediumSchema, config);
            ezDbCodeGenMediumSw.Stop();
            
            var ezDbCodeGenLargeSw = Stopwatch.StartNew();
            codeGenerator.Generate(largeSchema, config);
            ezDbCodeGenLargeSw.Stop();
            
            // Act - Measure EF Core scaling (simulated)
            var efCoreSmallSw = Stopwatch.StartNew();
            await _efCoreComparer.SimulateEFCoreGenerationAsync(smallSchema);
            efCoreSmallSw.Stop();
            
            var efCoreMediumSw = Stopwatch.StartNew();
            await _efCoreComparer.SimulateEFCoreGenerationAsync(mediumSchema);
            efCoreMediumSw.Stop();
            
            var efCoreLargeSw = Stopwatch.StartNew();
            await _efCoreComparer.SimulateEFCoreGenerationAsync(largeSchema);
            efCoreLargeSw.Stop();
            
            // Calculate scaling factors
            var ezDbCodeGenMediumToSmallRatio = (double)ezDbCodeGenMediumSw.ElapsedMilliseconds / ezDbCodeGenSmallSw.ElapsedMilliseconds;
            var ezDbCodeGenLargeToMediumRatio = (double)ezDbCodeGenLargeSw.ElapsedMilliseconds / ezDbCodeGenMediumSw.ElapsedMilliseconds;
            
            var efCoreMediumToSmallRatio = (double)efCoreMediumSw.ElapsedMilliseconds / efCoreSmallSw.ElapsedMilliseconds;
            var efCoreLargeToMediumRatio = (double)efCoreLargeSw.ElapsedMilliseconds / efCoreMediumSw.ElapsedMilliseconds;
            
            // Log the results
            _output.WriteLine("EzDbCodeGen scaling:");
            _output.WriteLine($"  Small schema: {ezDbCodeGenSmallSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"  Medium schema: {ezDbCodeGenMediumSw.ElapsedMilliseconds}ms (ratio: {ezDbCodeGenMediumToSmallRatio:F2}x)");
            _output.WriteLine($"  Large schema: {ezDbCodeGenLargeSw.ElapsedMilliseconds}ms (ratio: {ezDbCodeGenLargeToMediumRatio:F2}x)");
            
            _output.WriteLine("EF Core scaling:");
            _output.WriteLine($"  Small schema: {efCoreSmallSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"  Medium schema: {efCoreMediumSw.ElapsedMilliseconds}ms (ratio: {efCoreMediumToSmallRatio:F2}x)");
            _output.WriteLine($"  Large schema: {efCoreLargeSw.ElapsedMilliseconds}ms (ratio: {efCoreLargeToMediumRatio:F2}x)");
            
            // Assert
            ezDbCodeGenMediumToSmallRatio.Should().BeLessThan(efCoreMediumToSmallRatio,
                "because EzDbCodeGen should scale better from small to medium schemas than EF Core");
            
            ezDbCodeGenLargeToMediumRatio.Should().BeLessThan(efCoreLargeToMediumRatio,
                "because EzDbCodeGen should scale better from medium to large schemas than EF Core");
        }

        [Fact]
        public async Task Should_Complete_Generation_In_Under_Half_The_Time_Of_EFCore()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            services.AddEzDbCodeGenSqlServer(_connectionString);
            var serviceProvider = services.BuildServiceProvider();
            
            var schemaProvider = serviceProvider.GetRequiredService<EzDbCodeGen.Schema.Interfaces.Providers.ISqlServerSchemaProvider>();
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            
            // Act - Extract schema
            var options = new SchemaProviderOptions
            {
                ConnectionString = _connectionString,
                Schema = string.Join(",", schemas)
            };
            
            var schema = await schemaProvider.GetSchemaAsync(options);
            
            // Act - Measure EzDbCodeGen performance
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = string.Join(",", schemas)
            };
            
            var ezDbCodeGenSw = Stopwatch.StartNew();
            codeGenerator.Generate(schema, config);
            ezDbCodeGenSw.Stop();
            
            // Act - Measure EF Core performance (simulated)
            var efCoreSw = Stopwatch.StartNew();
            await _efCoreComparer.SimulateEFCoreGenerationAsync(schema);
            efCoreSw.Stop();
            
            // Log the results
            _output.WriteLine($"EF Core generation time: {efCoreSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"EzDbCodeGen generation time: {ezDbCodeGenSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"Speed improvement: {(double)efCoreSw.ElapsedMilliseconds / ezDbCodeGenSw.ElapsedMilliseconds:F2}x faster");
            
            // Assert
            ezDbCodeGenSw.ElapsedMilliseconds.Should().BeLessThan(efCoreSw.ElapsedMilliseconds * 0.5,
                "because EzDbCodeGen should complete generation in under half the time of EF Core");
        }

        private IDatabaseSchema GenerateLargeSchema(int tableCount)
        {
            var schema = new DatabaseSchema
            {
                Name = "LargeTestSchema",
                DefaultSchema = "dbo"
            };

            for (int i = 1; i <= tableCount; i++)
            {
                var table = new Table
                {
                    Name = $"Table{i}",
                    Schema = "dbo",
                    Description = $"Test table {i} for performance testing"
                };

                // Add primary key
                table.Columns.Add(new Column
                {
                    Name = $"Table{i}Id",
                    DataType = "int",
                    IsPrimaryKey = true,
                    IsNullable = false,
                    Description = $"Primary key for Table{i}"
                });

                // Add 10-20 columns per table
                int columnCount = 10 + (i % 10);
                for (int j = 1; j <= columnCount; j++)
                {
                    var column = new Column
                    {
                        Name = $"Column{j}",
                        IsNullable = j % 2 == 0,
                        Description = $"Column {j} for Table{i}"
                    };

                    // Mix of different data types
                    switch (j % 8)
                    {
                        case 0:
                            column.DataType = "int";
                            break;
                        case 1:
                            column.DataType = "nvarchar";
                            column.MaxLength = 100;
                            break;
                        case 2:
                            column.DataType = "decimal";
                            column.NumericPrecision = 18;
                            column.NumericScale = 2;
                            break;
                        case 3:
                            column.DataType = "datetime2";
                            break;
                        case 4:
                            column.DataType = "bit";
                            break;
                        case 5:
                            column.DataType = "uniqueidentifier";
                            break;
                        case 6:
                            column.DataType = "varbinary";
                            column.MaxLength = -1; // MAX
                            break;
                        case 7:
                            column.DataType = "xml";
                            break;
                    }

                    table.Columns.Add(column);
                }

                // Add foreign keys for some tables (to create relationships)
                if (i > 1 && i % 3 == 0)
                {
                    // Reference to previous table
                    var fkColumn = new Column
                    {
                        Name = $"Table{i-1}Id",
                        DataType = "int",
                        IsNullable = false,
                        IsForeignKey = true,
                        Description = $"Foreign key to Table{i-1}"
                    };
                    table.Columns.Add(fkColumn);

                    // Add the relationship
                    var relationship = new Relationship
                    {
                        Name = $"FK_Table{i}_Table{i-1}",
                        PrimaryKeyTableName = $"Table{i-1}",
                        PrimaryKeyTableSchema = "dbo",
                        ForeignKeyTableName = $"Table{i}",
                        ForeignKeyTableSchema = "dbo"
                    };

                    relationship.PrimaryKeyColumnNames.Add($"Table{i-1}Id");
                    relationship.ForeignKeyColumnNames.Add($"Table{i-1}Id");

                    schema.Relationships.Add(relationship);
                }

                schema.Tables.Add(table);
            }

            return schema;
        }
    }
}
