using Mamey.CQRS.Queries;
using Mamey.FWID.Notifications.Application.DTO;

namespace Mamey.FWID.Notifications.Application.Queries;

/// <summary>
/// Query to get notifications for an identity.
/// </summary>
internal record GetNotifications : IQuery<IEnumerable<NotificationDto>>
{
    public GetNotifications(Guid identityId)
    {
        IdentityId = identityId;
    }

    public Guid IdentityId { get; init; }
}







