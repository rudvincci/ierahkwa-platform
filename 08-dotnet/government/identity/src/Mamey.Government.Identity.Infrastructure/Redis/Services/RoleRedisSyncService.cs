using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class RoleRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RoleRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public RoleRedisSyncService(IServiceProvider serviceProvider, ILogger<RoleRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncRolesToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during role sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncRolesToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<RolePostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<RoleRedisRepository>();

        try
        {
            _logger.LogInformation("Starting role sync from PostgreSQL to Redis");
            var roles = await postgresRepo.BrowseAsync(cancellationToken);
            if (!roles.Any()) { _logger.LogInformation("No roles found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var role in roles)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(role.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(role, cancellationToken); syncedCount++; _logger.LogDebug("Synced new role {Id} to Redis", role.Id.Value); }
                    else { await redisRepo.UpdateAsync(role, cancellationToken); updatedCount++; _logger.LogDebug("Updated role {Id} in Redis", role.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync role {Id} to Redis", role.Id.Value); }
            }
            _logger.LogInformation("Successfully synced roles to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, roles.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync roles to Redis"); throw; }
    }
}
