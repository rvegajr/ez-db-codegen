namespace EzDbCodeGen.CodeGen.Interfaces;

/// <summary>
/// Defines the possible log levels for code generation logging.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Debug-level messages for detailed troubleshooting.
    /// </summary>
    Debug = 0,
    
    /// <summary>
    /// Informational messages about normal operation.
    /// </summary>
    Info = 1,
    
    /// <summary>
    /// Warning messages about potential issues.
    /// </summary>
    Warning = 2,
    
    /// <summary>
    /// Error messages about issues that prevent normal operation.
    /// </summary>
    Error = 3,
    
    /// <summary>
    /// Exception messages about critical failures.
    /// </summary>
    Exception = 4
}
