using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

internal interface ICitizenshipApplicationsUnitOfWork : IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}