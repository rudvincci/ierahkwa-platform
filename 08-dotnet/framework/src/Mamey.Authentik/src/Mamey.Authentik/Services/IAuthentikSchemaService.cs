using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Schema API operations.
/// </summary>
public interface IAuthentikSchemaService
{
    /// <summary>
    /// GET /schema/
    /// </summary>
    Task<PaginatedResult<object>> RetrieveAsync(string? format = null, string? lang = null, CancellationToken cancellationToken = default);

}
