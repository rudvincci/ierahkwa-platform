using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Crypto API operations.
/// </summary>
public interface IAuthentikCryptoService
{
    /// <summary>
    /// GET /crypto/certificatekeypairs/
    /// </summary>
    Task<PaginatedResult<object>> CertificatekeypairsListAsync(bool? has_key = null, bool? include_details = null, string? managed = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /crypto/certificatekeypairs/
    /// </summary>
    Task<object?> CertificatekeypairsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    Task<object?> CertificatekeypairsRetrieveAsync(string kp_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    Task<object?> CertificatekeypairsUpdateAsync(string kp_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    Task<object?> CertificatekeypairsPartialUpdateAsync(string kp_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /crypto/certificatekeypairs/{kp_uuid}/
    /// </summary>
    Task<object?> CertificatekeypairsDestroyAsync(string kp_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/used_by/
    /// </summary>
    Task<object?> CertificatekeypairsUsedByListAsync(string kp_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/view_certificate/
    /// </summary>
    Task<object?> CertificatekeypairsViewCertificateRetrieveAsync(string kp_uuid, bool? download = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /crypto/certificatekeypairs/{kp_uuid}/view_private_key/
    /// </summary>
    Task<object?> CertificatekeypairsViewPrivateKeyRetrieveAsync(string kp_uuid, bool? download = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /crypto/certificatekeypairs/generate/
    /// </summary>
    Task<object?> CertificatekeypairsGenerateCreateAsync(object request, CancellationToken cancellationToken = default);

}
