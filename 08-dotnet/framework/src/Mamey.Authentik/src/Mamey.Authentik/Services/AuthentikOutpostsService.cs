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
/// Service implementation for Authentik Outposts API operations.
/// </summary>
public class AuthentikOutpostsService : IAuthentikOutpostsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikOutpostsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikOutpostsService"/> class.
    /// </summary>
    public AuthentikOutpostsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikOutpostsService> logger,
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
    /// GET /outposts/instances/
    /// </summary>
    public async Task<PaginatedResult<object>> InstancesListAsync(string? managed__icontains = null, string? managed__iexact = null, string? name__icontains = null, string? name__iexact = null, bool? providers__isnull = null, string? providers_by_pk = null, string? service_connection__name__icontains = null, string? service_connection__name__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d334d19e = $"api/v3/outposts/instances/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed__icontains)) queryParams.Add($"managed__icontains={managed__icontains}");
        if (!string.IsNullOrEmpty(managed__iexact)) queryParams.Add($"managed__iexact={managed__iexact}");
        if (!string.IsNullOrEmpty(name__icontains)) queryParams.Add($"name__icontains={name__icontains}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (providers__isnull.HasValue) queryParams.Add($"providers__isnull={providers__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(providers_by_pk)) queryParams.Add($"providers_by_pk={providers_by_pk}");
        if (!string.IsNullOrEmpty(service_connection__name__icontains)) queryParams.Add($"service_connection__name__icontains={service_connection__name__icontains}");
        if (!string.IsNullOrEmpty(service_connection__name__iexact)) queryParams.Add($"service_connection__name__iexact={service_connection__name__iexact}");
        if (queryParams.Any()) url_d334d19e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d334d19e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d334d19e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d334d19e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /outposts/instances/
    /// </summary>
    public async Task<object?> InstancesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e0afd076 = $"api/v3/outposts/instances/";
        var response = await client.PostAsJsonAsync(url_e0afd076, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e0afd076 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e0afd076;
    }

    /// <summary>
    /// GET /outposts/instances/{uuid}/
    /// </summary>
    public async Task<object?> InstancesRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_754669ad = $"api/v3/outposts/instances/{uuid}/";
        var response = await client.GetAsync(url_754669ad, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_754669ad = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_754669ad;
    }

    /// <summary>
    /// PUT /outposts/instances/{uuid}/
    /// </summary>
    public async Task<object?> InstancesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3c3f77f2 = $"api/v3/outposts/instances/{uuid}/";
        var response = await client.PutAsJsonAsync(url_3c3f77f2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3c3f77f2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3c3f77f2;
    }

    /// <summary>
    /// PATCH /outposts/instances/{uuid}/
    /// </summary>
    public async Task<object?> InstancesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_64ff2f0f = $"api/v3/outposts/instances/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_64ff2f0f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_64ff2f0f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_64ff2f0f;
    }

    /// <summary>
    /// DELETE /outposts/instances/{uuid}/
    /// </summary>
    public async Task<object?> InstancesDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7414215f = $"api/v3/outposts/instances/{uuid}/";
        var response = await client.DeleteAsync(url_7414215f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7414215f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7414215f;
    }

    /// <summary>
    /// GET /outposts/instances/{uuid}/health/
    /// </summary>
    public async Task<object?> InstancesHealthListAsync(string uuid, string? managed__icontains = null, string? managed__iexact = null, string? name__icontains = null, string? name__iexact = null, bool? providers__isnull = null, string? providers_by_pk = null, string? service_connection__name__icontains = null, string? service_connection__name__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f98e392a = $"api/v3/outposts/instances/{uuid}/health/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed__icontains)) queryParams.Add($"managed__icontains={managed__icontains}");
        if (!string.IsNullOrEmpty(managed__iexact)) queryParams.Add($"managed__iexact={managed__iexact}");
        if (!string.IsNullOrEmpty(name__icontains)) queryParams.Add($"name__icontains={name__icontains}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (providers__isnull.HasValue) queryParams.Add($"providers__isnull={providers__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(providers_by_pk)) queryParams.Add($"providers_by_pk={providers_by_pk}");
        if (!string.IsNullOrEmpty(service_connection__name__icontains)) queryParams.Add($"service_connection__name__icontains={service_connection__name__icontains}");
        if (!string.IsNullOrEmpty(service_connection__name__iexact)) queryParams.Add($"service_connection__name__iexact={service_connection__name__iexact}");
        if (queryParams.Any()) url_f98e392a += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f98e392a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f98e392a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f98e392a;
    }

    /// <summary>
    /// GET /outposts/instances/{uuid}/used_by/
    /// </summary>
    public async Task<object?> InstancesUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d39d5b7e = $"api/v3/outposts/instances/{uuid}/used_by/";
        var response = await client.GetAsync(url_d39d5b7e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d39d5b7e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d39d5b7e;
    }

    /// <summary>
    /// GET /outposts/instances/default_settings/
    /// </summary>
    public async Task<PaginatedResult<object>> InstancesDefaultSettingsRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2a2d0f03 = $"api/v3/outposts/instances/default_settings/";
        var response = await client.GetAsync(url_2a2d0f03, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2a2d0f03 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2a2d0f03 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> LdapListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b4c9b420 = $"api/v3/outposts/ldap/";
        var response = await client.GetAsync(url_b4c9b420, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b4c9b420 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b4c9b420 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/ldap/{id}/check_access/
    /// </summary>
    public async Task<object?> LdapAccessCheckAsync(int id, string? app_slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b6de4524 = $"api/v3/outposts/ldap/{id}/check_access/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(app_slug)) queryParams.Add($"app_slug={app_slug}");
        if (queryParams.Any()) url_b6de4524 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b6de4524, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b6de4524 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b6de4524;
    }

    /// <summary>
    /// GET /outposts/proxy/
    /// </summary>
    public async Task<PaginatedResult<object>> ProxyListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_93ef65ce = $"api/v3/outposts/proxy/";
        var response = await client.GetAsync(url_93ef65ce, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_93ef65ce = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_93ef65ce ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/radius/
    /// </summary>
    public async Task<PaginatedResult<object>> RadiusListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8611aa59 = $"api/v3/outposts/radius/";
        var response = await client.GetAsync(url_8611aa59, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8611aa59 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8611aa59 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/radius/{id}/check_access/
    /// </summary>
    public async Task<object?> RadiusAccessCheckAsync(int id, string? app_slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_976a0569 = $"api/v3/outposts/radius/{id}/check_access/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(app_slug)) queryParams.Add($"app_slug={app_slug}");
        if (queryParams.Any()) url_976a0569 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_976a0569, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_976a0569 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_976a0569;
    }

    /// <summary>
    /// GET /outposts/service_connections/all/
    /// </summary>
    public async Task<PaginatedResult<object>> ServiceConnectionsAllListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7f5be96b = $"api/v3/outposts/service_connections/all/";
        var response = await client.GetAsync(url_7f5be96b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7f5be96b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7f5be96b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsAllRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9df11d2c = $"api/v3/outposts/service_connections/all/{uuid}/";
        var response = await client.GetAsync(url_9df11d2c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9df11d2c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9df11d2c;
    }

    /// <summary>
    /// DELETE /outposts/service_connections/all/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsAllDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c5f3b550 = $"api/v3/outposts/service_connections/all/{uuid}/";
        var response = await client.DeleteAsync(url_c5f3b550, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c5f3b550 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c5f3b550;
    }

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/state/
    /// </summary>
    public async Task<object?> ServiceConnectionsAllStateRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ac1ca911 = $"api/v3/outposts/service_connections/all/{uuid}/state/";
        var response = await client.GetAsync(url_ac1ca911, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ac1ca911 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ac1ca911;
    }

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/used_by/
    /// </summary>
    public async Task<object?> ServiceConnectionsAllUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a8a6e07f = $"api/v3/outposts/service_connections/all/{uuid}/used_by/";
        var response = await client.GetAsync(url_a8a6e07f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a8a6e07f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a8a6e07f;
    }

    /// <summary>
    /// GET /outposts/service_connections/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> ServiceConnectionsAllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4f8d6eb3 = $"api/v3/outposts/service_connections/all/types/";
        var response = await client.GetAsync(url_4f8d6eb3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4f8d6eb3 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4f8d6eb3 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /outposts/service_connections/docker/
    /// </summary>
    public async Task<PaginatedResult<object>> ServiceConnectionsDockerListAsync(bool? local = null, string? tls_authentication = null, string? tls_verification = null, string? url = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bba0db80 = $"api/v3/outposts/service_connections/docker/";
        var queryParams = new List<string>();
        if (local.HasValue) queryParams.Add($"local={local.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(tls_authentication)) queryParams.Add($"tls_authentication={tls_authentication}");
        if (!string.IsNullOrEmpty(tls_verification)) queryParams.Add($"tls_verification={tls_verification}");
        if (!string.IsNullOrEmpty(url)) queryParams.Add($"url={url}");
        if (queryParams.Any()) url_bba0db80 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_bba0db80, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bba0db80 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bba0db80 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /outposts/service_connections/docker/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0fb84f32 = $"api/v3/outposts/service_connections/docker/";
        var response = await client.PostAsJsonAsync(url_0fb84f32, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0fb84f32 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0fb84f32;
    }

    /// <summary>
    /// GET /outposts/service_connections/docker/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ef98d578 = $"api/v3/outposts/service_connections/docker/{uuid}/";
        var response = await client.GetAsync(url_ef98d578, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ef98d578 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ef98d578;
    }

    /// <summary>
    /// PUT /outposts/service_connections/docker/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_72033311 = $"api/v3/outposts/service_connections/docker/{uuid}/";
        var response = await client.PutAsJsonAsync(url_72033311, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_72033311 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_72033311;
    }

    /// <summary>
    /// PATCH /outposts/service_connections/docker/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d1920003 = $"api/v3/outposts/service_connections/docker/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_d1920003, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d1920003 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d1920003;
    }

    /// <summary>
    /// DELETE /outposts/service_connections/docker/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b663ddb1 = $"api/v3/outposts/service_connections/docker/{uuid}/";
        var response = await client.DeleteAsync(url_b663ddb1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b663ddb1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b663ddb1;
    }

    /// <summary>
    /// GET /outposts/service_connections/docker/{uuid}/used_by/
    /// </summary>
    public async Task<object?> ServiceConnectionsDockerUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_049ca116 = $"api/v3/outposts/service_connections/docker/{uuid}/used_by/";
        var response = await client.GetAsync(url_049ca116, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_049ca116 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_049ca116;
    }

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/
    /// </summary>
    public async Task<PaginatedResult<object>> ServiceConnectionsKubernetesListAsync(bool? local = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1fffb973 = $"api/v3/outposts/service_connections/kubernetes/";
        var queryParams = new List<string>();
        if (local.HasValue) queryParams.Add($"local={local.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_1fffb973 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_1fffb973, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1fffb973 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1fffb973 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /outposts/service_connections/kubernetes/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_72fc7ee3 = $"api/v3/outposts/service_connections/kubernetes/";
        var response = await client.PostAsJsonAsync(url_72fc7ee3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_72fc7ee3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_72fc7ee3;
    }

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f3538f02 = $"api/v3/outposts/service_connections/kubernetes/{uuid}/";
        var response = await client.GetAsync(url_f3538f02, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f3538f02 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f3538f02;
    }

    /// <summary>
    /// PUT /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_677d8dc9 = $"api/v3/outposts/service_connections/kubernetes/{uuid}/";
        var response = await client.PutAsJsonAsync(url_677d8dc9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_677d8dc9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_677d8dc9;
    }

    /// <summary>
    /// PATCH /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4fd38e3 = $"api/v3/outposts/service_connections/kubernetes/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_c4fd38e3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4fd38e3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4fd38e3;
    }

    /// <summary>
    /// DELETE /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_819edb8b = $"api/v3/outposts/service_connections/kubernetes/{uuid}/";
        var response = await client.DeleteAsync(url_819edb8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_819edb8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_819edb8b;
    }

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/{uuid}/used_by/
    /// </summary>
    public async Task<object?> ServiceConnectionsKubernetesUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c984236a = $"api/v3/outposts/service_connections/kubernetes/{uuid}/used_by/";
        var response = await client.GetAsync(url_c984236a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c984236a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c984236a;
    }

}
