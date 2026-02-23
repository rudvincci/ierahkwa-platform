using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Composite;

/// <summary>
/// Composite repository for Session entities.
/// Currently uses PostgreSQL directly; MongoDB/Redis can be added later.
/// </summary>
internal class CompositeSessionRepository : ISessionRepository
{
    private readonly SessionPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeSessionRepository> _logger;

    public CompositeSessionRepository(
        SessionPostgresRepository postgresRepo,
        ILogger<CompositeSessionRepository> logger)
    {
        _postgresRepo = postgresRepo ?? throw new ArgumentNullException(nameof(postgresRepo));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task AddAsync(Session entity, CancellationToken cancellationToken = default)
        => _postgresRepo.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Session entity, CancellationToken cancellationToken = default)
        => _postgresRepo.UpdateAsync(entity, cancellationToken);

    public Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteAsync(id, cancellationToken);

    public Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default)
        => _postgresRepo.GetAsync(id, cancellationToken);

    public Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default)
        => _postgresRepo.ExistsAsync(id, cancellationToken);

    public Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByRefreshTokenAsync(refreshToken, cancellationToken);

    public Task<IReadOnlyList<Session>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByIdentityIdAsync(identityId, cancellationToken);

    public Task<IReadOnlyList<Session>> FindAsync(System.Linq.Expressions.Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
        => _postgresRepo.FindAsync(predicate, cancellationToken);

    public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<Session, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(predicate, cancellationToken);
}

