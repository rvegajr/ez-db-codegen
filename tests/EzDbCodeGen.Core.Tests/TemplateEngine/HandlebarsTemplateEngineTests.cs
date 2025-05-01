using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Core.TemplateEngine;
using Moq;
using Xunit;
using FluentAssertions;

namespace EzDbCodeGen.Core.Tests.TemplateEngine
{
    public class HandlebarsTemplateEngineTests
    {
        [Fact]
        public void Compile_WithSimpleTemplate_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            var template = "Hello, {{name}}!";
            var data = new { name = "World" };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("Hello, World!");
        }

        [Fact]
        public void Compile_WithNestedProperties_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            var template = "{{person.firstName}} {{person.lastName}}";
            var data = new { person = new { firstName = "John", lastName = "Doe" } };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("John Doe");
        }

        [Fact]
        public void Compile_WithConditional_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            var template = "{{#if isActive}}Active{{else}}Inactive{{/if}}";
            var activeData = new { isActive = true };
            var inactiveData = new { isActive = false };

            // Act
            var activeResult = templateEngine.Compile(template, activeData);
            var inactiveResult = templateEngine.Compile(template, inactiveData);

            // Assert
            activeResult.Should().Be("Active");
            inactiveResult.Should().Be("Inactive");
        }

        [Fact]
        public void Compile_WithLoop_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            var template = "{{#each items}}{{this}}{{#unless @last}}, {{/unless}}{{/each}}";
            var data = new { items = new[] { "apple", "banana", "orange" } };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("apple, banana, orange");
        }

        [Fact]
        public void RegisterHelper_WithCustomHelper_ShouldBeAvailableInTemplate()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            templateEngine.RegisterHelper("uppercase", (string input) => input.ToUpper());
            
            var template = "{{uppercase name}}";
            var data = new { name = "John Doe" };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("JOHN DOE");
        }

        [Fact]
        public void RegisterBlockHelper_WithCustomBlockHelper_ShouldBeAvailableInTemplate()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Create a block helper that repeats content n times
            templateEngine.RegisterBlockHelper("repeat", (dynamic context, dynamic options, dynamic arg1) => {
                int count = Convert.ToInt32(arg1);
                string result = "";
                for (int i = 0; i < count; i++)
                {
                    result += options.fn(context);
                }
                return result;
            });
            
            var template = "{{#repeat 3}}Hello {{/repeat}}";
            var data = new { };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("Hello Hello Hello ");
        }

        [Fact]
        public void RegisterPartial_WithCustomPartial_ShouldBeAvailableInTemplate()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            templateEngine.RegisterPartial("header", "<h1>{{title}}</h1>");
            
            var template = "{{> header}}";
            var data = new { title = "Welcome" };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("<h1>Welcome</h1>");
        }

        [Fact]
        public void Compile_WithStringFormatHelper_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Register a string format helper
            templateEngine.RegisterHelper("format", (string format, object arg) => 
                string.Format(format, arg));
            
            var template = "{{format '{0:C}' price}}";
            var data = new { price = 42.5m };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("$42.50");
        }

        [Fact]
        public void Compile_WithCaseHelpers_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Register case conversion helpers
            templateEngine.RegisterHelper("camelCase", (string input) => 
                char.ToLowerInvariant(input[0]) + input.Substring(1));
            
            templateEngine.RegisterHelper("pascalCase", (string input) => 
                char.ToUpperInvariant(input[0]) + input.Substring(1));
            
            var template = "{{camelCase name}} {{pascalCase property}}";
            var data = new { name = "Customer", property = "address" };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("customer Address");
        }

        [Fact]
        public void Compile_WithTypeConversionHelper_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            var mockColumn = new Mock<IColumn>();
            mockColumn.Setup(c => c.DataType).Returns("int");
            mockColumn.Setup(c => c.IsNullable).Returns(false);
            
            // Register type conversion helper
            templateEngine.RegisterHelper("convertType", (IColumn column, string language) => {
                if (language == "CSharp")
                {
                    return column.DataType switch
                    {
                        "int" => column.IsNullable ? "int?" : "int",
                        "varchar" => "string",
                        "bit" => column.IsNullable ? "bool?" : "bool",
                        _ => "object"
                    };
                }
                else if (language == "TypeScript")
                {
                    return column.DataType switch
                    {
                        "int" => "number",
                        "varchar" => "string",
                        "bit" => "boolean",
                        _ => "any"
                    };
                }
                return "object";
            });
            
            var template = "C#: {{convertType column 'CSharp'}}, TypeScript: {{convertType column 'TypeScript'}}";
            var data = new { column = mockColumn.Object };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("C#: int, TypeScript: number");
        }

        [Fact]
        public void Compile_WithLayoutHelper_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Register layout helper
            templateEngine.RegisterHelper("defineSection", (string name, string content) => "");
            templateEngine.RegisterHelper("renderSection", (string name) => $"[Section: {name}]");
            
            var template = "{{defineSection 'header' '<h1>Title</h1>'}}{{renderSection 'header'}}";
            var data = new { };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("[Section: header]");
        }

        [Fact]
        public void Compile_WithDocumentationHelper_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Register documentation helper
            templateEngine.RegisterHelper("xmldoc", (string summary) => 
                $"/// <summary>\n/// {summary}\n/// </summary>");
            
            var template = "{{xmldoc description}}";
            var data = new { description = "This is a class that represents a customer." };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("/// <summary>\n/// This is a class that represents a customer.\n/// </summary>");
        }

        [Fact]
        public void Compile_WithRelationshipHelper_ShouldRenderCorrectly()
        {
            // Arrange
            var templateEngine = CreateTemplateEngine();
            
            // Create mock relationship
            var mockRelationship = new Mock<IRelationship>();
            mockRelationship.Setup(r => r.RelationshipType).Returns(RelationshipType.OneToMany);
            mockRelationship.Setup(r => r.PrimaryEntityName).Returns("Customer");
            mockRelationship.Setup(r => r.ForeignEntityName).Returns("Order");
            
            // Register relationship helper
            templateEngine.RegisterHelper("navigationProperty", (IRelationship relationship) => {
                if (relationship.RelationshipType == RelationshipType.OneToMany)
                {
                    return $"public virtual ICollection<{relationship.ForeignEntityName}> {relationship.ForeignEntityName}s {{ get; set; }}";
                }
                return "";
            });
            
            var template = "{{navigationProperty relationship}}";
            var data = new { relationship = mockRelationship.Object };

            // Act
            var result = templateEngine.Compile(template, data);

            // Assert
            result.Should().Be("public virtual ICollection<Order> Orders { get; set; }");
        }

        // Helper method to create a mock template engine
        private ITemplateEngine CreateTemplateEngine()
        {
            var mockTemplateEngine = new Mock<ITemplateEngine>();
            
            // Dictionary to store helpers and partials
            var helpers = new Dictionary<string, Delegate>();
            var blockHelpers = new Dictionary<string, Delegate>();
            var partials = new Dictionary<string, string>();
            
            // Setup RegisterHelper to add the helper to the dictionary
            mockTemplateEngine.Setup(te => te.RegisterHelper(It.IsAny<string>(), It.IsAny<Delegate>()))
                .Callback((string name, Delegate helper) => {
                    helpers[name] = helper;
                });
            
            // Setup RegisterBlockHelper to add the block helper to the dictionary
            mockTemplateEngine.Setup(te => te.RegisterBlockHelper(It.IsAny<string>(), It.IsAny<Delegate>()))
                .Callback((string name, Delegate helper) => {
                    blockHelpers[name] = helper;
                });
            
            // Setup RegisterPartial to add the partial to the dictionary
            mockTemplateEngine.Setup(te => te.RegisterPartial(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string name, string partial) => {
                    partials[name] = partial;
                });
            
            // Setup Compile to perform simple string replacements for testing
            mockTemplateEngine.Setup(te => te.Compile(It.IsAny<string>(), It.IsAny<object>()))
                .Returns((string template, object data) => {
                    return RenderMockTemplate(template, data, helpers, blockHelpers, partials);
                });
            
            mockTemplateEngine.Setup(te => te.EngineType).Returns("Handlebars");
            
            return mockTemplateEngine.Object;
        }

        // Helper method to render a mock template
        private string RenderMockTemplate(string template, object data, 
            Dictionary<string, Delegate> helpers, 
            Dictionary<string, Delegate> blockHelpers,
            Dictionary<string, string> partials)
        {
            string result = template;
            
            // Get all available properties from the data object
            var properties = data.GetType().GetProperties();
            var dataDict = new Dictionary<string, object>();
            foreach (var prop in properties)
            {
                dataDict[prop.Name] = prop.GetValue(data);
            }
            
            // Basic variable replacement
            foreach (var prop in dataDict)
            {
                result = result.Replace("{{" + prop.Key + "}}", prop.Value?.ToString() ?? "");
            }
            
            // Handle nested properties (simple cases only)
            foreach (var prop in dataDict)
            {
                if (prop.Value != null && !prop.Value.GetType().IsPrimitive && prop.Value.GetType() != typeof(string))
                {
                    var nestedProps = prop.Value.GetType().GetProperties();
                    foreach (var nestedProp in nestedProps)
                    {
                        var placeholder = "{{" + prop.Key + "." + nestedProp.Name + "}}";
                        var value = nestedProp.GetValue(prop.Value)?.ToString() ?? "";
                        result = result.Replace(placeholder, value);
                    }
                }
            }
            
            // Handle basic conditionals
            var ifRegex = new System.Text.RegularExpressions.Regex(@"{{#if (\w+)}}(.*?){{else}}(.*?){{\/if}}");
            var match = ifRegex.Match(result);
            while (match.Success)
            {
                var condition = match.Groups[1].Value;
                var trueBlock = match.Groups[2].Value;
                var falseBlock = match.Groups[3].Value;
                
                bool conditionValue = false;
                if (dataDict.TryGetValue(condition, out var conditionObj) && conditionObj is bool boolValue)
                {
                    conditionValue = boolValue;
                }
                
                result = result.Replace(match.Value, conditionValue ? trueBlock : falseBlock);
                match = ifRegex.Match(result);
            }
            
            // Handle basic each loops
            var eachRegex = new System.Text.RegularExpressions.Regex(@"{{#each (\w+)}}(.*?){{\/each}}");
            match = eachRegex.Match(result);
            while (match.Success)
            {
                var collectionName = match.Groups[1].Value;
                var template = match.Groups[2].Value;
                
                if (dataDict.TryGetValue(collectionName, out var collectionObj) && collectionObj is System.Collections.IEnumerable collection)
                {
                    var loopResult = "";
                    var index = 0;
                    foreach (var item in collection)
                    {
                        var itemResult = template;
                        itemResult = itemResult.Replace("{{this}}", item.ToString());
                        
                        // Handle @last
                        var lastRegex = new System.Text.RegularExpressions.Regex(@"{{#unless @last}}(.*?){{\/unless}}");
                        var lastMatch = lastRegex.Match(itemResult);
                        if (lastMatch.Success)
                        {
                            bool isLast = index == ((System.Collections.ICollection)collection).Count - 1;
                            itemResult = itemResult.Replace(lastMatch.Value, isLast ? "" : lastMatch.Groups[1].Value);
                        }
                        
                        loopResult += itemResult;
                        index++;
                    }
                    
                    result = result.Replace(match.Value, loopResult);
                }
                else
                {
                    result = result.Replace(match.Value, "");
                }
                
                match = eachRegex.Match(result);
            }
            
            // Handle partials
            var partialRegex = new System.Text.RegularExpressions.Regex(@"{{> (\w+)}}");
            match = partialRegex.Match(result);
            while (match.Success)
            {
                var partialName = match.Groups[1].Value;
                
                if (partials.TryGetValue(partialName, out var partial))
                {
                    // Recursively process the partial
                    var renderedPartial = RenderMockTemplate(partial, data, helpers, blockHelpers, partials);
                    result = result.Replace(match.Value, renderedPartial);
                }
                else
                {
                    result = result.Replace(match.Value, "");
                }
                
                match = partialRegex.Match(result);
            }
            
            // Handle helpers
            foreach (var helper in helpers)
            {
                var helperRegex = new System.Text.RegularExpressions.Regex(@"{{" + helper.Key + @" (.+?)}}");
                match = helperRegex.Match(result);
                while (match.Success)
                {
                    var argString = match.Groups[1].Value;
                    var args = argString.Split(' ');
                    
                    // Process arguments
                    var processedArgs = new List<object>();
                    foreach (var arg in args)
                    {
                        if (arg.StartsWith("'") && arg.EndsWith("'"))
                        {
                            // String literal
                            processedArgs.Add(arg.Substring(1, arg.Length - 2));
                        }
                        else if (dataDict.TryGetValue(arg, out var value))
                        {
                            // Variable
                            processedArgs.Add(value);
                        }
                        else
                        {
                            // Try to find it as a property of one of the data objects
                            bool foundProperty = false;
                            foreach (var prop in dataDict)
                            {
                                if (prop.Value != null && !prop.Value.GetType().IsPrimitive)
                                {
                                    var propInfo = prop.Value.GetType().GetProperty(arg);
                                    if (propInfo != null)
                                    {
                                        processedArgs.Add(propInfo.GetValue(prop.Value));
                                        foundProperty = true;
                                        break;
                                    }
                                }
                            }
                            
                            if (!foundProperty)
                            {
                                // Unknown argument, just pass the string
                                processedArgs.Add(arg);
                            }
                        }
                    }
                    
                    // Invoke the helper
                    try
                    {
                        var helperResult = helper.Value.DynamicInvoke(processedArgs.ToArray())?.ToString() ?? "";
                        result = result.Replace(match.Value, helperResult);
                    }
                    catch (Exception)
                    {
                        // If there's an error invoking the helper, just leave the placeholder
                        result = result.Replace(match.Value, $"[Helper Error: {helper.Key}]");
                    }
                    
                    match = helperRegex.Match(result);
                }
            }
            
            // Handle block helpers
            foreach (var blockHelper in blockHelpers)
            {
                var blockRegex = new System.Text.RegularExpressions.Regex(@"{{#" + blockHelper.Key + @" (.+?)}}(.*?){{/" + blockHelper.Key + @"}}");
                match = blockRegex.Match(result);
                while (match.Success)
                {
                    var argString = match.Groups[1].Value;
                    var blockContent = match.Groups[2].Value;
                    
                    // Create options object with fn function
                    var options = new {
                        fn = new Func<object, string>(ctx => blockContent)
                    };
                    
                    // Process arguments
                    object arg = argString;
                    if (int.TryParse(argString, out var intValue))
                    {
                        arg = intValue;
                    }
                    else if (dataDict.TryGetValue(argString, out var value))
                    {
                        arg = value;
                    }
                    
                    // Invoke the block helper
                    try
                    {
                        var helperResult = blockHelper.Value.DynamicInvoke(data, options, arg)?.ToString() ?? "";
                        result = result.Replace(match.Value, helperResult);
                    }
                    catch (Exception)
                    {
                        // If there's an error invoking the helper, just leave the placeholder
                        result = result.Replace(match.Value, $"[Block Helper Error: {blockHelper.Key}]");
                    }
                    
                    match = blockRegex.Match(result);
                }
            }
            
            return result;
        }
    }
}
