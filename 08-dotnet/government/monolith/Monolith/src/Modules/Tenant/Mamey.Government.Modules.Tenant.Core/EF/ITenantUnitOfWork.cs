using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Tenant.Core.EF;

internal interface ITenantUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
