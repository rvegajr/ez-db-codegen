namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Represents a log entry for code generation logging.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the timestamp of the log entry.
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Gets or sets the log level of the entry.
    /// </summary>
    public LogLevel Level { get; set; }
    
    /// <summary>
    /// Gets or sets the message of the log entry.
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Gets or sets the associated exception, if any.
    /// </summary>
    public Exception? Exception { get; set; }
    
    /// <summary>
    /// Gets or sets the context information for the log entry.
    /// </summary>
    public IDictionary<string, object>? Context { get; set; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    public LogEntry()
    {
        Timestamp = DateTime.UtcNow;
        Message = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with a message and log level.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    public LogEntry(LogLevel level, string message)
    {
        Timestamp = DateTime.UtcNow;
        Level = level;
        Message = message;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with a message, log level, and exception.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The log message.</param>
    /// <param name="exception">The associated exception.</param>
    public LogEntry(LogLevel level, string message, Exception exception)
    {
        Timestamp = DateTime.UtcNow;
        Level = level;
        Message = message;
        Exception = exception;
    }
    
    /// <summary>
    /// Gets a formatted representation of the log entry.
    /// </summary>
    /// <returns>A formatted log entry string.</returns>
    public override string ToString()
    {
        var exceptionInfo = Exception != null ? $" | Exception: {Exception.GetType().Name}: {Exception.Message}" : string.Empty;
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level}] {Message}{exceptionInfo}";
    }
}
