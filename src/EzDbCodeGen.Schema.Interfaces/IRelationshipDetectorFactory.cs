using System;
using System.Collections.Generic;

namespace EzDbCodeGen.Schema.Interfaces
{
    /// <summary>
    /// Factory for creating relationship detectors.
    /// </summary>
    public interface IRelationshipDetectorFactory
    {
        /// <summary>
        /// Creates a relationship detector with the specified name.
        /// </summary>
        /// <param name="name">The name of the detector to create. If null or empty, the default detector is created.</param>
        /// <returns>A relationship detector.</returns>
        IRelationshipDetector CreateRelationshipDetector(string name = "Default");

        /// <summary>
        /// Registers a detector factory with the specified name.
        /// </summary>
        /// <param name="name">The name of the detector.</param>
        /// <param name="factory">The factory function that creates the detector.</param>
        void RegisterDetector(string name, Func<IRelationshipDetector> factory);

        /// <summary>
        /// Determines whether a detector with the specified name is registered.
        /// </summary>
        /// <param name="name">The name of the detector.</param>
        /// <returns>True if the detector is registered; otherwise, false.</returns>
        bool IsDetectorRegistered(string name);

        /// <summary>
        /// Gets the names of all registered detectors.
        /// </summary>
        /// <returns>The names of all registered detectors.</returns>
        IEnumerable<string> GetRegisteredDetectors();
    }
}
