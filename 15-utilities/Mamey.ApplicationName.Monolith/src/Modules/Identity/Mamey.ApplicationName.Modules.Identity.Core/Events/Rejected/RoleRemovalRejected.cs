using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record RoleRemovalRejected(Guid UserId, string RoleName, string Reason) : IRejectedEvent
{
    public string Code { get; } = "role_removal_rejected";
}