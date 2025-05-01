using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;

namespace EzDbCodeGen.Cli
{
    /// <summary>
    /// Processes command-line arguments and executes the appropriate command.
    /// </summary>
    public class CommandLineProcessor : ICommandLineProcessor
    {
        private readonly ICommandFactory _commandFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineProcessor"/> class.
        /// </summary>
        /// <param name="commandFactory">The command factory to use for creating commands.</param>
        /// <param name="logger">The logger to use.</param>
        public CommandLineProcessor(ICommandFactory commandFactory, ILogger logger)
        {
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task ProcessAsync(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                DisplayGeneralHelp();
                return;
            }

            var commandName = args[0];
            var options = ParseOptions(args);

            if (!_commandFactory.IsCommandRegistered(commandName))
            {
                _logger.Error($"Unknown command: {commandName}");
                _logger.Info("\nUse 'ezdbcodegen help' to see available commands.");
                return;
            }

            await ProcessCommandAsync(commandName, options);
        }

        /// <inheritdoc/>
        public async Task ProcessCommandAsync(string commandName, IReadOnlyDictionary<string, string> options)
        {
            var command = _commandFactory.CreateCommand(commandName);

            if (!command.ValidateOptions(options))
            {
                _logger.Error($"Invalid options for command: {commandName}");
                foreach (var error in command.GetValidationErrors(options))
                {
                    _logger.Error($"  - {error}");
                }
                _logger.Info($"\nUsage: {command.Usage}");
                return;
            }

            try
            {
                await command.ExecuteAsync(options);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error executing command {commandName}: {ex.Message}");
                _logger.Debug(ex.ToString());
            }
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetAvailableCommands()
        {
            var commands = new Dictionary<string, string>();

            foreach (var command in _commandFactory.GetAllCommands())
            {
                commands[command.Name] = command.Description;
            }

            return commands;
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetCommandOptions(string commandName)
        {
            if (!_commandFactory.IsCommandRegistered(commandName))
            {
                return new Dictionary<string, string>();
            }

            return _commandFactory.CreateCommand(commandName).GetOptions();
        }

        /// <inheritdoc/>
        public string GetCommandUsage(string commandName)
        {
            if (!_commandFactory.IsCommandRegistered(commandName))
            {
                return string.Empty;
            }

            return _commandFactory.CreateCommand(commandName).Usage;
        }

        /// <inheritdoc/>
        public string GetUsage()
        {
            return "Usage: ezdbcodegen <command> [options]\n\nUse 'ezdbcodegen help' to see available commands.";
        }

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

        private void DisplayGeneralHelp()
        {
            _logger.Info("EzDbCodeGen - Database Code Generation Tool");
            _logger.Info("========================================");
            _logger.Info(GetUsage());
            _logger.Info("\nAvailable commands:");

            foreach (var command in _commandFactory.GetAllCommands())
            {
                _logger.Info($"  {command.Name,-15} {command.Description}");
            }

            _logger.Info("\nUse 'ezdbcodegen help <command>' for more information about a specific command.");
        }
    }
}
