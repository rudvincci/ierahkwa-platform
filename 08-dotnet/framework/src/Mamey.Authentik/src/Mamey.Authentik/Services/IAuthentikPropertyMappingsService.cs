using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik PropertyMappings API operations.
/// </summary>
public interface IAuthentikPropertyMappingsService
{
    /// <summary>
    /// GET /propertymappings/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/all/{pm_uuid}/
    /// </summary>
    Task<object?> AllRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/all/{pm_uuid}/
    /// </summary>
    Task<object?> AllDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/all/{pm_uuid}/test/
    /// </summary>
    Task<object?> AllTestCreateAsync(string pm_uuid, object request, bool? format_result = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/all/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> AllUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/all/types/
    /// </summary>
    Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/notification/
    /// </summary>
    Task<PaginatedResult<object>> NotificationListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/notification/
    /// </summary>
    Task<object?> NotificationCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/notification/{pm_uuid}/
    /// </summary>
    Task<object?> NotificationRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/notification/{pm_uuid}/
    /// </summary>
    Task<object?> NotificationUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/notification/{pm_uuid}/
    /// </summary>
    Task<object?> NotificationPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/notification/{pm_uuid}/
    /// </summary>
    Task<object?> NotificationDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/notification/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> NotificationUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/
    /// </summary>
    Task<PaginatedResult<object>> ProviderGoogleWorkspaceListAsync(string? expression = null, string? managed = null, string? pm_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/google_workspace/
    /// </summary>
    Task<object?> ProviderGoogleWorkspaceCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderGoogleWorkspaceRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderGoogleWorkspaceUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderGoogleWorkspacePartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderGoogleWorkspaceDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderGoogleWorkspaceUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/
    /// </summary>
    Task<PaginatedResult<object>> ProviderMicrosoftEntraListAsync(string? expression = null, string? managed = null, string? pm_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/microsoft_entra/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderMicrosoftEntraUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/rac/
    /// </summary>
    Task<PaginatedResult<object>> ProviderRacListAsync(string? managed = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/rac/
    /// </summary>
    Task<object?> ProviderRacCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRacRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRacUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRacPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRacDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/rac/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderRacUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/radius/
    /// </summary>
    Task<PaginatedResult<object>> ProviderRadiusListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/radius/
    /// </summary>
    Task<object?> ProviderRadiusCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRadiusRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRadiusUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRadiusPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderRadiusDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/radius/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderRadiusUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/saml/
    /// </summary>
    Task<PaginatedResult<object>> ProviderSamlListAsync(string? friendly_name = null, string? managed = null, bool? managed__isnull = null, string? saml_name = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/saml/
    /// </summary>
    Task<object?> ProviderSamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderSamlRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderSamlUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderSamlPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderSamlDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/saml/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderSamlUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scim/
    /// </summary>
    Task<PaginatedResult<object>> ProviderScimListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/scim/
    /// </summary>
    Task<object?> ProviderScimCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScimRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScimUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScimPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScimDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scim/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderScimUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scope/
    /// </summary>
    Task<PaginatedResult<object>> ProviderScopeListAsync(string? managed = null, bool? managed__isnull = null, string? scope_name = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/provider/scope/
    /// </summary>
    Task<object?> ProviderScopeCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScopeRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScopeUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScopePartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    Task<object?> ProviderScopeDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/provider/scope/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> ProviderScopeUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/kerberos/
    /// </summary>
    Task<PaginatedResult<object>> SourceKerberosListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/kerberos/
    /// </summary>
    Task<object?> SourceKerberosCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    Task<object?> SourceKerberosRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    Task<object?> SourceKerberosUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    Task<object?> SourceKerberosPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    Task<object?> SourceKerberosDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/kerberos/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceKerberosUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/ldap/
    /// </summary>
    Task<PaginatedResult<object>> SourceLdapListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/ldap/
    /// </summary>
    Task<object?> SourceLdapCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    Task<object?> SourceLdapRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    Task<object?> SourceLdapUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    Task<object?> SourceLdapPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    Task<object?> SourceLdapDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/ldap/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceLdapUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/oauth/
    /// </summary>
    Task<PaginatedResult<object>> SourceOauthListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/oauth/
    /// </summary>
    Task<object?> SourceOauthCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    Task<object?> SourceOauthRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    Task<object?> SourceOauthUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    Task<object?> SourceOauthPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    Task<object?> SourceOauthDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/oauth/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceOauthUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/plex/
    /// </summary>
    Task<PaginatedResult<object>> SourcePlexListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/plex/
    /// </summary>
    Task<object?> SourcePlexCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    Task<object?> SourcePlexRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    Task<object?> SourcePlexUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    Task<object?> SourcePlexPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    Task<object?> SourcePlexDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/plex/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourcePlexUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/saml/
    /// </summary>
    Task<PaginatedResult<object>> SourceSamlListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/saml/
    /// </summary>
    Task<object?> SourceSamlCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    Task<object?> SourceSamlRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    Task<object?> SourceSamlUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    Task<object?> SourceSamlPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    Task<object?> SourceSamlDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/saml/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceSamlUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/scim/
    /// </summary>
    Task<PaginatedResult<object>> SourceScimListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/scim/
    /// </summary>
    Task<object?> SourceScimCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    Task<object?> SourceScimRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    Task<object?> SourceScimUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    Task<object?> SourceScimPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    Task<object?> SourceScimDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/scim/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceScimUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/telegram/
    /// </summary>
    Task<PaginatedResult<object>> SourceTelegramListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /propertymappings/source/telegram/
    /// </summary>
    Task<object?> SourceTelegramCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    Task<object?> SourceTelegramRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    Task<object?> SourceTelegramUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    Task<object?> SourceTelegramPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    Task<object?> SourceTelegramDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /propertymappings/source/telegram/{pm_uuid}/used_by/
    /// </summary>
    Task<object?> SourceTelegramUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default);

}
