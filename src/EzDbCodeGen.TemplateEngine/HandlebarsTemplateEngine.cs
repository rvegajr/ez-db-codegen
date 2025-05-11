using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EzDbCodeGen.Core.Interfaces.Logging;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using EzDbCodeGen.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Implements a template engine using Handlebars.Net.
    /// </summary>
    public class HandlebarsTemplateEngine : EzDbCodeGen.TemplateEngine.Interfaces.ITemplateEngine
    {
        private readonly ICodeGenerationLogger _logger;
        private readonly TemplateEngineOptions _options;
        private readonly IHandlebars _handlebars;
        private readonly Dictionary<string, EzDbCodeGen.TemplateEngine.Interfaces.ICompiledTemplate> _compiledTemplates = new();
        private readonly List<object> _registeredHelpers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlebarsTemplateEngine"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="options">The options to use.</param>
        public HandlebarsTemplateEngine(ICodeGenerationLogger logger, TemplateEngineOptions options = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? new TemplateEngineOptions();
            
            // Configure Handlebars instance
            _handlebars = Handlebars.Create(ConfigureHandlebars());
            
            // Register the standard helpers if enabled
            if (_options.RegisterStandardHelpers)
            {
                RegisterStandardHelpers();
            }
        }

        /// <inheritdoc/>
        public EzDbCodeGen.TemplateEngine.Interfaces.ICompiledTemplate Compile(string templateContent)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                throw new ArgumentNullException(nameof(templateContent));
            }

            try
            {
                _logger.LogDebug($"Compiling template content");
                var compiled = _handlebars.Compile(templateContent);
                var template = new CompiledTemplate(compiled);
                
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error compiling template: {ex.Message}");
                throw new TemplateCompilationException("Error compiling template", ex);
            }
        }

        /// <inheritdoc/>
        public string Execute(EzDbCodeGen.TemplateEngine.Interfaces.ICompiledTemplate template, object dataModel)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            try
            {
                var compiledTemplate = template as CompiledTemplate;
                if (compiledTemplate == null)
                {
                    throw new InvalidCastException("Template must be a CompiledTemplate instance created by this engine");
                }

                _logger.LogDebug("Executing compiled template");
                return compiledTemplate.Execute(dataModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error executing template: {ex.Message}");
                throw new TemplateRenderingException("Error executing template", ex);
            }
        }

        /// <inheritdoc/>
        public string Process(string templateContent, object dataModel)
        {
            var template = Compile(templateContent);
            return Execute(template, dataModel);
        }

        /// <inheritdoc/>
        public void RegisterHelper(string name, Action<EncodedTextWriter, Context, Arguments> helper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            try
            {
                _logger.LogDebug($"Registering helper '{name}'");
                _handlebars.RegisterHelper(name, (writer, context, arguments) => helper(writer, context, arguments));
                _registeredHelpers.Add(helper);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering helper '{name}': {ex.Message}");
                throw new HelperRegistrationException($"Error registering helper '{name}'", ex);
            }
        }

        /// <inheritdoc/>
        public void RegisterBlockHelper(string name, Action<EncodedTextWriter, BlockHelperOptions, Context, Arguments> helper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (helper == null)
            {
                throw new ArgumentNullException(nameof(helper));
            }

            try
            {
                _logger.LogDebug($"Registering block helper '{name}'");
                _handlebars.RegisterHelper(name, (writer, options, context, arguments) => helper(writer, options, context, arguments));
                _registeredHelpers.Add(helper);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering block helper '{name}': {ex.Message}");
                throw new HelperRegistrationException($"Error registering block helper '{name}'", ex);
            }
        }

        /// <inheritdoc/>
        public void RegisterPartial(string name, string partialContent)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (partialContent == null)
            {
                throw new ArgumentNullException(nameof(partialContent));
            }

            try
            {
                _logger.LogDebug($"Registering partial: {name}");
                _handlebars.RegisterTemplate(name, partialContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering partial '{name}': {ex.Message}");
                throw new PartialRegistrationException($"Error registering partial '{name}'", ex);
            }
        }

        private HandlebarsConfiguration ConfigureHandlebars()
        {
            return new HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = true,
                NoEscape = false
            };
        }

        private void RegisterStandardHelpers()
        {
            // Register standard helpers here
            // TODO: Fix helper registrations
            /*
            var stringFormatHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsStringFormatHelpers();
            stringFormatHelpers.RegisterHelpers(this);
            
            var codeFormatHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsCodeFormatHelpers();
            codeFormatHelpers.RegisterHelpers(this);
            
            var documentationHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsDocumentationHelpers();
            documentationHelpers.RegisterHelpers(this);
            
            var layoutHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsLayoutHelpers();
            layoutHelpers.RegisterHelpers(this);
            
            var typeConversionHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsTypeConversionHelpers();
            typeConversionHelpers.RegisterHelpers(this);
            
            var testHelpers = new EzDbCodeGen.TemplateEngine.Helpers.HandlebarsTestHelpers();
            testHelpers.RegisterHelpers(this);
            */
        }
    }

    /// <summary>
    /// Implementation of ICompiledTemplate for Handlebars.Net.
    /// </summary>
    internal class CompiledTemplate : EzDbCodeGen.TemplateEngine.Interfaces.ICompiledTemplate
    {
        private readonly HandlebarsTemplate<object, object> _compiledTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledTemplate"/> class.
        /// </summary>
        /// <param name="compiledTemplate">The compiled Handlebars template.</param>
        public CompiledTemplate(HandlebarsTemplate<object, object> compiledTemplate)
        {
            _compiledTemplate = compiledTemplate ?? throw new ArgumentNullException(nameof(compiledTemplate));
        }

        /// <summary>
        /// Renders the template with the provided data.
        /// </summary>
        /// <param name="data">Data to use in rendering</param>
        /// <returns>The rendered template</returns>
        public string Render(object data)
        {
            try
            {
                return _compiledTemplate(data);
            }
            catch (Exception ex)
            {
                throw new TemplateRenderingException("Error executing template", ex);
            }
        }

        /// <summary>
        /// Executes the template with the provided data.
        /// </summary>
        /// <param name="data">The data to use when executing the template.</param>
        /// <returns>The result of the template execution.</returns>
        internal string Execute(object data)
        {
            try
            {
                return _compiledTemplate(data);
            }
            catch (Exception ex)
            {
                throw new TemplateRenderingException("Error executing template", ex);
            }
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during template compilation.
    /// </summary>
    public class TemplateCompilationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateCompilationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TemplateCompilationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateCompilationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TemplateCompilationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during template rendering.
    /// </summary>
    public class TemplateRenderingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateRenderingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TemplateRenderingException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateRenderingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TemplateRenderingException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during helper registration.
    /// </summary>
    public class HelperRegistrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelperRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public HelperRegistrationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public HelperRegistrationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an error occurs during partial registration.
    /// </summary>
    public class PartialRegistrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public PartialRegistrationException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialRegistrationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PartialRegistrationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
