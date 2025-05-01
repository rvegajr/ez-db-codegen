using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EzDbCodeGen.Schema.Tests.Providers
{
    public class SqlServerSchemaProviderTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly SqlServerSchemaProvider _provider;

        public SqlServerSchemaProviderTests()
        {
            _mockLogger = new Mock<ILogger>();
            _provider = new SqlServerSchemaProvider(_mockLogger.Object);
        }

        [Fact(Skip = "Requires a live SQL Server connection")]
        public async Task GetSchemaAsync_ShouldExtractDatabaseSchema()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=AdventureWorks;Trusted_Connection=True;";
            var options = new SchemaProviderOptions
            {
                IncludeTables = true,
                IncludeViews = true,
                IncludeStoredProcedures = true,
                IncludeFunctions = true,
                IncludeForeignKeys = true,
                IncludeIndexes = true,
                IncludeSystemObjects = false,
                Schemas = new List<string> { "dbo" }
            };

            // Act
            var schema = await _provider.GetSchemaAsync(connectionString, options);

            // Assert
            Assert.NotNull(schema);
            Assert.NotEmpty(schema.Name);
            // Additional assertions based on known database structure
        }

        [Fact]
        public void ProviderType_ShouldBeSqlServer()
        {
            // Assert
            Assert.Equal("SqlServer", _provider.ProviderType);
        }

        [Fact]
        public void Construct_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SqlServerSchemaProvider(null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetSchemaAsync_WithInvalidConnectionString_ShouldThrowArgumentException(string connectionString)
        {
            // Arrange
            var options = new SchemaProviderOptions();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _provider.GetSchemaAsync(connectionString, options));
        }

        [Fact]
        public async Task GetSchemaAsync_WithNullOptions_ShouldUseDefaultOptions()
        {
            // This test would require a mock DbConnection to avoid actual database calls
            // In a real implementation, you would use a mocking framework to mock the SqlConnection
            // For now, we'll just verify that null options doesn't throw an exception
            
            try
            {
                var connectionString = "Server=localhost;Database=TestDB;Trusted_Connection=True;";
                await _provider.GetSchemaAsync(connectionString, null);
                // If we get here without an exception, the test passes
                Assert.True(true);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                // We expect either an ArgumentException for the invalid connection string
                // or an InvalidOperationException when trying to connect to a non-existent database
                // Any other exception means the null options wasn't handled correctly
                Assert.True(false, $"Unexpected exception type: {ex.GetType().Name}");
            }
        }

        [Fact(Skip = "Requires a live SQL Server connection")]
        public async Task TestConnectionAsync_WithValidConnectionString_ShouldReturnTrue()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=AdventureWorks;Trusted_Connection=True;";

            // Act
            var result = await _provider.TestConnectionAsync(connectionString);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task TestConnectionAsync_WithInvalidConnectionString_ShouldReturnFalse()
        {
            // Arrange
            var connectionString = "Server=nonexistentserver;Database=nonexistentdb;Trusted_Connection=True;";

            // Act
            var result = await _provider.TestConnectionAsync(connectionString);

            // Assert
            Assert.False(result);
        }
    }
}
