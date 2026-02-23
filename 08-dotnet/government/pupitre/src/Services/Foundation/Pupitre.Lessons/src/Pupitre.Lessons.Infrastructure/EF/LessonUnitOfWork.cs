using Mamey.Postgres;

namespace Pupitre.Lessons.Infrastructure.EF;

internal class LessonUnitOfWork : PostgresUnitOfWork<LessonDbContext>, ILessonUnitOfWork
{
    public LessonUnitOfWork(LessonDbContext dbContext) : base(dbContext)
    {
    }
}
