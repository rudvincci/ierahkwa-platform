using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;

internal record UserCreationRejected(string Reason) : IRejectedEvent
{
    public string Code { get; } = "user_creation_rejected";
}