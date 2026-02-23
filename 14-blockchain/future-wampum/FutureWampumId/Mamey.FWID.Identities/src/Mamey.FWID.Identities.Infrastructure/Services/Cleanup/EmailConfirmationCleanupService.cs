using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Services.Cleanup;

/// <summary>
/// Background service that periodically cleans up expired email confirmations.
/// </summary>
internal sealed class EmailConfirmationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EmailConfirmationCleanupService> _logger;
    private readonly CleanupOptions _options;

    public EmailConfirmationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<EmailConfirmationCleanupService> logger,
        IOptions<CleanupOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EmailConfirmationCleanupEnabled)
        {
            _logger.LogInformation("Email confirmation cleanup service is disabled");
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
                await CleanupExpiredEmailConfirmations(stoppingToken);
                await Task.Delay(_options.EmailConfirmationCleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Email confirmation cleanup service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email confirmation cleanup");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.RetryDelay, stoppingToken);
                }
            }
        }
    }

    private async Task CleanupExpiredEmailConfirmations(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var emailConfirmationService = scope.ServiceProvider.GetRequiredService<IEmailConfirmationService>();

        try
        {
            _logger.LogInformation("Starting expired email confirmation cleanup");

            var deletedCount = await emailConfirmationService.CleanupExpiredEmailConfirmationsAsync(cancellationToken);

            _logger.LogInformation("Email confirmation cleanup completed. Deleted {Count} expired confirmations", deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation cleanup");
            throw;
        }
    }
}

