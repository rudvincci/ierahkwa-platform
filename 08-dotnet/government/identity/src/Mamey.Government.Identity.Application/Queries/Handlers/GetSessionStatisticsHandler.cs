using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Time;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetSessionStatisticsHandler : IQueryHandler<GetSessionStatistics, SessionStatisticsDto>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IClock _clock;

    public GetSessionStatisticsHandler(ISessionRepository sessionRepository, IClock clock)
    {
        _sessionRepository = sessionRepository;
        _clock = clock;
    }

    public async Task<SessionStatisticsDto> HandleAsync(GetSessionStatistics query, CancellationToken cancellationToken = default)
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
}
