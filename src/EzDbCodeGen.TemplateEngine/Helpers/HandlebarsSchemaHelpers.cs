using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.CoreHelpers;
using HandlebarsDotNet;
using HandlebarsDotNet.IO;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.TemplateEngine.Helpers;

/// <summary>
/// Provides Handlebars helpers for working with database schemas.
/// </summary>
public class HandlebarsSchemaHelpers : ISchemaHelpers, IHelperRegistration
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlebarsSchemaHelpers"/> class.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    public HandlebarsSchemaHelpers(ILogger? logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void RegisterHelpers(ITemplateEngine templateEngine)
    {
        if (templateEngine == null)
        {
            throw new ArgumentNullException(nameof(templateEngine));
        }

        _logger?.LogDebug("Registering schema helpers");
        
        // Basic schema helpers
        RegisterSchemaHelpers(templateEngine);
        
        // Relationship helpers
        RegisterRelationshipHelpers(templateEngine);
        
        // Naming convention helpers
        RegisterNamingHelpers(templateEngine);
        
        _logger?.LogInformation("Registered all schema helpers");
    }
    
    private void RegisterSchemaHelpers(ITemplateEngine templateEngine)
    {
        // Returns schema info
        templateEngine.RegisterHelper("schemaInfo", (EncodedTextWriter writer, Context context, Arguments parameters) =>
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
            string databaseName = schema.ContainsKey("DatabaseName") ? schema["DatabaseName"]?.ToString() ?? "Unknown" : "Unknown";
            
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
        templateEngine.RegisterHelper("getTable", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getTable}} helper requires exactly two arguments: schema and tableName");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var tableName = parameters[1]?.ToString() ?? "";
            
            if (schema == null || !schema.ContainsKey("Tables"))
            {
                return;
            }
            
            var tables = schema["Tables"] as IEnumerable<object>;
            if (tables == null)
            {
                return;
            }
            
            foreach (var table in tables)
            {
                var tableDict = table as IDictionary<string, object>;
                if (tableDict != null && tableDict.ContainsKey("Name") && tableDict["Name"]?.ToString() == tableName)
                {
                    writer.WriteSafeString(table.ToString());
                    return;
                }
            }
        });
        
        // Helper to retrieve a view by name
        templateEngine.RegisterHelper("getView", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 2)
            {
                throw new HandlebarsException("{{getView}} helper requires exactly two arguments: schema and viewName");
            }

            var schema = parameters[0] as IDictionary<string, object>;
            var viewName = parameters[1]?.ToString() ?? "";
            
            if (schema == null || !schema.ContainsKey("Views"))
            {
                return;
            }
            
            var views = schema["Views"] as IEnumerable<object>;
            if (views == null)
            {
                return;
            }
            
            foreach (var view in views)
            {
                var viewDict = view as IDictionary<string, object>;
                if (viewDict != null && viewDict.ContainsKey("Name") && viewDict["Name"]?.ToString() == viewName)
                {
                    writer.WriteSafeString(view.ToString());
                    return;
                }
            }
        });

        // Additional schema helpers would be implemented here...
    }
    
    private void RegisterRelationshipHelpers(ITemplateEngine templateEngine)
    {
        // Helper to get foreign keys for a table
        templateEngine.RegisterHelper("getForeignKeys", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{getForeignKeys}} helper requires exactly one argument: table");
            }

            var table = parameters[0] as IDictionary<string, object>;
            if (table == null || !table.ContainsKey("ForeignKeys"))
            {
                return;
            }
            
            var foreignKeys = table["ForeignKeys"] as IEnumerable<object>;
            if (foreignKeys != null)
            {
                writer.WriteSafeString(string.Join(", ", foreignKeys));
            }
        });
        
        // Helper to get primary key columns for a table
        templateEngine.RegisterHelper("getPrimaryKeyColumns", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{getPrimaryKeyColumns}} helper requires exactly one argument: table");
            }

            var table = parameters[0] as IDictionary<string, object>;
            if (table == null || !table.ContainsKey("PrimaryKeyColumns"))
            {
                return;
            }
            
            var pkColumns = table["PrimaryKeyColumns"] as IEnumerable<object>;
            if (pkColumns != null)
            {
                writer.WriteSafeString(string.Join(", ", pkColumns));
            }
        });
        
        // Helper to check if a table has any foreign keys
        templateEngine.RegisterHelper("hasForeignKeys", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{hasForeignKeys}} helper requires exactly one argument: table");
            }

            var table = parameters[0] as IDictionary<string, object>;
            if (table == null || !table.ContainsKey("ForeignKeys"))
            {
                writer.WriteSafeString("false");
                return;
            }
            
            var foreignKeys = table["ForeignKeys"] as IEnumerable<object>;
            var hasKeys = foreignKeys != null && foreignKeys.Any();
            writer.WriteSafeString(hasKeys.ToString());
        });

        // Additional relationship helpers would be implemented here...
    }
    
    private void RegisterNamingHelpers(ITemplateEngine templateEngine)
    {
        // Helper to convert to PascalCase
        templateEngine.RegisterHelper("pascalCase", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{pascalCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
            }

            // Handle underscore or space separated inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to title case
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            
            writer.WriteSafeString(string.Join("", words));
        });
        
        // Helper to convert to camelCase
        templateEngine.RegisterHelper("camelCase", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{camelCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
            }

            // First convert to PascalCase
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to title case
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            
            var pascalCase = string.Join("", words);
            
            // Then convert first character to lowercase
            if (pascalCase.Length > 0)
            {
                writer.WriteSafeString(char.ToLower(pascalCase[0]) + pascalCase.Substring(1));
            }
            else
            {
                writer.WriteSafeString(pascalCase);
            }
        });
        
        // Helper to convert to snake_case
        templateEngine.RegisterHelper("snakeCase", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{snakeCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
            }

            // Handle underscore or space separated inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to lowercase
            for (int i = 0; i <words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            writer.WriteSafeString(string.Join("_", words));
        });
        
        // Helper to convert to kebab-case (for file names)
        templateEngine.RegisterHelper("kebabCase", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{kebabCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
            }

            // Handle underscore, space, or camelCase inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to lowercase
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            writer.WriteSafeString(string.Join("-", words));
        });
        
        // Helper to convert to SCREAMING_SNAKE_CASE (for constants)
        templateEngine.RegisterHelper("screamingSnakeCase", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{screamingSnakeCase}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
            }

            // First convert to snake_case
            // Handle underscore or space separated inputs
            var words = input.Split(new[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Convert to lowercase
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].ToLower();
            }
            
            var snakeCase = string.Join("_", words);
            
            // Then convert to uppercase
            writer.WriteSafeString(snakeCase.ToUpper());
        });
        
        // Helper to sanitize a name for use as a code identifier
        templateEngine.RegisterHelper("sanitizeName", (EncodedTextWriter writer, Context context, Arguments parameters) =>
        {
            if (parameters.Length != 1)
            {
                throw new HandlebarsException("{{sanitizeName}} helper requires exactly one argument: input");
            }

            var input = parameters[0]?.ToString() ?? "";
            if (string.IsNullOrEmpty(input))
            {
                writer.WriteSafeString(input);
                return;
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
            
            writer.WriteSafeString(result);
        });
    }
}
