using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.AIBehavior.Infrastructure.EF.Repositories;
using Pupitre.AIBehavior.Infrastructure.Mongo.Options;
using Pupitre.AIBehavior.Infrastructure.Mongo.Repositories;

namespace Pupitre.AIBehavior.Infrastructure.Mongo.Services;

/// <summary>
/// Background service that syncs data from PostgreSQL to MongoDB.
/// </summary>
internal class BehaviorMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BehaviorMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public BehaviorMongoSyncService(
        IServiceProvider serviceProvider,
        ILogger<BehaviorMongoSyncService> logger,
        IOptions<MongoSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("MongoDB sync service is disabled");
            return;
        }

        _logger.LogInformation("MongoDB sync service starting with interval: {Interval}ms", _options.SyncIntervalMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MongoDB sync");
            }

            await Task.Delay(_options.SyncIntervalMs, stoppingToken);
        }
    }

    private async Task SyncAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<BehaviorPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<BehaviorMongoRepository>();

        var items = await postgresRepo.BrowseAsync(cancellationToken);
        var syncCount = 0;

        foreach (var item in items)
        {
            try
            {
                // Check if exists, update if yes, add if no
                if (await mongoRepo.ExistsAsync(item.Id, cancellationToken))
                {
                    await mongoRepo.UpdateAsync(item, cancellationToken);
                }
                else
                {
                    await mongoRepo.AddAsync(item, cancellationToken);
                }
                syncCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to sync item {Id} to MongoDB", item.Id);
            }
        }

        _logger.LogDebug("Synced {Count} items to MongoDB", syncCount);
    }
}
