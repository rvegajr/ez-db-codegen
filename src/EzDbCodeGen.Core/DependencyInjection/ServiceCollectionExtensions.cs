using System;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Interfaces.Analysis;
using EzDbCodeGen.Schema.Interfaces.Filters;
using EzDbCodeGen.Schema.Interfaces.Providers;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.Filters;
using EzDbCodeGen.TemplateEngine.Interfaces.Helpers;
using EzDbCodeGen.TypeMapping.Interfaces;
using HandlebarsDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Core.DependencyInjection;

/// <summary>
/// Extension methods for setting up EzDbCodeGen services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds EzDbCodeGen services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEzDbCodeGen(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // Register database schema providers
        services.AddSingleton<IDatabaseSchemaProviderFactory, DatabaseSchemaProviderFactory>();
        services.AddTransient<ISqlServerSchemaProvider, SqlServerSchemaProvider>();

        // Register template engine services
        services.AddSingleton<ITemplateEngineFactory, TemplateEngineFactory>();
        services.AddTransient<ITemplateEngine, HandlebarsTemplateEngine>();
        services.AddTransient<ITemplateProcessor, TemplateProcessor>();
        services.AddTransient<ITemplateProcessorFactory, TemplateProcessorFactory>();
        
        // Register filters
        services.AddTransient<ITemplateFilter, OutputTemplateFilter>();
        services.AddTransient<ISchemaFilter, PatternSchemaFilter>();
        
        // Register type mapping services
        services.AddSingleton<IDataTypeMapFactory, DataTypeMapFactory>();
        services.AddTransient<IDataTypeMap, SqlServerDataTypeMap>();
        
        // Register schema analysis services
        services.AddTransient<IRelationshipAnalyzer, RelationshipAnalyzer>();
        
        // Register model adapter services
        services.AddTransient<ISchemaModelAdapter, SchemaModelAdapter>();
        
        // Register code generation services
        services.AddTransient<ICodeGenerator, CodeGenerator>();

        // Register template engine helpers
        services.AddSingleton<ISchemaHelper, HandlebarsSchemaHelpers>();
        services.AddSingleton<IRelationshipHelper, HandlebarsRelationshipHelpers>();
        services.AddSingleton<ICodeFormatHelper, HandlebarsCodeFormatHelpers>();
        services.AddSingleton<IDocumentationHelper, HandlebarsDocumentationHelpers>();
        services.AddSingleton<ILayoutHelper, HandlebarsLayoutHelpers>();
        services.AddSingleton<IStringFormatHelper, HandlebarsStringFormatHelpers>();
        services.AddSingleton<ITypeConversionHelper, HandlebarsTypeConversionHelpers>();
        services.AddSingleton<ITestHelper, HandlebarsTestHelpers>();

        return services;
    }
    
    /// <summary>
    /// Adds EzDbCodeGen services with SQL Server support to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEzDbCodeGenWithSqlServer(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddEzDbCodeGen();
        
        // Register SQL Server specific services
        services.AddTransient<ISqlServerSchemaProvider, SqlServerSchemaProvider>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            return new SqlServerSchemaProvider(loggerFactory?.CreateLogger<SqlServerSchemaProvider>());
        });
        
        services.AddTransient<IDataTypeMap, SqlServerDataTypeMap>();

        return services;
    }
    
    /// <summary>
    /// Configures the specified <see cref="ICodeGenerator"/> instance.
    /// </summary>
    /// <param name="codeGenerator">The code generator to configure.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The configured <see cref="ICodeGenerator"/> instance.</returns>
    public static ICodeGenerator ConfigureCodeGenerator(this ICodeGenerator codeGenerator, Action<CodeGenerationOptions> configure)
    {
        if (codeGenerator == null)
        {
            throw new ArgumentNullException(nameof(codeGenerator));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var options = new CodeGenerationOptions();
        configure(options);
        
        return codeGenerator;
    }
    
    /// <summary>
    /// Configures the specified <see cref="SchemaProviderOptions"/> instance.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The configured <see cref="SchemaProviderOptions"/> instance.</returns>
    public static SchemaProviderOptions Configure(this SchemaProviderOptions options, Action<SchemaProviderOptions> configure)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        configure(options);
        
        return options;
    }
    
    /// <summary>
    /// Registers all Handlebars helpers with the Handlebars instance.
    /// </summary>
    /// <param name="handlebars">The Handlebars instance to register helpers with.</param>
    public static void RegisterHandlebarsHelpers(HandlebarsDotNet.IHandlebars handlebars)
    {
        if (handlebars == null)
        {
            throw new ArgumentNullException(nameof(handlebars));
        }

        // Register all helpers
        HandlebarsSchemaHelpers.RegisterHelpers(handlebars);
        HandlebarsRelationshipHelpers.RegisterHelpers(handlebars);
        HandlebarsCodeFormatHelpers.RegisterHelpers(handlebars);
        HandlebarsDocumentationHelpers.RegisterHelpers(handlebars);
        HandlebarsLayoutHelpers.RegisterHelpers(handlebars);
        HandlebarsStringFormatHelpers.RegisterHelpers(handlebars);
        HandlebarsTypeConversionHelpers.RegisterHelpers(handlebars);
        HandlebarsTestHelpers.RegisterHelpers(handlebars);
    }
}
