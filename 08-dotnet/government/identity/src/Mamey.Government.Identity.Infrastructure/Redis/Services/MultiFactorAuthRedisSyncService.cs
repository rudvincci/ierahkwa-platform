using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class MultiFactorAuthRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MultiFactorAuthRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public MultiFactorAuthRedisSyncService(IServiceProvider serviceProvider, ILogger<MultiFactorAuthRedisSyncService> logger, IOptions<RedisSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Redis sync service is disabled");
            return;
        }

        
        await Task.Delay(_options.InitialDelay, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncMultiFactorAuthsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during multi-factor auth sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncMultiFactorAuthsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<MultiFactorAuthPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<MultiFactorAuthRedisRepository>();

        try
        {
            _logger.LogInformation("Starting multi-factor auth sync from PostgreSQL to Redis");
            var multiFactorAuths = await postgresRepo.BrowseAsync(cancellationToken);
            if (!multiFactorAuths.Any()) { _logger.LogInformation("No multi-factor auths found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var multiFactorAuth in multiFactorAuths)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(multiFactorAuth.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(multiFactorAuth, cancellationToken); syncedCount++; _logger.LogDebug("Synced new multi-factor auth {Id} to Redis", multiFactorAuth.Id.Value); }
                    else { await redisRepo.UpdateAsync(multiFactorAuth, cancellationToken); updatedCount++; _logger.LogDebug("Updated multi-factor auth {Id} in Redis", multiFactorAuth.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync multi-factor auth {Id} to Redis", multiFactorAuth.Id.Value); }
            }
            _logger.LogInformation("Successfully synced multi-factor auths to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, multiFactorAuths.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync multi-factor auths to Redis"); throw; }
    }
}
