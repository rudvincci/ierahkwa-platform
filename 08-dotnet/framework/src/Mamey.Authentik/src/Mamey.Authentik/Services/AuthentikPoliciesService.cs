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
/// Service implementation for Authentik Policies API operations.
/// </summary>
public class AuthentikPoliciesService : IAuthentikPoliciesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikPoliciesService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikPoliciesService"/> class.
    /// </summary>
    public AuthentikPoliciesService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikPoliciesService> logger,
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
    /// GET /policies/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(bool? bindings__isnull = null, bool? promptstage__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/policies/all/";
        var queryParams = new List<string>();
        if (bindings__isnull.HasValue) queryParams.Add($"bindings__isnull={bindings__isnull.Value.ToString().ToLower()}");
        if (promptstage__isnull.HasValue) queryParams.Add($"promptstage__isnull={promptstage__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_f372512f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /policies/all/{policy_uuid}/
    /// </summary>
    public async Task<object?> AllRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95226f05 = $"api/v3/policies/all/{policy_uuid}/";
        var response = await client.GetAsync(url_95226f05, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95226f05 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95226f05;
    }

    /// <summary>
    /// DELETE /policies/all/{policy_uuid}/
    /// </summary>
    public async Task<object?> AllDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b02df8d9 = $"api/v3/policies/all/{policy_uuid}/";
        var response = await client.DeleteAsync(url_b02df8d9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b02df8d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b02df8d9;
    }

    /// <summary>
    /// POST /policies/all/{policy_uuid}/test/
    /// </summary>
    public async Task<object?> AllTestCreateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b1d73781 = $"api/v3/policies/all/{policy_uuid}/test/";
        var response = await client.PostAsJsonAsync(url_b1d73781, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b1d73781 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b1d73781;
    }

    /// <summary>
    /// GET /policies/all/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> AllUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a42dde8b = $"api/v3/policies/all/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_a42dde8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a42dde8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a42dde8b;
    }

    /// <summary>
    /// POST /policies/all/cache_clear/
    /// </summary>
    public async Task<object?> AllCacheClearCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f465c446 = $"api/v3/policies/all/cache_clear/";
        var response = await client.PostAsync(url_f465c446, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f465c446 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f465c446;
    }

    /// <summary>
    /// GET /policies/all/cache_info/
    /// </summary>
    public async Task<PaginatedResult<object>> AllCacheInfoRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_732face7 = $"api/v3/policies/all/cache_info/";
        var response = await client.GetAsync(url_732face7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_732face7 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_732face7 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /policies/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf388636 = $"api/v3/policies/all/types/";
        var response = await client.GetAsync(url_bf388636, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf388636 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf388636 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /policies/bindings/
    /// </summary>
    public async Task<PaginatedResult<object>> BindingsListAsync(bool? enabled = null, int? order = null, string? policy = null, bool? policy__isnull = null, string? target = null, string? target_in = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f5fcfdd5 = $"api/v3/policies/bindings/";
        var queryParams = new List<string>();
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (order.HasValue) queryParams.Add($"order={order}");
        if (!string.IsNullOrEmpty(policy)) queryParams.Add($"policy={policy}");
        if (policy__isnull.HasValue) queryParams.Add($"policy__isnull={policy__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(target)) queryParams.Add($"target={target}");
        if (!string.IsNullOrEmpty(target_in)) queryParams.Add($"target_in={target_in}");
        if (timeout.HasValue) queryParams.Add($"timeout={timeout}");
        if (queryParams.Any()) url_f5fcfdd5 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f5fcfdd5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f5fcfdd5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f5fcfdd5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/bindings/
    /// </summary>
    public async Task<object?> BindingsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_29f8cd67 = $"api/v3/policies/bindings/";
        var response = await client.PostAsJsonAsync(url_29f8cd67, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_29f8cd67 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_29f8cd67;
    }

    /// <summary>
    /// GET /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    public async Task<object?> BindingsRetrieveAsync(string policy_binding_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c6749f16 = $"api/v3/policies/bindings/{policy_binding_uuid}/";
        var response = await client.GetAsync(url_c6749f16, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c6749f16 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c6749f16;
    }

    /// <summary>
    /// PUT /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    public async Task<object?> BindingsUpdateAsync(string policy_binding_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e46d2fbb = $"api/v3/policies/bindings/{policy_binding_uuid}/";
        var response = await client.PutAsJsonAsync(url_e46d2fbb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e46d2fbb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e46d2fbb;
    }

    /// <summary>
    /// PATCH /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    public async Task<object?> BindingsPartialUpdateAsync(string policy_binding_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_281f9186 = $"api/v3/policies/bindings/{policy_binding_uuid}/";
        var response = await client.PatchAsJsonAsync(url_281f9186, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_281f9186 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_281f9186;
    }

    /// <summary>
    /// DELETE /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    public async Task<object?> BindingsDestroyAsync(string policy_binding_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_68702849 = $"api/v3/policies/bindings/{policy_binding_uuid}/";
        var response = await client.DeleteAsync(url_68702849, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_68702849 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_68702849;
    }

    /// <summary>
    /// GET /policies/bindings/{policy_binding_uuid}/used_by/
    /// </summary>
    public async Task<object?> BindingsUsedByListAsync(string policy_binding_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_af0e8757 = $"api/v3/policies/bindings/{policy_binding_uuid}/used_by/";
        var response = await client.GetAsync(url_af0e8757, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_af0e8757 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_af0e8757;
    }

    /// <summary>
    /// GET /policies/dummy/
    /// </summary>
    public async Task<PaginatedResult<object>> DummyListAsync(string? created = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, bool? result = null, int? wait_max = null, int? wait_min = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e6a69346 = $"api/v3/policies/dummy/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (result.HasValue) queryParams.Add($"result={result.Value.ToString().ToLower()}");
        if (wait_max.HasValue) queryParams.Add($"wait_max={wait_max}");
        if (wait_min.HasValue) queryParams.Add($"wait_min={wait_min}");
        if (queryParams.Any()) url_e6a69346 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_e6a69346, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e6a69346 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e6a69346 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/dummy/
    /// </summary>
    public async Task<object?> DummyCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_35825996 = $"api/v3/policies/dummy/";
        var response = await client.PostAsJsonAsync(url_35825996, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_35825996 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_35825996;
    }

    /// <summary>
    /// GET /policies/dummy/{policy_uuid}/
    /// </summary>
    public async Task<object?> DummyRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f667fc50 = $"api/v3/policies/dummy/{policy_uuid}/";
        var response = await client.GetAsync(url_f667fc50, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f667fc50 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f667fc50;
    }

    /// <summary>
    /// PUT /policies/dummy/{policy_uuid}/
    /// </summary>
    public async Task<object?> DummyUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_88f42186 = $"api/v3/policies/dummy/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_88f42186, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_88f42186 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_88f42186;
    }

    /// <summary>
    /// PATCH /policies/dummy/{policy_uuid}/
    /// </summary>
    public async Task<object?> DummyPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_241b5397 = $"api/v3/policies/dummy/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_241b5397, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_241b5397 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_241b5397;
    }

    /// <summary>
    /// DELETE /policies/dummy/{policy_uuid}/
    /// </summary>
    public async Task<object?> DummyDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_77058a71 = $"api/v3/policies/dummy/{policy_uuid}/";
        var response = await client.DeleteAsync(url_77058a71, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_77058a71 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_77058a71;
    }

    /// <summary>
    /// GET /policies/dummy/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> DummyUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ad2b17be = $"api/v3/policies/dummy/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_ad2b17be, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ad2b17be = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ad2b17be;
    }

    /// <summary>
    /// GET /policies/event_matcher/
    /// </summary>
    public async Task<PaginatedResult<object>> EventMatcherListAsync(string? action = null, string? app = null, string? client_ip = null, string? created = null, bool? execution_logging = null, string? last_updated = null, string? model = null, string? policy_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_40a60777 = $"api/v3/policies/event_matcher/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(action)) queryParams.Add($"action={action}");
        if (!string.IsNullOrEmpty(app)) queryParams.Add($"app={app}");
        if (!string.IsNullOrEmpty(client_ip)) queryParams.Add($"client_ip={client_ip}");
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(model)) queryParams.Add($"model={model}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (queryParams.Any()) url_40a60777 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_40a60777, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_40a60777 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_40a60777 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/event_matcher/
    /// </summary>
    public async Task<object?> EventMatcherCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_94be6c93 = $"api/v3/policies/event_matcher/";
        var response = await client.PostAsJsonAsync(url_94be6c93, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_94be6c93 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_94be6c93;
    }

    /// <summary>
    /// GET /policies/event_matcher/{policy_uuid}/
    /// </summary>
    public async Task<object?> EventMatcherRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dd652678 = $"api/v3/policies/event_matcher/{policy_uuid}/";
        var response = await client.GetAsync(url_dd652678, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dd652678 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dd652678;
    }

    /// <summary>
    /// PUT /policies/event_matcher/{policy_uuid}/
    /// </summary>
    public async Task<object?> EventMatcherUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_64f6285c = $"api/v3/policies/event_matcher/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_64f6285c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_64f6285c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_64f6285c;
    }

    /// <summary>
    /// PATCH /policies/event_matcher/{policy_uuid}/
    /// </summary>
    public async Task<object?> EventMatcherPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e2eb21ba = $"api/v3/policies/event_matcher/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_e2eb21ba, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e2eb21ba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e2eb21ba;
    }

    /// <summary>
    /// DELETE /policies/event_matcher/{policy_uuid}/
    /// </summary>
    public async Task<object?> EventMatcherDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_79173a1e = $"api/v3/policies/event_matcher/{policy_uuid}/";
        var response = await client.DeleteAsync(url_79173a1e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_79173a1e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_79173a1e;
    }

    /// <summary>
    /// GET /policies/event_matcher/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> EventMatcherUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_74063b7f = $"api/v3/policies/event_matcher/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_74063b7f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_74063b7f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_74063b7f;
    }

    /// <summary>
    /// GET /policies/expression/
    /// </summary>
    public async Task<PaginatedResult<object>> ExpressionListAsync(string? created = null, bool? execution_logging = null, string? expression = null, string? last_updated = null, string? policy_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f0cb8234 = $"api/v3/policies/expression/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(expression)) queryParams.Add($"expression={expression}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (queryParams.Any()) url_f0cb8234 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f0cb8234, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f0cb8234 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f0cb8234 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/expression/
    /// </summary>
    public async Task<object?> ExpressionCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_105da233 = $"api/v3/policies/expression/";
        var response = await client.PostAsJsonAsync(url_105da233, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_105da233 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_105da233;
    }

    /// <summary>
    /// GET /policies/expression/{policy_uuid}/
    /// </summary>
    public async Task<object?> ExpressionRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ff8091d0 = $"api/v3/policies/expression/{policy_uuid}/";
        var response = await client.GetAsync(url_ff8091d0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ff8091d0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ff8091d0;
    }

    /// <summary>
    /// PUT /policies/expression/{policy_uuid}/
    /// </summary>
    public async Task<object?> ExpressionUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d141a334 = $"api/v3/policies/expression/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_d141a334, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d141a334 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d141a334;
    }

    /// <summary>
    /// PATCH /policies/expression/{policy_uuid}/
    /// </summary>
    public async Task<object?> ExpressionPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7b13bd35 = $"api/v3/policies/expression/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_7b13bd35, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7b13bd35 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7b13bd35;
    }

    /// <summary>
    /// DELETE /policies/expression/{policy_uuid}/
    /// </summary>
    public async Task<object?> ExpressionDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5e5e7180 = $"api/v3/policies/expression/{policy_uuid}/";
        var response = await client.DeleteAsync(url_5e5e7180, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5e5e7180 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5e5e7180;
    }

    /// <summary>
    /// GET /policies/expression/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> ExpressionUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4d038aa8 = $"api/v3/policies/expression/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_4d038aa8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4d038aa8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4d038aa8;
    }

    /// <summary>
    /// GET /policies/geoip/
    /// </summary>
    public async Task<PaginatedResult<object>> GeoipListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_01b6bc26 = $"api/v3/policies/geoip/";
        var response = await client.GetAsync(url_01b6bc26, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_01b6bc26 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_01b6bc26 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/geoip/
    /// </summary>
    public async Task<object?> GeoipCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9addc714 = $"api/v3/policies/geoip/";
        var response = await client.PostAsJsonAsync(url_9addc714, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9addc714 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9addc714;
    }

    /// <summary>
    /// GET /policies/geoip/{policy_uuid}/
    /// </summary>
    public async Task<object?> GeoipRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_26c6065f = $"api/v3/policies/geoip/{policy_uuid}/";
        var response = await client.GetAsync(url_26c6065f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_26c6065f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_26c6065f;
    }

    /// <summary>
    /// PUT /policies/geoip/{policy_uuid}/
    /// </summary>
    public async Task<object?> GeoipUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_49c253d6 = $"api/v3/policies/geoip/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_49c253d6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_49c253d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_49c253d6;
    }

    /// <summary>
    /// PATCH /policies/geoip/{policy_uuid}/
    /// </summary>
    public async Task<object?> GeoipPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6f707ef3 = $"api/v3/policies/geoip/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_6f707ef3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6f707ef3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6f707ef3;
    }

    /// <summary>
    /// DELETE /policies/geoip/{policy_uuid}/
    /// </summary>
    public async Task<object?> GeoipDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_04d64663 = $"api/v3/policies/geoip/{policy_uuid}/";
        var response = await client.DeleteAsync(url_04d64663, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_04d64663 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_04d64663;
    }

    /// <summary>
    /// GET /policies/geoip/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> GeoipUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3b737e2c = $"api/v3/policies/geoip/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_3b737e2c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3b737e2c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3b737e2c;
    }

    /// <summary>
    /// GET /policies/geoip_iso3166/
    /// </summary>
    public async Task<PaginatedResult<object>> GeoipIso3166ListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_455d00c5 = $"api/v3/policies/geoip_iso3166/";
        var response = await client.GetAsync(url_455d00c5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_455d00c5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_455d00c5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /policies/password/
    /// </summary>
    public async Task<PaginatedResult<object>> PasswordListAsync(int? amount_digits = null, int? amount_lowercase = null, int? amount_symbols = null, int? amount_uppercase = null, bool? check_have_i_been_pwned = null, bool? check_static_rules = null, bool? check_zxcvbn = null, string? created = null, string? error_message = null, bool? execution_logging = null, int? hibp_allowed_count = null, string? last_updated = null, int? length_min = null, string? password_field = null, string? policy_uuid = null, string? symbol_charset = null, int? zxcvbn_score_threshold = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b63038fd = $"api/v3/policies/password/";
        var queryParams = new List<string>();
        if (amount_digits.HasValue) queryParams.Add($"amount_digits={amount_digits}");
        if (amount_lowercase.HasValue) queryParams.Add($"amount_lowercase={amount_lowercase}");
        if (amount_symbols.HasValue) queryParams.Add($"amount_symbols={amount_symbols}");
        if (amount_uppercase.HasValue) queryParams.Add($"amount_uppercase={amount_uppercase}");
        if (check_have_i_been_pwned.HasValue) queryParams.Add($"check_have_i_been_pwned={check_have_i_been_pwned.Value.ToString().ToLower()}");
        if (check_static_rules.HasValue) queryParams.Add($"check_static_rules={check_static_rules.Value.ToString().ToLower()}");
        if (check_zxcvbn.HasValue) queryParams.Add($"check_zxcvbn={check_zxcvbn.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (!string.IsNullOrEmpty(error_message)) queryParams.Add($"error_message={error_message}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (hibp_allowed_count.HasValue) queryParams.Add($"hibp_allowed_count={hibp_allowed_count}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (length_min.HasValue) queryParams.Add($"length_min={length_min}");
        if (!string.IsNullOrEmpty(password_field)) queryParams.Add($"password_field={password_field}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (!string.IsNullOrEmpty(symbol_charset)) queryParams.Add($"symbol_charset={symbol_charset}");
        if (zxcvbn_score_threshold.HasValue) queryParams.Add($"zxcvbn_score_threshold={zxcvbn_score_threshold}");
        if (queryParams.Any()) url_b63038fd += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b63038fd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b63038fd = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b63038fd ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/password/
    /// </summary>
    public async Task<object?> PasswordCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a5ca8702 = $"api/v3/policies/password/";
        var response = await client.PostAsJsonAsync(url_a5ca8702, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a5ca8702 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a5ca8702;
    }

    /// <summary>
    /// GET /policies/password/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2a6efff1 = $"api/v3/policies/password/{policy_uuid}/";
        var response = await client.GetAsync(url_2a6efff1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2a6efff1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2a6efff1;
    }

    /// <summary>
    /// PUT /policies/password/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e2bcf0a5 = $"api/v3/policies/password/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_e2bcf0a5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e2bcf0a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e2bcf0a5;
    }

    /// <summary>
    /// PATCH /policies/password/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb2ee305 = $"api/v3/policies/password/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_eb2ee305, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb2ee305 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb2ee305;
    }

    /// <summary>
    /// DELETE /policies/password/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_efa0097b = $"api/v3/policies/password/{policy_uuid}/";
        var response = await client.DeleteAsync(url_efa0097b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_efa0097b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_efa0097b;
    }

    /// <summary>
    /// GET /policies/password/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> PasswordUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df6912b3 = $"api/v3/policies/password/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_df6912b3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df6912b3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df6912b3;
    }

    /// <summary>
    /// GET /policies/password_expiry/
    /// </summary>
    public async Task<PaginatedResult<object>> PasswordExpiryListAsync(string? created = null, int? days = null, bool? deny_only = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2dd35aa0 = $"api/v3/policies/password_expiry/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (days.HasValue) queryParams.Add($"days={days}");
        if (deny_only.HasValue) queryParams.Add($"deny_only={deny_only.Value.ToString().ToLower()}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (queryParams.Any()) url_2dd35aa0 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_2dd35aa0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2dd35aa0 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2dd35aa0 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/password_expiry/
    /// </summary>
    public async Task<object?> PasswordExpiryCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7b9d5029 = $"api/v3/policies/password_expiry/";
        var response = await client.PostAsJsonAsync(url_7b9d5029, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7b9d5029 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7b9d5029;
    }

    /// <summary>
    /// GET /policies/password_expiry/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordExpiryRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0db6f6b3 = $"api/v3/policies/password_expiry/{policy_uuid}/";
        var response = await client.GetAsync(url_0db6f6b3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0db6f6b3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0db6f6b3;
    }

    /// <summary>
    /// PUT /policies/password_expiry/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordExpiryUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9009fb3f = $"api/v3/policies/password_expiry/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_9009fb3f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9009fb3f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9009fb3f;
    }

    /// <summary>
    /// PATCH /policies/password_expiry/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordExpiryPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2babef26 = $"api/v3/policies/password_expiry/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_2babef26, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2babef26 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2babef26;
    }

    /// <summary>
    /// DELETE /policies/password_expiry/{policy_uuid}/
    /// </summary>
    public async Task<object?> PasswordExpiryDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_659ede7a = $"api/v3/policies/password_expiry/{policy_uuid}/";
        var response = await client.DeleteAsync(url_659ede7a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_659ede7a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_659ede7a;
    }

    /// <summary>
    /// GET /policies/password_expiry/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> PasswordExpiryUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4388965b = $"api/v3/policies/password_expiry/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_4388965b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4388965b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4388965b;
    }

    /// <summary>
    /// GET /policies/reputation/
    /// </summary>
    public async Task<PaginatedResult<object>> ReputationListAsync(bool? check_ip = null, bool? check_username = null, string? created = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, int? threshold = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f7096c33 = $"api/v3/policies/reputation/";
        var queryParams = new List<string>();
        if (check_ip.HasValue) queryParams.Add($"check_ip={check_ip.Value.ToString().ToLower()}");
        if (check_username.HasValue) queryParams.Add($"check_username={check_username.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (threshold.HasValue) queryParams.Add($"threshold={threshold}");
        if (queryParams.Any()) url_f7096c33 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f7096c33, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f7096c33 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f7096c33 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/reputation/
    /// </summary>
    public async Task<object?> ReputationCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2d6f9de5 = $"api/v3/policies/reputation/";
        var response = await client.PostAsJsonAsync(url_2d6f9de5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2d6f9de5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2d6f9de5;
    }

    /// <summary>
    /// GET /policies/reputation/{policy_uuid}/
    /// </summary>
    public async Task<object?> ReputationRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5c065fdd = $"api/v3/policies/reputation/{policy_uuid}/";
        var response = await client.GetAsync(url_5c065fdd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5c065fdd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5c065fdd;
    }

    /// <summary>
    /// PUT /policies/reputation/{policy_uuid}/
    /// </summary>
    public async Task<object?> ReputationUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_85356578 = $"api/v3/policies/reputation/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_85356578, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_85356578 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_85356578;
    }

    /// <summary>
    /// PATCH /policies/reputation/{policy_uuid}/
    /// </summary>
    public async Task<object?> ReputationPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_94e3da9e = $"api/v3/policies/reputation/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_94e3da9e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_94e3da9e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_94e3da9e;
    }

    /// <summary>
    /// DELETE /policies/reputation/{policy_uuid}/
    /// </summary>
    public async Task<object?> ReputationDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8bf0d2f6 = $"api/v3/policies/reputation/{policy_uuid}/";
        var response = await client.DeleteAsync(url_8bf0d2f6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8bf0d2f6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8bf0d2f6;
    }

    /// <summary>
    /// GET /policies/reputation/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> ReputationUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_756c7c50 = $"api/v3/policies/reputation/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_756c7c50, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_756c7c50 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_756c7c50;
    }

    /// <summary>
    /// GET /policies/reputation/scores/
    /// </summary>
    public async Task<PaginatedResult<object>> ReputationScoresListAsync(string? identifier = null, string? identifier_in = null, string? ip = null, int? score = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0ff1430f = $"api/v3/policies/reputation/scores/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(identifier)) queryParams.Add($"identifier={identifier}");
        if (!string.IsNullOrEmpty(identifier_in)) queryParams.Add($"identifier_in={identifier_in}");
        if (!string.IsNullOrEmpty(ip)) queryParams.Add($"ip={ip}");
        if (score.HasValue) queryParams.Add($"score={score}");
        if (queryParams.Any()) url_0ff1430f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0ff1430f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0ff1430f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0ff1430f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /policies/reputation/scores/{reputation_uuid}/
    /// </summary>
    public async Task<object?> ReputationScoresRetrieveAsync(string reputation_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0eca326a = $"api/v3/policies/reputation/scores/{reputation_uuid}/";
        var response = await client.GetAsync(url_0eca326a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0eca326a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0eca326a;
    }

    /// <summary>
    /// DELETE /policies/reputation/scores/{reputation_uuid}/
    /// </summary>
    public async Task<object?> ReputationScoresDestroyAsync(string reputation_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c3594d0f = $"api/v3/policies/reputation/scores/{reputation_uuid}/";
        var response = await client.DeleteAsync(url_c3594d0f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c3594d0f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c3594d0f;
    }

    /// <summary>
    /// GET /policies/reputation/scores/{reputation_uuid}/used_by/
    /// </summary>
    public async Task<object?> ReputationScoresUsedByListAsync(string reputation_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8e5a8e78 = $"api/v3/policies/reputation/scores/{reputation_uuid}/used_by/";
        var response = await client.GetAsync(url_8e5a8e78, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8e5a8e78 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8e5a8e78;
    }

    /// <summary>
    /// GET /policies/unique_password/
    /// </summary>
    public async Task<PaginatedResult<object>> UniquePasswordListAsync(string? created = null, bool? execution_logging = null, string? last_updated = null, int? num_historical_passwords = null, string? password_field = null, string? policy_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8bc30e56 = $"api/v3/policies/unique_password/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (execution_logging.HasValue) queryParams.Add($"execution_logging={execution_logging.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (num_historical_passwords.HasValue) queryParams.Add($"num_historical_passwords={num_historical_passwords}");
        if (!string.IsNullOrEmpty(password_field)) queryParams.Add($"password_field={password_field}");
        if (!string.IsNullOrEmpty(policy_uuid)) queryParams.Add($"policy_uuid={policy_uuid}");
        if (queryParams.Any()) url_8bc30e56 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_8bc30e56, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8bc30e56 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8bc30e56 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /policies/unique_password/
    /// </summary>
    public async Task<object?> UniquePasswordCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_74e041df = $"api/v3/policies/unique_password/";
        var response = await client.PostAsJsonAsync(url_74e041df, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_74e041df = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_74e041df;
    }

    /// <summary>
    /// GET /policies/unique_password/{policy_uuid}/
    /// </summary>
    public async Task<object?> UniquePasswordRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_146316d6 = $"api/v3/policies/unique_password/{policy_uuid}/";
        var response = await client.GetAsync(url_146316d6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_146316d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_146316d6;
    }

    /// <summary>
    /// PUT /policies/unique_password/{policy_uuid}/
    /// </summary>
    public async Task<object?> UniquePasswordUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0efc3ec1 = $"api/v3/policies/unique_password/{policy_uuid}/";
        var response = await client.PutAsJsonAsync(url_0efc3ec1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0efc3ec1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0efc3ec1;
    }

    /// <summary>
    /// PATCH /policies/unique_password/{policy_uuid}/
    /// </summary>
    public async Task<object?> UniquePasswordPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_064772d4 = $"api/v3/policies/unique_password/{policy_uuid}/";
        var response = await client.PatchAsJsonAsync(url_064772d4, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_064772d4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_064772d4;
    }

    /// <summary>
    /// DELETE /policies/unique_password/{policy_uuid}/
    /// </summary>
    public async Task<object?> UniquePasswordDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9afab368 = $"api/v3/policies/unique_password/{policy_uuid}/";
        var response = await client.DeleteAsync(url_9afab368, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9afab368 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9afab368;
    }

    /// <summary>
    /// GET /policies/unique_password/{policy_uuid}/used_by/
    /// </summary>
    public async Task<object?> UniquePasswordUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_99c09de2 = $"api/v3/policies/unique_password/{policy_uuid}/used_by/";
        var response = await client.GetAsync(url_99c09de2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_99c09de2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_99c09de2;
    }

}
