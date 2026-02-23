using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IPermissionRepository
{
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeleteAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Permission> GetAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(PermissionId id, CancellationToken cancellationToken = default);
    
    // Permission-specific queries
    Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, PermissionId excludeId, CancellationToken cancellationToken = default);
    Task<bool> ResourceActionExistsAsync(string resource, string action, CancellationToken cancellationToken = default);
    Task<bool> ResourceActionExistsAsync(string resource, string action, PermissionId excludeId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default);
    
    // Search and batch methods
    Task<IReadOnlyList<Permission>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByIdsAsync(IEnumerable<PermissionId> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctResourcesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctActionsAsync(CancellationToken cancellationToken = default);
}
