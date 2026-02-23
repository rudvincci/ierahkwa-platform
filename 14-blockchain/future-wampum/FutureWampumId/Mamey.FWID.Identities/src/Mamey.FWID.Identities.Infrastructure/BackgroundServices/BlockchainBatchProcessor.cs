using Mamey.FWID.Identities.Infrastructure.Blockchain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using PrometheusMetrics = Prometheus.Metrics;

namespace Mamey.FWID.Identities.Infrastructure.BackgroundServices;

/// <summary>
/// Background service that processes batched blockchain events.
/// Runs continuously, collecting events and sending them in optimized batches.
/// 
/// TDD Reference: Architecture Analysis Lines 541-546
/// BDD Reference: Lines 152-170 (II.2 KPIs - System metrics)
/// </summary>
internal sealed class BlockchainBatchProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BatchLedgerOptions _options;
    private readonly ILogger<BlockchainBatchProcessor> _logger;

    // Prometheus metrics
    private static readonly Counter ProcessingCyclesCounter = PrometheusMetrics
        .CreateCounter("blockchain_batch_processing_cycles_total", "Total batch processing cycles",
            new CounterConfiguration { LabelNames = new[] { "status" } });

    private static readonly Gauge ProcessingActiveGauge = PrometheusMetrics
        .CreateGauge("blockchain_batch_processing_active", "Whether batch processing is active");

    private static readonly Summary EventsProcessedSummary = PrometheusMetrics
        .CreateSummary("blockchain_batch_events_processed_per_cycle", "Events processed per cycle");

    public BlockchainBatchProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<BatchLedgerOptions> options,
        ILogger<BlockchainBatchProcessor> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Blockchain batch processing is disabled");
            return;
        }

        _logger.LogInformation(
            "Starting blockchain batch processor with BatchSize={BatchSize}, BatchInterval={BatchInterval}s",
            _options.BatchSize, _options.BatchInterval.TotalSeconds);

        ProcessingActiveGauge.Set(1);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchCycleAsync(stoppingToken);
                    ProcessingCyclesCounter.WithLabels("success").Inc();
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Batch processor stopping due to cancellation");
                    break;
                }
                catch (Exception ex)
                {
                    ProcessingCyclesCounter.WithLabels("error").Inc();
                    _logger.LogError(ex, "Error in batch processing cycle");
                    
                    // Wait a bit before retrying to avoid tight error loops
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }
        finally
        {
            ProcessingActiveGauge.Set(0);
            _logger.LogInformation("Blockchain batch processor stopped");
        }
    }

    private async Task ProcessBatchCycleAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var batchClient = scope.ServiceProvider.GetService<IBatchLedgerClient>();

        if (batchClient is not BatchLedgerClient client)
        {
            _logger.LogDebug("BatchLedgerClient not available, skipping cycle");
            await Task.Delay(_options.BatchInterval, stoppingToken);
            return;
        }

        var eventsProcessed = await client.ProcessBatchAsync(stoppingToken);

        if (eventsProcessed > 0)
        {
            EventsProcessedSummary.Observe(eventsProcessed);
            _logger.LogDebug("Processed {Count} events in batch cycle", eventsProcessed);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping blockchain batch processor, flushing remaining events...");

        // Flush any remaining events before stopping
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var batchClient = scope.ServiceProvider.GetService<IBatchLedgerClient>();

            if (batchClient != null)
            {
                var flushed = await batchClient.FlushAsync(cancellationToken);
                _logger.LogInformation("Flushed {Count} events during shutdown", flushed);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flushing events during shutdown");
        }

        await base.StopAsync(cancellationToken);
    }
}
