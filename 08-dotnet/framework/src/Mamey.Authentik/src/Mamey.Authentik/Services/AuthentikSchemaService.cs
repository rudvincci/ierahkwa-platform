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
/// Service implementation for Authentik Schema API operations.
/// </summary>
public class AuthentikSchemaService : IAuthentikSchemaService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikSchemaService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikSchemaService"/> class.
    /// </summary>
    public AuthentikSchemaService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikSchemaService> logger,
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
    /// GET /schema/
    /// </summary>
    public async Task<PaginatedResult<object>> RetrieveAsync(string? format = null, string? lang = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2d67a16b = $"api/v3/schema/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(format)) queryParams.Add($"format={format}");
        if (!string.IsNullOrEmpty(lang)) queryParams.Add($"lang={lang}");
        if (queryParams.Any()) url_2d67a16b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_2d67a16b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2d67a16b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2d67a16b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

}
