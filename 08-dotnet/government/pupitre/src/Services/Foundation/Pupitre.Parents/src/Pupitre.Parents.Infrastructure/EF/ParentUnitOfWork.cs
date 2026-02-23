using Mamey.Postgres;

namespace Pupitre.Parents.Infrastructure.EF;

internal class ParentUnitOfWork : PostgresUnitOfWork<ParentDbContext>, IParentUnitOfWork
{
    public ParentUnitOfWork(ParentDbContext dbContext) : base(dbContext)
    {
    }
}
