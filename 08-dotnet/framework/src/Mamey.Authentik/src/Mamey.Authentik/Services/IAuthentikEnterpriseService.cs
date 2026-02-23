using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Enterprise API operations.
/// </summary>
public interface IAuthentikEnterpriseService
{
    /// <summary>
    /// GET /enterprise/license/
    /// </summary>
    Task<PaginatedResult<object>> LicenseListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /enterprise/license/
    /// </summary>
    Task<object?> LicenseCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /enterprise/license/{license_uuid}/
    /// </summary>
    Task<object?> LicenseRetrieveAsync(string license_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /enterprise/license/{license_uuid}/
    /// </summary>
    Task<object?> LicenseUpdateAsync(string license_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /enterprise/license/{license_uuid}/
    /// </summary>
    Task<object?> LicensePartialUpdateAsync(string license_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /enterprise/license/{license_uuid}/
    /// </summary>
    Task<object?> LicenseDestroyAsync(string license_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /enterprise/license/{license_uuid}/used_by/
    /// </summary>
    Task<object?> LicenseUsedByListAsync(string license_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /enterprise/license/forecast/
    /// </summary>
    Task<PaginatedResult<object>> LicenseForecastRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /enterprise/license/install_id/
    /// </summary>
    Task<PaginatedResult<object>> LicenseInstallIdRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /enterprise/license/summary/
    /// </summary>
    Task<PaginatedResult<object>> LicenseSummaryRetrieveAsync(bool? cached = null, CancellationToken cancellationToken = default);

}
