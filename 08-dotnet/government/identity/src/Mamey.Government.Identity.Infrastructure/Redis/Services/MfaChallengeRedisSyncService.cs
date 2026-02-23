using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class MfaChallengeRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MfaChallengeRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public MfaChallengeRedisSyncService(IServiceProvider serviceProvider, ILogger<MfaChallengeRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncMfaChallengesToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during MFA challenge sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncMfaChallengesToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<MfaChallengePostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<MfaChallengeRedisRepository>();

        try
        {
            _logger.LogInformation("Starting MFA challenge sync from PostgreSQL to Redis");
            var mfaChallenges = await postgresRepo.BrowseAsync(cancellationToken);
            if (!mfaChallenges.Any()) { _logger.LogInformation("No MFA challenges found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var mfaChallenge in mfaChallenges)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(mfaChallenge.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(mfaChallenge, cancellationToken); syncedCount++; _logger.LogDebug("Synced new MFA challenge {Id} to Redis", mfaChallenge.Id.Value); }
                    else { await redisRepo.UpdateAsync(mfaChallenge, cancellationToken); updatedCount++; _logger.LogDebug("Updated MFA challenge {Id} in Redis", mfaChallenge.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync MFA challenge {Id} to Redis", mfaChallenge.Id.Value); }
            }
            _logger.LogInformation("Successfully synced MFA challenges to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, mfaChallenges.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync MFA challenges to Redis"); throw; }
    }
}
