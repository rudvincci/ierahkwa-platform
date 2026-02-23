using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Blockchain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that retries failed blockchain account creations.
/// Runs periodically to ensure eventual consistency with the blockchain.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 326-378 (V.2 Biometric Identity Registration)
/// </summary>
internal sealed class BlockchainAccountSyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ResilientBlockchainOptions _options;
    private readonly ILogger<BlockchainAccountSyncService> _logger;

    // Prometheus metrics
    private static readonly Counter SyncCyclesTotal = PrometheusMetrics
        .CreateCounter("fwid_blockchain_sync_cycles_total", "Total background sync cycles",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Counter SyncRetriesTotal = PrometheusMetrics
        .CreateCounter("fwid_blockchain_sync_retries_total", "Total identities retried during sync",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Gauge SyncPendingIdentities = PrometheusMetrics
        .CreateGauge("fwid_blockchain_sync_pending_identities", "Number of identities pending blockchain sync");

    private static readonly Histogram SyncCycleDuration = PrometheusMetrics
        .CreateHistogram("fwid_blockchain_sync_cycle_duration_seconds", "Duration of sync cycles",
            new HistogramConfiguration { Buckets = Histogram.ExponentialBuckets(1, 2, 10) });

    public BlockchainAccountSyncService(
        IServiceScopeFactory scopeFactory,
        IOptions<ResilientBlockchainOptions> options,
        ILogger<BlockchainAccountSyncService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options?.Value ?? new ResilientBlockchainOptions();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnableBackgroundSync)
        {
            _logger.LogInformation("Blockchain account sync service is disabled");
            return;
        }

        _logger.LogInformation(
            "Blockchain account sync service started. Interval: {Interval}s, MaxPerCycle: {MaxPerCycle}",
            _options.BackgroundSyncIntervalSeconds, _options.MaxFailedCreationsPerSync);

        // Wait a bit before starting to allow service to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunSyncCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Blockchain account sync service shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in blockchain account sync cycle");
                SyncCyclesTotal.WithLabels("error").Inc();
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.BackgroundSyncIntervalSeconds), stoppingToken);
        }
    }

    private async Task RunSyncCycleAsync(CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogDebug("Starting blockchain account sync cycle");

        using var scope = _scopeFactory.CreateScope();
        var resilientClient = scope.ServiceProvider.GetRequiredService<IResilientBlockchainClient>();
        var identityRepository = scope.ServiceProvider.GetRequiredService<IIdentityRepository>();

        // Check if circuit breaker is open - skip sync if so
        if (resilientClient.IsCircuitBreakerOpen)
        {
            _logger.LogWarning("Skipping sync cycle - circuit breaker is open");
            SyncCyclesTotal.WithLabels("skipped_circuit_open").Inc();
            return;
        }

        // Get identities with failed blockchain account creation
        var failedIdentityIds = await resilientClient.GetFailedAccountCreationsAsync(
            _options.MaxFailedCreationsPerSync, cancellationToken);

        SyncPendingIdentities.Set(failedIdentityIds.Count);

        if (failedIdentityIds.Count == 0)
        {
            _logger.LogDebug("No identities pending blockchain account sync");
            SyncCyclesTotal.WithLabels("no_work").Inc();
            return;
        }

        _logger.LogInformation("Found {Count} identities pending blockchain account sync", failedIdentityIds.Count);

        var successCount = 0;
        var failureCount = 0;

        foreach (var identityIdStr in failedIdentityIds)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Check circuit breaker on each iteration
            if (resilientClient.IsCircuitBreakerOpen)
            {
                _logger.LogWarning("Circuit breaker opened during sync, aborting remaining retries");
                break;
            }

            try
            {
                if (!Guid.TryParse(identityIdStr, out var identityGuid))
                {
                    _logger.LogWarning("Invalid identity ID format: {IdentityId}", identityIdStr);
                    continue;
                }

                var identity = await identityRepository.GetAsync(
                    new Domain.Entities.IdentityId(identityGuid), cancellationToken);

                if (identity == null)
                {
                    _logger.LogWarning("Identity not found for sync: {IdentityId}", identityIdStr);
                    continue;
                }

                // Check if already has blockchain account (might have been created in the meantime)
                if (identity.Metadata.ContainsKey("BlockchainAccount") && 
                    !identity.Metadata.ContainsKey("BlockchainAccountCreationFailed"))
                {
                    _logger.LogDebug("Identity {IdentityId} already has blockchain account, skipping", identityIdStr);
                    continue;
                }

                var result = await resilientClient.RetryFailedAccountCreationAsync(identity, cancellationToken);

                if (result.Success)
                {
                    successCount++;
                    SyncRetriesTotal.WithLabels("success").Inc();
                    _logger.LogInformation(
                        "Successfully synced blockchain account for {IdentityId}, Address: {Address}",
                        identityIdStr, result.AccountAddress);
                }
                else
                {
                    failureCount++;
                    SyncRetriesTotal.WithLabels("failed").Inc();
                    _logger.LogWarning(
                        "Failed to sync blockchain account for {IdentityId}: {Error}",
                        identityIdStr, result.ErrorMessage);
                }

                // Small delay between retries to avoid overwhelming the blockchain service
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            }
            catch (Exception ex)
            {
                failureCount++;
                SyncRetriesTotal.WithLabels("error").Inc();
                _logger.LogError(ex, "Error syncing blockchain account for {IdentityId}", identityIdStr);
            }
        }

        var duration = DateTime.UtcNow - startTime;
        SyncCycleDuration.Observe(duration.TotalSeconds);
        SyncCyclesTotal.WithLabels("completed").Inc();

        _logger.LogInformation(
            "Blockchain account sync cycle completed. Duration: {Duration}ms, Success: {Success}, Failed: {Failed}",
            duration.TotalMilliseconds, successCount, failureCount);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Blockchain account sync service stopping");
        await base.StopAsync(cancellationToken);
    }
}
