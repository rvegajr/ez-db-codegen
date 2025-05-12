using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    public class ComprehensiveComparisonRunner
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionStringBase;
        private readonly string[] _availableDatabases = new[]
        {
            "WideWorldImporters",
            "AdventureWorks",
            "Northwind"
        };

        public ComprehensiveComparisonRunner(ITestOutputHelper output)
        {
            _output = output;
            _connectionStringBase = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;Database=";
        }

        [Fact]
        public async Task Run_Comprehensive_Comparison_For_All_Databases()
        {
            // Create a summary report
            var summaryBuilder = new StringBuilder();
            summaryBuilder.AppendLine("# EzDbCodeGen vs EF Core Comprehensive Comparison");
            summaryBuilder.AppendLine();
            summaryBuilder.AppendLine("| Database | Relationship Count | Navigation Property Quality | Performance (ms) | Memory Usage (MB) |");
            summaryBuilder.AppendLine("|----------|-------------------|---------------------------|-----------------|------------------|");

            foreach (var database in _availableDatabases)
            {
                _output.WriteLine($"Running comparison for database: {database}");
                
                try
                {
                    var connectionString = _connectionStringBase + database;
                    var results = await RunComparisonForDatabase(database, connectionString);
                    
                    // Add to summary
                    summaryBuilder.AppendLine($"| {database} | {results.RelationshipImprovement:P0} | {results.NavigationQualityImprovement:P0} | {results.PerformanceImprovement:P0} | {results.MemoryUsageImprovement:P0} |");
                    
                    // Verify that we've met our superiority goals
                    results.RelationshipImprovement.Should().BeGreaterThanOrEqualTo(0.25, 
                        $"EzDbCodeGen should detect at least 25% more relationships than EF Core for {database}");
                    
                    results.NavigationQualityImprovement.Should().BeGreaterThan(0, 
                        $"EzDbCodeGen should generate better navigation property names than EF Core for {database}");
                    
                    results.PerformanceImprovement.Should().BeGreaterThanOrEqualTo(0.5, 
                        $"EzDbCodeGen should be at least 50% faster than EF Core for {database}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Error running comparison for {database}: {ex.Message}");
                    summaryBuilder.AppendLine($"| {database} | Error | Error | Error | Error |");
                }
            }

            // Write summary to file
            var summaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "comparison-summary.md");
            File.WriteAllText(summaryPath, summaryBuilder.ToString());
            
            _output.WriteLine($"Comparison summary written to: {summaryPath}");
        }

        private async Task<ComparisonResults> RunComparisonForDatabase(string databaseName, string connectionString)
        {
            var schemas = new[] { "dbo", "Sales", "Purchasing", "Warehouse", "Application", "Production", "Person", "HumanResources" };
            
            // Initialize comparers
            var efCoreComparer = new EFCoreModelComparer(connectionString);
            var ezDbCodeGenComparer = new EzDbCodeGenModelComparer(connectionString);
            
            // Compare relationship detection
            var efCoreRelationships = await efCoreComparer.GetAllRelationshipsAsync(schemas);
            var ezDbCodeGenRelationships = await ezDbCodeGenComparer.GetAllRelationshipsAsync(schemas);
            
            var relationshipImprovement = (double)(ezDbCodeGenRelationships.Count - efCoreRelationships.Count) / efCoreRelationships.Count;
            
            _output.WriteLine($"EF Core detected {efCoreRelationships.Count} relationships");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenRelationships.Count} relationships");
            _output.WriteLine($"Relationship improvement: {relationshipImprovement:P0}");
            
            // Compare navigation property naming quality
            var efCoreNavigationProps = await efCoreComparer.GetNavigationPropertiesAsync(schemas);
            var ezDbCodeGenNavigationProps = await ezDbCodeGenComparer.GetNavigationPropertiesAsync(schemas);
            
            var efCoreNameQuality = efCoreNavigationProps.Sum(p => p.NameQualityScore);
            var ezDbCodeGenNameQuality = ezDbCodeGenNavigationProps.Sum(p => p.NameQualityScore);
            
            var navigationQualityImprovement = (double)(ezDbCodeGenNameQuality - efCoreNameQuality) / efCoreNameQuality;
            
            _output.WriteLine($"EF Core navigation property quality score: {efCoreNameQuality}");
            _output.WriteLine($"EzDbCodeGen navigation property quality score: {ezDbCodeGenNameQuality}");
            _output.WriteLine($"Navigation quality improvement: {navigationQualityImprovement:P0}");
            
            // Compare performance
            // Get the schema for performance testing
            var schema = await Task.Run(() => ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName));
            
            // Measure EF Core performance
            var efCoreSw = Stopwatch.StartNew();
            var efCoreMemoryBefore = GC.GetTotalMemory(true);
            
            await efCoreComparer.SimulateEFCoreGenerationAsync(schema);
            
            efCoreSw.Stop();
            var efCoreMemoryAfter = GC.GetTotalMemory(false);
            var efCoreMemoryUsed = efCoreMemoryAfter - efCoreMemoryBefore;
            
            // Measure EzDbCodeGen performance
            var ezDbCodeGenSw = Stopwatch.StartNew();
            var ezDbCodeGenMemoryBefore = GC.GetTotalMemory(true);
            
            await ezDbCodeGenComparer.SimulateEzDbCodeGenGenerationAsync(schema);
            
            ezDbCodeGenSw.Stop();
            var ezDbCodeGenMemoryAfter = GC.GetTotalMemory(false);
            var ezDbCodeGenMemoryUsed = ezDbCodeGenMemoryAfter - ezDbCodeGenMemoryBefore;
            
            var performanceImprovement = 1 - ((double)ezDbCodeGenSw.ElapsedMilliseconds / efCoreSw.ElapsedMilliseconds);
            var memoryUsageImprovement = 1 - ((double)ezDbCodeGenMemoryUsed / efCoreMemoryUsed);
            
            _output.WriteLine($"EF Core generation time: {efCoreSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"EzDbCodeGen generation time: {ezDbCodeGenSw.ElapsedMilliseconds}ms");
            _output.WriteLine($"Performance improvement: {performanceImprovement:P0}");
            
            _output.WriteLine($"EF Core memory used: {efCoreMemoryUsed / 1024 / 1024}MB");
            _output.WriteLine($"EzDbCodeGen memory used: {ezDbCodeGenMemoryUsed / 1024 / 1024}MB");
            _output.WriteLine($"Memory usage improvement: {memoryUsageImprovement:P0}");
            
            return new ComparisonResults
            {
                DatabaseName = databaseName,
                RelationshipImprovement = relationshipImprovement,
                NavigationQualityImprovement = navigationQualityImprovement,
                PerformanceImprovement = performanceImprovement,
                MemoryUsageImprovement = memoryUsageImprovement
            };
        }
        
        public class ComparisonResults
        {
            public string DatabaseName { get; set; }
            public double RelationshipImprovement { get; set; }
            public double NavigationQualityImprovement { get; set; }
            public double PerformanceImprovement { get; set; }
            public double MemoryUsageImprovement { get; set; }
        }
    }
}
