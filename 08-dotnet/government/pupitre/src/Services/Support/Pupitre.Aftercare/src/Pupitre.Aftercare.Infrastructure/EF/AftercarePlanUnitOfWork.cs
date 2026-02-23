using Mamey.Postgres;

namespace Pupitre.Aftercare.Infrastructure.EF;

internal class AftercarePlanUnitOfWork : PostgresUnitOfWork<AftercarePlanDbContext>, IAftercarePlanUnitOfWork
{
    public AftercarePlanUnitOfWork(AftercarePlanDbContext dbContext) : base(dbContext)
    {
    }
}
