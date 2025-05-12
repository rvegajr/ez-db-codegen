using System;
using EzDbCodeGen.Core.Schema.Models;
using EzDbCodeGen.Core.Schema.RelationshipDetection;
using EzDbCodeGen.Core.Schema.SchemaProviders;
using EzDbCodeGen.Core.TemplateEngine;
using EzDbCodeGen.Core.TemplateEngine.Helpers;
using EzDbCodeGen.Core.Utilities;
using EzDbCodeGen.Interfaces.CodeGen;
using EzDbCodeGen.Interfaces.Schema;
using EzDbCodeGen.Interfaces.TemplateEngine;
using EzDbCodeGen.Interfaces.TemplateEngine.Helpers;
using EzDbCodeGen.Interfaces.Utilities;
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

        // Register essential utility services
        services.AddSingleton<IStringUtility, StringUtility>();
        services.AddSingleton<IFileUtility, FileUtility>();

        // Register schema model services
        services.AddTransient<IDatabaseSchemaProvider, SqlServerSchemaProvider>();
        
        // Register relationship detection strategies
        services.AddTransient<IRelationshipDetectionStrategy, ForeignKeyRelationshipStrategy>();
        services.AddTransient<IRelationshipDetectionStrategy, ManyToManyRelationshipStrategy>();
        services.AddTransient<IRelationshipDetectionStrategy, InheritanceRelationshipStrategy>();
        
        // Register relationship detector
        services.AddTransient<IRelationshipDetector, RelationshipDetector>();

        // Register template engine services
        services.AddTransient<ITemplateProcessor, TemplateProcessor>();
        
        // Register template engine helpers
        services.AddTransient<ISchemaHelper, SchemaHelper>();
        services.AddTransient<IRelationshipHelper, RelationshipHelper>();
        services.AddTransient<ICodeFormatHelper, CodeFormatHelper>();

        // Register code generation services
        services.AddTransient<ICodeGenerator, CodeGenerator>();

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
