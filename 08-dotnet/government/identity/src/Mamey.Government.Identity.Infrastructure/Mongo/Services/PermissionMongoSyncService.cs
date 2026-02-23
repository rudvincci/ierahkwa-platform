using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class PermissionMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public PermissionMongoSyncService(IServiceProvider serviceProvider, ILogger<PermissionMongoSyncService> logger, IOptions<MongoSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Mongo sync service is disabled");
            return;
        }

        
        await Task.Delay(_options.InitialDelay, stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncPermissionsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during permission sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncPermissionsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<PermissionPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<PermissionMongoRepository>();

        try
        {
            _logger.LogInformation("Starting permission sync from PostgreSQL to MongoDB");
            var permissions = await postgresRepo.BrowseAsync(cancellationToken);
            if (!permissions.Any()) { _logger.LogInformation("No permissions found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var permission in permissions)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(permission.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(permission, cancellationToken); syncedCount++; _logger.LogDebug("Synced new permission {Id} to MongoDB", permission.Id.Value); }
                    else { await mongoRepo.UpdateAsync(permission, cancellationToken); updatedCount++; _logger.LogDebug("Updated permission {Id} in MongoDB", permission.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync permission {Id} to MongoDB", permission.Id.Value); }
            }
            _logger.LogInformation("Successfully synced permissions to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, permissions.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync permissions to MongoDB"); throw; }
    }
}
