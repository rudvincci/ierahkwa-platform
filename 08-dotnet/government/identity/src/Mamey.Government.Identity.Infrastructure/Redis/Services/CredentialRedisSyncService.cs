using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class CredentialRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CredentialRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public CredentialRedisSyncService(IServiceProvider serviceProvider, ILogger<CredentialRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncCredentialsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during credential sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncCredentialsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<CredentialPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<CredentialRedisRepository>();

        try
        {
            _logger.LogInformation("Starting credential sync from PostgreSQL to Redis");
            var credentials = await postgresRepo.BrowseAsync(cancellationToken);
            if (!credentials.Any()) { _logger.LogInformation("No credentials found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var credential in credentials)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(credential.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(credential, cancellationToken); syncedCount++; _logger.LogDebug("Synced new credential {Id} to Redis", credential.Id.Value); }
                    else { await redisRepo.UpdateAsync(credential, cancellationToken); updatedCount++; _logger.LogDebug("Updated credential {Id} in Redis", credential.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync credential {Id} to Redis", credential.Id.Value); }
            }
            _logger.LogInformation("Successfully synced credentials to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, credentials.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync credentials to Redis"); throw; }
    }
}
