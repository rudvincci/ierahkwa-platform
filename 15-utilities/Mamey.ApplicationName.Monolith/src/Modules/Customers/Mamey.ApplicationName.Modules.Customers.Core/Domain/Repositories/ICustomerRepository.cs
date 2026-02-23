using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;

namespace Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;

internal interface ICustomerRepository
{
    Task AddAsync(Customer customer); 
    Task<IReadOnlyList<Customer>> BrowseAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsAsync(string email);
    Task<Customer> GetAsync(Guid id);
    Task UpdateAsync(Customer customer);
}
