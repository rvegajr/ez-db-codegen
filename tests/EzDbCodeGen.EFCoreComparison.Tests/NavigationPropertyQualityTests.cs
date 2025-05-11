using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using EzDbCodeGen.Core.Metadata;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Tests focused on evaluating the quality of navigation property naming,
    /// an area where EzDbCodeGen should demonstrate clear superiority over EF Core.
    /// </summary>
    public class NavigationPropertyQualityTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<NavigationPropertyQualityTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        public NavigationPropertyQualityTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<NavigationPropertyQualityTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Generate_Semantic_Navigation_Property_Names(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen relationships
            var ezDbCodeGenRelationships = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenRelationships(connectionString, databaseName);
            
            // Evaluate navigation property name quality
            var efCoreNameQuality = CalculateNavigationPropertyNameQuality(efCoreRelationships);
            var ezDbCodeGenNameQuality = CalculateNavigationPropertyNameQuality(ezDbCodeGenRelationships);
            
            // Log the results
            _output.WriteLine($"=== Navigation Property Name Quality for {databaseName} ===");
            _output.WriteLine($"EF Core average name quality score: {efCoreNameQuality:F2}/10");
            _output.WriteLine($"EzDbCodeGen average name quality score: {ezDbCodeGenNameQuality:F2}/10");
            
            // Log some examples
            _output.WriteLine("Navigation property name examples from EF Core:");
            foreach (var rel in efCoreRelationships.Take(5))
            {
                _output.WriteLine($"  {rel.SourceTable}.{rel.SourceProperty} -> {rel.TargetTable} (Score: {ScoreNavigationPropertyName(rel.SourceProperty):F1}/10)");
            }
            
            _output.WriteLine("Navigation property name examples from EzDbCodeGen:");
            foreach (var rel in ezDbCodeGenRelationships.Take(5))
            {
                _output.WriteLine($"  {rel.SourceTable}.{rel.SourceProperty} -> {rel.TargetTable} (Score: {ScoreNavigationPropertyName(rel.SourceProperty):F1}/10)");
            }
            
            // Assert - EzDbCodeGen should have higher quality navigation property names
            // This test will initially fail under TDD until we implement better naming
            ezDbCodeGenNameQuality.Should().BeGreaterThan(efCoreNameQuality, 
                "EzDbCodeGen should generate higher quality navigation property names than EF Core");
        }
        
        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Use_Domain_Specific_Terms_In_Navigation_Properties(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen relationships
            var ezDbCodeGenRelationships = _ezDbCodeGenComparer.AnalyzeEzDbCodeGenRelationships(connectionString, databaseName);
            
            // Count domain-specific vs. generic names
            var efCoreDomainTerms = CountDomainSpecificTerms(efCoreRelationships);
            var ezDbCodeGenDomainTerms = CountDomainSpecificTerms(ezDbCodeGenRelationships);
            
            // Log the results
            _output.WriteLine($"=== Domain-Specific Terminology in Navigation Properties for {databaseName} ===");
            _output.WriteLine($"EF Core: {efCoreDomainTerms.DomainSpecific} domain-specific vs. {efCoreDomainTerms.Generic} generic names");
            _output.WriteLine($"EF Core domain-specific percentage: {efCoreDomainTerms.Percentage:P2}");
            _output.WriteLine($"EzDbCodeGen: {ezDbCodeGenDomainTerms.DomainSpecific} domain-specific vs. {ezDbCodeGenDomainTerms.Generic} generic names");
            _output.WriteLine($"EzDbCodeGen domain-specific percentage: {ezDbCodeGenDomainTerms.Percentage:P2}");
            
            // Assert - EzDbCodeGen should use more domain-specific terms
            // This test will initially fail under TDD until we implement better naming
            ezDbCodeGenDomainTerms.Percentage.Should().BeGreaterThan(efCoreDomainTerms.Percentage,
                "EzDbCodeGen should use more domain-specific terminology in navigation property names");
        }
        
        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Avoid_Navigation_Property_Name_Collisions(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Check for property name collisions in each entity
            var collisions = new Dictionary<string, List<string>>();
            
            foreach (var table in metadata.Tables)
            {
                var columnNames = table.Columns.Select(c => c.Name).ToList();
                var navigationPropertyNames = GetNavigationPropertiesForTable(table, metadata);
                
                // Check for navigation property names that would collide with existing column names
                var collidingNames = navigationPropertyNames.Intersect(columnNames, StringComparer.OrdinalIgnoreCase).ToList();
                
                if (collidingNames.Any())
                {
                    collisions[table.Name] = collidingNames;
                }
            }
            
            // Log the results
            _output.WriteLine($"=== Navigation Property Name Collisions in {databaseName} ===");
            if (collisions.Any())
            {
                _output.WriteLine($"Found {collisions.Count} entities with navigation property name collisions:");
                foreach (var entity in collisions.Keys)
                {
                    _output.WriteLine($"  Entity: {entity}, Colliding names: {string.Join(", ", collisions[entity])}");
                }
            }
            else
            {
                _output.WriteLine("No navigation property name collisions detected.");
            }
            
            // Assert - EzDbCodeGen should not have navigation property name collisions
            // This test will initially fail under TDD until we implement collision avoidance
            collisions.Should().BeEmpty("EzDbCodeGen should avoid navigation property name collisions with entity properties");
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
        
        /// <summary>
        /// Calculates the average quality score for navigation property names.
        /// </summary>
        private double CalculateNavigationPropertyNameQuality(List<RelationshipInfo> relationships)
        {
            if (!relationships.Any())
            {
                return 0;
            }
            
            // Calculate the average score
            return relationships
                .Select(r => ScoreNavigationPropertyName(r.SourceProperty))
                .Average();
        }
        
        /// <summary>
        /// Scores a navigation property name for quality on a scale of 0-10.
        /// </summary>
        private double ScoreNavigationPropertyName(string propertyName)
        {
            double score = 5.0; // Start with a neutral score
            
            if (string.IsNullOrEmpty(propertyName))
            {
                return 0;
            }
            
            // Generic names like "Products" or "OrdersCollection" are less preferred
            if (propertyName.EndsWith("s") && !propertyName.EndsWith("ss"))
            {
                score += 1; // Basic pluralization is slightly better than nothing
            }
            
            if (propertyName.EndsWith("List") || propertyName.EndsWith("Collection"))
            {
                score -= 1; // Too generic
            }
            
            if (propertyName.EndsWith("Reference"))
            {
                score -= 1; // Too generic
            }
            
            // Very generic names get penalized more
            if (propertyName == "Items" || propertyName == "Elements" || propertyName == "Entities")
            {
                score -= 2;
            }
            
            // Specific domain terms or combined terms are better
            // For example "OrderItems" is better than "Items"
            if (propertyName.Length > 6 && !propertyName.EndsWith("Collection") && !propertyName.EndsWith("Reference"))
            {
                score += 2;
            }
            
            // Prefer Pascal case
            if (char.IsUpper(propertyName[0]))
            {
                score += 1;
            }
            
            // Check for very descriptive names that likely include role information
            if (propertyName.Contains("By") || propertyName.Contains("For") || propertyName.Contains("With"))
            {
                score += 2;
            }
            
            // Cap the score between 0 and 10
            return Math.Max(0, Math.Min(10, score));
        }
        
        /// <summary>
        /// Counts domain-specific vs. generic navigation property names.
        /// </summary>
        private (int DomainSpecific, int Generic, double Percentage) CountDomainSpecificTerms(List<RelationshipInfo> relationships)
        {
            if (!relationships.Any())
            {
                return (0, 0, 0);
            }
            
            int domainSpecific = 0;
            int generic = 0;
            
            foreach (var rel in relationships)
            {
                if (IsGenericName(rel.SourceProperty))
                {
                    generic++;
                }
                else
                {
                    domainSpecific++;
                }
            }
            
            double percentage = (double)domainSpecific / relationships.Count;
            return (domainSpecific, generic, percentage);
        }
        
        /// <summary>
        /// Determines if a navigation property name is generic (non-domain-specific).
        /// </summary>
        private bool IsGenericName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return true;
            }
            
            // Check for generic collection names
            if (propertyName.EndsWith("Collection") || propertyName.EndsWith("List") || 
                propertyName.EndsWith("Set") || propertyName == "Items" || 
                propertyName == "Elements" || propertyName == "Entities")
            {
                return true;
            }
            
            // Check for generic reference names
            if (propertyName.EndsWith("Reference") || propertyName.EndsWith("Entity") ||
                propertyName == "Parent" || propertyName == "Related")
            {
                return true;
            }
            
            // Look for names that are just the table name + "s" or table name directly
            // This pattern check is a simplification, would need more context in real implementation
            var pluralPattern = new Regex(@"^[A-Z][a-z]+s$");
            if (pluralPattern.IsMatch(propertyName))
            {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the list of navigation property names for a given table.
        /// </summary>
        private List<string> GetNavigationPropertiesForTable(TableMetadata table, DatabaseMetadata metadata)
        {
            var navigationProperties = new List<string>();
            
            // In a real implementation, we would use our relationship detection to determine
            // what navigation properties would be generated for this table
            // For this test, we'll use a simplified approach
            
            // Outgoing relationships (this table has FKs to other tables)
            foreach (var foreignKey in table.ForeignKeys)
            {
                // Add singular navigation property name (to reference the target table)
                navigationProperties.Add(foreignKey.ReferencedTableName);
            }
            
            // Incoming relationships (other tables have FKs to this table)
            foreach (var otherTable in metadata.Tables)
            {
                foreach (var foreignKey in otherTable.ForeignKeys)
                {
                    if (foreignKey.ReferencedTableName == table.Name)
                    {
                        // Add plural navigation property name (collection of related tables)
                        navigationProperties.Add(otherTable.Name + "s");
                    }
                }
            }
            
            return navigationProperties;
        }
    }
}
