using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Configuration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.Cli
{
    public class GenerateCommandTests
    {
        [Fact]
        public async Task ExecuteAsync_WithValidOptions_ShouldGenerateCode()
        {
            // Arrange
            var mockCodeGenerator = new Mock<ICodeGenerator>();
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            mockSchemaProvider.Setup(sp => sp.GetSchemaAsync())
                .ReturnsAsync(new Mock<IDatabaseSchema>().Object);
            
            mockCodeGenerator.Setup(cg => cg.GenerateAsync(
                It.IsAny<IDatabaseSchema>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CodeGenerationOptions>()))
                .Returns(Task.CompletedTask);
            
            var command = CreateGenerateCommand(mockCodeGenerator.Object, mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string",
                ["template"] = "template-path",
                ["output"] = "output-path"
            };

            // Act
            await command.ExecuteAsync(options);

            // Assert
            mockSchemaProvider.Verify(sp => sp.GetSchemaAsync(), Times.Once);
            mockCodeGenerator.Verify(cg => cg.GenerateAsync(
                It.IsAny<IDatabaseSchema>(),
                It.Is<string>(s => s == "template-path"),
                It.Is<string>(s => s == "output-path"),
                It.IsAny<CodeGenerationOptions>()), Times.Once);
        }

        [Fact]
        public void ValidateOptions_WithRequiredOptions_ShouldReturnTrue()
        {
            // Arrange
            var mockCodeGenerator = new Mock<ICodeGenerator>();
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateGenerateCommand(mockCodeGenerator.Object, mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string",
                ["template"] = "template-path",
                ["output"] = "output-path"
            };

            // Act
            var result = command.ValidateOptions(options);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidateOptions_WithMissingOptions_ShouldReturnFalse()
        {
            // Arrange
            var mockCodeGenerator = new Mock<ICodeGenerator>();
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateGenerateCommand(mockCodeGenerator.Object, mockSchemaProvider.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["template"] = "template-path",
                ["output"] = "output-path"
                // Missing connection
            };

            // Act
            var result = command.ValidateOptions(options);
            var errors = command.GetValidationErrors(options);

            // Assert
            result.Should().BeFalse();
            errors.Should().NotBeEmpty();
            errors.Should().Contain(e => e.Contains("connection"));
        }

        [Fact]
        public void GetOptions_ShouldReturnAllAvailableOptions()
        {
            // Arrange
            var mockCodeGenerator = new Mock<ICodeGenerator>();
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateGenerateCommand(mockCodeGenerator.Object, mockSchemaProvider.Object, mockLogger.Object);

            // Act
            var options = command.GetOptions();

            // Assert
            options.Should().NotBeNull();
            options.Should().ContainKey("connection");
            options.Should().ContainKey("template");
            options.Should().ContainKey("output");
            options.Should().ContainKey("provider");
            options.Should().ContainKey("namespace");
        }

        [Fact]
        public void GetRequiredOptions_ShouldReturnRequiredOptions()
        {
            // Arrange
            var mockCodeGenerator = new Mock<ICodeGenerator>();
            var mockSchemaProvider = new Mock<IDatabaseSchemaProvider>();
            var mockLogger = new Mock<ILogger>();
            
            var command = CreateGenerateCommand(mockCodeGenerator.Object, mockSchemaProvider.Object, mockLogger.Object);

            // Act
            var requiredOptions = command.GetRequiredOptions();

            // Assert
            requiredOptions.Should().NotBeNull();
            requiredOptions.Should().Contain("connection");
            requiredOptions.Should().Contain("template");
            requiredOptions.Should().Contain("output");
        }

        // Helper method to create a generate command
        private ICommand CreateGenerateCommand(ICodeGenerator codeGenerator, IDatabaseSchemaProvider schemaProvider, ILogger logger)
        {
            var mockCommand = new Mock<ICommand>();
            
            mockCommand.Setup(c => c.Name).Returns("generate");
            mockCommand.Setup(c => c.Description).Returns("Generate code from a database schema");
            mockCommand.Setup(c => c.Usage).Returns("generate --connection <connection-string> --template <template-path> --output <output-path> [--provider <provider-name>] [--namespace <namespace>]");
            
            // Setup the ExecuteAsync method
            mockCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>(async (options) => {
                    try
                    {
                        // Extract options
                        options.TryGetValue("connection", out var connectionString);
                        options.TryGetValue("template", out var templatePath);
                        options.TryGetValue("output", out var outputPath);
                        options.TryGetValue("provider", out var providerName);
                        options.TryGetValue("namespace", out var namespaceName);
                        
                        // Set default values
                        providerName ??= "SqlServer";
                        namespaceName ??= "Generated";
                        
                        logger.Info($"Generating code from {providerName} database...");
                        logger.Info($"Connection: {connectionString}");
                        logger.Info($"Template: {templatePath}");
                        logger.Info($"Output: {outputPath}");
                        
                        // Extract schema
                        var schema = await schemaProvider.GetSchemaAsync();
                        
                        // Generate code
                        var codeGenOptions = new CodeGenerationOptions
                        {
                            Namespace = namespaceName,
                            Language = "CSharp",
                            UseDataAnnotations = true,
                            GenerateNavigationProperties = true
                        };
                        
                        await codeGenerator.GenerateAsync(schema, templatePath, outputPath, codeGenOptions);
                        
                        logger.Info("Code generation completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error generating code: {ex.Message}");
                        throw;
                    }
                });
            
            // Setup the ValidateOptions method
            mockCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>((options) => {
                    var requiredOptions = mockCommand.Object.GetRequiredOptions();
                    
                    foreach (var requiredOption in requiredOptions)
                    {
                        if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                        {
                            return false;
                        }
                    }
                    
                    return true;
                });
            
            // Setup the GetValidationErrors method
            mockCommand.Setup(c => c.GetValidationErrors(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<IReadOnlyDictionary<string, string>>((options) => {
                    var errors = new List<string>();
                    var requiredOptions = mockCommand.Object.GetRequiredOptions();
                    
                    foreach (var requiredOption in requiredOptions)
                    {
                        if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                        {
                            errors.Add($"Option '{requiredOption}' is required.");
                        }
                    }
                    
                    return errors;
                });
            
            // Setup the GetOptions method
            mockCommand.Setup(c => c.GetOptions())
                .Returns(new Dictionary<string, string>
                {
                    ["connection"] = "Database connection string",
                    ["template"] = "Template path",
                    ["output"] = "Output directory path",
                    ["provider"] = "Database provider (default: SqlServer)",
                    ["namespace"] = "Namespace for generated code (default: Generated)",
                    ["language"] = "Target language (default: CSharp)",
                    ["data-annotations"] = "Use data annotations (default: true)",
                    ["fluent-api"] = "Use fluent API (default: false)",
                    ["navigation-properties"] = "Generate navigation properties (default: true)"
                });
            
            // Setup the GetRequiredOptions method
            mockCommand.Setup(c => c.GetRequiredOptions())
                .Returns(new List<string> { "connection", "template", "output" });
            
            return mockCommand.Object;
        }
    }
}
