using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetUserSessionsHandler : IQueryHandler<GetUserSessions, IEnumerable<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetUserSessionsHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IEnumerable<SessionDto>> HandleAsync(GetUserSessions query, CancellationToken cancellationToken = default)
    {
        var sessions = await _sessionRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return sessions.Select(MapToSessionDto);
    }

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
}
