using System.Linq;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class BrowseRolesHandler : IQueryHandler<BrowseRoles, PagedResult<RoleDto>?>
{
    private readonly IRoleRepository _roleRepository;

    public BrowseRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<PagedResult<RoleDto>?> HandleAsync(BrowseRoles query, CancellationToken cancellationToken = default)
    {
        var allRoles = await _roleRepository.BrowseAsync(cancellationToken);
        
        // Filter roles based on query parameters
        var filteredRoles = allRoles.AsQueryable();
        
        if (!string.IsNullOrEmpty(query.Name))
        {
            filteredRoles = filteredRoles.Where(r => 
                r.Name.Contains(query.Name.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(query.Status))
        {
            if (Enum.TryParse<RoleStatus>(query.Status, true, out var status))
            {
                filteredRoles = filteredRoles.Where(r => r.Status == status);
            }
        }
        
        var rolesList = filteredRoles.ToList();
        
        // Manual pagination
        var page = query.Page > 0 ? query.Page : 1;
        var pageSize = query.ResultsPerPage > 0 ? query.ResultsPerPage : 10;
        var totalResults = rolesList.Count;
        var totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
        var skip = (page - 1) * pageSize;
        var pagedRoles = rolesList.Skip(skip).Take(pageSize);
        
        var roleDtos = pagedRoles.Select(MapToRoleDto).ToList();
        
        return PagedResult<RoleDto>.Create(
            roleDtos,
            page,
            pageSize,
            totalPages,
            totalResults
        );
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

