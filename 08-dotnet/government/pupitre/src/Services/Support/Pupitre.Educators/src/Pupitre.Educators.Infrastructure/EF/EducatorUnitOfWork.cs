using Mamey.Postgres;

namespace Pupitre.Educators.Infrastructure.EF;

internal class EducatorUnitOfWork : PostgresUnitOfWork<EducatorDbContext>, IEducatorUnitOfWork
{
    public EducatorUnitOfWork(EducatorDbContext dbContext) : base(dbContext)
    {
    }
}
