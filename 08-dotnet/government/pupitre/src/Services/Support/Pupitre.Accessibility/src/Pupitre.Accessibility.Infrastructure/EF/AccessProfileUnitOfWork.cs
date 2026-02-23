using Mamey.Postgres;

namespace Pupitre.Accessibility.Infrastructure.EF;

internal class AccessProfileUnitOfWork : PostgresUnitOfWork<AccessProfileDbContext>, IAccessProfileUnitOfWork
{
    public AccessProfileUnitOfWork(AccessProfileDbContext dbContext) : base(dbContext)
    {
    }
}
