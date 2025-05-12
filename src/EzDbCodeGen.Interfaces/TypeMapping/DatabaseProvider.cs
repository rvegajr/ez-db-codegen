namespace EzDbCodeGen.Interfaces.TypeMapping
{
    /// <summary>
    /// Defines the supported database providers for type mapping.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>
        /// Microsoft SQL Server database provider.
        /// </summary>
        SqlServer,

        /// <summary>
        /// PostgreSQL database provider.
        /// </summary>
        PostgreSQL,

        /// <summary>
        /// MySQL database provider.
        /// </summary>
        MySQL,

        /// <summary>
        /// Oracle database provider.
        /// </summary>
        Oracle,

        /// <summary>
        /// SQLite database provider.
        /// </summary>
        SQLite
    }
}
