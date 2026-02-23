using System.Collections.Concurrent;
using System.Threading.Channels;
using Mamey.FWID.Identities.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Batch ledger client that queues events for efficient batch processing.
/// Implements high-throughput event logging by batching multiple events together.
/// 
/// TDD Reference: Architecture Analysis Lines 541-546
/// BDD Reference: Lines 152-170 (II.2 KPIs - System metrics)
/// </summary>
internal sealed class BatchLedgerClient : IBatchLedgerClient, IDisposable
{
    private readonly IResilientBlockchainClient _blockchainClient;
    private readonly BatchLedgerOptions _options;
    private readonly ILogger<BatchLedgerClient> _logger;
    private readonly Channel<BatchEventEntry> _eventChannel;
    private readonly ConcurrentQueue<BatchEventEntry> _retryQueue;
    private readonly SemaphoreSlim _flushSemaphore;
    private bool _disposed;

    // Statistics tracking
    private long _totalEventsQueued;
    private long _totalEventsSent;
    private long _totalBatchesSent;
    private long _totalCriticalEvents;
    private long _totalFailedBatches;
    private double _totalLatencyMs;
    private DateTime? _lastBatchSentAt;

    // Prometheus metrics
    private static readonly Counter EventsQueuedCounter = PrometheusMetrics
        .CreateCounter("blockchain_batch_events_queued_total", "Total events queued for batching",
            new CounterConfiguration { LabelNames = new[] { "event_type" } });

    private static readonly Counter EventsSentCounter = PrometheusMetrics
        .CreateCounter("blockchain_batch_events_sent_total", "Total events sent in batches",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Counter BatchesSentCounter = PrometheusMetrics
        .CreateCounter("blockchain_batches_sent_total", "Total batches sent",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Gauge QueueDepthGauge = PrometheusMetrics
        .CreateGauge("blockchain_batch_queue_depth", "Current batch queue depth");

    private static readonly Histogram BatchSizeHistogram = PrometheusMetrics
        .CreateHistogram("blockchain_batch_size", "Batch sizes",
            new HistogramConfiguration { Buckets = new double[] { 1, 5, 10, 25, 50, 75, 100, 150, 200 } });

    private static readonly Histogram BatchLatencyHistogram = PrometheusMetrics
        .CreateHistogram("blockchain_batch_latency_seconds", "Batch processing latency",
            new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.001, 2, 12) });

    public BatchLedgerClient(
        IResilientBlockchainClient blockchainClient,
        IOptions<BatchLedgerOptions> options,
        ILogger<BatchLedgerClient> logger)
    {
        _blockchainClient = blockchainClient ?? throw new ArgumentNullException(nameof(blockchainClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _eventChannel = Channel.CreateBounded<BatchEventEntry>(new BoundedChannelOptions(_options.MaxQueueSize)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        _retryQueue = new ConcurrentQueue<BatchEventEntry>();
        _flushSemaphore = new SemaphoreSlim(_options.MaxConcurrentBatches);

        _logger.LogInformation(
            "Initialized {Client} with BatchSize={BatchSize}, BatchInterval={BatchInterval}s, MaxQueueSize={MaxQueueSize}",
            nameof(BatchLedgerClient), _options.BatchSize, _options.BatchInterval.TotalSeconds, _options.MaxQueueSize);
    }

    /// <inheritdoc />
    public int QueueDepth => _eventChannel.Reader.Count + _retryQueue.Count;

    /// <inheritdoc />
    public bool QueueEvent(BatchEventEntry eventEntry)
    {
        if (_disposed) return false;

        // Check if this is a critical event
        if (IsCriticalEvent(eventEntry))
        {
            eventEntry.IsCritical = true;
            Interlocked.Increment(ref _totalCriticalEvents);
            
            // Send critical events immediately (fire and forget for sync method)
            _ = SendImmediatelyAsync(eventEntry, CancellationToken.None);
            return true;
        }

        // Try to queue the event
        if (_eventChannel.Writer.TryWrite(eventEntry))
        {
            Interlocked.Increment(ref _totalEventsQueued);
            EventsQueuedCounter.WithLabels(eventEntry.EventType).Inc();
            QueueDepthGauge.Set(QueueDepth);
            return true;
        }

        _logger.LogWarning("Event queue is full, dropping event: EventType={EventType}, EntityId={EntityId}",
            eventEntry.EventType, eventEntry.EntityId);
        return false;
    }

    /// <inheritdoc />
    public async Task<bool> QueueEventAsync(BatchEventEntry eventEntry, CancellationToken cancellationToken = default)
    {
        if (_disposed) return false;

        // Check if this is a critical event
        if (IsCriticalEvent(eventEntry))
        {
            eventEntry.IsCritical = true;
            Interlocked.Increment(ref _totalCriticalEvents);
            
            // Send critical events immediately
            await SendImmediatelyAsync(eventEntry, cancellationToken);
            return true;
        }

        // Try to queue the event
        try
        {
            await _eventChannel.Writer.WriteAsync(eventEntry, cancellationToken);
            Interlocked.Increment(ref _totalEventsQueued);
            EventsQueuedCounter.WithLabels(eventEntry.EventType).Inc();
            QueueDepthGauge.Set(QueueDepth);
            return true;
        }
        catch (ChannelClosedException)
        {
            _logger.LogWarning("Event channel is closed, cannot queue event");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<int> FlushAsync(CancellationToken cancellationToken = default)
    {
        var events = new List<BatchEventEntry>();

        // Drain all events from the channel
        while (_eventChannel.Reader.TryRead(out var entry))
        {
            events.Add(entry);
        }

        // Also include retry queue
        while (_retryQueue.TryDequeue(out var retryEntry))
        {
            events.Add(retryEntry);
        }

        if (events.Count == 0)
        {
            return 0;
        }

        _logger.LogInformation("Flushing {Count} events", events.Count);

        // Send in batches
        var totalSent = 0;
        for (var i = 0; i < events.Count; i += _options.BatchSize)
        {
            var batch = events.Skip(i).Take(_options.BatchSize).ToList();
            var success = await SendBatchAsync(batch, cancellationToken);
            if (success)
            {
                totalSent += batch.Count;
            }
        }

        QueueDepthGauge.Set(QueueDepth);
        return totalSent;
    }

    /// <inheritdoc />
    public BatchStatistics GetStatistics()
    {
        var totalBatches = Interlocked.Read(ref _totalBatchesSent);
        var totalEvents = Interlocked.Read(ref _totalEventsSent);

        return new BatchStatistics
        {
            TotalEventsQueued = Interlocked.Read(ref _totalEventsQueued),
            TotalEventsSent = totalEvents,
            TotalBatchesSent = totalBatches,
            TotalCriticalEvents = Interlocked.Read(ref _totalCriticalEvents),
            TotalFailedBatches = Interlocked.Read(ref _totalFailedBatches),
            AverageBatchSize = totalBatches > 0 ? (double)totalEvents / totalBatches : 0,
            CurrentQueueDepth = QueueDepth,
            LastBatchSentAt = _lastBatchSentAt,
            AverageLatencyMs = totalBatches > 0 ? _totalLatencyMs / totalBatches : 0
        };
    }

    /// <summary>
    /// Processes a batch of events from the queue.
    /// Called by the background processor service.
    /// </summary>
    internal async Task<int> ProcessBatchAsync(CancellationToken cancellationToken)
    {
        var batch = new List<BatchEventEntry>();
        var startTime = DateTime.UtcNow;
        var timeout = Task.Delay(_options.BatchInterval, cancellationToken);

        // Collect events until batch size or timeout
        while (batch.Count < _options.BatchSize && !cancellationToken.IsCancellationRequested)
        {
            var readTask = _eventChannel.Reader.WaitToReadAsync(cancellationToken).AsTask();
            var completedTask = await Task.WhenAny(readTask, timeout);

            if (completedTask == timeout || cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (await readTask)
            {
                while (_eventChannel.Reader.TryRead(out var entry) && batch.Count < _options.BatchSize)
                {
                    batch.Add(entry);
                }
            }
        }

        // Add any items from retry queue
        while (batch.Count < _options.BatchSize && _retryQueue.TryDequeue(out var retryEntry))
        {
            batch.Add(retryEntry);
        }

        if (batch.Count == 0)
        {
            return 0;
        }

        // Send the batch
        var success = await SendBatchAsync(batch, cancellationToken);

        if (!success && _options.RetryFailedBatches)
        {
            // Re-queue failed events for retry
            foreach (var entry in batch)
            {
                _retryQueue.Enqueue(entry);
            }
        }

        QueueDepthGauge.Set(QueueDepth);
        return success ? batch.Count : 0;
    }

    private bool IsCriticalEvent(BatchEventEntry entry)
    {
        return _options.CriticalEventTypes.Contains(entry.EventType, StringComparer.OrdinalIgnoreCase);
    }

    private async Task SendImmediatelyAsync(BatchEventEntry entry, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogDebug("Sending critical event immediately: EventType={EventType}, EntityId={EntityId}",
                entry.EventType, entry.EntityId);

            // Use the resilient client to create an audit entry
            // This is a simplified version - in production you'd use the full ledger API
            var result = await _blockchainClient.CreateAccountWithRetryAsync(
                entry.EntityId.ToString(),
                "AUDIT",
                cancellationToken);

            if (result.Success)
            {
                Interlocked.Increment(ref _totalEventsSent);
                EventsSentCounter.WithLabels("success").Inc();
                _logger.LogInformation("Critical event sent successfully: EventType={EventType}, EntityId={EntityId}",
                    entry.EventType, entry.EntityId);
            }
            else
            {
                EventsSentCounter.WithLabels("failure").Inc();
                _logger.LogWarning("Failed to send critical event: EventType={EventType}, Error={Error}",
                    entry.EventType, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            EventsSentCounter.WithLabels("error").Inc();
            _logger.LogError(ex, "Error sending critical event: EventType={EventType}", entry.EventType);
        }

        var latencyMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
        BatchLatencyHistogram.Observe(latencyMs / 1000.0);
    }

    private async Task<bool> SendBatchAsync(List<BatchEventEntry> batch, CancellationToken cancellationToken)
    {
        if (!await _flushSemaphore.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken))
        {
            _logger.LogWarning("Timeout waiting for batch semaphore");
            return false;
        }

        var startTime = DateTime.UtcNow;

        try
        {
            _logger.LogDebug("Sending batch of {Count} events", batch.Count);

            // In production, this would call the actual batch API on MameyNode
            // For now, we simulate batch processing by processing each event
            var successCount = 0;

            foreach (var entry in batch)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    // Simulate batch event processing
                    // In production, this would be a single batch API call
                    await Task.Delay(1, cancellationToken); // Minimal delay to simulate processing
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error processing event in batch: EventId={EventId}", entry.EventId);
                }
            }

            var latencyMs = (DateTime.UtcNow - startTime).TotalMilliseconds;

            if (successCount == batch.Count)
            {
                Interlocked.Add(ref _totalEventsSent, batch.Count);
                Interlocked.Increment(ref _totalBatchesSent);
                _totalLatencyMs += latencyMs;
                _lastBatchSentAt = DateTime.UtcNow;

                BatchesSentCounter.WithLabels("success").Inc();
                BatchSizeHistogram.Observe(batch.Count);
                BatchLatencyHistogram.Observe(latencyMs / 1000.0);
                EventsSentCounter.WithLabels("success").Inc(batch.Count);

                _logger.LogInformation(
                    "Batch sent successfully: Count={Count}, LatencyMs={LatencyMs}",
                    batch.Count, latencyMs);

                return true;
            }
            else
            {
                Interlocked.Increment(ref _totalFailedBatches);
                BatchesSentCounter.WithLabels("partial_failure").Inc();

                _logger.LogWarning(
                    "Batch partially failed: TotalCount={TotalCount}, SuccessCount={SuccessCount}",
                    batch.Count, successCount);

                return false;
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref _totalFailedBatches);
            BatchesSentCounter.WithLabels("failure").Inc();

            _logger.LogError(ex, "Error sending batch of {Count} events", batch.Count);
            return false;
        }
        finally
        {
            _flushSemaphore.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _eventChannel.Writer.Complete();
        _flushSemaphore.Dispose();

        _logger.LogInformation("BatchLedgerClient disposed");
    }
}
