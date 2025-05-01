using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// The generate command is responsible for generating code from a database schema.
    /// </summary>
    public class GenerateCommand : ICommand
    {
        private readonly ICodeGenerator _codeGenerator;
        private readonly IDatabaseSchemaProvider _schemaProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateCommand"/> class.
        /// </summary>
        /// <param name="codeGenerator">The code generator to use.</param>
        /// <param name="schemaProvider">The schema provider to use.</param>
        /// <param name="logger">The logger to use.</param>
        public GenerateCommand(
            ICodeGenerator codeGenerator,
            IDatabaseSchemaProvider schemaProvider,
            ILogger logger)
        {
            _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
            _schemaProvider = schemaProvider ?? throw new ArgumentNullException(nameof(schemaProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Name => "generate";

        /// <inheritdoc/>
        public string Description => "Generate code from a database schema";

        /// <inheritdoc/>
        public string Usage => "generate --connection <connection-string> --template <template-path> --output <output-path> [--provider <provider-name>] [--namespace <namespace>] [--language <language>] [--data-annotations <true|false>] [--fluent-api <true|false>] [--navigation-properties <true|false>]";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            try
            {
                // Extract options
                options.TryGetValue("connection", out var connectionString);
                options.TryGetValue("template", out var templatePath);
                options.TryGetValue("output", out var outputPath);
                options.TryGetValue("provider", out var providerName);
                options.TryGetValue("namespace", out var namespaceName);
                options.TryGetValue("language", out var language);
                
                // Parse boolean options
                bool useDataAnnotations = ParseBoolOption(options, "data-annotations", true);
                bool useFluentApi = ParseBoolOption(options, "fluent-api", false);
                bool generateNavigationProperties = ParseBoolOption(options, "navigation-properties", true);
                
                // Set default values
                providerName ??= "SqlServer";
                namespaceName ??= "Generated";
                language ??= "CSharp";
                
                _logger.Info($"Generating code from {providerName} database...");
                _logger.Info($"Connection: {MaskConnectionString(connectionString)}");
                _logger.Info($"Template: {templatePath}");
                _logger.Info($"Output: {outputPath}");
                _logger.Info($"Namespace: {namespaceName}");
                _logger.Info($"Language: {language}");
                _logger.Info($"Data Annotations: {useDataAnnotations}");
                _logger.Info($"Fluent API: {useFluentApi}");
                _logger.Info($"Navigation Properties: {generateNavigationProperties}");
                
                // Extract schema
                _logger.Info("Extracting database schema...");
                var schema = await _schemaProvider.GetSchemaAsync();
                _logger.Info($"Schema extracted successfully with {schema.Tables.Count} tables.");
                
                // Generate code
                _logger.Info("Generating code...");
                var codeGenOptions = new CodeGenerationOptions
                {
                    Namespace = namespaceName,
                    Language = language,
                    UseDataAnnotations = useDataAnnotations,
                    UseFluentApi = useFluentApi,
                    GenerateNavigationProperties = generateNavigationProperties
                };
                
                await _codeGenerator.GenerateAsync(schema, templatePath, outputPath, codeGenOptions);
                
                _logger.Info("Code generation completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error generating code: {ex.Message}");
                _logger.Debug(ex.ToString());
                throw;
            }
        }

        /// <inheritdoc/>
        public bool ValidateOptions(IReadOnlyDictionary<string, string> options)
        {
            var requiredOptions = GetRequiredOptions();
            
            foreach (var requiredOption in requiredOptions)
            {
                if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <inheritdoc/>
        public IList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options)
        {
            var errors = new List<string>();
            var requiredOptions = GetRequiredOptions();
            
            foreach (var requiredOption in requiredOptions)
            {
                if (!options.ContainsKey(requiredOption) || string.IsNullOrWhiteSpace(options[requiredOption]))
                {
                    errors.Add($"Option '{requiredOption}' is required.");
                }
            }
            
            return errors;
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetOptions()
        {
            return new Dictionary<string, string>
            {
                ["connection"] = "Database connection string",
                ["template"] = "Template path",
                ["output"] = "Output directory path",
                ["provider"] = "Database provider (default: SqlServer)",
                ["namespace"] = "Namespace for generated code (default: Generated)",
                ["language"] = "Target language (default: CSharp)",
                ["data-annotations"] = "Use data annotations (default: true)",
                ["fluent-api"] = "Use fluent API (default: false)",
                ["navigation-properties"] = "Generate navigation properties (default: true)"
            };
        }

        /// <inheritdoc/>
        public IList<string> GetRequiredOptions()
        {
            return new List<string> { "connection", "template", "output" };
        }

        private bool ParseBoolOption(IReadOnlyDictionary<string, string> options, string key, bool defaultValue)
        {
            if (options.TryGetValue(key, out var value))
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }
                
                if (value.Equals("yes", StringComparison.OrdinalIgnoreCase) || 
                    value.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("1", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                
                if (value.Equals("no", StringComparison.OrdinalIgnoreCase) || 
                    value.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                    value.Equals("0", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            
            return defaultValue;
        }

        private string MaskConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return string.Empty;
            }
            
            // Simple masking for common password patterns in connection strings
            return connectionString
                .Replace("Password=", "Password=*****")
                .Replace("password=", "password=*****")
                .Replace("pwd=", "pwd=*****")
                .Replace("Pwd=", "Pwd=*****");
        }
    }
}
