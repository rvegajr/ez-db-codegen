using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Schema.Analysis;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.TypeMapping;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.CodeGen;

/// <summary>
/// Adapts database schema objects into template-friendly models.
/// </summary>
public class SchemaModelAdapter : ISchemaModelAdapter
{
    private readonly IDataTypeMap _dataTypeMap;
    private readonly ILogger<SchemaModelAdapter>? _logger;
    private readonly RelationshipAnalyzer _relationshipAnalyzer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaModelAdapter"/> class.
    /// </summary>
    /// <param name="dataTypeMap">The data type map.</param>
    /// <param name="logger">The logger.</param>
    public SchemaModelAdapter(IDataTypeMap dataTypeMap, ILogger<SchemaModelAdapter>? logger = null)
    {
        _dataTypeMap = dataTypeMap ?? throw new ArgumentNullException(nameof(dataTypeMap));
        _logger = logger;
        _relationshipAnalyzer = new RelationshipAnalyzer(_logger);
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ConvertSchemaToTemplateModelAsync(IDatabaseSchema schema)
    {
        if (schema == null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        _logger?.LogDebug("Converting schema {SchemaName} to template model", schema.DatabaseName);

        var model = new Dictionary<string, object>
        {
            ["DatabaseName"] = schema.DatabaseName,
            ["TableCount"] = schema.Tables.Count(),
            ["ViewCount"] = schema.Views.Count(),
            ["StoredProcedureCount"] = schema.StoredProcedures.Count(),
            ["FunctionCount"] = schema.Functions.Count()
        };

        // Analyze relationships
        var relationships = _relationshipAnalyzer.AnalyzeRelationships(schema).ToList();
        model["Relationships"] = relationships.Select(r => new Dictionary<string, object>
        {
            ["Type"] = r.Type.ToString(),
            ["Name"] = r.Name,
            ["Description"] = r.GetDescription()
        }).ToList();

        // Convert tables
        var tables = new List<IDictionary<string, object>>();
        foreach (var table in schema.Tables)
        {
            tables.Add(await ConvertTableToTemplateModelAsync(table));
        }
        model["Tables"] = tables;

        // Convert views
        var views = new List<IDictionary<string, object>>();
        foreach (var view in schema.Views)
        {
            views.Add(await ConvertViewToTemplateModelAsync(view));
        }
        model["Views"] = views;

        // Convert stored procedures
        var storedProcedures = new List<IDictionary<string, object>>();
        foreach (var storedProcedure in schema.StoredProcedures)
        {
            storedProcedures.Add(await ConvertStoredProcedureToTemplateModelAsync(storedProcedure));
        }
        model["StoredProcedures"] = storedProcedures;

        // Convert functions
        var functions = new List<IDictionary<string, object>>();
        foreach (var function in schema.Functions)
        {
            functions.Add(await ConvertFunctionToTemplateModelAsync(function));
        }
        model["Functions"] = functions;

        _logger?.LogInformation("Converted schema {SchemaName} to template model with {TableCount} tables, {ViewCount} views, {StoredProcedureCount} stored procedures, and {FunctionCount} functions",
            schema.DatabaseName, tables.Count, views.Count, storedProcedures.Count, functions.Count);

        return model;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ConvertTableToTemplateModelAsync(ITable table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        _logger?.LogDebug("Converting table {TableName} to template model", table.Name);

        var model = new Dictionary<string, object>
        {
            ["Name"] = table.Name,
            ["Schema"] = table.Schema ?? "",
            ["FullName"] = $"{table.Schema}.{table.Name}",
            ["Description"] = table.Description ?? "",
            ["HasIdentity"] = table.Columns.Any(c => c.IsIdentity),
            ["HasPrimaryKey"] = table.PrimaryKey != null,
            ["HasForeignKeys"] = table.ForeignKeys.Any(),
            ["HasIndexes"] = table.Indexes.Any(),
            ["HasUniqueConstraints"] = table.UniqueConstraints.Any(),
            ["IsTemporalTable"] = table.IsTemporalTable,
            ["TemporalTableHistoryTableSchema"] = table.TemporalTableHistoryTableSchema ?? "",
            ["TemporalTableHistoryTableName"] = table.TemporalTableHistoryTableName ?? ""
        };

        // Convert columns
        var columns = new List<IDictionary<string, object>>();
        foreach (var column in table.Columns)
        {
            columns.Add(ConvertColumnToTemplateModel(column));
        }
        model["Columns"] = columns;
        
        // Convert primary key
        if (table.PrimaryKey != null)
        {
            model["PrimaryKey"] = ConvertKeyToTemplateModel(table.PrimaryKey);
        }

        // Convert foreign keys
        var foreignKeys = new List<IDictionary<string, object>>();
        foreach (var foreignKey in table.ForeignKeys)
        {
            foreignKeys.Add(ConvertForeignKeyToTemplateModel(foreignKey));
        }
        model["ForeignKeys"] = foreignKeys;

        // Convert indexes
        var indexes = new List<IDictionary<string, object>>();
        foreach (var index in table.Indexes)
        {
            indexes.Add(ConvertIndexToTemplateModel(index));
        }
        model["Indexes"] = indexes;

        // Convert unique constraints
        var uniqueConstraints = new List<IDictionary<string, object>>();
        foreach (var uniqueConstraint in table.UniqueConstraints)
        {
            uniqueConstraints.Add(ConvertUniqueConstraintToTemplateModel(uniqueConstraint));
        }
        model["UniqueConstraints"] = uniqueConstraints;

        return model;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ConvertViewToTemplateModelAsync(IView view)
    {
        if (view == null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        _logger?.LogDebug("Converting view {ViewName} to template model", view.Name);

        var model = new Dictionary<string, object>
        {
            ["Name"] = view.Name,
            ["Schema"] = view.Schema ?? "",
            ["FullName"] = $"{view.Schema}.{view.Name}",
            ["Description"] = view.Description ?? "",
            ["Definition"] = view.Definition ?? "",
            ["IsIndexed"] = view.IsIndexed
        };

        // Convert columns
        var columns = new List<IDictionary<string, object>>();
        foreach (var column in view.Columns)
        {
            columns.Add(ConvertViewColumnToTemplateModel(column));
        }
        model["Columns"] = columns;

        return model;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ConvertStoredProcedureToTemplateModelAsync(IStoredProcedure storedProcedure)
    {
        if (storedProcedure == null)
        {
            throw new ArgumentNullException(nameof(storedProcedure));
        }

        _logger?.LogDebug("Converting stored procedure {ProcedureName} to template model", storedProcedure.Name);

        var model = new Dictionary<string, object>
        {
            ["Name"] = storedProcedure.Name,
            ["Schema"] = storedProcedure.Schema ?? "",
            ["FullName"] = $"{storedProcedure.Schema}.{storedProcedure.Name}",
            ["Description"] = storedProcedure.Description ?? "",
            ["Definition"] = storedProcedure.Definition ?? "",
            ["HasReturnValue"] = storedProcedure.HasReturnValue
        };

        // Convert parameters
        var parameters = new List<IDictionary<string, object>>();
        foreach (var parameter in storedProcedure.Parameters)
        {
            parameters.Add(ConvertParameterToTemplateModel(parameter));
        }
        model["Parameters"] = parameters;

        // Convert result columns
        var resultColumns = new List<IDictionary<string, object>>();
        foreach (var resultColumn in storedProcedure.ResultColumns)
        {
            resultColumns.Add(ConvertResultColumnToTemplateModel(resultColumn));
        }
        model["ResultColumns"] = resultColumns;

        return model;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<string, object>> ConvertFunctionToTemplateModelAsync(IFunction function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        _logger?.LogDebug("Converting function {FunctionName} to template model", function.Name);

        var model = new Dictionary<string, object>
        {
            ["Name"] = function.Name,
            ["Schema"] = function.Schema ?? "",
            ["FullName"] = $"{function.Schema}.{function.Name}",
            ["Description"] = function.Description ?? "",
            ["Definition"] = function.Definition ?? "",
            ["ReturnType"] = function.ReturnType ?? "",
            ["IsTableValued"] = function.IsTableValued
        };

        // Convert parameters
        var parameters = new List<IDictionary<string, object>>();
        foreach (var parameter in function.Parameters)
        {
            parameters.Add(ConvertParameterToTemplateModel(parameter));
        }
        model["Parameters"] = parameters;

        // Convert result columns (for table-valued functions)
        if (function.IsTableValued)
        {
            var resultColumns = new List<IDictionary<string, object>>();
            foreach (var resultColumn in function.ResultColumns)
            {
                resultColumns.Add(ConvertResultColumnToTemplateModel(resultColumn));
            }
            model["ResultColumns"] = resultColumns;
        }

        return model;
    }

    private IDictionary<string, object> ConvertColumnToTemplateModel(IColumn column)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = column.Name,
            ["OrdinalPosition"] = column.OrdinalPosition,
            ["DataType"] = column.DataType,
            ["IsNullable"] = column.IsNullable,
            ["MaxLength"] = column.MaxLength,
            ["Precision"] = column.Precision,
            ["Scale"] = column.Scale,
            ["IsIdentity"] = column.IsIdentity,
            ["IsComputed"] = column.IsComputed,
            ["DefaultValue"] = column.DefaultValue ?? "",
            ["Description"] = column.Description ?? ""
        };

        // Add type mappings
        try
        {
            model["CSharpType"] = _dataTypeMap.ToCSharpType(column.DataType, column.IsNullable);
            model["TypeScriptType"] = _dataTypeMap.ToTypeScriptType(column.DataType, column.IsNullable);
            model["JavaType"] = _dataTypeMap.ToJavaType(column.DataType, column.IsNullable);
            model["PythonType"] = _dataTypeMap.ToPythonType(column.DataType, column.IsNullable);
            
            model["CSharpDefaultValue"] = _dataTypeMap.GetCSharpDefaultValue(column.DataType, column.IsNullable);
            model["TypeScriptDefaultValue"] = _dataTypeMap.GetTypeScriptDefaultValue(column.DataType, column.IsNullable);
            model["JavaDefaultValue"] = _dataTypeMap.GetJavaDefaultValue(column.DataType, column.IsNullable);
            model["PythonDefaultValue"] = _dataTypeMap.GetPythonDefaultValue(column.DataType, column.IsNullable);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error mapping data type for column {ColumnName}: {ErrorMessage}", column.Name, ex.Message);
        }

        return model;
    }

    private IDictionary<string, object> ConvertKeyToTemplateModel(IKey key)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = key.Name,
            ["IsClustered"] = key.IsClustered
        };

        // Convert key columns
        var keyColumns = new List<IDictionary<string, object>>();
        foreach (var column in key.Columns)
        {
            keyColumns.Add(new Dictionary<string, object>
            {
                ["Name"] = column.Name,
                ["OrdinalPosition"] = column.OrdinalPosition
            });
        }
        model["Columns"] = keyColumns;

        return model;
    }

    private IDictionary<string, object> ConvertForeignKeyToTemplateModel(IForeignKey foreignKey)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = foreignKey.Name,
            ["ReferencedTableSchema"] = foreignKey.ReferencedTable.Schema ?? "",
            ["ReferencedTableName"] = foreignKey.ReferencedTable.Name,
            ["ReferencedTableFullName"] = $"{foreignKey.ReferencedTable.Schema}.{foreignKey.ReferencedTable.Name}",
            ["DeleteAction"] = foreignKey.DeleteAction.ToString(),
            ["UpdateAction"] = foreignKey.UpdateAction.ToString(),
            ["IsSelfReferencing"] = foreignKey.Table == foreignKey.ReferencedTable
        };

        // Convert foreign key columns
        var columnPairs = new List<IDictionary<string, object>>();
        for (int i = 0; i < foreignKey.Columns.Count(); i++)
        {
            var column = foreignKey.Columns.ElementAt(i);
            var referencedColumn = foreignKey.ReferencedColumns.ElementAt(i);
            
            columnPairs.Add(new Dictionary<string, object>
            {
                ["ColumnName"] = column.Name,
                ["ReferencedColumnName"] = referencedColumn.Name
            });
        }
        model["ColumnPairs"] = columnPairs;

        return model;
    }

    private IDictionary<string, object> ConvertIndexToTemplateModel(IIndex index)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = index.Name,
            ["IsUnique"] = index.IsUnique,
            ["IsClustered"] = index.IsClustered,
            ["FilterExpression"] = index.FilterExpression ?? ""
        };

        // Convert index columns
        var indexColumns = new List<IDictionary<string, object>>();
        foreach (var indexColumn in index.Columns)
        {
            indexColumns.Add(new Dictionary<string, object>
            {
                ["ColumnName"] = indexColumn.Column.Name,
                ["OrdinalPosition"] = indexColumn.OrdinalPosition,
                ["IsDescending"] = indexColumn.IsDescending
            });
        }
        model["Columns"] = indexColumns;

        return model;
    }

    private IDictionary<string, object> ConvertUniqueConstraintToTemplateModel(IUniqueConstraint uniqueConstraint)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = uniqueConstraint.Name,
            ["IsClustered"] = uniqueConstraint.IsClustered
        };

        // Convert unique constraint columns
        var constraintColumns = new List<IDictionary<string, object>>();
        foreach (var column in uniqueConstraint.Columns)
        {
            constraintColumns.Add(new Dictionary<string, object>
            {
                ["Name"] = column.Name,
                ["OrdinalPosition"] = column.OrdinalPosition
            });
        }
        model["Columns"] = constraintColumns;

        return model;
    }

    private IDictionary<string, object> ConvertViewColumnToTemplateModel(IViewColumn column)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = column.Name,
            ["OrdinalPosition"] = column.OrdinalPosition,
            ["DataType"] = column.DataType,
            ["IsNullable"] = column.IsNullable,
            ["MaxLength"] = column.MaxLength,
            ["Precision"] = column.Precision,
            ["Scale"] = column.Scale
        };

        // Add type mappings
        try
        {
            model["CSharpType"] = _dataTypeMap.ToCSharpType(column.DataType, column.IsNullable);
            model["TypeScriptType"] = _dataTypeMap.ToTypeScriptType(column.DataType, column.IsNullable);
            model["JavaType"] = _dataTypeMap.ToJavaType(column.DataType, column.IsNullable);
            model["PythonType"] = _dataTypeMap.ToPythonType(column.DataType, column.IsNullable);
            
            model["CSharpDefaultValue"] = _dataTypeMap.GetCSharpDefaultValue(column.DataType, column.IsNullable);
            model["TypeScriptDefaultValue"] = _dataTypeMap.GetTypeScriptDefaultValue(column.DataType, column.IsNullable);
            model["JavaDefaultValue"] = _dataTypeMap.GetJavaDefaultValue(column.DataType, column.IsNullable);
            model["PythonDefaultValue"] = _dataTypeMap.GetPythonDefaultValue(column.DataType, column.IsNullable);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error mapping data type for view column {ColumnName}: {ErrorMessage}", column.Name, ex.Message);
        }

        return model;
    }

    private IDictionary<string, object> ConvertParameterToTemplateModel(IParameter parameter)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = parameter.Name,
            ["OrdinalPosition"] = parameter.OrdinalPosition,
            ["DataType"] = parameter.DataType,
            ["IsNullable"] = parameter.IsNullable,
            ["MaxLength"] = parameter.MaxLength,
            ["Precision"] = parameter.Precision,
            ["Scale"] = parameter.Scale,
            ["IsOutput"] = parameter.IsOutput,
            ["DefaultValue"] = parameter.DefaultValue ?? ""
        };

        // Add type mappings
        try
        {
            model["CSharpType"] = _dataTypeMap.ToCSharpType(parameter.DataType, parameter.IsNullable);
            model["TypeScriptType"] = _dataTypeMap.ToTypeScriptType(parameter.DataType, parameter.IsNullable);
            model["JavaType"] = _dataTypeMap.ToJavaType(parameter.DataType, parameter.IsNullable);
            model["PythonType"] = _dataTypeMap.ToPythonType(parameter.DataType, parameter.IsNullable);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error mapping data type for parameter {ParameterName}: {ErrorMessage}", parameter.Name, ex.Message);
        }

        return model;
    }

    private IDictionary<string, object> ConvertResultColumnToTemplateModel(IResultColumn resultColumn)
    {
        var model = new Dictionary<string, object>
        {
            ["Name"] = resultColumn.Name,
            ["OrdinalPosition"] = resultColumn.OrdinalPosition,
            ["DataType"] = resultColumn.DataType,
            ["IsNullable"] = resultColumn.IsNullable,
            ["MaxLength"] = resultColumn.MaxLength,
            ["Precision"] = resultColumn.Precision,
            ["Scale"] = resultColumn.Scale
        };

        // Add type mappings
        try
        {
            model["CSharpType"] = _dataTypeMap.ToCSharpType(resultColumn.DataType, resultColumn.IsNullable);
            model["TypeScriptType"] = _dataTypeMap.ToTypeScriptType(resultColumn.DataType, resultColumn.IsNullable);
            model["JavaType"] = _dataTypeMap.ToJavaType(resultColumn.DataType, resultColumn.IsNullable);
            model["PythonType"] = _dataTypeMap.ToPythonType(resultColumn.DataType, resultColumn.IsNullable);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error mapping data type for result column {ColumnName}: {ErrorMessage}", resultColumn.Name, ex.Message);
        }

        return model;
    }
}
