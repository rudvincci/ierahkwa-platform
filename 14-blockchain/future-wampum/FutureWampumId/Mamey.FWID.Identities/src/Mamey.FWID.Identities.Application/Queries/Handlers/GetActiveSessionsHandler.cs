using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetActiveSessionsHandler : IQueryHandler<GetActiveSessions, List<SessionDto>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetActiveSessionsHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    public async Task<List<SessionDto>> HandleAsync(GetActiveSessions query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        var sessions = await _sessionRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        var activeSessions = sessions.Where(s => s.IsValid()).ToList();

        return activeSessions.Select(s => new SessionDto
        {
            SessionId = s.Id.Value,
            CreatedAt = s.CreatedAt,
            LastAccessedAt = s.LastAccessedAt,
            ExpiresAt = s.ExpiresAt,
            IpAddress = s.IpAddress,
            UserAgent = s.UserAgent
        }).ToList();
    }
}

