using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// Factory for creating and managing command instances.
    /// </summary>
    public class CommandFactory : ICommandFactory
    {
        private readonly Dictionary<string, ICommand> _commands = new();
        private readonly ILogger _logger;
        private readonly IDatabaseSchemaProviderFactory _schemaProviderFactory;
        private readonly ICodeGeneratorFactory _codeGeneratorFactory;
        private readonly IRelationshipDetectorFactory _relationshipDetectorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="schemaProviderFactory">The schema provider factory to use.</param>
        /// <param name="codeGeneratorFactory">The code generator factory to use.</param>
        /// <param name="relationshipDetectorFactory">The relationship detector factory to use.</param>
        public CommandFactory(
            ILogger logger,
            IDatabaseSchemaProviderFactory schemaProviderFactory,
            ICodeGeneratorFactory codeGeneratorFactory,
            IRelationshipDetectorFactory relationshipDetectorFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schemaProviderFactory = schemaProviderFactory ?? throw new ArgumentNullException(nameof(schemaProviderFactory));
            _codeGeneratorFactory = codeGeneratorFactory ?? throw new ArgumentNullException(nameof(codeGeneratorFactory));
            _relationshipDetectorFactory = relationshipDetectorFactory ?? throw new ArgumentNullException(nameof(relationshipDetectorFactory));
        }

        /// <inheritdoc/>
        public void RegisterCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commands[command.Name] = command;
        }

        /// <inheritdoc/>
        public bool IsCommandRegistered(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                return false;
            }

            return _commands.ContainsKey(commandName);
        }

        /// <inheritdoc/>
        public ICommand CreateCommand(string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                throw new ArgumentException("Command name cannot be empty.", nameof(commandName));
            }

            if (!_commands.TryGetValue(commandName, out var command))
            {
                throw new KeyNotFoundException($"Command '{commandName}' not found.");
            }

            return command;
        }

        /// <inheritdoc/>
        public IList<ICommand> GetAllCommands()
        {
            return _commands.Values.ToList();
        }

        /// <inheritdoc/>
        public void RegisterStandardCommands()
        {
            // Register generate command
            RegisterCommand(new GenerateCommand(
                _codeGeneratorFactory.CreateCodeGenerator(),
                _schemaProviderFactory.CreateSchemaProvider("SqlServer"),
                _logger));

            // Register schema-info command
            RegisterCommand(new SchemaInfoCommand(
                _schemaProviderFactory,
                _relationshipDetectorFactory,
                _logger));
                
            // Register schema-diff command
            RegisterCommand(new SchemaDiffCommand(
                _schemaProviderFactory,
                _relationshipDetectorFactory,
                _logger));

            // Register help command
            RegisterCommand(new HelpCommand(_logger));

            // Register version command
            RegisterCommand(new VersionCommand(_logger));
        }
    }
}
