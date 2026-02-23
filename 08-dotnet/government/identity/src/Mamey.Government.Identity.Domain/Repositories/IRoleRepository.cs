using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

internal interface IRoleRepository
{
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteAsync(RoleId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Role> GetAsync(RoleId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(RoleId id, CancellationToken cancellationToken = default);
    
    // Role-specific queries
    Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetBySubjectIdAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByPermissionIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, RoleId excludeId, CancellationToken cancellationToken = default);
    
    // Statistics and counting methods
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default);
    
    // Search methods
    Task<IReadOnlyList<Role>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
