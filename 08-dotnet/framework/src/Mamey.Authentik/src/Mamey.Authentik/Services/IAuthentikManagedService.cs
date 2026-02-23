using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Managed API operations.
/// </summary>
public interface IAuthentikManagedService
{
    /// <summary>
    /// GET /managed/blueprints/
    /// </summary>
    Task<PaginatedResult<object>> BlueprintsListAsync(string? path = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /managed/blueprints/
    /// </summary>
    Task<object?> BlueprintsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /managed/blueprints/{instance_uuid}/
    /// </summary>
    Task<object?> BlueprintsRetrieveAsync(string instance_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /managed/blueprints/{instance_uuid}/
    /// </summary>
    Task<object?> BlueprintsUpdateAsync(string instance_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /managed/blueprints/{instance_uuid}/
    /// </summary>
    Task<object?> BlueprintsPartialUpdateAsync(string instance_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /managed/blueprints/{instance_uuid}/
    /// </summary>
    Task<object?> BlueprintsDestroyAsync(string instance_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /managed/blueprints/{instance_uuid}/apply/
    /// </summary>
    Task<object?> BlueprintsApplyCreateAsync(string instance_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /managed/blueprints/{instance_uuid}/used_by/
    /// </summary>
    Task<object?> BlueprintsUsedByListAsync(string instance_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /managed/blueprints/available/
    /// </summary>
    Task<PaginatedResult<object>> BlueprintsAvailableListAsync(CancellationToken cancellationToken = default);

}
