using System;
using System.Collections.Generic;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.TemplateEngine;

namespace EzDbCodeGen.CodeGeneration
{
    /// <summary>
    /// Factory for creating code generators.
    /// </summary>
    public class CodeGeneratorFactory : ICodeGeneratorFactory
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngineFactory _templateEngineFactory;
        private readonly ITemplateProcessorFactory _templateProcessorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="templateEngineFactory">The template engine factory to use.</param>
        /// <param name="templateProcessorFactory">The template processor factory to use.</param>
        public CodeGeneratorFactory(
            ILogger logger,
            ITemplateEngineFactory templateEngineFactory,
            ITemplateProcessorFactory templateProcessorFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _templateEngineFactory = templateEngineFactory ?? throw new ArgumentNullException(nameof(templateEngineFactory));
            _templateProcessorFactory = templateProcessorFactory ?? throw new ArgumentNullException(nameof(templateProcessorFactory));
        }

        /// <inheritdoc/>
        public ICodeGenerator CreateCodeGenerator(CodeGeneratorOptions options = null)
        {
            // Create default options if none provided
            options ??= new CodeGeneratorOptions
            {
                Language = "CSharp",
                EngineType = "Handlebars",
                TemplateFormat = "hbs"
            };

            // Create the template engine
            var templateEngineOptions = new TemplateEngineOptions
            {
                CacheTemplates = true,
                EnablePartials = true,
                EnableCustomHelpers = true
            };
            
            var templateEngine = _templateEngineFactory.CreateTemplateEngine(options.EngineType, templateEngineOptions);

            // Create the template processor
            var templateProcessorOptions = new TemplateProcessorOptions
            {
                OutputEncoding = "utf-8",
                LineEnding = LineEndingType.PlatformDefault,
                OutputExtension = GetOutputExtension(options.Language),
                EnableFiltering = true
            };
            
            var templateProcessor = _templateProcessorFactory.CreateTemplateProcessor(
                templateEngine, 
                templateProcessorOptions);

            // Create the code generator
            var codeGenOptions = new CodeGenerationOptions
            {
                Language = options.Language,
                UseDataAnnotations = options.UseDataAnnotations,
                UseFluentApi = options.UseFluentApi,
                GenerateNavigationProperties = options.GenerateNavigationProperties,
                Namespace = options.Namespace ?? "Generated"
            };

            return new CodeGenerator(
                _logger,
                templateProcessor,
                codeGenOptions);
        }

        private string GetOutputExtension(string language)
        {
            return language?.ToLowerInvariant() switch
            {
                "csharp" => "cs",
                "typescript" => "ts",
                "javascript" => "js",
                "java" => "java",
                "python" => "py",
                "sql" => "sql",
                "html" => "html",
                "css" => "css",
                _ => "txt"
            };
        }
    }
}
