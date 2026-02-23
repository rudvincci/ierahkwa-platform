using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetPermissionHandler : IQueryHandler<Mamey.Government.Identity.Contracts.Queries.GetPermission, PermissionDto>
{
    private readonly IPermissionRepository _permissionRepository;

    public GetPermissionHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<PermissionDto> HandleAsync(Mamey.Government.Identity.Contracts.Queries.GetPermission query, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetAsync(query.Id, cancellationToken);
        
        if (permission is null)
        {
            throw new PermissionNotFoundException(query.Id);
        }

        return MapToPermissionDto(permission);
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
