using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record SignInWithAuthenticatorCodeRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; set; } = "sign_in_with_authenticator_code_rejected";
}
