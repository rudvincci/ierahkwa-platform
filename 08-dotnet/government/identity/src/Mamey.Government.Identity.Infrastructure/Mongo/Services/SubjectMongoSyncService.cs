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
/// Background service that periodically syncs subjects from PostgreSQL to MongoDB.
/// </summary>
internal sealed class SubjectMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubjectMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public SubjectMongoSyncService(IServiceProvider serviceProvider, ILogger<SubjectMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncSubjectsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during subject sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncSubjectsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<SubjectPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<SubjectMongoRepository>();

        try
        {
            _logger.LogInformation("Starting subject sync from PostgreSQL to MongoDB");
            var subjects = await postgresRepo.BrowseAsync(cancellationToken);
            if (!subjects.Any()) { _logger.LogInformation("No subjects found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var subject in subjects)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(subject.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(subject, cancellationToken); syncedCount++; _logger.LogDebug("Synced new subject {SubjectId} to MongoDB", subject.Id.Value); }
                    else { await mongoRepo.UpdateAsync(subject, cancellationToken); updatedCount++; _logger.LogDebug("Updated subject {SubjectId} in MongoDB", subject.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync subject {SubjectId} to MongoDB", subject.Id.Value); }
            }
            _logger.LogInformation("Successfully synced subjects to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, subjects.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync subjects to MongoDB"); throw; }
    }
}

