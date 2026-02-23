using System.Diagnostics;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.Blockchain;

/// <summary>
/// Resilient blockchain client with exponential backoff retry and circuit breaker pattern.
/// Wraps IMameyNodeBankingClient with enterprise-grade resilience.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity Registration)
/// </summary>
internal sealed class ResilientBlockchainClient : IResilientBlockchainClient
{
    private readonly IMameyNodeBankingClient? _bankingClient;
    private readonly IIdentityRepository _identityRepository;
    private readonly ResilientBlockchainOptions _options;
    private readonly ILogger<ResilientBlockchainClient> _logger;
    
    // Circuit breaker state
    private int _consecutiveFailures;
    private DateTime? _circuitOpenedAt;
    private DateTime? _lastSuccessfulOperation;
    private DateTime? _lastFailedOperation;
    private string? _lastErrorMessage;
    private readonly object _circuitBreakerLock = new();

    // Prometheus metrics
    private static readonly Counter BlockchainAccountCreationTotal = PrometheusMetrics
        .CreateCounter("fwid_blockchain_account_creation_total", "Total blockchain account creation attempts",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Histogram BlockchainAccountCreationDuration = PrometheusMetrics
        .CreateHistogram("fwid_blockchain_account_creation_duration_seconds", "Duration of blockchain account creation",
            new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(0.1, 2, 10) });

    private static readonly Counter BlockchainRetryTotal = PrometheusMetrics
        .CreateCounter("fwid_blockchain_retry_total", "Total blockchain operation retries");

    private static readonly Gauge BlockchainCircuitBreakerState = PrometheusMetrics
        .CreateGauge("fwid_blockchain_circuit_breaker_state", "Circuit breaker state (0=closed, 1=open)");

    private static readonly Gauge BlockchainConsecutiveFailures = PrometheusMetrics
        .CreateGauge("fwid_blockchain_consecutive_failures", "Number of consecutive blockchain failures");

    public ResilientBlockchainClient(
        IMameyNodeBankingClient? bankingClient,
        IIdentityRepository identityRepository,
        IOptions<ResilientBlockchainOptions> options,
        ILogger<ResilientBlockchainClient> logger)
    {
        _bankingClient = bankingClient;
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _options = options?.Value ?? new ResilientBlockchainOptions();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _logger.LogInformation(
            "Resilient blockchain client initialized. MaxRetries={MaxRetries}, CircuitBreakerThreshold={Threshold}",
            _options.MaxRetryAttempts, _options.CircuitBreakerFailureThreshold);
    }

    public bool IsCircuitBreakerOpen
    {
        get
        {
            lock (_circuitBreakerLock)
            {
                if (_circuitOpenedAt == null)
                    return false;

                // Check if recovery time has passed
                var recoveryTime = TimeSpan.FromSeconds(_options.CircuitBreakerRecoverySeconds);
                if (DateTime.UtcNow - _circuitOpenedAt > recoveryTime)
                {
                    _logger.LogInformation("Circuit breaker recovery time passed, closing circuit for half-open test");
                    return false; // Allow one request to test
                }

                return true;
            }
        }
    }

    public BlockchainHealthStatus GetHealthStatus()
    {
        lock (_circuitBreakerLock)
        {
            var isOpen = IsCircuitBreakerOpen;
            BlockchainCircuitBreakerState.Set(isOpen ? 1 : 0);
            BlockchainConsecutiveFailures.Set(_consecutiveFailures);

            return new BlockchainHealthStatus(
                IsHealthy: !isOpen && _consecutiveFailures < _options.CircuitBreakerFailureThreshold / 2,
                CircuitBreakerOpen: isOpen,
                ConsecutiveFailures: _consecutiveFailures,
                LastSuccessfulOperation: _lastSuccessfulOperation,
                LastFailedOperation: _lastFailedOperation,
                LastErrorMessage: _lastErrorMessage);
        }
    }

    public async Task<BlockchainAccountResult> CreateAccountWithRetryAsync(
        string identityId,
        string currency = "USD",
        CancellationToken cancellationToken = default)
    {
        if (_bankingClient == null)
        {
            _logger.LogDebug("MameyNode banking client not available, skipping blockchain account creation");
            return new BlockchainAccountResult(false, null, "Banking client not configured", 0, TimeSpan.Zero);
        }

        var stopwatch = Stopwatch.StartNew();
        var attemptCount = 0;
        string? lastError = null;

        // Check circuit breaker
        if (IsCircuitBreakerOpen)
        {
            _logger.LogWarning("Circuit breaker is open, skipping blockchain account creation for {IdentityId}", identityId);
            BlockchainAccountCreationTotal.WithLabels("circuit_open").Inc();
            return new BlockchainAccountResult(false, null, "Circuit breaker is open - blockchain service unavailable", 0, stopwatch.Elapsed);
        }

        while (attemptCount < _options.MaxRetryAttempts)
        {
            attemptCount++;
            
            try
            {
                _logger.LogDebug(
                    "Attempting blockchain account creation for {IdentityId}, attempt {Attempt}/{MaxAttempts}",
                    identityId, attemptCount, _options.MaxRetryAttempts);

                var accountAddress = await _bankingClient.CreateAccountAsync(identityId, currency, cancellationToken);

                if (!string.IsNullOrEmpty(accountAddress))
                {
                    stopwatch.Stop();
                    RecordSuccess();
                    
                    BlockchainAccountCreationTotal.WithLabels("success").Inc();
                    BlockchainAccountCreationDuration.Observe(stopwatch.Elapsed.TotalSeconds);

                    _logger.LogInformation(
                        "Successfully created blockchain account for {IdentityId} on attempt {Attempt}, Address: {Address}",
                        identityId, attemptCount, accountAddress);

                    return new BlockchainAccountResult(true, accountAddress, null, attemptCount, stopwatch.Elapsed);
                }

                lastError = "Account creation returned null address";
                _logger.LogWarning("Blockchain account creation returned null for {IdentityId}, attempt {Attempt}", identityId, attemptCount);
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                _logger.LogWarning(ex,
                    "Blockchain account creation failed for {IdentityId}, attempt {Attempt}/{MaxAttempts}",
                    identityId, attemptCount, _options.MaxRetryAttempts);
            }

            // Record failure and check circuit breaker
            RecordFailure(lastError);

            if (IsCircuitBreakerOpen)
            {
                _logger.LogWarning("Circuit breaker opened during retries, aborting");
                break;
            }

            // Calculate delay with exponential backoff and optional jitter
            if (attemptCount < _options.MaxRetryAttempts)
            {
                var delay = CalculateRetryDelay(attemptCount);
                BlockchainRetryTotal.Inc();

                _logger.LogDebug("Waiting {Delay}ms before retry for {IdentityId}", delay, identityId);
                await Task.Delay(delay, cancellationToken);
            }
        }

        stopwatch.Stop();
        BlockchainAccountCreationTotal.WithLabels("failed").Inc();
        BlockchainAccountCreationDuration.Observe(stopwatch.Elapsed.TotalSeconds);

        _logger.LogError(
            "Failed to create blockchain account for {IdentityId} after {Attempts} attempts. Last error: {Error}",
            identityId, attemptCount, lastError);

        return new BlockchainAccountResult(false, null, lastError, attemptCount, stopwatch.Elapsed);
    }

    public async Task<BlockchainAccountResult> RetryFailedAccountCreationAsync(
        Identity identity,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrying failed blockchain account creation for identity: {IdentityId}", identity.Id);

        var result = await CreateAccountWithRetryAsync(
            identity.Id.Value.ToString(),
            identity.Metadata.TryGetValue("BlockchainCurrency", out var currency) ? currency?.ToString() ?? "USD" : "USD",
            cancellationToken);

        if (result.Success && !string.IsNullOrEmpty(result.AccountAddress))
        {
            // Update identity metadata with successful creation
            identity.Metadata["BlockchainAccount"] = result.AccountAddress;
            identity.Metadata["BlockchainAccountCreatedAt"] = DateTime.UtcNow.ToString("O");
            identity.Metadata.Remove("BlockchainAccountCreationFailed");
            identity.Metadata.Remove("BlockchainAccountCreationError");
            identity.Metadata.Remove("BlockchainAccountCreationFailedAt");

            await _identityRepository.UpdateAsync(identity, cancellationToken);

            _logger.LogInformation(
                "Successfully recovered blockchain account for identity: {IdentityId}, Address: {Address}",
                identity.Id, result.AccountAddress);
        }
        else
        {
            // Update retry count
            var retryCount = 0;
            if (identity.Metadata.TryGetValue("BlockchainAccountRetryCount", out var count))
            {
                int.TryParse(count?.ToString(), out retryCount);
            }
            retryCount++;

            identity.Metadata["BlockchainAccountRetryCount"] = retryCount;
            identity.Metadata["BlockchainAccountLastRetryAt"] = DateTime.UtcNow.ToString("O");
            identity.Metadata["BlockchainAccountCreationError"] = result.ErrorMessage ?? "Unknown error";

            await _identityRepository.UpdateAsync(identity, cancellationToken);

            _logger.LogWarning(
                "Failed to recover blockchain account for identity: {IdentityId}, retry count: {RetryCount}",
                identity.Id, retryCount);
        }

        return result;
    }

    public async Task RecordFailedCreationAsync(
        string identityId,
        string error,
        CancellationToken cancellationToken = default)
    {
        // This is handled by IdentityService, but provided for interface compliance
        _logger.LogDebug("Recording failed blockchain creation for {IdentityId}: {Error}", identityId, error);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<string>> GetFailedAccountCreationsAsync(
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving identities with failed blockchain account creation, limit: {Limit}", limit);

        // Query identities with BlockchainAccountCreationFailed = true
        var failedIdentities = await _identityRepository.FindAsync(
            i => i.Metadata.ContainsKey("BlockchainAccountCreationFailed"),
            cancellationToken);

        // Filter by age
        var maxAge = TimeSpan.FromHours(_options.MaxFailedCreationAgeHours);
        var cutoffTime = DateTime.UtcNow - maxAge;

        var eligibleForRetry = failedIdentities
            .Where(i =>
            {
                if (i.Metadata.TryGetValue("BlockchainAccountCreationFailedAt", out var failedAt))
                {
                    if (DateTime.TryParse(failedAt?.ToString(), out var failedTime))
                    {
                        return failedTime >= cutoffTime;
                    }
                }
                return true; // Include if we can't determine age
            })
            .Take(limit)
            .Select(i => i.Id.Value.ToString())
            .ToList();

        _logger.LogDebug("Found {Count} identities eligible for blockchain account retry", eligibleForRetry.Count);

        return eligibleForRetry;
    }

    private int CalculateRetryDelay(int attemptNumber)
    {
        // Exponential backoff: delay = initial * multiplier^(attempt-1)
        var exponentialDelay = _options.InitialRetryDelayMs * Math.Pow(_options.BackoffMultiplier, attemptNumber - 1);
        var delay = (int)Math.Min(exponentialDelay, _options.MaxRetryDelayMs);

        if (_options.UseJitter)
        {
            // Add random jitter (±25%)
            var jitter = Random.Shared.Next(-delay / 4, delay / 4);
            delay = Math.Max(_options.InitialRetryDelayMs, delay + jitter);
        }

        return delay;
    }

    private void RecordSuccess()
    {
        lock (_circuitBreakerLock)
        {
            _consecutiveFailures = 0;
            _circuitOpenedAt = null;
            _lastSuccessfulOperation = DateTime.UtcNow;
            BlockchainConsecutiveFailures.Set(0);
            BlockchainCircuitBreakerState.Set(0);
        }
    }

    private void RecordFailure(string? errorMessage)
    {
        lock (_circuitBreakerLock)
        {
            _consecutiveFailures++;
            _lastFailedOperation = DateTime.UtcNow;
            _lastErrorMessage = errorMessage;
            BlockchainConsecutiveFailures.Set(_consecutiveFailures);

            if (_consecutiveFailures >= _options.CircuitBreakerFailureThreshold && _circuitOpenedAt == null)
            {
                _circuitOpenedAt = DateTime.UtcNow;
                BlockchainCircuitBreakerState.Set(1);
                _logger.LogWarning(
                    "Circuit breaker OPENED after {Failures} consecutive failures. Recovery in {Seconds}s",
                    _consecutiveFailures, _options.CircuitBreakerRecoverySeconds);
            }
        }
    }
}
