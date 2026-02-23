using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Government.Identity.Infrastructure.Redis.Services;

/// <summary>
/// Background service that periodically syncs sessions from Redis to PostgreSQL.
/// This ensures PostgreSQL backup stays up-to-date with active sessions in Redis.
/// Note: This syncs in the opposite direction (Redis → PostgreSQL) compared to other sync services.
/// </summary>
public class SessionSyncService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionSyncService> _logger;
    private readonly RedisSyncOptions _options;

    public SessionSyncService(IServiceProvider serviceProvider, ILogger<SessionSyncService> logger, IOptions<RedisSyncOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Session sync service is disabled");
            return;
        }

        // Wait for the application to fully start and database to be initialized
        await Task.Delay(_options.InitialDelay, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncSessionsToPostgreSQL();
                await Task.Delay(_options.SyncInterval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during session sync to PostgreSQL");
                await Task.Delay(_options.RetryDelay, stoppingToken);
            }
        }
    }

    private async Task SyncSessionsToPostgreSQL()
    {
        using var scope = _serviceProvider.CreateScope();
        var redisRepo = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
        var postgresRepo = scope.ServiceProvider.GetRequiredService<SessionPostgresRepository>();

        try
        {
            // Check if PostgreSQL database is available
            if (!await postgresRepo.CanConnectAsync())
            {
                _logger.LogWarning("PostgreSQL database is not available, skipping session sync");
                return;
            }

            // Get all active sessions from Redis
            var activeSessions = await redisRepo.GetActiveSessionsAsync();
            
            foreach (var session in activeSessions)
            {
                // Check if session exists in PostgreSQL
                var existingSession = await postgresRepo.GetByIdAsync(session.Id);
                
                if (existingSession == null)
                {
                    // Session doesn't exist in PostgreSQL, add it
                    await postgresRepo.AddAsync(session);
                    _logger.LogInformation("Synced new session {SessionId} to PostgreSQL", session.Id);
                }
                else
                {
                    // Session exists, update it with latest data from Redis
                    await postgresRepo.UpdateAsync(session);
                    _logger.LogDebug("Updated session {SessionId} in PostgreSQL", session.Id);
                }
            }

            _logger.LogInformation("Successfully synced {Count} sessions to PostgreSQL", activeSessions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync sessions to PostgreSQL");
            throw;
        }
    }
}
