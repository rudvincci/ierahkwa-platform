namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Interface for batch ledger client that queues events for batch processing.
/// Provides high-throughput event logging by batching multiple events together.
/// 
/// TDD Reference: Architecture Analysis Lines 541-546
/// BDD Reference: Lines 152-170 (II.2 KPIs - System metrics)
/// </summary>
internal interface IBatchLedgerClient
{
    /// <summary>
    /// Queues an event for batch processing.
    /// Critical events are sent immediately, bypassing the queue.
    /// </summary>
    /// <param name="eventEntry">The event to queue.</param>
    /// <returns>True if the event was queued or sent, false if the queue is full.</returns>
    bool QueueEvent(BatchEventEntry eventEntry);

    /// <summary>
    /// Queues an event for batch processing asynchronously.
    /// Critical events are sent immediately, bypassing the queue.
    /// </summary>
    /// <param name="eventEntry">The event to queue.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the event was queued or sent, false if the queue is full.</returns>
    Task<bool> QueueEventAsync(BatchEventEntry eventEntry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all queued events immediately.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of events flushed.</returns>
    Task<int> FlushAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current queue depth.
    /// </summary>
    int QueueDepth { get; }

    /// <summary>
    /// Gets statistics about batch processing.
    /// </summary>
    BatchStatistics GetStatistics();
}

/// <summary>
/// Represents an event entry to be batched.
/// </summary>
public class BatchEventEntry
{
    /// <summary>
    /// Unique identifier for this event.
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Type of the event (e.g., "IdentityCreated", "CredentialIssued").
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Source service that generated this event.
    /// </summary>
    public string SourceService { get; set; } = "identities-service";

    /// <summary>
    /// Entity type related to this event (e.g., "Identity", "Credential").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Entity ID related to this event.
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Correlation ID for distributed tracing.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Timestamp when the event was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional event data.
    /// </summary>
    public Dictionary<string, string> EventData { get; set; } = new();

    /// <summary>
    /// Whether this is a critical event that should be sent immediately.
    /// </summary>
    public bool IsCritical { get; set; }
}

/// <summary>
/// Statistics about batch processing.
/// </summary>
public class BatchStatistics
{
    /// <summary>
    /// Total number of events queued.
    /// </summary>
    public long TotalEventsQueued { get; set; }

    /// <summary>
    /// Total number of events sent.
    /// </summary>
    public long TotalEventsSent { get; set; }

    /// <summary>
    /// Total number of batches sent.
    /// </summary>
    public long TotalBatchesSent { get; set; }

    /// <summary>
    /// Total number of critical events (sent immediately).
    /// </summary>
    public long TotalCriticalEvents { get; set; }

    /// <summary>
    /// Total number of failed batches.
    /// </summary>
    public long TotalFailedBatches { get; set; }

    /// <summary>
    /// Average batch size.
    /// </summary>
    public double AverageBatchSize { get; set; }

    /// <summary>
    /// Current queue depth.
    /// </summary>
    public int CurrentQueueDepth { get; set; }

    /// <summary>
    /// Time of last batch sent.
    /// </summary>
    public DateTime? LastBatchSentAt { get; set; }

    /// <summary>
    /// Average latency for batch processing in milliseconds.
    /// </summary>
    public double AverageLatencyMs { get; set; }
}
