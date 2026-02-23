using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

/// <summary>
/// Background service that periodically syncs users from PostgreSQL to MongoDB.
/// This ensures MongoDB read model stays up-to-date with the source of truth in PostgreSQL.
/// </summary>
internal sealed class UserMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public UserMongoSyncService(
        IServiceProvider serviceProvider,
        ILogger<UserMongoSyncService> logger,
        IOptions<MongoSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("MongoDB sync service is disabled");
            return;
        }

        await Task.Delay(_options.InitialDelay, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncUsersToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncUsersToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<UserPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<UserMongoRepository>();

        try
        {
            _logger.LogInformation("Starting user sync from PostgreSQL to MongoDB");

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
                    var existsInMongo = await mongoRepo.ExistsAsync(user.Id, cancellationToken);
                    
                    if (!existsInMongo)
                    {
                        await mongoRepo.AddAsync(user, cancellationToken);
                        syncedCount++;
                        _logger.LogDebug("Synced new user {UserId} to MongoDB", user.Id.Value);
                    }
                    else
                    {
                        await mongoRepo.UpdateAsync(user, cancellationToken);
                        updatedCount++;
                        _logger.LogDebug("Updated user {UserId} in MongoDB", user.Id.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync user {UserId} to MongoDB, continuing with next user", user.Id.Value);
                }
            }

            _logger.LogInformation(
                "Successfully synced users to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total",
                syncedCount, updatedCount, users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync users to MongoDB");
            throw;
        }
    }
}

