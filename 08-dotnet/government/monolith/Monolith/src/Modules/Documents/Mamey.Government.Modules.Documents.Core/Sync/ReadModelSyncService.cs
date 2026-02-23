using System.Diagnostics;
using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.Mongo.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Sync;

internal class ReadModelSyncService : IReadModelSyncService
{
    private readonly IDocumentRepository _postgresRepository;
    private readonly DocumentMongoRepository _mongoRepository;
    private readonly ILogger<ReadModelSyncService> _logger;
    
    private long _totalSynced = 0;
    private long _totalFailed = 0;
    private DateTime _lastSyncTime = DateTime.UtcNow;
    private readonly List<TimeSpan> _recentSyncLatencies = new();
    private readonly object _metricsLock = new();

    public ReadModelSyncService(
        IDocumentRepository postgresRepository,
        DocumentMongoRepository mongoRepository,
        ILogger<ReadModelSyncService> logger)
    {
        _postgresRepository = postgresRepository;
        _mongoRepository = mongoRepository;
        _logger = logger;
    }

    public async Task SyncDocumentAsync(Document document, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var exists = await _mongoRepository.ExistsAsync(document.Id, cancellationToken);
            
            if (exists)
            {
                await _mongoRepository.UpdateAsync(document, cancellationToken);
            }
            else
            {
                await _mongoRepository.AddAsync(document, cancellationToken);
            }

            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            _logger.LogError(ex, "Failed to sync Document {DocumentId} to MongoDB", document.Id.Value);
            throw;
        }
    }

    public async Task RemoveDocumentAsync(DocumentId documentId, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await _mongoRepository.DeleteAsync(documentId, cancellationToken);
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: true);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            UpdateMetrics(stopwatch.Elapsed, success: false);
            _logger.LogError(ex, "Failed to remove Document {DocumentId} from MongoDB", documentId.Value);
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
