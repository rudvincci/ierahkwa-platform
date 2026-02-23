using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetMultiFactorAuthStatisticsHandler : IQueryHandler<GetMultiFactorAuthStatistics, MultiFactorAuthStatisticsDto>
{
    private readonly IMultiFactorAuthRepository _multiFactorAuthRepository;
    private readonly IMfaChallengeRepository _mfaChallengeRepository;

    public GetMultiFactorAuthStatisticsHandler(IMultiFactorAuthRepository multiFactorAuthRepository, IMfaChallengeRepository mfaChallengeRepository)
    {
        _multiFactorAuthRepository = multiFactorAuthRepository;
        _mfaChallengeRepository = mfaChallengeRepository;
    }

    public async Task<MultiFactorAuthStatisticsDto> HandleAsync(GetMultiFactorAuthStatistics query, CancellationToken cancellationToken = default)
    {
        var totalMFA = await _multiFactorAuthRepository.CountAsync(cancellationToken);
        var activeMFA = await _multiFactorAuthRepository.CountByStatusAsync(MultiFactorAuthStatus.Active, cancellationToken);
        var inactiveMFA = await _multiFactorAuthRepository.CountByStatusAsync(MultiFactorAuthStatus.Inactive, cancellationToken);
        var totalChallenges = await _mfaChallengeRepository.CountAsync(cancellationToken);
        var pendingChallenges = await _mfaChallengeRepository.CountByStatusAsync(MfaChallengeStatus.Pending, cancellationToken);

        return new MultiFactorAuthStatisticsDto
        {
            TotalMultiFactorAuth = totalMFA,
            ActiveMultiFactorAuth = activeMFA,
            InactiveMultiFactorAuth = inactiveMFA,
            TotalChallenges = totalChallenges,
            PendingChallenges = pendingChallenges
        };
    }
}
