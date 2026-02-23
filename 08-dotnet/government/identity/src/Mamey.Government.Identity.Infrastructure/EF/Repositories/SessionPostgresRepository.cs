using System.Collections.Immutable;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.SQL.Repositories;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Government.Identity.Infrastructure.EF.Repositories;

internal class SessionPostgresRepository : EFRepository<Session, SessionId>, ISessionRepository, IDisposable
{
    private readonly UserDbContext _dbContext;

    public SessionPostgresRepository(UserDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Session?>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions.ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Session? session, CancellationToken cancellationToken = default)
    {
        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Session? session, CancellationToken cancellationToken = default)
    {
        _dbContext.Sessions.Update(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        var session = await _dbContext.Sessions
            .SingleAsync(s => s.Id.Value == id.Value, cancellationToken);
        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Session-specific queries
    public async Task<Session?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.AccessToken == accessToken)
            .SingleAsync(cancellationToken);
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .Where(s => s.RefreshToken == refreshToken)
            .SingleAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Session?>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.Status == status)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.Status == SessionStatus.Active && s.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.ExpiresAt <= before)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.IpAddress == ipAddress)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbContext.Sessions
            .Where(s => s.UserAgent == userAgent)
            .ToListAsync(cancellationToken);
        return ImmutableList.CreateRange(sessions);
    }

    public async Task<bool> AccessTokenExistsAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .AnyAsync(s => s.AccessToken == accessToken, cancellationToken);
    }

    public async Task<bool> RefreshTokenExistsAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .AnyAsync(s => s.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<int> GetActiveSessionCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .CountAsync(s => s.UserId.Value == userId.Value && 
                           s.Status == SessionStatus.Active && 
                           s.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        var expiredSessions = await _dbContext.Sessions
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        _dbContext.Sessions.RemoveRange(expiredSessions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSessionsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var userSessions = await _dbContext.Sessions
            .Where(s => s.UserId.Value == userId.Value)
            .ToListAsync(cancellationToken);
        
        _dbContext.Sessions.RemoveRange(userSessions);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Statistics and counting methods
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions.CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .CountAsync(s => s.Status == SessionStatus.Active && s.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> CountExpiredAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .CountAsync(s => s.ExpiresAt <= DateTime.UtcNow, cancellationToken);
    }

    public async Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .CountAsync(s => s.ExpiresAt <= before, cancellationToken);
    }

    public async Task<int> CountRevokedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Sessions
            .CountAsync(s => s.Status == SessionStatus.Revoked, cancellationToken);
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
