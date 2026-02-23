using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record UserUpdateRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_update_rejected";
}