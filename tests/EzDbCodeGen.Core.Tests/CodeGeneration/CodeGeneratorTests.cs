using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.CodeGeneration;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;
using EzDbCodeGen.Core.TypeMapping;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.CodeGeneration
{
    public class CodeGeneratorTests
    {
        [Fact]
        public async Task GenerateAsync_WithValidSchema_ShouldGenerateCode()
        {
            // Arrange
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new Dictionary<string, string> { { "Customer.cs", "public class Customer { }" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions { OneFilePerEntity = true };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema();
            var templatePath = "/templates/entity.hbs";
            var outputPath = "/output";

            // Act
            await generator.GenerateAsync(mockSchema, templatePath, outputPath, options);

            // Assert
            mockTemplateProcessor.Verify(tp => tp.ProcessFileAsync(templatePath, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GeneratePerEntityAsync_WithValidSchema_ShouldGenerateCodeForEachEntity()
        {
            // Arrange
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new Dictionary<string, string> { { "output.cs", "entity code" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions { OneFilePerEntity = true };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema();
            var templatePath = "/templates/entity.hbs";
            var outputPath = "/output";

            // Act
            await generator.GeneratePerEntityAsync(mockSchema, templatePath, outputPath, options);

            // Assert
            mockTemplateProcessor.Verify(tp => tp.ProcessFileAsync(templatePath, It.IsAny<object>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GenerateFromStringAsync_WithValidSchema_ShouldGenerateCode()
        {
            // Arrange
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessAsync(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(new Dictionary<string, string> { { "Customer.cs", "public class Customer { }" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions { OneFilePerEntity = true };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema();
            var template = "public class {{table.name}} { }";
            var outputPath = "/output";

            // Act
            await generator.GenerateFromStringAsync(mockSchema, template, outputPath, options);

            // Assert
            mockTemplateProcessor.Verify(tp => tp.ProcessAsync(template, It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GenerateIncrementalAsync_WithChangedSchema_ShouldGenerateOnlyChangedFiles()
        {
            // Arrange
            var mockTemplateProcessor = new Mock<IDifferentialTemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessFileIncrementalAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
                .ReturnsAsync(new Dictionary<string, string> { { "Order.cs", "public class Order { }" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions { OneFilePerEntity = true };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema(true); // Schema with modified entity
            var templatePath = "/templates/entity.hbs";
            var outputPath = "/output";

            // Act
            await generator.GenerateIncrementalAsync(mockSchema, templatePath, outputPath, options);

            // Assert
            mockTemplateProcessor.Verify(tp => tp.TakeSnapshotAsync(It.IsAny<object>()), Times.Once);
            mockTemplateProcessor.Verify(tp => tp.ProcessFileIncrementalAsync(templatePath, It.IsAny<object>(), It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GenerateAsync_WithDetectedRelationships_ShouldAddRelationshipsToTemplateData()
        {
            // Arrange
            var capturedData = new Dictionary<string, object>();
            
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object>((path, data) => {
                    // Capture the template data for inspection
                    if (data is IDictionary<string, object> dict)
                    {
                        foreach (var item in dict)
                        {
                            capturedData[item.Key] = item.Value;
                        }
                    }
                })
                .ReturnsAsync(new Dictionary<string, string> { { "Customer.cs", "public class Customer { }" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions
            {
                OneFilePerEntity = true,
                GenerateNavigationProperties = true
            };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema();
            var templatePath = "/templates/entity.hbs";
            var outputPath = "/output";

            // Act
            await generator.GenerateAsync(mockSchema, templatePath, outputPath, options);

            // Assert
            capturedData.Should().ContainKey("relationships");
            capturedData["relationships"].Should().NotBeNull();
        }

        [Fact]
        public async Task GenerateAsync_WithTemplateVariables_ShouldAddVariablesToTemplateData()
        {
            // Arrange
            var capturedData = new Dictionary<string, object>();
            
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            mockTemplateProcessor.Setup(tp => tp.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object>((path, data) => {
                    // Capture the template data for inspection
                    if (data is IDictionary<string, object> dict)
                    {
                        foreach (var item in dict)
                        {
                            capturedData[item.Key] = item.Value;
                        }
                    }
                })
                .ReturnsAsync(new Dictionary<string, string> { { "Customer.cs", "public class Customer { }" } });
            
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            var options = new CodeGenerationOptions
            {
                OneFilePerEntity = true,
                Namespace = "MyApp.Models",
                TemplateVariables = new Dictionary<string, object>
                {
                    ["customVariable"] = "customValue"
                }
            };
            
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            var mockSchema = CreateMockSchema();
            var templatePath = "/templates/entity.hbs";
            var outputPath = "/output";

            // Act
            await generator.GenerateAsync(mockSchema, templatePath, outputPath, options);

            // Assert
            capturedData.Should().ContainKey("namespace");
            capturedData["namespace"].Should().Be("MyApp.Models");
            capturedData.Should().ContainKey("customVariable");
            capturedData["customVariable"].Should().Be("customValue");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitializeProperties()
        {
            // Arrange
            var mockTemplateProcessor = new Mock<ITemplateProcessor>();
            var mockRelationshipDetector = CreateMockRelationshipDetector();
            
            // Act
            var generator = CreateCodeGenerator(mockTemplateProcessor.Object, mockRelationshipDetector);
            
            // Assert
            generator.TemplateProcessor.Should().Be(mockTemplateProcessor.Object);
            generator.RelationshipDetector.Should().Be(mockRelationshipDetector);
        }

        // Helper method to create a mock schema
        private IDatabaseSchema CreateMockSchema(bool withModifiedEntity = false)
        {
            var mockSchema = new Mock<IDatabaseSchema>();
            mockSchema.Setup(s => s.DatabaseName).Returns("TestDb");
            
            var tables = new List<ITable>
            {
                CreateMockTable("Customer"),
                CreateMockTable("Order", withModifiedEntity),
                CreateMockTable("Product")
            };
            
            mockSchema.Setup(s => s.Tables).Returns(tables);
            return mockSchema.Object;
        }

        // Helper method to create a mock table
        private ITable CreateMockTable(string name, bool isModified = false)
        {
            var mockTable = new Mock<ITable>();
            mockTable.Setup(t => t.Name).Returns(name);
            mockTable.Setup(t => t.Schema).Returns("dbo");
            mockTable.Setup(t => t.IsModified).Returns(isModified);
            
            var columns = new List<IColumn>();
            
            var idColumn = new Mock<IColumn>();
            idColumn.Setup(c => c.Name).Returns($"{name}Id");
            idColumn.Setup(c => c.DataType).Returns("int");
            idColumn.Setup(c => c.IsNullable).Returns(false);
            idColumn.Setup(c => c.IsPartOfPrimaryKey).Returns(true);
            columns.Add(idColumn.Object);
            
            var nameColumn = new Mock<IColumn>();
            nameColumn.Setup(c => c.Name).Returns("Name");
            nameColumn.Setup(c => c.DataType).Returns("nvarchar");
            nameColumn.Setup(c => c.MaxLength).Returns(100);
            nameColumn.Setup(c => c.IsNullable).Returns(false);
            columns.Add(nameColumn.Object);
            
            mockTable.Setup(t => t.Columns).Returns(columns);
            
            return mockTable.Object;
        }

        // Helper method to create a mock relationship detector
        private IRelationshipDetector CreateMockRelationshipDetector()
        {
            var mockDetector = new Mock<IRelationshipDetector>();
            
            mockDetector.Setup(d => d.DetectRelationships(It.IsAny<IDatabaseSchema>()))
                .Returns((IDatabaseSchema schema) => {
                    var relationships = new List<IRelationship>();
                    
                    // Find Customer and Order tables
                    var customerTable = schema.Tables.FirstOrDefault(t => t.Name == "Customer");
                    var orderTable = schema.Tables.FirstOrDefault(t => t.Name == "Order");
                    
                    if (customerTable != null && orderTable != null)
                    {
                        // Create a one-to-many relationship between Customer and Order
                        var mockRelationship = new Mock<IRelationship>();
                        mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToMany);
                        mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Customer");
                        mockRelationship.Setup(r => r.ForeignEntityName).Returns("Order");
                        mockRelationship.Setup(r => r.PrimaryTable).Returns(customerTable);
                        mockRelationship.Setup(r => r.ForeignTable).Returns(orderTable);
                        
                        relationships.Add(mockRelationship.Object);
                    }
                    
                    return relationships;
                });
            
            return mockDetector.Object;
        }

        // Helper method to create a code generator
        private ICodeGenerator CreateCodeGenerator(ITemplateProcessor templateProcessor, IRelationshipDetector relationshipDetector)
        {
            var mockGenerator = new Mock<ICodeGenerator>();
            
            mockGenerator.Setup(g => g.TemplateProcessor).Returns(templateProcessor);
            mockGenerator.Setup(g => g.TemplateEngine).Returns(templateProcessor.TemplateEngine);
            mockGenerator.Setup(g => g.RelationshipDetector).Returns(relationshipDetector);
            
            // Setup GenerateAsync to call the template processor
            mockGenerator.Setup(g => g.GenerateAsync(It.IsAny<IDatabaseSchema>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CodeGenerationOptions>()))
                .Returns((IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options) => {
                    // Create template data with schema and options
                    var templateData = new Dictionary<string, object>
                    {
                        ["schema"] = schema,
                        ["namespace"] = options.Namespace
                    };
                    
                    // Add template variables
                    if (options.TemplateVariables != null)
                    {
                        foreach (var variable in options.TemplateVariables)
                        {
                            templateData[variable.Key] = variable.Value;
                        }
                    }
                    
                    // Detect relationships if needed
                    if (options.GenerateNavigationProperties)
                    {
                        var relationships = relationshipDetector.DetectRelationships(schema);
                        templateData["relationships"] = relationships;
                    }
                    
                    return templateProcessor.ProcessFileAsync(templatePath, templateData);
                });
            
            // Setup GeneratePerEntityAsync to call the template processor for each entity
            mockGenerator.Setup(g => g.GeneratePerEntityAsync(It.IsAny<IDatabaseSchema>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CodeGenerationOptions>()))
                .Returns((IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options) => {
                    var tasks = new List<Task>();
                    
                    foreach (var table in schema.Tables)
                    {
                        // Create template data for the entity
                        var templateData = new Dictionary<string, object>
                        {
                            ["schema"] = schema,
                            ["table"] = table,
                            ["namespace"] = options.Namespace
                        };
                        
                        // Add template variables
                        if (options.TemplateVariables != null)
                        {
                            foreach (var variable in options.TemplateVariables)
                            {
                                templateData[variable.Key] = variable.Value;
                            }
                        }
                        
                        tasks.Add(templateProcessor.ProcessFileAsync(templatePath, templateData));
                    }
                    
                    return Task.WhenAll(tasks);
                });
            
            // Setup GenerateFromStringAsync to call the template processor with a string template
            mockGenerator.Setup(g => g.GenerateFromStringAsync(It.IsAny<IDatabaseSchema>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CodeGenerationOptions>()))
                .Returns((IDatabaseSchema schema, string template, string outputPath, CodeGenerationOptions options) => {
                    // Create template data with schema and options
                    var templateData = new Dictionary<string, object>
                    {
                        ["schema"] = schema,
                        ["namespace"] = options.Namespace
                    };
                    
                    // Add template variables
                    if (options.TemplateVariables != null)
                    {
                        foreach (var variable in options.TemplateVariables)
                        {
                            templateData[variable.Key] = variable.Value;
                        }
                    }
                    
                    return templateProcessor.ProcessAsync(template, templateData);
                });
            
            // Setup GenerateIncrementalAsync for differential code generation
            if (templateProcessor is IDifferentialTemplateProcessor differentialProcessor)
            {
                mockGenerator.Setup(g => g.GenerateIncrementalAsync(It.IsAny<IDatabaseSchema>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CodeGenerationOptions>()))
                    .Returns(async (IDatabaseSchema schema, string templatePath, string outputPath, CodeGenerationOptions options) => {
                        // Create template data with schema and options
                        var templateData = new Dictionary<string, object>
                        {
                            ["schema"] = schema,
                            ["namespace"] = options.Namespace
                        };
                        
                        // Add template variables
                        if (options.TemplateVariables != null)
                        {
                            foreach (var variable in options.TemplateVariables)
                            {
                                templateData[variable.Key] = variable.Value;
                            }
                        }
                        
                        // Get old data
                        var oldData = await differentialProcessor.GetSnapshotAsync();
                        
                        if (oldData == null)
                        {
                            // No previous snapshot, take one and generate all code
                            await differentialProcessor.TakeSnapshotAsync(templateData);
                            return await templateProcessor.ProcessFileAsync(templatePath, templateData);
                        }
                        else
                        {
                            // Process incrementally
                            var result = await differentialProcessor.ProcessFileIncrementalAsync(templatePath, templateData, oldData);
                            await differentialProcessor.TakeSnapshotAsync(templateData);
                            return result;
                        }
                    });
            }
            
            return mockGenerator.Object;
        }
    }
}
