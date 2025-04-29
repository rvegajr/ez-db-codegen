namespace EzDbCodeGen.Core.Cli;

/// <summary>
/// Represents a command that can be executed from the command-line interface.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command with the specified options.
    /// </summary>
    /// <param name="options">The command options.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ExecuteAsync(IReadOnlyDictionary<string, string> options);
    
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the usage of the command.
    /// </summary>
    string Usage { get; }
    
    /// <summary>
    /// Gets the options supported by the command.
    /// </summary>
    /// <returns>A dictionary of option names and descriptions.</returns>
    IReadOnlyDictionary<string, string> GetOptions();
    
    /// <summary>
    /// Validates the options for the command.
    /// </summary>
    /// <param name="options">The command options.</param>
    /// <returns>True if the options are valid, false otherwise.</returns>
    bool ValidateOptions(IReadOnlyDictionary<string, string> options);
    
    /// <summary>
    /// Gets the validation errors for the options.
    /// </summary>
    /// <param name="options">The command options.</param>
    /// <returns>A list of validation errors.</returns>
    IReadOnlyList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options);
    
    /// <summary>
    /// Gets the required options for the command.
    /// </summary>
    /// <returns>A list of required option names.</returns>
    IReadOnlyList<string> GetRequiredOptions();
}
