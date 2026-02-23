using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class SearchRolesHandler : IQueryHandler<SearchRoles, IEnumerable<RoleDto>>
{
    private readonly IRoleRepository _roleRepository;

    public SearchRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<IEnumerable<RoleDto>> HandleAsync(SearchRoles query, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.SearchAsync(query.SearchTerm, cancellationToken);
        return roles.Select(MapToRoleDto);
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
