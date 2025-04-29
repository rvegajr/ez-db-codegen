namespace EzDbCodeGen.Core.Logging;

/// <summary>
/// Represents a logger for recording messages and events.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message);
    
    /// <summary>
    /// Logs a debug message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Debug(string message, Exception exception);
    
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message);
    
    /// <summary>
    /// Logs an information message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Info(string message, Exception exception);
    
    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warn(string message);
    
    /// <summary>
    /// Logs a warning message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Warn(string message, Exception exception);
    
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message);
    
    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Error(string message, Exception exception);
    
    /// <summary>
    /// Logs a fatal message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Fatal(string message);
    
    /// <summary>
    /// Logs a fatal message with an exception.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Fatal(string message, Exception exception);
    
    /// <summary>
    /// Gets or sets the minimum log level.
    /// </summary>
    LogLevel MinimumLogLevel { get; set; }
    
    /// <summary>
    /// Gets the name of the logger.
    /// </summary>
    string Name { get; }
}
