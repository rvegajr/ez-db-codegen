using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.Providers
{
    /// <summary>
    /// Extensions for the SQL Server schema provider to handle special data types and SQL Server-specific features.
    /// </summary>
    internal static class SqlServerSchemaProviderExtensions
    {
        /// <summary>
        /// Extracts information about special data types (XML, spatial, etc.) for columns.
        /// </summary>
        /// <param name="connection">The SQL connection.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task ExtractSpecialDataTypesAsync(SqlConnection connection, DatabaseSchema schema, ILogger logger)
        {
            // Query to get information about special data types
            string query = @"
                SELECT 
                    s.name AS SchemaName,
                    t.name AS TableName,
                    c.name AS ColumnName,
                    ty.name AS TypeName,
                    CASE 
                        WHEN ty.name = 'xml' THEN 
                            CASE WHEN c.is_xml_document = 1 THEN 'DOCUMENT' ELSE 'CONTENT' END
                        WHEN ty.name IN ('geography', 'geometry') THEN 
                            CASE WHEN c.system_type_id = 129 THEN 'SRID ' + CAST(ISNULL(c.spatial_reference_id, 0) AS VARCHAR(10)) ELSE '' END
                        ELSE ''
                    END AS SpecialTypeInfo
                FROM 
                    sys.columns c
                INNER JOIN 
                    sys.tables t ON c.object_id = t.object_id
                INNER JOIN 
                    sys.schemas s ON t.schema_id = s.schema_id
                INNER JOIN 
                    sys.types ty ON c.system_type_id = ty.system_type_id AND c.user_type_id = ty.user_type_id
                WHERE 
                    ty.name IN ('xml', 'geography', 'geometry', 'hierarchyid')
                ORDER BY 
                    s.name, t.name, c.column_id";

            try
            {
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        string schemaName = reader["SchemaName"].ToString();
                        string tableName = reader["TableName"].ToString();
                        string columnName = reader["ColumnName"].ToString();
                        string typeName = reader["TypeName"].ToString();
                        string specialTypeInfo = reader["SpecialTypeInfo"].ToString();

                        // Find the table and column
                        var table = schema.GetTable(schemaName, tableName);
                        if (table == null)
                        {
                            logger.LogWarning("Could not find table {Schema}.{Table} for special data type", schemaName, tableName);
                            continue;
                        }

                        var column = table.GetColumn(columnName);
                        if (column == null)
                        {
                            logger.LogWarning("Could not find column {Column} in table {Schema}.{Table} for special data type", 
                                columnName, schemaName, tableName);
                            continue;
                        }

                        // Update column with special type information
                        if (column is Column concreteColumn)
                        {
                            // Set additional properties for special types
                            switch (typeName.ToLowerInvariant())
                            {
                                case "xml":
                                    concreteColumn.SetExtendedProperty("XmlType", specialTypeInfo);
                                    break;
                                case "geography":
                                case "geometry":
                                    concreteColumn.SetExtendedProperty("SpatialType", typeName);
                                    concreteColumn.SetExtendedProperty("SpatialInfo", specialTypeInfo);
                                    break;
                                case "hierarchyid":
                                    concreteColumn.SetExtendedProperty("IsHierarchyId", "true");
                                    break;
                            }

                            logger.LogDebug("Updated special data type information for {Schema}.{Table}.{Column} ({Type})",
                                schemaName, tableName, columnName, typeName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error extracting special data type information: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// Extracts information about temporal tables.
        /// </summary>
        /// <param name="connection">The SQL connection.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task ExtractTemporalTablesAsync(SqlConnection connection, DatabaseSchema schema, ILogger logger)
        {
            // First check if the SQL Server version supports temporal tables (SQL Server 2016+)
            bool supportsTemporalTables = false;
            try
            {
                string versionQuery = "SELECT SERVERPROPERTY('ProductVersion') AS Version";
                using (var command = new SqlCommand(versionQuery, connection))
                {
                    var versionString = (await command.ExecuteScalarAsync().ConfigureAwait(false))?.ToString();
                    if (!string.IsNullOrEmpty(versionString))
                    {
                        var versionParts = versionString.Split('.');
                        if (versionParts.Length > 0 && int.TryParse(versionParts[0], out int majorVersion))
                        {
                            supportsTemporalTables = majorVersion >= 13; // SQL Server 2016 is version 13
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Could not determine SQL Server version: {Message}", ex.Message);
                return;
            }

            if (!supportsTemporalTables)
            {
                logger.LogInformation("SQL Server version does not support temporal tables");
                return;
            }

            // Query to get information about temporal tables
            string query = @"
                SELECT 
                    s.name AS SchemaName,
                    t.name AS TableName,
                    h.name AS HistoryTableName,
                    hs.name AS HistorySchemaName,
                    c1.name AS PeriodStartColumn,
                    c2.name AS PeriodEndColumn
                FROM 
                    sys.tables t
                INNER JOIN 
                    sys.schemas s ON t.schema_id = s.schema_id
                INNER JOIN 
                    sys.periods p ON t.object_id = p.object_id
                INNER JOIN 
                    sys.columns c1 ON t.object_id = c1.object_id AND p.start_column_id = c1.column_id
                INNER JOIN 
                    sys.columns c2 ON t.object_id = c2.object_id AND p.end_column_id = c2.column_id
                LEFT JOIN 
                    sys.tables h ON t.history_table_id = h.object_id
                LEFT JOIN 
                    sys.schemas hs ON h.schema_id = hs.schema_id
                WHERE 
                    t.temporal_type = 2 -- SYSTEM_VERSIONED_TEMPORAL_TABLE
                ORDER BY 
                    s.name, t.name";

            try
            {
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        string schemaName = reader["SchemaName"].ToString();
                        string tableName = reader["TableName"].ToString();
                        string historySchemaName = reader["HistorySchemaName"]?.ToString() ?? string.Empty;
                        string historyTableName = reader["HistoryTableName"]?.ToString() ?? string.Empty;
                        string periodStartColumn = reader["PeriodStartColumn"].ToString();
                        string periodEndColumn = reader["PeriodEndColumn"].ToString();

                        // Find the table
                        var table = schema.GetTable(schemaName, tableName);
                        if (table == null)
                        {
                            logger.LogWarning("Could not find temporal table {Schema}.{Table}", schemaName, tableName);
                            continue;
                        }

                        // Update table with temporal information
                        if (table is Table concreteTable)
                        {
                            concreteTable.SetExtendedProperty("IsTemporal", "true");
                            concreteTable.SetExtendedProperty("PeriodStartColumn", periodStartColumn);
                            concreteTable.SetExtendedProperty("PeriodEndColumn", periodEndColumn);
                            
                            if (!string.IsNullOrEmpty(historyTableName))
                            {
                                concreteTable.SetExtendedProperty("HistoryTable", $"{historySchemaName}.{historyTableName}");
                            }

                            logger.LogDebug("Updated temporal table information for {Schema}.{Table}", schemaName, tableName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error extracting temporal table information: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// Extracts information about JSON columns.
        /// </summary>
        /// <param name="connection">The SQL connection.</param>
        /// <param name="schema">The database schema.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task ExtractJsonColumnsAsync(SqlConnection connection, DatabaseSchema schema, ILogger logger)
        {
            // Query to identify columns that might contain JSON data
            string query = @"
                SELECT 
                    s.name AS SchemaName,
                    t.name AS TableName,
                    c.name AS ColumnName
                FROM 
                    sys.columns c
                INNER JOIN 
                    sys.tables t ON c.object_id = t.object_id
                INNER JOIN 
                    sys.schemas s ON t.schema_id = s.schema_id
                INNER JOIN 
                    sys.types ty ON c.system_type_id = ty.system_type_id AND c.user_type_id = ty.user_type_id
                WHERE 
                    ty.name IN ('nvarchar', 'varchar') AND
                    c.max_length = -1 AND
                    (c.name LIKE '%json%' OR c.name LIKE '%Json%' OR c.name LIKE '%JSON%')
                ORDER BY 
                    s.name, t.name, c.column_id";

            try
            {
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        string schemaName = reader["SchemaName"].ToString();
                        string tableName = reader["TableName"].ToString();
                        string columnName = reader["ColumnName"].ToString();

                        // Find the table and column
                        var table = schema.GetTable(schemaName, tableName);
                        if (table == null)
                        {
                            continue;
                        }

                        var column = table.GetColumn(columnName);
                        if (column == null)
                        {
                            continue;
                        }

                        // Update column with JSON information (this is a heuristic, not definitive)
                        if (column is Column concreteColumn)
                        {
                            concreteColumn.SetExtendedProperty("PotentialJsonColumn", "true");
                            logger.LogDebug("Marked potential JSON column: {Schema}.{Table}.{Column}", 
                                schemaName, tableName, columnName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error identifying JSON columns: {Message}", ex.Message);
            }
        }
    }
}
