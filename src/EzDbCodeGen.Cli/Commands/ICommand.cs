using System.Collections.Generic;
using System.Threading.Tasks;

namespace EzDbCodeGen.Cli.Commands;

/// <summary>
/// Defines a command that can be executed from the command line.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a short description of the command.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the usage instructions for the command.
    /// </summary>
    string Usage { get; }
    
    /// <summary>
    /// Gets examples of how to use the command.
    /// </summary>
    IEnumerable<string> Examples { get; }
    
    /// <summary>
    /// Executes the command with the specified arguments.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(string[] args);
}
