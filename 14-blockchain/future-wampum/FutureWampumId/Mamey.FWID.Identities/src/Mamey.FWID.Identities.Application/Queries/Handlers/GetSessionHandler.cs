using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetSessionHandler : IQueryHandler<GetSession, SessionDto>
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<SessionDto> HandleAsync(GetSession query, CancellationToken cancellationToken = default)
    {
        var sessionId = new SessionId(query.SessionId);
        var session = await _sessionRepository.GetAsync(sessionId, cancellationToken);
        if (session == null)
            throw new InvalidOperationException("Session not found");

        return new SessionDto
        {
            SessionId = session.Id.Value,
            CreatedAt = session.CreatedAt,
            LastAccessedAt = session.LastAccessedAt,
            ExpiresAt = session.ExpiresAt,
            IpAddress = session.IpAddress,
            UserAgent = session.UserAgent
        };
    }
}

