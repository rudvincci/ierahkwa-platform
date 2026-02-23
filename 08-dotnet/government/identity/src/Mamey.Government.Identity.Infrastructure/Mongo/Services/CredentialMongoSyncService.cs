using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class CredentialMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CredentialMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public CredentialMongoSyncService(IServiceProvider serviceProvider, ILogger<CredentialMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncCredentialsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during credential sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncCredentialsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<CredentialPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<CredentialMongoRepository>();

        try
        {
            _logger.LogInformation("Starting credential sync from PostgreSQL to MongoDB");
            var credentials = await postgresRepo.BrowseAsync(cancellationToken);
            if (!credentials.Any()) { _logger.LogInformation("No credentials found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var credential in credentials)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(credential.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(credential, cancellationToken); syncedCount++; _logger.LogDebug("Synced new credential {Id} to MongoDB", credential.Id.Value); }
                    else { await mongoRepo.UpdateAsync(credential, cancellationToken); updatedCount++; _logger.LogDebug("Updated credential {Id} in MongoDB", credential.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync credential {Id} to MongoDB", credential.Id.Value); }
            }
            _logger.LogInformation("Successfully synced credentials to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, credentials.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync credentials to MongoDB"); throw; }
    }
}
