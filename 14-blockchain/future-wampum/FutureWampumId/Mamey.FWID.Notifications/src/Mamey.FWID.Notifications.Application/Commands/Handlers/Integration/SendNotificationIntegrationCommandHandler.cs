using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Commands.Integration.Notifications;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Commands.Handlers.Integration;

/// <summary>
/// Handler for SendNotificationIntegrationCommand.
/// </summary>
internal sealed class SendNotificationIntegrationCommandHandler : ICommandHandler<SendNotificationIntegrationCommand>
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<SendNotificationIntegrationCommandHandler> _logger;

    public SendNotificationIntegrationCommandHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<SendNotificationIntegrationCommandHandler> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(SendNotificationIntegrationCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received SendNotificationIntegrationCommand: IdentityId={IdentityId}, Title={Title}, Type={Type}",
            command.IdentityId, command.Title, command.NotificationType);

        try
        {
            // Parse notification type
            if (!Enum.TryParse<NotificationType>(command.NotificationType, ignoreCase: true, out var notificationType))
            {
                _logger.LogWarning("Invalid notification type: {Type}", command.NotificationType);
                notificationType = NotificationType.InApp; // Default to InApp
            }

            // Create notification
            var identityId = new IdentityId(command.IdentityId);
            var notification = Notification.Create(
                identityId,
                command.Title,
                command.Description,
                command.Message,
                notificationType,
                command.RelatedEntityType,
                command.RelatedEntityId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send notification based on type
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed SendNotificationIntegrationCommand and sent notification for IdentityId: {IdentityId}, NotificationId: {NotificationId}",
                command.IdentityId, notification.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling SendNotificationIntegrationCommand for IdentityId: {IdentityId}", command.IdentityId);
            throw;
        }
    }
}

