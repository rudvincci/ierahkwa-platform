using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record RoleAdditionRejected(Guid UserId, string RoleName, string Reason) : IRejectedEvent
{
    public string Code { get; } = "role_addition_rejected";
}