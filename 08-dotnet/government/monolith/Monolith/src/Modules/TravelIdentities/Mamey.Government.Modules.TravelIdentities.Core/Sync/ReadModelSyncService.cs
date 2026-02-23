using System.Diagnostics;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Sync;

internal class ReadModelSyncService : IReadModelSyncService
{
    private readonly ITravelIdentityRepository _postgresRepository;
    private readonly TravelIdentityMongoRepository _mongoRepository;
    private readonly ILogger<ReadModelSyncService> _logger;
    
    private long _totalSynced = 0;
    private long _totalFailed = 0;
    private DateTime _lastSyncTime = DateTime.UtcNow;
    private readonly List<TimeSpan> _recentSyncLatencies = new();
    private readonly object _metricsLock = new();

    public ReadModelSyncService(
        ITravelIdentityRepository postgresRepository,
        TravelIdentityMongoRepository mongoRepository,
        ILogger<ReadModelSyncService> logger)
    {
        _postgresRepository = postgresRepository;
        _mongoRepository = mongoRepository;
        _logger = logger;
    }

    public async Task SyncTravelIdentityAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var exists = await _mongoRepository.ExistsAsync(travelIdentity.Id, cancellationToken);
            
            if (exists)
            {
                await _mongoRepository.UpdateAsync(travelIdentity, cancellationToken);
            }
            else
            {
                await _mongoRepository.AddAsync(travelIdentity, cancellationToken);
            }

            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            _logger.LogError(ex, "Failed to sync TravelIdentity {TravelIdentityId} to MongoDB", travelIdentity.Id.Value);
            throw;
        }
    }

    public async Task RemoveTravelIdentityAsync(TravelIdentityId travelIdentityId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _mongoRepository.DeleteAsync(travelIdentityId, cancellationToken);
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            _logger.LogError(ex, "Failed to remove TravelIdentity {TravelIdentityId} from MongoDB", travelIdentityId.Value);
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
            if (success) _totalSynced++; else _totalFailed++;
            _recentSyncLatencies.Add(latency);
            if (_recentSyncLatencies.Count > 100) _recentSyncLatencies.RemoveAt(0);
        }
    }
}
