using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Cli
{
    /// <summary>
    /// Defines a command-line application for EzDbCodeGen.
    /// </summary>
    public interface ICommandLineApplication
    {
        /// <summary>
        /// Registers a command handler with the application.
        /// </summary>
        /// <param name="handler">The command handler to register.</param>
        void RegisterCommand(ICommandLineHandler handler);

        /// <summary>
        /// Gets all registered command handlers.
        /// </summary>
        /// <returns>A collection of all registered command handlers.</returns>
        IReadOnlyCollection<ICommandLineHandler> GetCommands();

        /// <summary>
        /// Gets a command handler by name.
        /// </summary>
        /// <param name="commandName">The name of the command handler to get.</param>
        /// <returns>The command handler with the specified name, or null if no such command handler exists.</returns>
        ICommandLineHandler GetCommand(string commandName);

        /// <summary>
        /// Parses command-line arguments into options.
        /// </summary>
        /// <param name="args">The command-line arguments to parse.</param>
        /// <returns>The parsed command-line options.</returns>
        CommandLineOptions ParseArguments(string[] args);

        /// <summary>
        /// Executes the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>The exit code. 0 indicates success, non-zero indicates failure.</returns>
        Task<int> ExecuteAsync(string[] args);

        /// <summary>
        /// Displays general help information.
        /// </summary>
        void ShowHelp();

        /// <summary>
        /// Displays help information for a specific command.
        /// </summary>
        /// <param name="commandName">The name of the command to display help for.</param>
        void ShowCommandHelp(string commandName);

        /// <summary>
        /// Displays version information.
        /// </summary>
        void ShowVersion();

        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="path">The path to the configuration file.</param>
        /// <returns>The loaded command-line options.</returns>
        CommandLineOptions LoadConfigFile(string path);

        /// <summary>
        /// Saves a configuration file.
        /// </summary>
        /// <param name="options">The command-line options to save.</param>
        /// <param name="path">The path to save the configuration file to.</param>
        void SaveConfigFile(CommandLineOptions options, string path);

        /// <summary>
        /// Runs the application in interactive mode.
        /// </summary>
        /// <returns>The exit code. 0 indicates success, non-zero indicates failure.</returns>
        Task<int> RunInteractiveAsync();
    }
}
