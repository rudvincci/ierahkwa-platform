using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Rbac API operations.
/// </summary>
public interface IAuthentikRbacService
{
    /// <summary>
    /// GET /rbac/initial_permissions/
    /// </summary>
    Task<PaginatedResult<object>> InitialPermissionsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /rbac/initial_permissions/
    /// </summary>
    Task<object?> InitialPermissionsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/initial_permissions/{id}/
    /// </summary>
    Task<object?> InitialPermissionsRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rbac/initial_permissions/{id}/
    /// </summary>
    Task<object?> InitialPermissionsUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/initial_permissions/{id}/
    /// </summary>
    Task<object?> InitialPermissionsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rbac/initial_permissions/{id}/
    /// </summary>
    Task<object?> InitialPermissionsDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/initial_permissions/{id}/used_by/
    /// </summary>
    Task<object?> InitialPermissionsUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/
    /// </summary>
    Task<PaginatedResult<object>> PermissionsListAsync(string? codename = null, string? content_type__app_label = null, string? content_type__model = null, string? role = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/{id}/
    /// </summary>
    Task<object?> PermissionsRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/assigned_by_roles/
    /// </summary>
    Task<PaginatedResult<object>> PermissionsAssignedByRolesListAsync(string? model = null, string? object_pk = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /rbac/permissions/assigned_by_roles/{uuid}/assign/
    /// </summary>
    Task<object?> PermissionsAssignedByRolesAssignAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/permissions/assigned_by_roles/{uuid}/unassign/
    /// </summary>
    Task<object?> PermissionsAssignedByRolesUnassignPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/assigned_by_users/
    /// </summary>
    Task<PaginatedResult<object>> PermissionsAssignedByUsersListAsync(string? model = null, string? object_pk = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /rbac/permissions/assigned_by_users/{id}/assign/
    /// </summary>
    Task<object?> PermissionsAssignedByUsersAssignAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/permissions/assigned_by_users/{id}/unassign/
    /// </summary>
    Task<object?> PermissionsAssignedByUsersUnassignPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/roles/
    /// </summary>
    Task<PaginatedResult<object>> PermissionsRolesListAsync(string? uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/roles/{id}/
    /// </summary>
    Task<object?> PermissionsRolesRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rbac/permissions/roles/{id}/
    /// </summary>
    Task<object?> PermissionsRolesUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/permissions/roles/{id}/
    /// </summary>
    Task<object?> PermissionsRolesPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rbac/permissions/roles/{id}/
    /// </summary>
    Task<object?> PermissionsRolesDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/users/
    /// </summary>
    Task<PaginatedResult<object>> PermissionsUsersListAsync(int? user_id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/permissions/users/{id}/
    /// </summary>
    Task<object?> PermissionsUsersRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rbac/permissions/users/{id}/
    /// </summary>
    Task<object?> PermissionsUsersUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/permissions/users/{id}/
    /// </summary>
    Task<object?> PermissionsUsersPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rbac/permissions/users/{id}/
    /// </summary>
    Task<object?> PermissionsUsersDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/roles/
    /// </summary>
    Task<PaginatedResult<object>> RolesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /rbac/roles/
    /// </summary>
    Task<object?> RolesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/roles/{uuid}/
    /// </summary>
    Task<object?> RolesRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rbac/roles/{uuid}/
    /// </summary>
    Task<object?> RolesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rbac/roles/{uuid}/
    /// </summary>
    Task<object?> RolesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rbac/roles/{uuid}/
    /// </summary>
    Task<object?> RolesDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rbac/roles/{uuid}/used_by/
    /// </summary>
    Task<object?> RolesUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

}
