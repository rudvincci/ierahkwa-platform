using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;

internal class CustomerLockedException : MameyException
{
    public Guid CustomerId { get; }

    public CustomerLockedException(Guid customerId)
        : base($"Customer with ID: '{customerId}' is locked.")
    {
        CustomerId = customerId;
    }
}