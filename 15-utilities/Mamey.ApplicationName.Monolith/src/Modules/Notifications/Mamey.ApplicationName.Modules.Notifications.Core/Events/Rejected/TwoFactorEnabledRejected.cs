using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.Rejected;

internal record TwoFactorEnabledRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "two_factor_enabled_rejected";
}