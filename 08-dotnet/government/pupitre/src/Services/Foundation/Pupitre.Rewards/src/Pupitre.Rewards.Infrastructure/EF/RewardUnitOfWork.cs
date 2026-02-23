using Mamey.Postgres;

namespace Pupitre.Rewards.Infrastructure.EF;

internal class RewardUnitOfWork : PostgresUnitOfWork<RewardDbContext>, IRewardUnitOfWork
{
    public RewardUnitOfWork(RewardDbContext dbContext) : base(dbContext)
    {
    }
}
