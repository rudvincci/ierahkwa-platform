using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Notifications.Core.Events.Rejected;

internal record EmailConfirmationSendRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "email_confirmation_send_rejected";
}