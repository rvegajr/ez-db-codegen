using System;
using EzDbCodeGen.CodeGen;
using EzDbCodeGen.CodeGen.Interfaces;
using EzDbCodeGen.Schema.Analysis;
using EzDbCodeGen.Schema.Filters;
using EzDbCodeGen.Schema.Interfaces;
using EzDbCodeGen.Schema.Interfaces.Analysis;
using EzDbCodeGen.Schema.Interfaces.Filters;
using EzDbCodeGen.Schema.Interfaces.Providers;
using EzDbCodeGen.Schema.Providers;
using EzDbCodeGen.TemplateEngine;
using EzDbCodeGen.TemplateEngine.Filters;
using EzDbCodeGen.TemplateEngine.Helpers;
using EzDbCodeGen.TemplateEngine.Interfaces;
using EzDbCodeGen.TemplateEngine.Interfaces.Filters;
using EzDbCodeGen.TemplateEngine.Interfaces.Helpers;
using EzDbCodeGen.TypeMapping;
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
        services.AddSingleton<EzDbCodeGen.Schema.Interfaces.IDatabaseSchemaProviderFactory, EzDbCodeGen.Schema.Providers.DatabaseSchemaProviderFactory>();
        services.AddTransient<EzDbCodeGen.Schema.Interfaces.Providers.ISqlServerSchemaProvider, EzDbCodeGen.Schema.Providers.SqlServerSchemaProvider>();

        // Register template engine services
        services.AddSingleton<ITemplateEngine>(sp => HandlebarsDotNet.Handlebars.Create());
        services.AddTransient<ITemplateProcessor, EzDbCodeGen.TemplateEngine.TemplateProcessor>();
        services.AddTransient<ITemplateProcessorFactory, EzDbCodeGen.TemplateEngine.TemplateProcessorFactory>();
        
        // Register filters
        services.AddTransient<ITemplateFilter, EzDbCodeGen.TemplateEngine.Filters.OutputTemplateFilter>();
        services.AddTransient<ISchemaFilter, EzDbCodeGen.Schema.Filters.PatternSchemaFilter>();
        
        // Register type mapping services
        services.AddSingleton<IDataTypeMapFactory, EzDbCodeGen.TypeMapping.DataTypeMapFactory>();
        
        // Register schema analysis services
        services.AddTransient<IRelationshipAnalyzer, EzDbCodeGen.Schema.Analysis.RelationshipAnalyzer>();
        
        // Register model adapter services
        services.AddTransient<ISchemaModelAdapter, EzDbCodeGen.CodeGen.SchemaModelAdapter>();
        
        // Register code generation services
        services.AddTransient<ICodeGenerator, EzDbCodeGen.CodeGen.CodeGenerator>();

        // Register template engine helpers
        services.AddSingleton<ISchemaHelper, EzDbCodeGen.TemplateEngine.Helpers.HandlebarsSchemaHelpers>();
        services.AddSingleton<IRelationshipHelper, EzDbCodeGen.TemplateEngine.Helpers.HandlebarsRelationshipHelpers>();
        services.AddSingleton<ICodeFormatHelper, EzDbCodeGen.TemplateEngine.Helpers.HandlebarsCodeFormatHelpers>();

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
        services.AddTransient<ISqlServerSchemaProvider, EzDbCodeGen.Schema.Providers.SqlServerSchemaProvider>(sp =>
        {
            var loggerFactory = sp.GetService<ILoggerFactory>();
            return new EzDbCodeGen.Schema.Providers.SqlServerSchemaProvider(loggerFactory?.CreateLogger<EzDbCodeGen.Schema.Providers.SqlServerSchemaProvider>());
        });

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
        EzDbCodeGen.TemplateEngine.Helpers.HandlebarsSchemaHelpers.RegisterHelpers(handlebars);
        EzDbCodeGen.TemplateEngine.Helpers.HandlebarsRelationshipHelpers.RegisterHelpers(handlebars);
        EzDbCodeGen.TemplateEngine.Helpers.HandlebarsCodeFormatHelpers.RegisterHelpers(handlebars);
    }
}
