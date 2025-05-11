using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace EzDbCodeGen.Documentation.Tests
{
    public class DocumentationValidationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _docsPath;
        private readonly string _apiDocsPath;
        private readonly string _tutorialsPath;
        private readonly string _examplesPath;

        public DocumentationValidationTests(ITestOutputHelper output)
        {
            _output = output;
            _docsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "docs");
            _apiDocsPath = Path.Combine(_docsPath, "api");
            _tutorialsPath = Path.Combine(_docsPath, "tutorials");
            _examplesPath = Path.Combine(_docsPath, "examples");
        }

        [Fact]
        public void Should_Have_Complete_API_Documentation()
        {
            // Arrange
            var coreAssembly = typeof(EzDbCodeGen.Core.DependencyInjection.ServiceCollectionExtensions).Assembly;
            var publicTypes = coreAssembly.GetExportedTypes()
                .Where(t => !t.IsNested)
                .ToArray();

            var schemaAssembly = typeof(EzDbCodeGen.Schema.Models.DatabaseSchema).Assembly;
            var schemaPublicTypes = schemaAssembly.GetExportedTypes()
                .Where(t => !t.IsNested)
                .ToArray();

            // Combine all public types from relevant assemblies
            var allPublicTypes = publicTypes.Concat(schemaPublicTypes).ToArray();

            // Act
            Directory.CreateDirectory(_apiDocsPath);
            var apiDocFiles = Directory.GetFiles(_apiDocsPath, "*.md", SearchOption.AllDirectories);
            
            // Assert
            apiDocFiles.Should().NotBeEmpty("because API documentation should exist");
            
            // Check if we have documentation for key types
            var documentedTypeNames = apiDocFiles
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .ToArray();
            
            var keyTypes = allPublicTypes
                .Where(t => t.IsPublic && (t.IsClass || t.IsInterface) && !t.IsEnum)
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("EzDbCodeGen"))
                .Where(t => !t.Name.Contains("<"))  // Exclude generic type definitions with angle brackets
                .ToArray();
            
            foreach (var keyType in keyTypes.Take(10))  // Check first 10 key types for test purposes
            {
                _output.WriteLine($"Checking documentation for {keyType.Name}");
                documentedTypeNames.Should().Contain(n => n.Equals(keyType.Name, StringComparison.OrdinalIgnoreCase) || 
                                                     n.Equals(keyType.Name + "Class", StringComparison.OrdinalIgnoreCase),
                    $"because {keyType.Name} should have API documentation");
            }
        }

        [Fact]
        public void Should_Have_Valid_XML_Documentation_Comments()
        {
            // Arrange
            var coreAssemblyPath = typeof(EzDbCodeGen.Core.DependencyInjection.ServiceCollectionExtensions).Assembly.Location;
            var coreXmlPath = Path.ChangeExtension(coreAssemblyPath, ".xml");
            
            // Act
            var xmlExists = File.Exists(coreXmlPath);
            XDocument xmlDoc = null;
            
            if (xmlExists)
            {
                xmlDoc = XDocument.Load(coreXmlPath);
            }
            
            // Assert
            xmlExists.Should().BeTrue("because XML documentation file should be generated during build");
            xmlDoc.Should().NotBeNull("because XML documentation should be valid");
            
            if (xmlDoc != null)
            {
                var members = xmlDoc.Descendants("member").ToArray();
                members.Should().NotBeEmpty("because XML documentation should contain members");
                
                // Check for summary elements
                var membersWithSummary = members.Where(m => m.Element("summary") != null).ToArray();
                membersWithSummary.Length.Should().BeGreaterThanOrEqualTo(members.Length * 0.8, 
                    "because at least 80% of members should have summary documentation");
                
                // Check for parameter documentation in methods
                var methodMembers = members.Where(m => m.Attribute("name")?.Value.StartsWith("M:") == true).ToArray();
                var methodsWithParamDocs = methodMembers
                    .Where(m => m.Elements("param").Any())
                    .ToArray();
                
                if (methodMembers.Length > 0)
                {
                    methodsWithParamDocs.Length.Should().BeGreaterThanOrEqualTo(methodMembers.Length * 0.7,
                        "because at least 70% of methods should have parameter documentation");
                }
            }
        }

        [Fact]
        public void Should_Have_Comprehensive_Comparison_Guide_With_EF_Core()
        {
            // Arrange
            var comparisonGuidePath = Path.Combine(_docsPath, "EFCoreComparison.md");
            
            // Act
            var guideExists = File.Exists(comparisonGuidePath);
            string guideContent = string.Empty;
            
            if (guideExists)
            {
                guideContent = File.ReadAllText(comparisonGuidePath);
            }
            
            // Assert
            guideExists.Should().BeTrue("because a comparison guide with EF Core Power Tools should exist");
            
            if (!string.IsNullOrEmpty(guideContent))
            {
                // Check for key sections in the comparison guide
                guideContent.Should().Contain("# Comparison with EF Core Power Tools", 
                    "because the guide should have a clear title");
                
                guideContent.Should().Contain("## Relationship Detection", 
                    "because relationship detection is a key comparison point");
                
                guideContent.Should().Contain("## Performance", 
                    "because performance is a key comparison point");
                
                guideContent.Should().Contain("## Code Generation Quality", 
                    "because code generation quality is a key comparison point");
                
                guideContent.Should().Contain("## Template Customization", 
                    "because template customization is a key comparison point");
                
                // Check for quantitative metrics
                var metricsPattern = @"\d+%\s*(faster|more|better|higher)";
                var metricsMatches = Regex.Matches(guideContent, metricsPattern);
                
                metricsMatches.Count.Should().BeGreaterThanOrEqualTo(3, 
                    "because the comparison guide should include at least 3 quantitative metrics");
            }
        }

        [Fact]
        public void Should_Have_Comprehensive_Tutorials()
        {
            // Arrange
            Directory.CreateDirectory(_tutorialsPath);
            var tutorialFiles = Directory.GetFiles(_tutorialsPath, "*.md", SearchOption.AllDirectories);
            
            // Act & Assert
            tutorialFiles.Should().NotBeEmpty("because tutorials should exist");
            
            // Check for key tutorials
            var tutorialNames = tutorialFiles
                .Select(file => Path.GetFileNameWithoutExtension(file))
                .ToArray();
            
            tutorialNames.Should().Contain(n => n.Contains("GettingStarted", StringComparison.OrdinalIgnoreCase), 
                "because a getting started tutorial should exist");
            
            tutorialNames.Should().Contain(n => n.Contains("CustomTemplate", StringComparison.OrdinalIgnoreCase), 
                "because a custom template tutorial should exist");
            
            tutorialNames.Should().Contain(n => n.Contains("Relationship", StringComparison.OrdinalIgnoreCase), 
                "because a relationship detection tutorial should exist");
            
            // Check tutorial content quality
            foreach (var tutorialFile in tutorialFiles)
            {
                var content = File.ReadAllText(tutorialFile);
                
                // Should have code examples
                content.Should().Contain("```", "because tutorials should include code examples");
                
                // Should have step-by-step instructions
                var stepPattern = @"^\d+\.\s";
                var stepMatches = Regex.Matches(content, stepPattern, RegexOptions.Multiline);
                
                stepMatches.Count.Should().BeGreaterThan(0, 
                    $"because tutorial {Path.GetFileName(tutorialFile)} should have numbered steps");
                
                // Should have images or diagrams
                content.Should().Contain("![", "because tutorials should include images or diagrams");
            }
        }

        [Fact]
        public void Should_Have_Validated_Examples()
        {
            // Arrange
            Directory.CreateDirectory(_examplesPath);
            var exampleDirs = Directory.GetDirectories(_examplesPath);
            
            // Act & Assert
            exampleDirs.Should().NotBeEmpty("because example projects should exist");
            
            // Check for key example projects
            var exampleNames = exampleDirs
                .Select(dir => Path.GetFileName(dir))
                .ToArray();
            
            exampleNames.Should().Contain(n => n.Contains("Basic", StringComparison.OrdinalIgnoreCase), 
                "because a basic example should exist");
            
            exampleNames.Should().Contain(n => n.Contains("WebApi", StringComparison.OrdinalIgnoreCase), 
                "because a Web API example should exist");
            
            // Check example project structure
            foreach (var exampleDir in exampleDirs)
            {
                // Should have a README
                File.Exists(Path.Combine(exampleDir, "README.md")).Should().BeTrue(
                    $"because example {Path.GetFileName(exampleDir)} should have a README.md file");
                
                // Should have a project file
                Directory.GetFiles(exampleDir, "*.csproj").Should().NotBeEmpty(
                    $"because example {Path.GetFileName(exampleDir)} should have a .csproj file");
                
                // Should have source code
                Directory.GetFiles(exampleDir, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty(
                    $"because example {Path.GetFileName(exampleDir)} should have C# source files");
            }
        }
    }
}
