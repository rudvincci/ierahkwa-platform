using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Stages API operations.
/// </summary>
public interface IAuthentikStagesService
{
    /// <summary>
    /// GET /stages/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/all/{stage_uuid}/
    /// </summary>
    Task<object?> AllRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/all/{stage_uuid}/
    /// </summary>
    Task<object?> AllDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/all/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AllUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/all/types/
    /// </summary>
    Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/all/user_settings/
    /// </summary>
    Task<PaginatedResult<object>> AllUserSettingsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/duo/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorDuoListAsync(string? api_hostname = null, string? client_id = null, string? configure_flow = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/duo/
    /// </summary>
    Task<object?> AuthenticatorDuoCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorDuoRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorDuoUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorDuoPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorDuoDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/enrollment_status/
    /// </summary>
    Task<object?> AuthenticatorDuoEnrollmentStatusCreateAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/import_device_manual/
    /// </summary>
    Task<object?> AuthenticatorDuoImportDeviceManualCreateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/import_devices_automatic/
    /// </summary>
    Task<object?> AuthenticatorDuoImportDevicesAutomaticCreateAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/duo/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorDuoUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/email/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorEmailListAsync(string? configure_flow = null, string? friendly_name = null, string? from_address = null, string? host = null, string? password = null, int? port = null, string? stage_uuid = null, string? subject = null, string? template = null, int? timeout = null, string? token_expiry = null, bool? use_global_settings = null, bool? use_ssl = null, bool? use_tls = null, string? username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/email/
    /// </summary>
    Task<object?> AuthenticatorEmailCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEmailRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEmailUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEmailPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEmailDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/email/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorEmailUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorEndpointGdtcListAsync(string? configure_flow = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/endpoint_gdtc/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorEndpointGdtcUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/sms/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorSmsListAsync(string? account_sid = null, string? auth = null, string? auth_password = null, string? auth_type = null, string? configure_flow = null, string? friendly_name = null, string? from_number = null, string? mapping = null, string? provider = null, string? stage_uuid = null, bool? verify_only = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/sms/
    /// </summary>
    Task<object?> AuthenticatorSmsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorSmsRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorSmsUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorSmsPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorSmsDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/sms/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorSmsUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/static/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorStaticListAsync(string? configure_flow = null, string? friendly_name = null, string? stage_uuid = null, int? token_count = null, int? token_length = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/static/
    /// </summary>
    Task<object?> AuthenticatorStaticCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorStaticRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorStaticUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorStaticPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorStaticDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/static/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorStaticUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/totp/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorTotpListAsync(string? configure_flow = null, string? digits = null, string? friendly_name = null, string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/totp/
    /// </summary>
    Task<object?> AuthenticatorTotpCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorTotpRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorTotpUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorTotpPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorTotpDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/totp/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorTotpUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/validate/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorValidateListAsync(string? configuration_stages = null, string? not_configured_action = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/validate/
    /// </summary>
    Task<object?> AuthenticatorValidateCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorValidateRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorValidateUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorValidatePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorValidateDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/validate/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorValidateUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/webauthn/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorWebauthnListAsync(string? authenticator_attachment = null, string? configure_flow = null, string? device_type_restrictions = null, string? friendly_name = null, int? max_attempts = null, string? resident_key_requirement = null, string? stage_uuid = null, string? user_verification = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/authenticator/webauthn/
    /// </summary>
    Task<object?> AuthenticatorWebauthnCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorWebauthnRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorWebauthnUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorWebauthnPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    Task<object?> AuthenticatorWebauthnDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/webauthn/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> AuthenticatorWebauthnUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/webauthn_device_types/
    /// </summary>
    Task<PaginatedResult<object>> AuthenticatorWebauthnDeviceTypesListAsync(string? aaguid = null, string? description = null, string? icon = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/authenticator/webauthn_device_types/{aaguid}/
    /// </summary>
    Task<object?> AuthenticatorWebauthnDeviceTypesRetrieveAsync(string aaguid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/captcha/
    /// </summary>
    Task<PaginatedResult<object>> CaptchaListAsync(string? public_key = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/captcha/
    /// </summary>
    Task<object?> CaptchaCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/captcha/{stage_uuid}/
    /// </summary>
    Task<object?> CaptchaRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/captcha/{stage_uuid}/
    /// </summary>
    Task<object?> CaptchaUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/captcha/{stage_uuid}/
    /// </summary>
    Task<object?> CaptchaPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/captcha/{stage_uuid}/
    /// </summary>
    Task<object?> CaptchaDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/captcha/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> CaptchaUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/consent/
    /// </summary>
    Task<PaginatedResult<object>> ConsentListAsync(string? consent_expire_in = null, string? mode = null, string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/consent/
    /// </summary>
    Task<object?> ConsentCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/consent/{stage_uuid}/
    /// </summary>
    Task<object?> ConsentRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/consent/{stage_uuid}/
    /// </summary>
    Task<object?> ConsentUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/consent/{stage_uuid}/
    /// </summary>
    Task<object?> ConsentPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/consent/{stage_uuid}/
    /// </summary>
    Task<object?> ConsentDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/consent/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> ConsentUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/deny/
    /// </summary>
    Task<PaginatedResult<object>> DenyListAsync(string? deny_message = null, string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/deny/
    /// </summary>
    Task<object?> DenyCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/deny/{stage_uuid}/
    /// </summary>
    Task<object?> DenyRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/deny/{stage_uuid}/
    /// </summary>
    Task<object?> DenyUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/deny/{stage_uuid}/
    /// </summary>
    Task<object?> DenyPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/deny/{stage_uuid}/
    /// </summary>
    Task<object?> DenyDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/deny/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> DenyUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/dummy/
    /// </summary>
    Task<PaginatedResult<object>> DummyListAsync(string? stage_uuid = null, bool? throw_error = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/dummy/
    /// </summary>
    Task<object?> DummyCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/dummy/{stage_uuid}/
    /// </summary>
    Task<object?> DummyRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/dummy/{stage_uuid}/
    /// </summary>
    Task<object?> DummyUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/dummy/{stage_uuid}/
    /// </summary>
    Task<object?> DummyPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/dummy/{stage_uuid}/
    /// </summary>
    Task<object?> DummyDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/dummy/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> DummyUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/email/
    /// </summary>
    Task<PaginatedResult<object>> EmailListAsync(bool? activate_user_on_success = null, string? from_address = null, string? host = null, int? port = null, string? subject = null, string? template = null, int? timeout = null, string? token_expiry = null, bool? use_global_settings = null, bool? use_ssl = null, bool? use_tls = null, string? username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/email/
    /// </summary>
    Task<object?> EmailCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/email/{stage_uuid}/
    /// </summary>
    Task<object?> EmailRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/email/{stage_uuid}/
    /// </summary>
    Task<object?> EmailUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/email/{stage_uuid}/
    /// </summary>
    Task<object?> EmailPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/email/{stage_uuid}/
    /// </summary>
    Task<object?> EmailDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/email/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> EmailUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/email/templates/
    /// </summary>
    Task<PaginatedResult<object>> EmailTemplatesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/identification/
    /// </summary>
    Task<PaginatedResult<object>> IdentificationListAsync(string? captcha_stage = null, bool? case_insensitive_matching = null, string? enrollment_flow = null, string? password_stage = null, string? passwordless_flow = null, string? recovery_flow = null, bool? show_matched_user = null, bool? show_source_labels = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/identification/
    /// </summary>
    Task<object?> IdentificationCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/identification/{stage_uuid}/
    /// </summary>
    Task<object?> IdentificationRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/identification/{stage_uuid}/
    /// </summary>
    Task<object?> IdentificationUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/identification/{stage_uuid}/
    /// </summary>
    Task<object?> IdentificationPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/identification/{stage_uuid}/
    /// </summary>
    Task<object?> IdentificationDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/identification/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> IdentificationUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/invitations/
    /// </summary>
    Task<PaginatedResult<object>> InvitationInvitationsListAsync(string? created_by__username = null, string? expires = null, string? flow__slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/invitation/invitations/
    /// </summary>
    Task<object?> InvitationInvitationsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    Task<object?> InvitationInvitationsRetrieveAsync(string invite_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    Task<object?> InvitationInvitationsUpdateAsync(string invite_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    Task<object?> InvitationInvitationsPartialUpdateAsync(string invite_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    Task<object?> InvitationInvitationsDestroyAsync(string invite_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/invitations/{invite_uuid}/used_by/
    /// </summary>
    Task<object?> InvitationInvitationsUsedByListAsync(string invite_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/stages/
    /// </summary>
    Task<PaginatedResult<object>> InvitationStagesListAsync(bool? continue_flow_without_invitation = null, bool? no_flows = null, string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/invitation/stages/
    /// </summary>
    Task<object?> InvitationStagesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    Task<object?> InvitationStagesRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    Task<object?> InvitationStagesUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    Task<object?> InvitationStagesPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    Task<object?> InvitationStagesDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/invitation/stages/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> InvitationStagesUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/mtls/
    /// </summary>
    Task<PaginatedResult<object>> MtlsListAsync(string? cert_attribute = null, string? certificate_authorities = null, string? mode = null, string? stage_uuid = null, string? user_attribute = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/mtls/
    /// </summary>
    Task<object?> MtlsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/mtls/{stage_uuid}/
    /// </summary>
    Task<object?> MtlsRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/mtls/{stage_uuid}/
    /// </summary>
    Task<object?> MtlsUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/mtls/{stage_uuid}/
    /// </summary>
    Task<object?> MtlsPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/mtls/{stage_uuid}/
    /// </summary>
    Task<object?> MtlsDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/mtls/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> MtlsUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/password/
    /// </summary>
    Task<PaginatedResult<object>> PasswordListAsync(bool? allow_show_password = null, string? configure_flow = null, int? failed_attempts_before_cancel = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/password/
    /// </summary>
    Task<object?> PasswordCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/password/{stage_uuid}/
    /// </summary>
    Task<object?> PasswordRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/password/{stage_uuid}/
    /// </summary>
    Task<object?> PasswordUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/password/{stage_uuid}/
    /// </summary>
    Task<object?> PasswordPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/password/{stage_uuid}/
    /// </summary>
    Task<object?> PasswordDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/password/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> PasswordUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/prompts/
    /// </summary>
    Task<PaginatedResult<object>> PromptPromptsListAsync(string? field_key = null, string? label = null, string? placeholder = null, string? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/prompt/prompts/
    /// </summary>
    Task<object?> PromptPromptsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    Task<object?> PromptPromptsRetrieveAsync(string prompt_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    Task<object?> PromptPromptsUpdateAsync(string prompt_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    Task<object?> PromptPromptsPartialUpdateAsync(string prompt_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    Task<object?> PromptPromptsDestroyAsync(string prompt_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/prompts/{prompt_uuid}/used_by/
    /// </summary>
    Task<object?> PromptPromptsUsedByListAsync(string prompt_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/prompt/prompts/preview/
    /// </summary>
    Task<object?> PromptPromptsPreviewCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/stages/
    /// </summary>
    Task<PaginatedResult<object>> PromptStagesListAsync(string? fields = null, string? stage_uuid = null, string? validation_policies = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/prompt/stages/
    /// </summary>
    Task<object?> PromptStagesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    Task<object?> PromptStagesRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    Task<object?> PromptStagesUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    Task<object?> PromptStagesPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    Task<object?> PromptStagesDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/prompt/stages/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> PromptStagesUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/redirect/
    /// </summary>
    Task<PaginatedResult<object>> RedirectListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/redirect/
    /// </summary>
    Task<object?> RedirectCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/redirect/{stage_uuid}/
    /// </summary>
    Task<object?> RedirectRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/redirect/{stage_uuid}/
    /// </summary>
    Task<object?> RedirectUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/redirect/{stage_uuid}/
    /// </summary>
    Task<object?> RedirectPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/redirect/{stage_uuid}/
    /// </summary>
    Task<object?> RedirectDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/redirect/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> RedirectUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/source/
    /// </summary>
    Task<PaginatedResult<object>> SourceListAsync(string? resume_timeout = null, string? source = null, string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/source/
    /// </summary>
    Task<object?> SourceCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/source/{stage_uuid}/
    /// </summary>
    Task<object?> SourceRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/source/{stage_uuid}/
    /// </summary>
    Task<object?> SourceUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/source/{stage_uuid}/
    /// </summary>
    Task<object?> SourcePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/source/{stage_uuid}/
    /// </summary>
    Task<object?> SourceDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/source/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> SourceUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_delete/
    /// </summary>
    Task<PaginatedResult<object>> UserDeleteListAsync(string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/user_delete/
    /// </summary>
    Task<object?> UserDeleteCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_delete/{stage_uuid}/
    /// </summary>
    Task<object?> UserDeleteRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/user_delete/{stage_uuid}/
    /// </summary>
    Task<object?> UserDeleteUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/user_delete/{stage_uuid}/
    /// </summary>
    Task<object?> UserDeletePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/user_delete/{stage_uuid}/
    /// </summary>
    Task<object?> UserDeleteDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_delete/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> UserDeleteUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_login/
    /// </summary>
    Task<PaginatedResult<object>> UserLoginListAsync(string? geoip_binding = null, string? network_binding = null, string? remember_device = null, string? remember_me_offset = null, string? session_duration = null, string? stage_uuid = null, bool? terminate_other_sessions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/user_login/
    /// </summary>
    Task<object?> UserLoginCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_login/{stage_uuid}/
    /// </summary>
    Task<object?> UserLoginRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/user_login/{stage_uuid}/
    /// </summary>
    Task<object?> UserLoginUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/user_login/{stage_uuid}/
    /// </summary>
    Task<object?> UserLoginPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/user_login/{stage_uuid}/
    /// </summary>
    Task<object?> UserLoginDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_login/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> UserLoginUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_logout/
    /// </summary>
    Task<PaginatedResult<object>> UserLogoutListAsync(string? stage_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/user_logout/
    /// </summary>
    Task<object?> UserLogoutCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_logout/{stage_uuid}/
    /// </summary>
    Task<object?> UserLogoutRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/user_logout/{stage_uuid}/
    /// </summary>
    Task<object?> UserLogoutUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/user_logout/{stage_uuid}/
    /// </summary>
    Task<object?> UserLogoutPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/user_logout/{stage_uuid}/
    /// </summary>
    Task<object?> UserLogoutDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_logout/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> UserLogoutUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_write/
    /// </summary>
    Task<PaginatedResult<object>> UserWriteListAsync(bool? create_users_as_inactive = null, string? create_users_group = null, string? stage_uuid = null, string? user_creation_mode = null, string? user_path_template = null, string? user_type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /stages/user_write/
    /// </summary>
    Task<object?> UserWriteCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_write/{stage_uuid}/
    /// </summary>
    Task<object?> UserWriteRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /stages/user_write/{stage_uuid}/
    /// </summary>
    Task<object?> UserWriteUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /stages/user_write/{stage_uuid}/
    /// </summary>
    Task<object?> UserWritePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /stages/user_write/{stage_uuid}/
    /// </summary>
    Task<object?> UserWriteDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /stages/user_write/{stage_uuid}/used_by/
    /// </summary>
    Task<object?> UserWriteUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default);

}
