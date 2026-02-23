using System.Threading;
using System.Threading.Tasks;
using Mamey.Postgres;

namespace Mamey.FWID.Identities.Infrastructure.EF;

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
