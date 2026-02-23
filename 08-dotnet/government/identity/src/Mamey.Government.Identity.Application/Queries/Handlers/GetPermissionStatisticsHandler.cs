using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetPermissionStatisticsHandler : IQueryHandler<GetPermissionStatistics, PermissionStatisticsDto>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionStatisticsHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<PermissionStatisticsDto> HandleAsync(GetPermissionStatistics query, CancellationToken cancellationToken = default)
    {
        var totalPermissions = await _permissionRepository.CountAsync(cancellationToken);
        var activePermissions = await _permissionRepository.CountByStatusAsync(PermissionStatus.Active, cancellationToken);
        var inactivePermissions = await _permissionRepository.CountByStatusAsync(PermissionStatus.Inactive, cancellationToken);

        // Get resource and action statistics
        var resources = await _permissionRepository.GetDistinctResourcesAsync(cancellationToken);
        var actions = await _permissionRepository.GetDistinctActionsAsync(cancellationToken);

        return new PermissionStatisticsDto
        {
            TotalPermissions = totalPermissions,
            ActivePermissions = activePermissions,
            InactivePermissions = inactivePermissions,
            UniqueResources = resources.Count(),
            UniqueActions = actions.Count()
        };
    }
}
