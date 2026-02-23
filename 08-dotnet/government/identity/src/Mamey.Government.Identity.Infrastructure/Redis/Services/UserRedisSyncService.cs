using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

/// <summary>
/// Background service that periodically syncs users from PostgreSQL to Redis.
/// This ensures Redis cache stays up-to-date with the source of truth in PostgreSQL.
/// </summary>
internal sealed class UserRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public UserRedisSyncService(
        IServiceProvider serviceProvider,
        ILogger<UserRedisSyncService> logger,
        IOptions<RedisSyncOptions> options)
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
                await SyncUsersToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncUsersToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<UserPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<UserRedisRepository>();

        try
        {
            _logger.LogInformation("Starting user sync from PostgreSQL to Redis");

            var users = await postgresRepo.BrowseAsync(cancellationToken);
            
            if (!users.Any())
            {
                _logger.LogInformation("No users found in PostgreSQL, skipping sync");
                return;
            }

            var syncedCount = 0;
            var updatedCount = 0;

            foreach (var user in users)
            {
                try
                {
                    var existsInRedis = await redisRepo.ExistsAsync(user.Id, cancellationToken);
                    
                    if (!existsInRedis)
                    {
                        await redisRepo.AddAsync(user, cancellationToken);
                        syncedCount++;
                        _logger.LogDebug("Synced new user {UserId} to Redis", user.Id.Value);
                    }
                    else
                    {
                        await redisRepo.UpdateAsync(user, cancellationToken);
                        updatedCount++;
                        _logger.LogDebug("Updated user {UserId} in Redis", user.Id.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync user {UserId} to Redis, continuing with next user", user.Id.Value);
                }
            }

            _logger.LogInformation(
                "Successfully synced users to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total",
                syncedCount, updatedCount, users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync users to Redis");
            throw;
        }
    }
}

