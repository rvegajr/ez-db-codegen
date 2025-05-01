using System;

namespace EzDbCodeGen.Schema.Providers
{
    /// <summary>
    /// Represents connection options specific to SQL Server.
    /// </summary>
    public class SqlServerConnectionOptions
    {
        /// <summary>
        /// Gets or sets the command timeout in seconds.
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
        
        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string ApplicationName { get; set; } = "EzDbCodeGen";
        
        /// <summary>
        /// Gets or sets a value indicating whether to enable Multiple Active Result Sets (MARS).
        /// </summary>
        public bool EnableMultipleActiveResultSets { get; set; } = false;
    }
}
