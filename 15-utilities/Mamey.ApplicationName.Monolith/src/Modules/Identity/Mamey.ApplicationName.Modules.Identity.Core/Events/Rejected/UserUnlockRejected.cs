using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record UserUnlockRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_unlock_rejected";
}