using System.Linq;
using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class BrowsePermissionsHandler : IQueryHandler<BrowsePermissions, PagedResult<PermissionDto>?>
{
    private readonly IPermissionRepository _permissionRepository;

    public BrowsePermissionsHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<PagedResult<PermissionDto>?> HandleAsync(BrowsePermissions query, CancellationToken cancellationToken = default)
    {
        var allPermissions = await _permissionRepository.BrowseAsync(cancellationToken);
        
        // Filter permissions based on query parameters
        var filteredPermissions = allPermissions.AsQueryable();
        
        if (!string.IsNullOrEmpty(query.Resource))
        {
            filteredPermissions = filteredPermissions.Where(p => 
                p.Resource.Contains(query.Resource.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(query.Action))
        {
            filteredPermissions = filteredPermissions.Where(p => 
                p.Action.Contains(query.Action.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(query.Status))
        {
            if (Enum.TryParse<PermissionStatus>(query.Status, true, out var status))
            {
                filteredPermissions = filteredPermissions.Where(p => p.Status == status);
            }
        }
        
        var permissionsList = filteredPermissions.ToList();
        
        // Manual pagination
        var page = query.Page > 0 ? query.Page : 1;
        var pageSize = query.ResultsPerPage > 0 ? query.ResultsPerPage : 10;
        var totalResults = permissionsList.Count;
        var totalPages = (int)Math.Ceiling(totalResults / (double)pageSize);
        var skip = (page - 1) * pageSize;
        var pagedPermissions = permissionsList.Skip(skip).Take(pageSize);
        
        var permissionDtos = pagedPermissions.Select(MapToPermissionDto).ToList();
        
        return PagedResult<PermissionDto>.Create(
            permissionDtos,
            page,
            pageSize,
            totalPages,
            totalResults
        );
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

