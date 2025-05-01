using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema;
using FluentAssertions;
using Moq;
using Xunit;

namespace EzDbCodeGen.Core.Tests.Schema
{
    /// <summary>
    /// Integration tests for SqlServerSchemaProvider.
    /// Note: These tests require a live SQL Server database. If unavailable, the tests will be skipped.
    /// </summary>
    public class SqlServerSchemaProviderIntegrationTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly SqlServerSchemaProvider _provider;
        private readonly Dictionary<string, string> _connectionInfo;

        public SqlServerSchemaProviderIntegrationTests()
        {
            _loggerMock = new Mock<ILogger>();
            _provider = new SqlServerSchemaProvider(_loggerMock.Object);
            
            // You should update this connection string to point to your test database
            // Format: "Server=localhost;Database=TestDb;User Id=sa;Password=yourPassword;"
            var connectionString = Environment.GetEnvironmentVariable("TEST_SQL_CONNECTION_STRING");
            
            _connectionInfo = new Dictionary<string, string>
            {
                ["ConnectionString"] = connectionString
            };
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task TestConnectionAsync_WithValidConnection_ReturnsTrue()
        {
            // Skip if no connection string is available
            if (string.IsNullOrEmpty(_connectionInfo["ConnectionString"]))
            {
                return;
            }

            // Arrange
            _provider.Configure(_connectionInfo);

            // Act
            var result = await _provider.TestConnectionAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetSchemaAsync_WithValidConnection_ReturnsSchema()
        {
            // Skip if no connection string is available
            if (string.IsNullOrEmpty(_connectionInfo["ConnectionString"]))
            {
                return;
            }

            // Arrange
            _provider.Configure(_connectionInfo);

            // Act
            var schema = await _provider.GetSchemaAsync();

            // Assert
            schema.Should().NotBeNull();
            schema.Tables.Should().NotBeEmpty();
            
            // Log some basic info about what was retrieved
            _loggerMock.Verify(l => l.Info(It.Is<string>(s => s.Contains("Schema extraction complete"))), Times.Once);
        }
    }
}
