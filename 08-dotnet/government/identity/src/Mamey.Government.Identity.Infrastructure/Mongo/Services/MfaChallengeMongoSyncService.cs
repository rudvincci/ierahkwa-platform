using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class MfaChallengeMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MfaChallengeMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public MfaChallengeMongoSyncService(IServiceProvider serviceProvider, ILogger<MfaChallengeMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncMfaChallengesToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during MFA challenge sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncMfaChallengesToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<MfaChallengePostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<MfaChallengeMongoRepository>();

        try
        {
            _logger.LogInformation("Starting MFA challenge sync from PostgreSQL to MongoDB");
            var mfaChallenges = await postgresRepo.BrowseAsync(cancellationToken);
            if (!mfaChallenges.Any()) { _logger.LogInformation("No MFA challenges found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var mfaChallenge in mfaChallenges)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(mfaChallenge.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(mfaChallenge, cancellationToken); syncedCount++; _logger.LogDebug("Synced new MFA challenge {Id} to MongoDB", mfaChallenge.Id.Value); }
                    else { await mongoRepo.UpdateAsync(mfaChallenge, cancellationToken); updatedCount++; _logger.LogDebug("Updated MFA challenge {Id} in MongoDB", mfaChallenge.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync MFA challenge {Id} to MongoDB", mfaChallenge.Id.Value); }
            }
            _logger.LogInformation("Successfully synced MFA challenges to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, mfaChallenges.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync MFA challenges to MongoDB"); throw; }
    }
}
