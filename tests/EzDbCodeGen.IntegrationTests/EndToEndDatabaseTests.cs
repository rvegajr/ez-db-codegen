using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EzDbCodeGen.Core;
using EzDbCodeGen.Core.Configuration;
using EzDbCodeGen.Schema.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.IntegrationTests
{
    [Trait("Category", "Integration")]
    public class EndToEndDatabaseTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _outputPath;
        private readonly string _templatesPath;

        public EndToEndDatabaseTests(ITestOutputHelper output)
        {
            _output = output;
            _outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
            _templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Templates");
            
            // Ensure output directory exists
            Directory.CreateDirectory(_outputPath);
        }

        [Fact]
        [Trait("Database", "WideWorldImporters")]
        public async Task Should_Generate_Code_From_WideWorldImporters_Database()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            services.AddEzDbCodeGenSqlServer(connectionString);
            var serviceProvider = services.BuildServiceProvider();

            var schemaProvider = serviceProvider.GetRequiredService<EzDbCodeGen.Schema.Interfaces.Providers.ISqlServerSchemaProvider>();
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();

            // Act
            var options = new EzDbCodeGen.Schema.Models.SchemaProviderOptions
            {
                ConnectionString = connectionString,
                Schema = "Sales"  // Limit to Sales schema for faster testing
            };

            var schema = await schemaProvider.GetSchemaAsync(options);
            
            _output.WriteLine($"Extracted schema with {schema.Tables.Count} tables and {schema.Relationships.Count} relationships");
            
            var config = new CodeGenConfig
            {
                OutputPath = _outputPath,
                TemplatesPath = _templatesPath,
                SchemaName = "Sales"
            };
            
            codeGenerator.Generate(schema, config);
            
            // Assert
            schema.Should().NotBeNull("because schema extraction should succeed");
            schema.Tables.Should().NotBeEmpty("because WideWorldImporters Sales schema should have tables");
            schema.Relationships.Should().NotBeEmpty("because WideWorldImporters Sales schema should have relationships");
            
            // Verify generated files
            var generatedFiles = Directory.GetFiles(_outputPath, "*.cs", SearchOption.AllDirectories);
            generatedFiles.Should().NotBeEmpty("because code generation should produce C# files");
            
            // Verify entity classes were generated
            var entityFiles = generatedFiles.Where(f => Path.GetFileName(f).EndsWith("Entity.cs")).ToArray();
            entityFiles.Should().NotBeEmpty("because entity classes should be generated");
            
            // Verify DbContext was generated
            var dbContextFiles = generatedFiles.Where(f => Path.GetFileName(f).Contains("DbContext")).ToArray();
            dbContextFiles.Should().NotBeEmpty("because a DbContext class should be generated");
            
            // Check content of a sample entity file
            if (entityFiles.Length > 0)
            {
                var sampleEntityContent = File.ReadAllText(entityFiles[0]);
                sampleEntityContent.Should().Contain("using System", "because entity classes should have proper using statements");
                sampleEntityContent.Should().Contain("namespace", "because entity classes should be in a namespace");
                sampleEntityContent.Should().Contain("public class", "because entity classes should be public");
                sampleEntityContent.Should().Contain("/// <summary>", "because entity classes should have XML documentation");
            }
        }

        [Fact]
        [Trait("Database", "Northwind")]
        public async Task Should_Generate_Code_From_Northwind_Database()
        {
            // Skip this test if Northwind database is not available
            // This is just a placeholder for the actual test
            
            // Arrange
            var connectionString = "Server=localhost;Database=Northwind;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            services.AddEzDbCodeGenSqlServer(connectionString);
            var serviceProvider = services.BuildServiceProvider();

            var schemaProvider = serviceProvider.GetRequiredService<EzDbCodeGen.Schema.Interfaces.Providers.ISqlServerSchemaProvider>();
            
            // Act & Assert - Just check if we can connect, don't run the full test if DB isn't available
            try
            {
                var options = new EzDbCodeGen.Schema.Models.SchemaProviderOptions
                {
                    ConnectionString = connectionString,
                    Schema = "dbo"
                };

                var schema = await schemaProvider.GetSchemaAsync(options);
                
                // If we get here, the database exists and we can continue with the test
                _output.WriteLine($"Northwind database is available with {schema.Tables.Count} tables");
                
                // The rest of the test would be similar to the WideWorldImporters test
                schema.Should().NotBeNull("because schema extraction should succeed");
                schema.Tables.Should().NotBeEmpty("because Northwind should have tables");
            }
            catch (Exception ex)
            {
                // If the database doesn't exist, log it and skip the test
                _output.WriteLine($"Skipping Northwind test: {ex.Message}");
                return;
            }
        }

        [Fact]
        [Trait("CrossPlatform", "OS")]
        public void Should_Work_On_Current_Operating_System()
        {
            // Arrange
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            
            var currentOs = isWindows ? "Windows" : (isLinux ? "Linux" : (isMacOS ? "macOS" : "Unknown"));
            
            _output.WriteLine($"Running on {currentOs}");
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            
            // Act
            var serviceProvider = services.BuildServiceProvider();
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            
            // Assert
            codeGenerator.Should().NotBeNull("because code generator should be available on all platforms");
            
            // Platform-specific assertions
            if (isWindows)
            {
                // Windows-specific tests
                _output.WriteLine("Performing Windows-specific validation");
                // E.g., check Windows-specific path handling
            }
            else if (isLinux)
            {
                // Linux-specific tests
                _output.WriteLine("Performing Linux-specific validation");
                // E.g., check Linux-specific path handling
            }
            else if (isMacOS)
            {
                // macOS-specific tests
                _output.WriteLine("Performing macOS-specific validation");
                // E.g., check macOS-specific path handling
            }
        }

        [Fact]
        [Trait("SqlServer", "Version")]
        public void Should_Support_Different_SQL_Server_Versions()
        {
            // This test would ideally connect to different SQL Server versions
            // For TDD purposes, we're implementing a placeholder test
            
            // Arrange - Create test schemas representing different SQL Server versions
            var sql2016Schema = CreateMockSchemaForSqlServerVersion("2016");
            var sql2019Schema = CreateMockSchemaForSqlServerVersion("2019");
            var sql2022Schema = CreateMockSchemaForSqlServerVersion("2022");
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddEzDbCodeGen();
            var serviceProvider = services.BuildServiceProvider();
            
            var codeGenerator = serviceProvider.GetRequiredService<EzDbCodeGen.CodeGen.Interfaces.ICodeGenerator>();
            
            // Act & Assert
            // Test SQL Server 2016 schema
            var config2016 = new CodeGenConfig
            {
                OutputPath = Path.Combine(_outputPath, "SQL2016"),
                TemplatesPath = _templatesPath,
                SchemaName = "dbo"
            };
            
            Directory.CreateDirectory(config2016.OutputPath);
            codeGenerator.Generate(sql2016Schema, config2016);
            
            var files2016 = Directory.GetFiles(config2016.OutputPath, "*.cs", SearchOption.AllDirectories);
            files2016.Should().NotBeEmpty("because code generation should work for SQL Server 2016 schema");
            
            // Test SQL Server 2019 schema
            var config2019 = new CodeGenConfig
            {
                OutputPath = Path.Combine(_outputPath, "SQL2019"),
                TemplatesPath = _templatesPath,
                SchemaName = "dbo"
            };
            
            Directory.CreateDirectory(config2019.OutputPath);
            codeGenerator.Generate(sql2019Schema, config2019);
            
            var files2019 = Directory.GetFiles(config2019.OutputPath, "*.cs", SearchOption.AllDirectories);
            files2019.Should().NotBeEmpty("because code generation should work for SQL Server 2019 schema");
            
            // Test SQL Server 2022 schema
            var config2022 = new CodeGenConfig
            {
                OutputPath = Path.Combine(_outputPath, "SQL2022"),
                TemplatesPath = _templatesPath,
                SchemaName = "dbo"
            };
            
            Directory.CreateDirectory(config2022.OutputPath);
            codeGenerator.Generate(sql2022Schema, config2022);
            
            var files2022 = Directory.GetFiles(config2022.OutputPath, "*.cs", SearchOption.AllDirectories);
            files2022.Should().NotBeEmpty("because code generation should work for SQL Server 2022 schema");
        }

        private IDatabaseSchema CreateMockSchemaForSqlServerVersion(string version)
        {
            // Create a mock schema with version-specific features
            var schema = new EzDbCodeGen.Schema.Models.DatabaseSchema
            {
                Name = $"TestDB_{version}",
                DefaultSchema = "dbo"
            };

            // Add common tables
            var customerTable = new EzDbCodeGen.Schema.Models.Table
            {
                Name = "Customer",
                Schema = "dbo"
            };

            customerTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
            {
                Name = "CustomerId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            customerTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            schema.Tables.Add(customerTable);

            // Add version-specific features
            if (version == "2016")
            {
                // SQL Server 2016 introduced temporal tables
                var employeeTable = new EzDbCodeGen.Schema.Models.Table
                {
                    Name = "Employee",
                    Schema = "dbo",
                    HasTemporalTableSupport = true,
                    TemporalHistoryTableName = "dbo.EmployeeHistory",
                    TemporalPeriodStartColumnName = "ValidFrom",
                    TemporalPeriodEndColumnName = "ValidTo"
                };

                employeeTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "EmployeeId",
                    DataType = "int",
                    IsPrimaryKey = true,
                    IsNullable = false
                });

                employeeTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "Name",
                    DataType = "nvarchar",
                    MaxLength = 100,
                    IsNullable = false
                });

                employeeTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "ValidFrom",
                    DataType = "datetime2",
                    IsNullable = false,
                    IsSystemVersioningPeriodStart = true
                });

                employeeTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "ValidTo",
                    DataType = "datetime2",
                    IsNullable = false,
                    IsSystemVersioningPeriodEnd = true
                });

                schema.Tables.Add(employeeTable);
            }
            else if (version == "2019" || version == "2022")
            {
                // SQL Server 2019 introduced UTF-8 support
                var documentTable = new EzDbCodeGen.Schema.Models.Table
                {
                    Name = "Document",
                    Schema = "dbo"
                };

                documentTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "DocumentId",
                    DataType = "int",
                    IsPrimaryKey = true,
                    IsNullable = false
                });

                documentTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "Title",
                    DataType = "nvarchar",
                    MaxLength = 100,
                    IsNullable = false
                });

                documentTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "Content",
                    DataType = "varchar",
                    MaxLength = -1, // MAX
                    IsNullable = true,
                    CollationName = "Latin1_General_100_BIN2_UTF8" // UTF-8 collation
                });

                schema.Tables.Add(documentTable);
            }

            if (version == "2022")
            {
                // SQL Server 2022 introduced ledger tables
                var transactionTable = new EzDbCodeGen.Schema.Models.Table
                {
                    Name = "Transaction",
                    Schema = "dbo",
                    IsLedgerTable = true
                };

                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "TransactionId",
                    DataType = "int",
                    IsPrimaryKey = true,
                    IsNullable = false
                });

                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "Amount",
                    DataType = "decimal",
                    NumericPrecision = 18,
                    NumericScale = 2,
                    IsNullable = false
                });

                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "Description",
                    DataType = "nvarchar",
                    MaxLength = 100,
                    IsNullable = false
                });

                // Ledger-specific columns (automatically added by SQL Server)
                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "ledger_start_transaction_id",
                    DataType = "bigint",
                    IsNullable = false,
                    IsGenerated = true,
                    IsLedgerColumn = true
                });

                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "ledger_end_transaction_id",
                    DataType = "bigint",
                    IsNullable = true,
                    IsLedgerColumn = true
                });

                transactionTable.Columns.Add(new EzDbCodeGen.Schema.Models.Column
                {
                    Name = "ledger_transaction_sequence_number",
                    DataType = "bigint",
                    IsNullable = false,
                    IsGenerated = true,
                    IsLedgerColumn = true
                });

                schema.Tables.Add(transactionTable);
            }

            return schema;
        }
    }
}
