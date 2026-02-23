using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Users;

internal record PasswordResetRequested(Guid UserId, Guid OrganizationId, string Token) : IEvent;
[Message("identity")]
internal class PasswordResetRequestedContract : Contract<PasswordResetRequested>
{
    public PasswordResetRequestedContract()
    {
        RequireAll();
    }
}