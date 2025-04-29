namespace EzDbCodeGen.Core.Logging;

/// <summary>
/// Represents a factory for creating loggers.
/// </summary>
public interface ILoggerFactory
{
    /// <summary>
    /// Creates a logger with the specified name.
    /// </summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>A logger.</returns>
    ILogger CreateLogger(string name);
    
    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <returns>A logger.</returns>
    ILogger CreateLogger<T>();
    
    /// <summary>
    /// Creates a logger for the specified type.
    /// </summary>
    /// <param name="type">The type to create a logger for.</param>
    /// <returns>A logger.</returns>
    ILogger CreateLogger(Type type);
    
    /// <summary>
    /// Sets the minimum log level for all loggers.
    /// </summary>
    /// <param name="level">The minimum log level.</param>
    void SetMinimumLogLevel(LogLevel level);
    
    /// <summary>
    /// Gets the minimum log level for all loggers.
    /// </summary>
    /// <returns>The minimum log level.</returns>
    LogLevel GetMinimumLogLevel();
    
    /// <summary>
    /// Adds a log provider.
    /// </summary>
    /// <param name="provider">The log provider to add.</param>
    void AddProvider(ILogProvider provider);
    
    /// <summary>
    /// Gets all log providers.
    /// </summary>
    /// <returns>A list of log providers.</returns>
    IReadOnlyList<ILogProvider> GetProviders();
}
