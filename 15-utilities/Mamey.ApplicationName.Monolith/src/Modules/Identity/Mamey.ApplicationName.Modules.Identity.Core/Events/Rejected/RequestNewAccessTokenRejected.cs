using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record RequestNewAccessTokenRejected(Guid UserId, string Reason) : IRejectedEvent
{
    public string Code { get; } = "request_new_access_token";
}