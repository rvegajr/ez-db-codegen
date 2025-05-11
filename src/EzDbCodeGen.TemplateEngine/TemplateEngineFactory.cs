using System;
using System.Collections.Generic;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Factory for creating template engines.
    /// </summary>
    public class TemplateEngineFactory : EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngineFactory
    {
        private readonly EzDbCodeGen.Core.Interfaces.Logging.ICodeGenerationLogger _logger;
        private readonly Dictionary<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType, Func<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions, EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine>> _engineFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateEngineFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public TemplateEngineFactory(EzDbCodeGen.Core.Interfaces.Logging.ICodeGenerationLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _engineFactories = new Dictionary<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType, Func<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions, EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine>>();
            
            // Register standard engines
            RegisterStandardEngines();
        }

        /// <inheritdoc/>
        public EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine CreateEngine(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType engineType)
        {
            return CreateEngine(engineType, new EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions());
        }
        
        /// <inheritdoc/>
        public EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine CreateEngine(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType engineType, EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions options)
        {
            if (!_engineFactories.TryGetValue(engineType, out var factory))
            {
                throw new KeyNotFoundException($"Template engine '{engineType}' not found.");
            }

            // Create default options if none provided
            options ??= new EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions();

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
        public void RegisterEngineFactory(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType engineType, Func<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineOptions, EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine> factoryMethod)
        {
            if (factoryMethod == null)
            {
                throw new ArgumentNullException(nameof(factoryMethod));
            }

            _engineFactories[engineType] = factoryMethod;
            _logger.LogDebug($"Registered template engine: {engineType}");
        }
        
        /// <inheritdoc/>
        public bool IsEngineRegistered(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType engineType)
        {
            return _engineFactories.ContainsKey(engineType);
        }
        
        /// <inheritdoc/>
        public IReadOnlyList<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType> GetAvailableEngines()
        {
            return new List<EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType>(_engineFactories.Keys);
        }

        private void RegisterStandardEngines()
        {
            // Register Handlebars engine
            RegisterEngineFactory(EzDbCodeGen.TemplateEngine.Interfaces.TemplateEngineType.Handlebars, options => 
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
