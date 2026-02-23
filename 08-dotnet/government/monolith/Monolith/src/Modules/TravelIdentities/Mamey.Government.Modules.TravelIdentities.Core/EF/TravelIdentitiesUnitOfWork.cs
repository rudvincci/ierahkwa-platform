using Mamey.Postgres;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF;

internal class TravelIdentitiesUnitOfWork : PostgresUnitOfWork<TravelIdentitiesDbContext>, ITravelIdentitiesUnitOfWork
{
    private readonly TravelIdentitiesDbContext _dbContext;

    public TravelIdentitiesUnitOfWork(TravelIdentitiesDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
