using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EzDbCodeGen.Schema.IntegrationTests;

public class SqlServerSampleFixture : IAsyncLifetime
{
    private readonly string _databaseName;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SqlServerSampleFixture> _logger;
    
    public SqlServerSampleFixture(string databaseName = "AdventureWorks")
    {
        _databaseName = databaseName;
        
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
        
        _logger = _serviceProvider.GetRequiredService<ILogger<SqlServerSampleFixture>>();
    }
    
    private void ConfigureServices(IServiceCollection services)
    {
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddFile($"logs/{_databaseName}-extraction.log");
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        // Add configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
            
        services.AddSingleton<IConfiguration>(configuration);
        
        // Add schema provider
        services.AddScoped<ISqlServerSchemaProvider, SqlServerSchemaProvider>();
    }
    
    public ISqlServerSchemaProvider SchemaProvider => 
        _serviceProvider.GetRequiredService<ISqlServerSchemaProvider>();
        
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing test fixture for {Database}", _databaseName);
        // Database should already be restored by the restore-samples.sh script
        await Task.CompletedTask;
    }
    
    public async Task DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
