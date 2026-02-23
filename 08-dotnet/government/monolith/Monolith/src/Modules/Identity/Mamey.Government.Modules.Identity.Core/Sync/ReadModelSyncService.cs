using System.Diagnostics;
using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.Mongo.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.Sync;

/// <summary>
/// Service for synchronizing write model (PostgreSQL) to read model (MongoDB) in real-time.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal class ReadModelSyncService : IReadModelSyncService
{
    private readonly IUserProfileRepository _postgresRepository;
    private readonly UserProfileMongoRepository _mongoRepository;
    private readonly ILogger<ReadModelSyncService> _logger;
    
    private long _totalSynced = 0;
    private long _totalFailed = 0;
    private DateTime _lastSyncTime = DateTime.UtcNow;
    private readonly List<TimeSpan> _recentSyncLatencies = new();
    private readonly object _metricsLock = new();

    public ReadModelSyncService(
        IUserProfileRepository postgresRepository,
        UserProfileMongoRepository mongoRepository,
        ILogger<ReadModelSyncService> logger)
    {
        _postgresRepository = postgresRepository;
        _mongoRepository = mongoRepository;
        _logger = logger;
    }

    public async Task SyncUserProfileAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug(
                "Syncing UserProfile {UserId} to MongoDB read model",
                userProfile.Id.Value);

            // Check if exists in MongoDB
            var exists = await _mongoRepository.ExistsAsync(userProfile.Id, cancellationToken);
            
            if (exists)
            {
                await _mongoRepository.UpdateAsync(userProfile, cancellationToken);
                _logger.LogDebug(
                    "Updated UserProfile {UserId} in MongoDB read model",
                    userProfile.Id.Value);
            }
            else
            {
                await _mongoRepository.AddAsync(userProfile, cancellationToken);
                _logger.LogDebug(
                    "Added UserProfile {UserId} to MongoDB read model",
                    userProfile.Id.Value);
            }

            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
            
            _logger.LogInformation(
                "Successfully synced UserProfile {UserId} to MongoDB in {ElapsedMs}ms",
                userProfile.Id.Value,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            
            _logger.LogError(ex,
                "Failed to sync UserProfile {UserId} to MongoDB read model",
                userProfile.Id.Value);
            
            throw;
        }
    }

    public async Task RemoveUserProfileAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug(
                "Removing UserProfile {UserId} from MongoDB read model",
                userId.Value);

            await _mongoRepository.DeleteAsync(userId, cancellationToken);
            
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
            
            _logger.LogInformation(
                "Successfully removed UserProfile {UserId} from MongoDB read model in {ElapsedMs}ms",
                userId.Value,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            
            _logger.LogError(ex,
                "Failed to remove UserProfile {UserId} from MongoDB read model",
                userId.Value);
            
            throw;
        }
    }

    public async Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        lock (_metricsLock)
        {
            var averageLatency = _recentSyncLatencies.Any()
                ? TimeSpan.FromMilliseconds(_recentSyncLatencies.Average(l => l.TotalMilliseconds))
                : TimeSpan.Zero;

            // Consider healthy if less than 5% failure rate and average latency < 1 second
            var failureRate = _totalSynced + _totalFailed > 0
                ? (double)_totalFailed / (_totalSynced + _totalFailed)
                : 0.0;
            var isHealthy = failureRate < 0.05 && averageLatency < TimeSpan.FromSeconds(1);

            return new SyncStatus
            {
                LastSyncTime = _lastSyncTime,
                TotalSynced = _totalSynced,
                TotalFailed = _totalFailed,
                AverageSyncLatency = averageLatency,
                IsHealthy = isHealthy
            };
        }
    }

    private void UpdateMetrics(TimeSpan latency, bool success)
    {
        lock (_metricsLock)
        {
            _lastSyncTime = DateTime.UtcNow;
            
            if (success)
            {
                _totalSynced++;
            }
            else
            {
                _totalFailed++;
            }

            // Keep only last 100 latencies for rolling average
            _recentSyncLatencies.Add(latency);
            if (_recentSyncLatencies.Count > 100)
            {
                _recentSyncLatencies.RemoveAt(0);
            }
        }
    }
}
