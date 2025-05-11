using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using EzDbCodeGen.Core;
using EzDbCodeGen.Core.Config;
using EzDbCodeGen.Core.Interfaces;
using EzDbCodeGen.Core.Metadata;
using EzDbCodeGen.Cli;

namespace EzDbCodeGen.EFCoreComparison.Tests
{
    public class CodeGenerationComparisonTests
    {
        private readonly ITestOutputHelper _output;
        private readonly ILogger<CodeGenerationComparisonTests> _logger;
        private readonly string _tempOutputDir;

        // Connection string for test database
        private const string BaseConnectionString = "Server=localhost;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";

        public CodeGenerationComparisonTests(ITestOutputHelper output)
        {
            _output = output;
            _logger = new Logger<CodeGenerationComparisonTests>(new LoggerFactory(new[] { new TestLoggerProvider(output) }));
            _tempOutputDir = Path.Combine(Path.GetTempPath(), "EzDbCodeGen_Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempOutputDir);
        }

        [Theory]
        [InlineData("Northwind")]
        [InlineData("AdventureWorks")]
        public void Should_Compare_Code_Generation_Performance(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var ezDbCodeGenOutputDir = Path.Combine(_tempOutputDir, "EzDbCodeGen", databaseName);
            var efCoreOutputDir = Path.Combine(_tempOutputDir, "EFCore", databaseName);
            
            Directory.CreateDirectory(ezDbCodeGenOutputDir);
            Directory.CreateDirectory(efCoreOutputDir);

            // Act & Assert - EzDbCodeGen Code Generation
            var ezDbCodeGenStopwatch = Stopwatch.StartNew();
            GenerateCodeWithEzDbCodeGen(connectionString, databaseName, ezDbCodeGenOutputDir);
            ezDbCodeGenStopwatch.Stop();
            
            // Act & Assert - EF Core Code Generation (using dotnet ef command)
            var efCoreStopwatch = Stopwatch.StartNew();
            GenerateCodeWithEFCore(connectionString, databaseName, efCoreOutputDir);
            efCoreStopwatch.Stop();

            // Compare results
            _output.WriteLine($"=== Code Generation Performance Comparison for {databaseName} ===");
            _output.WriteLine($"EzDbCodeGen code generation: {ezDbCodeGenStopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"EF Core code generation: {efCoreStopwatch.ElapsedMilliseconds}ms");
            _output.WriteLine($"Performance difference: {(efCoreStopwatch.ElapsedMilliseconds - ezDbCodeGenStopwatch.ElapsedMilliseconds)}ms");
            
            var percentageDiff = Math.Round(((double)ezDbCodeGenStopwatch.ElapsedMilliseconds / efCoreStopwatch.ElapsedMilliseconds - 1) * 100, 2);
            _output.WriteLine($"EzDbCodeGen is {(percentageDiff < 0 ? Math.Abs(percentageDiff) + "% faster" : percentageDiff + "% slower")} than EF Core");

            // Compare file counts
            var ezDbCodeGenFiles = Directory.GetFiles(ezDbCodeGenOutputDir, "*.cs", SearchOption.AllDirectories);
            var efCoreFiles = Directory.GetFiles(efCoreOutputDir, "*.cs", SearchOption.AllDirectories);
            
            _output.WriteLine($"EzDbCodeGen generated {ezDbCodeGenFiles.Length} files");
            _output.WriteLine($"EF Core generated {efCoreFiles.Length} files");

            // Compare total lines of code
            var ezDbCodeGenLinesOfCode = CountLinesOfCode(ezDbCodeGenFiles);
            var efCoreLinesOfCode = CountLinesOfCode(efCoreFiles);
            
            _output.WriteLine($"EzDbCodeGen generated {ezDbCodeGenLinesOfCode} lines of code");
            _output.WriteLine($"EF Core generated {efCoreLinesOfCode} lines of code");
            _output.WriteLine($"Line count ratio: {Math.Round((double)ezDbCodeGenLinesOfCode / efCoreLinesOfCode, 2)}");
        }

        [Theory]
        [InlineData("Northwind")]
        public void Should_Compare_Code_Quality_Metrics(string databaseName)
        {
            // Arrange
            var connectionString = $"{BaseConnectionString}Database={databaseName};";
            var ezDbCodeGenOutputDir = Path.Combine(_tempOutputDir, "EzDbCodeGen", databaseName);
            var efCoreOutputDir = Path.Combine(_tempOutputDir, "EFCore", databaseName);
            
            Directory.CreateDirectory(ezDbCodeGenOutputDir);
            Directory.CreateDirectory(efCoreOutputDir);

            // Generate code with both tools
            GenerateCodeWithEzDbCodeGen(connectionString, databaseName, ezDbCodeGenOutputDir);
            GenerateCodeWithEFCore(connectionString, databaseName, efCoreOutputDir);

            // Compare code quality metrics
            _output.WriteLine($"=== Code Quality Metrics Comparison for {databaseName} ===");

            // 1. Compare presence of XML documentation
            var ezDbCodeGenXmlDocCount = CountFilesWithXmlDocumentation(ezDbCodeGenOutputDir);
            var efCoreXmlDocCount = CountFilesWithXmlDocumentation(efCoreOutputDir);
            
            _output.WriteLine($"Files with XML documentation:");
            _output.WriteLine($"  EzDbCodeGen: {ezDbCodeGenXmlDocCount}");
            _output.WriteLine($"  EF Core: {efCoreXmlDocCount}");

            // 2. Compare nullable reference type handling
            var ezDbCodeGenNullableHandling = AnalyzeNullableHandling(ezDbCodeGenOutputDir);
            var efCoreNullableHandling = AnalyzeNullableHandling(efCoreOutputDir);
            
            _output.WriteLine($"Nullable reference type handling:");
            _output.WriteLine($"  EzDbCodeGen: {ezDbCodeGenNullableHandling}");
            _output.WriteLine($"  EF Core: {efCoreNullableHandling}");

            // 3. Compare use of modern C# features
            var ezDbCodeGenModernFeatures = AnalyzeModernCSharpFeatures(ezDbCodeGenOutputDir);
            var efCoreModernFeatures = AnalyzeModernCSharpFeatures(efCoreOutputDir);
            
            _output.WriteLine($"Modern C# features usage:");
            foreach (var feature in ezDbCodeGenModernFeatures.Keys)
            {
                var ezDbCount = ezDbCodeGenModernFeatures.TryGetValue(feature, out var ezCount) ? ezCount : 0;
                var efCoreCount = efCoreModernFeatures.TryGetValue(feature, out var efCount) ? efCount : 0;
                
                _output.WriteLine($"  {feature}: EzDbCodeGen: {ezDbCount}, EF Core: {efCoreCount}");
            }
        }

        private void GenerateCodeWithEzDbCodeGen(string connectionString, string databaseName, string outputDir)
        {
            try
            {
                _logger.LogInformation($"Generating code with EzDbCodeGen for {databaseName}...");

                // Create a database configuration
                var dbConfig = new DatabaseConfig
                {
                    ConnectionString = connectionString,
                    DatabaseType = "SqlServer",
                    Name = databaseName
                };

                // Create a project configuration
                var projectConfig = new ProjectConfig
                {
                    OutputPath = outputDir,
                    TemplateLanguage = "handlebars",
                    TemplatePath = "templates/csharp",
                    Database = dbConfig
                };

                // Use the CLI runner to generate code
                var runner = new CliRunner();
                runner.GenerateCode(projectConfig);

                _logger.LogInformation($"EzDbCodeGen code generation completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code with EzDbCodeGen");
                throw;
            }
        }

        private void GenerateCodeWithEFCore(string connectionString, string databaseName, string outputDir)
        {
            try
            {
                _logger.LogInformation($"Generating code with EF Core for {databaseName}...");

                // Create a temporary project for EF Core scaffolding
                var tempProjectDir = Path.Combine(_tempOutputDir, "EFCoreTemp", databaseName);
                Directory.CreateDirectory(tempProjectDir);

                // Create a simple console project
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"new console -o {tempProjectDir}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(processStartInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    _logger.LogError($"Error creating temporary project: {process.StandardError.ReadToEnd()}");
                    throw new Exception("Failed to create temporary project for EF Core scaffolding");
                }

                // Add EF Core packages
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"add {tempProjectDir}/EFCoreTemp.csproj package Microsoft.EntityFrameworkCore.Design",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = Process.Start(processStartInfo);
                process.WaitForExit();

                processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"add {tempProjectDir}/EFCoreTemp.csproj package Microsoft.EntityFrameworkCore.SqlServer",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = Process.Start(processStartInfo);
                process.WaitForExit();

                // Run EF Core scaffolding
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"ef dbcontext scaffold \"{connectionString}\" Microsoft.EntityFrameworkCore.SqlServer -o {outputDir} --context {databaseName}Context --force",
                    WorkingDirectory = tempProjectDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                process = Process.Start(processStartInfo);
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    _logger.LogError($"Error scaffolding with EF Core: {process.StandardError.ReadToEnd()}");
                    throw new Exception("Failed to scaffold database with EF Core");
                }

                _logger.LogInformation($"EF Core code generation completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating code with EF Core");
                throw;
            }
        }

        private int CountLinesOfCode(string[] files)
        {
            int totalLines = 0;
            
            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);
                totalLines += lines.Length;
            }
            
            return totalLines;
        }

        private int CountFilesWithXmlDocumentation(string directory)
        {
            int count = 0;
            var files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                if (content.Contains("/// <summary>"))
                {
                    count++;
                }
            }
            
            return count;
        }

        private string AnalyzeNullableHandling(string directory)
        {
            var files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
            bool usesNullableAnnotations = false;
            bool usesNullableReferences = false;
            
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                
                if (content.Contains("#nullable enable") || content.Contains("#nullable disable"))
                {
                    usesNullableAnnotations = true;
                }
                
                if (content.Contains("string?") || content.Contains(" ? "))
                {
                    usesNullableReferences = true;
                }
            }
            
            if (usesNullableAnnotations && usesNullableReferences)
            {
                return "Full nullable reference type support";
            }
            else if (usesNullableAnnotations || usesNullableReferences)
            {
                return "Partial nullable reference type support";
            }
            else
            {
                return "No nullable reference type support";
            }
        }

        private Dictionary<string, int> AnalyzeModernCSharpFeatures(string directory)
        {
            var result = new Dictionary<string, int>
            {
                { "Record types", 0 },
                { "Init-only properties", 0 },
                { "Pattern matching", 0 },
                { "Switch expressions", 0 },
                { "Using declarations", 0 },
                { "Target-typed new", 0 }
            };
            
            var files = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                
                if (content.Contains("record "))
                {
                    result["Record types"]++;
                }
                
                if (content.Contains(" init;") || content.Contains(" init "))
                {
                    result["Init-only properties"]++;
                }
                
                if (content.Contains(" is ") && (content.Contains(" when ") || content.Contains(" and ") || content.Contains(" or ")))
                {
                    result["Pattern matching"]++;
                }
                
                if (content.Contains(" switch ") && content.Contains(" => "))
                {
                    result["Switch expressions"]++;
                }
                
                if (content.Contains("using var ") || content.Contains("using ") && !content.Contains("using ("))
                {
                    result["Using declarations"]++;
                }
                
                if (content.Contains("= new()"))
                {
                    result["Target-typed new"]++;
                }
            }
            
            return result;
        }
    }
}
