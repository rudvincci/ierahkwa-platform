using Mamey.Postgres;

namespace Mamey.Government.Modules.Tenant.Core.EF;

internal class TenantUnitOfWork : PostgresUnitOfWork<TenantDbContext>, ITenantUnitOfWork
{
    private readonly TenantDbContext _dbContext;

    public TenantUnitOfWork(TenantDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
