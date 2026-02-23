using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record EnableTwoFactorRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_two_factor_enable_rejected";
}