using System;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Customers.Core.Events.External;

internal record UserStateUpdated(Guid UserId, string State) : IEvent;
    
[Message("users")]
internal class UserStateUpdatedContract : Contract<UserStateUpdated>
{
    public UserStateUpdatedContract()
    {
        RequireAll();
    }
}