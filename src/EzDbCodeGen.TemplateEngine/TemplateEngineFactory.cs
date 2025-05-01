using System;
using System.Collections.Generic;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.TemplateEngine;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Factory for creating template engines.
    /// </summary>
    public class TemplateEngineFactory : ITemplateEngineFactory
    {
        private readonly ICodeGenerationLogger _logger;
        private readonly Dictionary<TemplateEngineType, Func<TemplateEngineOptions, ITemplateEngine>> _engineFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEngineFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public TemplateEngineFactory(ICodeGenerationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _engineFactories = new Dictionary<TemplateEngineType, Func<TemplateEngineOptions, ITemplateEngine>>();
            
            // Register standard engines
            RegisterStandardEngines();
        }

        /// <inheritdoc/>
        public ITemplateEngine CreateEngine(TemplateEngineType engineType)
        {
            return CreateEngine(engineType, new TemplateEngineOptions());
        }
        
        /// <inheritdoc/>
        public ITemplateEngine CreateEngine(TemplateEngineType engineType, TemplateEngineOptions options)
        {
            if (!_engineFactories.TryGetValue(engineType, out var factory))
            {
                throw new KeyNotFoundException($"Template engine '{engineType}' not found.");
            }

            // Create default options if none provided
            options ??= new TemplateEngineOptions();

            try
            {
                _logger.LogDebug($"Creating template engine: {engineType}");
                return factory(options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating template engine '{engineType}': {ex.Message}");
                throw new TemplateEngineCreationException($"Error creating template engine '{engineType}'", ex);
            }
        }

        /// <inheritdoc/>
        public void RegisterEngineFactory(TemplateEngineType engineType, Func<TemplateEngineOptions, ITemplateEngine> factoryMethod)
        {
            if (factoryMethod == null)
            {
                throw new ArgumentNullException(nameof(factoryMethod));
            }

            _engineFactories[engineType] = factoryMethod;
            _logger.LogDebug($"Registered template engine: {engineType}");
        }

        /// <inheritdoc/>
        public bool IsEngineRegistered(TemplateEngineType engineType)
        {
            return _engineFactories.ContainsKey(engineType);
        }

        /// <inheritdoc/>
        public IReadOnlyList<TemplateEngineType> GetAvailableEngines()
        {
            return new List<TemplateEngineType>(_engineFactories.Keys);
        }

        private void RegisterStandardEngines()
        {
            // Register Handlebars engine
            RegisterEngineFactory(TemplateEngineType.Handlebars, options => 
                new HandlebarsTemplateEngine(_logger, options));
            
            // Additional engines can be registered here as they are implemented
            // RegisterEngineFactory(TemplateEngineType.Razor, options => new RazorTemplateEngine(_logger, options));
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during template engine creation.
    /// </summary>
    public class TemplateEngineCreationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEngineCreationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TemplateEngineCreationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEngineCreationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TemplateEngineCreationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
