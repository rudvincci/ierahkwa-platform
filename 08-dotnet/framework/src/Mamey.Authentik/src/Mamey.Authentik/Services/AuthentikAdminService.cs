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
/// Service implementation for Authentik Admin API operations.
/// </summary>
public class AuthentikAdminService : IAuthentikAdminService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikAdminService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAdminService"/> class.
    /// </summary>
    public AuthentikAdminService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikAdminService> logger,
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
    /// GET /admin/apps/
    /// </summary>
    public async Task<PaginatedResult<object>> AppsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c0dcb13c = $"api/v3/admin/apps/";
        var response = await client.GetAsync(url_c0dcb13c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c0dcb13c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c0dcb13c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /admin/models/
    /// </summary>
    public async Task<PaginatedResult<object>> ModelsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e7d166b5 = $"api/v3/admin/models/";
        var response = await client.GetAsync(url_e7d166b5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e7d166b5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e7d166b5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /admin/settings/
    /// </summary>
    public async Task<PaginatedResult<object>> SettingsRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8b61ae42 = $"api/v3/admin/settings/";
        var response = await client.GetAsync(url_8b61ae42, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8b61ae42 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8b61ae42 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// PUT /admin/settings/
    /// </summary>
    public async Task<object?> SettingsUpdateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4bd8e0be = $"api/v3/admin/settings/";
        var response = await client.PutAsJsonAsync(url_4bd8e0be, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4bd8e0be = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4bd8e0be;
    }

    /// <summary>
    /// PATCH /admin/settings/
    /// </summary>
    public async Task<object?> SettingsPartialUpdateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3392d699 = $"api/v3/admin/settings/";
        var response = await client.PatchAsJsonAsync(url_3392d699, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3392d699 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3392d699;
    }

    /// <summary>
    /// GET /admin/system/
    /// </summary>
    public async Task<PaginatedResult<object>> SystemRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_46a9e575 = $"api/v3/admin/system/";
        var response = await client.GetAsync(url_46a9e575, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_46a9e575 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_46a9e575 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /admin/system/
    /// </summary>
    public async Task<object?> SystemCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4687390b = $"api/v3/admin/system/";
        var response = await client.PostAsync(url_4687390b, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4687390b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4687390b;
    }

    /// <summary>
    /// GET /admin/version/
    /// </summary>
    public async Task<PaginatedResult<object>> VersionRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e66eda84 = $"api/v3/admin/version/";
        var response = await client.GetAsync(url_e66eda84, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e66eda84 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e66eda84 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /admin/version/history/
    /// </summary>
    public async Task<PaginatedResult<object>> VersionHistoryListAsync(string? build = null, string? version = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b84b3cad = $"api/v3/admin/version/history/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(build)) queryParams.Add($"build={build}");
        if (!string.IsNullOrEmpty(version)) queryParams.Add($"version={version}");
        if (queryParams.Any()) url_b84b3cad += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b84b3cad, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b84b3cad = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b84b3cad ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /admin/version/history/{id}/
    /// </summary>
    public async Task<object?> VersionHistoryRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5e6460ae = $"api/v3/admin/version/history/{id}/";
        var response = await client.GetAsync(url_5e6460ae, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5e6460ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5e6460ae;
    }

}
