using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Bank.Modules.Customers.Infrastructure.EF.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext _context;
    private readonly DbSet<Customer> _customers;

    public CustomerRepository(CustomersDbContext context)
    {
        _context = context;
        _customers = _context.Customers;
    }

    public  Task<Customer> GetAsync(Guid id)
        => _customers.Include(x => x.Id).SingleOrDefaultAsync(x => x.Id == id);

    public  Task<Customer> GetAsync(string email)
        => _customers.Include(x => x.Email).SingleOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(Customer customer)
    {
        await _customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer user)
    {
        _customers.Update(user);
        await _context.SaveChangesAsync();
    }

    public Task<IReadOnlyList<Customer>> BrowseAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid id)
        => _customers.AnyAsync(c=> c.Id == id);
    public Task<bool> ExistsAsync(string email)
        => _customers.AnyAsync(c=> c.Email == email);
}
