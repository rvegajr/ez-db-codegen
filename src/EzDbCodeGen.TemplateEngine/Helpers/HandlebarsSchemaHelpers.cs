using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.TemplateEngine.Helpers;

/// <summary>
/// Provides Handlebars helpers for working with database schemas.
/// </summary>
public static class HandlebarsSchemaHelpers
{
    /// <summary>
    /// Registers all schema helpers with the Handlebars context.
    /// </summary>
    /// <param name="handlebars">The Handlebars context.</param>
    /// <param name="logger">The logger.</param>
    public static void RegisterAll(IHandlebars handlebars, ILogger? logger = null)
    {
        if (handlebars == null)
        {
            throw new ArgumentNullException(nameof(handlebars));
        }

        logger?.LogDebug("Registering schema helpers");
        
        // Basic schema helpers
        RegisterSchemaHelpers(handlebars, logger);
        
        // Relationship helpers
        RegisterRelationshipHelpers(handlebars, logger);
        
        // Naming convention helpers
        RegisterNamingHelpers(handlebars, logger);
        
        logger?.LogInformation("Registered all schema helpers");
    }
    
    private static void RegisterSchemaHelpers(IHandlebars handlebars, ILogger? logger = null)
    {
        // Returns schema info
        handlebars.RegisterHelper("schemaInfo", (writer, context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{schemaInfo}} helper requires exactly one argument: schema");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            if (schema == null)
            {
                writer.WriteSafeString("Invalid schema object");
                return;
            }

            var sb = new StringBuilder();
            string databaseName = schema.ContainsKey("DatabaseName") ? schema["DatabaseName"].ToString() : "Unknown";
            
            int tableCount = schema.ContainsKey("TableCount") ? Convert.ToInt32(schema["TableCount"]) : 0;
            int viewCount = schema.ContainsKey("ViewCount") ? Convert.ToInt32(schema["ViewCount"]) : 0;
            int spCount = schema.ContainsKey("StoredProcedureCount") ? Convert.ToInt32(schema["StoredProcedureCount"]) : 0;
            int funcCount = schema.ContainsKey("FunctionCount") ? Convert.ToInt32(schema["FunctionCount"]) : 0;
            
            sb.AppendLine($"Database: {databaseName}");
            sb.AppendLine($"Tables: {tableCount}");
            sb.AppendLine($"Views: {viewCount}");
            sb.AppendLine($"Stored Procedures: {spCount}");
            sb.AppendLine($"Functions: {funcCount}");
            
            writer.WriteSafeString(sb.ToString());
        });
        
        // Helper to retrieve a table by name
        handlebars.RegisterHelper("getTable", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getTable}} helper requires exactly two arguments: schema and tableName");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var tableName = parameters[1].ToString();
            
            if (schema == null || !schema.ContainsKey("Tables"))
            {
                return null;
            }
            
            var tables = schema["Tables"] as IEnumerable<object>;
            if (tables == null)
            {
                return null;
            }
            
            foreach (var table in tables)
            {
                var tableDict = table as IDictionary<string, object>;
                if (tableDict != null && tableDict.ContainsKey("Name") && tableDict["Name"].ToString() == tableName)
                {
                    return table;
                }
            }
            
            return null;
        });
        
        // Helper to retrieve a view by name
        handlebars.RegisterHelper("getView", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getView}} helper requires exactly two arguments: schema and viewName");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var viewName = parameters[1].ToString();
            
            if (schema == null || !schema.ContainsKey("Views"))
            {
                return null;
            }
            
            var views = schema["Views"] as IEnumerable<object>;
            if (views == null)
            {
                return null;
            }
            
            foreach (var view in views)
            {
                var viewDict = view as IDictionary<string, object>;
                if (viewDict != null && viewDict.ContainsKey("Name") && viewDict["Name"].ToString() == viewName)
                {
                    return view;
                }
            }
            
            return null;
        });
        
        // Helper to check if a column is a primary key
        handlebars.RegisterHelper("isPrimaryKey", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{isPrimaryKey}} helper requires exactly two arguments: table and columnName");
            }

            var table = parameters[0] as IDictionary<string, object>;
            var columnName = parameters[1].ToString();
            
            if (table == null || !table.ContainsKey("PrimaryKey") || table["PrimaryKey"] == null)
            {
                return false;
            }
            
            var primaryKey = table["PrimaryKey"] as IDictionary<string, object>;
            if (primaryKey == null || !primaryKey.ContainsKey("Columns"))
            {
                return false;
            }
            
            var keyColumns = primaryKey["Columns"] as IEnumerable<object>;
            if (keyColumns == null)
            {
                return false;
            }
            
            foreach (var column in keyColumns)
            {
                var columnDict = column as IDictionary<string, object>;
                if (columnDict != null && columnDict.ContainsKey("Name") && columnDict["Name"].ToString() == columnName)
                {
                    return true;
                }
            }
            
            return false;
        });
        
        // Helper to check if a column is a foreign key
        handlebars.RegisterHelper("isForeignKey", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{isForeignKey}} helper requires exactly two arguments: table and columnName");
            }

            var table = parameters[0] as IDictionary<string, object>;
            var columnName = parameters[1].ToString();
            
            if (table == null || !table.ContainsKey("ForeignKeys"))
            {
                return false;
            }
            
            var foreignKeys = table["ForeignKeys"] as IEnumerable<object>;
            if (foreignKeys == null)
            {
                return false;
            }
            
            foreach (var fk in foreignKeys)
            {
                var fkDict = fk as IDictionary<string, object>;
                if (fkDict == null || !fkDict.ContainsKey("ColumnPairs"))
                {
                    continue;
                }
                
                var columnPairs = fkDict["ColumnPairs"] as IEnumerable<object>;
                if (columnPairs == null)
                {
                    continue;
                }
                
                foreach (var pair in columnPairs)
                {
                    var pairDict = pair as IDictionary<string, object>;
                    if (pairDict != null && pairDict.ContainsKey("ColumnName") && pairDict["ColumnName"].ToString() == columnName)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        });
        
        // Helper to get foreign key referencing information
        handlebars.RegisterHelper("getForeignKeyInfo", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getForeignKeyInfo}} helper requires exactly two arguments: table and columnName");
            }

            var table = parameters[0] as IDictionary<string, object>;
            var columnName = parameters[1].ToString();
            
            if (table == null || !table.ContainsKey("ForeignKeys"))
            {
                return null;
            }
            
            var foreignKeys = table["ForeignKeys"] as IEnumerable<object>;
            if (foreignKeys == null)
            {
                return null;
            }
            
            foreach (var fk in foreignKeys)
            {
                var fkDict = fk as IDictionary<string, object>;
                if (fkDict == null || !fkDict.ContainsKey("ColumnPairs"))
                {
                    continue;
                }
                
                var columnPairs = fkDict["ColumnPairs"] as IEnumerable<object>;
                if (columnPairs == null)
                {
                    continue;
                }
                
                foreach (var pair in columnPairs)
                {
                    var pairDict = pair as IDictionary<string, object>;
                    if (pairDict != null && pairDict.ContainsKey("ColumnName") && pairDict["ColumnName"].ToString() == columnName)
                    {
                        return fkDict;
                    }
                }
            }
            
            return null;
        });
    }
    
    private static void RegisterRelationshipHelpers(IHandlebars handlebars, ILogger? logger = null)
    {
        // Helper to get all relationships of a specific type
        handlebars.RegisterHelper("getRelationships", (context, parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getRelationships}} helper requires exactly two arguments: schema and relationshipType");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var relationshipType = parameters[1].ToString();
            
            if (schema == null || !schema.ContainsKey("Relationships"))
            {
                return new object[0];
            }
            
            var relationships = schema["Relationships"] as IEnumerable<object>;
            if (relationships == null)
            {
                return new object[0];
            }
            
            var result = new List<object>();
            foreach (var rel in relationships)
            {
                var relDict = rel as IDictionary<string, object>;
                if (relDict != null && relDict.ContainsKey("Type") && relDict["Type"].ToString() == relationshipType)
                {
                    result.Add(rel);
                }
            }
            
            return result;
        });
        
        // Helper to check if a table has a specific relationship type
        handlebars.RegisterHelper("hasRelationship", (context, parameters) =>
        {
            if (parameters.Length != 3)
            {
                throw new HandlebarsException("{{hasRelationship}} helper requires exactly three arguments: schema, tableName, and relationshipType");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var tableName = parameters[1].ToString();
            var relationshipType = parameters[2].ToString();
            
            if (schema == null || !schema.ContainsKey("Relationships"))
            {
                return false;
            }
            
            var relationships = schema["Relationships"] as IEnumerable<object>;
            if (relationships == null)
            {
                return false;
            }
            
            foreach (var rel in relationships)
            {
                var relDict = rel as IDictionary<string, object>;
                if (relDict == null || !relDict.ContainsKey("Type") || !relDict.ContainsKey("Description"))
                {
                    continue;
                }
                
                if (relDict["Type"].ToString() == relationshipType)
                {
                    var description = relDict["Description"].ToString();
                    if (description.Contains(tableName))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        });
        
        // Helper to get tables that have a relationship with the given table
        handlebars.RegisterHelper("getRelatedTables", (context, parameters) =>
        {
            if (parameters.Length != 2 && parameters.Length != 3)
            {
                throw new HandlebarsException("{{getRelatedTables}} helper requires two or three arguments: schema, tableName, and optional relationshipType");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var tableName = parameters[1].ToString();
            var relationshipType = parameters.Length > 2 ? parameters[2].ToString() : null;
            
            if (schema == null || !schema.ContainsKey("Relationships") || !schema.ContainsKey("Tables"))
            {
                return new object[0];
            }
            
            var relationships = schema["Relationships"] as IEnumerable<object>;
            var tables = schema["Tables"] as IEnumerable<object>;
            
            if (relationships == null || tables == null)
            {
                return new object[0];
            }
            
            var relatedTableNames = new HashSet<string>();
            
            foreach (var rel in relationships)
            {
                var relDict = rel as IDictionary<string, object>;
                if (relDict == null || !relDict.ContainsKey("Description"))
                {
                    continue;
                }
                
                if (relationshipType != null && (!relDict.ContainsKey("Type") || relDict["Type"].ToString() != relationshipType))
                {
                    continue;
                }
                
                var description = relDict["Description"].ToString();
                if (description.Contains(tableName))
                {
                    // Extract other table names from the description
                    var parts = description.Split(' ');
                    foreach (var part in parts)
                    {
                        // Skip non-table parts
                        if (part != tableName && !part.StartsWith("from") && !part.StartsWith("to") && 
                            !part.StartsWith("between") && !part.StartsWith("via") && !part.StartsWith("in") &&
                            !part.StartsWith("and") && !part.Contains("-to-"))
                        {
                            // Clean up the name from any punctuation
                            var cleanName = part.Trim('.', ',', ':', ';');
                            if (!string.IsNullOrEmpty(cleanName) && cleanName != tableName)
                            {
                                relatedTableNames.Add(cleanName);
                            }
                        }
                    }
                }
            }
            
            var relatedTables = new List<object>();
            foreach (var table in tables)
            {
                var tableDict = table as IDictionary<string, object>;
                if (tableDict != null && tableDict.ContainsKey("Name") && 
                    relatedTableNames.Contains(tableDict["Name"].ToString()))
                {
                    relatedTables.Add(table);
                }
            }
            
            return relatedTables;
        });
        
        // Helper for determining navigation property name
        handlebars.RegisterHelper("navigationPropertyName", (context, parameters) =>
        {
            if (parameters.Length != 2 && parameters.Length != 3)
            {
                throw new HandlebarsException("{{navigationPropertyName}} helper requires two or three arguments: tableName, isManyRelationship, and optional suffix");
            }

            var tableName = parameters[0].ToString();
            var isManyRelationship = Convert.ToBoolean(parameters[1]);
            var suffix = parameters.Length > 2 ? parameters[2].ToString() : "";
            
            // Remove common prefixes/suffixes that shouldn't be in property names
            tableName = tableName.Replace("tbl", "").Replace("Tbl", "");
            
            // Convert to PascalCase
            if (!string.IsNullOrEmpty(tableName) && char.IsLower(tableName[0]))
            {
                tableName = char.ToUpper(tableName[0]) + (tableName.Length > 1 ? tableName.Substring(1) : "");
            }
            
            // For many relationships, pluralize the name
            if (isManyRelationship)
            {
                // Simple pluralization - add "s" or "es"
                if (tableName.EndsWith("s") || tableName.EndsWith("x") || 
                    tableName.EndsWith("z") || tableName.EndsWith("ch") || 
                    tableName.EndsWith("sh"))
                {
                    tableName += "es";
                }
                else if (tableName.EndsWith("y") && !"aeiou".Contains(char.ToLower(tableName[tableName.Length - 2])))
                {
                    tableName = tableName.Substring(0, tableName.Length - 1) + "ies";
                }
                else
                {
                    tableName += "s";
                }
            }
            
            return tableName + suffix;
        });
    }
    
    private static void RegisterNamingHelpers(IHandlebars handlebars, ILogger? logger = null)
    {
        // Helper to convert to PascalCase (for class names)
        handlebars.RegisterHelper("pascalCase", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{pascalCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Handle underscore or space separated inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Capitalize the first letter of each word
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + (words[i].Length > 1 ? words[i].Substring(1) : "");
                }
            }
            
            return string.Join("", words);
        });
        
        // Helper to convert to camelCase (for property/variable names)
        handlebars.RegisterHelper("camelCase", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{camelCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // First convert to PascalCase
            var pascalCase = handlebars.Helpers["pascalCase"](context, new[] { input });
            
            // Then convert to camelCase
            return char.ToLower(pascalCase.ToString()[0]) + 
                   (pascalCase.ToString().Length > 1 ? pascalCase.ToString().Substring(1) : "");
        });
        
        // Helper to convert to snake_case (for some database/file names)
        handlebars.RegisterHelper("snakeCase", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{snakeCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Handle underscore or space separated inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to lowercase
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            return string.Join("_", words);
        });
        
        // Helper to convert to kebab-case (for file names)
        handlebars.RegisterHelper("kebabCase", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{kebabCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Handle underscore, space, or camelCase inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to lowercase
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            return string.Join("-", words);
        });
        
        // Helper to convert to SCREAMING_SNAKE_CASE (for constants)
        handlebars.RegisterHelper("screamingSnakeCase", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{screamingSnakeCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // First convert to snake_case
            var snakeCase = handlebars.Helpers["snakeCase"](context, new[] { input });
            
            // Then convert to uppercase
            return snakeCase.ToString().ToUpper();
        });
        
        // Helper to sanitize a name for use as a code identifier
        handlebars.RegisterHelper("sanitizeName", (context, parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{sanitizeName}} helper requires exactly one argument: input");
            }

            var input = parameters[0].ToString();
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            // Remove invalid characters
            var validChars = new char[input.Length];
            int len = 0;
            
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetterOrDigit(input[i]) || input[i] == '_')
                {
                    validChars[len++] = input[i];
                }
            }
            
            var result = new string(validChars, 0, len);
            
            // Ensure it starts with a letter or underscore
            if (result.Length > 0 && !char.IsLetter(result[0]) && result[0] != '_')
            {
                result = "_" + result;
            }
            
            return result;
        });
    }
}
