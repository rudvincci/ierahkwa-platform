using Mamey.Postgres;

namespace Pupitre.Analytics.Infrastructure.EF;

internal class AnalyticUnitOfWork : PostgresUnitOfWork<AnalyticDbContext>, IAnalyticUnitOfWork
{
    public AnalyticUnitOfWork(AnalyticDbContext dbContext) : base(dbContext)
    {
    }
}
