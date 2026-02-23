using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

internal sealed class EmailConfirmationRedisSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailConfirmationRedisSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public EmailConfirmationRedisSyncService(IServiceProvider serviceProvider, ILogger<EmailConfirmationRedisSyncService> logger, IOptions<RedisSyncOptions> options)
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
                await SyncEmailConfirmationsToRedis(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during emailconfirmation sync to Redis");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncEmailConfirmationsToRedis(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<EmailConfirmationPostgresRepository>();
        var redisRepo = scope.ServiceProvider.GetRequiredService<EmailConfirmationRedisRepository>();

        try
        {
            _logger.LogInformation("Starting emailconfirmation sync from PostgreSQL to Redis");
            var emailconfirmations = await postgresRepo.BrowseAsync(cancellationToken);
            if (!emailconfirmations.Any()) { _logger.LogInformation("No emailconfirmations found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var emailconfirmation in emailconfirmations)
            {
                try
                {
                    var exists = await redisRepo.ExistsAsync(emailconfirmation.Id, cancellationToken);
                    if (!exists) { await redisRepo.AddAsync(emailconfirmation, cancellationToken); syncedCount++; _logger.LogDebug("Synced new emailconfirmation {Id} to Redis", emailconfirmation.Id.Value); }
                    else { await redisRepo.UpdateAsync(emailconfirmation, cancellationToken); updatedCount++; _logger.LogDebug("Updated emailconfirmation {Id} in Redis", emailconfirmation.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync emailconfirmation {Id} to Redis", emailconfirmation.Id.Value); }
            }
            _logger.LogInformation("Successfully synced emailconfirmations to Redis: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, emailconfirmations.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync emailconfirmations to Redis"); throw; }
    }
}
