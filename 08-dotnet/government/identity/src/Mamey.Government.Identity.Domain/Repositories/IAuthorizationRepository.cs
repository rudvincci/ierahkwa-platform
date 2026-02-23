using System;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Repositories;

/// <summary>
/// Repository for authorization-related queries that span multiple aggregates.
/// </summary>
internal interface IAuthorizationRepository
{
    // Subject authorization queries
    Task<bool> HasPermissionAsync(SubjectId subjectId, string resource, string action, CancellationToken cancellationToken = default);
    Task<bool> HasRoleAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default);
    Task<bool> HasAnyRoleAsync(SubjectId subjectId, IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);
    Task<bool> HasAllRolesAsync(SubjectId subjectId, IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);
    
    // Role and permission queries
    Task<IReadOnlyList<Role>> GetRolesForSubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsForSubjectAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsForRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetRolesWithPermissionAsync(PermissionId permissionId, CancellationToken cancellationToken = default);
    
    // Permission checking queries
    Task<bool> PermissionExistsAsync(string resource, string action, CancellationToken cancellationToken = default);
    Task<Permission?> GetPermissionByResourceAndActionAsync(string resource, string action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsByResourceAsync(string resource, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetPermissionsByActionAsync(string action, CancellationToken cancellationToken = default);
    
    // Role management queries
    Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetSubjectsWithRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Subject>> GetSubjectsWithAnyRoleAsync(IEnumerable<RoleId> roleIds, CancellationToken cancellationToken = default);
    
    // Bulk operations
    Task AssignRoleToSubjectAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default);
    Task RemoveRoleFromSubjectAsync(SubjectId subjectId, RoleId roleId, CancellationToken cancellationToken = default);
    Task AssignPermissionToRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default);
}
