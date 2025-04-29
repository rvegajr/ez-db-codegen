using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EzDbCodeGen.Schema.Interfaces;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Filters;

/// <summary>
/// A schema filter that uses regular expression patterns to filter database objects.
/// </summary>
public class PatternSchemaFilter : ISchemaFilter
{
    private readonly ILogger? _logger;
    private readonly List<Regex> _schemaIncludePatterns = new();
    private readonly List<Regex> _schemaExcludePatterns = new();
    private readonly List<Regex> _tableIncludePatterns = new();
    private readonly List<Regex> _tableExcludePatterns = new();
    private readonly List<Regex> _columnIncludePatterns = new();
    private readonly List<Regex> _columnExcludePatterns = new();
    private readonly List<Regex> _viewIncludePatterns = new();
    private readonly List<Regex> _viewExcludePatterns = new();
    private readonly List<Regex> _procedureIncludePatterns = new();
    private readonly List<Regex> _procedureExcludePatterns = new();
    private readonly List<Regex> _functionIncludePatterns = new();
    private readonly List<Regex> _functionExcludePatterns = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PatternSchemaFilter"/> class.
    /// </summary>
    /// <param name="name">The name of the filter.</param>
    /// <param name="description">The description of the filter.</param>
    /// <param name="logger">The logger.</param>
    public PatternSchemaFilter(string name, string description = "", ILogger? logger = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        _logger = logger;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <summary>
    /// Adds a schema include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match schema names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddSchemaIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _schemaIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added schema include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding schema include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a schema exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match schema names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddSchemaExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _schemaExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added schema exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding schema exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a table include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match table names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddTableIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _tableIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added table include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding table include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a table exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match table names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddTableExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _tableExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added table exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding table exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a column include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match column names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddColumnIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _columnIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added column include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding column include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a column exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match column names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddColumnExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _columnExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added column exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding column exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a view include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match view names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddViewIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _viewIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added view include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding view include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a view exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match view names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddViewExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _viewExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added view exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding view exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a stored procedure include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match stored procedure names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddStoredProcedureIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _procedureIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added stored procedure include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding stored procedure include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a stored procedure exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match stored procedure names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddStoredProcedureExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _procedureExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added stored procedure exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding stored procedure exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a function include pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match function names.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddFunctionIncludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _functionIncludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added function include pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding function include pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <summary>
    /// Adds a function exclude pattern.
    /// </summary>
    /// <param name="pattern">The regex pattern to match function names to exclude.</param>
    /// <returns>The current filter instance for method chaining.</returns>
    public PatternSchemaFilter AddFunctionExcludePattern(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            throw new ArgumentException("Pattern cannot be null or empty.", nameof(pattern));
        }

        try
        {
            _functionExcludePatterns.Add(new Regex(pattern, RegexOptions.IgnoreCase));
            _logger?.LogDebug("Added function exclude pattern: {Pattern}", pattern);
            return this;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding function exclude pattern: {Pattern}", pattern);
            throw;
        }
    }

    /// <inheritdoc/>
    public bool IncludeSchema(string schema)
    {
        if (string.IsNullOrEmpty(schema))
        {
            return false;
        }

        // If exclude patterns match, exclude the schema
        if (_schemaExcludePatterns.Any(p => p.IsMatch(schema)))
        {
            _logger?.LogDebug("Schema '{Schema}' excluded by exclude pattern", schema);
            return false;
        }

        // If no include patterns are defined, include all schemas that aren't excluded
        if (!_schemaIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only schemas that match include patterns
        bool included = _schemaIncludePatterns.Any(p => p.IsMatch(schema));
        if (included)
        {
            _logger?.LogDebug("Schema '{Schema}' included by include pattern", schema);
        }
        else
        {
            _logger?.LogDebug("Schema '{Schema}' excluded (no matching include pattern)", schema);
        }

        return included;
    }

    /// <inheritdoc/>
    public bool IncludeTable(string schema, string tableName)
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(tableName))
        {
            return false;
        }

        // First check if the schema is included
        if (!IncludeSchema(schema))
        {
            return false;
        }

        // If exclude patterns match, exclude the table
        if (_tableExcludePatterns.Any(p => p.IsMatch(tableName)))
        {
            _logger?.LogDebug("Table '{Schema}.{Table}' excluded by exclude pattern", schema, tableName);
            return false;
        }

        // If no include patterns are defined, include all tables that aren't excluded
        if (!_tableIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only tables that match include patterns
        bool included = _tableIncludePatterns.Any(p => p.IsMatch(tableName));
        if (included)
        {
            _logger?.LogDebug("Table '{Schema}.{Table}' included by include pattern", schema, tableName);
        }
        else
        {
            _logger?.LogDebug("Table '{Schema}.{Table}' excluded (no matching include pattern)", schema, tableName);
        }

        return included;
    }

    /// <inheritdoc/>
    public bool IncludeColumn(string schema, string tableName, string columnName)
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
        {
            return false;
        }

        // First check if the table is included
        if (!IncludeTable(schema, tableName))
        {
            return false;
        }

        // If exclude patterns match, exclude the column
        if (_columnExcludePatterns.Any(p => p.IsMatch(columnName)))
        {
            _logger?.LogDebug("Column '{Schema}.{Table}.{Column}' excluded by exclude pattern", schema, tableName, columnName);
            return false;
        }

        // If no include patterns are defined, include all columns that aren't excluded
        if (!_columnIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only columns that match include patterns
        bool included = _columnIncludePatterns.Any(p => p.IsMatch(columnName));
        if (included)
        {
            _logger?.LogDebug("Column '{Schema}.{Table}.{Column}' included by include pattern", schema, tableName, columnName);
        }
        else
        {
            _logger?.LogDebug("Column '{Schema}.{Table}.{Column}' excluded (no matching include pattern)", schema, tableName, columnName);
        }

        return included;
    }

    /// <inheritdoc/>
    public bool IncludeView(string schema, string viewName)
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(viewName))
        {
            return false;
        }

        // First check if the schema is included
        if (!IncludeSchema(schema))
        {
            return false;
        }

        // If exclude patterns match, exclude the view
        if (_viewExcludePatterns.Any(p => p.IsMatch(viewName)))
        {
            _logger?.LogDebug("View '{Schema}.{View}' excluded by exclude pattern", schema, viewName);
            return false;
        }

        // If no include patterns are defined, include all views that aren't excluded
        if (!_viewIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only views that match include patterns
        bool included = _viewIncludePatterns.Any(p => p.IsMatch(viewName));
        if (included)
        {
            _logger?.LogDebug("View '{Schema}.{View}' included by include pattern", schema, viewName);
        }
        else
        {
            _logger?.LogDebug("View '{Schema}.{View}' excluded (no matching include pattern)", schema, viewName);
        }

        return included;
    }

    /// <inheritdoc/>
    public bool IncludeStoredProcedure(string schema, string procedureName)
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(procedureName))
        {
            return false;
        }

        // First check if the schema is included
        if (!IncludeSchema(schema))
        {
            return false;
        }

        // If exclude patterns match, exclude the stored procedure
        if (_procedureExcludePatterns.Any(p => p.IsMatch(procedureName)))
        {
            _logger?.LogDebug("Stored procedure '{Schema}.{Procedure}' excluded by exclude pattern", schema, procedureName);
            return false;
        }

        // If no include patterns are defined, include all stored procedures that aren't excluded
        if (!_procedureIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only stored procedures that match include patterns
        bool included = _procedureIncludePatterns.Any(p => p.IsMatch(procedureName));
        if (included)
        {
            _logger?.LogDebug("Stored procedure '{Schema}.{Procedure}' included by include pattern", schema, procedureName);
        }
        else
        {
            _logger?.LogDebug("Stored procedure '{Schema}.{Procedure}' excluded (no matching include pattern)", schema, procedureName);
        }

        return included;
    }

    /// <inheritdoc/>
    public bool IncludeFunction(string schema, string functionName)
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(functionName))
        {
            return false;
        }

        // First check if the schema is included
        if (!IncludeSchema(schema))
        {
            return false;
        }

        // If exclude patterns match, exclude the function
        if (_functionExcludePatterns.Any(p => p.IsMatch(functionName)))
        {
            _logger?.LogDebug("Function '{Schema}.{Function}' excluded by exclude pattern", schema, functionName);
            return false;
        }

        // If no include patterns are defined, include all functions that aren't excluded
        if (!_functionIncludePatterns.Any())
        {
            return true;
        }

        // Otherwise, include only functions that match include patterns
        bool included = _functionIncludePatterns.Any(p => p.IsMatch(functionName));
        if (included)
        {
            _logger?.LogDebug("Function '{Schema}.{Function}' included by include pattern", schema, functionName);
        }
        else
        {
            _logger?.LogDebug("Function '{Schema}.{Function}' excluded (no matching include pattern)", schema, functionName);
        }

        return included;
    }
}
