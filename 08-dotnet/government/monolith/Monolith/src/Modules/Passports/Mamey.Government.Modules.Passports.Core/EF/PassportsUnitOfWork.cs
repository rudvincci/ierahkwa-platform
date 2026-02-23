using Mamey.Postgres;

namespace Mamey.Government.Modules.Passports.Core.EF;

internal class PassportsUnitOfWork : PostgresUnitOfWork<PassportsDbContext>, IPassportsUnitOfWork
{
    private readonly PassportsDbContext _dbContext;

    public PassportsUnitOfWork(PassportsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
