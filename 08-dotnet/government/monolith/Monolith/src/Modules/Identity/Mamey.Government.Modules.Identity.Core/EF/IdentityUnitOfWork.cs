using Mamey.Postgres;

namespace Mamey.Government.Modules.Identity.Core.EF;

internal class IdentityUnitOfWork : PostgresUnitOfWork<IdentityDbContext>, IIdentityUnitOfWork
{
    private readonly IdentityDbContext _dbContext;

    public IdentityUnitOfWork(IdentityDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
