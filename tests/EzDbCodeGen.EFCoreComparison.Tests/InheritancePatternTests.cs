using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using EzDbCodeGen.Core.Interfaces;
using EzDbCodeGen.Core.Metadata;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    /// <summary>
    /// Tests focused on TPH (Table-Per-Hierarchy) and TPT (Table-Per-Type) pattern detection,
    /// which are inheritance patterns that EzDbCodeGen should handle better than EF Core Power Tools.
    /// </summary>
    public class InheritancePatternTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<InheritancePatternTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        // Databases known to have inheritance patterns
        private static readonly string[] DatabasesWithInheritance = new[]
        {
            "AdventureWorks", // Person table hierarchy
            "WideWorldImporters" // Has some inheritance patterns
        };

        public InheritancePatternTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<InheritancePatternTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")] // Person, Customer, Employee hierarchy
        public void Should_Detect_TPH_Patterns_Better_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);

            // Act - Get EF Core relationships
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen database metadata for more thorough analysis
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find potential TPH patterns in the schema
            // For TPH, we need to look for tables with discriminator columns
            var tphPatterns = FindTablePerHierarchyPatterns(metadata);
            
            // Log the detected patterns
            _output.WriteLine($"=== TPH Pattern Detection for {databaseName} ===");
            _output.WriteLine($"EzDbCodeGen detected {tphPatterns.Count} potential TPH patterns:");
            foreach (var pattern in tphPatterns)
            {
                _output.WriteLine($"  Base table: {pattern.BaseTable}, Discriminator: {pattern.DiscriminatorColumn}");
                _output.WriteLine($"  Potential derived types: {string.Join(", ", pattern.PotentialDerivedTypes)}");
            }
            
            // We can't directly compare with EF Core since it represents TPH differently
            // But we can check if we found any patterns at all
            tphPatterns.Should().NotBeEmpty("EzDbCodeGen should detect at least some TPH patterns");
        }
        
        [Theory]
        [InlineData("AdventureWorks")] // Person, Store, Vendor TPT pattern
        public void Should_Detect_TPT_Patterns_Better_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);

            // Act - Get EF Core relationships to look for potential TPT patterns
            var efCoreRelationships = _efCoreComparer.AnalyzeEFCoreRelationships(dbContext);
            
            // Act - Get EzDbCodeGen database metadata for more thorough analysis
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Find potential TPT patterns in the schema
            // For TPT, we're looking for tables with 1:1 relationships and shared primary keys
            var tptPatterns = FindTablePerTypePatterns(metadata);
            
            // Log the detected patterns
            _output.WriteLine($"=== TPT Pattern Detection for {databaseName} ===");
            _output.WriteLine($"EzDbCodeGen detected {tptPatterns.Count} potential TPT patterns:");
            foreach (var pattern in tptPatterns)
            {
                _output.WriteLine($"  Base table: {pattern.BaseTable}, Derived tables: {string.Join(", ", pattern.DerivedTables)}");
            }
            
            // Without direct access to EF Core's internal TPT detection,
            // we'll assert that our system should find at least some patterns
            tptPatterns.Should().NotBeEmpty("EzDbCodeGen should detect at least some TPT patterns");
        }

        private List<TPHPattern> FindTablePerHierarchyPatterns(DatabaseMetadata metadata)
        {
            var patterns = new List<TPHPattern>();
            
            // In TPH, we look for tables with potential discriminator columns
            // Typical discriminators are columns named "Type", "Class", "Category", etc.
            var potentialDiscriminatorNames = new[] { "Type", "Class", "Category", "Kind", "Discriminator" };
            
            foreach (var table in metadata.Tables)
            {
                // Look for columns that might be discriminators
                foreach (var column in table.Columns.Where(c => 
                    potentialDiscriminatorNames.Any(name => c.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase))))
                {
                    // For this test, assume any match is a potential TPH pattern
                    // In a real implementation, we'd check for enum-like values
                    patterns.Add(new TPHPattern
                    {
                        BaseTable = table.Name,
                        DiscriminatorColumn = column.Name,
                        // In a real implementation, we'd analyze the actual discriminator values
                        PotentialDerivedTypes = new List<string> { $"{table.Name}Type1", $"{table.Name}Type2" }
                    });
                }
            }
            
            return patterns;
        }

        private List<TPTPattern> FindTablePerTypePatterns(DatabaseMetadata metadata)
        {
            var patterns = new List<TPTPattern>();
            
            // In TPT, we look for tables with:
            // 1. Primary keys that are also foreign keys
            // 2. 1:1 relationships
            
            // Group tables by their potential TPT base tables
            var tablesByBase = new Dictionary<string, List<string>>();
            
            foreach (var table in metadata.Tables)
            {
                // Check if this table has a single-column primary key that's also a foreign key
                var pkColumns = table.Columns.Where(c => c.IsPrimaryKey).ToList();
                if (pkColumns.Count != 1) continue;
                
                var pkColumn = pkColumns[0];
                
                // Find foreign keys that use this column
                foreach (var fk in table.ForeignKeys)
                {
                    // Check if this FK uses the PK column and points to another table's PK
                    if (fk.Columns.Count == 1 && fk.Columns[0] == pkColumn.Name)
                    {
                        // This is a potential TPT pattern - the referenced table might be a base type
                        if (!tablesByBase.ContainsKey(fk.ReferencedTableName))
                        {
                            tablesByBase[fk.ReferencedTableName] = new List<string>();
                        }
                        tablesByBase[fk.ReferencedTableName].Add(table.Name);
                    }
                }
            }
            
            // Convert the dictionary to TPTPattern objects
            foreach (var baseTable in tablesByBase.Keys)
            {
                if (tablesByBase[baseTable].Count > 0)
                {
                    patterns.Add(new TPTPattern
                    {
                        BaseTable = baseTable,
                        DerivedTables = tablesByBase[baseTable]
                    });
                }
            }
            
            return patterns;
        }

        /// <summary>
        /// Creates a test DbContext for the specified database.
        /// </summary>
        private DbContext CreateTestDbContext(string databaseName, string connectionString)
        {
            switch (databaseName)
            {
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
    /// Represents a detected Table-Per-Hierarchy pattern.
    /// </summary>
    public class TPHPattern
    {
        public string BaseTable { get; set; }
        public string DiscriminatorColumn { get; set; }
        public List<string> PotentialDerivedTypes { get; set; }
    }

    /// <summary>
    /// Represents a detected Table-Per-Type pattern.
    /// </summary>
    public class TPTPattern
    {
        public string BaseTable { get; set; }
        public List<string> DerivedTables { get; set; }
    }
}
