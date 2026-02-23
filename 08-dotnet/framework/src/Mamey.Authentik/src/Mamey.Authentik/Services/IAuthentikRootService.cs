using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Root API operations.
/// </summary>
public interface IAuthentikRootService
{
    /// <summary>
    /// GET /root/config/
    /// </summary>
    Task<PaginatedResult<object>> ConfigRetrieveAsync(CancellationToken cancellationToken = default);

}
