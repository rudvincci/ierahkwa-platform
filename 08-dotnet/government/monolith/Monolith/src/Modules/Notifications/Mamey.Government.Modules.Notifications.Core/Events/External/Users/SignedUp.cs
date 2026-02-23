using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Events.External.Users;

internal record SignedUp(Guid UserId, string Email, string Role, Name Name, string ConfirmUrl) : IEvent;

[Message("identity")]
internal class SignedUpContract : Contract<SignedUp>
{
    public SignedUpContract()
    {
        RequireAll();
    }
}