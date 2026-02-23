using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Services.Cleanup;

/// <summary>
/// Background service that periodically cleans up expired SMS confirmations.
/// </summary>
internal sealed class SmsConfirmationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SmsConfirmationCleanupService> _logger;
    private readonly CleanupOptions _options;

    public SmsConfirmationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<SmsConfirmationCleanupService> logger,
        IOptions<CleanupOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.SmsConfirmationCleanupEnabled)
        {
            _logger.LogInformation("SMS confirmation cleanup service is disabled");
            return;
        }

        try
        {
            await Task.Delay(_options.InitialDelay, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredSmsConfirmations(stoppingToken);
                await Task.Delay(_options.SmsConfirmationCleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("SMS confirmation cleanup service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during SMS confirmation cleanup");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.RetryDelay, stoppingToken);
                }
            }
        }
    }

    private async Task CleanupExpiredSmsConfirmations(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var smsConfirmationService = scope.ServiceProvider.GetRequiredService<ISmsConfirmationService>();

        try
        {
            _logger.LogInformation("Starting expired SMS confirmation cleanup");

            var deletedCount = await smsConfirmationService.CleanupExpiredSmsConfirmationsAsync(cancellationToken);

            _logger.LogInformation("SMS confirmation cleanup completed. Deleted {Count} expired confirmations", deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SMS confirmation cleanup");
            throw;
        }
    }
}

