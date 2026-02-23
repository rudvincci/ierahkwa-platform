using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Events;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Time;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Application.Services;

internal sealed class SessionService : ISessionService
{
    #region Read-only Fields

    private readonly ILogger<SessionService> _logger;
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventProcessor _eventProcessor;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    #endregion

    public SessionService(
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        IClock clock,
        IMessageBroker messageBroker,
        IEventProcessor eventProcessor,
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        ILogger<SessionService> logger)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _eventProcessor = eventProcessor;
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _logger = logger;
    }

    #region Session CRUD Operations

    public async Task<SessionDto?> GetSessionAsync(SessionId id, CancellationToken cancellationToken = default)
    {
        var query = new Mamey.Government.Identity.Contracts.Queries.GetSession(id);
        return await _queryDispatcher.QueryAsync<Mamey.Government.Identity.Contracts.Queries.GetSession, Mamey.Government.Identity.Contracts.DTO.SessionDto>(query, cancellationToken);
    }

    public async Task<SessionDto> CreateSessionAsync(CreateSession command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating session for user: {UserId}", command.UserId);

        var user = await _userRepository.GetAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(command.UserId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        var session = await _sessionRepository.GetAsync(command.Id, cancellationToken);
        if (session is null)
        {
            throw new SessionNotFoundException(command.Id);
        }

        await _messageBroker.PublishAsync(new SessionCreated(session));

        return MapToSessionDto(session);
    }

    public async Task<SessionDto> RefreshSessionAsync(RefreshSession command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refreshing session: {SessionId}", command.SessionId);

        var session = await _sessionRepository.GetAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            throw new SessionNotFoundException(command.SessionId);
        }

        if (!session.IsActive())
        {
            throw new SessionNotActiveException(command.SessionId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
        
        session = await _sessionRepository.GetAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            throw new SessionNotFoundException(command.SessionId);
        }

        return MapToSessionDto(session);
    }

    public async Task RevokeSessionAsync(RevokeSession command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking session: {SessionId}", command.SessionId);

        var session = await _sessionRepository.GetAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            throw new SessionNotFoundException(command.SessionId);
        }

        await _commandDispatcher.SendAsync(command, cancellationToken);
    }

    #endregion

    #region Session Management

    public async Task<SessionDto?> GetSessionByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        return session is null ? null : MapToSessionDto(session);
    }

    public async Task<SessionDto?> GetSessionByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByAccessTokenAsync(accessToken, cancellationToken);
        return session is null ? null : MapToSessionDto(session);
    }

    public async Task<IEnumerable<SessionDto>> GetUserSessionsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetByUserIdAsync(userId, cancellationToken);
        return sessions.Select(MapToSessionDto);
    }

    public async Task RevokeAllUserSessionsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking all sessions for user: {UserId}", userId);

        var sessions = await _sessionRepository.GetByUserIdAsync(userId, cancellationToken);
        var activeSessions = sessions.Where(s => s.IsActive());

        foreach (var session in activeSessions)
        {
            var revokeCommand = new RevokeSession(session.Id);
            await _commandDispatcher.SendAsync(revokeCommand, cancellationToken);
        }
    }

    public async Task RevokeExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking expired sessions");

        var expiredSessions = await _sessionRepository.GetExpiredSessionsAsync(_clock.CurrentDate(), cancellationToken);
        
        foreach (var session in expiredSessions)
        {
            var revokeCommand = new RevokeSession(session.Id);
            await _commandDispatcher.SendAsync(revokeCommand, cancellationToken);
        }
    }

    #endregion

    #region Session Validation

    public async Task<bool> IsSessionValidAsync(SessionId sessionId, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetAsync(sessionId, cancellationToken);
        return session is not null && session.IsActive() && session.ExpiresAt > _clock.CurrentDate();
    }

    public async Task<bool> IsRefreshTokenValidAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        return session is not null && session.IsActive() && session.ExpiresAt > _clock.CurrentDate();
    }

    public async Task<bool> IsAccessTokenValidAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByAccessTokenAsync(accessToken, cancellationToken);
        return session is not null && session.IsActive() && session.ExpiresAt > _clock.CurrentDate();
    }

    #endregion

    #region Session Statistics

    public async Task<SessionStatisticsDto> GetSessionStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalSessions = await _sessionRepository.CountAsync(cancellationToken);
        var activeSessions = await _sessionRepository.CountActiveAsync(cancellationToken);
        var expiredSessions = await _sessionRepository.CountExpiredAsync(cancellationToken);
        var revokedSessions = await _sessionRepository.CountRevokedAsync(cancellationToken);

        return new SessionStatisticsDto
        {
            TotalSessions = totalSessions,
            ActiveSessions = activeSessions,
            ExpiredSessions = expiredSessions,
            RevokedSessions = revokedSessions
        };
    }

    #endregion

    #region Private Helper Methods

    private static SessionDto MapToSessionDto(Session session)
    {
        return new SessionDto(
            session.Id,
            session.UserId,
            session.AccessToken,
            session.RefreshToken,
            session.ExpiresAt,
            session.Status.ToString(),
            session.IpAddress,
            session.UserAgent,
            session.LastAccessedAt,
            session.CreatedAt,
            session.ModifiedAt
        );
    }

    #endregion
}

