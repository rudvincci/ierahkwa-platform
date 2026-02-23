using Mamey.Rbac;

namespace Mamey.Blockchain.Rbac;

/// <summary>
/// Client interface for MameyNode RBAC (Role-Based Access Control) service.
/// </summary>
public interface IRbacClient : IDisposable
{
    // Role management
    Task<CreateRoleResponse> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<GetRoleResponse> GetRoleAsync(GetRoleRequest request, CancellationToken cancellationToken = default);
    Task<ListRolesResponse> ListRolesAsync(ListRolesRequest request, CancellationToken cancellationToken = default);
    Task<UpdateRoleResponse> UpdateRoleAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<DeleteRoleResponse> DeleteRoleAsync(DeleteRoleRequest request, CancellationToken cancellationToken = default);

    // User-role assignment
    Task<AssignRoleResponse> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task<RevokeRoleResponse> RevokeRoleAsync(RevokeRoleRequest request, CancellationToken cancellationToken = default);
    Task<GetUserRolesResponse> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default);
    Task<GetRoleUsersResponse> GetRoleUsersAsync(GetRoleUsersRequest request, CancellationToken cancellationToken = default);

    // Permission management
    Task<CreatePermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default);
    Task<CheckPermissionResponse> CheckPermissionAsync(CheckPermissionRequest request, CancellationToken cancellationToken = default);
    Task<GetRolePermissionsResponse> GetRolePermissionsAsync(GetRolePermissionsRequest request, CancellationToken cancellationToken = default);

    // Hierarchy
    Task<AddInheritanceResponse> AddInheritanceAsync(AddInheritanceRequest request, CancellationToken cancellationToken = default);
    Task<RemoveInheritanceResponse> RemoveInheritanceAsync(RemoveInheritanceRequest request, CancellationToken cancellationToken = default);
    Task<GetRoleHierarchyResponse> GetRoleHierarchyAsync(GetRoleHierarchyRequest request, CancellationToken cancellationToken = default);
}
