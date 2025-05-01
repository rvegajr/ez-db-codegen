using System;
using System.Collections.Generic;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using EzDbCodeGen.TemplateEngine.Interfaces;
using HandlebarsDotNet;

namespace EzDbCodeGen.TemplateEngine
{
    /// <summary>
    /// Provides type conversion helpers for Handlebars templates.
    /// </summary>
    public class HandlebarsTypeConversionHelpers : ITypeConversionHelpers, IHelperRegistration
    {
        private static readonly Dictionary<string, Dictionary<string, string>> TypeMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            ["csharp"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // SQL Server types to C# types
                ["bigint"] = "long",
                ["bit"] = "bool",
                ["decimal"] = "decimal",
                ["int"] = "int",
                ["money"] = "decimal",
                ["numeric"] = "decimal",
                ["smallint"] = "short",
                ["smallmoney"] = "decimal",
                ["tinyint"] = "byte",
                ["float"] = "double",
                ["real"] = "float",
                ["date"] = "DateTime",
                ["datetime"] = "DateTime",
                ["datetime2"] = "DateTime",
                ["datetimeoffset"] = "DateTimeOffset",
                ["smalldatetime"] = "DateTime",
                ["time"] = "TimeSpan",
                ["char"] = "string",
                ["nchar"] = "string",
                ["ntext"] = "string",
                ["nvarchar"] = "string",
                ["text"] = "string",
                ["varchar"] = "string",
                ["binary"] = "byte[]",
                ["image"] = "byte[]",
                ["varbinary"] = "byte[]",
                ["rowversion"] = "byte[]",
                ["timestamp"] = "byte[]",
                ["uniqueidentifier"] = "Guid",
                ["geography"] = "Microsoft.SqlServer.Types.SqlGeography",
                ["geometry"] = "Microsoft.SqlServer.Types.SqlGeometry",
                ["hierarchyid"] = "Microsoft.SqlServer.Types.SqlHierarchyId",
                ["xml"] = "string"
            },
            ["typescript"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // SQL Server types to TypeScript types
                ["bigint"] = "number",
                ["bit"] = "boolean",
                ["decimal"] = "number",
                ["int"] = "number",
                ["money"] = "number",
                ["numeric"] = "number",
                ["smallint"] = "number",
                ["smallmoney"] = "number",
                ["tinyint"] = "number",
                ["float"] = "number",
                ["real"] = "number",
                ["date"] = "Date",
                ["datetime"] = "Date",
                ["datetime2"] = "Date",
                ["datetimeoffset"] = "Date",
                ["smalldatetime"] = "Date",
                ["time"] = "string",
                ["char"] = "string",
                ["nchar"] = "string",
                ["ntext"] = "string",
                ["nvarchar"] = "string",
                ["text"] = "string",
                ["varchar"] = "string",
                ["binary"] = "ArrayBuffer",
                ["image"] = "ArrayBuffer",
                ["varbinary"] = "ArrayBuffer",
                ["rowversion"] = "ArrayBuffer",
                ["timestamp"] = "ArrayBuffer",
                ["uniqueidentifier"] = "string",
                ["geography"] = "any",
                ["geometry"] = "any",
                ["hierarchyid"] = "any",
                ["xml"] = "string"
            },
            ["java"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // SQL Server types to Java types
                ["bigint"] = "Long",
                ["bit"] = "Boolean",
                ["decimal"] = "BigDecimal",
                ["int"] = "Integer",
                ["money"] = "BigDecimal",
                ["numeric"] = "BigDecimal",
                ["smallint"] = "Short",
                ["smallmoney"] = "BigDecimal",
                ["tinyint"] = "Byte",
                ["float"] = "Double",
                ["real"] = "Float",
                ["date"] = "LocalDate",
                ["datetime"] = "LocalDateTime",
                ["datetime2"] = "LocalDateTime",
                ["datetimeoffset"] = "OffsetDateTime",
                ["smalldatetime"] = "LocalDateTime",
                ["time"] = "LocalTime",
                ["char"] = "String",
                ["nchar"] = "String",
                ["ntext"] = "String",
                ["nvarchar"] = "String",
                ["text"] = "String",
                ["varchar"] = "String",
                ["binary"] = "byte[]",
                ["image"] = "byte[]",
                ["varbinary"] = "byte[]",
                ["rowversion"] = "byte[]",
                ["timestamp"] = "byte[]",
                ["uniqueidentifier"] = "UUID",
                ["geography"] = "Object",
                ["geometry"] = "Object",
                ["hierarchyid"] = "Object",
                ["xml"] = "String"
            },
            ["python"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // SQL Server types to Python types
                ["bigint"] = "int",
                ["bit"] = "bool",
                ["decimal"] = "Decimal",
                ["int"] = "int",
                ["money"] = "Decimal",
                ["numeric"] = "Decimal",
                ["smallint"] = "int",
                ["smallmoney"] = "Decimal",
                ["tinyint"] = "int",
                ["float"] = "float",
                ["real"] = "float",
                ["date"] = "date",
                ["datetime"] = "datetime",
                ["datetime2"] = "datetime",
                ["datetimeoffset"] = "datetime",
                ["smalldatetime"] = "datetime",
                ["time"] = "time",
                ["char"] = "str",
                ["nchar"] = "str",
                ["ntext"] = "str",
                ["nvarchar"] = "str",
                ["text"] = "str",
                ["varchar"] = "str",
                ["binary"] = "bytes",
                ["image"] = "bytes",
                ["varbinary"] = "bytes",
                ["rowversion"] = "bytes",
                ["timestamp"] = "bytes",
                ["uniqueidentifier"] = "UUID",
                ["geography"] = "Any",
                ["geometry"] = "Any",
                ["hierarchyid"] = "Any",
                ["xml"] = "str"
            }
        };

        /// <inheritdoc/>
        public void RegisterHelpers(ITemplateEngine templateEngine)
        {
            // Register ToCSharpType helper
            templateEngine.RegisterHelper("toCSharpType", (context) => {
                if (context == null) return string.Empty;
                return ConvertType(context.ToString(), "csharp");
            });

            // Register ToTypescriptType helper
            templateEngine.RegisterHelper("toTypeScriptType", (context) => {
                if (context == null) return string.Empty;
                return ConvertType(context.ToString(), "typescript");
            });

            // Register ToJavaType helper
            templateEngine.RegisterHelper("toJavaType", (context) => {
                if (context == null) return string.Empty;
                return ConvertType(context.ToString(), "java");
            });

            // Register ToPythonType helper
            templateEngine.RegisterHelper("toPythonType", (context) => {
                if (context == null) return string.Empty;
                return ConvertType(context.ToString(), "python");
            });

            // Register ConvertType helper
            templateEngine.RegisterHelper("convertType", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var dbType = arguments[0]?.ToString() ?? string.Empty;
                var targetLanguage = arguments[1]?.ToString() ?? "csharp";
                
                return ConvertType(dbType, targetLanguage);
            });

            // Register Nullable helper
            templateEngine.RegisterHelper("nullable", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var isNullable = false;
                
                if (arguments[1] is bool boolValue)
                {
                    isNullable = boolValue;
                }
                else if (bool.TryParse(arguments[1]?.ToString(), out var parsedBool))
                {
                    isNullable = parsedBool;
                }
                
                var targetLanguage = "csharp";
                if (arguments.Length >= 3)
                {
                    targetLanguage = arguments[2]?.ToString() ?? "csharp";
                }
                
                return MakeNullable(type, isNullable, targetLanguage);
            });
            
            // Register ShortNullable helper aliases
            RegisterNullableHelpers(templateEngine);
        }

        private void RegisterNullableHelpers(ITemplateEngine templateEngine)
        {
            // C# nullable helper
            templateEngine.RegisterHelper("nullableCSharp", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var isNullable = false;
                
                if (arguments[1] is bool boolValue)
                {
                    isNullable = boolValue;
                }
                else if (bool.TryParse(arguments[1]?.ToString(), out var parsedBool))
                {
                    isNullable = parsedBool;
                }
                
                return MakeNullable(type, isNullable, "csharp");
            });
            
            // TypeScript nullable helper
            templateEngine.RegisterHelper("nullableTypeScript", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var isNullable = false;
                
                if (arguments[1] is bool boolValue)
                {
                    isNullable = boolValue;
                }
                else if (bool.TryParse(arguments[1]?.ToString(), out var parsedBool))
                {
                    isNullable = parsedBool;
                }
                
                return MakeNullable(type, isNullable, "typescript");
            });
            
            // Java nullable helper
            templateEngine.RegisterHelper("nullableJava", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var isNullable = false;
                
                if (arguments[1] is bool boolValue)
                {
                    isNullable = boolValue;
                }
                else if (bool.TryParse(arguments[1]?.ToString(), out var parsedBool))
                {
                    isNullable = parsedBool;
                }
                
                return MakeNullable(type, isNullable, "java");
            });
            
            // Python nullable helper (optional typing)
            templateEngine.RegisterHelper("nullablePython", (context, options, arguments, blockParams) => {
                if (arguments.Length < 2) return string.Empty;
                
                var type = arguments[0]?.ToString() ?? string.Empty;
                var isNullable = false;
                
                if (arguments[1] is bool boolValue)
                {
                    isNullable = boolValue;
                }
                else if (bool.TryParse(arguments[1]?.ToString(), out var parsedBool))
                {
                    isNullable = parsedBool;
                }
                
                return MakeNullable(type, isNullable, "python");
            });
        }

        private string ConvertType(string dbType, string targetLanguage)
        {
            if (string.IsNullOrEmpty(dbType)) return string.Empty;
            
            // Extract the base type without size or precision info
            var baseType = dbType.Split('(')[0].Trim();
            
            // Try to find the type mapping
            if (TypeMappings.TryGetValue(targetLanguage.ToLowerInvariant(), out var mappings) && 
                mappings.TryGetValue(baseType, out var mappedType))
            {
                return mappedType;
            }
            
            // Default to string if no mapping found
            return targetLanguage.ToLowerInvariant() switch
            {
                "csharp" => "string",
                "typescript" => "string",
                "java" => "String",
                "python" => "str",
                _ => "string"
            };
        }

        private string MakeNullable(string type, bool isNullable, string targetLanguage)
        {
            if (string.IsNullOrEmpty(type)) return string.Empty;
            
            if (!isNullable) return type;
            
            return targetLanguage.ToLowerInvariant() switch
            {
                "csharp" => type switch
                {
                    "string" => "string",  // String is already nullable in C#
                    "byte[]" => "byte[]",  // Arrays are already nullable in C#
                    _ => $"{type}?"        // Add ? for value types
                },
                "typescript" => $"{type} | null",
                "java" => IsJavaPrimitive(type) ? GetJavaWrapperType(type) : type,
                "python" => $"Optional[{type}]",
                _ => type
            };
        }

        private bool IsJavaPrimitive(string type)
        {
            return type switch
            {
                "boolean" or "byte" or "short" or "int" or "long" or "float" or "double" or "char" => true,
                _ => false
            };
        }

        private string GetJavaWrapperType(string primitiveType)
        {
            return primitiveType switch
            {
                "boolean" => "Boolean",
                "byte" => "Byte",
                "short" => "Short",
                "int" => "Integer",
                "long" => "Long",
                "float" => "Float",
                "double" => "Double",
                "char" => "Character",
                _ => primitiveType
            };
        }
    }
}
