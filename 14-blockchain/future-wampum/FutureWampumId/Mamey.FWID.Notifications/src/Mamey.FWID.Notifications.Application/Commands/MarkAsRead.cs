using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Notifications.Application.Commands;

/// <summary>
/// Command to mark a notification as read.
/// </summary>
[Contract]
internal record MarkAsRead : ICommand
{
    public MarkAsRead(Guid notificationId, Guid identityId)
    {
        NotificationId = notificationId;
        IdentityId = identityId;
    }

    public Guid NotificationId { get; init; }
    public Guid IdentityId { get; init; }
}







