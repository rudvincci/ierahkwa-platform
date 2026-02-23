using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;

internal class CustomerNotActiveException : MameyException
{
    public Guid CustomerId { get; }

    public CustomerNotActiveException(Guid customerId)
        : base($"Customer with ID: '{customerId}' is not active.")
    {
        CustomerId = customerId;
    }
}
