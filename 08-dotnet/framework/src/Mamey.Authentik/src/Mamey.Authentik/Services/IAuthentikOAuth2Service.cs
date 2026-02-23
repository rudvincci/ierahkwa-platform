using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik OAuth2 API operations.
/// </summary>
public interface IAuthentikOAuth2Service
{
    /// <summary>
    /// GET /oauth2/access_tokens/
    /// </summary>
    Task<PaginatedResult<object>> AccessTokensListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/access_tokens/{id}/
    /// </summary>
    Task<object?> AccessTokensRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /oauth2/access_tokens/{id}/
    /// </summary>
    Task<object?> AccessTokensDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/access_tokens/{id}/used_by/
    /// </summary>
    Task<object?> AccessTokensUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/authorization_codes/
    /// </summary>
    Task<PaginatedResult<object>> AuthorizationCodesListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/authorization_codes/{id}/
    /// </summary>
    Task<object?> AuthorizationCodesRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /oauth2/authorization_codes/{id}/
    /// </summary>
    Task<object?> AuthorizationCodesDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/authorization_codes/{id}/used_by/
    /// </summary>
    Task<object?> AuthorizationCodesUsedByListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/refresh_tokens/
    /// </summary>
    Task<PaginatedResult<object>> RefreshTokensListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/refresh_tokens/{id}/
    /// </summary>
    Task<object?> RefreshTokensRetrieveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /oauth2/refresh_tokens/{id}/
    /// </summary>
    Task<object?> RefreshTokensDestroyAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /oauth2/refresh_tokens/{id}/used_by/
    /// </summary>
    Task<object?> RefreshTokensUsedByListAsync(int id, CancellationToken cancellationToken = default);

}
