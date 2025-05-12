using System.Collections.Generic;

namespace EzDbCodeGen.Interfaces.TemplateEngine.Filters
{
    /// <summary>
    /// Defines options for template filtering.
    /// </summary>
    public class TemplateFilterOptions
    {
        /// <summary>
        /// Gets or sets a collection of template patterns to include (using wildcard matching).
        /// </summary>
        public ICollection<string> IncludeTemplatePatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of template patterns to exclude (using wildcard matching).
        /// </summary>
        public ICollection<string> ExcludeTemplatePatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of output patterns to include (using wildcard matching).
        /// </summary>
        public ICollection<string> IncludeOutputPatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a collection of output patterns to exclude (using wildcard matching).
        /// </summary>
        public ICollection<string> ExcludeOutputPatterns { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite existing files.
        /// </summary>
        public bool OverwriteExistingFiles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use case-sensitive pattern matching.
        /// </summary>
        public bool CaseSensitiveMatching { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to create directories that don't exist.
        /// </summary>
        public bool CreateDirectories { get; set; } = true;

        /// <summary>
        /// Gets or sets a function that transforms output paths.
        /// </summary>
        public System.Func<string, string> OutputPathTransformer { get; set; } = null;

        /// <summary>
        /// Gets or sets a value indicating whether to process templates in dry run mode (no files written).
        /// </summary>
        public bool DryRun { get; set; } = false;

        /// <summary>
        /// Gets or sets the root output directory for all generated files.
        /// </summary>
        public string OutputRoot { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets additional custom properties for template filtering.
        /// </summary>
        public IDictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Creates a new instance of TemplateFilterOptions with default settings.
        /// </summary>
        /// <returns>A TemplateFilterOptions instance with default settings.</returns>
        public static TemplateFilterOptions Default() => new TemplateFilterOptions();

        /// <summary>
        /// Creates a new instance of TemplateFilterOptions in dry run mode (no files written).
        /// </summary>
        /// <returns>A TemplateFilterOptions instance in dry run mode.</returns>
        public static TemplateFilterOptions DryRunMode() => new TemplateFilterOptions
        {
            DryRun = true
        };
    }
}
