using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Rac API operations.
/// </summary>
public interface IAuthentikRacService
{
    /// <summary>
    /// GET /rac/connection_tokens/
    /// </summary>
    Task<PaginatedResult<object>> ConnectionTokensListAsync(string? endpoint = null, int? provider = null, int? session__user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    Task<object?> ConnectionTokensRetrieveAsync(string connection_token_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    Task<object?> ConnectionTokensUpdateAsync(string connection_token_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    Task<object?> ConnectionTokensPartialUpdateAsync(string connection_token_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    Task<object?> ConnectionTokensDestroyAsync(string connection_token_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rac/connection_tokens/{connection_token_uuid}/used_by/
    /// </summary>
    Task<object?> ConnectionTokensUsedByListAsync(string connection_token_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rac/endpoints/
    /// </summary>
    Task<PaginatedResult<object>> EndpointsListAsync(int? provider = null, bool? superuser_full_list = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /rac/endpoints/
    /// </summary>
    Task<object?> EndpointsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rac/endpoints/{pbm_uuid}/
    /// </summary>
    Task<object?> EndpointsRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /rac/endpoints/{pbm_uuid}/
    /// </summary>
    Task<object?> EndpointsUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /rac/endpoints/{pbm_uuid}/
    /// </summary>
    Task<object?> EndpointsPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /rac/endpoints/{pbm_uuid}/
    /// </summary>
    Task<object?> EndpointsDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /rac/endpoints/{pbm_uuid}/used_by/
    /// </summary>
    Task<object?> EndpointsUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default);

}
