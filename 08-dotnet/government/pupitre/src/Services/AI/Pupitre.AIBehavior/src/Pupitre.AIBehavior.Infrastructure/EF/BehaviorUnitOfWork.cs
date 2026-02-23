using Mamey.Postgres;

namespace Pupitre.AIBehavior.Infrastructure.EF;

internal class BehaviorUnitOfWork : PostgresUnitOfWork<BehaviorDbContext>, IBehaviorUnitOfWork
{
    public BehaviorUnitOfWork(BehaviorDbContext dbContext) : base(dbContext)
    {
    }
}
