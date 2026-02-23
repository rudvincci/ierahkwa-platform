using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Customers.Core.Events.External;

internal record SignedUp(Guid UserId, string Email, string Role) : IEvent;
    
[Message("identity")]
internal class SignedUpContract : Contract<SignedUp>
{
    public SignedUpContract()
    {
        RequireAll();
    }
}
