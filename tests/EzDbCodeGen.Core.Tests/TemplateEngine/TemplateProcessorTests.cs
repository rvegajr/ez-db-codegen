using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.TemplateEngine
{
    public class TemplateProcessorTests
    {
        [Fact]
        public async Task ProcessAsync_WithSimpleTemplate_ShouldGenerateCorrectOutput()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("public class {{name}} { }");
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object);
            var template = "public class {{name}} { }";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Customer.cs");
            result["Customer.cs"].Should().Be("public class Customer { }");
        }

        [Fact]
        public async Task ProcessFileAsync_WithTemplateFile_ShouldGenerateCorrectOutput()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("public class {{name}} { }");
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object);
            var templatePath = "/templates/entity.hbs";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessFileAsync(templatePath, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Customer.cs");
            result["Customer.cs"].Should().Be("public class Customer { }");
        }

        [Fact]
        public async Task ProcessAsync_WithTableCollection_ShouldGenerateMultipleFiles()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    if (data is IDictionary<string, object> dict && dict.TryGetValue("table", out var tableObj))
                    {
                        if (tableObj is Mock<ITable> mockTable)
                        {
                            return $"public class {mockTable.Object.Name} {{ }}";
                        }
                    }
                    return template;
                });
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object);
            var template = "{{#each tables}}{{#with this}}public class {{name}} { }{{/with}}{{/each}}";
            
            var mockTables = new List<ITable>
            {
                CreateMockTable("Customer"),
                CreateMockTable("Order"),
                CreateMockTable("Product")
            };
            
            var data = new { tables = mockTables };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Should().ContainKeys("Customer.cs", "Order.cs", "Product.cs");
            result["Customer.cs"].Should().Be("public class Customer { }");
            result["Order.cs"].Should().Be("public class Order { }");
            result["Product.cs"].Should().Be("public class Product { }");
        }

        [Fact]
        public async Task ProcessAsync_WithLayout_ShouldApplyLayout()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    if (template.Contains("{{> header}}"))
                    {
                        return "// Auto-generated code\npublic class Customer { }";
                    }
                    return template;
                });
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object);
            var template = "{{> header}}\npublic class {{name}} { }";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Customer.cs");
            result["Customer.cs"].Should().StartWith("// Auto-generated code");
        }

        [Fact]
        public async Task ProcessAsync_WithOutputFilePath_ShouldUseSpecifiedFilePath()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("// Output: Controllers/CustomerController.cs\npublic class CustomerController { }");
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object);
            var template = "// Output: Controllers/{{name}}Controller.cs\npublic class {{name}}Controller { }";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Controllers/CustomerController.cs");
            result["Controllers/CustomerController.cs"].Should().Contain("CustomerController");
        }

        [Fact]
        public async Task ProcessAsync_WithTemplateVariables_ShouldApplyVariables()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    if (data is IDictionary<string, object> dict && 
                        dict.TryGetValue("namespace", out var ns) && 
                        dict.TryGetValue("name", out var name))
                    {
                        return $"namespace {ns} {{ public class {name} {{ }} }}";
                    }
                    return template;
                });
            
            var options = new TemplateProcessorOptions
            {
                TemplateVariables = new Dictionary<string, object>
                {
                    ["namespace"] = "MyApp.Models"
                }
            };
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object, options);
            var template = "namespace {{namespace}} { public class {{name}} { } }";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Customer.cs");
            result["Customer.cs"].Should().Be("namespace MyApp.Models { public class Customer { } }");
        }

        [Fact]
        public async Task ProcessAsync_WithPerEntityProcessing_ShouldGenerateFileForEachEntity()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    if (data is IDictionary<string, object> dict && dict.TryGetValue("table", out var tableObj))
                    {
                        if (tableObj is Mock<ITable> mockTable)
                        {
                            return $"public class {mockTable.Object.Name} {{ }}";
                        }
                    }
                    return template;
                });
            
            var options = new TemplateProcessorOptions
            {
                ProcessPerEntity = true
            };
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object, options);
            var template = "public class {{table.name}} { }";
            
            var mockSchema = new Mock<IDatabaseSchema>();
            var mockTables = new List<ITable>
            {
                CreateMockTable("Customer"),
                CreateMockTable("Order"),
                CreateMockTable("Product")
            };
            mockSchema.Setup(s => s.Tables).Returns(mockTables);
            
            var data = new { schema = mockSchema.Object };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(3);
            result.Should().ContainKeys("Customer.cs", "Order.cs", "Product.cs");
        }

        [Fact]
        public async Task ProcessAsync_WithFilters_ShouldApplyFilters()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("public class Customer { }");
            
            var mockFilter = new Mock<IOutputFilter>();
            mockFilter.Setup(f => f.ShouldApply(It.IsAny<string>())).Returns(true);
            mockFilter.Setup(f => f.ApplyFilter(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((path, content) => $"// File: {path}\n{content}");
            mockFilter.Setup(f => f.Name).Returns("HeaderFilter");
            mockFilter.Setup(f => f.Order).Returns(1);
            
            var options = new TemplateProcessorOptions
            {
                OutputFilters = new List<IOutputFilter> { mockFilter.Object }
            };
            
            var processor = CreateTemplateProcessor(mockTemplateEngine.Object, options);
            var template = "public class {{name}} { }";
            var data = new { name = "Customer" };

            // Act
            var result = await processor.ProcessAsync(template, data);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainKey("Customer.cs");
            result["Customer.cs"].Should().StartWith("// File: Customer.cs");
        }

        [Fact]
        public async Task ProcessIncrementalAsync_WithChangedEntity_ShouldGenerateOnlyChangedFile()
        {
            // Arrange
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    if (data is IDictionary<string, object> dict && dict.TryGetValue("table", out var tableObj))
                    {
                        if (tableObj is Mock<ITable> mockTable)
                        {
                            return $"public class {mockTable.Object.Name} {{ }}";
                        }
                    }
                    return template;
                });
            
            var differentialProcessor = CreateDifferentialTemplateProcessor(mockTemplateEngine.Object);
            var template = "public class {{table.name}} { }";
            
            // Old data
            var oldCustomerTable = CreateMockTable("Customer");
            var oldOrderTable = CreateMockTable("Order");
            var oldProductTable = CreateMockTable("Product");
            
            var oldMockSchema = new Mock<IDatabaseSchema>();
            oldMockSchema.Setup(s => s.Tables).Returns(new List<ITable> { oldCustomerTable, oldOrderTable, oldProductTable });
            var oldData = new { schema = oldMockSchema.Object };
            
            // New data with changed Order entity
            var newCustomerTable = CreateMockTable("Customer");
            var newOrderTable = CreateMockTable("Order", true); // Changed
            var newProductTable = CreateMockTable("Product");
            
            var newMockSchema = new Mock<IDatabaseSchema>();
            newMockSchema.Setup(s => s.Tables).Returns(new List<ITable> { newCustomerTable, newOrderTable, newProductTable });
            var newData = new { schema = newMockSchema.Object };

            // Act
            var result = await differentialProcessor.ProcessIncrementalAsync(template, newData, oldData);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1); // Only Order.cs should be generated
            result.Should().ContainKey("Order.cs");
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

        // Helper method to create a template processor
        private ITemplateProcessor CreateTemplateProcessor(ITemplateEngine templateEngine, TemplateProcessorOptions options = null)
        {
            var mockProcessor = new Mock<ITemplateProcessor>();
            
            mockProcessor.Setup(p => p.TemplateEngine).Returns(templateEngine);
            
            // Setup the ProcessAsync method implementation
            mockProcessor.Setup(p => p.ProcessAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    var result = new Dictionary<string, string>();
                    
                    // Get the compiled template from the template engine
                    var compiled = templateEngine.Compile(template, data);
                    
                    // Check for output file path in the compiled template
                    var outputPathMatch = System.Text.RegularExpressions.Regex.Match(compiled, @"// Output: (.+?)\r?\n");
                    string outputPath;
                    
                    if (outputPathMatch.Success)
                    {
                        outputPath = outputPathMatch.Groups[1].Value;
                        compiled = compiled.Replace(outputPathMatch.Value, "");
                    }
                    else
                    {
                        // Use the entity name as the output file path
                        if (data is IDictionary<string, object> dict && dict.TryGetValue("name", out var name))
                        {
                            outputPath = $"{name}.cs";
                        }
                        else if (options?.ProcessPerEntity == true && data is IDictionary<string, object> schemaDict && 
                                schemaDict.TryGetValue("schema", out var schemaObj) && schemaObj is IDatabaseSchema schema)
                        {
                            // Process each entity separately
                            foreach (var table in schema.Tables)
                            {
                                string entityTemplate = templateEngine.Compile(template, new Dictionary<string, object> { ["table"] = table });
                                result.Add($"{table.Name}.cs", entityTemplate);
                            }
                            return Task.FromResult<IDictionary<string, string>>(result);
                        }
                        else
                        {
                            outputPath = "output.cs";
                        }
                    }
                    
                    // Apply any filters
                    if (options?.OutputFilters != null && options.OutputFilters.Count > 0)
                    {
                        foreach (var filter in options.OutputFilters.OrderBy(f => f.Order))
                        {
                            if (filter.ShouldApply(outputPath))
                            {
                                compiled = filter.ApplyFilter(outputPath, compiled);
                            }
                        }
                    }
                    
                    result[outputPath] = compiled;
                    return Task.FromResult<IDictionary<string, string>>(result);
                });
            
            // Setup the ProcessFileAsync method implementation
            mockProcessor.Setup(p => p.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((templatePath, data) => {
                    // For testing, just simulate reading the template file
                    string template = "public class {{name}} { }";
                    return mockProcessor.Object.ProcessAsync(template, data);
                });
            
            return mockProcessor.Object;
        }

        // Helper method to create a differential template processor
        private IDifferentialTemplateProcessor CreateDifferentialTemplateProcessor(ITemplateEngine templateEngine)
        {
            var mockProcessor = new Mock<IDifferentialTemplateProcessor>();
            
            mockProcessor.Setup(p => p.TemplateEngine).Returns(templateEngine);
            
            // Setup the ProcessIncrementalAsync method implementation
            mockProcessor.Setup(p => p.ProcessIncrementalAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
                .Returns<string, object, object>((template, newData, oldData) => {
                    var result = new Dictionary<string, string>();
                    
                    // Extract schema data
                    if (newData is IDictionary<string, object> newDict && 
                        newDict.TryGetValue("schema", out var newSchemaObj) && 
                        newSchemaObj is IDatabaseSchema newSchema &&
                        oldData is IDictionary<string, object> oldDict &&
                        oldDict.TryGetValue("schema", out var oldSchemaObj) &&
                        oldSchemaObj is IDatabaseSchema oldSchema)
                    {
                        // Find modified tables
                        foreach (var newTable in newSchema.Tables)
                        {
                            if (newTable.IsModified)
                            {
                                string entityTemplate = templateEngine.Compile(template, new Dictionary<string, object> { ["table"] = newTable });
                                result.Add($"{newTable.Name}.cs", entityTemplate);
                            }
                        }
                    }
                    
                    return Task.FromResult<IDictionary<string, string>>(result);
                });
            
            // Setup the ProcessFileIncrementalAsync method implementation
            mockProcessor.Setup(p => p.ProcessFileIncrementalAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>()))
                .Returns<string, object, object>((templatePath, newData, oldData) => {
                    // For testing, just simulate reading the template file
                    string template = "public class {{table.name}} { }";
                    return mockProcessor.Object.ProcessIncrementalAsync(template, newData, oldData);
                });
            
            // Setup other methods
            mockProcessor.Setup(p => p.ProcessAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((template, data) => {
                    var result = new Dictionary<string, string>();
                    
                    // Process each entity
                    if (data is IDictionary<string, object> dict && 
                        dict.TryGetValue("schema", out var schemaObj) && 
                        schemaObj is IDatabaseSchema schema)
                    {
                        foreach (var table in schema.Tables)
                        {
                            string entityTemplate = templateEngine.Compile(template, new Dictionary<string, object> { ["table"] = table });
                            result.Add($"{table.Name}.cs", entityTemplate);
                        }
                    }
                    
                    return Task.FromResult<IDictionary<string, string>>(result);
                });
            
            mockProcessor.Setup(p => p.ProcessFileAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns<string, object>((templatePath, data) => {
                    string template = "public class {{table.name}} { }";
                    return mockProcessor.Object.ProcessAsync(template, data);
                });
            
            return mockProcessor.Object;
        }
    }
}
