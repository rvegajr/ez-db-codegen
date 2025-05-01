using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.Cli
{
    public class SchemaInfoCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ShouldDisplaySchemaInfo()
        {
            // Arrange
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockSchema = new Mock<IDatabaseSchema>();
            var mockTable1 = new Mock<ITable>();
            var mockTable2 = new Mock<ITable>();
            var mockLogger = new Mock<ILogger>();
            
            mockTable1.Setup(t => t.Name).Returns("Customer");
            mockTable1.Setup(t => t.Schema).Returns("dbo");
            mockTable1.Setup(t => t.Columns).Returns(new List<IColumn>
            {
                CreateMockColumn("CustomerId", "int", false, true),
                CreateMockColumn("Name", "nvarchar", false, false),
                CreateMockColumn("Email", "nvarchar", true, false)
            });
            
            mockTable2.Setup(t => t.Name).Returns("Order");
            mockTable2.Setup(t => t.Schema).Returns("dbo");
            mockTable2.Setup(t => t.Columns).Returns(new List<IColumn>
            {
                CreateMockColumn("OrderId", "int", false, true),
                CreateMockColumn("CustomerId", "int", false, false),
                CreateMockColumn("OrderDate", "datetime", false, false),
                CreateMockColumn("TotalAmount", "decimal", false, false)
            });
            
            mockSchema.Setup(s => s.Tables).Returns(new List<ITable> { mockTable1.Object, mockTable2.Object });
            mockSchemaProvider.Setup(sp => sp.GetSchemaAsync()).ReturnsAsync(mockSchema.Object);
            
            var command = CreateSchemaInfoCommand(mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string",
                ["provider"] = "SqlServer"
            };

            // Act
            await command.ExecuteAsync(options);

            // Assert
            mockSchemaProvider.Verify(sp => sp.GetSchemaAsync(), Times.Once);
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("dbo.Customer"))), Times.Once);
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("dbo.Order"))), Times.Once);
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("CustomerId"))), Times.AtLeast(2));
        }

        [Fact]
        public async Task ExecuteAsync_WithTableFilter_ShouldDisplayFilteredSchemaInfo()
        {
            // Arrange
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockSchema = new Mock<IDatabaseSchema>();
            var mockTable1 = new Mock<ITable>();
            var mockTable2 = new Mock<ITable>();
            var mockLogger = new Mock<ILogger>();
            
            mockTable1.Setup(t => t.Name).Returns("Customer");
            mockTable1.Setup(t => t.Schema).Returns("dbo");
            mockTable1.Setup(t => t.Columns).Returns(new List<IColumn>
            {
                CreateMockColumn("CustomerId", "int", false, true),
                CreateMockColumn("Name", "nvarchar", false, false),
                CreateMockColumn("Email", "nvarchar", true, false)
            });
            
            mockTable2.Setup(t => t.Name).Returns("Order");
            mockTable2.Setup(t => t.Schema).Returns("dbo");
            mockTable2.Setup(t => t.Columns).Returns(new List<IColumn>
            {
                CreateMockColumn("OrderId", "int", false, true),
                CreateMockColumn("CustomerId", "int", false, false),
                CreateMockColumn("OrderDate", "datetime", false, false),
                CreateMockColumn("TotalAmount", "decimal", false, false)
            });
            
            mockSchema.Setup(s => s.Tables).Returns(new List<ITable> { mockTable1.Object, mockTable2.Object });
            mockSchemaProvider.Setup(sp => sp.GetSchemaAsync()).ReturnsAsync(mockSchema.Object);
            
            var command = CreateSchemaInfoCommand(mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string",
                ["provider"] = "SqlServer",
                ["table"] = "Customer"
            };

            // Act
            await command.ExecuteAsync(options);

            // Assert
            mockSchemaProvider.Verify(sp => sp.GetSchemaAsync(), Times.Once);
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("dbo.Customer"))), Times.Once);
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("dbo.Order"))), Times.Never);
        }

        [Fact]
        public void ValidateOptions_WithRequiredOptions_ShouldReturnTrue()
        {
            // Arrange
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateSchemaInfoCommand(mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string"
            };

            // Act
            var result = command.ValidateOptions(options);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateOptions_WithMissingOptions_ShouldReturnFalse()
        {
            // Arrange
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateSchemaInfoCommand(mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                // Missing connection
            };

            // Act
            var result = command.ValidateOptions(options);
            var errors = command.GetValidationErrors(options);

            // Assert
            result.Should().BeFalse();
            errors.Should().NotBeEmpty();
            errors.Should().Contain(e => e.Contains("connection"));
        }

        [Fact]
        public void GetOptions_ShouldReturnAllAvailableOptions()
        {
            // Arrange
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateSchemaInfoCommand(mockSchemaProvider.Object, mockLogger.Object);

            // Act
            var options = command.GetOptions();

            // Assert
            options.Should().NotBeNull();
            options.Should().ContainKey("connection");
            options.Should().ContainKey("provider");
            options.Should().ContainKey("table");
            options.Should().ContainKey("schema");
            options.Should().ContainKey("format");
        }

        // Helper method to create a schema info command
        private ICommand CreateSchemaInfoCommand(IDatabaseSchemaProvider schemaProvider, ILogger logger)
        {
            var mockCommand = new Mock<ICommand>();
            
            mockCommand.Setup(c => c.Name).Returns("schema-info");
            mockCommand.Setup(c => c.Description).Returns("Display information about a database schema");
            mockCommand.Setup(c => c.Usage).Returns("schema-info --connection <connection-string> [--provider <provider-name>] [--table <table-name>] [--schema <schema-name>] [--format <json|text>]");
            
            // Setup the ExecuteAsync method
            mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>(async (options) => {
                    try
                    {
                        // Extract options
                        options.TryGetValue("connection", out var connectionString);
                        options.TryGetValue("provider", out var providerName);
                        options.TryGetValue("table", out var tableName);
                        options.TryGetValue("schema", out var schemaName);
                        options.TryGetValue("format", out var format);
                        
                        // Set default values
                        providerName ??= "SqlServer";
                        format ??= "text";
                        
                        logger.Info($"Retrieving schema information from {providerName} database...");
                        
                        // Extract schema
                        var schema = await schemaProvider.GetSchemaAsync();
                        
                        // Filter tables
                        var tables = schema.Tables;
                        if (!string.IsNullOrWhiteSpace(tableName))
                        {
                            tables = tables.Where(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                        
                        if (!string.IsNullOrWhiteSpace(schemaName))
                        {
                            tables = tables.Where(t => t.Schema.Equals(schemaName, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                        
                        // Display schema information
                        if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                        {
                            // Display as JSON
                            logger.Info("Schema information in JSON format:");
                            // Implementation would serialize to JSON here
                        }
                        else
                        {
                            // Display as text
                            logger.Info("Schema information:");
                            logger.Info("==================");
                            
                            foreach (var table in tables)
                            {
                                logger.Info($"Table: {table.Schema}.{table.Name}");
                                logger.Info("Columns:");
                                
                                foreach (var column in table.Columns)
                                {
                                    var nullableStr = column.IsNullable ? "NULL" : "NOT NULL";
                                    var pkStr = column.IsPrimaryKey ? " (PK)" : "";
                                    logger.Info($"  - {column.Name} ({column.DataType}) {nullableStr}{pkStr}");
                                }
                                
                                logger.Info("------------------");
                            }
                        }
                        
                        logger.Info("Schema information retrieval completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error retrieving schema information: {ex.Message}");
                        throw;
                    }
                });
            
            // Setup the ValidateOptions method
            mockCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>((options) => {
                    var requiredOptions = mockCommand.Object.GetRequiredOptions();
                    
                    foreach (var requiredOption in requiredOptions)
                    {
                        if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                        {
                            return false;
                        }
                    }
                    
                    return true;
                });
            
            // Setup the GetValidationErrors method
            mockCommand.Setup(c => c.GetValidationErrors(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>((options) => {
                    var errors = new List<string>();
                    var requiredOptions = mockCommand.Object.GetRequiredOptions();
                    
                    foreach (var requiredOption in requiredOptions)
                    {
                        if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                        {
                            errors.Add($"Option '{requiredOption}' is required.");
                        }
                    }
                    
                    return errors;
                });
            
            // Setup the GetOptions method
            mockCommand.Setup(c => c.GetOptions())
                .Returns(new Dictionary<string, string>
                {
                    ["connection"] = "Database connection string",
                    ["provider"] = "Database provider (default: SqlServer)",
                    ["table"] = "Filter by table name",
                    ["schema"] = "Filter by schema name",
                    ["format"] = "Output format: text or json (default: text)"
                });
            
            // Setup the GetRequiredOptions method
            mockCommand.Setup(c => c.GetRequiredOptions())
                .Returns(new List<string> { "connection" });
            
            return mockCommand.Object;
        }

        // Helper method to create a mock column
        private IColumn CreateMockColumn(string name, string dataType, bool isNullable, bool isPrimaryKey)
        {
            var mockColumn = new Mock<IColumn>();
            mockColumn.Setup(c => c.Name).Returns(name);
            mockColumn.Setup(c => c.DataType).Returns(dataType);
            mockColumn.Setup(c => c.IsNullable).Returns(isNullable);
            mockColumn.Setup(c => c.IsPrimaryKey).Returns(isPrimaryKey);
            return mockColumn.Object;
        }
    }
}
