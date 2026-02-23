using Mamey.Postgres;

namespace Pupitre.GLEs.Infrastructure.EF;

internal class GLEUnitOfWork : PostgresUnitOfWork<GLEDbContext>, IGLEUnitOfWork
{
    public GLEUnitOfWork(GLEDbContext dbContext) : base(dbContext)
    {
    }
}
