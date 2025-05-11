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

namespace StandaloneEFCoreComparison
{
    public class EFCoreComparisonTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<EFCoreComparisonTests> _logger;
        private readonly EFCoreModelComparer _modelComparer;

        // Connection string for test database - should be configurable
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        // List of sample databases to test
        private static readonly string[] SampleDatabases = new[]
        {
            "Northwind",
            "AdventureWorks",
            "WideWorldImporters",
            "ContosoDataWarehouse"
        };

        public EFCoreComparisonTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<EFCoreComparisonTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _modelComparer = new EFCoreModelComparer(_logger);
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Analyze_EFCore_Relationships(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var relationships = _modelComparer.AnalyzeEFCoreRelationships(dbContext);
            stopwatch.Stop();

            // Assert
            relationships.Should().NotBeEmpty();
            _output.WriteLine($"Found {relationships.Count} relationships in EF Core model for {databaseName}");
            _output.WriteLine($"EF Core relationship analysis took {stopwatch.ElapsedMilliseconds}ms");

            // Output details of relationships (limit to first 20 for readability)
            foreach (var relationship in relationships.Take(20))
            {
                _output.WriteLine($"Relationship: {relationship}");
            }
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Analyze_EFCore_Column_Types(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var columnTypes = _modelComparer.AnalyzeEFCoreColumnTypes(dbContext);
            stopwatch.Stop();

            // Assert
            columnTypes.Should().NotBeEmpty();
            _output.WriteLine($"Found {columnTypes.Count} columns in EF Core model for {databaseName}");
            _output.WriteLine($"EF Core column type analysis took {stopwatch.ElapsedMilliseconds}ms");

            // Output details of column types (limit to first 20 for readability)
            foreach (var (column, type) in columnTypes.Take(20))
            {
                _output.WriteLine($"Column: {column}, Type: {type}");
            }
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        [InlineData("WideWorldImporters")]
        [InlineData("ContosoDataWarehouse")]
        public void Should_Compare_EFCore_And_EzDbCodeGen_Performance(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var dbContext = CreateDbContext(databaseName, connectionString);

            // Act
            var metrics = _modelComparer.ComparePerformance(dbContext);

            // Assert
            _output.WriteLine($"Performance comparison for {databaseName}:");
            _output.WriteLine(metrics.ToString());
            
            // We're not making assertions about specific performance metrics
            // as they will vary by environment, but we want to ensure the test runs
        }

        private DbContext CreateDbContext(string databaseName, string connectionString)
        {
            switch (databaseName)
            {
                case "Northwind":
                    return new NorthwindContext(connectionString);
                case "AdventureWorks":
                    return new AdventureWorksContext(connectionString);
                case "WideWorldImporters":
                    return new WideWorldImportersContext(connectionString);
                case "ContosoDataWarehouse":
                    return new ContosoDataWarehouseContext(connectionString);
                default:
                    throw new ArgumentException($"Unknown database: {databaseName}", nameof(databaseName));
            }
        }

        // Sample DbContext classes for each database
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

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // For Northwind database
                modelBuilder.Entity<Customer>()
                    .ToTable("Customers")
                    .HasKey(c => c.CustomerID);

                modelBuilder.Entity<Order>()
                    .ToTable("Orders")
                    .HasKey(o => o.OrderID);

                modelBuilder.Entity<Order>()
                    .HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerID);
            }

            public DbSet<Customer> Customers { get; set; } = null!;
            public DbSet<Order> Orders { get; set; } = null!;
        }

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
                // For AdventureWorks database
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

            public DbSet<Person> People { get; set; } = null!;
            public DbSet<Employee> Employees { get; set; } = null!;
        }

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

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // For WideWorldImporters database
                // This is a simplified model for testing purposes
                modelBuilder.Entity<Customer>()
                    .ToTable("Customers", "Sales")
                    .HasKey(c => c.CustomerID);

                modelBuilder.Entity<Order>()
                    .ToTable("Orders", "Sales")
                    .HasKey(o => o.OrderID);

                modelBuilder.Entity<Order>()
                    .HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerID);
            }

            public DbSet<Customer> Customers { get; set; } = null!;
            public DbSet<Order> Orders { get; set; } = null!;
        }

        public class ContosoDataWarehouseContext : DbContext
        {
            private readonly string _connectionString;

            public ContosoDataWarehouseContext(string connectionString)
            {
                _connectionString = connectionString;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // For ContosoDataWarehouse database
                // This is a simplified model for testing purposes
                modelBuilder.Entity<DimCustomer>()
                    .ToTable("DimCustomer")
                    .HasKey(c => c.CustomerKey);

                modelBuilder.Entity<FactSales>()
                    .ToTable("FactSales")
                    .HasKey(s => s.SalesKey);

                modelBuilder.Entity<FactSales>()
                    .HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.CustomerKey);
            }

            public DbSet<DimCustomer> Customers { get; set; } = null!;
            public DbSet<FactSales> Sales { get; set; } = null!;
        }

        // Sample entity classes for Northwind
        public class Customer
        {
            public string CustomerID { get; set; } = string.Empty;
            public string CompanyName { get; set; } = string.Empty;
            public string ContactName { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            
            public ICollection<Order> Orders { get; set; } = new List<Order>();
        }
        
        public class Order
        {
            public int OrderID { get; set; }
            public string CustomerID { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal? Freight { get; set; }
            
            public Customer Customer { get; set; } = null!;
        }

        // Sample entity classes for AdventureWorks
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

        // Sample entity classes for ContosoDataWarehouse
        public class DimCustomer
        {
            public int CustomerKey { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerType { get; set; } = string.Empty;
            
            public ICollection<FactSales> Sales { get; set; } = new List<FactSales>();
        }
        
        public class FactSales
        {
            public int SalesKey { get; set; }
            public int CustomerKey { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal SalesAmount { get; set; }
            
            public DimCustomer Customer { get; set; } = null!;
        }
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
