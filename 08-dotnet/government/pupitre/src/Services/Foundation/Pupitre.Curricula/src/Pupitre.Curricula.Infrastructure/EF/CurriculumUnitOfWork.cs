using Mamey.Postgres;

namespace Pupitre.Curricula.Infrastructure.EF;

internal class CurriculumUnitOfWork : PostgresUnitOfWork<CurriculumDbContext>, ICurriculumUnitOfWork
{
    public CurriculumUnitOfWork(CurriculumDbContext dbContext) : base(dbContext)
    {
    }
}
