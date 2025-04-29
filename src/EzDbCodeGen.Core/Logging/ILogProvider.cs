namespace EzDbCodeGen.Core.Logging;

/// <summary>
/// Represents a provider that writes log messages to a specific destination.
/// </summary>
public interface ILogProvider
{
    /// <summary>
    /// Writes a log message to the provider's destination.
    /// </summary>
    /// <param name="level">The level of the log message.</param>
    /// <param name="loggerName">The name of the logger.</param>
    /// <param name="message">The log message.</param>
    /// <param name="exception">The exception, if any.</param>
    void Write(LogLevel level, string loggerName, string message, Exception? exception = null);
    
    /// <summary>
    /// Gets or sets the minimum log level for this provider.
    /// </summary>
    LogLevel MinimumLogLevel { get; set; }
    
    /// <summary>
    /// Gets a value indicating whether this provider is enabled.
    /// </summary>
    bool IsEnabled { get; }
    
    /// <summary>
    /// Enables the provider.
    /// </summary>
    void Enable();
    
    /// <summary>
    /// Disables the provider.
    /// </summary>
    void Disable();
    
    /// <summary>
    /// Gets the name of the provider.
    /// </summary>
    string Name { get; }
}
