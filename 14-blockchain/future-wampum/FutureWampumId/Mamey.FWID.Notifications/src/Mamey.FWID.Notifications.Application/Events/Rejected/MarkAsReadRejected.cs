using Mamey.CQRS.Events;

namespace Mamey.FWID.Notifications.Application.Events.Rejected;

/// <summary>
/// Rejected event for MarkAsRead command.
/// </summary>
internal record MarkAsReadRejected(Guid NotificationId, Guid IdentityId, string Reason, string Code) : IRejectedEvent;







