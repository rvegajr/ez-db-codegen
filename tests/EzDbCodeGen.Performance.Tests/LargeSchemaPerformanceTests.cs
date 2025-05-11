using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EzDbCodeGen.Core;
using EzDbCodeGen.Core.Configuration;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.Performance.Tests
{
    public class LargeSchemaPerformanceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IServiceProvider _serviceProvider;

        public LargeSchemaPerformanceTests(ITestOutputHelper output)
        {
            _output = output;

            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            services.AddEzDbCodeGenSqlServer("Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;");
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Should_Process_Large_Schema_Under_Memory_Threshold()
        {
            // Arrange
            var largeSchema = GenerateLargeSchema(tableCount: 100);
            var memoryBefore = GC.GetTotalMemory(true);
            
            // Act
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
            
            var sw = Stopwatch.StartNew();
            processor.Generate(largeSchema, config);
            sw.Stop();
            
            var memoryAfter = GC.GetTotalMemory(false);
            var memoryUsed = memoryAfter - memoryBefore;
            
            // Log results
            _output.WriteLine($"Generation Time: {sw.ElapsedMilliseconds}ms");
            _output.WriteLine($"Memory Used: {memoryUsed / 1024 / 1024}MB");
            
            // Assert
            memoryUsed.Should().BeLessThan(500 * 1024 * 1024, "because large schema processing should use less than 500MB of memory");
            sw.ElapsedMilliseconds.Should().BeLessThan(30000, "because large schema processing should complete in under 30 seconds");
        }

        [Fact]
        public void Should_Benefit_From_Template_Compilation_Caching()
        {
            // Arrange
            var schema = GenerateLargeSchema(tableCount: 20);
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
            
            // Act - First run (cold cache)
            var sw1 = Stopwatch.StartNew();
            processor.Generate(schema, config);
            sw1.Stop();
            
            // Act - Second run (warm cache)
            var sw2 = Stopwatch.StartNew();
            processor.Generate(schema, config);
            sw2.Stop();
            
            // Log results
            _output.WriteLine($"First run (cold cache): {sw1.ElapsedMilliseconds}ms");
            _output.WriteLine($"Second run (warm cache): {sw2.ElapsedMilliseconds}ms");
            _output.WriteLine($"Improvement: {(double)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds:F2}x faster");
            
            // Assert
            sw2.ElapsedMilliseconds.Should().BeLessThan(sw1.ElapsedMilliseconds * 0.7, 
                "because template compilation caching should make the second run at least 30% faster");
        }

        [Fact]
        public void Should_Process_Differential_Generation_Efficiently()
        {
            // Arrange
            var fullSchema = GenerateLargeSchema(tableCount: 50);
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            var config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
            
            // Act - Full generation
            var swFull = Stopwatch.StartNew();
            processor.Generate(fullSchema, config);
            swFull.Stop();
            
            // Create a differential schema (just 5 tables)
            var differentialSchema = new DatabaseSchema
            {
                Name = fullSchema.Name,
                DefaultSchema = fullSchema.DefaultSchema
            };
            
            // Add just 5 tables from the full schema
            for (int i = 0; i < 5; i++)
            {
                differentialSchema.Tables.Add(fullSchema.Tables[i]);
            }
            
            // Configure for differential generation
            config.DifferentialGeneration = true;
            
            // Act - Differential generation
            var swDiff = Stopwatch.StartNew();
            processor.Generate(differentialSchema, config);
            swDiff.Stop();
            
            // Log results
            _output.WriteLine($"Full generation: {swFull.ElapsedMilliseconds}ms");
            _output.WriteLine($"Differential generation: {swDiff.ElapsedMilliseconds}ms");
            _output.WriteLine($"Improvement: {(double)swFull.ElapsedMilliseconds / swDiff.ElapsedMilliseconds:F2}x faster");
            
            // Assert
            swDiff.ElapsedMilliseconds.Should().BeLessThan(swFull.ElapsedMilliseconds * 0.2, 
                "because differential generation of 10% of tables should be at least 5x faster than full generation");
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

                // Add some many-to-many relationships via junction tables
                if (i % 10 == 0 && i > 10)
                {
                    // Create a junction table
                    var junctionTable = new Table
                    {
                        Name = $"Table{i-5}_Table{i}_Junction",
                        Schema = "dbo",
                        Description = $"Junction table between Table{i-5} and Table{i}"
                    };

                    // Add composite primary key columns
                    junctionTable.Columns.Add(new Column
                    {
                        Name = $"Table{i-5}Id",
                        DataType = "int",
                        IsPrimaryKey = true,
                        IsNullable = false,
                        IsForeignKey = true,
                        Description = $"Foreign key to Table{i-5}, part of composite primary key"
                    });

                    junctionTable.Columns.Add(new Column
                    {
                        Name = $"Table{i}Id",
                        DataType = "int",
                        IsPrimaryKey = true,
                        IsNullable = false,
                        IsForeignKey = true,
                        Description = $"Foreign key to Table{i}, part of composite primary key"
                    });

                    // Add a payload column
                    junctionTable.Columns.Add(new Column
                    {
                        Name = "CreatedDate",
                        DataType = "datetime2",
                        IsNullable = false,
                        Description = "Date when the relationship was created"
                    });

                    // Add relationships for the junction table
                    var relationship1 = new Relationship
                    {
                        Name = $"FK_Junction_Table{i-5}",
                        PrimaryKeyTableName = $"Table{i-5}",
                        PrimaryKeyTableSchema = "dbo",
                        ForeignKeyTableName = $"Table{i-5}_Table{i}_Junction",
                        ForeignKeyTableSchema = "dbo"
                    };

                    relationship1.PrimaryKeyColumnNames.Add($"Table{i-5}Id");
                    relationship1.ForeignKeyColumnNames.Add($"Table{i-5}Id");

                    var relationship2 = new Relationship
                    {
                        Name = $"FK_Junction_Table{i}",
                        PrimaryKeyTableName = $"Table{i}",
                        PrimaryKeyTableSchema = "dbo",
                        ForeignKeyTableName = $"Table{i-5}_Table{i}_Junction",
                        ForeignKeyTableSchema = "dbo"
                    };

                    relationship2.PrimaryKeyColumnNames.Add($"Table{i}Id");
                    relationship2.ForeignKeyColumnNames.Add($"Table{i}Id");

                    schema.Tables.Add(junctionTable);
                    schema.Relationships.Add(relationship1);
                    schema.Relationships.Add(relationship2);
                }

                schema.Tables.Add(table);
            }

            return schema;
        }
    }

    // BenchmarkDotNet benchmarks for more detailed performance analysis
    [MemoryDiagnoser]
    public class LargeSchemaBenchmarks
    {
        private IDatabaseSchema _smallSchema;
        private IDatabaseSchema _mediumSchema;
        private IDatabaseSchema _largeSchema;
        private IServiceProvider _serviceProvider;
        private CodeGenConfig _config;

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            services.AddEzDbCodeGenSqlServer("Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;");
            _serviceProvider = services.BuildServiceProvider();

            // Create test schemas of different sizes
            _smallSchema = GenerateTestSchema(20);
            _mediumSchema = GenerateTestSchema(50);
            _largeSchema = GenerateTestSchema(100);

            _config = new CodeGenConfig
            {
                OutputPath = "./Output",
                TemplatesPath = "./Templates",
                SchemaName = "dbo"
            };
        }

        [Benchmark(Baseline = true)]
        public void SmallSchema_Generation()
        {
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            processor.Generate(_smallSchema, _config);
        }

        [Benchmark]
        public void MediumSchema_Generation()
        {
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            processor.Generate(_mediumSchema, _config);
        }

        [Benchmark]
        public void LargeSchema_Generation()
        {
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            processor.Generate(_largeSchema, _config);
        }

        [Benchmark]
        public void CachedTemplates_Generation()
        {
            var processor = _serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            // First run to warm up the cache
            processor.Generate(_smallSchema, _config);
            // Second run with warm cache
            processor.Generate(_smallSchema, _config);
        }

        private IDatabaseSchema GenerateTestSchema(int tableCount)
        {
            var schema = new DatabaseSchema
            {
                Name = "BenchmarkSchema",
                DefaultSchema = "dbo"
            };

            for (int i = 1; i <= tableCount; i++)
            {
                var table = new Table
                {
                    Name = $"BenchTable{i}",
                    Schema = "dbo"
                };

                // Add primary key
                table.Columns.Add(new Column
                {
                    Name = $"BenchTable{i}Id",
                    DataType = "int",
                    IsPrimaryKey = true,
                    IsNullable = false
                });

                // Add 10 columns per table
                for (int j = 1; j <= 10; j++)
                {
                    var column = new Column
                    {
                        Name = $"BenchColumn{j}",
                        IsNullable = j % 2 == 0
                    };

                    // Mix of different data types
                    switch (j % 5)
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
                    }

                    table.Columns.Add(column);
                }

                schema.Tables.Add(table);
            }

            return schema;
        }
    }
}
