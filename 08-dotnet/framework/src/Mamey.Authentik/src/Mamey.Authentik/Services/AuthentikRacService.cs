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
/// Service implementation for Authentik Rac API operations.
/// </summary>
public class AuthentikRacService : IAuthentikRacService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikRacService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikRacService"/> class.
    /// </summary>
    public AuthentikRacService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikRacService> logger,
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
    /// GET /rac/connection_tokens/
    /// </summary>
    public async Task<PaginatedResult<object>> ConnectionTokensListAsync(string? endpoint = null, int? provider = null, int? session__user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f3dca0e2 = $"api/v3/rac/connection_tokens/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(endpoint)) queryParams.Add($"endpoint={endpoint}");
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (session__user.HasValue) queryParams.Add($"session__user={session__user}");
        if (queryParams.Any()) url_f3dca0e2 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f3dca0e2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f3dca0e2 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f3dca0e2 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    public async Task<object?> ConnectionTokensRetrieveAsync(string connection_token_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_60a7b64c = $"api/v3/rac/connection_tokens/{connection_token_uuid}/";
        var response = await client.GetAsync(url_60a7b64c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_60a7b64c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_60a7b64c;
    }

    /// <summary>
    /// PUT /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    public async Task<object?> ConnectionTokensUpdateAsync(string connection_token_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4296b1d7 = $"api/v3/rac/connection_tokens/{connection_token_uuid}/";
        var response = await client.PutAsJsonAsync(url_4296b1d7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4296b1d7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4296b1d7;
    }

    /// <summary>
    /// PATCH /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    public async Task<object?> ConnectionTokensPartialUpdateAsync(string connection_token_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a674c7c8 = $"api/v3/rac/connection_tokens/{connection_token_uuid}/";
        var response = await client.PatchAsJsonAsync(url_a674c7c8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a674c7c8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a674c7c8;
    }

    /// <summary>
    /// DELETE /rac/connection_tokens/{connection_token_uuid}/
    /// </summary>
    public async Task<object?> ConnectionTokensDestroyAsync(string connection_token_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0b81ba4d = $"api/v3/rac/connection_tokens/{connection_token_uuid}/";
        var response = await client.DeleteAsync(url_0b81ba4d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0b81ba4d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0b81ba4d;
    }

    /// <summary>
    /// GET /rac/connection_tokens/{connection_token_uuid}/used_by/
    /// </summary>
    public async Task<object?> ConnectionTokensUsedByListAsync(string connection_token_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_647c74ad = $"api/v3/rac/connection_tokens/{connection_token_uuid}/used_by/";
        var response = await client.GetAsync(url_647c74ad, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_647c74ad = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_647c74ad;
    }

    /// <summary>
    /// GET /rac/endpoints/
    /// </summary>
    public async Task<PaginatedResult<object>> EndpointsListAsync(int? provider = null, bool? superuser_full_list = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ae3e23e7 = $"api/v3/rac/endpoints/";
        var queryParams = new List<string>();
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (superuser_full_list.HasValue) queryParams.Add($"superuser_full_list={superuser_full_list.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_ae3e23e7 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_ae3e23e7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ae3e23e7 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ae3e23e7 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /rac/endpoints/
    /// </summary>
    public async Task<object?> EndpointsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e298c4d9 = $"api/v3/rac/endpoints/";
        var response = await client.PostAsJsonAsync(url_e298c4d9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e298c4d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e298c4d9;
    }

    /// <summary>
    /// GET /rac/endpoints/{pbm_uuid}/
    /// </summary>
    public async Task<object?> EndpointsRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0f70a6ea = $"api/v3/rac/endpoints/{pbm_uuid}/";
        var response = await client.GetAsync(url_0f70a6ea, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0f70a6ea = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0f70a6ea;
    }

    /// <summary>
    /// PUT /rac/endpoints/{pbm_uuid}/
    /// </summary>
    public async Task<object?> EndpointsUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0a26af9c = $"api/v3/rac/endpoints/{pbm_uuid}/";
        var response = await client.PutAsJsonAsync(url_0a26af9c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0a26af9c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0a26af9c;
    }

    /// <summary>
    /// PATCH /rac/endpoints/{pbm_uuid}/
    /// </summary>
    public async Task<object?> EndpointsPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7e85bd3a = $"api/v3/rac/endpoints/{pbm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_7e85bd3a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7e85bd3a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7e85bd3a;
    }

    /// <summary>
    /// DELETE /rac/endpoints/{pbm_uuid}/
    /// </summary>
    public async Task<object?> EndpointsDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ed9ffbb0 = $"api/v3/rac/endpoints/{pbm_uuid}/";
        var response = await client.DeleteAsync(url_ed9ffbb0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ed9ffbb0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ed9ffbb0;
    }

    /// <summary>
    /// GET /rac/endpoints/{pbm_uuid}/used_by/
    /// </summary>
    public async Task<object?> EndpointsUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_85bde622 = $"api/v3/rac/endpoints/{pbm_uuid}/used_by/";
        var response = await client.GetAsync(url_85bde622, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_85bde622 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_85bde622;
    }

}
