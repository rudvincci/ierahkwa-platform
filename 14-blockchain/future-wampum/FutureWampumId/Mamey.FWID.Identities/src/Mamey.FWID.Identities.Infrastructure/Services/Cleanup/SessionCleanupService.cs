using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.Services.Cleanup;

/// <summary>
/// Background service that periodically cleans up expired sessions.
/// </summary>
internal sealed class SessionCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SessionCleanupService> _logger;
    private readonly CleanupOptions _options;

    public SessionCleanupService(
        IServiceProvider serviceProvider,
        ILogger<SessionCleanupService> logger,
        IOptions<CleanupOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.SessionCleanupEnabled)
        {
            _logger.LogInformation("Session cleanup service is disabled");
            return;
        }

        // Wait for the application to fully start
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
                await CleanupExpiredSessions(stoppingToken);
                await Task.Delay(_options.SessionCleanupInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Session cleanup service is shutting down");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during session cleanup");
                if (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_options.RetryDelay, stoppingToken);
                }
            }
        }
    }

    private async Task CleanupExpiredSessions(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();

        try
        {
            _logger.LogInformation("Starting expired session cleanup");

            // Find all expired sessions
            var expiredSessions = await sessionRepository.FindAsync(
                s => s.Status == SessionStatus.Expired || s.ExpiresAt < DateTime.UtcNow,
                cancellationToken);

            var deletedCount = 0;
            foreach (var session in expiredSessions)
            {
                try
                {
                    await sessionRepository.DeleteAsync(session.Id, cancellationToken);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete expired session {SessionId}", session.Id.Value);
                }
            }

            _logger.LogInformation("Session cleanup completed. Deleted {Count} expired sessions", deletedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during session cleanup");
            throw;
        }
    }
}

/// <summary>
/// Options for cleanup services.
/// </summary>
public class CleanupOptions
{
    public bool SessionCleanupEnabled { get; set; } = true;
    public TimeSpan SessionCleanupInterval { get; set; } = TimeSpan.FromHours(1);
    public bool EmailConfirmationCleanupEnabled { get; set; } = true;
    public TimeSpan EmailConfirmationCleanupInterval { get; set; } = TimeSpan.FromHours(6);
    public bool SmsConfirmationCleanupEnabled { get; set; } = true;
    public TimeSpan SmsConfirmationCleanupInterval { get; set; } = TimeSpan.FromHours(6);
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(5);
}

