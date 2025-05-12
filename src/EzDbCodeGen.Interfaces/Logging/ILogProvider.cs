using System;

namespace EzDbCodeGen.Interfaces.Logging
{
    /// <summary>
    /// Defines a provider for logging.
    /// </summary>
    public interface ILogProvider
    {
        /// <summary>
        /// Logs a message at the debug level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Debug(string message);

        /// <summary>
        /// Logs a message at the debug level with formatting.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The format arguments.</param>
        void Debug(string messageFormat, params object[] args);

        /// <summary>
        /// Logs a message at the information level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Logs a message at the information level with formatting.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The format arguments.</param>
        void Info(string messageFormat, params object[] args);

        /// <summary>
        /// Logs a message at the warning level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Warning(string message);

        /// <summary>
        /// Logs a message at the warning level with formatting.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The format arguments.</param>
        void Warning(string messageFormat, params object[] args);

        /// <summary>
        /// Logs a message at the error level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs a message at the error level with formatting.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The format arguments.</param>
        void Error(string messageFormat, params object[] args);

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">An optional message to accompany the exception.</param>
        void Exception(Exception exception, string message = null);

        /// <summary>
        /// Gets or sets a value indicating whether to show debug messages.
        /// </summary>
        bool ShowDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show verbose messages.
        /// </summary>
        bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show timestamps in log messages.
        /// </summary>
        bool ShowTimestamps { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use colors in console output.
        /// </summary>
        bool UseColors { get; set; }
    }
}
