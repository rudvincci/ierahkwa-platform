using Mamey.Postgres;

namespace Pupitre.AITutors.Infrastructure.EF;

internal class TutorUnitOfWork : PostgresUnitOfWork<TutorDbContext>, ITutorUnitOfWork
{
    public TutorUnitOfWork(TutorDbContext dbContext) : base(dbContext)
    {
    }
}
