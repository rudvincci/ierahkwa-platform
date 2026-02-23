using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class MultiFactorAuthMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MultiFactorAuthMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public MultiFactorAuthMongoSyncService(IServiceProvider serviceProvider, ILogger<MultiFactorAuthMongoSyncService> logger, IOptions<MongoSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Mongo sync service is disabled");
            return;
        }

        
        await Task.Delay(_options.InitialDelay, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncMultiFactorAuthsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during multi-factor auth sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncMultiFactorAuthsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<MultiFactorAuthPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<MultiFactorAuthMongoRepository>();

        try
        {
            _logger.LogInformation("Starting multi-factor auth sync from PostgreSQL to MongoDB");
            var multiFactorAuths = await postgresRepo.BrowseAsync(cancellationToken);
            if (!multiFactorAuths.Any()) { _logger.LogInformation("No multi-factor auths found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var multiFactorAuth in multiFactorAuths)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(multiFactorAuth.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(multiFactorAuth, cancellationToken); syncedCount++; _logger.LogDebug("Synced new multi-factor auth {Id} to MongoDB", multiFactorAuth.Id.Value); }
                    else { await mongoRepo.UpdateAsync(multiFactorAuth, cancellationToken); updatedCount++; _logger.LogDebug("Updated multi-factor auth {Id} in MongoDB", multiFactorAuth.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync multi-factor auth {Id} to MongoDB", multiFactorAuth.Id.Value); }
            }
            _logger.LogInformation("Successfully synced multi-factor auths to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, multiFactorAuths.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync multi-factor auths to MongoDB"); throw; }
    }
}
