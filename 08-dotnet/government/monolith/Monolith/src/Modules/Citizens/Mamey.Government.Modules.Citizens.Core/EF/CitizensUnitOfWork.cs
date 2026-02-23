using Mamey.Postgres;

namespace Mamey.Government.Modules.Citizens.Core.EF;

internal class CitizensUnitOfWork : PostgresUnitOfWork<CitizensDbContext>, ICitizensUnitOfWork
{
    private readonly CitizensDbContext _dbContext;

    public CitizensUnitOfWork(CitizensDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
