using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Providers API operations.
/// </summary>
public interface IAuthentikProvidersService
{
    /// <summary>
    /// GET /providers/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(bool? application__isnull = null, bool? backchannel = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/all/{id}/
    /// </summary>
    Task<object?> AllRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/all/{id}/
    /// </summary>
    Task<object?> AllDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/all/{id}/used_by/
    /// </summary>
    Task<object?> AllUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/all/types/
    /// </summary>
    Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace/
    /// </summary>
    Task<PaginatedResult<object>> GoogleWorkspaceListAsync(string? delegated_subject = null, bool? exclude_users_service_account = null, string? filter_group = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/google_workspace/
    /// </summary>
    Task<object?> GoogleWorkspaceCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/google_workspace/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/google_workspace/{id}/
    /// </summary>
    Task<object?> GoogleWorkspacePartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/google_workspace/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/google_workspace/{id}/sync/object/
    /// </summary>
    Task<object?> GoogleWorkspaceSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace/{id}/sync/status/
    /// </summary>
    Task<object?> GoogleWorkspaceSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace/{id}/used_by/
    /// </summary>
    Task<object?> GoogleWorkspaceUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_groups/
    /// </summary>
    Task<PaginatedResult<object>> GoogleWorkspaceGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/google_workspace_groups/
    /// </summary>
    Task<object?> GoogleWorkspaceGroupsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_groups/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/google_workspace_groups/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceGroupsDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_groups/{id}/used_by/
    /// </summary>
    Task<object?> GoogleWorkspaceGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_users/
    /// </summary>
    Task<PaginatedResult<object>> GoogleWorkspaceUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/google_workspace_users/
    /// </summary>
    Task<object?> GoogleWorkspaceUsersCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_users/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceUsersRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/google_workspace_users/{id}/
    /// </summary>
    Task<object?> GoogleWorkspaceUsersDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/google_workspace_users/{id}/used_by/
    /// </summary>
    Task<object?> GoogleWorkspaceUsersUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ldap/
    /// </summary>
    Task<PaginatedResult<object>> LdapListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, string? base_dn__iexact = null, string? certificate__kp_uuid__iexact = null, string? certificate__name__iexact = null, int? gid_start_number__iexact = null, string? name__iexact = null, string? tls_server_name__iexact = null, int? uid_start_number__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/ldap/
    /// </summary>
    Task<object?> LdapCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ldap/{id}/
    /// </summary>
    Task<object?> LdapRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/ldap/{id}/
    /// </summary>
    Task<object?> LdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/ldap/{id}/
    /// </summary>
    Task<object?> LdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/ldap/{id}/
    /// </summary>
    Task<object?> LdapDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ldap/{id}/used_by/
    /// </summary>
    Task<object?> LdapUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra/
    /// </summary>
    Task<PaginatedResult<object>> MicrosoftEntraListAsync(bool? exclude_users_service_account = null, string? filter_group = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/microsoft_entra/
    /// </summary>
    Task<object?> MicrosoftEntraCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/microsoft_entra/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/microsoft_entra/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/microsoft_entra/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/microsoft_entra/{id}/sync/object/
    /// </summary>
    Task<object?> MicrosoftEntraSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/sync/status/
    /// </summary>
    Task<object?> MicrosoftEntraSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/used_by/
    /// </summary>
    Task<object?> MicrosoftEntraUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_groups/
    /// </summary>
    Task<PaginatedResult<object>> MicrosoftEntraGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/microsoft_entra_groups/
    /// </summary>
    Task<object?> MicrosoftEntraGroupsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_groups/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/microsoft_entra_groups/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraGroupsDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_groups/{id}/used_by/
    /// </summary>
    Task<object?> MicrosoftEntraGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_users/
    /// </summary>
    Task<PaginatedResult<object>> MicrosoftEntraUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/microsoft_entra_users/
    /// </summary>
    Task<object?> MicrosoftEntraUsersCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_users/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraUsersRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/microsoft_entra_users/{id}/
    /// </summary>
    Task<object?> MicrosoftEntraUsersDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/microsoft_entra_users/{id}/used_by/
    /// </summary>
    Task<object?> MicrosoftEntraUsersUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/oauth2/
    /// </summary>
    Task<PaginatedResult<object>> Oauth2ListAsync(string? access_code_validity = null, string? access_token_validity = null, string? application = null, string? authorization_flow = null, string? client_id = null, string? client_type = null, bool? include_claims_in_id_token = null, string? issuer_mode = null, string? property_mappings = null, string? refresh_token_validity = null, string? signing_key = null, string? sub_mode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/oauth2/
    /// </summary>
    Task<object?> Oauth2CreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/oauth2/{id}/
    /// </summary>
    Task<object?> Oauth2RetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/oauth2/{id}/
    /// </summary>
    Task<object?> Oauth2UpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/oauth2/{id}/
    /// </summary>
    Task<object?> Oauth2PartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/oauth2/{id}/
    /// </summary>
    Task<object?> Oauth2DestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/oauth2/{id}/preview_user/
    /// </summary>
    Task<object?> Oauth2PreviewUserRetrieveAsync(int id, int? for_user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/oauth2/{id}/setup_urls/
    /// </summary>
    Task<object?> Oauth2SetupUrlsRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/oauth2/{id}/used_by/
    /// </summary>
    Task<object?> Oauth2UsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/proxy/
    /// </summary>
    Task<PaginatedResult<object>> ProxyListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, bool? basic_auth_enabled__iexact = null, string? basic_auth_password_attribute__iexact = null, string? basic_auth_user_attribute__iexact = null, string? certificate__kp_uuid__iexact = null, string? certificate__name__iexact = null, string? cookie_domain__iexact = null, string? external_host__iexact = null, string? internal_host__iexact = null, bool? internal_host_ssl_validation__iexact = null, string? mode__iexact = null, string? name__iexact = null, string? property_mappings__iexact = null, string? skip_path_regex__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/proxy/
    /// </summary>
    Task<object?> ProxyCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/proxy/{id}/
    /// </summary>
    Task<object?> ProxyRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/proxy/{id}/
    /// </summary>
    Task<object?> ProxyUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/proxy/{id}/
    /// </summary>
    Task<object?> ProxyPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/proxy/{id}/
    /// </summary>
    Task<object?> ProxyDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/proxy/{id}/used_by/
    /// </summary>
    Task<object?> ProxyUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/rac/
    /// </summary>
    Task<PaginatedResult<object>> RacListAsync(bool? application__isnull = null, string? name__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/rac/
    /// </summary>
    Task<object?> RacCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/rac/{id}/
    /// </summary>
    Task<object?> RacRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/rac/{id}/
    /// </summary>
    Task<object?> RacUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/rac/{id}/
    /// </summary>
    Task<object?> RacPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/rac/{id}/
    /// </summary>
    Task<object?> RacDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/rac/{id}/used_by/
    /// </summary>
    Task<object?> RacUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/radius/
    /// </summary>
    Task<PaginatedResult<object>> RadiusListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, string? client_networks__iexact = null, string? name__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/radius/
    /// </summary>
    Task<object?> RadiusCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/radius/{id}/
    /// </summary>
    Task<object?> RadiusRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/radius/{id}/
    /// </summary>
    Task<object?> RadiusUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/radius/{id}/
    /// </summary>
    Task<object?> RadiusPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/radius/{id}/
    /// </summary>
    Task<object?> RadiusDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/radius/{id}/used_by/
    /// </summary>
    Task<object?> RadiusUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/saml/
    /// </summary>
    Task<PaginatedResult<object>> SamlListAsync(string? acs_url = null, string? assertion_valid_not_before = null, string? assertion_valid_not_on_or_after = null, string? audience = null, string? authentication_flow = null, string? authn_context_class_ref_mapping = null, string? authorization_flow = null, string? backchannel_application = null, string? default_name_id_policy = null, string? default_relay_state = null, string? digest_algorithm = null, string? encryption_kp = null, string? invalidation_flow = null, bool? is_backchannel = null, string? issuer = null, string? logout_method = null, string? name_id_mapping = null, string? property_mappings = null, string? session_valid_not_on_or_after = null, bool? sign_assertion = null, bool? sign_logout_request = null, bool? sign_response = null, string? signature_algorithm = null, string? signing_kp = null, string? sls_binding = null, string? sls_url = null, string? sp_binding = null, string? verification_kp = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/saml/
    /// </summary>
    Task<object?> SamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/saml/{id}/
    /// </summary>
    Task<object?> SamlRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/saml/{id}/
    /// </summary>
    Task<object?> SamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/saml/{id}/
    /// </summary>
    Task<object?> SamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/saml/{id}/
    /// </summary>
    Task<object?> SamlDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/saml/{id}/metadata/
    /// </summary>
    Task<object?> SamlMetadataRetrieveAsync(int id, bool? download = null, string? force_binding = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/saml/{id}/preview_user/
    /// </summary>
    Task<object?> SamlPreviewUserRetrieveAsync(int id, int? for_user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/saml/{id}/used_by/
    /// </summary>
    Task<object?> SamlUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/saml/import_metadata/
    /// </summary>
    Task<object?> SamlImportMetadataCreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim/
    /// </summary>
    Task<PaginatedResult<object>> ScimListAsync(bool? exclude_users_service_account = null, string? filter_group = null, string? url = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/scim/
    /// </summary>
    Task<object?> ScimCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim/{id}/
    /// </summary>
    Task<object?> ScimRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/scim/{id}/
    /// </summary>
    Task<object?> ScimUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/scim/{id}/
    /// </summary>
    Task<object?> ScimPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/scim/{id}/
    /// </summary>
    Task<object?> ScimDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/scim/{id}/sync/object/
    /// </summary>
    Task<object?> ScimSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim/{id}/sync/status/
    /// </summary>
    Task<object?> ScimSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim/{id}/used_by/
    /// </summary>
    Task<object?> ScimUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_groups/
    /// </summary>
    Task<PaginatedResult<object>> ScimGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/scim_groups/
    /// </summary>
    Task<object?> ScimGroupsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_groups/{id}/used_by/
    /// </summary>
    Task<object?> ScimGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_users/
    /// </summary>
    Task<PaginatedResult<object>> ScimUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/scim_users/
    /// </summary>
    Task<object?> ScimUsersCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/scim_users/{id}/used_by/
    /// </summary>
    Task<object?> ScimUsersUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ssf/
    /// </summary>
    Task<PaginatedResult<object>> SsfListAsync(bool? application__isnull = null, string? name__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /providers/ssf/
    /// </summary>
    Task<object?> SsfCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ssf/{id}/
    /// </summary>
    Task<object?> SsfRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /providers/ssf/{id}/
    /// </summary>
    Task<object?> SsfUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /providers/ssf/{id}/
    /// </summary>
    Task<object?> SsfPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /providers/ssf/{id}/
    /// </summary>
    Task<object?> SsfDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /providers/ssf/{id}/used_by/
    /// </summary>
    Task<object?> SsfUsedByListAsync(int id, CancellationToken cancellationToken = default);

}
