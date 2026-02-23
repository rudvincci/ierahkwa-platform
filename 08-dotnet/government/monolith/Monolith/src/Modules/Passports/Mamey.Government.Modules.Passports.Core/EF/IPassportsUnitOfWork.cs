using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Passports.Core.EF;

internal interface IPassportsUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
