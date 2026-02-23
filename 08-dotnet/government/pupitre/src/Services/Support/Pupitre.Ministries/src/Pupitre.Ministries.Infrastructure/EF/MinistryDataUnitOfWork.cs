using Mamey.Postgres;

namespace Pupitre.Ministries.Infrastructure.EF;

internal class MinistryDataUnitOfWork : PostgresUnitOfWork<MinistryDataDbContext>, IMinistryDataUnitOfWork
{
    public MinistryDataUnitOfWork(MinistryDataDbContext dbContext) : base(dbContext)
    {
    }
}
