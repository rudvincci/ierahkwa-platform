using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Admin API operations.
/// </summary>
public interface IAuthentikAdminService
{
    /// <summary>
    /// GET /admin/apps/
    /// </summary>
    Task<PaginatedResult<object>> AppsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/models/
    /// </summary>
    Task<PaginatedResult<object>> ModelsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/settings/
    /// </summary>
    Task<PaginatedResult<object>> SettingsRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /admin/settings/
    /// </summary>
    Task<object?> SettingsUpdateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /admin/settings/
    /// </summary>
    Task<object?> SettingsPartialUpdateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/system/
    /// </summary>
    Task<PaginatedResult<object>> SystemRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /admin/system/
    /// </summary>
    Task<object?> SystemCreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/version/
    /// </summary>
    Task<PaginatedResult<object>> VersionRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/version/history/
    /// </summary>
    Task<PaginatedResult<object>> VersionHistoryListAsync(string? build = null, string? version = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /admin/version/history/{id}/
    /// </summary>
    Task<object?> VersionHistoryRetrieveAsync(int id, CancellationToken cancellationToken = default);

}
