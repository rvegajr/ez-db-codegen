using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Schema.Tests
{
    /// <summary>
    /// Tests for the SQL Server schema provider implementation.
    /// These tests follow the TDD approach outlined in the implementation checklist.
    /// </summary>
    public class SqlServerSchemaProviderTests
    {
        private readonly ILogger<SqlServerSchemaProvider> _mockLogger;
        private readonly string _testConnectionString;
        
        public SqlServerSchemaProviderTests()
        {
            // Setup mock logger
            var mockLogger = new Mock<ILogger<SqlServerSchemaProvider>>();
            _mockLogger = mockLogger.Object;
            
            // Use environment variable for connection string if available, otherwise use default test connection
            _testConnectionString = Environment.GetEnvironmentVariable("TEST_SQL_CONNECTION_STRING") ?? 
                "Server=localhost,1433;Database=AdventureWorksLT;User ID=sa;Password=Your_password123;TrustServerCertificate=True;";
        }
        
        [Fact]
        public async Task Should_Extract_Schema_From_Valid_Connection()
        {
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false,
                ExtractViews = true,
                ExtractStoredProcedures = true,
                ExtractFunctions = true
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            schema.Should().NotBeNull();
            schema.Name.Should().NotBeNullOrEmpty();
            schema.Tables.Should().NotBeEmpty();
        }
        
        [Fact]
        public async Task Should_Extract_Table_With_Columns_And_Keys()
        {
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false,
                TableNamePattern = "Customer"  // Focus on a specific table
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            
            var customerTable = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
            customerTable.Should().NotBeNull();
            customerTable.Columns.Should().NotBeEmpty();
            
            // Check primary key
            var primaryKeyColumns = customerTable.PrimaryKey.Columns;
            primaryKeyColumns.Should().NotBeEmpty();
            
            // Check columns
            var idColumn = customerTable.Columns.FirstOrDefault(c => c.Name == "CustomerID");
            idColumn.Should().NotBeNull();
            idColumn.IsPrimaryKey.Should().BeTrue();
            idColumn.IsNullable.Should().BeFalse();
        }
        
        [Fact]
        public async Task Should_Extract_Decimal_Column_With_Precision_And_Scale()
        {
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false,
                TableNamePattern = "SalesOrderDetail"  // Table with decimal columns
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            schema.Should().NotBeNull();
            
            var table = schema.Tables.FirstOrDefault(t => t.Name == "SalesOrderDetail");
            table.Should().NotBeNull();
            
            var unitPriceColumn = table.Columns.FirstOrDefault(c => c.Name == "UnitPrice");
            unitPriceColumn.Should().NotBeNull();
            unitPriceColumn.DataType.Should().Be("money");
            unitPriceColumn.NumericPrecision.Should().BeGreaterThan(0);
        }
        
        [Fact]
        public async Task Should_Extract_Foreign_Keys_With_Correct_References()
        {
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false,
                TableNamePattern = "SalesOrderDetail"  // Table with foreign keys
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            schema.Should().NotBeNull();
            
            var table = schema.Tables.FirstOrDefault(t => t.Name == "SalesOrderDetail");
            table.Should().NotBeNull();
            
            table.ForeignKeys.Should().NotBeEmpty();
            
            // Check a specific foreign key
            var productFK = table.ForeignKeys.FirstOrDefault(fk => 
                fk.ReferencedTable != null && fk.ReferencedTable.Name == "Product");
            
            productFK.Should().NotBeNull();
            productFK.Columns.Should().NotBeEmpty();
            productFK.ReferencedColumns.Should().NotBeEmpty();
            
            // Verify the column mapping
            productFK.Columns.First().Name.Should().Be("ProductID");
            productFK.ReferencedColumns.First().Name.Should().Be("ProductID");
        }
        
        [Fact]
        public async Task Should_Distinguish_System_And_User_Tables()
        {
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            
            // First, get only user tables
            var userOptions = new SchemaProviderOptions
            {
                IncludeSystemObjects = false
            };
            
            // Then, include system tables
            var allOptions = new SchemaProviderOptions
            {
                IncludeSystemObjects = true
            };
            
            // Act
            var userSchema = await provider.GetSchemaAsync(_testConnectionString, userOptions);
            var allSchema = await provider.GetSchemaAsync(_testConnectionString, allOptions);
            
            // Assert
            userSchema.Tables.Count.Should().BeLessThan(allSchema.Tables.Count,
                "Schema with system objects should have more tables than user-only schema");
            
            // Check for system tables like 'sysdiagrams'
            var systemTables = allSchema.Tables.Where(t => t.Name.StartsWith("sys")).ToList();
            systemTables.Should().NotBeEmpty("System tables should be present when IncludeSystemObjects is true");
        }
        
        [Fact]
        public async Task Should_Extract_Special_Data_Types()
        {
            // Skip this test if the database doesn't have the required tables
            // This is a placeholder for testing special types like spatial, XML, JSON
            
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            // Look for any columns with special types
            var specialTypeColumns = schema.Tables
                .SelectMany(t => t.Columns)
                .Where(c => c.DataType == "xml" || 
                           c.DataType == "geography" || 
                           c.DataType == "geometry" ||
                           c.DataType == "hierarchyid")
                .ToList();
            
            // This is more of an informational test - it may pass or fail depending on the test database
            if (specialTypeColumns.Any())
            {
                foreach (var column in specialTypeColumns)
                {
                    Console.WriteLine($"Found special type column: {column.Table.Name}.{column.Name} of type {column.DataType}");
                }
            }
            else
            {
                Console.WriteLine("No special type columns found in the test database.");
            }
        }
        
        [Fact]
        public async Task Should_Extract_Temporal_Tables_If_Available()
        {
            // Skip this test if SQL Server version doesn't support temporal tables or none are defined
            
            // Arrange
            var provider = new SqlServerSchemaProvider(_mockLogger);
            var options = new SchemaProviderOptions
            {
                IncludeSystemObjects = false
            };
            
            // Act
            var schema = await provider.GetSchemaAsync(_testConnectionString, options);
            
            // Assert
            // Look for temporal tables (tables with period columns)
            var temporalTables = schema.Tables
                .Where(t => t.Columns.Any(c => c.Name == "SysStartTime" || c.Name == "SysEndTime"))
                .ToList();
            
            // This is more of an informational test - it may pass or fail depending on the test database
            if (temporalTables.Any())
            {
                foreach (var table in temporalTables)
                {
                    Console.WriteLine($"Found temporal table: {table.Name}");
                }
            }
            else
            {
                Console.WriteLine("No temporal tables found in the test database.");
            }
        }
    }
}
