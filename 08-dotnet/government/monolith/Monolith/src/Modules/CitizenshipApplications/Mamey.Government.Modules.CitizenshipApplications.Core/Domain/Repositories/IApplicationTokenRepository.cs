using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;

internal interface IApplicationTokenRepository
{
    Task<ApplicationToken?> GetByTokenHashAsync(string tokenHash, string email, CancellationToken cancellationToken = default);
    Task AddAsync(ApplicationToken token, CancellationToken cancellationToken = default);
    Task MarkAsUsedAsync(Guid tokenId, CancellationToken cancellationToken = default);
    Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
}
