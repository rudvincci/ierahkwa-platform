using Mamey.CQRS.Commands;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Application.Commands.Integration.Notifications;

/// <summary>
/// Integration command to send a notification.
/// </summary>
[Message("notifications")]
public record SendNotificationIntegrationCommand(
    Guid IdentityId,
    string Title,
    string Description,
    string Message,
    string NotificationType,
    string? RelatedEntityType = null,
    Guid? RelatedEntityId = null) : ICommand;







