using System.Diagnostics;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Sync;

/// <summary>
/// Service for synchronizing write model (PostgreSQL) to read model (MongoDB) in real-time.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal class ReadModelSyncService : IReadModelSyncService
{
    private readonly IIdentityRepository _postgresRepository;
    private readonly IdentityMongoRepository _mongoRepository;
    private readonly ILogger<ReadModelSyncService> _logger;
    
    private long _totalSynced = 0;
    private long _totalFailed = 0;
    private DateTime _lastSyncTime = DateTime.UtcNow;
    private readonly List<TimeSpan> _recentSyncLatencies = new();
    private readonly object _metricsLock = new();

    public ReadModelSyncService(
        IIdentityRepository postgresRepository,
        IdentityMongoRepository mongoRepository,
        ILogger<ReadModelSyncService> logger)
    {
        _postgresRepository = postgresRepository;
        _mongoRepository = mongoRepository;
        _logger = logger;
    }

    public async Task SyncIdentityAsync(Identity identity, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug(
                "Syncing Identity {IdentityId} to MongoDB read model",
                identity.Id.Value);

            // Check if exists in MongoDB
            var exists = await _mongoRepository.ExistsAsync(identity.Id, cancellationToken);
            
            if (exists)
            {
                await _mongoRepository.UpdateAsync(identity, cancellationToken);
                _logger.LogDebug(
                    "Updated Identity {IdentityId} in MongoDB read model",
                    identity.Id.Value);
            }
            else
            {
                await _mongoRepository.AddAsync(identity, cancellationToken);
                _logger.LogDebug(
                    "Added Identity {IdentityId} to MongoDB read model",
                    identity.Id.Value);
            }

            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
            
            _logger.LogInformation(
                "Successfully synced Identity {IdentityId} to MongoDB in {ElapsedMs}ms",
                identity.Id.Value,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            
            _logger.LogError(ex,
                "Failed to sync Identity {IdentityId} to MongoDB read model",
                identity.Id.Value);
            
            throw;
        }
    }

    public async Task RemoveIdentityAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug(
                "Removing Identity {IdentityId} from MongoDB read model",
                identityId.Value);

            await _mongoRepository.DeleteAsync(identityId, cancellationToken);
            
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
            
            _logger.LogInformation(
                "Successfully removed Identity {IdentityId} from MongoDB read model in {ElapsedMs}ms",
                identityId.Value,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            
            _logger.LogError(ex,
                "Failed to remove Identity {IdentityId} from MongoDB read model",
                identityId.Value);
            
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
