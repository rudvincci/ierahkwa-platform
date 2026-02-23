using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Authentik.Caching;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service implementation for Authentik Endpoints API operations.
/// </summary>
public class AuthentikEndpointsService : IAuthentikEndpointsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikEndpointsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikEndpointsService"/> class.
    /// </summary>
    public AuthentikEndpointsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikEndpointsService> logger,
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
}
