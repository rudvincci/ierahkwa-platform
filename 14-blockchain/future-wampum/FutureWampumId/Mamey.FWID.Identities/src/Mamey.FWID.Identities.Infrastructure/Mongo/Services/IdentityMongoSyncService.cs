using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Options;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Mongo.Services;

/// <summary>
/// Background service that periodically syncs Identity entities from PostgreSQL to MongoDB.
/// This ensures MongoDB read model stays up-to-date with the source of truth in PostgreSQL.
/// </summary>
internal sealed class IdentityMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentityMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public IdentityMongoSyncService(
        IServiceProvider serviceProvider,
        ILogger<IdentityMongoSyncService> logger,
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

        // Wait for the application to fully start and database to be initialized
        try
        {
            await Task.Delay(_options.InitialDelay, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("MongoDB sync service is shutting down before initialization completed");
            return;
        }
        catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("MongoDB sync service is shutting down before initialization completed");
            return;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncIdentitysToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown requested - exit gracefully
                _logger.LogInformation("MongoDB sync service is shutting down");
                break;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutdown requested - exit gracefully
                _logger.LogInformation("MongoDB sync service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Identity sync to MongoDB");
                
                // Only retry if not shutting down
                if (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_options.RetryDelay, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("MongoDB sync service is shutting down");
                        break;
                    }
                    catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("MongoDB sync service is shutting down");
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private async Task SyncIdentitysToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<IdentityPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<IdentityMongoRepository>();

        try
        {
            _logger.LogInformation("Starting Identity sync from PostgreSQL to MongoDB");

            // Get all Identity entities from PostgreSQL (source of truth)
            var entities = await postgresRepo.BrowseAsync(cancellationToken);
            
            if (!entities.Any())
            {
                _logger.LogInformation("No Identity entities found in PostgreSQL, skipping sync");
                return;
            }

            var syncedCount = 0;
            var updatedCount = 0;

            foreach (var entity in entities)
            {
                try
                {
                    // Check if entity exists in MongoDB
                    var existsInMongo = await mongoRepo.ExistsAsync(entity.Id, cancellationToken);
                    
                    if (!existsInMongo)
                    {
                        // Entity doesn't exist in MongoDB, add it
                        await mongoRepo.AddAsync(entity, cancellationToken);
                        syncedCount++;
                        _logger.LogDebug("Synced new Identity {IdentityId} to MongoDB", entity.Id.Value);
                    }
                    else
                    {
                        // Entity exists, update it with latest data from PostgreSQL
                        await mongoRepo.UpdateAsync(entity, cancellationToken);
                        updatedCount++;
                        _logger.LogDebug("Updated Identity {IdentityId} in MongoDB", entity.Id.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to sync Identity {IdentityId} to MongoDB, continuing with next entity", entity.Id.Value);
                }
            }

            _logger.LogInformation(
                "Successfully synced Identity entities to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total",
                syncedCount, updatedCount, entities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync Identity entities to MongoDB");
            throw;
        }
    }
}

