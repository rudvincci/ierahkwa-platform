using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class RoleMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RoleMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public RoleMongoSyncService(IServiceProvider serviceProvider, ILogger<RoleMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncRolesToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during role sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncRolesToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<RolePostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<RoleMongoRepository>();

        try
        {
            _logger.LogInformation("Starting role sync from PostgreSQL to MongoDB");
            var roles = await postgresRepo.BrowseAsync(cancellationToken);
            if (!roles.Any()) { _logger.LogInformation("No roles found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var role in roles)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(role.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(role, cancellationToken); syncedCount++; _logger.LogDebug("Synced new role {Id} to MongoDB", role.Id.Value); }
                    else { await mongoRepo.UpdateAsync(role, cancellationToken); updatedCount++; _logger.LogDebug("Updated role {Id} in MongoDB", role.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync role {Id} to MongoDB", role.Id.Value); }
            }
            _logger.LogInformation("Successfully synced roles to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, roles.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync roles to MongoDB"); throw; }
    }
}
