using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Sources API operations.
/// </summary>
public interface IAuthentikSourcesService
{
    /// <summary>
    /// GET /sources/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(string? managed = null, string? pbm_uuid = null, string? slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/all/{slug}/
    /// </summary>
    Task<object?> AllRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/all/{slug}/
    /// </summary>
    Task<object?> AllDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/all/{slug}/set_icon/
    /// </summary>
    Task<object?> AllSetIconCreateAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/all/{slug}/set_icon_url/
    /// </summary>
    Task<object?> AllSetIconUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/all/{slug}/used_by/
    /// </summary>
    Task<object?> AllUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/all/types/
    /// </summary>
    Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/all/user_settings/
    /// </summary>
    Task<PaginatedResult<object>> AllUserSettingsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/all/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsAllListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/all/{id}/
    /// </summary>
    Task<object?> GroupConnectionsAllRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/all/{id}/
    /// </summary>
    Task<object?> GroupConnectionsAllUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/all/{id}/
    /// </summary>
    Task<object?> GroupConnectionsAllPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/all/{id}/
    /// </summary>
    Task<object?> GroupConnectionsAllDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/all/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsAllUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/kerberos/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsKerberosListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/kerberos/
    /// </summary>
    Task<object?> GroupConnectionsKerberosCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/kerberos/{id}/
    /// </summary>
    Task<object?> GroupConnectionsKerberosRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/kerberos/{id}/
    /// </summary>
    Task<object?> GroupConnectionsKerberosUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/kerberos/{id}/
    /// </summary>
    Task<object?> GroupConnectionsKerberosPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/kerberos/{id}/
    /// </summary>
    Task<object?> GroupConnectionsKerberosDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/kerberos/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsKerberosUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/ldap/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsLdapListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/ldap/
    /// </summary>
    Task<object?> GroupConnectionsLdapCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/ldap/{id}/
    /// </summary>
    Task<object?> GroupConnectionsLdapRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/ldap/{id}/
    /// </summary>
    Task<object?> GroupConnectionsLdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/ldap/{id}/
    /// </summary>
    Task<object?> GroupConnectionsLdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/ldap/{id}/
    /// </summary>
    Task<object?> GroupConnectionsLdapDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/ldap/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsLdapUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/oauth/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsOauthListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/oauth/
    /// </summary>
    Task<object?> GroupConnectionsOauthCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/oauth/{id}/
    /// </summary>
    Task<object?> GroupConnectionsOauthRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/oauth/{id}/
    /// </summary>
    Task<object?> GroupConnectionsOauthUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/oauth/{id}/
    /// </summary>
    Task<object?> GroupConnectionsOauthPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/oauth/{id}/
    /// </summary>
    Task<object?> GroupConnectionsOauthDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/oauth/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsOauthUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/plex/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsPlexListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/plex/
    /// </summary>
    Task<object?> GroupConnectionsPlexCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/plex/{id}/
    /// </summary>
    Task<object?> GroupConnectionsPlexRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/plex/{id}/
    /// </summary>
    Task<object?> GroupConnectionsPlexUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/plex/{id}/
    /// </summary>
    Task<object?> GroupConnectionsPlexPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/plex/{id}/
    /// </summary>
    Task<object?> GroupConnectionsPlexDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/plex/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsPlexUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/saml/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsSamlListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/saml/
    /// </summary>
    Task<object?> GroupConnectionsSamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/saml/{id}/
    /// </summary>
    Task<object?> GroupConnectionsSamlRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/saml/{id}/
    /// </summary>
    Task<object?> GroupConnectionsSamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/saml/{id}/
    /// </summary>
    Task<object?> GroupConnectionsSamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/saml/{id}/
    /// </summary>
    Task<object?> GroupConnectionsSamlDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/saml/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsSamlUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/telegram/
    /// </summary>
    Task<PaginatedResult<object>> GroupConnectionsTelegramListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/group_connections/telegram/
    /// </summary>
    Task<object?> GroupConnectionsTelegramCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/telegram/{id}/
    /// </summary>
    Task<object?> GroupConnectionsTelegramRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/group_connections/telegram/{id}/
    /// </summary>
    Task<object?> GroupConnectionsTelegramUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/group_connections/telegram/{id}/
    /// </summary>
    Task<object?> GroupConnectionsTelegramPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/group_connections/telegram/{id}/
    /// </summary>
    Task<object?> GroupConnectionsTelegramDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/group_connections/telegram/{id}/used_by/
    /// </summary>
    Task<object?> GroupConnectionsTelegramUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/kerberos/
    /// </summary>
    Task<PaginatedResult<object>> KerberosListAsync(bool? enabled = null, string? kadmin_type = null, bool? password_login_update_internal_password = null, string? pbm_uuid = null, string? realm = null, string? slug = null, string? spnego_server_name = null, string? sync_principal = null, bool? sync_users = null, bool? sync_users_password = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/kerberos/
    /// </summary>
    Task<object?> KerberosCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/kerberos/{slug}/
    /// </summary>
    Task<object?> KerberosRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/kerberos/{slug}/
    /// </summary>
    Task<object?> KerberosUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/kerberos/{slug}/
    /// </summary>
    Task<object?> KerberosPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/kerberos/{slug}/
    /// </summary>
    Task<object?> KerberosDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/kerberos/{slug}/sync/status/
    /// </summary>
    Task<object?> KerberosSyncStatusRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/kerberos/{slug}/used_by/
    /// </summary>
    Task<object?> KerberosUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/ldap/
    /// </summary>
    Task<PaginatedResult<object>> LdapListAsync(string? additional_group_dn = null, string? additional_user_dn = null, string? base_dn = null, string? bind_cn = null, string? client_certificate = null, bool? delete_not_found_objects = null, bool? enabled = null, string? group_membership_field = null, string? group_object_filter = null, string? group_property_mappings = null, bool? lookup_groups_from_user = null, string? object_uniqueness_field = null, bool? password_login_update_internal_password = null, string? pbm_uuid = null, string? peer_certificate = null, string? server_uri = null, string? slug = null, bool? sni = null, bool? start_tls = null, bool? sync_groups = null, string? sync_parent_group = null, bool? sync_users = null, bool? sync_users_password = null, string? user_membership_attribute = null, string? user_object_filter = null, string? user_property_mappings = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/ldap/
    /// </summary>
    Task<object?> LdapCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/ldap/{slug}/
    /// </summary>
    Task<object?> LdapRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/ldap/{slug}/
    /// </summary>
    Task<object?> LdapUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/ldap/{slug}/
    /// </summary>
    Task<object?> LdapPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/ldap/{slug}/
    /// </summary>
    Task<object?> LdapDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/ldap/{slug}/debug/
    /// </summary>
    Task<object?> LdapDebugRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/ldap/{slug}/sync/status/
    /// </summary>
    Task<object?> LdapSyncStatusRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/ldap/{slug}/used_by/
    /// </summary>
    Task<object?> LdapUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/oauth/
    /// </summary>
    Task<PaginatedResult<object>> OauthListAsync(string? access_token_url = null, string? additional_scopes = null, string? authentication_flow = null, string? authorization_url = null, string? consumer_key = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, bool? has_jwks = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? profile_url = null, string? provider_type = null, string? request_token_url = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/oauth/
    /// </summary>
    Task<object?> OauthCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/oauth/{slug}/
    /// </summary>
    Task<object?> OauthRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/oauth/{slug}/
    /// </summary>
    Task<object?> OauthUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/oauth/{slug}/
    /// </summary>
    Task<object?> OauthPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/oauth/{slug}/
    /// </summary>
    Task<object?> OauthDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/oauth/{slug}/used_by/
    /// </summary>
    Task<object?> OauthUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/oauth/source_types/
    /// </summary>
    Task<PaginatedResult<object>> OauthSourceTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/plex/
    /// </summary>
    Task<PaginatedResult<object>> PlexListAsync(bool? allow_friends = null, string? authentication_flow = null, string? client_id = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/plex/
    /// </summary>
    Task<object?> PlexCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/plex/{slug}/
    /// </summary>
    Task<object?> PlexRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/plex/{slug}/
    /// </summary>
    Task<object?> PlexUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/plex/{slug}/
    /// </summary>
    Task<object?> PlexPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/plex/{slug}/
    /// </summary>
    Task<object?> PlexDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/plex/{slug}/used_by/
    /// </summary>
    Task<object?> PlexUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/plex/redeem_token/
    /// </summary>
    Task<object?> PlexRedeemTokenCreateAsync(object request, string? slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/plex/redeem_token_authenticated/
    /// </summary>
    Task<object?> PlexRedeemTokenAuthenticatedCreateAsync(object request, string? slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/saml/
    /// </summary>
    Task<PaginatedResult<object>> SamlListAsync(bool? allow_idp_initiated = null, string? authentication_flow = null, string? binding_type = null, string? digest_algorithm = null, bool? enabled = null, string? enrollment_flow = null, string? issuer = null, string? managed = null, string? name_id_policy = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? pre_authentication_flow = null, string? signature_algorithm = null, bool? signed_assertion = null, bool? signed_response = null, string? signing_kp = null, string? slo_url = null, string? slug = null, string? sso_url = null, string? temporary_user_delete_after = null, string? user_matching_mode = null, string? verification_kp = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/saml/
    /// </summary>
    Task<object?> SamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/saml/{slug}/
    /// </summary>
    Task<object?> SamlRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/saml/{slug}/
    /// </summary>
    Task<object?> SamlUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/saml/{slug}/
    /// </summary>
    Task<object?> SamlPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/saml/{slug}/
    /// </summary>
    Task<object?> SamlDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/saml/{slug}/metadata/
    /// </summary>
    Task<object?> SamlMetadataRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/saml/{slug}/used_by/
    /// </summary>
    Task<object?> SamlUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim/
    /// </summary>
    Task<PaginatedResult<object>> ScimListAsync(string? pbm_uuid = null, string? slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/scim/
    /// </summary>
    Task<object?> ScimCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim/{slug}/
    /// </summary>
    Task<object?> ScimRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/scim/{slug}/
    /// </summary>
    Task<object?> ScimUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/scim/{slug}/
    /// </summary>
    Task<object?> ScimPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/scim/{slug}/
    /// </summary>
    Task<object?> ScimDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim/{slug}/used_by/
    /// </summary>
    Task<object?> ScimUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_groups/
    /// </summary>
    Task<PaginatedResult<object>> ScimGroupsListAsync(string? group__group_uuid = null, string? group__name = null, string? source__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/scim_groups/
    /// </summary>
    Task<object?> ScimGroupsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/scim_groups/{id}/
    /// </summary>
    Task<object?> ScimGroupsDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_groups/{id}/used_by/
    /// </summary>
    Task<object?> ScimGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_users/
    /// </summary>
    Task<PaginatedResult<object>> ScimUsersListAsync(string? source__slug = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/scim_users/
    /// </summary>
    Task<object?> ScimUsersCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/scim_users/{id}/
    /// </summary>
    Task<object?> ScimUsersDestroyAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/scim_users/{id}/used_by/
    /// </summary>
    Task<object?> ScimUsersUsedByListAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/telegram/
    /// </summary>
    Task<PaginatedResult<object>> TelegramListAsync(string? authentication_flow = null, string? bot_username = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, string? pbm_uuid = null, string? policy_engine_mode = null, bool? request_message_access = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/telegram/
    /// </summary>
    Task<object?> TelegramCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/telegram/{slug}/
    /// </summary>
    Task<object?> TelegramRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/telegram/{slug}/
    /// </summary>
    Task<object?> TelegramUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/telegram/{slug}/
    /// </summary>
    Task<object?> TelegramPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/telegram/{slug}/
    /// </summary>
    Task<object?> TelegramDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/telegram/{slug}/used_by/
    /// </summary>
    Task<object?> TelegramUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/all/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsAllListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/all/{id}/
    /// </summary>
    Task<object?> UserConnectionsAllRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/all/{id}/
    /// </summary>
    Task<object?> UserConnectionsAllUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/all/{id}/
    /// </summary>
    Task<object?> UserConnectionsAllPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/all/{id}/
    /// </summary>
    Task<object?> UserConnectionsAllDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/all/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsAllUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/kerberos/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsKerberosListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/kerberos/
    /// </summary>
    Task<object?> UserConnectionsKerberosCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/kerberos/{id}/
    /// </summary>
    Task<object?> UserConnectionsKerberosRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/kerberos/{id}/
    /// </summary>
    Task<object?> UserConnectionsKerberosUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/kerberos/{id}/
    /// </summary>
    Task<object?> UserConnectionsKerberosPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/kerberos/{id}/
    /// </summary>
    Task<object?> UserConnectionsKerberosDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/kerberos/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsKerberosUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/ldap/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsLdapListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/ldap/
    /// </summary>
    Task<object?> UserConnectionsLdapCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/ldap/{id}/
    /// </summary>
    Task<object?> UserConnectionsLdapRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/ldap/{id}/
    /// </summary>
    Task<object?> UserConnectionsLdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/ldap/{id}/
    /// </summary>
    Task<object?> UserConnectionsLdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/ldap/{id}/
    /// </summary>
    Task<object?> UserConnectionsLdapDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/ldap/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsLdapUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/oauth/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsOauthListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/oauth/
    /// </summary>
    Task<object?> UserConnectionsOauthCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/oauth/{id}/
    /// </summary>
    Task<object?> UserConnectionsOauthRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/oauth/{id}/
    /// </summary>
    Task<object?> UserConnectionsOauthUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/oauth/{id}/
    /// </summary>
    Task<object?> UserConnectionsOauthPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/oauth/{id}/
    /// </summary>
    Task<object?> UserConnectionsOauthDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/oauth/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsOauthUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/plex/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsPlexListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/plex/
    /// </summary>
    Task<object?> UserConnectionsPlexCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/plex/{id}/
    /// </summary>
    Task<object?> UserConnectionsPlexRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/plex/{id}/
    /// </summary>
    Task<object?> UserConnectionsPlexUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/plex/{id}/
    /// </summary>
    Task<object?> UserConnectionsPlexPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/plex/{id}/
    /// </summary>
    Task<object?> UserConnectionsPlexDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/plex/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsPlexUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/saml/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsSamlListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/saml/
    /// </summary>
    Task<object?> UserConnectionsSamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/saml/{id}/
    /// </summary>
    Task<object?> UserConnectionsSamlRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/saml/{id}/
    /// </summary>
    Task<object?> UserConnectionsSamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/saml/{id}/
    /// </summary>
    Task<object?> UserConnectionsSamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/saml/{id}/
    /// </summary>
    Task<object?> UserConnectionsSamlDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/saml/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsSamlUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/telegram/
    /// </summary>
    Task<PaginatedResult<object>> UserConnectionsTelegramListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /sources/user_connections/telegram/
    /// </summary>
    Task<object?> UserConnectionsTelegramCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/telegram/{id}/
    /// </summary>
    Task<object?> UserConnectionsTelegramRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /sources/user_connections/telegram/{id}/
    /// </summary>
    Task<object?> UserConnectionsTelegramUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /sources/user_connections/telegram/{id}/
    /// </summary>
    Task<object?> UserConnectionsTelegramPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /sources/user_connections/telegram/{id}/
    /// </summary>
    Task<object?> UserConnectionsTelegramDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /sources/user_connections/telegram/{id}/used_by/
    /// </summary>
    Task<object?> UserConnectionsTelegramUsedByListAsync(int id, CancellationToken cancellationToken = default);

}
