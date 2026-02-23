using Mamey.Postgres;

namespace Mamey.ApplicationName.Modules.Products.Core.EF;

internal class BankingProductUnitOfWork : PostgresUnitOfWork<BankingProductDbContext>
{
    public BankingProductUnitOfWork(BankingProductDbContext dbContext) : base(dbContext)
    {
    }
}