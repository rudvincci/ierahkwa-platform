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
/// Service implementation for Authentik Ssf API operations.
/// </summary>
public class AuthentikSsfService : IAuthentikSsfService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikSsfService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikSsfService"/> class.
    /// </summary>
    public AuthentikSsfService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikSsfService> logger,
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
    /// GET /ssf/streams/
    /// </summary>
    public async Task<PaginatedResult<object>> StreamsListAsync(string? delivery_method = null, string? endpoint_url = null, int? provider = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cb0f0c5c = $"api/v3/ssf/streams/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(delivery_method)) queryParams.Add($"delivery_method={delivery_method}");
        if (!string.IsNullOrEmpty(endpoint_url)) queryParams.Add($"endpoint_url={endpoint_url}");
        if (provider.HasValue) queryParams.Add($"provider={provider}");
        if (queryParams.Any()) url_cb0f0c5c += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_cb0f0c5c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cb0f0c5c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cb0f0c5c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /ssf/streams/{uuid}/
    /// </summary>
    public async Task<object?> StreamsRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9cb2a480 = $"api/v3/ssf/streams/{uuid}/";
        var response = await client.GetAsync(url_9cb2a480, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9cb2a480 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9cb2a480;
    }

}
