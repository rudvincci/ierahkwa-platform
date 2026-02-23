using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Mongo.Services;

internal sealed class EmailConfirmationMongoSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailConfirmationMongoSyncService> _logger;
    private readonly MongoSyncOptions _options;

    public EmailConfirmationMongoSyncService(IServiceProvider serviceProvider, ILogger<EmailConfirmationMongoSyncService> logger, IOptions<MongoSyncOptions> options)
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
                await SyncEmailConfirmationsToMongo(stoppingToken);
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email confirmation sync to MongoDB");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncEmailConfirmationsToMongo(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<EmailConfirmationPostgresRepository>();
        var mongoRepo = scope.ServiceProvider.GetRequiredService<EmailConfirmationMongoRepository>();

        try
        {
            _logger.LogInformation("Starting email confirmation sync from PostgreSQL to MongoDB");
            var emailConfirmations = await postgresRepo.BrowseAsync(cancellationToken);
            if (!emailConfirmations.Any()) { _logger.LogInformation("No email confirmations found, skipping sync"); return; }

            var syncedCount = 0; var updatedCount = 0;
            foreach (var emailConfirmation in emailConfirmations)
            {
                try
                {
                    var exists = await mongoRepo.ExistsAsync(emailConfirmation.Id, cancellationToken);
                    if (!exists) { await mongoRepo.AddAsync(emailConfirmation, cancellationToken); syncedCount++; _logger.LogDebug("Synced new email confirmation {Id} to MongoDB", emailConfirmation.Id.Value); }
                    else { await mongoRepo.UpdateAsync(emailConfirmation, cancellationToken); updatedCount++; _logger.LogDebug("Updated email confirmation {Id} in MongoDB", emailConfirmation.Id.Value); }
                }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to sync email confirmation {Id} to MongoDB", emailConfirmation.Id.Value); }
            }
            _logger.LogInformation("Successfully synced email confirmations to MongoDB: {SyncedCount} new, {UpdatedCount} updated, {TotalCount} total", syncedCount, updatedCount, emailConfirmations.Count);
        }
        catch (Exception ex) { _logger.LogError(ex, "Failed to sync email confirmations to MongoDB"); throw; }
    }
}
