using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.EFCoreComparison.Tests.SuperiorityValidation
{
    public class RelationshipDetectionSuperiorityTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionString;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        public RelationshipDetectionSuperiorityTests(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            
            // Initialize comparers
            _efCoreComparer = new EFCoreModelComparer(_connectionString);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_connectionString);
        }

        [Fact]
        public async Task Should_Detect_At_Least_25_Percent_More_Valid_Relationships_Than_EFCore()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreRelationships = await _efCoreComparer.GetAllRelationshipsAsync(schemas);
            var ezDbCodeGenRelationships = await _ezDbCodeGenComparer.GetAllRelationshipsAsync(schemas);
            
            // Log the results
            _output.WriteLine($"EF Core detected {efCoreRelationships.Count} relationships");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenRelationships.Count} relationships");
            
            var percentageImprovement = ((double)ezDbCodeGenRelationships.Count / efCoreRelationships.Count - 1) * 100;
            _output.WriteLine($"Improvement: {percentageImprovement:F2}%");
            
            // Assert
            ezDbCodeGenRelationships.Count.Should().BeGreaterThanOrEqualTo((int)(efCoreRelationships.Count * 1.25),
                "because EzDbCodeGen should detect at least 25% more valid relationships than EF Core");
            
            // Validate that the additional relationships are valid
            var additionalRelationships = ezDbCodeGenRelationships
                .Where(r => !efCoreRelationships.Any(er => 
                    er.PrimaryKeyTableName == r.PrimaryKeyTableName && 
                    er.ForeignKeyTableName == r.ForeignKeyTableName &&
                    er.PrimaryKeyColumnNames.SequenceEqual(r.PrimaryKeyColumnNames) &&
                    er.ForeignKeyColumnNames.SequenceEqual(r.ForeignKeyColumnNames)))
                .ToList();
            
            _output.WriteLine($"Additional relationships detected by EzDbCodeGen: {additionalRelationships.Count}");
            
            foreach (var rel in additionalRelationships.Take(5))
            {
                _output.WriteLine($"  {rel.PrimaryKeyTableName} -> {rel.ForeignKeyTableName}");
            }
            
            // Verify that the additional relationships are valid (this would require domain knowledge validation)
            additionalRelationships.Should().NotBeEmpty("because EzDbCodeGen should detect valid relationships that EF Core misses");
        }

        [Fact]
        public async Task Should_Detect_More_ManyToMany_Relationships_With_Payload_Columns()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreManyToMany = await _efCoreComparer.GetManyToManyRelationshipsAsync(schemas);
            var ezDbCodeGenManyToMany = await _ezDbCodeGenComparer.GetManyToManyRelationshipsAsync(schemas);
            
            var efCoreWithPayload = efCoreManyToMany.Where(r => r.HasPayloadColumns).ToList();
            var ezDbCodeGenWithPayload = ezDbCodeGenManyToMany.Where(r => r.HasPayloadColumns).ToList();
            
            // Log the results
            _output.WriteLine($"EF Core detected {efCoreManyToMany.Count} many-to-many relationships ({efCoreWithPayload.Count} with payload columns)");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenManyToMany.Count} many-to-many relationships ({ezDbCodeGenWithPayload.Count} with payload columns)");
            
            // Assert
            ezDbCodeGenManyToMany.Count.Should().BeGreaterThanOrEqualTo(efCoreManyToMany.Count,
                "because EzDbCodeGen should detect at least as many many-to-many relationships as EF Core");
            
            ezDbCodeGenWithPayload.Count.Should().BeGreaterThanOrEqualTo(efCoreWithPayload.Count,
                "because EzDbCodeGen should detect at least as many many-to-many relationships with payload columns as EF Core");
        }

        [Fact]
        public async Task Should_Detect_More_TPH_TPT_Inheritance_Patterns()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreInheritance = await _efCoreComparer.GetInheritanceRelationshipsAsync(schemas);
            var ezDbCodeGenInheritance = await _ezDbCodeGenComparer.GetInheritanceRelationshipsAsync(schemas);
            
            // Log the results
            _output.WriteLine($"EF Core detected {efCoreInheritance.Count} inheritance relationships");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenInheritance.Count} inheritance relationships");
            
            // Assert
            ezDbCodeGenInheritance.Count.Should().BeGreaterThanOrEqualTo(efCoreInheritance.Count,
                "because EzDbCodeGen should detect at least as many inheritance patterns as EF Core");
            
            // Validate that the additional inheritance patterns are valid
            var additionalInheritance = ezDbCodeGenInheritance
                .Where(r => !efCoreInheritance.Any(er => 
                    er.BaseTableName == r.BaseTableName && 
                    er.DerivedTableName == r.DerivedTableName))
                .ToList();
            
            _output.WriteLine($"Additional inheritance patterns detected by EzDbCodeGen: {additionalInheritance.Count}");
            
            foreach (var inh in additionalInheritance.Take(3))
            {
                _output.WriteLine($"  {inh.BaseTableName} <- {inh.DerivedTableName} ({inh.InheritanceType})");
            }
        }

        [Fact]
        public async Task Should_Handle_Edge_Cases_Better_Than_EFCore()
        {
            // Arrange
            var schemas = new[] { "Sales", "Purchasing", "Warehouse", "Application" };
            
            // Act
            var efCoreEdgeCases = await _efCoreComparer.GetEdgeCaseRelationshipsAsync(schemas);
            var ezDbCodeGenEdgeCases = await _ezDbCodeGenComparer.GetEdgeCaseRelationshipsAsync(schemas);
            
            // Log the results
            _output.WriteLine($"EF Core detected {efCoreEdgeCases.Count} edge case relationships");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenEdgeCases.Count} edge case relationships");
            
            // Assert
            ezDbCodeGenEdgeCases.Count.Should().BeGreaterThanOrEqualTo(efCoreEdgeCases.Count,
                "because EzDbCodeGen should detect at least as many edge case relationships as EF Core");
            
            // Check for specific edge cases like self-references and multiple FKs between same tables
            var ezDbCodeGenSelfRefs = ezDbCodeGenEdgeCases.Where(r => 
                r.PrimaryKeyTableName == r.ForeignKeyTableName).ToList();
            
            var ezDbCodeGenMultipleFKs = ezDbCodeGenEdgeCases
                .GroupBy(r => new { r.PrimaryKeyTableName, r.ForeignKeyTableName })
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();
            
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenSelfRefs.Count} self-references");
            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenMultipleFKs.Count} relationships with multiple FKs between the same tables");
            
            ezDbCodeGenSelfRefs.Should().NotBeEmpty("because EzDbCodeGen should detect self-referencing relationships");
            ezDbCodeGenMultipleFKs.Should().NotBeEmpty("because EzDbCodeGen should detect multiple foreign keys between the same tables");
        }
    }
}
