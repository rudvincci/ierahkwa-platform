using System;
using Mamey.ApplicationName.Modules.Customers.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Customers.Core.Queries;

internal class GetCustomer : IQuery<CustomerDetailsDto?>
{
    public Guid CustomerId { get; set; }
}