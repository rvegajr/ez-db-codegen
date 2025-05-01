using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Schema.Interfaces;

namespace EzDbCodeGen.Schema
{
    /// <summary>
    /// Factory for creating relationship detectors.
    /// </summary>
    public class RelationshipDetectorFactory : IRelationshipDetectorFactory
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, Func<IRelationshipDetector>> _detectorFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipDetectorFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public RelationshipDetectorFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _detectorFactories = new Dictionary<string, Func<IRelationshipDetector>>(StringComparer.OrdinalIgnoreCase);
            
            // Register standard detectors
            RegisterStandardDetectors();
        }

        /// <inheritdoc/>
        public IRelationshipDetector CreateRelationshipDetector(string name = "Default")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Default";
            }

            if (!_detectorFactories.TryGetValue(name, out var factory))
            {
                throw new KeyNotFoundException($"Relationship detector '{name}' not found. Available detectors: {string.Join(", ", _detectorFactories.Keys)}");
            }

            return factory();
        }

        /// <inheritdoc/>
        public void RegisterDetector(string name, Func<IRelationshipDetector> factory)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Detector name cannot be empty.", nameof(name));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _detectorFactories[name] = factory;
            _logger.LogDebug($"Registered relationship detector: {name}");
        }

        /// <inheritdoc/>
        public bool IsDetectorRegistered(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            return _detectorFactories.ContainsKey(name);
        }

        /// <inheritdoc/>
        public IEnumerable<string> GetRegisteredDetectors()
        {
            return _detectorFactories.Keys;
        }

        private void RegisterStandardDetectors()
        {
            // Register the default detector
            RegisterDetector("Default", () => new RelationshipDetector(_logger));
            
            // Register the advanced detector (which is the same for now, but could be extended)
            RegisterDetector("Advanced", () => new RelationshipDetector(_logger));
        }
    }
}
