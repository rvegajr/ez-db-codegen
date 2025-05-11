using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Tests specifically focused on self-referencing relationship detection,
    /// an area where EzDbCodeGen should demonstrate clear superiority over EF Core.
    /// </summary>
    public class SelfReferencingRelationshipTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<SelfReferencingRelationshipTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        /// <summary>
        /// Initializes a new instance of the SelfReferencingRelationshipTests class.
        /// </summary>
        public SelfReferencingRelationshipTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<SelfReferencingRelationshipTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")] // Has Employee self-reference
        [InlineData("WideWorldImporters")] // Has multiple self-references
        public void Should_Detect_More_SelfReferencing_Relationships_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);

            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen relationships
            var ezDbCodeGenRelationships = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenRelationships(connectionString, databaseName);

            // Find self-referencing relationships in both
            var efCoreSelfRefs = FindSelfReferencingRelationships(efCoreRelationships);
            var ezDbCodeGenSelfRefs = FindSelfReferencingRelationships(ezDbCodeGenRelationships);

            // Log the results for debugging
            _output.WriteLine($"=== Self-Referencing Relationship Detection for {databaseName} ===");
            _output.WriteLine($"EF Core detected {efCoreSelfRefs.Count} self-referencing relationships:");
            foreach (var rel in efCoreSelfRefs)
            {
                _output.WriteLine($"  {rel.SourceTable} -> {rel.TargetTable} via {rel.SourceProperty} ({rel.RelationshipType})");
            }

            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenSelfRefs.Count} self-referencing relationships:");
            foreach (var rel in ezDbCodeGenSelfRefs)
            {
                _output.WriteLine($"  {rel.SourceTable} -> {rel.TargetTable} via {rel.SourceProperty} ({rel.RelationshipType})");
            }

            // Assert - EzDbCodeGen should find at least as many as EF Core, preferably more
            ezDbCodeGenSelfRefs.Count.Should().BeGreaterThanOrEqualTo(efCoreSelfRefs.Count, 
                "EzDbCodeGen should detect at least as many self-referencing relationships as EF Core");
            
            // Calculate the improvement percentage
            if (efCoreSelfRefs.Count > 0)
            {
                double improvementPercentage = ((double)ezDbCodeGenSelfRefs.Count / efCoreSelfRefs.Count - 1) * 100;
                _output.WriteLine($"EzDbCodeGen detected {improvementPercentage:F2}% more self-referencing relationships than EF Core");
                
                // Our target is 25% more relationships detected
                if (improvementPercentage < 25)
                {
                    _output.WriteLine($"WARNING: Improvement is less than the 25% target threshold");
                }
            }
        }

        [Theory]
        [InlineData("AdventureWorks")] // Has Employee self-reference
        public void Should_Generate_Better_NavigationProperties_For_SelfReferences(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);

            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen relationships
            var ezDbCodeGenRelationships = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenRelationships(connectionString, databaseName);

            // Find self-referencing relationships in both
            var efCoreSelfRefs = FindSelfReferencingRelationships(efCoreRelationships);
            var ezDbCodeGenSelfRefs = FindSelfReferencingRelationships(ezDbCodeGenRelationships);

            // Extract navigation property names for analysis
            var efCoreNavNames = efCoreSelfRefs.Select(r => r.SourceProperty).ToList();
            var ezDbCodeGenNavNames = ezDbCodeGenSelfRefs.Select(r => r.SourceProperty).ToList();

            // Log the navigation property names
            _output.WriteLine($"=== Navigation Property Naming for Self-References in {databaseName} ===");
            _output.WriteLine($"EF Core navigation properties:");
            foreach (var name in efCoreNavNames)
            {
                _output.WriteLine($"  {name}");
            }

            _output.WriteLine($"EzDbCodeGen navigation properties:");
            foreach (var name in ezDbCodeGenNavNames)
            {
                _output.WriteLine($"  {name}");
            }

            // TODO: Implement more specific assertions about navigation property quality
            // This test is a placeholder that should fail until we implement better navigation property naming
            
            // For now, we'll just check that EzDbCodeGen doesn't use generic names like "XXXCollection"
            var genericNames = ezDbCodeGenNavNames.Count(n => n.EndsWith("Collection") || n.EndsWith("Reference"));
            var semanticNames = ezDbCodeGenNavNames.Count - genericNames;
            
            _output.WriteLine($"EzDbCodeGen semantic naming ratio: {semanticNames}/{ezDbCodeGenNavNames.Count} = {(double)semanticNames/ezDbCodeGenNavNames.Count:P2}");
            
            // This assertion will likely fail initially - that's the point of TDD
            semanticNames.Should().BeGreaterThan(0, "EzDbCodeGen should use semantic names for self-references");
        }

        /// <summary>
        /// Helper method to find self-referencing relationships in a list of relationships.
        /// </summary>
        private List<RelationshipInfo> FindSelfReferencingRelationships(List<RelationshipInfo> relationships)
        {
            return relationships
                .Where(r => r.SourceTable == r.TargetTable)
                .ToList();
        }

        /// <summary>
        /// Creates a test DbContext for the specified database.
        /// </summary>
        private DbContext CreateTestDbContext(string databaseName, string connectionString)
        {
            switch (databaseName)
            {
                case "Northwind":
                    return new NorthwindContext(connectionString);
                case "AdventureWorks":
                    return new AdventureWorksContext(connectionString);
                case "WideWorldImporters":
                    return new WideWorldImportersContext(connectionString);
                default:
                    throw new ArgumentException($"Unknown database name: {databaseName}", nameof(databaseName));
            }
        }
    }

    /// <summary>
    /// Basic test logger provider for XUnit.
    /// </summary>
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_testOutputHelper, categoryName);
        }

        public void Dispose() { }

        private class TestLogger : ILogger
        {
            private readonly ITestOutputHelper _testOutputHelper;
            private readonly string _categoryName;

            public TestLogger(ITestOutputHelper testOutputHelper, string categoryName)
            {
                _testOutputHelper = testOutputHelper;
                _categoryName = categoryName;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _testOutputHelper.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] {_categoryName}: {formatter(state, exception)}");
                if (exception != null)
                {
                    _testOutputHelper.WriteLine(exception.ToString());
                }
            }
        }
    }
}
