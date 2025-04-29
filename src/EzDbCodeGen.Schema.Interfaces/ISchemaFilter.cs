using System;
using System.Text.RegularExpressions;

namespace EzDbCodeGen.Schema.Interfaces;

/// <summary>
/// Defines a filter for database schema objects.
/// </summary>
public interface ISchemaFilter
{
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the filter.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Determines whether the specified schema matches the filter.
    /// </summary>
    /// <param name="schema">The schema name to check.</param>
    /// <returns>True if the schema should be included; otherwise, false.</returns>
    bool IncludeSchema(string schema);

    /// <summary>
    /// Determines whether the specified table matches the filter.
    /// </summary>
    /// <param name="schema">The schema of the table.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <returns>True if the table should be included; otherwise, false.</returns>
    bool IncludeTable(string schema, string tableName);

    /// <summary>
    /// Determines whether the specified column matches the filter.
    /// </summary>
    /// <param name="schema">The schema of the table.</param>
    /// <param name="tableName">The name of the table.</param>
    /// <param name="columnName">The name of the column.</param>
    /// <returns>True if the column should be included; otherwise, false.</returns>
    bool IncludeColumn(string schema, string tableName, string columnName);

    /// <summary>
    /// Determines whether the specified view matches the filter.
    /// </summary>
    /// <param name="schema">The schema of the view.</param>
    /// <param name="viewName">The name of the view.</param>
    /// <returns>True if the view should be included; otherwise, false.</returns>
    bool IncludeView(string schema, string viewName);

    /// <summary>
    /// Determines whether the specified stored procedure matches the filter.
    /// </summary>
    /// <param name="schema">The schema of the stored procedure.</param>
    /// <param name="procedureName">The name of the stored procedure.</param>
    /// <returns>True if the stored procedure should be included; otherwise, false.</returns>
    bool IncludeStoredProcedure(string schema, string procedureName);

    /// <summary>
    /// Determines whether the specified function matches the filter.
    /// </summary>
    /// <param name="schema">The schema of the function.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>True if the function should be included; otherwise, false.</returns>
    bool IncludeFunction(string schema, string functionName);
}
