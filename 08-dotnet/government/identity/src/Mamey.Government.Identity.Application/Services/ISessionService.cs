using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface ISessionService
{
    #region Session CRUD Operations
    Task<SessionDto?> GetSessionAsync(SessionId id, CancellationToken cancellationToken = default);
    Task<SessionDto> CreateSessionAsync(CreateSession command, CancellationToken cancellationToken = default);
    Task<SessionDto> RefreshSessionAsync(RefreshSession command, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(RevokeSession command, CancellationToken cancellationToken = default);
    #endregion

    #region Session Management
    Task<SessionDto?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<SessionDto?> GetSessionByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionDto>> GetUserSessionsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task RevokeExpiredSessionsAsync(CancellationToken cancellationToken = default);
    #endregion

    #region Session Validation
    Task<bool> IsSessionValidAsync(SessionId sessionId, CancellationToken cancellationToken = default);
    Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> IsAccessTokenValidAsync(string accessToken, CancellationToken cancellationToken = default);
    #endregion

    #region Session Statistics
    Task<SessionStatisticsDto> GetSessionStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
