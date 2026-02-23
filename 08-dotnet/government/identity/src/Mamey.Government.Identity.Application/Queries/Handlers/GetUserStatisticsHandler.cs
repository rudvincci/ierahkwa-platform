using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetUserStatisticsHandler : IQueryHandler<GetUserStatistics, UserStatisticsDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserStatisticsHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserStatisticsDto> HandleAsync(GetUserStatistics query, CancellationToken cancellationToken = default)
    {
        var totalUsers = await _userRepository.CountAsync(cancellationToken);
        var activeUsers = await _userRepository.CountByStatusAsync(Mamey.Government.Identity.Domain.Entities.UserStatus.Active, cancellationToken);
        var lockedUsers = await _userRepository.CountLockedAsync(cancellationToken);
        var usersWith2FA = await _userRepository.CountWithTwoFactorAsync(cancellationToken);
        var usersWithMFA = await _userRepository.CountWithMultiFactorAsync(cancellationToken);

        return new UserStatisticsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = totalUsers - activeUsers,
            LockedUsers = lockedUsers,
            UsersWithTwoFactor = usersWith2FA,
            UsersWithMultiFactor = usersWithMFA
        };
    }
}
