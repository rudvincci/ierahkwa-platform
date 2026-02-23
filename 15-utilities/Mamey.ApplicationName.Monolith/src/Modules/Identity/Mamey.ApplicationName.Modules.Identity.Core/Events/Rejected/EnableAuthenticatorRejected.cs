using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record EnableAuthenticatorRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenticator_enable_rejected";
}