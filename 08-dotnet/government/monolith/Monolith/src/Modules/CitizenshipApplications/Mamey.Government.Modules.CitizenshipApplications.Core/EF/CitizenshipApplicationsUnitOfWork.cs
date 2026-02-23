using Mamey.Postgres;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

internal class CitizenshipApplicationsUnitOfWork : PostgresUnitOfWork<CitizenshipApplicationsDbContext>, ICitizenshipApplicationsUnitOfWork
{
    private readonly CitizenshipApplicationsDbContext _dbContext;

    public CitizenshipApplicationsUnitOfWork(CitizenshipApplicationsDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}