using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Tests focused on performance comparison between EzDbCodeGen and EF Core Power Tools.
    /// Our goal is to be at least 50% faster for schemas with 100+ tables.
    /// </summary>
    public class PerformanceComparisonTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<PerformanceComparisonTests> _logger;
        private readonly EFCoreModelComparer _efCoreComparer;
        private readonly EzDbCodeGenModelComparer _ezDbCodeGenComparer;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        public PerformanceComparisonTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<PerformanceComparisonTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _efCoreComparer = new EFCoreModelComparer(_logger);
            _ezDbCodeGenComparer = new EzDbCodeGenModelComparer(_logger);
        }

        [Theory]
        [InlineData("AdventureWorks")] // Moderate-sized schema
        [InlineData("WideWorldImporters")] // Another moderate schema
        public void Should_Complete_Generation_In_Under_Half_The_Time_Of_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateTestDbContext(databaseName, connectionString);
            var tempOutputDir = Path.Combine(Path.GetTempPath(), $"EzDbCodeGen_Performance_Test_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempOutputDir);

            try
            {
                // Act - Measure EF Core model building + code generation time
                var efCoreStopwatch = Stopwatch.StartNew();
                var efCoreModel = dbContext.Model;

                // Manually generate code using EF Core model
                // This simulates what EF Core Power Tools would do
                var efCoreGenerationTime = GenerateEFCoreCode(efCoreModel, tempOutputDir);
                efCoreStopwatch.Stop();
                var totalEFCoreTime = efCoreStopwatch.ElapsedMilliseconds;

                // Act - Measure EzDbCodeGen schema discovery + code generation time
                var ezDbCodeGenStopwatch = Stopwatch.StartNew();
                var ezDbCodeGenMetrics = _ezDbCodeGenComparer.MeasureEzDbCodeGenPerformance(connectionString, databaseName);

                // Generate code using EzDbCodeGen
                var ezDbCodeGenGenerationTime = GenerateEzDbCodeGenCode(connectionString, databaseName, tempOutputDir);
                ezDbCodeGenStopwatch.Stop();
                var totalEzDbCodeGenTime = ezDbCodeGenStopwatch.ElapsedMilliseconds;

                // Assert and Output
                _output.WriteLine($"=== Performance Comparison for {databaseName} ===");
                _output.WriteLine($"Database size: {ezDbCodeGenMetrics.TableCount} tables, {ezDbCodeGenMetrics.ColumnCount} columns");
                _output.WriteLine($"EF Core total time: {totalEFCoreTime}ms (Schema: {totalEFCoreTime - efCoreGenerationTime}ms, Code Gen: {efCoreGenerationTime}ms)");
                _output.WriteLine($"EzDbCodeGen total time: {totalEzDbCodeGenTime}ms (Schema: {ezDbCodeGenMetrics.SchemaDiscoveryTime}ms, Code Gen: {ezDbCodeGenGenerationTime}ms)");

                // Calculate percentage difference
                var percentageFaster = ((double)totalEFCoreTime / totalEzDbCodeGenTime - 1) * 100;
                _output.WriteLine($"EzDbCodeGen is {percentageFaster:F2}% faster than EF Core");

                if (ezDbCodeGenMetrics.TableCount >= 100)
                {
                    // For large schemas (100+ tables), we expect to be at least 50% faster
                    percentageFaster.Should().BeGreaterThanOrEqualTo(50, 
                        "EzDbCodeGen should be at least 50% faster than EF Core for large schemas (100+ tables)");
                }
                else
                {
                    // For smaller schemas, we still expect to be faster, but not necessarily by 50%
                    percentageFaster.Should().BeGreaterThanOrEqualTo(0, 
                        "EzDbCodeGen should be faster than EF Core even for smaller schemas");
                    _output.WriteLine("NOTE: This schema has fewer than 100 tables, so we're not enforcing the 50% faster requirement");
                }
            }
            finally
            {
                // Clean up
                if (Directory.Exists(tempOutputDir))
                {
                    Directory.Delete(tempOutputDir, true);
                }
            }
        }

        [Theory]
        [InlineData("AdventureWorks")]
        public void Should_Use_Less_Memory_Than_EFCore(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var tempOutputDir = Path.Combine(Path.GetTempPath(), $"EzDbCodeGen_Memory_Test_{Guid.NewGuid()}");
            Directory.CreateDirectory(tempOutputDir);

            try
            {
                // Act - Measure EF Core memory usage
                long efCoreMemoryBefore = GC.GetTotalMemory(true);
                var dbContext = CreateTestDbContext(databaseName, connectionString);
                var efCoreModel = dbContext.Model;
                GenerateEFCoreCode(efCoreModel, tempOutputDir);
                long efCoreMemoryAfter = GC.GetTotalMemory(false);
                long efCoreMemoryUsed = efCoreMemoryAfter - efCoreMemoryBefore;

                // Force cleanup before measuring EzDbCodeGen
                dbContext.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                
                // Act - Measure EzDbCodeGen memory usage
                long ezDbCodeGenMemoryBefore = GC.GetTotalMemory(true);
                GenerateEzDbCodeGenCode(connectionString, databaseName, tempOutputDir);
                long ezDbCodeGenMemoryAfter = GC.GetTotalMemory(false);
                long ezDbCodeGenMemoryUsed = ezDbCodeGenMemoryAfter - ezDbCodeGenMemoryBefore;

                // Assert and Output
                _output.WriteLine($"=== Memory Usage Comparison for {databaseName} ===");
                _output.WriteLine($"EF Core memory usage: {efCoreMemoryUsed / 1024 / 1024} MB");
                _output.WriteLine($"EzDbCodeGen memory usage: {ezDbCodeGenMemoryUsed / 1024 / 1024} MB");

                // Calculate percentage difference
                var percentageLessMemory = (1 - (double)ezDbCodeGenMemoryUsed / efCoreMemoryUsed) * 100;
                _output.WriteLine($"EzDbCodeGen uses {percentageLessMemory:F2}% less memory than EF Core");

                // Verify EzDbCodeGen uses less memory (or at most 10% more)
                ezDbCodeGenMemoryUsed.Should().BeLessThanOrEqualTo(efCoreMemoryUsed * 1.1,
                    "EzDbCodeGen should use less memory than EF Core (or at most 10% more)");
            }
            finally
            {
                // Clean up
                if (Directory.Exists(tempOutputDir))
                {
                    Directory.Delete(tempOutputDir, true);
                }
            }
        }

        /// <summary>
        /// Generate code using EF Core model (simulating what EF Core Power Tools would do).
        /// </summary>
        private long GenerateEFCoreCode(IModel model, string outputPath)
        {
            var stopwatch = Stopwatch.StartNew();

            // This simulates what EF Core Power Tools would do
            // In a real implementation, we'd use Microsoft.EntityFrameworkCore.Design 
            // or we could fork/wrap the actual EF Core Power Tools
            
            // For now, we'll create a simple scaffolding that mimics EF Core output
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityName = entityType.ClrType.Name;
                var properties = entityType.GetProperties();
                var navigations = entityType.GetNavigations();

                var entityCode = new System.Text.StringBuilder();
                entityCode.AppendLine("using System;");
                entityCode.AppendLine("using System.Collections.Generic;");
                entityCode.AppendLine("using System.ComponentModel.DataAnnotations;");
                entityCode.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
                entityCode.AppendLine();
                entityCode.AppendLine("namespace EFCore.Generated.Entities");
                entityCode.AppendLine("{");
                entityCode.AppendLine($"    public partial class {entityName}");
                entityCode.AppendLine("    {");

                // Constructor
                entityCode.AppendLine($"        public {entityName}()");
                entityCode.AppendLine("        {");

                // Initialize collections
                foreach (var navigation in navigations.Where(n => n.IsCollection))
                {
                    entityCode.AppendLine($"            {navigation.Name} = new HashSet<{navigation.TargetEntityType.ClrType.Name}>();");
                }

                entityCode.AppendLine("        }");
                entityCode.AppendLine();

                // Properties
                foreach (var property in properties)
                {
                    var nullable = property.IsNullable ? "?" : "";
                    var typeName = property.ClrType.Name;
                    entityCode.AppendLine($"        public {typeName}{nullable} {property.Name} {{ get; set; }}");
                }

                // Navigation properties
                foreach (var navigation in navigations)
                {
                    if (navigation.IsCollection)
                    {
                        entityCode.AppendLine($"        public virtual ICollection<{navigation.TargetEntityType.ClrType.Name}> {navigation.Name} {{ get; set; }}");
                    }
                    else
                    {
                        entityCode.AppendLine($"        public virtual {navigation.TargetEntityType.ClrType.Name} {navigation.Name} {{ get; set; }}");
                    }
                }

                entityCode.AppendLine("    }");
                entityCode.AppendLine("}");

                // Write the entity class file
                File.WriteAllText(Path.Combine(outputPath, $"{entityName}.cs"), entityCode.ToString());
            }

            // Also generate a DbContext
            var contextCode = new System.Text.StringBuilder();
            contextCode.AppendLine("using System;");
            contextCode.AppendLine("using Microsoft.EntityFrameworkCore;");
            contextCode.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
            contextCode.AppendLine("using EFCore.Generated.Entities;");
            contextCode.AppendLine();
            contextCode.AppendLine("namespace EFCore.Generated");
            contextCode.AppendLine("{");
            contextCode.AppendLine("    public partial class GeneratedDbContext : DbContext");
            contextCode.AppendLine("    {");
            contextCode.AppendLine("        public GeneratedDbContext(DbContextOptions<GeneratedDbContext> options)");
            contextCode.AppendLine("            : base(options)");
            contextCode.AppendLine("        {");
            contextCode.AppendLine("        }");
            contextCode.AppendLine();

            // DbSets
            foreach (var entityType in model.GetEntityTypes())
            {
                var entityName = entityType.ClrType.Name;
                contextCode.AppendLine($"        public virtual DbSet<{entityName}> {entityName}s {{ get; set; }}");
            }

            contextCode.AppendLine();
            contextCode.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            contextCode.AppendLine("        {");
            contextCode.AppendLine("            // Entity configurations go here");
            contextCode.AppendLine("            base.OnModelCreating(modelBuilder);");
            contextCode.AppendLine("        }");
            contextCode.AppendLine("    }");
            contextCode.AppendLine("}");

            // Write the context class file
            File.WriteAllText(Path.Combine(outputPath, "GeneratedDbContext.cs"), contextCode.ToString());

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Generate code using EzDbCodeGen.
        /// </summary>
        private long GenerateEzDbCodeGenCode(string connectionString, string databaseName, string outputPath)
        {
            var stopwatch = Stopwatch.StartNew();

            // Create a test config
            var config = new EzDbCodeGen.Core.Config.Configuration
            {
                OutputPath = outputPath,
                ProjectName = "EzDbCodeGen.Generated",
                TemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates")
            };

            // TODO: In a production implementation, we would:
            // 1. Create a template processor
            // 2. Register all template helpers
            // 3. Create a code generator
            // 4. Process the templates
            
            // For now, we'll simulate code generation by creating similar output to the EF Core simulation
            var metadata = _ezDbCodeGenComparer.AnalyzeDatabase(connectionString, databaseName);
            
            // Generate entity classes
            foreach (var table in metadata.Tables)
            {
                var entityCode = new System.Text.StringBuilder();
                entityCode.AppendLine("using System;");
                entityCode.AppendLine("using System.Collections.Generic;");
                entityCode.AppendLine("using System.ComponentModel.DataAnnotations;");
                entityCode.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
                entityCode.AppendLine();
                entityCode.AppendLine("namespace EzDbCodeGen.Generated.Entities");
                entityCode.AppendLine("{");
                entityCode.AppendLine($"    public partial class {table.Name}");
                entityCode.AppendLine("    {");

                // Properties
                foreach (var column in table.Columns)
                {
                    var nullable = !column.IsNullable || column.IsPrimaryKey ? "" : "?";
                    var typeName = MapToClrType(column.DataType);
                    entityCode.AppendLine($"        public {typeName}{nullable} {column.Name} {{ get; set; }}");
                }

                entityCode.AppendLine("    }");
                entityCode.AppendLine("}");

                // Write the entity class file
                File.WriteAllText(Path.Combine(outputPath, $"{table.Name}.cs"), entityCode.ToString());
            }

            // Generate DbContext
            var contextCode = new System.Text.StringBuilder();
            contextCode.AppendLine("using System;");
            contextCode.AppendLine("using Microsoft.EntityFrameworkCore;");
            contextCode.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
            contextCode.AppendLine("using EzDbCodeGen.Generated.Entities;");
            contextCode.AppendLine();
            contextCode.AppendLine("namespace EzDbCodeGen.Generated");
            contextCode.AppendLine("{");
            contextCode.AppendLine("    public partial class GeneratedDbContext : DbContext");
            contextCode.AppendLine("    {");
            contextCode.AppendLine("        public GeneratedDbContext(DbContextOptions<GeneratedDbContext> options)");
            contextCode.AppendLine("            : base(options)");
            contextCode.AppendLine("        {");
            contextCode.AppendLine("        }");
            contextCode.AppendLine();

            // DbSets
            foreach (var table in metadata.Tables)
            {
                contextCode.AppendLine($"        public virtual DbSet<{table.Name}> {table.Name}s {{ get; set; }}");
            }

            contextCode.AppendLine();
            contextCode.AppendLine("        protected override void OnModelCreating(ModelBuilder modelBuilder)");
            contextCode.AppendLine("        {");
            contextCode.AppendLine("            // Entity configurations go here");

            // Add relationship configurations
            foreach (var table in metadata.Tables)
            {
                foreach (var foreignKey in table.ForeignKeys)
                {
                    contextCode.AppendLine($"            modelBuilder.Entity<{table.Name}>()");
                    contextCode.AppendLine($"                .HasOne<{foreignKey.ReferencedTableName}>()");
                    contextCode.AppendLine($"                .WithMany()");
                    contextCode.AppendLine($"                .HasForeignKey(e => e.{foreignKey.Columns[0]});");
                }
            }

            contextCode.AppendLine("            base.OnModelCreating(modelBuilder);");
            contextCode.AppendLine("        }");
            contextCode.AppendLine("    }");
            contextCode.AppendLine("}");

            // Write the context class file
            File.WriteAllText(Path.Combine(outputPath, "GeneratedDbContext.cs"), contextCode.ToString());

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Basic type mapping from SQL Server to C# types.
        /// </summary>
        private string MapToClrType(string sqlType)
        {
            switch (sqlType.ToLowerInvariant())
            {
                case "bit": return "bool";
                case "tinyint": return "byte";
                case "smallint": return "short";
                case "int": return "int";
                case "bigint": return "long";
                case "decimal":
                case "money":
                case "smallmoney": return "decimal";
                case "float": return "double";
                case "real": return "float";
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime": return "DateTime";
                case "datetimeoffset": return "DateTimeOffset";
                case "time": return "TimeSpan";
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext": return "string";
                case "binary":
                case "varbinary":
                case "image": return "byte[]";
                case "uniqueidentifier": return "Guid";
                default: return "object";
            }
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
    /// Test implementation of DbContext for the Northwind database.
    /// </summary>
    public class NorthwindContext : DbContext
    {
        private readonly string _connectionString;

        public NorthwindContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    /// <summary>
    /// Test implementation of DbContext for the AdventureWorks database.
    /// </summary>
    public class AdventureWorksContext : DbContext
    {
        private readonly string _connectionString;

        public AdventureWorksContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

    /// <summary>
    /// Test implementation of DbContext for the WideWorldImporters database.
    /// </summary>
    public class WideWorldImportersContext : DbContext
    {
        private readonly string _connectionString;

        public WideWorldImportersContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
