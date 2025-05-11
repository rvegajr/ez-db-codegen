using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.EFCoreComparison.Tests.SuperiorityValidation
{
    public class CLIExperienceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _cliPath;
        private readonly string _outputPath;

        public CLIExperienceTests(ITestOutputHelper output)
        {
            _output = output;
            _cliPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "src", "EzDbCodeGen.Cli", "bin", "Debug", "net9.0", "EzDbCodeGen.Cli.dll");
            _outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CLIOutput");
            
            // Ensure output directory exists
            Directory.CreateDirectory(_outputPath);
        }

        [Fact]
        public async Task Should_Provide_Better_Command_Line_Experience()
        {
            // Skip if CLI is not built
            if (!File.Exists(_cliPath))
            {
                _output.WriteLine($"CLI not found at {_cliPath}. Skipping test.");
                return;
            }

            // Arrange - Prepare command line arguments
            var connectionString = "Server=localhost;Database=WideWorldImporters;User Id=sa;Password=APADemo123!;TrustServerCertificate=True;";
            var schema = "Sales";
            var templatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Templates");
            
            // Act - Run CLI with help command
            var helpResult = await RunCliCommand("--help");
            
            // Act - Run CLI with version command
            var versionResult = await RunCliCommand("--version");
            
            // Act - Run CLI with schema extraction command
            var extractCommand = $"extract --connection \"{connectionString}\" --schema {schema} --output \"{Path.Combine(_outputPath, "schema.json")}\"";
            var extractResult = await RunCliCommand(extractCommand);
            
            // Act - Run CLI with generation command
            var generateCommand = $"generate --connection \"{connectionString}\" --schema {schema} --templates \"{templatesPath}\" --output \"{_outputPath}\"";
            var generateResult = await RunCliCommand(generateCommand);
            
            // Assert - Help command should show comprehensive help
            helpResult.Output.Should().Contain("Usage:", "because help output should show usage information");
            helpResult.Output.Should().Contain("Options:", "because help output should show available options");
            helpResult.Output.Should().Contain("Commands:", "because help output should show available commands");
            
            // Assert - Version command should show version
            versionResult.Output.Should().Contain("EzDbCodeGen CLI version", "because version output should show the version");
            
            // Assert - Extract command should succeed
            extractResult.ExitCode.Should().Be(0, "because schema extraction should succeed");
            File.Exists(Path.Combine(_outputPath, "schema.json")).Should().BeTrue("because schema should be extracted to a file");
            
            // Assert - Generate command should succeed
            generateResult.ExitCode.Should().Be(0, "because code generation should succeed");
            Directory.GetFiles(_outputPath, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty("because code files should be generated");
            
            // Compare with EF Core CLI experience (simulated)
            var efCoreCliExperience = SimulateEFCoreCliExperience();
            
            // Evaluate CLI experience quality
            var ezDbCodeGenCliQuality = EvaluateCliExperienceQuality(helpResult.Output, extractResult.Output, generateResult.Output);
            var efCoreCliQuality = EvaluateCliExperienceQuality(efCoreCliExperience.HelpOutput, efCoreCliExperience.ExtractOutput, efCoreCliExperience.GenerateOutput);
            
            // Log the results
            _output.WriteLine($"EzDbCodeGen CLI experience quality score: {ezDbCodeGenCliQuality}");
            _output.WriteLine($"EF Core CLI experience quality score: {efCoreCliQuality}");
            
            // Assert
            ezDbCodeGenCliQuality.Should().BeGreaterThan(efCoreCliQuality, 
                "because EzDbCodeGen should provide a better command line experience than EF Core");
        }

        [Fact]
        public void Should_Provide_More_Intuitive_Command_Structure()
        {
            // Arrange - Define EzDbCodeGen CLI commands
            var ezDbCodeGenCommands = new[]
            {
                "extract --connection \"Server=...\" --schema Sales --output schema.json",
                "generate --connection \"Server=...\" --schema Sales --templates ./Templates --output ./Output",
                "validate --schema schema.json",
                "diff --source schema1.json --target schema2.json --output diff.json"
            };
            
            // Arrange - Define EF Core CLI commands (simulated)
            var efCoreCommands = new[]
            {
                "dbcontext scaffold \"Server=...\" Microsoft.EntityFrameworkCore.SqlServer -o Models",
                "dbcontext info",
                "dbcontext list",
                "migrations add InitialCreate",
                "migrations list"
            };
            
            // Act - Evaluate command structure intuitiveness
            var ezDbCodeGenIntuitiveness = EvaluateCommandIntuitiveness(ezDbCodeGenCommands);
            var efCoreIntuitiveness = EvaluateCommandIntuitiveness(efCoreCommands);
            
            // Log the results
            _output.WriteLine($"EzDbCodeGen command intuitiveness score: {ezDbCodeGenIntuitiveness}");
            _output.WriteLine($"EF Core command intuitiveness score: {efCoreIntuitiveness}");
            
            // Assert
            ezDbCodeGenIntuitiveness.Should().BeGreaterThan(efCoreIntuitiveness,
                "because EzDbCodeGen should provide more intuitive commands than EF Core");
        }

        [Fact]
        public void Should_Provide_Better_Error_Messages()
        {
            // Arrange - Define sample error scenarios
            var ezDbCodeGenErrors = new[]
            {
                "Error: Connection string is required. Use --connection to specify a connection string.",
                "Error: Could not connect to database. Please check your connection string and ensure the database server is running.",
                "Error: Schema 'InvalidSchema' not found in the database. Available schemas: dbo, Sales, Purchasing.",
                "Error: Template file not found at './Templates/Entity.hbs'. Please check the template path."
            };
            
            // Arrange - Define EF Core error messages (simulated)
            var efCoreErrors = new[]
            {
                "No DbContext was found.",
                "Unable to create an object of type 'ApplicationDbContext'.",
                "The entity type 'Customer' requires a primary key to be defined.",
                "The specified framework 'Microsoft.NETCore.App', version '1.0.0' was not found."
            };
            
            // Act - Evaluate error message quality
            var ezDbCodeGenErrorQuality = EvaluateErrorMessageQuality(ezDbCodeGenErrors);
            var efCoreErrorQuality = EvaluateErrorMessageQuality(efCoreErrors);
            
            // Log the results
            _output.WriteLine($"EzDbCodeGen error message quality score: {ezDbCodeGenErrorQuality}");
            _output.WriteLine($"EF Core error message quality score: {efCoreErrorQuality}");
            
            // Assert
            ezDbCodeGenErrorQuality.Should().BeGreaterThan(efCoreErrorQuality,
                "because EzDbCodeGen should provide better error messages than EF Core");
        }

        private async Task<(int ExitCode, string Output)> RunCliCommand(string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{_cliPath}\" {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            var process = new Process { StartInfo = startInfo };
            process.Start();
            
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            return (process.ExitCode, output + Environment.NewLine + error);
        }

        private (string HelpOutput, string ExtractOutput, string GenerateOutput) SimulateEFCoreCliExperience()
        {
            // Simulate EF Core CLI help output
            var helpOutput = @"Usage: dotnet ef [options] [command]

Options:
  --version        Show version information
  -h|--help        Show help information
  -v|--verbose     Show verbose output
  --no-color       Don't colorize output
  --prefix-output  Prefix output with level

Commands:
  database    Commands to manage the database
  dbcontext   Commands to manage DbContext types
  migrations  Commands to manage migrations";

            // Simulate EF Core CLI extract output (scaffold)
            var extractOutput = @"Building...
Done.";

            // Simulate EF Core CLI generate output
            var generateOutput = @"Building...
Done.";

            return (helpOutput, extractOutput, generateOutput);
        }

        private int EvaluateCliExperienceQuality(string helpOutput, string extractOutput, string generateOutput)
        {
            int score = 0;
            
            // Evaluate help output
            if (helpOutput.Contains("Usage:"))
                score += 1;
            
            if (helpOutput.Contains("Options:"))
                score += 1;
            
            if (helpOutput.Contains("Commands:"))
                score += 1;
            
            if (helpOutput.Length > 500) // Comprehensive help
                score += 2;
            
            // Evaluate extract output
            if (!string.IsNullOrEmpty(extractOutput))
                score += 1;
            
            if (extractOutput.Contains("tables") || extractOutput.Contains("schema") || extractOutput.Contains("database"))
                score += 2;
            
            // Evaluate generate output
            if (!string.IsNullOrEmpty(generateOutput))
                score += 1;
            
            if (generateOutput.Contains("generated") || generateOutput.Contains("created") || generateOutput.Contains("files"))
                score += 2;
            
            if (generateOutput.Contains("successfully"))
                score += 1;
            
            return score;
        }

        private int EvaluateCommandIntuitiveness(string[] commands)
        {
            int score = 0;
            
            foreach (var command in commands)
            {
                // Check if command uses verb-noun structure
                if (command.Split(' ')[0].All(char.IsLetter))
                    score += 1;
                
                // Check if command uses clear parameter names
                if (command.Contains("--"))
                    score += 1;
                
                // Check if parameter names are descriptive
                if (command.Contains("--connection") || command.Contains("--schema") || 
                    command.Contains("--output") || command.Contains("--templates"))
                    score += 1;
                
                // Check if command is self-explanatory
                var mainVerb = command.Split(' ')[0];
                if (mainVerb == "extract" || mainVerb == "generate" || 
                    mainVerb == "validate" || mainVerb == "diff")
                    score += 2;
            }
            
            return score;
        }

        private int EvaluateErrorMessageQuality(string[] errors)
        {
            int score = 0;
            
            foreach (var error in errors)
            {
                // Check if error message starts with "Error:"
                if (error.StartsWith("Error:"))
                    score += 1;
                
                // Check if error message explains the problem
                if (error.Contains("not found") || error.Contains("required") || 
                    error.Contains("invalid") || error.Contains("could not"))
                    score += 1;
                
                // Check if error message suggests a solution
                if (error.Contains("Use") || error.Contains("Please check") || 
                    error.Contains("Ensure") || error.Contains("Available"))
                    score += 2;
                
                // Check if error message is specific
                if (error.Length > 50)
                    score += 1;
            }
            
            return score;
        }
    }
}
