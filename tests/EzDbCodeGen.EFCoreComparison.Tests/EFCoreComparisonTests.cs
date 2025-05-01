using System;
using System.Collections.Generic;
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
    public class EFCoreComparisonTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<EFCoreComparisonTests> _logger;
        private readonly EFCoreModelComparer _modelComparer;

        // Connection string for test database - should be configurable
        private const string TestConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=AdventureWorks;Trusted_Connection=True;TrustServerCertificate=True;";

        public EFCoreComparisonTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<EFCoreComparisonTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _modelComparer = new EFCoreModelComparer(_logger);
        }

        [Fact(Skip = "Requires database connection")]
        public void Should_Analyze_EFCore_Relationships()
        {
            // Arrange
            var dbContext = new AdventureWorksContext(TestConnectionString);

            // Act
            var relationships = _modelComparer.AnalyzeEFCoreRelationships(dbContext);

            // Assert
            relationships.Should().NotBeEmpty();
            _output.WriteLine($"Found {relationships.Count} relationships in EF Core model");

            // Output details of relationships
            foreach (var relationship in relationships)
            {
                _output.WriteLine($"Relationship: {relationship}");
            }
        }

        [Fact(Skip = "Requires database connection")]
        public void Should_Analyze_EFCore_Column_Types()
        {
            // Arrange
            var dbContext = new AdventureWorksContext(TestConnectionString);

            // Act
            var columnTypes = _modelComparer.AnalyzeEFCoreColumnTypes(dbContext);

            // Assert
            columnTypes.Should().NotBeEmpty();
            _output.WriteLine($"Found {columnTypes.Count} columns in EF Core model");

            // Output details of column types
            foreach (var (column, type) in columnTypes)
            {
                _output.WriteLine($"Column: {column}, Type: {type}");
            }
        }
    }

    /// <summary>
    /// Sample DbContext for AdventureWorks database
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This would be populated by EF Core's reverse engineering process
            // For testing purposes, we'll scaffold this manually or use EF Core's scaffolding tools
            
            // Example of manual configuration:
            modelBuilder.Entity<Person>()
                .ToTable("Person", "Person")
                .HasKey(p => p.BusinessEntityID);
            
            modelBuilder.Entity<Employee>()
                .ToTable("Employee", "HumanResources")
                .HasKey(e => e.BusinessEntityID);
            
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Person)
                .WithOne()
                .HasForeignKey<Employee>(e => e.BusinessEntityID);
        }
        
        // Sample entity classes for AdventureWorks
        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
    }
    
    // Sample entity classes
    public class Person
    {
        public int BusinessEntityID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    
    public class Employee
    {
        public int BusinessEntityID { get; set; }
        public DateTime HireDate { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        
        public Person Person { get; set; } = null!;
    }

    /// <summary>
    /// Test logger provider for xUnit
    /// </summary>
    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        public TestLoggerProvider(ITestOutputHelper output)
        {
            _output = output;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_output);
        }

        public void Dispose() { }
    }

    /// <summary>
    /// Logger that outputs to xUnit's test output
    /// </summary>
    public class TestLogger : ILogger
    {
        private readonly ITestOutputHelper _output;

        public TestLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _output.WriteLine($"[{logLevel}] {formatter(state, exception)}");
            if (exception != null)
            {
                _output.WriteLine($"Exception: {exception}");
            }
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }
}
