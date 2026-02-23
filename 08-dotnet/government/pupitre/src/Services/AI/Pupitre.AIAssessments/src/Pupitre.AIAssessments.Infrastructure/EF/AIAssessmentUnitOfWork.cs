using Mamey.Postgres;

namespace Pupitre.AIAssessments.Infrastructure.EF;

internal class AIAssessmentUnitOfWork : PostgresUnitOfWork<AIAssessmentDbContext>, IAIAssessmentUnitOfWork
{
    public AIAssessmentUnitOfWork(AIAssessmentDbContext dbContext) : base(dbContext)
    {
    }
}
