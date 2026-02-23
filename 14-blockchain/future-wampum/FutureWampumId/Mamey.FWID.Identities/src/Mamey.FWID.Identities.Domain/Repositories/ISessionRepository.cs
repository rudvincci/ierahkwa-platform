using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Repositories;

public interface ISessionRepository
{
    Task AddAsync(Session entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default);
    
    Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session>> FindAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

