using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Domain.Repositories;

internal interface IUserSessionRepository
{
    Task<UserSession?> GetAsync(string sessionId, CancellationToken cancellationToken = default);
    Task AddAsync(UserSession session, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
    Task DeleteAsync(string sessionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserSession>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
}
