using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record AuthenticatorCodeVerificationRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenicator_verification_code";
}