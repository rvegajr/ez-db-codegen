using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using EzDbCodeGen.Core.TemplateEngine;
using EzDbCodeGen.CodeGen.Interfaces;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Factory for creating template processors.
    /// </summary>
    public class TemplateProcessorFactory : ITemplateProcessorFactory
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngineFactory _engineFactory;
        private readonly List<IFilterProvider> _filterProviders = new();
        private readonly string _basePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessorFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="engineFactory">The template engine factory.</param>
        /// <param name="basePath">Base path for templates.</param>
        public TemplateProcessorFactory(
            ILogger logger, 
            ITemplateEngineFactory engineFactory,
            string basePath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
            _basePath = basePath;
        }

        /// <inheritdoc/>
        public ITemplateProcessor CreateProcessor(TemplateEngineType engineType)
        {
            return CreateProcessor(engineType, new TemplateEngineOptions());
        }

        /// <inheritdoc/>
        public ITemplateProcessor CreateProcessor(TemplateEngineType engineType, TemplateEngineOptions options)
        {
            var templateEngine = _engineFactory.CreateEngine(engineType, options);
            return CreateProcessor(templateEngine);
        }

        /// <inheritdoc/>
        public ITemplateProcessor CreateProcessor(ITemplateEngine templateEngine)
        {
            if (templateEngine == null)
            {
                throw new ArgumentNullException(nameof(templateEngine));
            }

            _logger.LogDebug($"Creating template processor with engine: {templateEngine.GetType().Name}");
            return new TemplateProcessor(templateEngine, _logger, _basePath);
        }

        /// <inheritdoc/>
        public ITemplateProcessor CreateFilteredProcessor(TemplateEngineType engineType, IEnumerable<string> filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            // Create a basic processor
            var processor = CreateProcessor(engineType);
            
            // Apply filters
            var filtersList = filters.ToList();
            if (filtersList.Count > 0)
            {
                _logger.LogDebug($"Creating filtered template processor with {filtersList.Count} filters");
                
                // Create filtered processor (conceptual - actual implementation would depend on filter mechanism)
                // For now, we'll just return the basic processor as the filtering mechanism is being developed
                // The actual implementation would wrap the processor with filter capabilities
            }
            
            return processor;
        }

        /// <inheritdoc/>
        public ITemplateProcessor CreateDifferentialProcessor(TemplateEngineType engineType)
        {
            // Create a basic processor with the specified engine
            var baseProcessor = CreateProcessor(engineType);
            
            _logger.LogDebug("Creating differential template processor");
            
            // Wrap in a differential processor
            // Since we already have our new TemplateProcessor implementation, we'll need to develop 
            // the differential processor capability separately
            // For now, we'll just return the basic processor as the differential mechanism is being developed
            
            return baseProcessor;
        }
        
        /// <summary>
        /// Registers a filter provider with the factory.
        /// </summary>
        /// <param name="filterProvider">The filter provider to register.</param>
        /// <returns>This factory instance for chaining.</returns>
        public ITemplateProcessorFactory RegisterFilterProvider(IFilterProvider filterProvider)
        {
            if (filterProvider == null)
            {
                throw new ArgumentNullException(nameof(filterProvider));
            }
            
            _filterProviders.Add(filterProvider);
            _logger.LogDebug($"Registered filter provider: {filterProvider.GetType().Name}");
            
            return this;
        }
    }
    
    /// <summary>
    /// Interface for providing template filters.
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Gets whether the provider can create a filter for the specified name.
        /// </summary>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>True if the provider can create a filter for the name; otherwise, false.</returns>
        bool CanCreateFilter(string filterName);
        
        /// <summary>
        /// Creates a filter for the specified name.
        /// </summary>
        /// <param name="filterName">The name of the filter.</param>
        /// <returns>The created filter.</returns>
        object CreateFilter(string filterName);
    }
}
