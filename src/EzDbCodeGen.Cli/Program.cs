using System;
using System.Threading.Tasks;
using EzDbCodeGen.Cli.Adapters;
using EzDbCodeGen.Cli.Commands;
using EzDbCodeGen.CodeGen;
using EzDbCodeGen.Core.DependencyInjection;
using EzDbCodeGen.Core.Logging;
using EzDbCodeGen.Core.Schema;
using EzDbCodeGen.Schema;
using EzDbCodeGen.Schema.Analysis;
using EzDbCodeGen.TypeMapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                // Configure services
                var serviceProvider = ConfigureServices();

                // Get required services
                var commandLineProcessor = serviceProvider.GetRequiredService<ICommandLineProcessor>();
                
                // Process command line arguments
                await commandLineProcessor.ProcessAsync(args);
                
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fatal error: {ex.Message}");
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Set up logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // Register legacy logger
            services.AddSingleton<ILogger, ConsoleLogger>();
            
            // Add core EzDbCodeGen services
            services.AddEzDbCodeGenWithSqlServer();
            
            // Register relationship analyzer
            services.AddSingleton<RelationshipAnalyzer>();
            
            // Register adapter factories
            services.AddSingleton<ICodeGeneratorFactory>(provider => {
                var codeGenerator = provider.GetRequiredService<CodeGen.ICodeGenerator>();
                var dataTypeMap = provider.GetRequiredService<IDataTypeMap>();
                var schemaModelAdapter = provider.GetRequiredService<ISchemaModelAdapter>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                return new CodeGeneratorAdapterFactory(codeGenerator, dataTypeMap, schemaModelAdapter, loggerFactory);
            });
            
            services.AddSingleton<IDatabaseSchemaProviderFactory, DatabaseSchemaProviderFactory>();
            
            services.AddSingleton<IRelationshipDetectorFactory>(provider => {
                var relationshipAnalyzer = provider.GetRequiredService<RelationshipAnalyzer>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                
                return new RelationshipDetectorAdapterFactory(relationshipAnalyzer, loggerFactory);
            });
            
            // Register command factory
            services.AddSingleton<ICommandFactory, CommandFactory>(provider => {
                var factory = new CommandFactory(
                    provider.GetRequiredService<ILogger>(),
                    provider.GetRequiredService<IDatabaseSchemaProviderFactory>(),
                    provider.GetRequiredService<ICodeGeneratorFactory>(),
                    provider.GetRequiredService<IRelationshipDetectorFactory>());
                
                factory.RegisterStandardCommands();
                
                // Resolve circular dependency for HelpCommand
                var helpCommand = factory.GetAllCommands().FirstOrDefault(c => c.Name == "help") as HelpCommand;
                if (helpCommand != null)
                {
                    helpCommand.SetCommandFactory(factory);
                }
                
                return factory;
            });
            
            // Register command line processor
            services.AddSingleton<ICommandLineProcessor, CommandLineProcessor>();
            
            return services.BuildServiceProvider();
        }
    }

    /// <summary>
    /// Simple console logger implementation.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc/>
        public void Debug(string message)
        {
            // Only log debug messages if DEBUG environment variable is set
            if (Environment.GetEnvironmentVariable("DEBUG") == "1")
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"DEBUG: {message}");
                Console.ResetColor();
            }
        }

        /// <inheritdoc/>
        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        /// <inheritdoc/>
        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING: {message}");
            Console.ResetColor();
        }

        /// <inheritdoc/>
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"ERROR: {message}");
            Console.ResetColor();
        }
    }
}
