using System.Collections.Immutable;
using System.Linq.Expressions;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using DomainEntities = Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for Session entities.
/// </summary>
internal class SessionPostgresRepository : ISessionRepository
{
    private readonly IdentityDbContext _dbContext;

    public SessionPostgresRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Session entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Sessions.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Session entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Sessions.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Sessions
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.Sessions.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.RefreshToken == refreshToken)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.IdentityId == identityId)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session>> FindAsync(Expression<Func<Session, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(predicate)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<int> CountAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbContext.Sessions.CountAsync(cancellationToken);
        
        return await _dbContext.Sessions.CountAsync(predicate, cancellationToken);
    }
}

