using Mamey.Postgres;

namespace Pupitre.Progress.Infrastructure.EF;

internal class LearningProgressUnitOfWork : PostgresUnitOfWork<LearningProgressDbContext>, ILearningProgressUnitOfWork
{
    public LearningProgressUnitOfWork(LearningProgressDbContext dbContext) : base(dbContext)
    {
    }
}
