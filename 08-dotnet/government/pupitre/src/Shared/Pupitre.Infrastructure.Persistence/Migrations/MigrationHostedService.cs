using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pupitre.Infrastructure.Persistence.Migrations;

/// <summary>
/// Hosted service that runs database migrations on startup.
/// </summary>
public class MigrationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MigrationHostedService> _logger;

    public MigrationHostedService(
        IServiceProvider serviceProvider,
        ILogger<MigrationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database migration hosted service");

        using var scope = _serviceProvider.CreateScope();
        var migrationServices = scope.ServiceProvider.GetServices<IMigrationService>();

        foreach (var migrationService in migrationServices)
        {
            try
            {
                var hasPending = await migrationService.HasPendingMigrationsAsync(cancellationToken);
                if (hasPending)
                {
                    await migrationService.MigrateAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration failed for {ServiceType}", migrationService.GetType().Name);
                throw; // Fail fast on migration errors
            }
        }

        _logger.LogInformation("Database migration hosted service completed");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
