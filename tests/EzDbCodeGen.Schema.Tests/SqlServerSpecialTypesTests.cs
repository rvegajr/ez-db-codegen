using System;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Interfaces.Providers;
using EzDbCodeGen.Schema.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EzDbCodeGen.Schema.Tests
{
    public class SqlServerSpecialTypesTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ILogger<Providers.SqlServerSchemaProvider>> _mockLogger;

        public SqlServerSpecialTypesTests()
        {
            _mockLogger = new Mock<ILogger<Providers.SqlServerSchemaProvider>>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            var services = new ServiceCollection();
            services.AddSingleton(_mockLoggerFactory.Object);
            services.AddTransient<ISqlServerSchemaProvider, Providers.SqlServerSchemaProvider>();
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Should_Extract_Spatial_Data_Types()
        {
            // Arrange
            var schema = CreateSchemaWithSpatialTypes();

            // Act
            var locationTable = schema.Tables.FirstOrDefault(t => t.Name == "Location");
            var geometryColumn = locationTable?.Columns.FirstOrDefault(c => c.DataType == "geometry");
            var geographyColumn = locationTable?.Columns.FirstOrDefault(c => c.DataType == "geography");

            // Assert
            locationTable.Should().NotBeNull("because the Location table should exist in the schema");
            
            geometryColumn.Should().NotBeNull("because the Location table should have a geometry column");
            geometryColumn.Name.Should().Be("Shape", "because that's the geometry column we defined");
            geometryColumn.DataType.Should().Be("geometry", "because it's a spatial geometry type");
            
            geographyColumn.Should().NotBeNull("because the Location table should have a geography column");
            geographyColumn.Name.Should().Be("Coordinates", "because that's the geography column we defined");
            geographyColumn.DataType.Should().Be("geography", "because it's a spatial geography type");
        }

        [Fact]
        public void Should_Extract_JSON_Data_Type()
        {
            // Arrange
            var schema = CreateSchemaWithJsonType();

            // Act
            var documentTable = schema.Tables.FirstOrDefault(t => t.Name == "Document");
            var jsonColumn = documentTable?.Columns.FirstOrDefault(c => c.DataType == "nvarchar" && c.IsJsonColumn);

            // Assert
            documentTable.Should().NotBeNull("because the Document table should exist in the schema");
            jsonColumn.Should().NotBeNull("because the Document table should have a JSON column");
            jsonColumn.Name.Should().Be("JsonData", "because that's the JSON column we defined");
            jsonColumn.IsJsonColumn.Should().BeTrue("because we defined it as a JSON column");
        }

        [Fact]
        public void Should_Extract_XML_Data_Type()
        {
            // Arrange
            var schema = CreateSchemaWithXmlType();

            // Act
            var documentTable = schema.Tables.FirstOrDefault(t => t.Name == "Document");
            var xmlColumn = documentTable?.Columns.FirstOrDefault(c => c.DataType == "xml");

            // Assert
            documentTable.Should().NotBeNull("because the Document table should exist in the schema");
            xmlColumn.Should().NotBeNull("because the Document table should have an XML column");
            xmlColumn.Name.Should().Be("XmlData", "because that's the XML column we defined");
            xmlColumn.DataType.Should().Be("xml", "because it's an XML type");
        }

        [Fact]
        public void Should_Map_Special_Types_To_Appropriate_CSharp_Types()
        {
            // Arrange
            var schema = CreateSchemaWithAllSpecialTypes();
            var typeMapper = new TypeMapping.SqlServerDataTypeMap();

            // Act
            var locationTable = schema.Tables.First(t => t.Name == "Location");
            var documentTable = schema.Tables.First(t => t.Name == "Document");
            
            var geometryColumn = locationTable.Columns.First(c => c.DataType == "geometry");
            var geographyColumn = locationTable.Columns.First(c => c.DataType == "geography");
            var jsonColumn = documentTable.Columns.First(c => c.IsJsonColumn);
            var xmlColumn = documentTable.Columns.First(c => c.DataType == "xml");

            // Assert
            typeMapper.GetCSharpType(geometryColumn).Should().Be("Microsoft.SqlServer.Types.SqlGeometry", "because geometry should map to SqlGeometry");
            typeMapper.GetCSharpType(geographyColumn).Should().Be("Microsoft.SqlServer.Types.SqlGeography", "because geography should map to SqlGeography");
            typeMapper.GetCSharpType(jsonColumn).Should().Be("string", "because JSON columns should map to string");
            typeMapper.GetCSharpType(xmlColumn).Should().Be("System.Xml.Linq.XElement", "because XML should map to XElement");
        }

        private IDatabaseSchema CreateSchemaWithSpatialTypes()
        {
            var schema = new DatabaseSchema
            {
                Name = "SpecialTypesTest",
                DefaultSchema = "dbo"
            };

            var locationTable = new Table
            {
                Name = "Location",
                Schema = "dbo"
            };

            locationTable.Columns.Add(new Column
            {
                Name = "LocationId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            locationTable.Columns.Add(new Column
            {
                Name = "Name",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            locationTable.Columns.Add(new Column
            {
                Name = "Shape",
                DataType = "geometry",
                IsNullable = true
            });

            locationTable.Columns.Add(new Column
            {
                Name = "Coordinates",
                DataType = "geography",
                IsNullable = true
            });

            schema.Tables.Add(locationTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithJsonType()
        {
            var schema = new DatabaseSchema
            {
                Name = "SpecialTypesTest",
                DefaultSchema = "dbo"
            };

            var documentTable = new Table
            {
                Name = "Document",
                Schema = "dbo"
            };

            documentTable.Columns.Add(new Column
            {
                Name = "DocumentId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            documentTable.Columns.Add(new Column
            {
                Name = "Title",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            documentTable.Columns.Add(new Column
            {
                Name = "JsonData",
                DataType = "nvarchar",
                MaxLength = -1, // MAX
                IsNullable = true,
                IsJsonColumn = true
            });

            schema.Tables.Add(documentTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithXmlType()
        {
            var schema = new DatabaseSchema
            {
                Name = "SpecialTypesTest",
                DefaultSchema = "dbo"
            };

            var documentTable = new Table
            {
                Name = "Document",
                Schema = "dbo"
            };

            documentTable.Columns.Add(new Column
            {
                Name = "DocumentId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            documentTable.Columns.Add(new Column
            {
                Name = "Title",
                DataType = "nvarchar",
                MaxLength = 100,
                IsNullable = false
            });

            documentTable.Columns.Add(new Column
            {
                Name = "XmlData",
                DataType = "xml",
                IsNullable = true
            });

            schema.Tables.Add(documentTable);
            return schema;
        }

        private IDatabaseSchema CreateSchemaWithAllSpecialTypes()
        {
            var schema = new DatabaseSchema
            {
                Name = "SpecialTypesTest",
                DefaultSchema = "dbo"
            };

            // Location table with spatial types
            var locationTable = new Table
            {
                Name = "Location",
                Schema = "dbo"
            };

            locationTable.Columns.Add(new Column
            {
                Name = "LocationId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            locationTable.Columns.Add(new Column
            {
                Name = "Shape",
                DataType = "geometry",
                IsNullable = true
            });

            locationTable.Columns.Add(new Column
            {
                Name = "Coordinates",
                DataType = "geography",
                IsNullable = true
            });

            // Document table with JSON and XML
            var documentTable = new Table
            {
                Name = "Document",
                Schema = "dbo"
            };

            documentTable.Columns.Add(new Column
            {
                Name = "DocumentId",
                DataType = "int",
                IsPrimaryKey = true,
                IsNullable = false
            });

            documentTable.Columns.Add(new Column
            {
                Name = "JsonData",
                DataType = "nvarchar",
                MaxLength = -1, // MAX
                IsNullable = true,
                IsJsonColumn = true
            });

            documentTable.Columns.Add(new Column
            {
                Name = "XmlData",
                DataType = "xml",
                IsNullable = true
            });

            schema.Tables.Add(locationTable);
            schema.Tables.Add(documentTable);
            return schema;
        }
    }
}
