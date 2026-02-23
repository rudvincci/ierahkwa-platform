using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class TwoFactorAuthRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TwoFactorAuthRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public TwoFactorAuthRedisSyncService(IServiceProvider serviceProvider, ILogger<TwoFactorAuthRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncTwoFactorAuthsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during twofactorauth sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncTwoFactorAuthsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<TwoFactorAuthPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<TwoFactorAuthRedisRepository>();

        try
        {
            _logger.LogInformation("Starting twofactorauth sync from PostgreSQL to Redis");
            var twofactorauthss = await postgresRepo.BrowseAsync(cancellationToken);
            if (!twofactorauthss.Any()) { _logger.LogInformation("No twofactorauthss found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var twofactorauth in twofactorauthss)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(twofactorauth.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(twofactorauth, cancellationToken); syncedCount++; _logger.LogDebug("Synced new twofactorauth {Id} to Redis", twofactorauth.Id.Value); }
                    else { await redisRepo.UpdateAsync(twofactorauth, cancellationToken); updatedCount++; _logger.LogDebug("Updated twofactorauth {Id} in Redis", twofactorauth.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync twofactorauth {Id} to Redis", twofactorauth.Id.Value); }
            }
            _logger.LogInformation("Successfully synced twofactorauthss to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, twofactorauthss.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync twofactorauthss to Redis"); throw; }
    }
}
