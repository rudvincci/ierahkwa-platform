using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class TwoFactorAuthMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TwoFactorAuthMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public TwoFactorAuthMongoSyncService(IServiceProvider serviceProvider, ILogger<TwoFactorAuthMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncTwoFactorAuthsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during two-factor auth sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncTwoFactorAuthsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<TwoFactorAuthPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<TwoFactorAuthMongoRepository>();

        try
        {
            _logger.LogInformation("Starting two-factor auth sync from PostgreSQL to MongoDB");
            var twoFactorAuths = await postgresRepo.BrowseAsync(cancellationToken);
            if (!twoFactorAuths.Any()) { _logger.LogInformation("No two-factor auths found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var twoFactorAuth in twoFactorAuths)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(twoFactorAuth.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(twoFactorAuth, cancellationToken); syncedCount++; _logger.LogDebug("Synced new two-factor auth {Id} to MongoDB", twoFactorAuth.Id.Value); }
                    else { await mongoRepo.UpdateAsync(twoFactorAuth, cancellationToken); updatedCount++; _logger.LogDebug("Updated two-factor auth {Id} in MongoDB", twoFactorAuth.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync two-factor auth {Id} to MongoDB", twoFactorAuth.Id.Value); }
            }
            _logger.LogInformation("Successfully synced two-factor auths to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, twoFactorAuths.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync two-factor auths to MongoDB"); throw; }
    }
}
