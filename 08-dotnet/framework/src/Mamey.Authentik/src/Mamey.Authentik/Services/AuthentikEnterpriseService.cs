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
/// Service implementation for Authentik Enterprise API operations.
/// </summary>
public class AuthentikEnterpriseService : IAuthentikEnterpriseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikEnterpriseService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikEnterpriseService"/> class.
    /// </summary>
    public AuthentikEnterpriseService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikEnterpriseService> logger,
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
    /// GET /enterprise/license/
    /// </summary>
    public async Task<PaginatedResult<object>> LicenseListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e259c4af = $"api/v3/enterprise/license/";
        var response = await client.GetAsync(url_e259c4af, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e259c4af = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e259c4af ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /enterprise/license/
    /// </summary>
    public async Task<object?> LicenseCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_92b5dc01 = $"api/v3/enterprise/license/";
        var response = await client.PostAsJsonAsync(url_92b5dc01, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_92b5dc01 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_92b5dc01;
    }

    /// <summary>
    /// GET /enterprise/license/{license_uuid}/
    /// </summary>
    public async Task<object?> LicenseRetrieveAsync(string license_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9d5bdc6a = $"api/v3/enterprise/license/{license_uuid}/";
        var response = await client.GetAsync(url_9d5bdc6a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9d5bdc6a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9d5bdc6a;
    }

    /// <summary>
    /// PUT /enterprise/license/{license_uuid}/
    /// </summary>
    public async Task<object?> LicenseUpdateAsync(string license_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9373011e = $"api/v3/enterprise/license/{license_uuid}/";
        var response = await client.PutAsJsonAsync(url_9373011e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9373011e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9373011e;
    }

    /// <summary>
    /// PATCH /enterprise/license/{license_uuid}/
    /// </summary>
    public async Task<object?> LicensePartialUpdateAsync(string license_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_77be50c2 = $"api/v3/enterprise/license/{license_uuid}/";
        var response = await client.PatchAsJsonAsync(url_77be50c2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_77be50c2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_77be50c2;
    }

    /// <summary>
    /// DELETE /enterprise/license/{license_uuid}/
    /// </summary>
    public async Task<object?> LicenseDestroyAsync(string license_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f19df7db = $"api/v3/enterprise/license/{license_uuid}/";
        var response = await client.DeleteAsync(url_f19df7db, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f19df7db = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f19df7db;
    }

    /// <summary>
    /// GET /enterprise/license/{license_uuid}/used_by/
    /// </summary>
    public async Task<object?> LicenseUsedByListAsync(string license_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_20d559ac = $"api/v3/enterprise/license/{license_uuid}/used_by/";
        var response = await client.GetAsync(url_20d559ac, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_20d559ac = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_20d559ac;
    }

    /// <summary>
    /// GET /enterprise/license/forecast/
    /// </summary>
    public async Task<PaginatedResult<object>> LicenseForecastRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_054a8032 = $"api/v3/enterprise/license/forecast/";
        var response = await client.GetAsync(url_054a8032, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_054a8032 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_054a8032 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /enterprise/license/install_id/
    /// </summary>
    public async Task<PaginatedResult<object>> LicenseInstallIdRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_72bbe5a4 = $"api/v3/enterprise/license/install_id/";
        var response = await client.GetAsync(url_72bbe5a4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_72bbe5a4 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_72bbe5a4 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /enterprise/license/summary/
    /// </summary>
    public async Task<PaginatedResult<object>> LicenseSummaryRetrieveAsync(bool? cached = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e97262b9 = $"api/v3/enterprise/license/summary/";
        var queryParams = new List<string>();
        if (cached.HasValue) queryParams.Add($"cached={cached.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_e97262b9 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_e97262b9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e97262b9 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e97262b9 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

}
