using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.Performance
{
    /// <summary>
    /// Defines a tracker for measuring performance.
    /// </summary>
    public interface IPerformanceTracker
    {
        /// <summary>
        /// Starts tracking performance for an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        void StartOperation(string operationName);

        /// <summary>
        /// Ends tracking performance for an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The elapsed milliseconds for the operation.</returns>
        long EndOperation(string operationName);

        /// <summary>
        /// Tracks the performance of an action.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="action">The action to track.</param>
        /// <returns>The elapsed milliseconds for the operation.</returns>
        long TrackOperation(string operationName, Action action);

        /// <summary>
        /// Tracks the performance of a function.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="operationName">The name of the operation.</param>
        /// <param name="func">The function to track.</param>
        /// <returns>A tuple containing the function result and the elapsed milliseconds.</returns>
        (T Result, long ElapsedMilliseconds) TrackOperation<T>(string operationName, Func<T> func);

        /// <summary>
        /// Gets the elapsed milliseconds for an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The elapsed milliseconds for the operation.</returns>
        long GetElapsedMilliseconds(string operationName);

        /// <summary>
        /// Gets the elapsed seconds for an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The elapsed seconds for the operation.</returns>
        double GetElapsedSeconds(string operationName);

        /// <summary>
        /// Gets all tracked operations.
        /// </summary>
        /// <returns>A collection of operation names.</returns>
        IEnumerable<string> GetOperations();

        /// <summary>
        /// Gets performance metrics for all operations.
        /// </summary>
        /// <returns>A dictionary mapping operation names to elapsed milliseconds.</returns>
        IDictionary<string, long> GetAllMetrics();

        /// <summary>
        /// Clears all tracked operations.
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Gets a formatted report of all performance metrics.
        /// </summary>
        /// <returns>A formatted report of all performance metrics.</returns>
        string GetReport();

        /// <summary>
        /// Gets memory usage at the start of an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The memory usage in bytes.</returns>
        long GetStartMemoryUsage(string operationName);

        /// <summary>
        /// Gets memory usage at the end of an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The memory usage in bytes.</returns>
        long GetEndMemoryUsage(string operationName);

        /// <summary>
        /// Gets memory usage difference for an operation.
        /// </summary>
        /// <param name="operationName">The name of the operation.</param>
        /// <returns>The memory usage difference in bytes.</returns>
        long GetMemoryUsageDifference(string operationName);
    }
}
