using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.Metrics;

/// <summary>
/// Centralized Prometheus metrics for blockchain operations.
/// Provides comprehensive observability for MameyNode blockchain integration.
/// 
/// TDD Reference: Architecture Analysis Lines 604-609
/// BDD Reference: Lines 152-170 (II.2 KPIs)
/// 
/// Metrics are organized into categories:
/// - Operations: Track blockchain operation success/failure
/// - Latency: Track operation duration
/// - Queue: Track batch queue depths and processing
/// - Circuit Breaker: Track resilience patterns
/// - Health: Track overall blockchain health
/// </summary>
public static class BlockchainMetrics
{
    #region Operation Metrics

    /// <summary>
    /// Total blockchain operations counter.
    /// Labels: service, operation, status
    /// </summary>
    public static readonly Counter OperationsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_operations_total",
            "Total blockchain operations",
            new CounterConfiguration
            {
                LabelNames = new[] { "service", "operation", "status" }
            });

    /// <summary>
    /// Blockchain operation duration histogram.
    /// Labels: service, operation
    /// Buckets optimized for typical blockchain latencies.
    /// </summary>
    public static readonly Histogram OperationDuration = PrometheusMetrics
        .CreateHistogram(
            "blockchain_operation_duration_seconds",
            "Duration of blockchain operations in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "service", "operation" },
                Buckets = new[] { 0.01, 0.05, 0.1, 0.25, 0.5, 1.0, 2.5, 5.0, 10.0 }
            });

    /// <summary>
    /// Blockchain account creation counter.
    /// Labels: status (success, failure, circuit_open)
    /// </summary>
    public static readonly Counter AccountCreationsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_account_creations_total",
            "Total blockchain account creation attempts",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    /// <summary>
    /// Blockchain account creation duration histogram.
    /// </summary>
    public static readonly Histogram AccountCreationDuration = PrometheusMetrics
        .CreateHistogram(
            "blockchain_account_creation_duration_seconds",
            "Duration of blockchain account creation in seconds",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.1, 2, 10)
            });

    #endregion

    #region Queue Metrics

    /// <summary>
    /// Current batch queue depth gauge.
    /// </summary>
    public static readonly Gauge BatchQueueDepth = PrometheusMetrics
        .CreateGauge(
            "blockchain_batch_queue_depth",
            "Current depth of the blockchain batch queue");

    /// <summary>
    /// Outbox queue depth for failed operations awaiting retry.
    /// </summary>
    public static readonly Gauge OutboxQueueDepth = PrometheusMetrics
        .CreateGauge(
            "blockchain_outbox_queue_depth",
            "Current depth of the blockchain outbox queue for failed operations");

    /// <summary>
    /// Total events queued for batching counter.
    /// Labels: event_type
    /// </summary>
    public static readonly Counter EventsQueuedTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_events_queued_total",
            "Total events queued for batch processing",
            new CounterConfiguration
            {
                LabelNames = new[] { "event_type" }
            });

    /// <summary>
    /// Total events sent counter.
    /// Labels: status
    /// </summary>
    public static readonly Counter EventsSentTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_events_sent_total",
            "Total events sent to blockchain",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    /// <summary>
    /// Total batches sent counter.
    /// Labels: status
    /// </summary>
    public static readonly Counter BatchesSentTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_batches_sent_total",
            "Total batches sent to blockchain",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    /// <summary>
    /// Batch size histogram.
    /// </summary>
    public static readonly Histogram BatchSize = PrometheusMetrics
        .CreateHistogram(
            "blockchain_batch_size",
            "Number of events per batch",
            new HistogramConfiguration
            {
                Buckets = new[] { 1.0, 5.0, 10.0, 25.0, 50.0, 75.0, 100.0, 150.0, 200.0 }
            });

    /// <summary>
    /// Batch processing latency histogram.
    /// </summary>
    public static readonly Histogram BatchLatency = PrometheusMetrics
        .CreateHistogram(
            "blockchain_batch_latency_seconds",
            "Batch processing latency in seconds",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 12)
            });

    #endregion

    #region Circuit Breaker Metrics

    /// <summary>
    /// Circuit breaker state gauge.
    /// 0 = Closed (healthy), 1 = Open (blocking), 2 = Half-Open (testing)
    /// </summary>
    public static readonly Gauge CircuitBreakerState = PrometheusMetrics
        .CreateGauge(
            "blockchain_circuit_breaker_state",
            "Circuit breaker state: 0=Closed, 1=Open, 2=HalfOpen");

    /// <summary>
    /// Consecutive failures count gauge.
    /// </summary>
    public static readonly Gauge ConsecutiveFailures = PrometheusMetrics
        .CreateGauge(
            "blockchain_consecutive_failures",
            "Number of consecutive blockchain operation failures");

    /// <summary>
    /// Total retry attempts counter.
    /// Labels: operation, outcome (success, failure, exhausted)
    /// </summary>
    public static readonly Counter RetryAttemptsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_retry_attempts_total",
            "Total retry attempts for blockchain operations",
            new CounterConfiguration
            {
                LabelNames = new[] { "operation", "outcome" }
            });

    #endregion

    #region Health Metrics

    /// <summary>
    /// Blockchain connection health gauge.
    /// 1 = healthy, 0 = unhealthy
    /// </summary>
    public static readonly Gauge ConnectionHealth = PrometheusMetrics
        .CreateGauge(
            "blockchain_connection_health",
            "Blockchain connection health: 1=healthy, 0=unhealthy");

    /// <summary>
    /// Last successful operation timestamp gauge.
    /// Unix timestamp of last successful blockchain operation.
    /// </summary>
    public static readonly Gauge LastSuccessfulOperation = PrometheusMetrics
        .CreateGauge(
            "blockchain_last_successful_operation_timestamp",
            "Unix timestamp of the last successful blockchain operation");

    /// <summary>
    /// Background service processing state gauge.
    /// 1 = active, 0 = inactive
    /// </summary>
    public static readonly Gauge BackgroundProcessingActive = PrometheusMetrics
        .CreateGauge(
            "blockchain_background_processing_active",
            "Whether background blockchain processing is active: 1=active, 0=inactive");

    /// <summary>
    /// Sync service cycles counter.
    /// Labels: status (success, skipped, error)
    /// </summary>
    public static readonly Counter SyncCyclesTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_sync_cycles_total",
            "Total blockchain sync cycles",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    #endregion

    #region Compliance Metrics

    /// <summary>
    /// Compliance audit entries created counter.
    /// Labels: entry_type
    /// </summary>
    public static readonly Counter ComplianceEntriesTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_compliance_entries_total",
            "Total compliance audit entries created",
            new CounterConfiguration
            {
                LabelNames = new[] { "entry_type" }
            });

    /// <summary>
    /// KYC verifications counter.
    /// Labels: status (verified, failed, pending)
    /// </summary>
    public static readonly Counter KycVerificationsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_kyc_verifications_total",
            "Total KYC verifications performed",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    /// <summary>
    /// AML checks counter.
    /// Labels: status (cleared, flagged, pending)
    /// </summary>
    public static readonly Counter AmlChecksTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_aml_checks_total",
            "Total AML checks performed",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    #endregion

    #region Government Identity Metrics

    /// <summary>
    /// Government identity registrations counter.
    /// Labels: status
    /// </summary>
    public static readonly Counter GovernmentRegistrationsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_government_registrations_total",
            "Total government identity registrations",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    /// <summary>
    /// Government identity verifications counter.
    /// Labels: status
    /// </summary>
    public static readonly Counter GovernmentVerificationsTotal = PrometheusMetrics
        .CreateCounter(
            "blockchain_government_verifications_total",
            "Total government identity verifications",
            new CounterConfiguration
            {
                LabelNames = new[] { "status" }
            });

    #endregion

    #region Helper Methods

    /// <summary>
    /// Records a successful blockchain operation.
    /// </summary>
    public static void RecordOperationSuccess(string service, string operation, double durationSeconds)
    {
        OperationsTotal.WithLabels(service, operation, "success").Inc();
        OperationDuration.WithLabels(service, operation).Observe(durationSeconds);
        ConnectionHealth.Set(1);
        LastSuccessfulOperation.Set(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    /// <summary>
    /// Records a failed blockchain operation.
    /// </summary>
    public static void RecordOperationFailure(string service, string operation, double durationSeconds)
    {
        OperationsTotal.WithLabels(service, operation, "failure").Inc();
        OperationDuration.WithLabels(service, operation).Observe(durationSeconds);
    }

    /// <summary>
    /// Records an operation blocked by circuit breaker.
    /// </summary>
    public static void RecordOperationBlocked(string service, string operation)
    {
        OperationsTotal.WithLabels(service, operation, "circuit_open").Inc();
    }

    /// <summary>
    /// Updates the circuit breaker state metric.
    /// </summary>
    public static void UpdateCircuitBreakerState(CircuitBreakerStateValue state)
    {
        CircuitBreakerState.Set((int)state);
    }

    /// <summary>
    /// Records batch processing metrics.
    /// </summary>
    public static void RecordBatchSent(int batchSize, double latencySeconds, bool success)
    {
        BatchesSentTotal.WithLabels(success ? "success" : "failure").Inc();
        if (success)
        {
            BatchSize.Observe(batchSize);
            BatchLatency.Observe(latencySeconds);
            EventsSentTotal.WithLabels("success").Inc(batchSize);
        }
    }

    #endregion
}

/// <summary>
/// Circuit breaker state values for metrics.
/// </summary>
public enum CircuitBreakerStateValue
{
    /// <summary>Circuit is closed (healthy, allowing operations)</summary>
    Closed = 0,
    
    /// <summary>Circuit is open (blocking operations due to failures)</summary>
    Open = 1,
    
    /// <summary>Circuit is half-open (testing if service recovered)</summary>
    HalfOpen = 2
}
