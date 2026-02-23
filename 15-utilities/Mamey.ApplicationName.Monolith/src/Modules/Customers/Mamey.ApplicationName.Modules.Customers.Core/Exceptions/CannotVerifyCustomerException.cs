using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;

internal class CannotVerifyCustomerException : MameyException
{
    public Guid CustomerId { get; }

    public CannotVerifyCustomerException(Guid customerId)
        : base($"Customer with ID: '{customerId}' cannot be verified.")
    {
        CustomerId = customerId;
    }
}
