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
/// Service implementation for Authentik Root API operations.
/// </summary>
public class AuthentikRootService : IAuthentikRootService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikRootService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikRootService"/> class.
    /// </summary>
    public AuthentikRootService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikRootService> logger,
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
    /// GET /root/config/
    /// </summary>
    public async Task<PaginatedResult<object>> ConfigRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4af7509e = $"api/v3/root/config/";
        var response = await client.GetAsync(url_4af7509e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4af7509e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4af7509e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

}
