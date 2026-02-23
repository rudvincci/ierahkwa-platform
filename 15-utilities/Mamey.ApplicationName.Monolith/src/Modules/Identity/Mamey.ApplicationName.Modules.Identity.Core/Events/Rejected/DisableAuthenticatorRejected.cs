using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record DisableAuthenticatorRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_authenicator_disable_rejected";
}