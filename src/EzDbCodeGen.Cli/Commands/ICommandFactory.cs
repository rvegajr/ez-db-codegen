using System.Collections.Generic;

namespace EzDbCodeGen.Cli.Commands;

/// <summary>
/// Defines a factory for creating and managing commands.
/// </summary>
public interface ICommandFactory
{
    /// <summary>
    /// Registers a command with the factory.
    /// </summary>
    /// <param name="command">The command to register.</param>
    void RegisterCommand(ICommand command);
    
    /// <summary>
    /// Gets a command by name.
    /// </summary>
    /// <param name="name">The name of the command.</param>
    /// <returns>The command if found; otherwise, null.</returns>
    ICommand? GetCommand(string name);
    
    /// <summary>
    /// Gets all registered commands.
    /// </summary>
    /// <returns>A collection of all registered commands.</returns>
    IEnumerable<ICommand> GetAllCommands();
    
    /// <summary>
    /// Registers the standard commands.
    /// </summary>
    void RegisterStandardCommands();
}
