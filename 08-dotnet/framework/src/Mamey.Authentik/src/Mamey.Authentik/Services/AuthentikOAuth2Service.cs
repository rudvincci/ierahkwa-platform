using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;
using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service implementation for Authentik OAuth2 API operations.
/// </summary>
public class AuthentikOAuth2Service : IAuthentikOAuth2Service
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikOAuth2Service> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikOAuth2Service"/> class.
    /// </summary>
    public AuthentikOAuth2Service(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikOAuth2Service> logger,
        IAuthentikCache? cache = null)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Gets the HTTP client for making requests.
    /// </summary>
    protected HttpClient GetHttpClient() => _httpClientFactory.CreateClient("Authentik");

    /// <summary>
    /// GET /oauth2/access_tokens/
    /// </summary>
    public async Task<PaginatedResult<object>> AccessTokensListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9a170ab1 = $"api/v3/oauth2/access_tokens/";
        var queryParams = new List<string>();
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_9a170ab1 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_9a170ab1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9a170ab1 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9a170ab1 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /oauth2/access_tokens/{id}/
    /// </summary>
    public async Task<object?> AccessTokensRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4cb2b4f7 = $"api/v3/oauth2/access_tokens/{id}/";
        var response = await client.GetAsync(url_4cb2b4f7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4cb2b4f7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4cb2b4f7;
    }

    /// <summary>
    /// DELETE /oauth2/access_tokens/{id}/
    /// </summary>
    public async Task<object?> AccessTokensDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_50756949 = $"api/v3/oauth2/access_tokens/{id}/";
        var response = await client.DeleteAsync(url_50756949, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_50756949 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_50756949;
    }

    /// <summary>
    /// GET /oauth2/access_tokens/{id}/used_by/
    /// </summary>
    public async Task<object?> AccessTokensUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8ab4e28d = $"api/v3/oauth2/access_tokens/{id}/used_by/";
        var response = await client.GetAsync(url_8ab4e28d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8ab4e28d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8ab4e28d;
    }

    /// <summary>
    /// GET /oauth2/authorization_codes/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthorizationCodesListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9adfe0c4 = $"api/v3/oauth2/authorization_codes/";
        var queryParams = new List<string>();
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_9adfe0c4 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_9adfe0c4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9adfe0c4 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9adfe0c4 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /oauth2/authorization_codes/{id}/
    /// </summary>
    public async Task<object?> AuthorizationCodesRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dbb2c403 = $"api/v3/oauth2/authorization_codes/{id}/";
        var response = await client.GetAsync(url_dbb2c403, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dbb2c403 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dbb2c403;
    }

    /// <summary>
    /// DELETE /oauth2/authorization_codes/{id}/
    /// </summary>
    public async Task<object?> AuthorizationCodesDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_80ad2824 = $"api/v3/oauth2/authorization_codes/{id}/";
        var response = await client.DeleteAsync(url_80ad2824, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_80ad2824 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_80ad2824;
    }

    /// <summary>
    /// GET /oauth2/authorization_codes/{id}/used_by/
    /// </summary>
    public async Task<object?> AuthorizationCodesUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_db7cb01e = $"api/v3/oauth2/authorization_codes/{id}/used_by/";
        var response = await client.GetAsync(url_db7cb01e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_db7cb01e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_db7cb01e;
    }

    /// <summary>
    /// GET /oauth2/refresh_tokens/
    /// </summary>
    public async Task<PaginatedResult<object>> RefreshTokensListAsync(int? provider = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_916d275c = $"api/v3/oauth2/refresh_tokens/";
        var queryParams = new List<string>();
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_916d275c += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_916d275c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_916d275c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_916d275c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /oauth2/refresh_tokens/{id}/
    /// </summary>
    public async Task<object?> RefreshTokensRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6ce374fc = $"api/v3/oauth2/refresh_tokens/{id}/";
        var response = await client.GetAsync(url_6ce374fc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6ce374fc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6ce374fc;
    }

    /// <summary>
    /// DELETE /oauth2/refresh_tokens/{id}/
    /// </summary>
    public async Task<object?> RefreshTokensDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ccdf6610 = $"api/v3/oauth2/refresh_tokens/{id}/";
        var response = await client.DeleteAsync(url_ccdf6610, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ccdf6610 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ccdf6610;
    }

    /// <summary>
    /// GET /oauth2/refresh_tokens/{id}/used_by/
    /// </summary>
    public async Task<object?> RefreshTokensUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7e4a7157 = $"api/v3/oauth2/refresh_tokens/{id}/used_by/";
        var response = await client.GetAsync(url_7e4a7157, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7e4a7157 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7e4a7157;
    }

}
