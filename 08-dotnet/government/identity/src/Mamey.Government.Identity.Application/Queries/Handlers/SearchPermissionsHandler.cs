using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class SearchPermissionsHandler : IQueryHandler<SearchPermissions, IEnumerable<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public SearchPermissionsHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> HandleAsync(SearchPermissions query, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.SearchAsync(query.SearchTerm, cancellationToken);
        return permissions.Select(MapToPermissionDto);
    }

    private static PermissionDto MapToPermissionDto(Permission permission)
    {
        return new PermissionDto(
            permission.Id,
            permission.Name,
            permission.Description,
            permission.Resource,
            permission.Action,
            permission.Status.ToString(),
            permission.CreatedAt,
            permission.ModifiedAt
        );
    }
}
