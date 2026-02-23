using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class PermissionRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public PermissionRedisSyncService(IServiceProvider serviceProvider, ILogger<PermissionRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncPermissionsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during permission sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncPermissionsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<PermissionPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<PermissionRedisRepository>();

        try
        {
            _logger.LogInformation("Starting permission sync from PostgreSQL to Redis");
            var permissions = await postgresRepo.BrowseAsync(cancellationToken);
            if (!permissions.Any()) { _logger.LogInformation("No permissions found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var permission in permissions)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(permission.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(permission, cancellationToken); syncedCount++; _logger.LogDebug("Synced new permission {Id} to Redis", permission.Id.Value); }
                    else { await redisRepo.UpdateAsync(permission, cancellationToken); updatedCount++; _logger.LogDebug("Updated permission {Id} in Redis", permission.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync permission {Id} to Redis", permission.Id.Value); }
            }
            _logger.LogInformation("Successfully synced permissions to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, permissions.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync permissions to Redis"); throw; }
    }
}
