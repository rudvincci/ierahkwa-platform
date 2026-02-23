using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetSessionByRefreshTokenHandler : IQueryHandler<GetSessionByRefreshToken, SessionDto>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionByRefreshTokenHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionDto> HandleAsync(GetSessionByRefreshToken query, CancellationToken cancellationToken = default)
    {
        var session = await _sessionRepository.GetByRefreshTokenAsync(query.RefreshToken, cancellationToken);
        
        if (session is null)
        {
            throw new SessionNotFoundException();
        }

        return MapToSessionDto(session);
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
