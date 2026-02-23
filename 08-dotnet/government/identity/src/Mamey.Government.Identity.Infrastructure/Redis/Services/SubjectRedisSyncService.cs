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
/// Background service that periodically syncs subjects from PostgreSQL to Redis.
/// </summary>
internal sealed class SubjectRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubjectRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public SubjectRedisSyncService(IServiceProvider serviceProvider, ILogger<SubjectRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncSubjectsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during subject sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncSubjectsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<SubjectPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<SubjectRedisRepository>();

        try
        {
            _logger.LogInformation("Starting subject sync from PostgreSQL to Redis");
            var subjects = await postgresRepo.BrowseAsync(cancellationToken);
            if (!subjects.Any()) { _logger.LogInformation("No subjects found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var subject in subjects)
            {
                try
                {
                    // Check if key exists (this doesn't try to deserialize)
                    var exists = await redisRepo.ExistsAsync(subject.Id, cancellationToken);
                    if (!exists)
                    {
                        await redisRepo.AddAsync(subject, cancellationToken);
                        syncedCount++;
                        _logger.LogDebug("Synced new subject {SubjectId} to Redis", subject.Id.Value);
                    }
                    else
                    {
                        // If key exists, try to update. If it fails due to deserialization error,
                        // the invalid entry will be deleted by RedisCache.GetAsync, and we'll add it fresh
                        try
                        {
                            await redisRepo.UpdateAsync(subject, cancellationToken);
                            updatedCount++;
                            _logger.LogDebug("Updated subject {SubjectId} in Redis", subject.Id.Value);
                        }
                        catch
                        {
                            // If update fails (e.g., invalid cache entry), delete and re-add
                            await redisRepo.DeleteAsync(subject.Id, cancellationToken);
                            await redisRepo.AddAsync(subject, cancellationToken);
                            syncedCount++;
                            _logger.LogDebug("Re-synced subject {SubjectId} to Redis after cleaning invalid entry", subject.Id.Value);
                        }
                    }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync subject {SubjectId} to Redis", subject.Id.Value); }
            }
            _logger.LogInformation("Successfully synced subjects to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, subjects.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync subjects to Redis"); throw; }
    }
}
