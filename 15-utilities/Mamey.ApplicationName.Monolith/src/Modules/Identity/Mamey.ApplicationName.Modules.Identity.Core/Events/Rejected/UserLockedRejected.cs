using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record UserLockedRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_locked_rejected";
}