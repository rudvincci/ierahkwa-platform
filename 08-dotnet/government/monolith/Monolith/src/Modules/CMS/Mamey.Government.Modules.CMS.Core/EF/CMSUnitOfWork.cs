using Mamey.Postgres;

namespace Mamey.Government.Modules.CMS.Core.EF;

internal class CMSUnitOfWork : PostgresUnitOfWork<CMSDbContext>, ICMSUnitOfWork
{
    private readonly CMSDbContext _dbContext;

    public CMSUnitOfWork(CMSDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
