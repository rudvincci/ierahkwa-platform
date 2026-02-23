using Mamey.Postgres;

namespace Pupitre.AIAdaptive.Infrastructure.EF;

internal class AdaptiveLearningUnitOfWork : PostgresUnitOfWork<AdaptiveLearningDbContext>, IAdaptiveLearningUnitOfWork
{
    public AdaptiveLearningUnitOfWork(AdaptiveLearningDbContext dbContext) : base(dbContext)
    {
    }
}
