# EzDbCodeGen Clean Architecture Implementation Plan

## Clean Slate Approach

After thorough analysis of the codebase, we've decided to take a clean slate approach to resolve the interface and implementation issues. Rather than patching or adapting the existing code, we'll implement a properly structured architecture from the ground up with no legacy constraints.

## Core Architectural Principles

1. **Clear Domain Separation** - Distinct projects with focused responsibilities
2. **No Duplicate Interfaces** - Single source of truth for all interface definitions
3. **Proper DI Registration** - Consistent registration patterns throughout
4. **Zero Adapters** - Direct implementations without intermediate adapters
5. **Clean External Integration** - Properly abstracted external dependencies

## Implementation Plan

### Phase 1: Clean Interface Definitions (2 days)

1. **Create new project structure**
   ```
   src/
     EzDbCodeGen.Core/           # Core functionality
     EzDbCodeGen.Interfaces/     # All interfaces in one place
     EzDbCodeGen.Schema/         # Schema management implementation
     EzDbCodeGen.CodeGen/        # Code generation implementation
     EzDbCodeGen.TemplateEngine/ # Template processing implementation
     EzDbCodeGen.TypeMapping/    # Type mapping implementation
     EzDbCodeGen.Cli/            # Command-line interface
   ```

2. **Define clean interfaces in `EzDbCodeGen.Interfaces`**

   ```csharp
   namespace EzDbCodeGen.Interfaces.Schema;
   
   public interface IRelationship
   {
       string Name { get; }
       RelationshipType Type { get; }
       ITable SourceTable { get; }
       ITable TargetTable { get; }
       bool IsSelfReferencing { get; }
       // Additional properties...
   }
   
   public interface IRelationshipDetector
   {
       IReadOnlyCollection<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options);
   }
   
   namespace EzDbCodeGen.Interfaces.CodeGen;
   
   public interface ICodeGenerator
   {
       Task<IReadOnlyList<string>> GenerateCodeAsync(string templatePath, IDatabaseSchema schema, string outputPath, CodeGenerationOptions options);
       Task<IDictionary<string, string>> PreviewCodeAsync(string templatePath, IDatabaseSchema schema, CodeGenerationOptions options);
       // Additional methods...
   }
   ```

### Phase 2: Implement Core Components (3 days)

1. **Create clean implementations**

   ```csharp
   namespace EzDbCodeGen.Schema;
   
   public class RelationshipDetector : IRelationshipDetector
   {
       private readonly ILogger<RelationshipDetector> _logger;
       
       public RelationshipDetector(ILogger<RelationshipDetector> logger)
       {
           _logger = logger;
       }
       
       public IReadOnlyCollection<IRelationship> DetectRelationships(IDatabaseSchema schema, RelationshipDetectionOptions options)
       {
           // Direct implementation without adapters
           // ...
       }
   }
   ```

2. **Implement Handlebars integration without wrappers**

   ```csharp
   namespace EzDbCodeGen.TemplateEngine;
   
   public class HandlebarsTemplateEngine : ITemplateEngine
   {
       private readonly IHandlebars _handlebars;
       
       public HandlebarsTemplateEngine()
       {
           // Direct initialization of Handlebars
           _handlebars = Handlebars.Create();
           RegisterHelpers();
       }
       
       private void RegisterHelpers()
       {
           // Direct registration of helpers
           _handlebars.RegisterHelper("pascalCase", (context, args) => TextTransforms.ToPascalCase(args[0]?.ToString() ?? string.Empty));
           // Additional helpers...
       }
       
       // Interface implementation methods
   }
   ```

### Phase 3: Clean Dependency Injection (1 day)

1. **Implement a clean registration pattern**

   ```csharp
   namespace EzDbCodeGen.Core.DependencyInjection;
   
   public static class ServiceCollectionExtensions
   {
       public static IServiceCollection AddEzDbCodeGen(this IServiceCollection services)
       {
           // Register interfaces directly with implementations
           services.AddSingleton<ITemplateEngine, HandlebarsTemplateEngine>();
           services.AddTransient<IRelationshipDetector, RelationshipDetector>();
           services.AddTransient<ICodeGenerator, CodeGenerator>();
           // Additional registrations...
           
           return services;
       }
   }
   ```

### Phase 4: Project Integration (2 days)

1. **Update test project dependencies**
   - Replace references to old interface projects
   - Update namespace imports
   - Fix test assertions to match new implementations

2. **Implement CLI using new core**
   ```csharp
   namespace EzDbCodeGen.Cli;
   
   public class Program
   {
       public static async Task<int> Main(string[] args)
       {
           var host = CreateHostBuilder(args).Build();
           return await host.RunCommandLineApplicationAsync<CommandMain>(args);
       }
       
       private static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddEzDbCodeGen();
               });
   }
   ```

### Phase 5: Testing and Documentation (2 days)

1. **Run all tests against new implementation**
   - Execute existing superiority tests
   - Verify correct behavior of relationship detection
   - Validate code generation quality

2. **Create clean documentation for the new architecture**
   - Document interface hierarchy
   - Provide usage examples
   - Explain component responsibilities

## Timeline

This clean architecture implementation should take approximately 10 days to complete. By taking a clean slate approach, we eliminate the need for adapters and workarounds, resulting in a more maintainable codebase.

## Benefits of Clean Approach

1. **Simplified Codebase** - No adapters, no wrappers, no dual interface implementations
2. **Better Performance** - No conversion overhead or delegation chains
3. **Clearer Responsibility Boundaries** - Well-defined component boundaries
4. **Improved Testability** - Direct implementation testing without mocking adapters
5. **Future-Proof Design** - Built on modern .NET principles without legacy constraints

## Implementation Strategy

To minimize risk, we'll follow these steps:

1. Create the new projects and implement interfaces
2. Implement core components in isolation
3. Build a simple test application to validate the implementation
4. Gradually integrate with the test suite
5. Finally, update the CLI to use the new core

This approach ensures we can validate each component before moving to the next, with a working system at each step.
