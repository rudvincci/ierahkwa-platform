using Mamey.Portal.Auth.Domain.Entities;

namespace Mamey.Portal.Auth.Domain.Repositories;

public interface IAuthUserRepository
{
    Task<AuthUser?> GetAsync(string issuer, string subject, CancellationToken ct = default);
    Task SaveAsync(AuthUser user, CancellationToken ct = default);
}
