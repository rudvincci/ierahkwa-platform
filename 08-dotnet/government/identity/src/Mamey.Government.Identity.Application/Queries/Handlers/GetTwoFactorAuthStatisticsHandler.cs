using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetTwoFactorAuthStatisticsHandler : IQueryHandler<GetTwoFactorAuthStatistics, TwoFactorAuthStatisticsDto>
{
    private readonly ITwoFactorAuthRepository _twoFactorAuthRepository;

    public GetTwoFactorAuthStatisticsHandler(ITwoFactorAuthRepository twoFactorAuthRepository)
    {
        _twoFactorAuthRepository = twoFactorAuthRepository;
    }

    public async Task<TwoFactorAuthStatisticsDto> HandleAsync(GetTwoFactorAuthStatistics query, CancellationToken cancellationToken = default)
    {
        var total2FA = await _twoFactorAuthRepository.CountAsync(cancellationToken);
        var active2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Active, cancellationToken);
        var pending2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Pending, cancellationToken);
        var disabled2FA = await _twoFactorAuthRepository.CountByStatusAsync(TwoFactorAuthStatus.Disabled, cancellationToken);

        return new TwoFactorAuthStatisticsDto
        {
            TotalTwoFactorAuth = total2FA,
            ActiveTwoFactorAuth = active2FA,
            PendingTwoFactorAuth = pending2FA,
            DisabledTwoFactorAuth = disabled2FA
        };
    }
}
