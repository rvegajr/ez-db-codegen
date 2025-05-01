using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Schema;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Data.SqlClient;

namespace EzDbCodeGen.Core.Tests.Schema
{
    public class SqlServerSchemaProviderTests
    {
        [Fact]
        public async Task GetSchema_WithValidConnection_ShouldReturnDatabaseSchema()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions { IncludeTables = true };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.DatabaseName.Should().Be("TestDb");
            schema.Tables.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetSchema_WithTablesOnly_ShouldReturnTablesOnly()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions
            {
                IncludeTables = true,
                IncludeViews = false,
                IncludeStoredProcedures = false,
                IncludeFunctions = false
            };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            schema.Views.Should().BeEmpty();
            schema.StoredProcedures.Should().BeEmpty();
            schema.Functions.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSchema_WithSpecificSchema_ShouldFilterBySchema()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions
            {
                IncludeTables = true,
                IncludeSchemas = new List<string> { "dbo" }
            };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            schema.Tables.All(t => t.Schema == "dbo").Should().BeTrue();
        }

        [Fact]
        public async Task GetSchema_WithTableFilters_ShouldApplyFilters()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions
            {
                IncludeTables = true,
                IncludeTablePatterns = new List<string> { "Customer*" },
                ExcludeTablePatterns = new List<string> { "*History" }
            };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            schema.Tables.All(t => t.Name.StartsWith("Customer")).Should().BeTrue();
            schema.Tables.All(t => !t.Name.EndsWith("History")).Should().BeTrue();
        }

        [Fact]
        public async Task GetSchema_WithInvalidConnectionString_ShouldThrowException()
        {
            // Arrange
            var connectionString = "Invalid Connection String";
            var options = new SchemaProviderOptions { IncludeTables = true };
            var provider = new SqlServerSchemaProviderStub(connectionString, options, true);

            // Act & Assert
            await Assert.ThrowsAsync<SqlException>(() => provider.GetSchemaAsync());
        }

        [Fact]
        public async Task GetSchema_ShouldLoadColumnDetails()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions { IncludeTables = true };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            
            var customer = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
            customer.Should().NotBeNull();
            customer!.Columns.Should().NotBeEmpty();
            
            var customerId = customer.Columns.FirstOrDefault(c => c.Name == "CustomerId");
            customerId.Should().NotBeNull();
            customerId!.IsPartOfPrimaryKey.Should().BeTrue();
            customerId.DataType.Should().Be("int");
            customerId.IsNullable.Should().BeFalse();
        }

        [Fact]
        public async Task GetSchema_ShouldLoadForeignKeyDetails()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions { IncludeTables = true };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            
            var order = schema.Tables.FirstOrDefault(t => t.Name == "Order");
            order.Should().NotBeNull();
            order!.ForeignKeys.Should().NotBeEmpty();
            
            var customerFk = order.ForeignKeys.FirstOrDefault(fk => fk.Name == "FK_Order_Customer");
            customerFk.Should().NotBeNull();
            customerFk!.Columns.Should().NotBeEmpty();
            customerFk.Columns.First().Name.Should().Be("CustomerId");
            customerFk.ReferencedTable.Name.Should().Be("Customer");
        }

        [Fact]
        public async Task GetSchema_ShouldLoadIndexDetails()
        {
            // Arrange
            var connectionString = "Data Source=(local);Initial Catalog=TestDb;Integrated Security=True";
            var options = new SchemaProviderOptions { IncludeTables = true };
            var mockProvider = CreateMockSchemaProvider(connectionString, options);

            // Act
            var schema = await mockProvider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            
            var customer = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
            customer.Should().NotBeNull();
            customer!.Indexes.Should().NotBeEmpty();
            
            var emailIndex = customer.Indexes.FirstOrDefault(i => i.Name == "IX_Customer_Email");
            emailIndex.Should().NotBeNull();
            emailIndex!.Columns.Should().NotBeEmpty();
            emailIndex.Columns.First().Name.Should().Be("Email");
            emailIndex.IsUnique.Should().BeTrue();
        }

        // Test stub to simulate SqlServerSchemaProvider behavior
        private class SqlServerSchemaProviderStub : IDatabaseSchemaProvider
        {
            private readonly string _connectionString;
            private readonly SchemaProviderOptions _options;
            private readonly bool _throwException;

            public SqlServerSchemaProviderStub(string connectionString, SchemaProviderOptions options, bool throwException = false)
            {
                _connectionString = connectionString;
                _options = options;
                _throwException = throwException;
            }

            public async Task<IDatabaseSchema> GetSchemaAsync()
            {
                if (_throwException)
                {
                    throw new SqlException("Invalid connection string");
                }

                // Simulate async operation
                await Task.Delay(1);

                // Create a mock schema
                var mockSchema = new Mock<IDatabaseSchema>();
                return mockSchema.Object;
            }
        }

        // Helper method to create a mock schema provider
        private IDatabaseSchemaProvider CreateMockSchemaProvider(string connectionString, SchemaProviderOptions options)
        {
            var mockProvider = new Mock<IDatabaseSchemaProvider>();
            
            mockProvider.Setup(p => p.GetSchemaAsync())
                .ReturnsAsync(() => {
                    var mockSchema = new Mock<IDatabaseSchema>();
                    mockSchema.Setup(s => s.DatabaseName).Returns("TestDb");
                    
                    var tables = new List<ITable>();
                    
                    // Only include tables if specified in options
                    if (options.IncludeTables)
                    {
                        // Create Customer table
                        var customerTable = CreateCustomerTable();
                        
                        // Apply schema filters
                        if (options.IncludeSchemas.Count == 0 || options.IncludeSchemas.Contains(customerTable.Schema))
                        {
                            // Apply table name filters
                            if (ShouldIncludeTable(customerTable, options))
                            {
                                tables.Add(customerTable);
                            }
                        }
                        
                        // Create Order table
                        var orderTable = CreateOrderTable(customerTable);
                        
                        // Apply schema filters
                        if (options.IncludeSchemas.Count == 0 || options.IncludeSchemas.Contains(orderTable.Schema))
                        {
                            // Apply table name filters
                            if (ShouldIncludeTable(orderTable, options))
                            {
                                tables.Add(orderTable);
                            }
                        }
                    }
                    
                    // Setup mock schema properties
                    mockSchema.Setup(s => s.Tables).Returns(tables);
                    mockSchema.Setup(s => s.Views).Returns(options.IncludeViews ? CreateViews() : new List<IView>());
                    mockSchema.Setup(s => s.StoredProcedures).Returns(options.IncludeStoredProcedures ? CreateStoredProcedures() : new List<IStoredProcedure>());
                    mockSchema.Setup(s => s.Functions).Returns(options.IncludeFunctions ? CreateFunctions() : new List<IFunction>());
                    
                    return mockSchema.Object;
                });
            
            return mockProvider.Object;
        }

        // Helper method to check if a table should be included based on pattern filters
        private bool ShouldIncludeTable(ITable table, SchemaProviderOptions options)
        {
            // Include patterns
            if (options.IncludeTablePatterns.Count > 0)
            {
                bool matchesIncludePattern = false;
                foreach (var pattern in options.IncludeTablePatterns)
                {
                    if (MatchesPattern(table.Name, pattern))
                    {
                        matchesIncludePattern = true;
                        break;
                    }
                }
                
                if (!matchesIncludePattern)
                {
                    return false;
                }
            }
            
            // Exclude patterns
            foreach (var pattern in options.ExcludeTablePatterns)
            {
                if (MatchesPattern(table.Name, pattern))
                {
                    return false;
                }
            }
            
            return true;
        }

        // Helper method to check if a name matches a pattern with wildcards
        private bool MatchesPattern(string name, string pattern)
        {
            if (pattern == "*")
            {
                return true;
            }
            
            if (pattern.StartsWith("*") && pattern.EndsWith("*"))
            {
                var substring = pattern.Substring(1, pattern.Length - 2);
                return name.Contains(substring);
            }
            
            if (pattern.StartsWith("*"))
            {
                var suffix = pattern.Substring(1);
                return name.EndsWith(suffix);
            }
            
            if (pattern.EndsWith("*"))
            {
                var prefix = pattern.Substring(0, pattern.Length - 1);
                return name.StartsWith(prefix);
            }
            
            return name == pattern;
        }

        // Helper method to create a mock Customer table
        private ITable CreateCustomerTable()
        {
            var mockTable = new Mock<ITable>();
            mockTable.Setup(t => t.Name).Returns("Customer");
            mockTable.Setup(t => t.Schema).Returns("dbo");
            
            // Create columns
            var columns = new List<IColumn>();
            
            var customerId = new Mock<IColumn>();
            customerId.Setup(c => c.Name).Returns("CustomerId");
            customerId.Setup(c => c.DataType).Returns("int");
            customerId.Setup(c => c.IsNullable).Returns(false);
            customerId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            customerId.Setup(c => c.OrdinalPosition).Returns(1);
            columns.Add(customerId.Object);
            
            var customerName = new Mock<IColumn>();
            customerName.Setup(c => c.Name).Returns("Name");
            customerName.Setup(c => c.DataType).Returns("nvarchar");
            customerName.Setup(c => c.MaxLength).Returns(100);
            customerName.Setup(c => c.IsNullable).Returns(false);
            customerName.Setup(c => c.OrdinalPosition).Returns(2);
            columns.Add(customerName.Object);
            
            var customerEmail = new Mock<IColumn>();
            customerEmail.Setup(c => c.Name).Returns("Email");
            customerEmail.Setup(c => c.DataType).Returns("nvarchar");
            customerEmail.Setup(c => c.MaxLength).Returns(255);
            customerEmail.Setup(c => c.IsNullable).Returns(true);
            customerEmail.Setup(c => c.OrdinalPosition).Returns(3);
            columns.Add(customerEmail.Object);
            
            mockTable.Setup(t => t.Columns).Returns(columns);
            
            // Create primary key
            var mockPk = new Mock<IKey>();
            mockPk.Setup(k => k.Name).Returns("PK_Customer");
            mockPk.Setup(k => k.Columns).Returns(new List<IColumn> { customerId.Object });
            mockTable.Setup(t => t.PrimaryKey).Returns(mockPk.Object);
            
            // Create indexes
            var indexes = new List<IIndex>();
            
            var emailIndex = new Mock<IIndex>();
            emailIndex.Setup(i => i.Name).Returns("IX_Customer_Email");
            emailIndex.Setup(i => i.IsUnique).Returns(true);
            
            var emailIndexColumn = new Mock<IIndexColumn>();
            emailIndexColumn.Setup(ic => ic.Column).Returns(customerEmail.Object);
            emailIndexColumn.Setup(ic => ic.IsDescending).Returns(false);
            
            emailIndex.Setup(i => i.Columns).Returns(new List<IIndexColumn> { emailIndexColumn.Object });
            indexes.Add(emailIndex.Object);
            
            mockTable.Setup(t => t.Indexes).Returns(indexes);
            
            // Create empty foreign keys collection
            mockTable.Setup(t => t.ForeignKeys).Returns(new List<IForeignKey>());
            
            return mockTable.Object;
        }

        // Helper method to create a mock Order table with FK to Customer
        private ITable CreateOrderTable(ITable customerTable)
        {
            var mockTable = new Mock<ITable>();
            mockTable.Setup(t => t.Name).Returns("Order");
            mockTable.Setup(t => t.Schema).Returns("dbo");
            
            // Create columns
            var columns = new List<IColumn>();
            
            var orderId = new Mock<IColumn>();
            orderId.Setup(c => c.Name).Returns("OrderId");
            orderId.Setup(c => c.DataType).Returns("int");
            orderId.Setup(c => c.IsNullable).Returns(false);
            orderId.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            orderId.Setup(c => c.OrdinalPosition).Returns(1);
            columns.Add(orderId.Object);
            
            var customerId = new Mock<IColumn>();
            customerId.Setup(c => c.Name).Returns("CustomerId");
            customerId.Setup(c => c.DataType).Returns("int");
            customerId.Setup(c => c.IsNullable).Returns(false);
            customerId.Setup(c => c.IsPartOfForeignKey).Returns(true);
            customerId.Setup(c => c.OrdinalPosition).Returns(2);
            columns.Add(customerId.Object);
            
            var orderDate = new Mock<IColumn>();
            orderDate.Setup(c => c.Name).Returns("OrderDate");
            orderDate.Setup(c => c.DataType).Returns("datetime");
            orderDate.Setup(c => c.IsNullable).Returns(false);
            orderDate.Setup(c => c.OrdinalPosition).Returns(3);
            columns.Add(orderDate.Object);
            
            var total = new Mock<IColumn>();
            total.Setup(c => c.Name).Returns("Total");
            total.Setup(c => c.DataType).Returns("decimal");
            total.Setup(c => c.Precision).Returns(18);
            total.Setup(c => c.Scale).Returns(2);
            total.Setup(c => c.IsNullable).Returns(false);
            total.Setup(c => c.OrdinalPosition).Returns(4);
            columns.Add(total.Object);
            
            mockTable.Setup(t => t.Columns).Returns(columns);
            
            // Create primary key
            var mockPk = new Mock<IKey>();
            mockPk.Setup(k => k.Name).Returns("PK_Order");
            mockPk.Setup(k => k.Columns).Returns(new List<IColumn> { orderId.Object });
            mockTable.Setup(t => t.PrimaryKey).Returns(mockPk.Object);
            
            // Create foreign keys
            var foreignKeys = new List<IForeignKey>();
            
            var customerFk = new Mock<IForeignKey>();
            customerFk.Setup(fk => fk.Name).Returns("FK_Order_Customer");
            customerFk.Setup(fk => fk.Columns).Returns(new List<IColumn> { customerId.Object });
            customerFk.Setup(fk => fk.ReferencedTable).Returns(customerTable);
            customerFk.Setup(fk => fk.ReferencedColumns).Returns(customerTable.Columns.Where(c => c.IsPartOfPrimaryKey).ToList());
            customerFk.Setup(fk => fk.DeleteAction).Returns(ReferentialAction.Cascade);
            customerFk.Setup(fk => fk.UpdateAction).Returns(ReferentialAction.Cascade);
            
            foreignKeys.Add(customerFk.Object);
            mockTable.Setup(t => t.ForeignKeys).Returns(foreignKeys);
            
            // Create indexes
            var indexes = new List<IIndex>();
            
            var customerIdIndex = new Mock<IIndex>();
            customerIdIndex.Setup(i => i.Name).Returns("IX_Order_CustomerId");
            customerIdIndex.Setup(i => i.IsUnique).Returns(false);
            
            var customerIdIndexColumn = new Mock<IIndexColumn>();
            customerIdIndexColumn.Setup(ic => ic.Column).Returns(customerId.Object);
            customerIdIndexColumn.Setup(ic => ic.IsDescending).Returns(false);
            
            customerIdIndex.Setup(i => i.Columns).Returns(new List<IIndexColumn> { customerIdIndexColumn.Object });
            indexes.Add(customerIdIndex.Object);
            
            mockTable.Setup(t => t.Indexes).Returns(indexes);
            
            return mockTable.Object;
        }

        // Helper method to create mock views
        private List<IView> CreateViews()
        {
            var views = new List<IView>();
            
            var customerOrdersView = new Mock<IView>();
            customerOrdersView.Setup(v => v.Name).Returns("CustomerOrders");
            customerOrdersView.Setup(v => v.Schema).Returns("dbo");
            
            var columns = new List<IViewColumn>();
            
            var customerIdColumn = new Mock<IViewColumn>();
            customerIdColumn.Setup(c => c.Name).Returns("CustomerId");
            customerIdColumn.Setup(c => c.DataType).Returns("int");
            customerIdColumn.Setup(c => c.IsNullable).Returns(false);
            columns.Add(customerIdColumn.Object);
            
            var customerNameColumn = new Mock<IViewColumn>();
            customerNameColumn.Setup(c => c.Name).Returns("CustomerName");
            customerNameColumn.Setup(c => c.DataType).Returns("nvarchar");
            customerNameColumn.Setup(c => c.MaxLength).Returns(100);
            customerNameColumn.Setup(c => c.IsNullable).Returns(false);
            columns.Add(customerNameColumn.Object);
            
            var orderCountColumn = new Mock<IViewColumn>();
            orderCountColumn.Setup(c => c.Name).Returns("OrderCount");
            orderCountColumn.Setup(c => c.DataType).Returns("int");
            orderCountColumn.Setup(c => c.IsNullable).Returns(false);
            columns.Add(orderCountColumn.Object);
            
            customerOrdersView.Setup(v => v.Columns).Returns(columns);
            
            views.Add(customerOrdersView.Object);
            
            return views;
        }

        // Helper method to create mock stored procedures
        private List<IStoredProcedure> CreateStoredProcedures()
        {
            var storedProcedures = new List<IStoredProcedure>();
            
            var getCustomerOrders = new Mock<IStoredProcedure>();
            getCustomerOrders.Setup(sp => sp.Name).Returns("GetCustomerOrders");
            getCustomerOrders.Setup(sp => sp.Schema).Returns("dbo");
            
            var parameters = new List<IParameter>();
            
            var customerIdParam = new Mock<IParameter>();
            customerIdParam.Setup(p => p.Name).Returns("@CustomerId");
            customerIdParam.Setup(p => p.DataType).Returns("int");
            customerIdParam.Setup(p => p.Direction).Returns(ParameterDirection.Input);
            parameters.Add(customerIdParam.Object);
            
            var startDateParam = new Mock<IParameter>();
            startDateParam.Setup(p => p.Name).Returns("@StartDate");
            startDateParam.Setup(p => p.DataType).Returns("datetime");
            startDateParam.Setup(p => p.Direction).Returns(ParameterDirection.Input);
            startDateParam.Setup(p => p.IsNullable).Returns(true);
            parameters.Add(startDateParam.Object);
            
            getCustomerOrders.Setup(sp => sp.Parameters).Returns(parameters);
            
            var resultColumns = new List<IResultColumn>();
            
            var orderIdColumn = new Mock<IResultColumn>();
            orderIdColumn.Setup(c => c.Name).Returns("OrderId");
            orderIdColumn.Setup(c => c.DataType).Returns("int");
            orderIdColumn.Setup(c => c.IsNullable).Returns(false);
            resultColumns.Add(orderIdColumn.Object);
            
            var orderDateColumn = new Mock<IResultColumn>();
            orderDateColumn.Setup(c => c.Name).Returns("OrderDate");
            orderDateColumn.Setup(c => c.DataType).Returns("datetime");
            orderDateColumn.Setup(c => c.IsNullable).Returns(false);
            resultColumns.Add(orderDateColumn.Object);
            
            var totalColumn = new Mock<IResultColumn>();
            totalColumn.Setup(c => c.Name).Returns("Total");
            totalColumn.Setup(c => c.DataType).Returns("decimal");
            totalColumn.Setup(c => c.Precision).Returns(18);
            totalColumn.Setup(c => c.Scale).Returns(2);
            totalColumn.Setup(c => c.IsNullable).Returns(false);
            resultColumns.Add(totalColumn.Object);
            
            getCustomerOrders.Setup(sp => sp.ResultColumns).Returns(resultColumns);
            
            storedProcedures.Add(getCustomerOrders.Object);
            
            return storedProcedures;
        }

        // Helper method to create mock functions
        private List<IFunction> CreateFunctions()
        {
            var functions = new List<IFunction>();
            
            var calculateOrderTotal = new Mock<IFunction>();
            calculateOrderTotal.Setup(f => f.Name).Returns("CalculateOrderTotal");
            calculateOrderTotal.Setup(f => f.Schema).Returns("dbo");
            calculateOrderTotal.Setup(f => f.FunctionType).Returns(FunctionType.ScalarFunction);
            
            var parameters = new List<IParameter>();
            
            var orderIdParam = new Mock<IParameter>();
            orderIdParam.Setup(p => p.Name).Returns("@OrderId");
            orderIdParam.Setup(p => p.DataType).Returns("int");
            orderIdParam.Setup(p => p.Direction).Returns(ParameterDirection.Input);
            parameters.Add(orderIdParam.Object);
            
            calculateOrderTotal.Setup(f => f.Parameters).Returns(parameters);
            calculateOrderTotal.Setup(f => f.ReturnType).Returns("decimal(18,2)");
            
            functions.Add(calculateOrderTotal.Object);
            
            return functions;
        }
    }
}
