using System.Threading.Tasks;

namespace EzDbCodeGen.Interfaces.Cli
{
    /// <summary>
    /// Defines a handler for command-line commands.
    /// </summary>
    public interface ICommandLineHandler
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the help text for the command.
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// Gets a value indicating whether this command is hidden from help listings.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// Gets a value indicating whether this command requires a connection string.
        /// </summary>
        bool RequiresConnectionString { get; }

        /// <summary>
        /// Validates the command arguments.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <returns>True if the arguments are valid, false otherwise.</returns>
        bool ValidateArgs(string[] args);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <returns>The exit code. 0 indicates success, non-zero indicates failure.</returns>
        Task<int> ExecuteAsync(string[] args);

        /// <summary>
        /// Gets example usage of the command.
        /// </summary>
        /// <returns>Example usage of the command.</returns>
        string GetExampleUsage();
    }
}
