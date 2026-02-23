using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class InvalidCustomerNameException : MameyException
{
    public Guid CustomerId { get; }

    public InvalidCustomerNameException(Guid customerId)
        : base($"Customer with ID: '{customerId}' has invalid name.")
    {
        CustomerId = customerId;
    }
}
