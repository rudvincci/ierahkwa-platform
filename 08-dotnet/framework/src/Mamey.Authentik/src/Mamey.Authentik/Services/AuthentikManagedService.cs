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
/// Service implementation for Authentik Managed API operations.
/// </summary>
public class AuthentikManagedService : IAuthentikManagedService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikManagedService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikManagedService"/> class.
    /// </summary>
    public AuthentikManagedService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikManagedService> logger,
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
    /// GET /managed/blueprints/
    /// </summary>
    public async Task<PaginatedResult<object>> BlueprintsListAsync(string? path = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e1fe3421 = $"api/v3/managed/blueprints/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(path)) queryParams.Add($"path={path}");
        if (queryParams.Any()) url_e1fe3421 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_e1fe3421, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e1fe3421 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e1fe3421 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /managed/blueprints/
    /// </summary>
    public async Task<object?> BlueprintsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75d9d444 = $"api/v3/managed/blueprints/";
        var response = await client.PostAsJsonAsync(url_75d9d444, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75d9d444 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75d9d444;
    }

    /// <summary>
    /// GET /managed/blueprints/{instance_uuid}/
    /// </summary>
    public async Task<object?> BlueprintsRetrieveAsync(string instance_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4173f898 = $"api/v3/managed/blueprints/{instance_uuid}/";
        var response = await client.GetAsync(url_4173f898, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4173f898 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4173f898;
    }

    /// <summary>
    /// PUT /managed/blueprints/{instance_uuid}/
    /// </summary>
    public async Task<object?> BlueprintsUpdateAsync(string instance_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bb1fd256 = $"api/v3/managed/blueprints/{instance_uuid}/";
        var response = await client.PutAsJsonAsync(url_bb1fd256, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bb1fd256 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bb1fd256;
    }

    /// <summary>
    /// PATCH /managed/blueprints/{instance_uuid}/
    /// </summary>
    public async Task<object?> BlueprintsPartialUpdateAsync(string instance_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bc933c96 = $"api/v3/managed/blueprints/{instance_uuid}/";
        var response = await client.PatchAsJsonAsync(url_bc933c96, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bc933c96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bc933c96;
    }

    /// <summary>
    /// DELETE /managed/blueprints/{instance_uuid}/
    /// </summary>
    public async Task<object?> BlueprintsDestroyAsync(string instance_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_49b8fa48 = $"api/v3/managed/blueprints/{instance_uuid}/";
        var response = await client.DeleteAsync(url_49b8fa48, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_49b8fa48 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_49b8fa48;
    }

    /// <summary>
    /// POST /managed/blueprints/{instance_uuid}/apply/
    /// </summary>
    public async Task<object?> BlueprintsApplyCreateAsync(string instance_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d24e45d2 = $"api/v3/managed/blueprints/{instance_uuid}/apply/";
        var response = await client.PostAsync(url_d24e45d2, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d24e45d2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d24e45d2;
    }

    /// <summary>
    /// GET /managed/blueprints/{instance_uuid}/used_by/
    /// </summary>
    public async Task<object?> BlueprintsUsedByListAsync(string instance_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4e06a890 = $"api/v3/managed/blueprints/{instance_uuid}/used_by/";
        var response = await client.GetAsync(url_4e06a890, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4e06a890 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4e06a890;
    }

    /// <summary>
    /// GET /managed/blueprints/available/
    /// </summary>
    public async Task<PaginatedResult<object>> BlueprintsAvailableListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e19b9517 = $"api/v3/managed/blueprints/available/";
        var response = await client.GetAsync(url_e19b9517, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e19b9517 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e19b9517 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

}
