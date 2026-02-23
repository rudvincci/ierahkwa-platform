using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Identity.Core.EF;

internal interface IIdentityUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
