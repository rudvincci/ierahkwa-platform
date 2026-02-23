using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;

internal class CannotCompleteCustomerException : MameyException
{
    public Guid CustomerId { get; }

    public CannotCompleteCustomerException(Guid customerId)
        : base($"Customer with ID: '{customerId}' cannot be completed.")
    {
        CustomerId = customerId;
    }
}