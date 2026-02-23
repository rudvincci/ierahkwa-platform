using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.TravelIdentities.Core.EF;

internal interface ITravelIdentitiesUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
