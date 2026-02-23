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
/// Service implementation for Authentik Flows API operations.
/// </summary>
public class AuthentikFlowsService : IAuthentikFlowsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikFlowsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikFlowsService"/> class.
    /// </summary>
    public AuthentikFlowsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikFlowsService> logger,
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
    /// GET /flows/bindings/
    /// </summary>
    public async Task<PaginatedResult<object>> BindingsListAsync(bool? evaluate_on_plan = null, string? fsb_uuid = null, string? invalid_response_action = null, int? order = null, string? pbm_uuid = null, string? policies = null, string? policy_engine_mode = null, bool? re_evaluate_policies = null, string? stage = null, string? target = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f5fcfdd5 = $"api/v3/flows/bindings/";
        var queryParams = new List<string>();
        if (evaluate_on_plan.HasValue) queryParams.Add($"evaluate_on_plan={evaluate_on_plan.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(fsb_uuid)) queryParams.Add($"fsb_uuid={fsb_uuid}");
        if (!string.IsNullOrEmpty(invalid_response_action)) queryParams.Add($"invalid_response_action={invalid_response_action}");
        if (order.HasValue) queryParams.Add($"order={order}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(policies)) queryParams.Add($"policies={policies}");
        if (!string.IsNullOrEmpty(policy_engine_mode)) queryParams.Add($"policy_engine_mode={policy_engine_mode}");
        if (re_evaluate_policies.HasValue) queryParams.Add($"re_evaluate_policies={re_evaluate_policies.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(stage)) queryParams.Add($"stage={stage}");
        if (!string.IsNullOrEmpty(target)) queryParams.Add($"target={target}");
        if (queryParams.Any()) url_f5fcfdd5 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f5fcfdd5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f5fcfdd5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f5fcfdd5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /flows/bindings/
    /// </summary>
    public async Task<object?> BindingsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_29f8cd67 = $"api/v3/flows/bindings/";
        var response = await client.PostAsJsonAsync(url_29f8cd67, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_29f8cd67 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_29f8cd67;
    }

    /// <summary>
    /// GET /flows/bindings/{fsb_uuid}/
    /// </summary>
    public async Task<object?> BindingsRetrieveAsync(string fsb_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c6749f16 = $"api/v3/flows/bindings/{fsb_uuid}/";
        var response = await client.GetAsync(url_c6749f16, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c6749f16 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c6749f16;
    }

    /// <summary>
    /// PUT /flows/bindings/{fsb_uuid}/
    /// </summary>
    public async Task<object?> BindingsUpdateAsync(string fsb_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e46d2fbb = $"api/v3/flows/bindings/{fsb_uuid}/";
        var response = await client.PutAsJsonAsync(url_e46d2fbb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e46d2fbb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e46d2fbb;
    }

    /// <summary>
    /// PATCH /flows/bindings/{fsb_uuid}/
    /// </summary>
    public async Task<object?> BindingsPartialUpdateAsync(string fsb_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_281f9186 = $"api/v3/flows/bindings/{fsb_uuid}/";
        var response = await client.PatchAsJsonAsync(url_281f9186, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_281f9186 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_281f9186;
    }

    /// <summary>
    /// DELETE /flows/bindings/{fsb_uuid}/
    /// </summary>
    public async Task<object?> BindingsDestroyAsync(string fsb_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_68702849 = $"api/v3/flows/bindings/{fsb_uuid}/";
        var response = await client.DeleteAsync(url_68702849, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_68702849 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_68702849;
    }

    /// <summary>
    /// GET /flows/bindings/{fsb_uuid}/used_by/
    /// </summary>
    public async Task<object?> BindingsUsedByListAsync(string fsb_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_af0e8757 = $"api/v3/flows/bindings/{fsb_uuid}/used_by/";
        var response = await client.GetAsync(url_af0e8757, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_af0e8757 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_af0e8757;
    }

    /// <summary>
    /// GET /flows/executor/{flow_slug}/
    /// </summary>
    public async Task<object?> ExecutorGetAsync(string flow_slug, string? query = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_88a7b84e = $"api/v3/flows/executor/{flow_slug}/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(query)) queryParams.Add($"query={query}");
        if (queryParams.Any()) url_88a7b84e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_88a7b84e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_88a7b84e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_88a7b84e;
    }

    /// <summary>
    /// POST /flows/executor/{flow_slug}/
    /// </summary>
    public async Task<object?> ExecutorSolveAsync(string flow_slug, object request, string? query = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9f14041f = $"api/v3/flows/executor/{flow_slug}/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(query)) queryParams.Add($"query={query}");
        if (queryParams.Any()) url_9f14041f += "?" + string.Join("&", queryParams);
        var response = await client.PostAsJsonAsync(url_9f14041f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9f14041f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9f14041f;
    }

    /// <summary>
    /// GET /flows/inspector/{flow_slug}/
    /// </summary>
    public async Task<object?> InspectorGetAsync(string flow_slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ea8d1771 = $"api/v3/flows/inspector/{flow_slug}/";
        var response = await client.GetAsync(url_ea8d1771, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ea8d1771 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ea8d1771;
    }

    /// <summary>
    /// GET /flows/instances/
    /// </summary>
    public async Task<PaginatedResult<object>> InstancesListAsync(string? denied_action = null, string? designation = null, string? flow_uuid = null, string? slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d334d19e = $"api/v3/flows/instances/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(denied_action)) queryParams.Add($"denied_action={denied_action}");
        if (!string.IsNullOrEmpty(designation)) queryParams.Add($"designation={designation}");
        if (!string.IsNullOrEmpty(flow_uuid)) queryParams.Add($"flow_uuid={flow_uuid}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (queryParams.Any()) url_d334d19e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d334d19e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d334d19e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d334d19e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /flows/instances/
    /// </summary>
    public async Task<object?> InstancesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e0afd076 = $"api/v3/flows/instances/";
        var response = await client.PostAsJsonAsync(url_e0afd076, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e0afd076 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e0afd076;
    }

    /// <summary>
    /// GET /flows/instances/{slug}/
    /// </summary>
    public async Task<object?> InstancesRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_754669ad = $"api/v3/flows/instances/{slug}/";
        var response = await client.GetAsync(url_754669ad, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_754669ad = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_754669ad;
    }

    /// <summary>
    /// PUT /flows/instances/{slug}/
    /// </summary>
    public async Task<object?> InstancesUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3c3f77f2 = $"api/v3/flows/instances/{slug}/";
        var response = await client.PutAsJsonAsync(url_3c3f77f2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3c3f77f2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3c3f77f2;
    }

    /// <summary>
    /// PATCH /flows/instances/{slug}/
    /// </summary>
    public async Task<object?> InstancesPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_64ff2f0f = $"api/v3/flows/instances/{slug}/";
        var response = await client.PatchAsJsonAsync(url_64ff2f0f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_64ff2f0f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_64ff2f0f;
    }

    /// <summary>
    /// DELETE /flows/instances/{slug}/
    /// </summary>
    public async Task<object?> InstancesDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7414215f = $"api/v3/flows/instances/{slug}/";
        var response = await client.DeleteAsync(url_7414215f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7414215f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7414215f;
    }

    /// <summary>
    /// GET /flows/instances/{slug}/diagram/
    /// </summary>
    public async Task<object?> InstancesDiagramRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9fe46be7 = $"api/v3/flows/instances/{slug}/diagram/";
        var response = await client.GetAsync(url_9fe46be7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9fe46be7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9fe46be7;
    }

    /// <summary>
    /// GET /flows/instances/{slug}/execute/
    /// </summary>
    public async Task<object?> InstancesExecuteRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b7afb167 = $"api/v3/flows/instances/{slug}/execute/";
        var response = await client.GetAsync(url_b7afb167, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b7afb167 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b7afb167;
    }

    /// <summary>
    /// GET /flows/instances/{slug}/export/
    /// </summary>
    public async Task<object?> InstancesExportRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_798c3e53 = $"api/v3/flows/instances/{slug}/export/";
        var response = await client.GetAsync(url_798c3e53, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_798c3e53 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_798c3e53;
    }

    /// <summary>
    /// POST /flows/instances/{slug}/set_background/
    /// </summary>
    public async Task<object?> InstancesSetBackgroundCreateAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0ac9378c = $"api/v3/flows/instances/{slug}/set_background/";
        var response = await client.PostAsync(url_0ac9378c, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0ac9378c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0ac9378c;
    }

    /// <summary>
    /// POST /flows/instances/{slug}/set_background_url/
    /// </summary>
    public async Task<object?> InstancesSetBackgroundUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1ea7a1b9 = $"api/v3/flows/instances/{slug}/set_background_url/";
        var response = await client.PostAsJsonAsync(url_1ea7a1b9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1ea7a1b9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1ea7a1b9;
    }

    /// <summary>
    /// GET /flows/instances/{slug}/used_by/
    /// </summary>
    public async Task<object?> InstancesUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d39d5b7e = $"api/v3/flows/instances/{slug}/used_by/";
        var response = await client.GetAsync(url_d39d5b7e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d39d5b7e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d39d5b7e;
    }

    /// <summary>
    /// POST /flows/instances/cache_clear/
    /// </summary>
    public async Task<object?> InstancesCacheClearCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1504aed0 = $"api/v3/flows/instances/cache_clear/";
        var response = await client.PostAsync(url_1504aed0, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1504aed0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1504aed0;
    }

    /// <summary>
    /// GET /flows/instances/cache_info/
    /// </summary>
    public async Task<PaginatedResult<object>> InstancesCacheInfoRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d5dc23f7 = $"api/v3/flows/instances/cache_info/";
        var response = await client.GetAsync(url_d5dc23f7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d5dc23f7 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d5dc23f7 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /flows/instances/import/
    /// </summary>
    public async Task<object?> InstancesImportCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0e078ef3 = $"api/v3/flows/instances/import/";
        var response = await client.PostAsync(url_0e078ef3, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0e078ef3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0e078ef3;
    }

}
