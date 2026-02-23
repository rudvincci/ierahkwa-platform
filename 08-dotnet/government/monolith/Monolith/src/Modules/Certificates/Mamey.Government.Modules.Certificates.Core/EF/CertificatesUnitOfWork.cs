using Mamey.Postgres;

namespace Mamey.Government.Modules.Certificates.Core.EF;

internal class CertificatesUnitOfWork : PostgresUnitOfWork<CertificatesDbContext>, ICertificatesUnitOfWork
{
    private readonly CertificatesDbContext _dbContext;

    public CertificatesUnitOfWork(CertificatesDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
