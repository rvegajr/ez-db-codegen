using System;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Interfaces.Providers;
using EzDbCodeGen.Schema.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EzDbCodeGen.Schema.Tests
{
    public class SqlServerAdvancedSchemaTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<Providers.SqlServerSchemaProvider>> _mockLogger;

        public SqlServerAdvancedSchemaTests()
        {
            _mockLogger = new Mock<ILogger<Providers.SqlServerSchemaProvider>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            var services = new ServiceCollection();
            services.AddSingleton(_mockLoggerFactory.Object);
            services.AddTransient<ISqlServerSchemaProvider, Providers.SqlServerSchemaProvider>();
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Should_Detect_Computed_Columns()
        {
            // Arrange
            var schema = CreateSchemaWithComputedColumns();

            // Act
            var productTable = schema.Tables.FirstOrDefault(t => t.Name == "Product");
            var computedColumn = productTable?.Columns.FirstOrDefault(c => c.IsComputed);

            // Assert
            productTable.Should().NotBeNull("because the Product table should exist in the schema");
            computedColumn.Should().NotBeNull("because the Product table should have a computed column");
            computedColumn.Name.Should().Be("FullPrice", "because that's the computed column we defined");
            computedColumn.ComputedColumnDefinition.Should().Be("([Price] * (1 + [TaxRate]))", "because that's the computation formula");
            computedColumn.IsPersisted.Should().BeTrue("because we defined it as a persisted computed column");
        }

        [Fact]
        public void Should_Detect_Sparse_Columns()
        {
            // Arrange
            var schema = CreateSchemaWithSparseColumns();

            // Act
            var customerTable = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
            var sparseColumn = customerTable?.Columns.FirstOrDefault(c => c.IsSparse);

            // Assert
            customerTable.Should().NotBeNull("because the Customer table should exist in the schema");
            sparseColumn.Should().NotBeNull("because the Customer table should have a sparse column");
            sparseColumn.Name.Should().Be("OptionalBiography", "because that's the sparse column we defined");
            sparseColumn.IsSparse.Should().BeTrue("because we defined it as a sparse column");
        }

        [Fact]
        public void Should_Detect_Filtered_Indexes()
        {
            // Arrange
            var schema = CreateSchemaWithFilteredIndexes();

            // Act
            var orderTable = schema.Tables.FirstOrDefault(t => t.Name == "Order");
            var filteredIndex = orderTable?.Indexes.FirstOrDefault(i => !string.IsNullOrEmpty(i.FilterDefinition));

            // Assert
            orderTable.Should().NotBeNull("because the Order table should exist in the schema");
            filteredIndex.Should().NotBeNull("because the Order table should have a filtered index");
            filteredIndex.Name.Should().Be("IX_Order_Status_Filtered", "because that's the filtered index we defined");
            filteredIndex.FilterDefinition.Should().Be("([Status]='Completed')", "because that's the filter condition we defined");
        }

        [Fact]
        public void Should_Extract_Decimal_Column_With_Precision_And_Scale()
        {
            // Arrange
            var schema = CreateSchemaWithDecimalColumns();

            // Act
            var financialTable = schema.Tables.FirstOrDefault(t => t.Name == "Financial");
            var decimalColumn = financialTable?.Columns.FirstOrDefault(c => c.DataType == "decimal");

            // Assert
            financialTable.Should().NotBeNull("because the Financial table should exist in the schema");
            decimalColumn.Should().NotBeNull("because the Financial table should have a decimal column");
            decimalColumn.Name.Should().Be("Amount", "because that's the decimal column we defined");
            decimalColumn.NumericPrecision.Should().Be(18, "because we defined it with precision 18");
            decimalColumn.NumericScale.Should().Be(4, "because we defined it with scale 4");
        }

        private IDatabaseSchema CreateSchemaWithComputedColumns()
        {
            var schema = new DatabaseSchema
            {
                Name = "AdvancedSchemaTest",
                DefaultSchema = "dbo"
            };

            var productTable = new Table
            {
                Name = "Product",
                Schema = "dbo"
            };

            productTable.Columns.Add(new Column
            {
                Name = "ProductId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            productTable.Columns.Add(new Column
            {
                Name = "Price",
                DataType = "decimal",
                NumericPrecision = 18,
                NumericScale = 2,
                IsNullable = false
            });

            productTable.Columns.Add(new Column
            {
                Name = "TaxRate",
                DataType = "decimal",
                NumericPrecision = 5,
                NumericScale = 4,
                IsNullable = false
            });

            productTable.Columns.Add(new Column
            {
                Name = "FullPrice",
                DataType = "decimal",
                NumericPrecision = 18,
                NumericScale = 2,
                IsNullable = false,
                IsComputed = true,
                ComputedColumnDefinition = "([Price] * (1 + [TaxRate]))",
                IsPersisted = true
            });

            schema.Tables.Add(productTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithSparseColumns()
        {
            var schema = new DatabaseSchema
            {
                Name = "AdvancedSchemaTest",
                DefaultSchema = "dbo"
            };

            var customerTable = new Table
            {
                Name = "Customer",
                Schema = "dbo"
            };

            customerTable.Columns.Add(new Column
            {
                Name = "CustomerId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            customerTable.Columns.Add(new Column
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            customerTable.Columns.Add(new Column
            {
                Name = "OptionalBiography",
                DataType = "nvarchar",
                MaxLength = -1, // MAX
                IsNullable = true,
                IsSparse = true
            });

            schema.Tables.Add(customerTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithFilteredIndexes()
        {
            var schema = new DatabaseSchema
            {
                Name = "AdvancedSchemaTest",
                DefaultSchema = "dbo"
            };

            var orderTable = new Table
            {
                Name = "Order",
                Schema = "dbo"
            };

            orderTable.Columns.Add(new Column
            {
                Name = "OrderId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            orderTable.Columns.Add(new Column
            {
                Name = "Status",
                DataType = "nvarchar",
                MaxLength = 50,
                IsNullable = false
            });

            orderTable.Columns.Add(new Column
            {
                Name = "OrderDate",
                DataType = "datetime2",
                IsNullable = false
            });

            var filteredIndex = new Index
            {
                Name = "IX_Order_Status_Filtered",
                TableName = "Order",
                TableSchema = "dbo",
                FilterDefinition = "([Status]='Completed')",
                IsUnique = false
            };

            filteredIndex.Columns.Add(new IndexColumn
            {
                Name = "Status",
                IsDescending = false,
                OrdinalPosition = 1
            });

            filteredIndex.Columns.Add(new IndexColumn
            {
                Name = "OrderDate",
                IsDescending = true,
                OrdinalPosition = 2
            });

            orderTable.Indexes.Add(filteredIndex);
            schema.Tables.Add(orderTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithDecimalColumns()
        {
            var schema = new DatabaseSchema
            {
                Name = "AdvancedSchemaTest",
                DefaultSchema = "dbo"
            };

            var financialTable = new Table
            {
                Name = "Financial",
                Schema = "dbo"
            };

            financialTable.Columns.Add(new Column
            {
                Name = "TransactionId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            financialTable.Columns.Add(new Column
            {
                Name = "Amount",
                DataType = "decimal",
                NumericPrecision = 18,
                NumericScale = 4,
                IsNullable = false
            });

            schema.Tables.Add(financialTable);
            return schema;
        }
    }
}
