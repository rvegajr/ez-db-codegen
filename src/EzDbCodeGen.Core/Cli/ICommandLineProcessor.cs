namespace EzDbCodeGen.Core.Cli;

/// <summary>
/// Represents a processor for command-line arguments.
/// </summary>
public interface ICommandLineProcessor
{
    /// <summary>
    /// Processes command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessAsync(string[] args);
    
    /// <summary>
    /// Processes a command from the command-line interface.
    /// </summary>
    /// <param name="command">The command to process.</param>
    /// <param name="options">The command options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessCommandAsync(string command, IReadOnlyDictionary<string, string> options);
    
    /// <summary>
    /// Gets all available commands.
    /// </summary>
    /// <returns>A dictionary of command names and descriptions.</returns>
    IReadOnlyDictionary<string, string> GetAvailableCommands();
    
    /// <summary>
    /// Gets the options for a specific command.
    /// </summary>
    /// <param name="command">The command to get options for.</param>
    /// <returns>A dictionary of option names and descriptions.</returns>
    IReadOnlyDictionary<string, string> GetCommandOptions(string command);
    
    /// <summary>
    /// Gets the usage for a specific command.
    /// </summary>
    /// <param name="command">The command to get usage for.</param>
    /// <returns>The usage string.</returns>
    string GetCommandUsage(string command);
    
    /// <summary>
    /// Gets the overall usage for the command-line interface.
    /// </summary>
    /// <returns>The usage string.</returns>
    string GetUsage();
    
    /// <summary>
    /// Registers a command handler.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <param name="description">The command description.</param>
    /// <param name="handler">The command handler.</param>
    void RegisterCommandHandler(string command, string description, Func<IReadOnlyDictionary<string, string>, Task> handler);
}
