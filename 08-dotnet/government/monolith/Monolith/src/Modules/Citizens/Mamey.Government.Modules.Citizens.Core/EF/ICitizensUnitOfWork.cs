using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Citizens.Core.EF;

internal interface ICitizensUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
