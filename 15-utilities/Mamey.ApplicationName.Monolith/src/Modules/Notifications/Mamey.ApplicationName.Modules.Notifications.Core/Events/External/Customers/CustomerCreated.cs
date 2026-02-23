
using Mamey.MicroMonolith.Abstractions.Contracts;
using Mamey.CQRS.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Customers;

public record CustomerCreated(Guid CustomerId) : IEvent;

[Message("customers")]
public class CustomerCreatedContract : Contract<CustomerCreated>
{
    public CustomerCreatedContract()
    {
        RequireAll();
    }
}