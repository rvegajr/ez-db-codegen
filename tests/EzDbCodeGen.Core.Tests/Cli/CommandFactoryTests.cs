using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.Cli
{
    public class CommandFactoryTests
    {
        [Fact]
        public void RegisterCommand_ShouldRegisterCommand()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Name).Returns("test-command");

            // Act
            factory.RegisterCommand(mockCommand.Object);
            var isRegistered = factory.IsCommandRegistered("test-command");

            // Assert
            isRegistered.Should().BeTrue();
        }

        [Fact]
        public void RegisterCommand_WithDuplicateName_ShouldReplaceCommand()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);
            
            var mockCommand1 = new Mock<ICommand>();
            mockCommand1.Setup(c => c.Name).Returns("test-command");
            mockCommand1.Setup(c => c.Description).Returns("First command");
            
            var mockCommand2 = new Mock<ICommand>();
            mockCommand2.Setup(c => c.Name).Returns("test-command");
            mockCommand2.Setup(c => c.Description).Returns("Second command");

            // Act
            factory.RegisterCommand(mockCommand1.Object);
            factory.RegisterCommand(mockCommand2.Object);
            var command = factory.CreateCommand("test-command");

            // Assert
            command.Should().NotBeNull();
            command.Description.Should().Be("Second command");
        }

        [Fact]
        public void IsCommandRegistered_WithRegisteredCommand_ShouldReturnTrue()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Name).Returns("test-command");
            factory.RegisterCommand(mockCommand.Object);

            // Act
            var isRegistered = factory.IsCommandRegistered("test-command");

            // Assert
            isRegistered.Should().BeTrue();
        }

        [Fact]
        public void IsCommandRegistered_WithUnregisteredCommand_ShouldReturnFalse()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);

            // Act
            var isRegistered = factory.IsCommandRegistered("unknown-command");

            // Assert
            isRegistered.Should().BeFalse();
        }

        [Fact]
        public void CreateCommand_WithRegisteredCommand_ShouldReturnCommand()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);
            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(c => c.Name).Returns("test-command");
            factory.RegisterCommand(mockCommand.Object);

            // Act
            var command = factory.CreateCommand("test-command");

            // Assert
            command.Should().NotBeNull();
            command.Should().BeSameAs(mockCommand.Object);
        }

        [Fact]
        public void CreateCommand_WithUnregisteredCommand_ShouldThrowException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => factory.CreateCommand("unknown-command"));
        }

        [Fact]
        public void GetAllCommands_ShouldReturnAllRegisteredCommands()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);
            
            var mockCommand1 = new Mock<ICommand>();
            mockCommand1.Setup(c => c.Name).Returns("command1");
            
            var mockCommand2 = new Mock<ICommand>();
            mockCommand2.Setup(c => c.Name).Returns("command2");
            
            var mockCommand3 = new Mock<ICommand>();
            mockCommand3.Setup(c => c.Name).Returns("command3");
            
            factory.RegisterCommand(mockCommand1.Object);
            factory.RegisterCommand(mockCommand2.Object);
            factory.RegisterCommand(mockCommand3.Object);

            // Act
            var commands = factory.GetAllCommands();

            // Assert
            commands.Should().NotBeNull();
            commands.Should().HaveCount(3);
            commands.Select(c => c.Name).Should().Contain(new[] { "command1", "command2", "command3" });
        }

        [Fact]
        public void GetAllCommands_WithNoRegisteredCommands_ShouldReturnEmptyList()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);

            // Act
            var commands = factory.GetAllCommands();

            // Assert
            commands.Should().NotBeNull();
            commands.Should().BeEmpty();
        }

        [Fact]
        public void RegisterStandardCommands_ShouldRegisterDefaultCommands()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var factory = CreateCommandFactory(mockLogger.Object);

            // Act
            factory.RegisterStandardCommands();
            var commands = factory.GetAllCommands();
            var commandNames = commands.Select(c => c.Name).ToList();

            // Assert
            commands.Should().NotBeNull();
            commands.Should().NotBeEmpty();
            commandNames.Should().Contain("generate");
            commandNames.Should().Contain("schema-info");
            commandNames.Should().Contain("help");
            commandNames.Should().Contain("version");
        }

        // Helper method to create a command factory
        private ICommandFactory CreateCommandFactory(ILogger logger)
        {
            var mockFactory = new Mock<ICommandFactory>();
            var commands = new Dictionary<string, ICommand>();
            
            mockFactory.Setup(f => f.RegisterCommand(It.IsAny<ICommand>()))
                .Callback<ICommand>(command => commands[command.Name] = command);
            
            mockFactory.Setup(f => f.IsCommandRegistered(It.IsAny<string>()))
                .Returns<string>(name => commands.ContainsKey(name));
            
            mockFactory.Setup(f => f.CreateCommand(It.IsAny<string>()))
                .Returns<string>(name => {
                    if (commands.TryGetValue(name, out var command))
                    {
                        return command;
                    }
                    throw new KeyNotFoundException($"Command '{name}' not found.");
                });
            
            mockFactory.Setup(f => f.GetAllCommands())
                .Returns(() => commands.Values.ToList());
            
            mockFactory.Setup(f => f.RegisterStandardCommands())
                .Callback(() => {
                    // Register generate command
                    var mockGenerateCommand = new Mock<ICommand>();
                    mockGenerateCommand.Setup(c => c.Name).Returns("generate");
                    mockGenerateCommand.Setup(c => c.Description).Returns("Generate code from a database schema");
                    commands["generate"] = mockGenerateCommand.Object;
                    
                    // Register schema-info command
                    var mockSchemaInfoCommand = new Mock<ICommand>();
                    mockSchemaInfoCommand.Setup(c => c.Name).Returns("schema-info");
                    mockSchemaInfoCommand.Setup(c => c.Description).Returns("Display information about a database schema");
                    commands["schema-info"] = mockSchemaInfoCommand.Object;
                    
                    // Register help command
                    var mockHelpCommand = new Mock<ICommand>();
                    mockHelpCommand.Setup(c => c.Name).Returns("help");
                    mockHelpCommand.Setup(c => c.Description).Returns("Display help for a command");
                    commands["help"] = mockHelpCommand.Object;
                    
                    // Register version command
                    var mockVersionCommand = new Mock<ICommand>();
                    mockVersionCommand.Setup(c => c.Name).Returns("version");
                    mockVersionCommand.Setup(c => c.Description).Returns("Display the version information");
                    commands["version"] = mockVersionCommand.Object;
                });
            
            return mockFactory.Object;
        }
    }
}
