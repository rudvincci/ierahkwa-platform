using Mamey.Postgres;

namespace Mamey.Bank.Modules.Customers.Infrastructure.EF;

internal class CustomersUnitOfWork : PostgresUnitOfWork<CustomersDbContext>
{
    public CustomersUnitOfWork(CustomersDbContext dbContext) : base(dbContext)
    {
    }
}
