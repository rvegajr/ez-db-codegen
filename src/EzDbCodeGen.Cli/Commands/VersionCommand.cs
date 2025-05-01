using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Cli;
using EzDbCodeGen.Core.Logging;

namespace EzDbCodeGen.Cli.Commands
{
    /// <summary>
    /// Command for displaying version information.
    /// </summary>
    public class VersionCommand : ICommand
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionCommand"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public VersionCommand(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Name => "version";

        /// <inheritdoc/>
        public string Description => "Display the version information";

        /// <inheritdoc/>
        public string Usage => "version";

        /// <inheritdoc/>
        public async Task ExecuteAsync(IReadOnlyDictionary<string, string> options)
        {
            _logger.Info("EzDbCodeGen - Database Code Generation Tool");
            _logger.Info("========================================");
            
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            
            _logger.Info($"Version: {version}");
            _logger.Info($"File Version: {fileVersionInfo.FileVersion}");
            _logger.Info($"Product Version: {fileVersionInfo.ProductVersion}");
            
            _logger.Info("\nComponent Versions:");
            
            // Get versions of important components
            var coreAssembly = Assembly.Load("EzDbCodeGen.Core");
            if (coreAssembly != null)
            {
                _logger.Info($"  Core: {coreAssembly.GetName().Version}");
            }
            
            var schemaAssembly = Assembly.Load("EzDbCodeGen.Schema");
            if (schemaAssembly != null)
            {
                _logger.Info($"  Schema: {schemaAssembly.GetName().Version}");
            }
            
            var typeMappingAssembly = Assembly.Load("EzDbCodeGen.TypeMapping");
            if (typeMappingAssembly != null)
            {
                _logger.Info($"  TypeMapping: {typeMappingAssembly.GetName().Version}");
            }
            
            var templateEngineAssembly = Assembly.Load("EzDbCodeGen.TemplateEngine");
            if (templateEngineAssembly != null)
            {
                _logger.Info($"  TemplateEngine: {templateEngineAssembly.GetName().Version}");
            }
            
            var codeGenerationAssembly = Assembly.Load("EzDbCodeGen.CodeGeneration");
            if (codeGenerationAssembly != null)
            {
                _logger.Info($"  CodeGeneration: {codeGenerationAssembly.GetName().Version}");
            }
            
            // Display framework information
            _logger.Info($"\nRuntime: {Environment.Version}");
            _logger.Info($"OS: {Environment.OSVersion}");
            _logger.Info($"Framework: {AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName}");
            
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool ValidateOptions(IReadOnlyDictionary<string, string> options)
        {
            // No validation needed for version command
            return true;
        }

        /// <inheritdoc/>
        public IList<string> GetValidationErrors(IReadOnlyDictionary<string, string> options)
        {
            // No validation errors for version command
            return new List<string>();
        }

        /// <inheritdoc/>
        public IDictionary<string, string> GetOptions()
        {
            // No options for version command
            return new Dictionary<string, string>();
        }

        /// <inheritdoc/>
        public IList<string> GetRequiredOptions()
        {
            // No required options for version command
            return new List<string>();
        }
    }
}
