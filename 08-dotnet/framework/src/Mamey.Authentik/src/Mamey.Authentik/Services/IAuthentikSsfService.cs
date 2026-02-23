using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Ssf API operations.
/// </summary>
public interface IAuthentikSsfService
{
    /// <summary>
    /// GET /ssf/streams/
    /// </summary>
    Task<PaginatedResult<object>> StreamsListAsync(string? delivery_method = null, string? endpoint_url = null, int? provider = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /ssf/streams/{uuid}/
    /// </summary>
    Task<object?> StreamsRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

}
