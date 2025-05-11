using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    public class ComprehensiveComparisonTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<ComprehensiveComparisonTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        // List of sample databases to test
        private static readonly string[] SampleDatabases = new[]
        {
            "Northwind",
            "AdventureWorks",
            "WideWorldImporters",
            "ContosoDataWarehouse"
        };

        public ComprehensiveComparisonTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<ComprehensiveComparisonTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Compare_Schema_Discovery_Performance(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act - EF Core Performance
            var efCoreStopwatch = Stopwatch.StartNew();
            var efCoreModel = dbContext.Model;
            var entityTypes = efCoreModel.GetEntityTypes().ToList();
            efCoreStopwatch.Stop();
            var efCoreTime = efCoreStopwatch.ElapsedMilliseconds;

            // Act - EzDbCodeGen Performance
            var ezDbCodeGenMetrics = _ezDbCodeGenComparer.MeasureEzDbCodeGenPerformance(connectionString, databaseName);

            // Assert and Output
            _output.WriteLine($"=== Performance Comparison for {databaseName} ===");
            _output.WriteLine($"EF Core schema discovery: {efCoreTime}ms, discovered {entityTypes.Count} entity types");
            _output.WriteLine($"EzDbCodeGen schema discovery: {ezDbCodeGenMetrics.SchemaDiscoveryTime}ms, discovered {ezDbCodeGenMetrics.TableCount} tables");
            _output.WriteLine($"Performance difference: {(efCoreTime - ezDbCodeGenMetrics.SchemaDiscoveryTime)}ms");
            _output.WriteLine($"EzDbCodeGen is {(efCoreTime > ezDbCodeGenMetrics.SchemaDiscoveryTime ? "faster" : "slower")} by {Math.Abs(efCoreTime - ezDbCodeGenMetrics.SchemaDiscoveryTime)}ms");
            
            // Calculate percentage difference
            var percentageDiff = Math.Round(((double)ezDbCodeGenMetrics.SchemaDiscoveryTime / efCoreTime - 1) * 100, 2);
            _output.WriteLine($"EzDbCodeGen is {(percentageDiff < 0 ? Math.Abs(percentageDiff) + "% faster" : percentageDiff + "% slower")} than EF Core");
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Compare_Type_Mapping(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act
            var efCoreColumnTypes = _efCoreComparer.AnalyzeEFCoreColumnTypes(dbContext);
            var ezDbCodeGenColumnTypes = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenColumnTypes(connectionString, databaseName);

            // Assert
            _output.WriteLine($"=== Type Mapping Comparison for {databaseName} ===");
            _output.WriteLine($"EF Core mapped {efCoreColumnTypes.Count} columns");
            _output.WriteLine($"EzDbCodeGen mapped {ezDbCodeGenColumnTypes.Count} columns");

            // Find common columns to compare type mappings
            var commonColumns = efCoreColumnTypes.Keys.Intersect(ezDbCodeGenColumnTypes.Keys).ToList();
            _output.WriteLine($"Found {commonColumns.Count} common columns for comparison");

            // Compare type mappings
            var differentMappings = new List<(string Column, string EFCoreType, string EzDbCodeGenType)>();
            foreach (var column in commonColumns)
            {
                var efCoreType = efCoreColumnTypes[column];
                var ezDbCodeGenType = ezDbCodeGenColumnTypes[column];

                if (!string.Equals(efCoreType, ezDbCodeGenType, StringComparison.OrdinalIgnoreCase))
                {
                    differentMappings.Add((column, efCoreType, ezDbCodeGenType));
                }
            }

            // Output differences
            _output.WriteLine($"Found {differentMappings.Count} differences in type mapping");
            foreach (var (column, efCoreType, ezDbCodeGenType) in differentMappings.Take(20))
            {
                _output.WriteLine($"Column: {column}, EF Core: {efCoreType}, EzDbCodeGen: {ezDbCodeGenType}");
            }
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Compare_Relationship_Detection(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            var ezDbCodeGenRelationships = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenRelationships(connectionString, databaseName);

            // Assert
            _output.WriteLine($"=== Relationship Detection Comparison for {databaseName} ===");
            _output.WriteLine($"EF Core detected {efCoreRelationships.Count} relationships");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenRelationships.Count} relationships");

            // Compare source and target tables (ignoring property names which may differ)
            var efCoreRelationshipPairs = efCoreRelationships
                .Select(r => (Source: r.SourceTable, Target: r.TargetTable, r.IsCollection))
                .ToList();
            
            var ezDbCodeGenRelationshipPairs = ezDbCodeGenRelationships
                .Select(r => (Source: r.SourceTable, Target: r.TargetTable, r.IsCollection))
                .ToList();

            // Find relationships detected by both
            var commonRelationships = efCoreRelationshipPairs
                .Intersect(ezDbCodeGenRelationshipPairs)
                .ToList();

            // Find relationships only detected by EF Core
            var efCoreOnlyRelationships = efCoreRelationshipPairs
                .Except(ezDbCodeGenRelationshipPairs)
                .ToList();

            // Find relationships only detected by EzDbCodeGen
            var ezDbCodeGenOnlyRelationships = ezDbCodeGenRelationshipPairs
                .Except(efCoreRelationshipPairs)
                .ToList();

            _output.WriteLine($"Common relationships: {commonRelationships.Count}");
            _output.WriteLine($"EF Core only relationships: {efCoreOnlyRelationships.Count}");
            _output.WriteLine($"EzDbCodeGen only relationships: {ezDbCodeGenOnlyRelationships.Count}");

            // Output some examples of differences
            if (efCoreOnlyRelationships.Any())
            {
                _output.WriteLine("Examples of relationships only detected by EF Core:");
                foreach (var rel in efCoreOnlyRelationships.Take(5))
                {
                    _output.WriteLine($"  {rel.Source} -> {rel.Target} (IsCollection: {rel.IsCollection})");
                }
            }

            if (ezDbCodeGenOnlyRelationships.Any())
            {
                _output.WriteLine("Examples of relationships only detected by EzDbCodeGen:");
                foreach (var rel in ezDbCodeGenOnlyRelationships.Take(5))
                {
                    _output.WriteLine($"  {rel.Source} -> {rel.Target} (IsCollection: {rel.IsCollection})");
                }
            }
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Compare_Schema_Coverage(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act - Get EF Core schema info
            var efCoreModel = dbContext.Model;
            var efCoreTables = efCoreModel.GetEntityTypes()
                .Select(e => e.GetTableName())
                .Where(t => t != null)
                .ToList();

            // Act - Get EzDbCodeGen schema info
            var ezDbCodeGenMetadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            var ezDbCodeGenTables = ezDbCodeGenMetadata.Tables
                .Select(t => t.Name)
                .ToList();

            // Assert
            _output.WriteLine($"=== Schema Coverage Comparison for {databaseName} ===");
            _output.WriteLine($"EF Core discovered {efCoreTables.Count} tables");
            _output.WriteLine($"EzDbCodeGen discovered {ezDbCodeGenTables.Count} tables");

            // Find common tables
            var commonTables = efCoreTables.Intersect(ezDbCodeGenTables, StringComparer.OrdinalIgnoreCase).ToList();
            
            // Find tables only in EF Core
            var efCoreOnlyTables = efCoreTables.Except(ezDbCodeGenTables, StringComparer.OrdinalIgnoreCase).ToList();
            
            // Find tables only in EzDbCodeGen
            var ezDbCodeGenOnlyTables = ezDbCodeGenTables.Except(efCoreTables, StringComparer.OrdinalIgnoreCase).ToList();

            _output.WriteLine($"Common tables: {commonTables.Count}");
            _output.WriteLine($"EF Core only tables: {efCoreOnlyTables.Count}");
            _output.WriteLine($"EzDbCodeGen only tables: {ezDbCodeGenOnlyTables.Count}");

            // Output some examples
            if (efCoreOnlyTables.Any())
            {
                _output.WriteLine("Examples of tables only discovered by EF Core:");
                foreach (var table in efCoreOnlyTables.Take(10))
                {
                    _output.WriteLine($"  {table}");
                }
            }

            if (ezDbCodeGenOnlyTables.Any())
            {
                _output.WriteLine("Examples of tables only discovered by EzDbCodeGen:");
                foreach (var table in ezDbCodeGenOnlyTables.Take(10))
                {
                    _output.WriteLine($"  {table}");
                }
            }
        }

        private DbContext CreateDbContext(string databaseName, string connectionString)
        {
            switch (databaseName)
            {
                case "Northwind":
                    return new NorthwindContext(connectionString);
                case "AdventureWorks":
                    return new AdventureWorksContext(connectionString);
                case "WideWorldImporters":
                    return new WideWorldImportersContext(connectionString);
                case "ContosoDataWarehouse":
                    return new ContosoDataWarehouseContext(connectionString);
                default:
                    throw new ArgumentException($"Unknown database name: {databaseName}", nameof(databaseName));
            }
        }
    }
}
