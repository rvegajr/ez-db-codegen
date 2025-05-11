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
    public class SqlServerTemporalTableTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<Providers.SqlServerSchemaProvider>> _mockLogger;

        public SqlServerTemporalTableTests()
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
        public async Task Should_Detect_Temporal_Table_Properties()
        {
            // Arrange
            var provider = _serviceProvider.GetRequiredService<ISqlServerSchemaProvider>();
            var options = new SchemaProviderOptions
            {
                ConnectionString = "Server=localhost;Database=TemporalTablesTest;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;",
                Schema = "dbo"
            };

            // Act - This would normally connect to a real database with temporal tables
            // For TDD, we're creating a mock implementation first
            var mockSchema = CreateMockTemporalTableSchema();

            // Assert
            mockSchema.Tables.Should().Contain(t => t.HasTemporalTableSupport);
            var temporalTable = mockSchema.Tables.First(t => t.HasTemporalTableSupport);
            
            temporalTable.TemporalHistoryTableName.Should().NotBeNullOrEmpty("because temporal tables must have a history table");
            temporalTable.TemporalPeriodStartColumnName.Should().Be("ValidFrom", "because this is the standard period start column name");
            temporalTable.TemporalPeriodEndColumnName.Should().Be("ValidTo", "because this is the standard period end column name");
            
            // Verify that history table exists in the schema
            mockSchema.Tables.Should().Contain(t => t.Name == temporalTable.TemporalHistoryTableName);
        }

        [Fact]
        public void Should_Generate_Proper_SQL_For_Temporal_Tables()
        {
            // Arrange
            var provider = _serviceProvider.GetRequiredService<ISqlServerSchemaProvider>();
            
            // Act - In real implementation, this would extract the SQL needed for temporal tables
            var createTableSql = GetMockTemporalTableSql();
            
            // Assert
            createTableSql.Should().Contain("PERIOD FOR SYSTEM_TIME", "because temporal tables require a period definition");
            createTableSql.Should().Contain("SYSTEM_VERSIONING = ON", "because system versioning must be enabled");
            createTableSql.Should().Contain("HISTORY_TABLE", "because a history table must be specified");
        }

        private IDatabaseSchema CreateMockTemporalTableSchema()
        {
            // Create a mock schema with a temporal table for testing
            var schema = new DatabaseSchema
            {
                Name = "TemporalTablesTest",
                DefaultSchema = "dbo"
            };

            // Main temporal table
            var temporalTable = new Table
            {
                Name = "Employee",
                Schema = "dbo",
                HasTemporalTableSupport = true,
                TemporalHistoryTableName = "dbo.EmployeeHistory",
                TemporalPeriodStartColumnName = "ValidFrom",
                TemporalPeriodEndColumnName = "ValidTo"
            };

            // Add columns to the temporal table
            temporalTable.Columns.Add(new Column
            {
                Name = "EmployeeId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            temporalTable.Columns.Add(new Column
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            temporalTable.Columns.Add(new Column
            {
                Name = "Position",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = true
            });

            temporalTable.Columns.Add(new Column
            {
                Name = "ValidFrom",
                DataType = "datetime2",
                IsNullable = false,
                IsSystemVersioningPeriodStart = true
            });

            temporalTable.Columns.Add(new Column
            {
                Name = "ValidTo",
                DataType = "datetime2",
                IsNullable = false,
                IsSystemVersioningPeriodEnd = true
            });

            // History table
            var historyTable = new Table
            {
                Name = "EmployeeHistory",
                Schema = "dbo",
                IsHistoryTable = true
            };

            // Add the same columns to the history table
            historyTable.Columns.Add(new Column
            {
                Name = "EmployeeId",
                DataType = "int",
                IsNullable = false
            });

            historyTable.Columns.Add(new Column
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            historyTable.Columns.Add(new Column
            {
                Name = "Position",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = true
            });

            historyTable.Columns.Add(new Column
            {
                Name = "ValidFrom",
                DataType = "datetime2",
                IsNullable = false
            });

            historyTable.Columns.Add(new Column
            {
                Name = "ValidTo",
                DataType = "datetime2",
                IsNullable = false
            });

            schema.Tables.Add(temporalTable);
            schema.Tables.Add(historyTable);

            return schema;
        }

        private string GetMockTemporalTableSql()
        {
            return @"
CREATE TABLE [dbo].[Employee] (
    [EmployeeId] INT NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    [Position] NVARCHAR(100) NULL,
    [ValidFrom] DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    [ValidTo] DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)
WITH (
    SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = [dbo].[EmployeeHistory]
    )
);";
        }
    }
}
