using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// Command for displaying help information about other commands.
    /// </summary>
    public class HelpCommand : ICommand
    {
        private readonly ILogger _logger;
        private ICommandFactory _commandFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public HelpCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sets the command factory. This needs to be set after construction to avoid circular dependency.
        /// </summary>
        /// <param name="commandFactory">The command factory to use.</param>
        public void SetCommandFactory(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
        }

        /// <inheritdoc/>
        public string Name => "help";

        /// <inheritdoc/>
        public string Description => "Display help for a command";

        /// <inheritdoc/>
        public string Usage => "help [command]";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            // Check if the command factory has been set
            if (_commandFactory == null)
            {
                _logger.Error("Command factory has not been set. Help command cannot be executed.");
                return;
            }

            // Check if a specific command was requested
            if (options.TryGetValue("command", out var commandName))
            {
                await DisplayCommandHelp(commandName);
            }
            else
            {
                DisplayGeneralHelp();
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool ValidateOptions(IReadOnlyDictionary<string, string> options)
        {
            // No validation needed for help command
            return true;
        }

        /// <inheritdoc/>
        public IList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options)
        {
            // No validation errors for help command
            return new List<string>();
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetOptions()
        {
            return new Dictionary<string, string>
            {
                ["command"] = "Name of the command to get help for"
            };
        }

        /// <inheritdoc/>
        public IList<string> GetRequiredOptions()
        {
            // No required options for help command
            return new List<string>();
        }

        private void DisplayGeneralHelp()
        {
            _logger.Info("EzDbCodeGen - Database Code Generation Tool");
            _logger.Info("========================================");
            _logger.Info("Usage: ezdbcodegen <command> [options]\n");
            _logger.Info("Available commands:");

            foreach (var command in _commandFactory.GetAllCommands())
            {
                _logger.Info($"  {command.Name,-15} {command.Description}");
            }

            _logger.Info("\nUse 'ezdbcodegen help <command>' for more information about a specific command.");
        }

        private async Task DisplayCommandHelp(string commandName)
        {
            // Check if the command exists
            if (!_commandFactory.IsCommandRegistered(commandName))
            {
                _logger.Error($"Unknown command: {commandName}");
                _logger.Info("\nUse 'ezdbcodegen help' to see available commands.");
                return;
            }

            // Get the command
            var command = _commandFactory.CreateCommand(commandName);

            // Display command help
            _logger.Info($"Help for command: {commandName}");
            _logger.Info($"Description: {command.Description}");
            _logger.Info($"Usage: ezdbcodegen {command.Usage}\n");
            _logger.Info("Options:");

            var options = command.GetOptions();
            var requiredOptions = command.GetRequiredOptions();

            foreach (var option in options)
            {
                var requiredStr = requiredOptions.Contains(option.Key) ? " (required)" : "";
                _logger.Info($"  --{option.Key,-20} {option.Value}{requiredStr}");
            }

            await Task.CompletedTask;
        }
    }
}
