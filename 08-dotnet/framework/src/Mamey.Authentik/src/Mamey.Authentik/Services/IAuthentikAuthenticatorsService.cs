using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Authenticators API operations.
/// </summary>
public interface IAuthentikAuthenticatorsService
{
    /// <summary>
    /// GET /authenticators/admin/all/
    /// </summary>
    Task<PaginatedResult<object>> AdminAllListAsync(int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/duo/
    /// </summary>
    Task<PaginatedResult<object>> AdminDuoListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/duo/
    /// </summary>
    Task<object?> AdminDuoCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/duo/{id}/
    /// </summary>
    Task<object?> AdminDuoRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/duo/{id}/
    /// </summary>
    Task<object?> AdminDuoUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/duo/{id}/
    /// </summary>
    Task<object?> AdminDuoPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/duo/{id}/
    /// </summary>
    Task<object?> AdminDuoDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/email/
    /// </summary>
    Task<PaginatedResult<object>> AdminEmailListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/email/
    /// </summary>
    Task<object?> AdminEmailCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/email/{id}/
    /// </summary>
    Task<object?> AdminEmailRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/email/{id}/
    /// </summary>
    Task<object?> AdminEmailUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/email/{id}/
    /// </summary>
    Task<object?> AdminEmailPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/email/{id}/
    /// </summary>
    Task<object?> AdminEmailDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/endpoint/
    /// </summary>
    Task<PaginatedResult<object>> AdminEndpointListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/endpoint/
    /// </summary>
    Task<object?> AdminEndpointCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    Task<object?> AdminEndpointRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    Task<object?> AdminEndpointUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    Task<object?> AdminEndpointPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    Task<object?> AdminEndpointDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/sms/
    /// </summary>
    Task<PaginatedResult<object>> AdminSmsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/sms/
    /// </summary>
    Task<object?> AdminSmsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/sms/{id}/
    /// </summary>
    Task<object?> AdminSmsRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/sms/{id}/
    /// </summary>
    Task<object?> AdminSmsUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/sms/{id}/
    /// </summary>
    Task<object?> AdminSmsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/sms/{id}/
    /// </summary>
    Task<object?> AdminSmsDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/static/
    /// </summary>
    Task<PaginatedResult<object>> AdminStaticListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/static/
    /// </summary>
    Task<object?> AdminStaticCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/static/{id}/
    /// </summary>
    Task<object?> AdminStaticRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/static/{id}/
    /// </summary>
    Task<object?> AdminStaticUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/static/{id}/
    /// </summary>
    Task<object?> AdminStaticPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/static/{id}/
    /// </summary>
    Task<object?> AdminStaticDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/totp/
    /// </summary>
    Task<PaginatedResult<object>> AdminTotpListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/totp/
    /// </summary>
    Task<object?> AdminTotpCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/totp/{id}/
    /// </summary>
    Task<object?> AdminTotpRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/totp/{id}/
    /// </summary>
    Task<object?> AdminTotpUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/totp/{id}/
    /// </summary>
    Task<object?> AdminTotpPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/totp/{id}/
    /// </summary>
    Task<object?> AdminTotpDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/webauthn/
    /// </summary>
    Task<PaginatedResult<object>> AdminWebauthnListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /authenticators/admin/webauthn/
    /// </summary>
    Task<object?> AdminWebauthnCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/admin/webauthn/{id}/
    /// </summary>
    Task<object?> AdminWebauthnRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/admin/webauthn/{id}/
    /// </summary>
    Task<object?> AdminWebauthnUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/admin/webauthn/{id}/
    /// </summary>
    Task<object?> AdminWebauthnPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/admin/webauthn/{id}/
    /// </summary>
    Task<object?> AdminWebauthnDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/duo/
    /// </summary>
    Task<PaginatedResult<object>> DuoListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/duo/{id}/
    /// </summary>
    Task<object?> DuoRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/duo/{id}/
    /// </summary>
    Task<object?> DuoUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/duo/{id}/
    /// </summary>
    Task<object?> DuoPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/duo/{id}/
    /// </summary>
    Task<object?> DuoDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/duo/{id}/used_by/
    /// </summary>
    Task<object?> DuoUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/email/
    /// </summary>
    Task<PaginatedResult<object>> EmailListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/email/{id}/
    /// </summary>
    Task<object?> EmailRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/email/{id}/
    /// </summary>
    Task<object?> EmailUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/email/{id}/
    /// </summary>
    Task<object?> EmailPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/email/{id}/
    /// </summary>
    Task<object?> EmailDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/email/{id}/used_by/
    /// </summary>
    Task<object?> EmailUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/endpoint/
    /// </summary>
    Task<PaginatedResult<object>> EndpointListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/endpoint/{uuid}/
    /// </summary>
    Task<object?> EndpointRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/endpoint/{uuid}/used_by/
    /// </summary>
    Task<object?> EndpointUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/sms/
    /// </summary>
    Task<PaginatedResult<object>> SmsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/sms/{id}/
    /// </summary>
    Task<object?> SmsRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/sms/{id}/
    /// </summary>
    Task<object?> SmsUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/sms/{id}/
    /// </summary>
    Task<object?> SmsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/sms/{id}/
    /// </summary>
    Task<object?> SmsDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/sms/{id}/used_by/
    /// </summary>
    Task<object?> SmsUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/static/
    /// </summary>
    Task<PaginatedResult<object>> StaticListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/static/{id}/
    /// </summary>
    Task<object?> StaticRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/static/{id}/
    /// </summary>
    Task<object?> StaticUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/static/{id}/
    /// </summary>
    Task<object?> StaticPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/static/{id}/
    /// </summary>
    Task<object?> StaticDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/static/{id}/used_by/
    /// </summary>
    Task<object?> StaticUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/totp/
    /// </summary>
    Task<PaginatedResult<object>> TotpListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/totp/{id}/
    /// </summary>
    Task<object?> TotpRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/totp/{id}/
    /// </summary>
    Task<object?> TotpUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/totp/{id}/
    /// </summary>
    Task<object?> TotpPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/totp/{id}/
    /// </summary>
    Task<object?> TotpDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/totp/{id}/used_by/
    /// </summary>
    Task<object?> TotpUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/webauthn/
    /// </summary>
    Task<PaginatedResult<object>> WebauthnListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/webauthn/{id}/
    /// </summary>
    Task<object?> WebauthnRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /authenticators/webauthn/{id}/
    /// </summary>
    Task<object?> WebauthnUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /authenticators/webauthn/{id}/
    /// </summary>
    Task<object?> WebauthnPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /authenticators/webauthn/{id}/
    /// </summary>
    Task<object?> WebauthnDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /authenticators/webauthn/{id}/used_by/
    /// </summary>
    Task<object?> WebauthnUsedByListAsync(int id, CancellationToken cancellationToken = default);

}
