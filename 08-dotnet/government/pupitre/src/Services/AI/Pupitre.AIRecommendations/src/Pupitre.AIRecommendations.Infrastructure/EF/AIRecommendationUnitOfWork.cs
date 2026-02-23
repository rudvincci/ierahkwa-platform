using Mamey.Postgres;

namespace Pupitre.AIRecommendations.Infrastructure.EF;

internal class AIRecommendationUnitOfWork : PostgresUnitOfWork<AIRecommendationDbContext>, IAIRecommendationUnitOfWork
{
    public AIRecommendationUnitOfWork(AIRecommendationDbContext dbContext) : base(dbContext)
    {
    }
}
