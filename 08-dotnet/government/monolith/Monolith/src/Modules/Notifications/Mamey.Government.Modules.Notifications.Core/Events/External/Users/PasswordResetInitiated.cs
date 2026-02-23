using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.Government.Modules.Notifications.Core.Events.External.Users;

internal record PasswordResetInitiated(string Email, string ResetUrl) : IEvent;

[Message("identity")]
internal class PasswordResetInitiatedContract : Contract<PasswordResetInitiated>
{
    public PasswordResetInitiatedContract()
    {
        RequireAll();
    }
}