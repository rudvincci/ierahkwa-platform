using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetPermissionsByResourceHandler : IQueryHandler<GetPermissionsByResource, IEnumerable<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionsByResourceHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<IEnumerable<PermissionDto>> HandleAsync(GetPermissionsByResource query, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetByResourceAsync(query.Resource, cancellationToken);
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
