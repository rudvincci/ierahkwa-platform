using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetRoleHandler : IQueryHandler<Mamey.Government.Identity.Contracts.Queries.GetRole, RoleDto>
{
    private readonly IRoleRepository _roleRepository;

    public GetRoleHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDto> HandleAsync(Mamey.Government.Identity.Contracts.Queries.GetRole query, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(query.Id, cancellationToken);
        
        if (role is null)
        {
            throw new RoleNotFoundException(query.Id);
        }

        return MapToRoleDto(role);
    }

    private static RoleDto MapToRoleDto(Role role)
    {
        return new RoleDto(
            role.Id,
            role.Name,
            role.Description,
            role.Status.ToString(),
            role.Permissions.Select(p => p.Value),
            role.CreatedAt,
            role.ModifiedAt
        );
    }
}
