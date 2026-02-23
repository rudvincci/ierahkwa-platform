using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record SmsTwoFactorEnableRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_sms_two_factor_enable_rejected";
}