using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Certificates.Core.EF;

internal interface ICertificatesUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
