using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Commands;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Commands.Handlers;

/// <summary>
/// Handler for AddNotification command.
/// </summary>
internal sealed class AddNotificationHandler : ICommandHandler<AddNotification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AddNotificationHandler> _logger;

    public AddNotificationHandler(
        INotificationRepository notificationRepository,
        INotificationService notificationService,
        ILogger<AddNotificationHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(AddNotification command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling AddNotification command: Id={Id}, IdentityId={IdentityId}, Title={Title}",
            command.Id, command.IdentityId, command.Title);

        try
        {
            // Parse notification type
            if (!Enum.TryParse<NotificationType>(command.NotificationType, ignoreCase: true, out var notificationType))
            {
                _logger.LogWarning("Invalid notification type: {Type}, defaulting to InApp", command.NotificationType);
                notificationType = NotificationType.InApp;
            }

            // Create notification
            var identityId = new IdentityId(command.IdentityId);
            var notification = Notification.Create(
                identityId,
                command.Title,
                command.Description,
                command.Message,
                notificationType);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation("Successfully created and sent notification: NotificationId={NotificationId}",
                notification.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling AddNotification command: Id={Id}", command.Id);
            throw;
        }
    }
}







