using System;
using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Customers.Core.Exceptions;
internal class InvalidCustomerEmailException : MameyException
{
    public Guid CustomerId { get; }

    public InvalidCustomerEmailException(Guid customerId)
        : base($"Customer with ID: '{customerId}' has invalid email.")
    {
        CustomerId = customerId;
    }
}
