namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines an interface for logging during the code generation process.
/// </summary>
public interface ICodeGenerationLogger
{
    /// <summary>
    /// Logs a debug message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Debug(string message);
    
    /// <summary>
    /// Logs a formatted debug message.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    void Debug(string format, params object[] args);
    
    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Info(string message);
    
    /// <summary>
    /// Logs a formatted information message.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    void Info(string format, params object[] args);
    
    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Warning(string message);
    
    /// <summary>
    /// Logs a formatted warning message.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    void Warning(string format, params object[] args);
    
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Error(string message);
    
    /// <summary>
    /// Logs a formatted error message.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    void Error(string format, params object[] args);
    
    /// <summary>
    /// Logs an exception.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    void Exception(Exception exception);
    
    /// <summary>
    /// Logs an exception with a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">The exception to log.</param>
    void Exception(string message, Exception exception);
    
    /// <summary>
    /// Gets or sets the minimum log level to record.
    /// </summary>
    LogLevel MinimumLogLevel { get; set; }
    
    /// <summary>
    /// Gets all log entries.
    /// </summary>
    /// <returns>A collection of log entries.</returns>
    IReadOnlyCollection<LogEntry> GetLogEntries();
    
    /// <summary>
    /// Clears all log entries.
    /// </summary>
    void ClearLog();
}
