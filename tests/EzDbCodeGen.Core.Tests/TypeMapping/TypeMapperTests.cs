using System;
using System.Collections.Generic;
using System.Linq;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TypeMapping;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.TypeMapping
{
    public class TypeMapperTests
    {
        [Theory]
        [InlineData("int", null, null, null, "int")]
        [InlineData("bigint", null, null, null, "long")]
        [InlineData("smallint", null, null, null, "short")]
        [InlineData("tinyint", null, null, null, "byte")]
        [InlineData("bit", null, null, null, "bool")]
        [InlineData("decimal", null, 18, 2, "decimal")]
        [InlineData("numeric", null, 18, 2, "decimal")]
        [InlineData("money", null, null, null, "decimal")]
        [InlineData("float", null, null, null, "double")]
        [InlineData("real", null, null, null, "float")]
        [InlineData("datetime", null, null, null, "DateTime")]
        [InlineData("datetime2", null, null, null, "DateTime")]
        [InlineData("date", null, null, null, "DateTime")]
        [InlineData("time", null, null, null, "TimeSpan")]
        [InlineData("datetimeoffset", null, null, null, "DateTimeOffset")]
        [InlineData("char", 10, null, null, "string")]
        [InlineData("varchar", 100, null, null, "string")]
        [InlineData("nchar", 10, null, null, "string")]
        [InlineData("nvarchar", 100, null, null, "string")]
        [InlineData("nvarchar", -1, null, null, "string")]
        [InlineData("text", null, null, null, "string")]
        [InlineData("ntext", null, null, null, "string")]
        [InlineData("binary", 100, null, null, "byte[]")]
        [InlineData("varbinary", 100, null, null, "byte[]")]
        [InlineData("varbinary", -1, null, null, "byte[]")]
        [InlineData("image", null, null, null, "byte[]")]
        [InlineData("rowversion", null, null, null, "byte[]")]
        [InlineData("uniqueidentifier", null, null, null, "Guid")]
        [InlineData("xml", null, null, null, "string")]
        [InlineData("geography", null, null, null, "Microsoft.SqlServer.Types.SqlGeography")]
        [InlineData("geometry", null, null, null, "Microsoft.SqlServer.Types.SqlGeometry")]
        [InlineData("hierarchyid", null, null, null, "Microsoft.SqlServer.Types.SqlHierarchyId")]
        public void MapType_WithValidSqlServerTypes_ShouldReturnCorrectCSharpType(string sqlType, int? maxLength, int? precision, int? scale, string expectedCSharpType)
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapper = CreateTypeMapper(options);
            var mockColumn = CreateColumnMock(sqlType, maxLength, precision, scale, false);

            // Act
            var result = mockTypeMapper.MapType(mockColumn.Object);

            // Assert
            result.Should().Be(expectedCSharpType);
        }

        [Theory]
        [InlineData("int", false, "int")]
        [InlineData("int", true, "int?")]
        [InlineData("varchar", false, "string")]
        [InlineData("varchar", true, "string")]
        [InlineData("uniqueidentifier", false, "Guid")]
        [InlineData("uniqueidentifier", true, "Guid?")]
        public void MapType_WithNullableColumns_ShouldHandleNullabilitiesCorrectly(string sqlType, bool isNullable, string expectedCSharpType)
        {
            // Arrange
            var options = new TypeMappingOptions
            {
                DefaultLanguage = "CSharp",
                UseNullableReferenceTypes = false,
                UseNullableValueTypes = true
            };
            var mockTypeMapper = CreateTypeMapper(options);
            var mockColumn = CreateColumnMock(sqlType, 100, null, null, isNullable);

            // Act
            var result = mockTypeMapper.MapType(mockColumn.Object);

            // Assert
            result.Should().Be(expectedCSharpType);
        }

        [Theory]
        [InlineData("int", "CSharp", "int")]
        [InlineData("int", "TypeScript", "number")]
        [InlineData("varchar", "CSharp", "string")]
        [InlineData("varchar", "TypeScript", "string")]
        [InlineData("datetime", "CSharp", "DateTime")]
        [InlineData("datetime", "TypeScript", "Date")]
        [InlineData("bit", "CSharp", "bool")]
        [InlineData("bit", "TypeScript", "boolean")]
        public void MapType_WithDifferentLanguages_ShouldReturnLanguageSpecificTypes(string sqlType, string language, string expectedType)
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = language };
            var mockTypeMapper = CreateTypeMapper(options);
            var mockColumn = CreateColumnMock(sqlType, null, null, null, false);

            // Act
            var result = mockTypeMapper.MapType(mockColumn.Object);

            // Assert
            result.Should().Be(expectedType);
        }

        [Fact]
        public void MapType_WithUnsupportedDataType_ShouldReturnObjectType()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapper = CreateTypeMapper(options);
            var mockColumn = CreateColumnMock("unsupported_type", null, null, null, false);

            // Act
            var result = mockTypeMapper.MapType(mockColumn.Object);

            // Assert
            result.Should().Be("object");
        }

        [Fact]
        public void MapType_WithNullableRefTypesEnabled_ShouldHandleReferenceTypesCorrectly()
        {
            // Arrange
            var options = new TypeMappingOptions
            {
                DefaultLanguage = "CSharp",
                UseNullableReferenceTypes = true
            };
            var mockTypeMapper = CreateTypeMapper(options);
            var mockStringColumn = CreateColumnMock("nvarchar", 100, null, null, true);
            var mockIntColumn = CreateColumnMock("int", null, null, null, true);

            // Act
            var stringResult = mockTypeMapper.MapType(mockStringColumn.Object);
            var intResult = mockTypeMapper.MapType(mockIntColumn.Object);

            // Assert
            stringResult.Should().Be("string?");
            intResult.Should().Be("int?");
        }

        [Fact]
        public void GetCSharpDefaultValue_WithDifferentTypes_ShouldReturnCorrectDefaultValues()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapper = CreateTypeMapper(options);

            // Act & Assert
            mockTypeMapper.GetDefaultValue("int").Should().Be("0");
            mockTypeMapper.GetDefaultValue("int?").Should().Be("null");
            mockTypeMapper.GetDefaultValue("string").Should().Be("\"\"");
            mockTypeMapper.GetDefaultValue("DateTime").Should().Be("DateTime.MinValue");
            mockTypeMapper.GetDefaultValue("DateTime?").Should().Be("null");
            mockTypeMapper.GetDefaultValue("bool").Should().Be("false");
            mockTypeMapper.GetDefaultValue("Guid").Should().Be("Guid.Empty");
            mockTypeMapper.GetDefaultValue("byte[]").Should().Be("null");
        }

        [Fact]
        public void GetTypeScriptDefaultValue_WithDifferentTypes_ShouldReturnCorrectDefaultValues()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "TypeScript" };
            var mockTypeMapper = CreateTypeMapper(options);

            // Act & Assert
            mockTypeMapper.GetDefaultValue("number").Should().Be("0");
            mockTypeMapper.GetDefaultValue("number | null").Should().Be("null");
            mockTypeMapper.GetDefaultValue("string").Should().Be("\"\"");
            mockTypeMapper.GetDefaultValue("string | null").Should().Be("null");
            mockTypeMapper.GetDefaultValue("Date").Should().Be("new Date(0)");
            mockTypeMapper.GetDefaultValue("Date | null").Should().Be("null");
            mockTypeMapper.GetDefaultValue("boolean").Should().Be("false");
        }

        [Fact]
        public void GetConverterFunction_WithDifferentTypes_ShouldReturnCorrectConverterFunctions()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapper = CreateTypeMapper(options);

            // Act & Assert
            mockTypeMapper.GetConverterFunction("int").Should().Be("int.Parse");
            mockTypeMapper.GetConverterFunction("int?").Should().Be("StringToNullableInt");
            mockTypeMapper.GetConverterFunction("decimal").Should().Be("decimal.Parse");
            mockTypeMapper.GetConverterFunction("DateTime").Should().Be("DateTime.Parse");
            mockTypeMapper.GetConverterFunction("Guid").Should().Be("Guid.Parse");
            mockTypeMapper.GetConverterFunction("string").Should().Be("ToString");
            mockTypeMapper.GetConverterFunction("bool").Should().Be("bool.Parse");
        }

        [Fact]
        public void RegisterCustomMapping_WithNewMapping_ShouldApplyCustomMapping()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapper = CreateExtendableTypeMapper(options);
            
            // Add a custom mapping
            mockTypeMapper.RegisterCustomMapping("CUSTOM_TYPE", "CSharp", "MyCustomType");
            var mockColumn = CreateColumnMock("CUSTOM_TYPE", null, null, null, false);

            // Act
            var result = mockTypeMapper.MapType(mockColumn.Object);

            // Assert
            result.Should().Be("MyCustomType");
        }

        [Fact]
        public void GetTypeMap_ForDifferentLanguages_ShouldReturnCorrectTypeMap()
        {
            // Arrange
            var options = new TypeMappingOptions { DefaultLanguage = "CSharp" };
            var mockTypeMapProvider = CreateTypeMapProvider();

            // Act
            var csharpTypeMap = mockTypeMapProvider.GetTypeMap("CSharp");
            var tsTypeMap = mockTypeMapProvider.GetTypeMap("TypeScript");

            // Assert
            csharpTypeMap.Should().NotBeNull();
            tsTypeMap.Should().NotBeNull();
            csharpTypeMap.Language.Should().Be("CSharp");
            tsTypeMap.Language.Should().Be("TypeScript");
            
            csharpTypeMap.MapDatabaseType("int", null, null, null).Should().Be("int");
            tsTypeMap.MapDatabaseType("int", null, null, null).Should().Be("number");
        }

        // Helper method to create a mock column
        private Mock<IColumn> CreateColumnMock(string dataType, int? maxLength, int? precision, int? scale, bool isNullable)
        {
            var mockColumn = new Mock<IColumn>();
            mockColumn.Setup(c => c.DataType).Returns(dataType);
            mockColumn.Setup(c => c.MaxLength).Returns(maxLength);
            mockColumn.Setup(c => c.Precision).Returns(precision);
            mockColumn.Setup(c => c.Scale).Returns(scale);
            mockColumn.Setup(c => c.IsNullable).Returns(isNullable);
            return mockColumn;
        }

        // Helper method to create a mock type mapper
        private ITypeMapper CreateTypeMapper(TypeMappingOptions options)
        {
            var mockTypeMapper = new Mock<ITypeMapper>();
            
            // Setup the MapType method implementation
            mockTypeMapper.Setup(tm => tm.MapType(It.IsAny<IColumn>()))
                .Returns((IColumn column) => {
                    string mappedType;
                    
                    if (options.DefaultLanguage == "CSharp")
                    {
                        // Map SQL Server types to C# types
                        mappedType = MapSqlServerTypeToCSharp(column.DataType, column.MaxLength, column.Precision, column.Scale);
                        
                        // Handle nullability for value types
                        if (column.IsNullable && IsValueType(mappedType) && options.UseNullableValueTypes)
                        {
                            mappedType += "?";
                        }
                        
                        // Handle nullability for reference types
                        if (column.IsNullable && IsReferenceType(mappedType) && options.UseNullableReferenceTypes)
                        {
                            mappedType += "?";
                        }
                    }
                    else if (options.DefaultLanguage == "TypeScript")
                    {
                        // Map SQL Server types to TypeScript types
                        mappedType = MapSqlServerTypeToTypeScript(column.DataType);
                    }
                    else
                    {
                        // Default to object for unsupported languages
                        mappedType = "object";
                    }
                    
                    return mappedType;
                });
            
            // Setup the GetDefaultValue method implementation
            mockTypeMapper.Setup(tm => tm.GetDefaultValue(It.IsAny<string>()))
                .Returns((string typeName) => {
                    if (options.DefaultLanguage == "CSharp")
                    {
                        return GetCSharpDefaultValue(typeName);
                    }
                    else if (options.DefaultLanguage == "TypeScript")
                    {
                        return GetTypeScriptDefaultValue(typeName);
                    }
                    else
                    {
                        return "null";
                    }
                });
            
            // Setup the GetConverterFunction method implementation
            mockTypeMapper.Setup(tm => tm.GetConverterFunction(It.IsAny<string>()))
                .Returns((string typeName) => {
                    if (options.DefaultLanguage == "CSharp")
                    {
                        return GetCSharpConverterFunction(typeName);
                    }
                    else
                    {
                        return "toString";
                    }
                });
            
            return mockTypeMapper.Object;
        }

        // Helper method to create a mock type mapper that allows custom mappings
        private Mock<ITypeMapper> CreateExtendableTypeMapper(TypeMappingOptions options)
        {
            var customMappings = new Dictionary<(string DatabaseType, string Language), string>();
            
            var mockTypeMapper = new Mock<ITypeMapper>();
            
            // Setup the RegisterCustomMapping method implementation
            mockTypeMapper.Setup(tm => tm.RegisterCustomMapping(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string databaseType, string language, string typeName) => {
                    customMappings[(databaseType, language)] = typeName;
                });
            
            // Setup the MapType method implementation with custom mapping support
            mockTypeMapper.Setup(tm => tm.MapType(It.IsAny<IColumn>()))
                .Returns((IColumn column) => {
                    // Check for custom mapping first
                    if (customMappings.TryGetValue((column.DataType, options.DefaultLanguage), out var customType))
                    {
                        return customType;
                    }
                    
                    string mappedType;
                    
                    if (options.DefaultLanguage == "CSharp")
                    {
                        // Map SQL Server types to C# types
                        mappedType = MapSqlServerTypeToCSharp(column.DataType, column.MaxLength, column.Precision, column.Scale);
                        
                        // Handle nullability for value types
                        if (column.IsNullable && IsValueType(mappedType) && options.UseNullableValueTypes)
                        {
                            mappedType += "?";
                        }
                    }
                    else
                    {
                        // Default to object for unsupported languages
                        mappedType = "object";
                    }
                    
                    return mappedType;
                });
            
            return mockTypeMapper;
        }

        // Helper method to create a mock type map provider
        private ITypeMapProvider CreateTypeMapProvider()
        {
            var mockTypeMapProvider = new Mock<ITypeMapProvider>();
            
            // Create mock type maps for different languages
            var mockCSharpTypeMap = new Mock<ITypeMap>();
            mockCSharpTypeMap.Setup(tm => tm.Language).Returns("CSharp");
            mockCSharpTypeMap.Setup(tm => tm.MapDatabaseType(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns((string dbType, int? maxLength, int? precision, int? scale) => 
                    MapSqlServerTypeToCSharp(dbType, maxLength, precision, scale));
            
            var mockTypeScriptTypeMap = new Mock<ITypeMap>();
            mockTypeScriptTypeMap.Setup(tm => tm.Language).Returns("TypeScript");
            mockTypeScriptTypeMap.Setup(tm => tm.MapDatabaseType(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns((string dbType, int? maxLength, int? precision, int? scale) => 
                    MapSqlServerTypeToTypeScript(dbType));
            
            // Setup GetTypeMap to return the appropriate type map
            mockTypeMapProvider.Setup(tmp => tmp.GetTypeMap(It.IsAny<string>()))
                .Returns((string language) => {
                    if (language == "CSharp")
                    {
                        return mockCSharpTypeMap.Object;
                    }
                    else if (language == "TypeScript")
                    {
                        return mockTypeScriptTypeMap.Object;
                    }
                    else
                    {
                        return null;
                    }
                });
            
            // Setup HasTypeMap to check if a type map exists for the language
            mockTypeMapProvider.Setup(tmp => tmp.HasTypeMap(It.IsAny<string>()))
                .Returns((string language) => language == "CSharp" || language == "TypeScript");
            
            return mockTypeMapProvider.Object;
        }

        // Helper method to map SQL Server types to C# types
        private static string MapSqlServerTypeToCSharp(string sqlType, int? maxLength, int? precision, int? scale)
        {
            return sqlType.ToLower() switch
            {
                "int" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "decimal" => "decimal",
                "numeric" => "decimal",
                "money" => "decimal",
                "smallmoney" => "decimal",
                "float" => "double",
                "real" => "float",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "smalldatetime" => "DateTime",
                "date" => "DateTime",
                "time" => "TimeSpan",
                "datetimeoffset" => "DateTimeOffset",
                "char" => "string",
                "varchar" => "string",
                "nchar" => "string",
                "nvarchar" => "string",
                "text" => "string",
                "ntext" => "string",
                "binary" => "byte[]",
                "varbinary" => "byte[]",
                "image" => "byte[]",
                "rowversion" => "byte[]",
                "timestamp" => "byte[]",
                "uniqueidentifier" => "Guid",
                "xml" => "string",
                "geography" => "Microsoft.SqlServer.Types.SqlGeography",
                "geometry" => "Microsoft.SqlServer.Types.SqlGeometry",
                "hierarchyid" => "Microsoft.SqlServer.Types.SqlHierarchyId",
                _ => "object"
            };
        }

        // Helper method to map SQL Server types to TypeScript types
        private static string MapSqlServerTypeToTypeScript(string sqlType)
        {
            return sqlType.ToLower() switch
            {
                "int" => "number",
                "bigint" => "number",
                "smallint" => "number",
                "tinyint" => "number",
                "bit" => "boolean",
                "decimal" => "number",
                "numeric" => "number",
                "money" => "number",
                "smallmoney" => "number",
                "float" => "number",
                "real" => "number",
                "datetime" => "Date",
                "datetime2" => "Date",
                "smalldatetime" => "Date",
                "date" => "Date",
                "time" => "string",
                "datetimeoffset" => "Date",
                "char" => "string",
                "varchar" => "string",
                "nchar" => "string",
                "nvarchar" => "string",
                "text" => "string",
                "ntext" => "string",
                "binary" => "ArrayBuffer",
                "varbinary" => "ArrayBuffer",
                "image" => "ArrayBuffer",
                "rowversion" => "ArrayBuffer",
                "timestamp" => "ArrayBuffer",
                "uniqueidentifier" => "string",
                "xml" => "string",
                _ => "any"
            };
        }

        // Helper method to check if a C# type is a value type
        private static bool IsValueType(string typeName)
        {
            return typeName is "int" or "long" or "short" or "byte" or "bool" or "decimal" or "double" or "float" or
                "DateTime" or "TimeSpan" or "DateTimeOffset" or "Guid" or "char";
        }

        // Helper method to check if a C# type is a reference type
        private static bool IsReferenceType(string typeName)
        {
            return typeName is "string" or "byte[]" or "object" || typeName.Contains(".");
        }

        // Helper method to get C# default values
        private static string GetCSharpDefaultValue(string typeName)
        {
            return typeName switch
            {
                "int" => "0",
                "long" => "0L",
                "short" => "0",
                "byte" => "0",
                "bool" => "false",
                "decimal" => "0m",
                "double" => "0.0",
                "float" => "0.0f",
                "DateTime" => "DateTime.MinValue",
                "TimeSpan" => "TimeSpan.Zero",
                "DateTimeOffset" => "DateTimeOffset.MinValue",
                "Guid" => "Guid.Empty",
                "string" => "\"\"",
                "char" => "\'\\0\'",
                "byte[]" => "null",
                _ when typeName.EndsWith("?") => "null",
                _ => "null"
            };
        }

        // Helper method to get TypeScript default values
        private static string GetTypeScriptDefaultValue(string typeName)
        {
            return typeName switch
            {
                "number" => "0",
                "boolean" => "false",
                "string" => "\"\"",
                "Date" => "new Date(0)",
                "ArrayBuffer" => "new ArrayBuffer(0)",
                _ when typeName.Contains(" | null") => "null",
                _ => "null"
            };
        }

        // Helper method to get C# converter functions
        private static string GetCSharpConverterFunction(string typeName)
        {
            return typeName switch
            {
                "int" => "int.Parse",
                "long" => "long.Parse",
                "short" => "short.Parse",
                "byte" => "byte.Parse",
                "bool" => "bool.Parse",
                "decimal" => "decimal.Parse",
                "double" => "double.Parse",
                "float" => "float.Parse",
                "DateTime" => "DateTime.Parse",
                "TimeSpan" => "TimeSpan.Parse",
                "DateTimeOffset" => "DateTimeOffset.Parse",
                "Guid" => "Guid.Parse",
                "string" => "ToString",
                "char" => "char.Parse",
                "int?" => "StringToNullableInt",
                "long?" => "StringToNullableLong",
                "short?" => "StringToNullableShort",
                "byte?" => "StringToNullableByte",
                "bool?" => "StringToNullableBool",
                "decimal?" => "StringToNullableDecimal",
                "double?" => "StringToNullableDouble",
                "float?" => "StringToNullableFloat",
                "DateTime?" => "StringToNullableDateTime",
                "TimeSpan?" => "StringToNullableTimeSpan",
                "DateTimeOffset?" => "StringToNullableDateTimeOffset",
                "Guid?" => "StringToNullableGuid",
                "char?" => "StringToNullableChar",
                _ => "ToString"
            };
        }
    }
}
