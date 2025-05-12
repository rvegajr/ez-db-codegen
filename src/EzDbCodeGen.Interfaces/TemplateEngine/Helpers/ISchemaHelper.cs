using EzDbCodeGen.Interfaces.Schema;

namespace EzDbCodeGen.Interfaces.TemplateEngine.Helpers
{
    /// <summary>
    /// Defines helpers for working with database schemas in templates.
    /// </summary>
    public interface ISchemaHelper
    {
        /// <summary>
        /// Gets the properly formatted name for a table.
        /// </summary>
        /// <param name="table">The table to get the name for.</param>
        /// <param name="format">The format to use (e.g., "Pascal", "Camel", "Snake").</param>
        /// <returns>The formatted table name.</returns>
        string GetTableName(ITable table, string format = "Pascal");

        /// <summary>
        /// Gets the properly formatted name for a column.
        /// </summary>
        /// <param name="column">The column to get the name for.</param>
        /// <param name="format">The format to use (e.g., "Pascal", "Camel", "Snake").</param>
        /// <returns>The formatted column name.</returns>
        string GetColumnName(IColumn column, string format = "Pascal");

        /// <summary>
        /// Gets the programming language type for a column.
        /// </summary>
        /// <param name="column">The column to get the type for.</param>
        /// <param name="language">The programming language to get the type for.</param>
        /// <returns>The programming language type for the column.</returns>
        string GetColumnType(IColumn column, string language = "CSharp");

        /// <summary>
        /// Gets the default value for a column.
        /// </summary>
        /// <param name="column">The column to get the default value for.</param>
        /// <param name="language">The programming language to get the default value for.</param>
        /// <returns>The default value for the column.</returns>
        string GetColumnDefaultValue(IColumn column, string language = "CSharp");

        /// <summary>
        /// Gets the primary key for a table.
        /// </summary>
        /// <param name="table">The table to get the primary key for.</param>
        /// <returns>A comma-separated list of primary key column names.</returns>
        string GetPrimaryKey(ITable table);

        /// <summary>
        /// Gets the C# Entity Framework Core property configuration for a column.
        /// </summary>
        /// <param name="column">The column to get the configuration for.</param>
        /// <returns>The C# Entity Framework Core property configuration for the column.</returns>
        string GetEfCoreConfiguration(IColumn column);

        /// <summary>
        /// Gets the singular form of a table name.
        /// </summary>
        /// <param name="table">The table to get the singular name for.</param>
        /// <returns>The singular form of the table name.</returns>
        string GetSingularName(ITable table);

        /// <summary>
        /// Gets the plural form of a table name.
        /// </summary>
        /// <param name="table">The table to get the plural name for.</param>
        /// <returns>The plural form of the table name.</returns>
        string GetPluralName(ITable table);

        /// <summary>
        /// Gets the XML documentation for a table.
        /// </summary>
        /// <param name="table">The table to get the documentation for.</param>
        /// <returns>The XML documentation for the table.</returns>
        string GetXmlDocumentation(ITable table);

        /// <summary>
        /// Gets the XML documentation for a column.
        /// </summary>
        /// <param name="column">The column to get the documentation for.</param>
        /// <returns>The XML documentation for the column.</returns>
        string GetXmlDocumentation(IColumn column);
    }
}
