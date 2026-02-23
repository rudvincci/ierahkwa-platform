using Mamey.Postgres;

namespace Pupitre.Assessments.Infrastructure.EF;

internal class AssessmentUnitOfWork : PostgresUnitOfWork<AssessmentDbContext>, IAssessmentUnitOfWork
{
    public AssessmentUnitOfWork(AssessmentDbContext dbContext) : base(dbContext)
    {
    }
}
