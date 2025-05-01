using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.TemplateEngine;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Implements a template engine using Handlebars.Net.
    /// </summary>
    public class HandlebarsTemplateEngine : ITemplateEngine
    {
        private readonly ICodeGenerationLogger _logger;
        private readonly TemplateEngineOptions _options;
        private readonly IHandlebars _handlebars;
        private readonly Dictionary<string, ICompiledTemplate> _compiledTemplates = new();
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
        public ICompiledTemplate Compile(string templateContent)
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
        public string Execute(ICompiledTemplate template, object dataModel)
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
        public void RegisterHelper(string name, Delegate helper)
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
                _logger.LogDebug($"Registering helper: {name}");
                _handlebars.RegisterHelper(name, helper);
                _registeredHelpers.Add(helper);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering helper {name}: {ex.Message}");
                throw new HelperRegistrationException($"Error registering helper {name}", ex);
            }
        }

        /// <inheritdoc/>
        public void RegisterBlockHelper(string name, Delegate helper)
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
                _logger.LogDebug($"Registering block helper: {name}");
                _handlebars.RegisterHelper(name, helper);
                _registeredHelpers.Add(helper);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering block helper {name}: {ex.Message}");
                throw new HelperRegistrationException($"Error registering block helper {name}", ex);
            }
        }

        /// <inheritdoc/>
        public void RegisterPartial(string name, string template)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            try
            {
                _logger.LogDebug($"Registering partial: {name}");
                _handlebars.RegisterTemplate(name, template);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error registering partial {name}: {ex.Message}");
                throw new PartialRegistrationException($"Error registering partial {name}", ex);
            }
        }

        private HandlebarsConfiguration ConfigureHandlebars()
        {
            var config = new HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = _options.ThrowOnUnresolvedBindings,
                NoEscape = _options.DisableEncoding
            };

            return config;
        }

        private void RegisterStandardHelpers()
        {
            // Register standard helpers for string operations, comparison, etc.
            _logger.LogDebug("Registering standard helpers");
            
            // String helpers
            RegisterHelper("toLowerCase", (string value) => value?.ToLowerInvariant());
            RegisterHelper("toUpperCase", (string value) => value?.ToUpperInvariant());
            RegisterHelper("capitalize", (string value) => 
                string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value.Substring(1));
            
            // Comparison helpers
            RegisterHelper("equals", (object left, object right) => Equals(left, right));
            RegisterHelper("notEquals", (object left, object right) => !Equals(left, right));
            RegisterHelper("gt", (double left, double right) => left > right);
            RegisterHelper("gte", (double left, double right) => left >= right);
            RegisterHelper("lt", (double left, double right) => left < right);
            RegisterHelper("lte", (double left, double right) => left <= right);
            
            // Conditional helpers
            RegisterHelper("if", (bool condition, string trueValue, string falseValue) => condition ? trueValue : falseValue);
            
            // Collection helpers
            RegisterHelper("count", (IEnumerable<object> collection) => collection?.Count() ?? 0);
            RegisterHelper("join", (IEnumerable<object> collection, string separator) => 
                collection == null ? string.Empty : string.Join(separator ?? ",", collection));
        }
    }

    /// <summary>
    /// Implementation of ICompiledTemplate for Handlebars.Net.
    /// </summary>
    public class CompiledTemplate : ICompiledTemplate
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
        /// Executes the compiled template with the given data.
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
