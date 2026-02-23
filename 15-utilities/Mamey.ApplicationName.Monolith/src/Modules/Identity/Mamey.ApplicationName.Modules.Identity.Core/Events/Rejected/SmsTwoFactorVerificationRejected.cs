using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record SmsTwoFactorVerificationRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenicator_two_factor_rejected";
}