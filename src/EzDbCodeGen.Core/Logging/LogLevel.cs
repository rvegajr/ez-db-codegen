namespace EzDbCodeGen.Core.Logging;

/// <summary>
/// Represents the level of a log message.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Debug level.
    /// </summary>
    Debug = 0,
    
    /// <summary>
    /// Information level.
    /// </summary>
    Info = 1,
    
    /// <summary>
    /// Warning level.
    /// </summary>
    Warn = 2,
    
    /// <summary>
    /// Error level.
    /// </summary>
    Error = 3,
    
    /// <summary>
    /// Fatal level.
    /// </summary>
    Fatal = 4,
    
    /// <summary>
    /// No logging.
    /// </summary>
    None = 5
}
