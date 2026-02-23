using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetRoleStatisticsHandler : IQueryHandler<GetRoleStatistics, RoleStatisticsDto>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleStatisticsHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleStatisticsDto> HandleAsync(GetRoleStatistics query, CancellationToken cancellationToken = default)
    {
        var totalRoles = await _roleRepository.CountAsync(cancellationToken);
        var activeRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Active, cancellationToken);
        var inactiveRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Inactive, cancellationToken);
        var deprecatedRoles = await _roleRepository.CountByStatusAsync(RoleStatus.Deprecated, cancellationToken);

        return new RoleStatisticsDto
        {
            TotalRoles = totalRoles,
            ActiveRoles = activeRoles,
            InactiveRoles = inactiveRoles,
            DeprecatedRoles = deprecatedRoles
        };
    }
}
