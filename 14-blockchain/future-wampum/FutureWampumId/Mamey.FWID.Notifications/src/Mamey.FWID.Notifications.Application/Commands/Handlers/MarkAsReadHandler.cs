using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Commands;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Exceptions;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Commands.Handlers;

/// <summary>
/// Handler for MarkAsRead command.
/// </summary>
internal sealed class MarkAsReadHandler : ICommandHandler<MarkAsRead>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<MarkAsReadHandler> _logger;

    public MarkAsReadHandler(
        INotificationRepository notificationRepository,
        ILogger<MarkAsReadHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(MarkAsRead command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling MarkAsRead command: NotificationId={NotificationId}, IdentityId={IdentityId}",
            command.NotificationId, command.IdentityId);

        try
        {
            var identityId = new IdentityId(command.IdentityId);
            var notificationId = new NotificationId(command.NotificationId, identityId);

            // Get notification
            var notification = await _notificationRepository.GetAsync(notificationId, cancellationToken);
            if (notification == null)
            {
                _logger.LogWarning("Notification not found: NotificationId={NotificationId}", command.NotificationId);
                throw new InvalidOperationException($"Notification {command.NotificationId} not found.");
            }

            // Verify identity matches
            if (notification.IdentityId.Value != command.IdentityId)
            {
                _logger.LogWarning("Identity mismatch: Notification IdentityId={NotificationIdentityId}, Command IdentityId={CommandIdentityId}",
                    notification.IdentityId.Value, command.IdentityId);
                throw new UnauthorizedAccessException("Notification does not belong to the specified identity.");
            }

            // Mark as read
            notification.MarkAsRead();

            // Update notification
            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation("Successfully marked notification as read: NotificationId={NotificationId}",
                command.NotificationId);
        }
        catch (NotificationAlreadyReadException)
        {
            _logger.LogWarning("Notification already read: NotificationId={NotificationId}", command.NotificationId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MarkAsRead command: NotificationId={NotificationId}", command.NotificationId);
            throw;
        }
    }
}







