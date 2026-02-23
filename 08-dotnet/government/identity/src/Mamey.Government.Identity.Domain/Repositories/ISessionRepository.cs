using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface ISessionRepository
{
    Task AddAsync(Session? session, CancellationToken cancellationToken = default);
    Task UpdateAsync(Session? session, CancellationToken cancellationToken = default);
    Task DeleteAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Session?> GetAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SessionId id, CancellationToken cancellationToken = default);
    
    // Session-specific queries
    Task<Session?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetByStatusAsync(SessionStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetSessionsByIpAddressAsync(string ipAddress, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Session?>> GetSessionsByUserAgentAsync(string userAgent, CancellationToken cancellationToken = default);
    Task<bool> AccessTokenExistsAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<bool> RefreshTokenExistsAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<int> GetActiveSessionCountAsync(UserId userId, CancellationToken cancellationToken = default);
    Task DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task DeleteSessionsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CountExpiredAsync(CancellationToken cancellationToken = default);
    Task<int> CountExpiredAsync(DateTime before, CancellationToken cancellationToken = default);
    Task<int> CountRevokedAsync(CancellationToken cancellationToken = default);
    
    // Additional query methods
    Task<IReadOnlyList<Session?>> GetExpiredSessionsAsync(DateTime before, CancellationToken cancellationToken = default);
    
    // Connection methods
    Task<bool> CanConnectAsync(CancellationToken cancellationToken = default);
}
