using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.CMS.Core.EF;

internal interface ICMSUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
