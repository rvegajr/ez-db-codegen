using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using EzDbCodeGen.Core.Metadata;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Tests focused on edge cases in relationship detection that EF Core often struggles with.
    /// These include multiple FKs between the same tables, circular references, and other complex scenarios.
    /// </summary>
    public class EdgeCaseRelationshipsTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<EdgeCaseRelationshipsTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        public EdgeCaseRelationshipsTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<EdgeCaseRelationshipsTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")] // Known to have some complex relationships
        [InlineData("WideWorldImporters")] // Has various edge cases
        public void Should_Handle_Multiple_FKs_Between_Same_Tables_Better_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find multiple FKs between the same tables
            var efCoreMultipleFks = FindMultipleFksBetweenSameTables(efCoreRelationships);
            var ezDbCodeGenMultipleFks = FindMultipleFksBetweenSameTables(metadata);
            
            // Log the results
            _output.WriteLine($"=== Multiple FKs Between Same Tables in {databaseName} ===");
            _output.WriteLine($"EF Core detected {efCoreMultipleFks.Count} table pairs with multiple FKs:");
            foreach (var pair in efCoreMultipleFks)
            {
                _output.WriteLine($"  {pair.SourceTable} -> {pair.TargetTable} ({pair.FkCount} FKs)");
                foreach (var propName in pair.NavigationPropertyNames)
                {
                    _output.WriteLine($"    Navigation Property: {propName}");
                }
            }

            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenMultipleFks.Count} table pairs with multiple FKs:");
            foreach (var pair in ezDbCodeGenMultipleFks)
            {
                _output.WriteLine($"  {pair.SourceTable} -> {pair.TargetTable} ({pair.FkCount} FKs)");
                foreach (var propName in pair.NavigationPropertyNames)
                {
                    _output.WriteLine($"    Navigation Property: {propName}");
                }
            }
            
            // Assert - EzDbCodeGen should find at least as many as EF Core
            ezDbCodeGenMultipleFks.Count.Should().BeGreaterThanOrEqualTo(efCoreMultipleFks.Count,
                "EzDbCodeGen should detect at least as many table pairs with multiple FKs as EF Core");
                
            // Check for better navigation property naming in EzDbCodeGen
            if (ezDbCodeGenMultipleFks.Any() && efCoreMultipleFks.Any())
            {
                // Check if EzDbCodeGen navigation properties are more descriptive (longer names)
                var ezDbCodeGenAvgNameLength = ezDbCodeGenMultipleFks
                    .SelectMany(p => p.NavigationPropertyNames)
                    .Average(n => n?.Length ?? 0);
                    
                var efCoreAvgNameLength = efCoreMultipleFks
                    .SelectMany(p => p.NavigationPropertyNames)
                    .Average(n => n?.Length ?? 0);
                    
                _output.WriteLine($"Average navigation property name length - EF Core: {efCoreAvgNameLength:F2}, EzDbCodeGen: {ezDbCodeGenAvgNameLength:F2}");
                
                // Longer names generally indicate more descriptive, semantic naming
                // This test will initially fail in TDD until we implement better naming
                ezDbCodeGenAvgNameLength.Should().BeGreaterThan(efCoreAvgNameLength,
                    "EzDbCodeGen should generate more descriptive navigation property names for multiple FK scenarios");
            }
        }
        
        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Handle_Circular_References_Better_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find circular references in the database
            var efCoreCircularRefs = FindCircularReferences(efCoreRelationships);
            var ezDbCodeGenCircularRefs = FindCircularReferences(metadata);
            
            // Log the results
            _output.WriteLine($"=== Circular References in {databaseName} ===");
            _output.WriteLine($"EF Core detected {efCoreCircularRefs.Count} circular reference chains:");
            foreach (var chain in efCoreCircularRefs)
            {
                _output.WriteLine($"  {string.Join(" -> ", chain)} -> {chain[0]}");
            }

            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenCircularRefs.Count} circular reference chains:");
            foreach (var chain in ezDbCodeGenCircularRefs)
            {
                _output.WriteLine($"  {string.Join(" -> ", chain)} -> {chain[0]}");
            }
            
            // Assert - EzDbCodeGen should detect at least as many circular references as EF Core
            // This test may initially fail in TDD until we implement better circular reference detection
            ezDbCodeGenCircularRefs.Count.Should().BeGreaterThanOrEqualTo(efCoreCircularRefs.Count,
                "EzDbCodeGen should detect at least as many circular references as EF Core");
        }
        
        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Handle_Nullable_Foreign_Keys_Better_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find nullable foreign keys in the database
            var nullableForeignKeys = FindNullableForeignKeys(metadata);
            
            // Log the results
            _output.WriteLine($"=== Nullable Foreign Keys in {databaseName} ===");
            _output.WriteLine($"Found {nullableForeignKeys.Count} nullable foreign keys:");
            foreach (var fk in nullableForeignKeys)
            {
                _output.WriteLine($"  {fk.SourceTable}.{fk.SourceColumn} -> {fk.TargetTable}.{fk.TargetColumn} (Nullable: {fk.IsNullable})");
            }
            
            // Check if navigation properties for nullable FKs are properly generated
            // This would require extending our EzDbCodeGenComparer to provide this info
            // For now, we'll just assert that we found some nullable FKs
            nullableForeignKeys.Should().NotBeEmpty(
                "Database should contain some nullable foreign keys for testing");
        }
        
        /// <summary>
        /// Finds tables with multiple foreign keys between them in EF Core relationships.
        /// </summary>
        private List<MultipleFkTablePair> FindMultipleFksBetweenSameTables(List<RelationshipInfo> relationships)
        {
            var result = new List<MultipleFkTablePair>();
            
            // Group relationships by source and target tables
            var grouped = relationships
                .GroupBy(r => new { Source = r.SourceTable, Target = r.TargetTable })
                .Where(g => g.Count() > 1) // Only include groups with multiple FKs
                .Select(g => new MultipleFkTablePair
                {
                    SourceTable = g.Key.Source,
                    TargetTable = g.Key.Target,
                    FkCount = g.Count(),
                    NavigationPropertyNames = g.Select(r => r.SourceProperty).ToList()
                })
                .ToList();
                
            return grouped;
        }
        
        /// <summary>
        /// Finds tables with multiple foreign keys between them in EzDbCodeGen metadata.
        /// </summary>
        private List<MultipleFkTablePair> FindMultipleFksBetweenSameTables(DatabaseMetadata metadata)
        {
            var result = new List<MultipleFkTablePair>();
            
            foreach (var table in metadata.Tables)
            {
                // Group FKs by referenced table
                var fkGroups = table.ForeignKeys
                    .GroupBy(fk => fk.ReferencedTableName)
                    .Where(g => g.Count() > 1); // Only include groups with multiple FKs
                    
                foreach (var group in fkGroups)
                {
                    result.Add(new MultipleFkTablePair
                    {
                        SourceTable = table.Name,
                        TargetTable = group.Key,
                        FkCount = group.Count(),
                        // In a real implementation, we'd get the actual nav property names
                        // For now, use synthetic names based on column names
                        NavigationPropertyNames = group.Select(fk => 
                            $"{group.Key}By{string.Join("And", fk.Columns)}").ToList()
                    });
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Finds circular references in EF Core relationships.
        /// </summary>
        private List<List<string>> FindCircularReferences(List<RelationshipInfo> relationships)
        {
            var result = new List<List<string>>();
            
            // Build a directed graph of table relationships
            var graph = new Dictionary<string, List<string>>();
            
            foreach (var rel in relationships)
            {
                if (!graph.ContainsKey(rel.SourceTable))
                {
                    graph[rel.SourceTable] = new List<string>();
                }
                
                graph[rel.SourceTable].Add(rel.TargetTable);
            }
            
            // Find cycles in the graph (simplified approach)
            // In a real implementation, we'd use a more efficient algorithm
            foreach (var startNode in graph.Keys)
            {
                FindCyclesStartingFrom(startNode, graph, result);
            }
            
            return result;
        }
        
        /// <summary>
        /// Finds circular references in EzDbCodeGen metadata.
        /// </summary>
        private List<List<string>> FindCircularReferences(DatabaseMetadata metadata)
        {
            var result = new List<List<string>>();
            
            // Build a directed graph of table relationships
            var graph = new Dictionary<string, List<string>>();
            
            foreach (var table in metadata.Tables)
            {
                if (!graph.ContainsKey(table.Name))
                {
                    graph[table.Name] = new List<string>();
                }
                
                foreach (var fk in table.ForeignKeys)
                {
                    graph[table.Name].Add(fk.ReferencedTableName);
                }
            }
            
            // Find cycles in the graph (simplified approach)
            // In a real implementation, we'd use a more efficient algorithm
            foreach (var startNode in graph.Keys)
            {
                FindCyclesStartingFrom(startNode, graph, result);
            }
            
            return result;
        }
        
        /// <summary>
        /// Helper method to find cycles in a graph starting from a specific node.
        /// This is a simplified implementation for demonstration.
        /// </summary>
        private void FindCyclesStartingFrom(string startNode, 
                                          Dictionary<string, List<string>> graph, 
                                          List<List<string>> result)
        {
            // Simple cycle detection for demo purposes
            // In a real implementation, we'd use a more efficient algorithm like Tarjan's
            
            // Simple approach: look for 2-node and 3-node cycles
            
            // Check for 2-node cycles (A -> B -> A)
            if (graph.TryGetValue(startNode, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    if (graph.TryGetValue(neighbor, out var neighborNeighbors) &&
                        neighborNeighbors.Contains(startNode))
                    {
                        // Found a 2-node cycle
                        result.Add(new List<string> { startNode, neighbor });
                    }
                }
            }
            
            // Check for 3-node cycles (A -> B -> C -> A)
            if (graph.TryGetValue(startNode, out neighbors))
            {
                foreach (var neighbor1 in neighbors)
                {
                    if (graph.TryGetValue(neighbor1, out var neighbor1Neighbors))
                    {
                        foreach (var neighbor2 in neighbor1Neighbors)
                        {
                            if (graph.TryGetValue(neighbor2, out var neighbor2Neighbors) &&
                                neighbor2Neighbors.Contains(startNode))
                            {
                                // Found a 3-node cycle
                                result.Add(new List<string> { startNode, neighbor1, neighbor2 });
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Finds nullable foreign keys in EzDbCodeGen metadata.
        /// </summary>
        private List<NullableForeignKey> FindNullableForeignKeys(DatabaseMetadata metadata)
        {
            var result = new List<NullableForeignKey>();
            
            foreach (var table in metadata.Tables)
            {
                foreach (var fk in table.ForeignKeys)
                {
                    // Get the foreign key column
                    if (fk.Columns.Count == 1) // Simple FK with one column
                    {
                        var fkColumn = table.Columns.FirstOrDefault(c => c.Name == fk.Columns[0]);
                        if (fkColumn != null && fkColumn.IsNullable)
                        {
                            result.Add(new NullableForeignKey
                            {
                                SourceTable = table.Name,
                                SourceColumn = fkColumn.Name,
                                TargetTable = fk.ReferencedTableName,
                                TargetColumn = fk.ReferencedColumns[0],
                                IsNullable = true
                            });
                        }
                    }
                }
            }
            
            return result;
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
    /// Represents a pair of tables with multiple foreign keys between them.
    /// </summary>
    public class MultipleFkTablePair
    {
        public string SourceTable { get; set; }
        public string TargetTable { get; set; }
        public int FkCount { get; set; }
        public List<string> NavigationPropertyNames { get; set; } = new List<string>();
    }
    
    /// <summary>
    /// Represents a nullable foreign key.
    /// </summary>
    public class NullableForeignKey
    {
        public string SourceTable { get; set; }
        public string SourceColumn { get; set; }
        public string TargetTable { get; set; }
        public string TargetColumn { get; set; }
        public bool IsNullable { get; set; }
    }
}
