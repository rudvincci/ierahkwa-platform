using Mamey.Postgres;

namespace Mamey.ServiceName.Infrastructure.EF;

internal class EntityNameUnitOfWork : PostgresUnitOfWork<EntityNameDbContext>, IEntityNameUnitOfWork
{
    public EntityNameUnitOfWork(EntityNameDbContext dbContext) : base(dbContext)
    {
    }
}
