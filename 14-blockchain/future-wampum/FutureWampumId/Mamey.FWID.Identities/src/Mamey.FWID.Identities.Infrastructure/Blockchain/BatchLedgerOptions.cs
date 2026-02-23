namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Configuration options for batch ledger processing.
/// Enables high-throughput event logging by batching multiple events together.
/// 
/// TDD Reference: Architecture Analysis Lines 541-546
/// BDD Reference: Lines 152-170 (II.2 KPIs - System metrics)
/// </summary>
public class BatchLedgerOptions
{
    /// <summary>
    /// Whether batch processing is enabled.
    /// Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Maximum number of events to batch before sending.
    /// Default: 100.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Maximum time to wait before sending a batch (even if not full).
    /// Default: 5 seconds.
    /// </summary>
    public TimeSpan BatchInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum number of concurrent batches being processed.
    /// Default: 4.
    /// </summary>
    public int MaxConcurrentBatches { get; set; } = 4;

    /// <summary>
    /// Maximum queue size before rejecting new events.
    /// Default: 10,000.
    /// </summary>
    public int MaxQueueSize { get; set; } = 10_000;

    /// <summary>
    /// Event types that should bypass batching and be sent immediately.
    /// These are critical events that need immediate blockchain confirmation.
    /// </summary>
    public List<string> CriticalEventTypes { get; set; } = new()
    {
        "IdentityRevoked",
        "IdentityDeactivated",
        "CredentialRevoked",
        "AccessDenied",
        "SecurityAlert"
    };

    /// <summary>
    /// Whether to retry failed batches.
    /// Default: true.
    /// </summary>
    public bool RetryFailedBatches { get; set; } = true;

    /// <summary>
    /// Maximum number of retry attempts for failed batches.
    /// Default: 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts.
    /// Default: 2 seconds.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
}
