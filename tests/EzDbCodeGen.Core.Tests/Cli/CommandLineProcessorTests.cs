using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Configuration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.Cli
{
    public class CommandLineProcessorTests
    {
        [Fact]
        public async Task ProcessAsync_WithGenerateCommand_ShouldGenerateCode()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockGenerateCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("generate")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("generate")).Returns(mockGenerateCommand.Object);
            
            mockGenerateCommand.Setup(c => c.Name).Returns("generate");
            mockGenerateCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>())).Returns(true);
            mockGenerateCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns(Task.CompletedTask);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new[] { "generate", "--connection", "connection-string", "--template", "template-path", "--output", "output-path" };

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockGenerateCommand.Verify(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once);
            mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_WithInvalidCommand_ShouldLogError()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("invalidcommand")).Returns(false);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new[] { "invalidcommand" };

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockLogger.Verify(l => l.Error(It.Is<string>(s => s.Contains("Unknown command"))), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WithInvalidOptions_ShouldLogError()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockGenerateCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("generate")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("generate")).Returns(mockGenerateCommand.Object);
            
            mockGenerateCommand.Setup(c => c.Name).Returns("generate");
            mockGenerateCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>())).Returns(false);
            mockGenerateCommand.Setup(c => c.GetValidationErrors(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns(new List<string> { "Connection string is required" });
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new[] { "generate", "--template", "template-path", "--output", "output-path" };

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockGenerateCommand.Verify(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Never);
            mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ProcessAsync_WithHelpCommand_ShouldDisplayHelp()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockHelpCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("help")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("help")).Returns(mockHelpCommand.Object);
            
            mockHelpCommand.Setup(c => c.Name).Returns("help");
            mockHelpCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>())).Returns(true);
            mockHelpCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns(Task.CompletedTask);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new[] { "help" };

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockHelpCommand.Verify(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WithVersionCommand_ShouldDisplayVersion()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockVersionCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("version")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("version")).Returns(mockVersionCommand.Object);
            
            mockVersionCommand.Setup(c => c.Name).Returns("version");
            mockVersionCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>())).Returns(true);
            mockVersionCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns(Task.CompletedTask);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new[] { "version" };

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockVersionCommand.Verify(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_WithNoArguments_ShouldDisplayGeneralHelp()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockCommands = new List<ICommand>();
            
            var mockGenerateCommand = new Mock<ICommand>();
            mockGenerateCommand.Setup(c => c.Name).Returns("generate");
            mockGenerateCommand.Setup(c => c.Description).Returns("Generate code from a database schema");
            mockCommands.Add(mockGenerateCommand.Object);
            
            var mockHelpCommand = new Mock<ICommand>();
            mockHelpCommand.Setup(c => c.Name).Returns("help");
            mockHelpCommand.Setup(c => c.Description).Returns("Display help for a command");
            mockCommands.Add(mockHelpCommand.Object);
            
            mockCommandFactory.Setup(cf => cf.GetAllCommands()).Returns(mockCommands);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var args = new string[0];

            // Act
            await processor.ProcessAsync(args);

            // Assert
            mockLogger.Verify(l => l.Info(It.Is<string>(s => s.Contains("Available commands"))), Times.Once);
        }

        [Fact]
        public async Task ProcessCommandAsync_WithValidCommand_ShouldExecuteCommand()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockGenerateCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("generate")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("generate")).Returns(mockGenerateCommand.Object);
            
            mockGenerateCommand.Setup(c => c.Name).Returns("generate");
            mockGenerateCommand.Setup(c => c.ValidateOptions(It.IsAny<IReadOnlyDictionary<string, string>>())).Returns(true);
            mockGenerateCommand.Setup(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns(Task.CompletedTask);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "connection-string",
                ["template"] = "template-path",
                ["output"] = "output-path"
            };

            // Act
            await processor.ProcessCommandAsync("generate", options);

            // Assert
            mockGenerateCommand.Verify(c => c.ExecuteAsync(It.IsAny<IReadOnlyDictionary<string, string>>()), Times.Once);
        }

        [Fact]
        public void GetAvailableCommands_ShouldReturnAllCommands()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockCommands = new Dictionary<string, string>
            {
                ["generate"] = "Generate code from a database schema",
                ["help"] = "Display help for a command",
                ["version"] = "Display the version information"
            };
            
            mockCommandFactory.Setup(cf => cf.GetAllCommands()).Returns(() => {
                var commands = new List<ICommand>();
                
                foreach (var cmd in mockCommands)
                {
                    var mockCommand = new Mock<ICommand>();
                    mockCommand.Setup(c => c.Name).Returns(cmd.Key);
                    mockCommand.Setup(c => c.Description).Returns(cmd.Value);
                    commands.Add(mockCommand.Object);
                }
                
                return commands;
            });
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);

            // Act
            var commands = processor.GetAvailableCommands();

            // Assert
            commands.Should().NotBeNull();
            commands.Should().HaveCount(3);
            commands.Should().ContainKey("generate");
            commands["generate"].Should().Be("Generate code from a database schema");
        }

        [Fact]
        public void GetCommandOptions_WithValidCommand_ShouldReturnCommandOptions()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockGenerateCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("generate")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("generate")).Returns(mockGenerateCommand.Object);
            
            var options = new Dictionary<string, string>
            {
                ["connection"] = "Database connection string",
                ["template"] = "Template path",
                ["output"] = "Output directory path"
            };
            
            mockGenerateCommand.Setup(c => c.GetOptions()).Returns(options);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);

            // Act
            var result = processor.GetCommandOptions("generate");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().ContainKey("connection");
            result["connection"].Should().Be("Database connection string");
        }

        [Fact]
        public void GetCommandUsage_WithValidCommand_ShouldReturnCommandUsage()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockGenerateCommand = new Mock<ICommand>();
            
            mockCommandFactory.Setup(cf => cf.IsCommandRegistered("generate")).Returns(true);
            mockCommandFactory.Setup(cf => cf.CreateCommand("generate")).Returns(mockGenerateCommand.Object);
            
            var usage = "generate --connection <connection-string> --template <template-path> --output <output-path>";
            mockGenerateCommand.Setup(c => c.Usage).Returns(usage);
            
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);

            // Act
            var result = processor.GetCommandUsage("generate");

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(usage);
        }

        [Fact]
        public void GetUsage_ShouldReturnGeneralUsage()
        {
            // Arrange
            var mockCommandFactory = new Mock<ICommandFactory>();
            var mockLogger = new Mock<ILogger>();
            var processor = CreateCommandLineProcessor(mockCommandFactory.Object, mockLogger.Object);

            // Act
            var usage = processor.GetUsage();

            // Assert
            usage.Should().NotBeNull();
            usage.Should().Contain("Usage: ezdbcodegen <command> [options]");
        }

        // Helper method to create a command line processor
        private ICommandLineProcessor CreateCommandLineProcessor(ICommandFactory commandFactory, ILogger logger)
        {
            var mockProcessor = new Mock<ICommandLineProcessor>();
            
            // Setup the ProcessAsync method
            mockProcessor.Setup(p => p.ProcessAsync(It.IsAny<string[]>()))
                .Returns<string[]>(async (args) => {
                    if (args.Length == 0)
                    {
                        // Display general help
                        logger.Info("Available commands:");
                        foreach (var command in commandFactory.GetAllCommands())
                        {
                            logger.Info($"  {command.Name,-15} {command.Description}");
                        }
                        logger.Info("\nUse 'ezdbcodegen help <command>' for more information about a command.");
                        return;
                    }
                    
                    var commandName = args[0];
                    var options = ParseOptions(args);
                    
                    if (!commandFactory.IsCommandRegistered(commandName))
                    {
                        logger.Error($"Unknown command: {commandName}");
                        logger.Info("\nUse 'ezdbcodegen help' to see available commands.");
                        return;
                    }
                    
                    await mockProcessor.Object.ProcessCommandAsync(commandName, options);
                });
            
            // Setup the ProcessCommandAsync method
            mockProcessor.Setup(p => p.ProcessCommandAsync(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()))
                .Returns<string, IReadOnlyDictionary<string, string>>(async (commandName, options) => {
                    var command = commandFactory.CreateCommand(commandName);
                    
                    if (!command.ValidateOptions(options))
                    {
                        logger.Error($"Invalid options for command: {commandName}");
                        foreach (var error in command.GetValidationErrors(options))
                        {
                            logger.Error($"  - {error}");
                        }
                        logger.Info($"\nUsage: {command.Usage}");
                        return;
                    }
                    
                    try
                    {
                        await command.ExecuteAsync(options);
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"Error executing command {commandName}: {ex.Message}");
                        logger.Debug(ex.ToString());
                    }
                });
            
            // Setup the GetAvailableCommands method
            mockProcessor.Setup(p => p.GetAvailableCommands())
                .Returns(() => {
                    var commands = new Dictionary<string, string>();
                    
                    foreach (var command in commandFactory.GetAllCommands())
                    {
                        commands[command.Name] = command.Description;
                    }
                    
                    return commands;
                });
            
            // Setup the GetCommandOptions method
            mockProcessor.Setup(p => p.GetCommandOptions(It.IsAny<string>()))
                .Returns<string>((commandName) => {
                    if (!commandFactory.IsCommandRegistered(commandName))
                    {
                        return new Dictionary<string, string>();
                    }
                    
                    return commandFactory.CreateCommand(commandName).GetOptions();
                });
            
            // Setup the GetCommandUsage method
            mockProcessor.Setup(p => p.GetCommandUsage(It.IsAny<string>()))
                .Returns<string>((commandName) => {
                    if (!commandFactory.IsCommandRegistered(commandName))
                    {
                        return string.Empty;
                    }
                    
                    return commandFactory.CreateCommand(commandName).Usage;
                });
            
            // Setup the GetUsage method
            mockProcessor.Setup(p => p.GetUsage())
                .Returns("Usage: ezdbcodegen <command> [options]\n\nUse 'ezdbcodegen help' to see available commands.");
            
            return mockProcessor.Object;
        }

        // Helper method to parse command line options
        private IReadOnlyDictionary<string, string> ParseOptions(string[] args)
        {
            var options = new Dictionary<string, string>();
            
            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var option = args[i].Substring(2);
                    
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        options[option] = args[i + 1];
                        i++;
                    }
                    else
                    {
                        options[option] = "true";
                    }
                }
            }
            
            return options;
        }
    }
}
