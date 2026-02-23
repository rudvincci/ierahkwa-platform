using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetPermissionsByRoleHandler : IQueryHandler<GetPermissionsByRole, IEnumerable<PermissionDto>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;

    public GetPermissionsByRoleHandler(IPermissionRepository permissionRepository, IRoleRepository roleRepository)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<PermissionDto>> HandleAsync(GetPermissionsByRole query, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(query.RoleId, cancellationToken);
        if (role is null)
        {
            throw new RoleNotFoundException(query.RoleId);
        }

        var permissions = await _permissionRepository.GetByIdsAsync(role.Permissions, cancellationToken);
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
