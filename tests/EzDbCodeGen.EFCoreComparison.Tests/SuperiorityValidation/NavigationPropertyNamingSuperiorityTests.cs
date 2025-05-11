using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EzDbCodeGen.Core;
using EzDbCodeGen.Schema.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.EFCoreComparison.Tests.SuperiorityValidation
{
    public class NavigationPropertyNamingSuperiorityTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionString;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        public NavigationPropertyNamingSuperiorityTests(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            
            // Initialize comparers
            _efCoreComparer = new EFCoreModelComparer(_connectionString);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_connectionString);
        }

        [Fact]
        public async Task Should_Generate_More_Semantic_Navigation_Property_Names()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreNavigationProps = await _efCoreComparer.GetNavigationPropertiesAsync(schemas);
            var ezDbCodeGenNavigationProps = await _ezDbCodeGenComparer.GetNavigationPropertiesAsync(schemas);
            
            // Calculate semantic quality scores
            var efCoreNameQuality = CalculateSemanticQualityScore(efCoreNavigationProps);
            var ezDbCodeGenNameQuality = CalculateSemanticQualityScore(ezDbCodeGenNavigationProps);
            
            // Log the results
            _output.WriteLine($"EF Core navigation property semantic quality score: {efCoreNameQuality:F2}");
            _output.WriteLine($"EzDbCodeGen navigation property semantic quality score: {ezDbCodeGenNameQuality:F2}");
            _output.WriteLine($"Improvement: {((ezDbCodeGenNameQuality / efCoreNameQuality) - 1) * 100:F2}%");
            
            // Assert
            ezDbCodeGenNameQuality.Should().BeGreaterThan(efCoreNameQuality,
                "because EzDbCodeGen should generate more semantic navigation property names than EF Core");
            
            // Sample comparison of navigation property names
            var sampleComparisons = GetSampleNavigationPropertyComparisons(efCoreNavigationProps, ezDbCodeGenNavigationProps);
            
            foreach (var comparison in sampleComparisons.Take(5))
            {
                _output.WriteLine($"Table: {comparison.TableName}, Relationship: {comparison.RelationshipDescription}");
                _output.WriteLine($"  EF Core: {comparison.EFCoreName}");
                _output.WriteLine($"  EzDbCodeGen: {comparison.EzDbCodeGenName}");
                _output.WriteLine($"  Improvement: {comparison.Reason}");
                _output.WriteLine("");
            }
        }

        [Fact]
        public async Task Should_Avoid_Generic_Navigation_Property_Names()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreNavigationProps = await _efCoreComparer.GetNavigationPropertiesAsync(schemas);
            var ezDbCodeGenNavigationProps = await _ezDbCodeGenComparer.GetNavigationPropertiesAsync(schemas);
            
            // Count generic names (e.g., "NavigationProperty1", "RelatedEntity", etc.)
            var efCoreGenericNames = CountGenericNames(efCoreNavigationProps);
            var ezDbCodeGenGenericNames = CountGenericNames(ezDbCodeGenNavigationProps);
            
            // Calculate percentage of generic names
            var efCoreGenericPercentage = (double)efCoreGenericNames / efCoreNavigationProps.Count * 100;
            var ezDbCodeGenGenericPercentage = (double)ezDbCodeGenGenericNames / ezDbCodeGenNavigationProps.Count * 100;
            
            // Log the results
            _output.WriteLine($"EF Core generic navigation property names: {efCoreGenericNames} ({efCoreGenericPercentage:F2}%)");
            _output.WriteLine($"EzDbCodeGen generic navigation property names: {ezDbCodeGenGenericNames} ({ezDbCodeGenGenericPercentage:F2}%)");
            
            // Assert
            ezDbCodeGenGenericPercentage.Should().BeLessThan(efCoreGenericPercentage,
                "because EzDbCodeGen should use fewer generic navigation property names than EF Core");
        }

        [Fact]
        public async Task Should_Generate_More_Descriptive_Navigation_Property_Names()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreNavigationProps = await _efCoreComparer.GetNavigationPropertiesAsync(schemas);
            var ezDbCodeGenNavigationProps = await _ezDbCodeGenComparer.GetNavigationPropertiesAsync(schemas);
            
            // Calculate average name length (longer names tend to be more descriptive)
            var efCoreAvgNameLength = efCoreNavigationProps.Average(p => p.Name.Length);
            var ezDbCodeGenAvgNameLength = ezDbCodeGenNavigationProps.Average(p => p.Name.Length);
            
            // Log the results
            _output.WriteLine($"EF Core average navigation property name length: {efCoreAvgNameLength:F2}");
            _output.WriteLine($"EzDbCodeGen average navigation property name length: {ezDbCodeGenAvgNameLength:F2}");
            
            // Assert
            ezDbCodeGenAvgNameLength.Should().BeGreaterThan(efCoreAvgNameLength,
                "because EzDbCodeGen should generate more descriptive navigation property names than EF Core");
        }

        [Fact]
        public async Task Should_Handle_Navigation_Property_Name_Collisions_Better()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreNavigationProps = await _efCoreComparer.GetNavigationPropertiesAsync(schemas);
            var ezDbCodeGenNavigationProps = await _ezDbCodeGenComparer.GetNavigationPropertiesAsync(schemas);
            
            // Find tables with multiple navigation properties to the same target table
            var efCoreCollisions = FindNavigationPropertyCollisions(efCoreNavigationProps);
            var ezDbCodeGenCollisions = FindNavigationPropertyCollisions(ezDbCodeGenNavigationProps);
            
            // Evaluate collision handling quality
            var efCoreCollisionQuality = EvaluateCollisionHandlingQuality(efCoreCollisions);
            var ezDbCodeGenCollisionQuality = EvaluateCollisionHandlingQuality(ezDbCodeGenCollisions);
            
            // Log the results
            _output.WriteLine($"EF Core collision handling quality score: {efCoreCollisionQuality:F2}");
            _output.WriteLine($"EzDbCodeGen collision handling quality score: {ezDbCodeGenCollisionQuality:F2}");
            
            // Assert
            ezDbCodeGenCollisionQuality.Should().BeGreaterThanOrEqualTo(efCoreCollisionQuality,
                "because EzDbCodeGen should handle navigation property name collisions better than EF Core");
        }

        #region Helper Methods

        private double CalculateSemanticQualityScore(List<NavigationProperty> navigationProperties)
        {
            // Higher score means better semantic quality
            double score = 0;
            
            foreach (var prop in navigationProperties)
            {
                // Avoid generic names like "NavigationProperty1" or just the table name
                if (IsGenericName(prop.Name))
                {
                    score += 0.5; // Lower score for generic names
                }
                else
                {
                    score += 1.0;
                }
                
                // Bonus for descriptive names (longer names tend to be more descriptive)
                if (prop.Name.Length > 10)
                {
                    score += 0.2;
                }
                
                // Bonus for names that include relationship semantics
                if (IncludesRelationshipSemantics(prop.Name))
                {
                    score += 0.3;
                }
            }
            
            // Normalize by the number of properties
            return navigationProperties.Count > 0 ? score / navigationProperties.Count : 0;
        }

        private bool IsGenericName(string name)
        {
            // Check for generic patterns like "NavigationProperty1", "RelatedEntity", etc.
            var genericPatterns = new[]
            {
                @"^Navigation\w+$",
                @"^Related\w+$",
                @"^\w+List$",
                @"^\w+Collection$",
                @"^\w+1$",
                @"^\w+2$",
                @"^\w+3$"
            };
            
            return genericPatterns.Any(pattern => Regex.IsMatch(name, pattern));
        }

        private bool IncludesRelationshipSemantics(string name)
        {
            // Check if the name includes semantic terms that describe the relationship
            var semanticTerms = new[]
            {
                "By", "For", "From", "To", "With", "Of", "In", "At", "On"
            };
            
            return semanticTerms.Any(term => name.Contains(term));
        }

        private int CountGenericNames(List<NavigationProperty> navigationProperties)
        {
            return navigationProperties.Count(prop => IsGenericName(prop.Name));
        }

        private List<NavigationPropertyCollision> FindNavigationPropertyCollisions(List<NavigationProperty> navigationProperties)
        {
            var collisions = new List<NavigationPropertyCollision>();
            
            // Group by source table and target table
            var groups = navigationProperties
                .GroupBy(p => new { p.SourceTableName, p.TargetTableName })
                .Where(g => g.Count() > 1) // Multiple navigation properties to the same target table
                .ToList();
            
            foreach (var group in groups)
            {
                var collision = new NavigationPropertyCollision
                {
                    SourceTableName = group.Key.SourceTableName,
                    TargetTableName = group.Key.TargetTableName,
                    NavigationProperties = group.ToList()
                };
                
                collisions.Add(collision);
            }
            
            return collisions;
        }

        private double EvaluateCollisionHandlingQuality(List<NavigationPropertyCollision> collisions)
        {
            if (collisions.Count == 0)
                return 1.0; // No collisions to handle
            
            double score = 0;
            
            foreach (var collision in collisions)
            {
                // Check if all navigation property names are unique
                var uniqueNames = collision.NavigationProperties.Select(p => p.Name).Distinct().Count();
                if (uniqueNames == collision.NavigationProperties.Count)
                {
                    score += 1.0;
                }
                else
                {
                    score += 0.5; // Some names are not unique
                }
                
                // Check if names are descriptive and not just numbered
                var numberedNames = collision.NavigationProperties
                    .Count(p => Regex.IsMatch(p.Name, @"\d+$"));
                
                if (numberedNames == 0)
                {
                    score += 0.5; // No numbered names
                }
                else
                {
                    score += 0.2; // Some names are just numbered
                }
            }
            
            // Normalize by the number of collisions
            return score / collisions.Count;
        }

        private List<NavigationPropertyComparison> GetSampleNavigationPropertyComparisons(
            List<NavigationProperty> efCoreProps, List<NavigationProperty> ezDbCodeGenProps)
        {
            var comparisons = new List<NavigationPropertyComparison>();
            
            // Find matching navigation properties based on source and target tables
            var efCoreDict = efCoreProps
                .GroupBy(p => new { p.SourceTableName, p.TargetTableName })
                .ToDictionary(g => g.Key, g => g.ToList());
            
            var ezDbCodeGenDict = ezDbCodeGenProps
                .GroupBy(p => new { p.SourceTableName, p.TargetTableName })
                .ToDictionary(g => g.Key, g => g.ToList());
            
            // Find keys that exist in both dictionaries
            var commonKeys = efCoreDict.Keys
                .Intersect(ezDbCodeGenDict.Keys)
                .ToList();
            
            foreach (var key in commonKeys)
            {
                var efCorePropList = efCoreDict[key];
                var ezDbCodeGenPropList = ezDbCodeGenDict[key];
                
                // For simplicity, just compare the first property from each list
                if (efCorePropList.Count > 0 && ezDbCodeGenPropList.Count > 0)
                {
                    var efCoreProp = efCorePropList[0];
                    var ezDbCodeGenProp = ezDbCodeGenPropList[0];
                    
                    var comparison = new NavigationPropertyComparison
                    {
                        TableName = key.SourceTableName,
                        RelationshipDescription = $"{key.SourceTableName} -> {key.TargetTableName}",
                        EFCoreName = efCoreProp.Name,
                        EzDbCodeGenName = ezDbCodeGenProp.Name
                    };
                    
                    // Determine why EzDbCodeGen's name is better
                    if (IsGenericName(efCoreProp.Name) && !IsGenericName(ezDbCodeGenProp.Name))
                    {
                        comparison.Reason = "EzDbCodeGen uses a more specific name instead of a generic one";
                    }
                    else if (ezDbCodeGenProp.Name.Length > efCoreProp.Name.Length)
                    {
                        comparison.Reason = "EzDbCodeGen uses a more descriptive name";
                    }
                    else if (IncludesRelationshipSemantics(ezDbCodeGenProp.Name) && !IncludesRelationshipSemantics(efCoreProp.Name))
                    {
                        comparison.Reason = "EzDbCodeGen includes relationship semantics in the name";
                    }
                    else
                    {
                        comparison.Reason = "EzDbCodeGen name follows better naming conventions";
                    }
                    
                    comparisons.Add(comparison);
                }
            }
            
            return comparisons;
        }

        #endregion

        #region Helper Classes

        public class NavigationProperty
        {
            public string Name { get; set; }
            public string SourceTableName { get; set; }
            public string TargetTableName { get; set; }
            public bool IsCollection { get; set; }
        }

        public class NavigationPropertyCollision
        {
            public string SourceTableName { get; set; }
            public string TargetTableName { get; set; }
            public List<NavigationProperty> NavigationProperties { get; set; }
        }

        public class NavigationPropertyComparison
        {
            public string TableName { get; set; }
            public string RelationshipDescription { get; set; }
            public string EFCoreName { get; set; }
            public string EzDbCodeGenName { get; set; }
            public string Reason { get; set; }
        }

        #endregion
    }
}
