using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class CustomerNotFoundException : MameyException
{
    public Guid CustomerId { get; }

    public CustomerNotFoundException(Guid customerId)
        : base($"Customer with ID: '{customerId}' was not found.")
    {
        CustomerId = customerId;
    }
}
