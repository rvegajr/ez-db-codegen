using EzDbCodeGen.CodeGen.Interfaces.TypeMapping;

namespace EzDbCodeGen.TypeMapping;

/// <summary>
/// Implementation of IDataTypeMap for SQL Server database types.
/// </summary>
public class SqlServerDataTypeMap : DataTypeMap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlServerDataTypeMap"/> class.
    /// </summary>
    public SqlServerDataTypeMap() : base()
    {
    }

    /// <summary>
    /// Initializes the type mappings for SQL Server to various programming languages.
    /// </summary>
    protected override void InitializeTypeMappings()
    {
        // C# mappings
        AddTypeMapping("bit", "csharp", "bool");
        AddTypeMapping("tinyint", "csharp", "byte");
        AddTypeMapping("smallint", "csharp", "short");
        AddTypeMapping("int", "csharp", "int");
        AddTypeMapping("bigint", "csharp", "long");
        AddTypeMapping("decimal", "csharp", "decimal");
        AddTypeMapping("numeric", "csharp", "decimal");
        AddTypeMapping("smallmoney", "csharp", "decimal");
        AddTypeMapping("money", "csharp", "decimal");
        AddTypeMapping("float", "csharp", "double");
        AddTypeMapping("real", "csharp", "float");
        AddTypeMapping("date", "csharp", "DateTime");
        AddTypeMapping("datetime", "csharp", "DateTime");
        AddTypeMapping("datetime2", "csharp", "DateTime");
        AddTypeMapping("smalldatetime", "csharp", "DateTime");
        AddTypeMapping("datetimeoffset", "csharp", "DateTimeOffset");
        AddTypeMapping("time", "csharp", "TimeSpan");
        AddTypeMapping("char", "csharp", "string");
        AddTypeMapping("varchar", "csharp", "string");
        AddTypeMapping("text", "csharp", "string");
        AddTypeMapping("nchar", "csharp", "string");
        AddTypeMapping("nvarchar", "csharp", "string");
        AddTypeMapping("ntext", "csharp", "string");
        AddTypeMapping("binary", "csharp", "byte[]");
        AddTypeMapping("varbinary", "csharp", "byte[]");
        AddTypeMapping("image", "csharp", "byte[]");
        AddTypeMapping("rowversion", "csharp", "byte[]");
        AddTypeMapping("timestamp", "csharp", "byte[]");
        AddTypeMapping("uniqueidentifier", "csharp", "Guid");
        AddTypeMapping("sql_variant", "csharp", "object");
        AddTypeMapping("xml", "csharp", "string");
        AddTypeMapping("hierarchyid", "csharp", "string");
        AddTypeMapping("geography", "csharp", "object");
        AddTypeMapping("geometry", "csharp", "object");

        // TypeScript mappings
        AddTypeMapping("bit", "typescript", "boolean");
        AddTypeMapping("tinyint", "typescript", "number");
        AddTypeMapping("smallint", "typescript", "number");
        AddTypeMapping("int", "typescript", "number");
        AddTypeMapping("bigint", "typescript", "number");
        AddTypeMapping("decimal", "typescript", "number");
        AddTypeMapping("numeric", "typescript", "number");
        AddTypeMapping("smallmoney", "typescript", "number");
        AddTypeMapping("money", "typescript", "number");
        AddTypeMapping("float", "typescript", "number");
        AddTypeMapping("real", "typescript", "number");
        AddTypeMapping("date", "typescript", "Date");
        AddTypeMapping("datetime", "typescript", "Date");
        AddTypeMapping("datetime2", "typescript", "Date");
        AddTypeMapping("smalldatetime", "typescript", "Date");
        AddTypeMapping("datetimeoffset", "typescript", "Date");
        AddTypeMapping("time", "typescript", "string");
        AddTypeMapping("char", "typescript", "string");
        AddTypeMapping("varchar", "typescript", "string");
        AddTypeMapping("text", "typescript", "string");
        AddTypeMapping("nchar", "typescript", "string");
        AddTypeMapping("nvarchar", "typescript", "string");
        AddTypeMapping("ntext", "typescript", "string");
        AddTypeMapping("binary", "typescript", "Uint8Array");
        AddTypeMapping("varbinary", "typescript", "Uint8Array");
        AddTypeMapping("image", "typescript", "Uint8Array");
        AddTypeMapping("rowversion", "typescript", "Uint8Array");
        AddTypeMapping("timestamp", "typescript", "Uint8Array");
        AddTypeMapping("uniqueidentifier", "typescript", "string");
        AddTypeMapping("sql_variant", "typescript", "any");
        AddTypeMapping("xml", "typescript", "string");
        AddTypeMapping("hierarchyid", "typescript", "string");
        AddTypeMapping("geography", "typescript", "any");
        AddTypeMapping("geometry", "typescript", "any");

        // Java mappings
        AddTypeMapping("bit", "java", "Boolean");
        AddTypeMapping("tinyint", "java", "Byte");
        AddTypeMapping("smallint", "java", "Short");
        AddTypeMapping("int", "java", "Integer");
        AddTypeMapping("bigint", "java", "Long");
        AddTypeMapping("decimal", "java", "BigDecimal");
        AddTypeMapping("numeric", "java", "BigDecimal");
        AddTypeMapping("smallmoney", "java", "BigDecimal");
        AddTypeMapping("money", "java", "BigDecimal");
        AddTypeMapping("float", "java", "Double");
        AddTypeMapping("real", "java", "Float");
        AddTypeMapping("date", "java", "LocalDate");
        AddTypeMapping("datetime", "java", "LocalDateTime");
        AddTypeMapping("datetime2", "java", "LocalDateTime");
        AddTypeMapping("smalldatetime", "java", "LocalDateTime");
        AddTypeMapping("datetimeoffset", "java", "OffsetDateTime");
        AddTypeMapping("time", "java", "LocalTime");
        AddTypeMapping("char", "java", "String");
        AddTypeMapping("varchar", "java", "String");
        AddTypeMapping("text", "java", "String");
        AddTypeMapping("nchar", "java", "String");
        AddTypeMapping("nvarchar", "java", "String");
        AddTypeMapping("ntext", "java", "String");
        AddTypeMapping("binary", "java", "byte[]");
        AddTypeMapping("varbinary", "java", "byte[]");
        AddTypeMapping("image", "java", "byte[]");
        AddTypeMapping("rowversion", "java", "byte[]");
        AddTypeMapping("timestamp", "java", "byte[]");
        AddTypeMapping("uniqueidentifier", "java", "UUID");
        AddTypeMapping("sql_variant", "java", "Object");
        AddTypeMapping("xml", "java", "String");
        AddTypeMapping("hierarchyid", "java", "String");
        AddTypeMapping("geography", "java", "Object");
        AddTypeMapping("geometry", "java", "Object");

        // Python mappings
        AddTypeMapping("bit", "python", "bool");
        AddTypeMapping("tinyint", "python", "int");
        AddTypeMapping("smallint", "python", "int");
        AddTypeMapping("int", "python", "int");
        AddTypeMapping("bigint", "python", "int");
        AddTypeMapping("decimal", "python", "Decimal");
        AddTypeMapping("numeric", "python", "Decimal");
        AddTypeMapping("smallmoney", "python", "Decimal");
        AddTypeMapping("money", "python", "Decimal");
        AddTypeMapping("float", "python", "float");
        AddTypeMapping("real", "python", "float");
        AddTypeMapping("date", "python", "date");
        AddTypeMapping("datetime", "python", "datetime");
        AddTypeMapping("datetime2", "python", "datetime");
        AddTypeMapping("smalldatetime", "python", "datetime");
        AddTypeMapping("datetimeoffset", "python", "datetime");
        AddTypeMapping("time", "python", "time");
        AddTypeMapping("char", "python", "str");
        AddTypeMapping("varchar", "python", "str");
        AddTypeMapping("text", "python", "str");
        AddTypeMapping("nchar", "python", "str");
        AddTypeMapping("nvarchar", "python", "str");
        AddTypeMapping("ntext", "python", "str");
        AddTypeMapping("binary", "python", "bytes");
        AddTypeMapping("varbinary", "python", "bytes");
        AddTypeMapping("image", "python", "bytes");
        AddTypeMapping("rowversion", "python", "bytes");
        AddTypeMapping("timestamp", "python", "bytes");
        AddTypeMapping("uniqueidentifier", "python", "str");
        AddTypeMapping("sql_variant", "python", "Any");
        AddTypeMapping("xml", "python", "str");
        AddTypeMapping("hierarchyid", "python", "str");
        AddTypeMapping("geography", "python", "Any");
        AddTypeMapping("geometry", "python", "Any");
    }

    /// <summary>
    /// Initializes the default values for SQL Server types in various programming languages.
    /// </summary>
    protected override void InitializeDefaultValues()
    {
        // C# default values
        AddDefaultValue("bit", "csharp", "false");
        AddDefaultValue("tinyint", "csharp", "0");
        AddDefaultValue("smallint", "csharp", "0");
        AddDefaultValue("int", "csharp", "0");
        AddDefaultValue("bigint", "csharp", "0L");
        AddDefaultValue("decimal", "csharp", "0M");
        AddDefaultValue("numeric", "csharp", "0M");
        AddDefaultValue("smallmoney", "csharp", "0M");
        AddDefaultValue("money", "csharp", "0M");
        AddDefaultValue("float", "csharp", "0.0");
        AddDefaultValue("real", "csharp", "0.0f");
        AddDefaultValue("date", "csharp", "DateTime.MinValue");
        AddDefaultValue("datetime", "csharp", "DateTime.MinValue");
        AddDefaultValue("datetime2", "csharp", "DateTime.MinValue");
        AddDefaultValue("smalldatetime", "csharp", "DateTime.MinValue");
        AddDefaultValue("datetimeoffset", "csharp", "DateTimeOffset.MinValue");
        AddDefaultValue("time", "csharp", "TimeSpan.Zero");
        AddDefaultValue("char", "csharp", "string.Empty");
        AddDefaultValue("varchar", "csharp", "string.Empty");
        AddDefaultValue("text", "csharp", "string.Empty");
        AddDefaultValue("nchar", "csharp", "string.Empty");
        AddDefaultValue("nvarchar", "csharp", "string.Empty");
        AddDefaultValue("ntext", "csharp", "string.Empty");
        AddDefaultValue("binary", "csharp", "Array.Empty<byte>()");
        AddDefaultValue("varbinary", "csharp", "Array.Empty<byte>()");
        AddDefaultValue("image", "csharp", "Array.Empty<byte>()");
        AddDefaultValue("rowversion", "csharp", "Array.Empty<byte>()");
        AddDefaultValue("timestamp", "csharp", "Array.Empty<byte>()");
        AddDefaultValue("uniqueidentifier", "csharp", "Guid.Empty");
        AddDefaultValue("sql_variant", "csharp", "null");
        AddDefaultValue("xml", "csharp", "string.Empty");
        AddDefaultValue("hierarchyid", "csharp", "string.Empty");
        AddDefaultValue("geography", "csharp", "null");
        AddDefaultValue("geometry", "csharp", "null");

        // TypeScript default values
        AddDefaultValue("bit", "typescript", "false");
        AddDefaultValue("tinyint", "typescript", "0");
        AddDefaultValue("smallint", "typescript", "0");
        AddDefaultValue("int", "typescript", "0");
        AddDefaultValue("bigint", "typescript", "0");
        AddDefaultValue("decimal", "typescript", "0");
        AddDefaultValue("numeric", "typescript", "0");
        AddDefaultValue("smallmoney", "typescript", "0");
        AddDefaultValue("money", "typescript", "0");
        AddDefaultValue("float", "typescript", "0");
        AddDefaultValue("real", "typescript", "0");
        AddDefaultValue("date", "typescript", "new Date()");
        AddDefaultValue("datetime", "typescript", "new Date()");
        AddDefaultValue("datetime2", "typescript", "new Date()");
        AddDefaultValue("smalldatetime", "typescript", "new Date()");
        AddDefaultValue("datetimeoffset", "typescript", "new Date()");
        AddDefaultValue("time", "typescript", "''");
        AddDefaultValue("char", "typescript", "''");
        AddDefaultValue("varchar", "typescript", "''");
        AddDefaultValue("text", "typescript", "''");
        AddDefaultValue("nchar", "typescript", "''");
        AddDefaultValue("nvarchar", "typescript", "''");
        AddDefaultValue("ntext", "typescript", "''");
        AddDefaultValue("binary", "typescript", "new Uint8Array()");
        AddDefaultValue("varbinary", "typescript", "new Uint8Array()");
        AddDefaultValue("image", "typescript", "new Uint8Array()");
        AddDefaultValue("rowversion", "typescript", "new Uint8Array()");
        AddDefaultValue("timestamp", "typescript", "new Uint8Array()");
        AddDefaultValue("uniqueidentifier", "typescript", "''");
        AddDefaultValue("sql_variant", "typescript", "null");
        AddDefaultValue("xml", "typescript", "''");
        AddDefaultValue("hierarchyid", "typescript", "''");
        AddDefaultValue("geography", "typescript", "null");
        AddDefaultValue("geometry", "typescript", "null");

        // Java default values
        AddDefaultValue("bit", "java", "Boolean.FALSE");
        AddDefaultValue("tinyint", "java", "(byte) 0");
        AddDefaultValue("smallint", "java", "(short) 0");
        AddDefaultValue("int", "java", "0");
        AddDefaultValue("bigint", "java", "0L");
        AddDefaultValue("decimal", "java", "BigDecimal.ZERO");
        AddDefaultValue("numeric", "java", "BigDecimal.ZERO");
        AddDefaultValue("smallmoney", "java", "BigDecimal.ZERO");
        AddDefaultValue("money", "java", "BigDecimal.ZERO");
        AddDefaultValue("float", "java", "0.0d");
        AddDefaultValue("real", "java", "0.0f");
        AddDefaultValue("date", "java", "LocalDate.now()");
        AddDefaultValue("datetime", "java", "LocalDateTime.now()");
        AddDefaultValue("datetime2", "java", "LocalDateTime.now()");
        AddDefaultValue("smalldatetime", "java", "LocalDateTime.now()");
        AddDefaultValue("datetimeoffset", "java", "OffsetDateTime.now()");
        AddDefaultValue("time", "java", "LocalTime.MIDNIGHT");
        AddDefaultValue("char", "java", "\"\"");
        AddDefaultValue("varchar", "java", "\"\"");
        AddDefaultValue("text", "java", "\"\"");
        AddDefaultValue("nchar", "java", "\"\"");
        AddDefaultValue("nvarchar", "java", "\"\"");
        AddDefaultValue("ntext", "java", "\"\"");
        AddDefaultValue("binary", "java", "new byte[0]");
        AddDefaultValue("varbinary", "java", "new byte[0]");
        AddDefaultValue("image", "java", "new byte[0]");
        AddDefaultValue("rowversion", "java", "new byte[0]");
        AddDefaultValue("timestamp", "java", "new byte[0]");
        AddDefaultValue("uniqueidentifier", "java", "UUID.randomUUID()");
        AddDefaultValue("sql_variant", "java", "null");
        AddDefaultValue("xml", "java", "\"\"");
        AddDefaultValue("hierarchyid", "java", "\"\"");
        AddDefaultValue("geography", "java", "null");
        AddDefaultValue("geometry", "java", "null");

        // Python default values
        AddDefaultValue("bit", "python", "False");
        AddDefaultValue("tinyint", "python", "0");
        AddDefaultValue("smallint", "python", "0");
        AddDefaultValue("int", "python", "0");
        AddDefaultValue("bigint", "python", "0");
        AddDefaultValue("decimal", "python", "Decimal('0')");
        AddDefaultValue("numeric", "python", "Decimal('0')");
        AddDefaultValue("smallmoney", "python", "Decimal('0')");
        AddDefaultValue("money", "python", "Decimal('0')");
        AddDefaultValue("float", "python", "0.0");
        AddDefaultValue("real", "python", "0.0");
        AddDefaultValue("date", "python", "date.today()");
        AddDefaultValue("datetime", "python", "datetime.now()");
        AddDefaultValue("datetime2", "python", "datetime.now()");
        AddDefaultValue("smalldatetime", "python", "datetime.now()");
        AddDefaultValue("datetimeoffset", "python", "datetime.now()");
        AddDefaultValue("time", "python", "time()");
        AddDefaultValue("char", "python", "''");
        AddDefaultValue("varchar", "python", "''");
        AddDefaultValue("text", "python", "''");
        AddDefaultValue("nchar", "python", "''");
        AddDefaultValue("nvarchar", "python", "''");
        AddDefaultValue("ntext", "python", "''");
        AddDefaultValue("binary", "python", "b''");
        AddDefaultValue("varbinary", "python", "b''");
        AddDefaultValue("image", "python", "b''");
        AddDefaultValue("rowversion", "python", "b''");
        AddDefaultValue("timestamp", "python", "b''");
        AddDefaultValue("uniqueidentifier", "python", "''");
        AddDefaultValue("sql_variant", "python", "None");
        AddDefaultValue("xml", "python", "''");
        AddDefaultValue("hierarchyid", "python", "''");
        AddDefaultValue("geography", "python", "None");
        AddDefaultValue("geometry", "python", "None");
    }

    /// <summary>
    /// Applies SQL Server specific type transformations based on precision, scale, and length.
    /// </summary>
    /// <param name="mappedType">The already mapped type.</param>
    /// <param name="databaseType">The original database type.</param>
    /// <param name="targetLanguage">The target language.</param>
    /// <param name="precision">The precision for numeric types.</param>
    /// <param name="scale">The scale for numeric types.</param>
    /// <param name="maxLength">The maximum length for string types.</param>
    /// <returns>The transformed type.</returns>
    protected override string ApplyTypeSpecificTransformations(string mappedType, string databaseType, string targetLanguage, int? precision, int? scale, int? maxLength)
    {
        var normalizedType = NormalizeTypeName(databaseType);
        
        // For C# specific transformations with high precision decimals
        if (targetLanguage.ToLowerInvariant() == "csharp" || targetLanguage.ToLowerInvariant() == "c#")
        {
            if ((normalizedType == "decimal" || normalizedType == "numeric") && precision.HasValue && precision.Value > 28)
            {
                return "string"; // Use string for very high precision decimals that exceed C# decimal capacity
            }
        }
        
        return base.ApplyTypeSpecificTransformations(mappedType, databaseType, targetLanguage, precision, scale, maxLength);
    }
}
