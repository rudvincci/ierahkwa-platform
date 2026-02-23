using Mamey.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mamey.Services;

public class AppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppInitializer> _logger;
    private readonly DatabaseInitializationCompletionService _completionService;

    public AppInitializer(
        IServiceProvider serviceProvider, 
        ILogger<AppInitializer> logger,
        DatabaseInitializationCompletionService completionService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _completionService = completionService;
    }
        
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Wait for database initialization to complete before running application initializers
        _logger.LogInformation("Waiting for database initialization to complete...");
        await _completionService.WaitForCompletionAsync(cancellationToken);
        _logger.LogInformation("Database initialization completed. Starting application initializers...");
        
        using var scope = _serviceProvider.CreateScope();
        var initializers = scope.ServiceProvider.GetServices<IInitializer>();
        foreach (var initializer in initializers)
        {
            try
            {
                _logger.LogInformation($"Running the initializer: {initializer.GetType().Name}...");
                await initializer.InitializeAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}