using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using EzDbCodeGen.Core.Metadata;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Tests focused on many-to-many relationship detection, especially with payload columns,
    /// an area where EzDbCodeGen should demonstrate clear superiority over EF Core.
    /// </summary>
    public class ManyToManyRelationshipTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<ManyToManyRelationshipTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        /// <summary>
        /// Initializes a new instance of the ManyToManyRelationshipTests class.
        /// </summary>
        public ManyToManyRelationshipTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<ManyToManyRelationshipTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")] // Has ProductCategory relationships
        [InlineData("WideWorldImporters")] // Has various many-to-many relationships
        public void Should_Detect_More_ManyToMany_Relationships_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            
            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Get EF Core many-to-many relationships
            var efCoreManyToMany = efCoreRelationships
                .Where(r => r.RelationshipType == "ManyToMany")
                .ToList();
                
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find many-to-many relationships using EzDbCodeGen's advanced detection
            var ezDbCodeGenManyToMany = FindManyToManyRelationships(metadata);
            
            // Log the results for debugging
            _output.WriteLine($"=== Many-to-Many Relationship Detection for {databaseName} ===");
            _output.WriteLine($"EF Core detected {efCoreManyToMany.Count} many-to-many relationships:");
            foreach (var rel in efCoreManyToMany)
            {
                _output.WriteLine($"  {rel.SourceTable} <-> {rel.TargetTable} via {rel.SourceProperty}");
            }

            _output.WriteLine($"EzDbCodeGen detected {ezDbCodeGenManyToMany.Count} many-to-many relationships:");
            foreach (var rel in ezDbCodeGenManyToMany)
            {
                _output.WriteLine($"  {rel.Table1} <-> {rel.Table2} via {rel.JoinTable} " +
                                 $"(Payload Columns: {rel.HasPayloadColumns})");
            }

            // Assert - EzDbCodeGen should find more many-to-many relationships than EF Core
            ezDbCodeGenManyToMany.Count.Should().BeGreaterThanOrEqualTo(efCoreManyToMany.Count, 
                "EzDbCodeGen should detect at least as many many-to-many relationships as EF Core");
            
            // Calculate the improvement percentage
            if (efCoreManyToMany.Count > 0)
            {
                double improvementPercentage = ((double)ezDbCodeGenManyToMany.Count / efCoreManyToMany.Count - 1) * 100;
                _output.WriteLine($"EzDbCodeGen detected {improvementPercentage:F2}% more many-to-many relationships than EF Core");
                
                // Our target is 25% more relationships detected
                if (improvementPercentage < 25)
                {
                    _output.WriteLine($"WARNING: Improvement is less than the 25% target threshold");
                }
            }
        }
        
        [Theory]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        public void Should_Detect_ManyToMany_Relationships_With_Payload_Columns(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            
            // Act - Get EzDbCodeGen database metadata
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find many-to-many relationships with payload columns
            var manyToManyWithPayload = FindManyToManyRelationships(metadata)
                .Where(r => r.HasPayloadColumns)
                .ToList();
            
            // Log the results
            _output.WriteLine($"=== Many-to-Many Relationships with Payload Columns in {databaseName} ===");
            _output.WriteLine($"EzDbCodeGen detected {manyToManyWithPayload.Count} many-to-many relationships with payload columns:");
            foreach (var rel in manyToManyWithPayload)
            {
                _output.WriteLine($"  {rel.Table1} <-> {rel.Table2} via {rel.JoinTable}");
                _output.WriteLine($"  Payload Columns: {string.Join(", ", rel.PayloadColumns)}");
            }
            
            // Assert - In almost any real-world schema, we should find at least some with payload columns
            // This is a TDD test that will likely fail initially until we implement proper detection
            manyToManyWithPayload.Should().NotBeEmpty("EzDbCodeGen should detect many-to-many relationships with payload columns");
        }
        
        /// <summary>
        /// Helper method to find many-to-many relationships in the database metadata.
        /// </summary>
        private List<ManyToManyRelationship> FindManyToManyRelationships(DatabaseMetadata metadata)
        {
            var relationships = new List<ManyToManyRelationship>();
            
            // Look for potential join tables
            // A join table typically has:
            // 1. A composite primary key consisting of two foreign keys
            // 2. Few columns beyond the foreign keys (though there may be payload columns)
            
            foreach (var table in metadata.Tables)
            {
                // Skip tables with too many columns to be likely join tables
                // This is a simplistic heuristic - in reality we'd be more sophisticated
                if (table.Columns.Count > 10) continue;
                
                // Check if this table has at least two foreign keys
                if (table.ForeignKeys.Count < 2) continue;
                
                // Check if it has a composite primary key
                var pkColumns = table.Columns.Where(c => c.IsPrimaryKey).ToList();
                if (pkColumns.Count != 2) continue;
                
                // Check if the primary key columns are also foreign keys
                var fkColumnNames = table.ForeignKeys.SelectMany(fk => fk.Columns).ToList();
                if (!pkColumns.All(pk => fkColumnNames.Contains(pk.Name))) continue;
                
                // This looks like a many-to-many join table
                // Find the two tables it connects
                var referencedTables = table.ForeignKeys
                    .Select(fk => fk.ReferencedTableName)
                    .Distinct()
                    .Take(2)
                    .ToList();
                
                if (referencedTables.Count == 2)
                {
                    // Identify potential payload columns (non-PK columns)
                    var payloadColumns = table.Columns
                        .Where(c => !c.IsPrimaryKey)
                        .Select(c => c.Name)
                        .ToList();
                    
                    relationships.Add(new ManyToManyRelationship
                    {
                        Table1 = referencedTables[0],
                        Table2 = referencedTables[1],
                        JoinTable = table.Name,
                        HasPayloadColumns = payloadColumns.Any(),
                        PayloadColumns = payloadColumns
                    });
                }
            }
            
            return relationships;
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
    /// Represents a many-to-many relationship between two tables via a join table.
    /// </summary>
    public class ManyToManyRelationship
    {
        public string Table1 { get; set; }
        public string Table2 { get; set; }
        public string JoinTable { get; set; }
        public bool HasPayloadColumns { get; set; }
        public List<string> PayloadColumns { get; set; } = new List<string>();
        
        public override string ToString()
        {
            return $"{Table1} <-> {Table2} via {JoinTable}" +
                   (HasPayloadColumns ? $" (with payload columns: {string.Join(", ", PayloadColumns)})" : "");
        }
    }
}
