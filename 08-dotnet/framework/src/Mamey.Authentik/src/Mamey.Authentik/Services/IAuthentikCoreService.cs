using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Core API operations.
/// </summary>
public interface IAuthentikCoreService
{
    /// <summary>
    /// GET /core/application_entitlements/
    /// </summary>
    Task<PaginatedResult<object>> ApplicationEntitlementsListAsync(string? app = null, string? pbm_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/application_entitlements/
    /// </summary>
    Task<object?> ApplicationEntitlementsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    Task<object?> ApplicationEntitlementsRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    Task<object?> ApplicationEntitlementsUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    Task<object?> ApplicationEntitlementsPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    Task<object?> ApplicationEntitlementsDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/application_entitlements/{pbm_uuid}/used_by/
    /// </summary>
    Task<object?> ApplicationEntitlementsUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/applications/
    /// </summary>
    Task<PaginatedResult<object>> ApplicationsListAsync(int? for_user = null, string? group = null, string? meta_description = null, string? meta_launch_url = null, string? meta_publisher = null, bool? only_with_launch_url = null, string? slug = null, bool? superuser_full_list = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/applications/
    /// </summary>
    Task<object?> ApplicationsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/applications/{slug}/
    /// </summary>
    Task<object?> ApplicationsRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/applications/{slug}/
    /// </summary>
    Task<object?> ApplicationsUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/applications/{slug}/
    /// </summary>
    Task<object?> ApplicationsPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/applications/{slug}/
    /// </summary>
    Task<object?> ApplicationsDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/applications/{slug}/check_access/
    /// </summary>
    Task<object?> ApplicationsCheckAccessRetrieveAsync(string slug, int? for_user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/applications/{slug}/set_icon/
    /// </summary>
    Task<object?> ApplicationsSetIconCreateAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/applications/{slug}/set_icon_url/
    /// </summary>
    Task<object?> ApplicationsSetIconUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/applications/{slug}/used_by/
    /// </summary>
    Task<object?> ApplicationsUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/authenticated_sessions/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatedSessionsListAsync(string? session__last_ip = null, string? session__last_user_agent = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/authenticated_sessions/{uuid}/
    /// </summary>
    Task<object?> AuthenticatedSessionsRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/authenticated_sessions/{uuid}/
    /// </summary>
    Task<object?> AuthenticatedSessionsDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/authenticated_sessions/{uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatedSessionsUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/brands/
    /// </summary>
    Task<PaginatedResult<object>> BrandsListAsync(string? brand_uuid = null, string? branding_default_flow_background = null, string? branding_favicon = null, string? branding_logo = null, string? branding_title = null, string? client_certificates = null, bool? @default = null, string? domain = null, string? flow_authentication = null, string? flow_device_code = null, string? flow_invalidation = null, string? flow_recovery = null, string? flow_unenrollment = null, string? flow_user_settings = null, string? web_certificate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/brands/
    /// </summary>
    Task<object?> BrandsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/brands/{brand_uuid}/
    /// </summary>
    Task<object?> BrandsRetrieveAsync(string brand_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/brands/{brand_uuid}/
    /// </summary>
    Task<object?> BrandsUpdateAsync(string brand_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/brands/{brand_uuid}/
    /// </summary>
    Task<object?> BrandsPartialUpdateAsync(string brand_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/brands/{brand_uuid}/
    /// </summary>
    Task<object?> BrandsDestroyAsync(string brand_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/brands/{brand_uuid}/used_by/
    /// </summary>
    Task<object?> BrandsUsedByListAsync(string brand_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/brands/current/
    /// </summary>
    Task<PaginatedResult<object>> BrandsCurrentRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/groups/
    /// </summary>
    Task<PaginatedResult<object>> GroupsListAsync(string? attributes = null, bool? include_children = null, bool? include_users = null, bool? is_superuser = null, string? members_by_pk = null, string? members_by_username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/groups/
    /// </summary>
    Task<object?> GroupsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/groups/{group_uuid}/
    /// </summary>
    Task<object?> GroupsRetrieveAsync(string group_uuid, bool? include_children = null, bool? include_users = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/groups/{group_uuid}/
    /// </summary>
    Task<object?> GroupsUpdateAsync(string group_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/groups/{group_uuid}/
    /// </summary>
    Task<object?> GroupsPartialUpdateAsync(string group_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/groups/{group_uuid}/
    /// </summary>
    Task<object?> GroupsDestroyAsync(string group_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/groups/{group_uuid}/add_user/
    /// </summary>
    Task<object?> GroupsAddUserCreateAsync(string group_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/groups/{group_uuid}/remove_user/
    /// </summary>
    Task<object?> GroupsRemoveUserCreateAsync(string group_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/groups/{group_uuid}/used_by/
    /// </summary>
    Task<object?> GroupsUsedByListAsync(string group_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/tokens/
    /// </summary>
    Task<PaginatedResult<object>> TokensListAsync(string? description = null, string? expires = null, bool? expiring = null, string? identifier = null, string? intent = null, string? managed = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/tokens/
    /// </summary>
    Task<object?> TokensCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/tokens/{identifier}/
    /// </summary>
    Task<object?> TokensRetrieveAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/tokens/{identifier}/
    /// </summary>
    Task<object?> TokensUpdateAsync(string identifier, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/tokens/{identifier}/
    /// </summary>
    Task<object?> TokensPartialUpdateAsync(string identifier, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/tokens/{identifier}/
    /// </summary>
    Task<object?> TokensDestroyAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/tokens/{identifier}/set_key/
    /// </summary>
    Task<object?> TokensSetKeyCreateAsync(string identifier, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/tokens/{identifier}/used_by/
    /// </summary>
    Task<object?> TokensUsedByListAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/tokens/{identifier}/view_key/
    /// </summary>
    Task<object?> TokensViewKeyRetrieveAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/transactional/applications/
    /// </summary>
    Task<object?> TransactionalApplicationsUpdateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/user_consent/
    /// </summary>
    Task<PaginatedResult<object>> UserConsentListAsync(string? application = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/user_consent/{id}/
    /// </summary>
    Task<object?> UserConsentRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/user_consent/{id}/
    /// </summary>
    Task<object?> UserConsentDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/user_consent/{id}/used_by/
    /// </summary>
    Task<object?> UserConsentUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/
    /// </summary>
    Task<PaginatedResult<object>> UsersListAsync(string? attributes = null, string? date_joined = null, string? date_joined__gt = null, string? date_joined__lt = null, string? email = null, string? groups_by_name = null, string? groups_by_pk = null, bool? include_groups = null, bool? is_active = null, bool? is_superuser = null, string? last_updated = null, string? last_updated__gt = null, string? last_updated__lt = null, string? path = null, string? path_startswith = null, string? type = null, string? username = null, string? uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/
    /// </summary>
    Task<object?> UsersCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/{id}/
    /// </summary>
    Task<object?> UsersRetrieveAsync(int id, CancellationToken cancellationToken = default);
    
    // Backward compatibility methods
    /// <summary>
    /// Gets a user by ID (backward compatibility wrapper).
    /// </summary>
    Task<object?> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists users with pagination (backward compatibility wrapper).
    /// </summary>
    Task<PaginatedResult<object>> ListUsersAsync(int? page = null, int? pageSize = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /core/users/{id}/
    /// </summary>
    Task<object?> UsersUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /core/users/{id}/
    /// </summary>
    Task<object?> UsersPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /core/users/{id}/
    /// </summary>
    Task<object?> UsersDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/{id}/impersonate/
    /// </summary>
    Task<object?> UsersImpersonateCreateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/{id}/recovery/
    /// </summary>
    Task<object?> UsersRecoveryCreateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/{id}/recovery_email/
    /// </summary>
    Task<object?> UsersRecoveryEmailCreateAsync(int id, string? email_stage = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/{id}/set_password/
    /// </summary>
    Task<object?> UsersSetPasswordCreateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/{id}/used_by/
    /// </summary>
    Task<object?> UsersUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/impersonate_end/
    /// </summary>
    Task<PaginatedResult<object>> UsersImpersonateEndRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/me/
    /// </summary>
    Task<PaginatedResult<object>> UsersMeRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /core/users/paths/
    /// </summary>
    Task<PaginatedResult<object>> UsersPathsRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /core/users/service_account/
    /// </summary>
    Task<object?> UsersServiceAccountCreateAsync(object request, CancellationToken cancellationToken = default);

}
