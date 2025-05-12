using System.Collections.Generic;
using EzDbCodeGen.Interfaces.CodeGen;
using EzDbCodeGen.Interfaces.Schema;
using EzDbCodeGen.Interfaces.Schema.Filters;
using EzDbCodeGen.Interfaces.TemplateEngine.Filters;

namespace EzDbCodeGen.Interfaces.Cli
{
    /// <summary>
    /// Defines options for command-line operation.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the template file path.
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the database provider name.
        /// </summary>
        public string ProviderName { get; set; } = "SqlServer";

        /// <summary>
        /// Gets or sets the configuration file path.
        /// </summary>
        public string ConfigFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to run in verbose mode.
        /// </summary>
        public bool Verbose { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to run in dry run mode (no files written).
        /// </summary>
        public bool DryRun { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to run in quiet mode (minimal output).
        /// </summary>
        public bool Quiet { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite existing files.
        /// </summary>
        public bool Overwrite { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show help.
        /// </summary>
        public bool ShowHelp { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show version information.
        /// </summary>
        public bool ShowVersion { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to test the connection.
        /// </summary>
        public bool TestConnection { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to run in interactive mode.
        /// </summary>
        public bool Interactive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to save the configuration to a file.
        /// </summary>
        public bool SaveConfig { get; set; } = false;

        /// <summary>
        /// Gets or sets the schema provider options.
        /// </summary>
        public SchemaProviderOptions SchemaOptions { get; set; } = SchemaProviderOptions.Default();

        /// <summary>
        /// Gets or sets the code generation options.
        /// </summary>
        public CodeGenerationOptions CodeGenOptions { get; set; } = CodeGenerationOptions.Default();

        /// <summary>
        /// Gets or sets the schema filter options.
        /// </summary>
        public SchemaFilterOptions SchemaFilterOptions { get; set; } = SchemaFilterOptions.Default();

        /// <summary>
        /// Gets or sets the template filter options.
        /// </summary>
        public TemplateFilterOptions TemplateFilterOptions { get; set; } = TemplateFilterOptions.Default();

        /// <summary>
        /// Gets or sets the command to execute.
        /// </summary>
        public string Command { get; set; } = "generate";

        /// <summary>
        /// Gets or sets additional arguments.
        /// </summary>
        public IDictionary<string, string> AdditionalArgs { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets a formatted connection string suitable for display (with password masked).
        /// </summary>
        public string DisplayConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(ConnectionString))
                {
                    return string.Empty;
                }

                // Mask password in connection string for display
                string result = ConnectionString;
                string[] passwordPatterns = { "password=", "pwd=", "pass=" };
                
                foreach (var pattern in passwordPatterns)
                {
                    int index = result.ToLowerInvariant().IndexOf(pattern);
                    if (index >= 0)
                    {
                        int valueStart = index + pattern.Length;
                        int valueEnd = result.IndexOf(';', valueStart);
                        if (valueEnd < 0)
                        {
                            valueEnd = result.Length;
                        }

                        result = result.Substring(0, valueStart) + "******" +
                                (valueEnd < result.Length ? result.Substring(valueEnd) : string.Empty);
                    }
                }

                return result;
            }
        }
    }
}
