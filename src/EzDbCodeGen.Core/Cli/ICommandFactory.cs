namespace EzDbCodeGen.Core.Cli;

/// <summary>
/// Represents a factory for creating commands.
/// </summary>
public interface ICommandFactory
{
    /// <summary>
    /// Creates a command with the specified name.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns>A command.</returns>
    ICommand CreateCommand(string commandName);
    
    /// <summary>
    /// Registers a command factory method.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <param name="factoryMethod">The factory method.</param>
    void RegisterCommandFactory(string commandName, Func<ICommand> factoryMethod);
    
    /// <summary>
    /// Gets all available command names.
    /// </summary>
    /// <returns>A list of available command names.</returns>
    IReadOnlyList<string> GetAvailableCommands();
    
    /// <summary>
    /// Gets all available commands.
    /// </summary>
    /// <returns>A list of available commands.</returns>
    IReadOnlyList<ICommand> GetAllCommands();
    
    /// <summary>
    /// Checks if a command is registered.
    /// </summary>
    /// <param name="commandName">The name of the command.</param>
    /// <returns>True if the command is registered, false otherwise.</returns>
    bool IsCommandRegistered(string commandName);
}
