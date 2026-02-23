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
/// Service implementation for Authentik PropertyMappings API operations.
/// </summary>
public class AuthentikPropertyMappingsService : IAuthentikPropertyMappingsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikPropertyMappingsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikPropertyMappingsService"/> class.
    /// </summary>
    public AuthentikPropertyMappingsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikPropertyMappingsService> logger,
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
    /// GET /propertymappings/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/propertymappings/all/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_f372512f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /propertymappings/all/{pm_uuid}/
    /// </summary>
    public async Task<object?> AllRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95226f05 = $"api/v3/propertymappings/all/{pm_uuid}/";
        var response = await client.GetAsync(url_95226f05, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95226f05 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95226f05;
    }

    /// <summary>
    /// DELETE /propertymappings/all/{pm_uuid}/
    /// </summary>
    public async Task<object?> AllDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b02df8d9 = $"api/v3/propertymappings/all/{pm_uuid}/";
        var response = await client.DeleteAsync(url_b02df8d9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b02df8d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b02df8d9;
    }

    /// <summary>
    /// POST /propertymappings/all/{pm_uuid}/test/
    /// </summary>
    public async Task<object?> AllTestCreateAsync(string pm_uuid, object request, bool? format_result = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b1d73781 = $"api/v3/propertymappings/all/{pm_uuid}/test/";
        var queryParams = new List<string>();
        if (format_result.HasValue) queryParams.Add($"format_result={format_result.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_b1d73781 += "?" + string.Join("&", queryParams);
        var response = await client.PostAsJsonAsync(url_b1d73781, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b1d73781 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b1d73781;
    }

    /// <summary>
    /// GET /propertymappings/all/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> AllUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a42dde8b = $"api/v3/propertymappings/all/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_a42dde8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a42dde8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a42dde8b;
    }

    /// <summary>
    /// GET /propertymappings/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf388636 = $"api/v3/propertymappings/all/types/";
        var response = await client.GetAsync(url_bf388636, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf388636 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf388636 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /propertymappings/notification/
    /// </summary>
    public async Task<PaginatedResult<object>> NotificationListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_317ed80d = $"api/v3/propertymappings/notification/";
        var response = await client.GetAsync(url_317ed80d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_317ed80d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_317ed80d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/notification/
    /// </summary>
    public async Task<object?> NotificationCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75f38706 = $"api/v3/propertymappings/notification/";
        var response = await client.PostAsJsonAsync(url_75f38706, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75f38706 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75f38706;
    }

    /// <summary>
    /// GET /propertymappings/notification/{pm_uuid}/
    /// </summary>
    public async Task<object?> NotificationRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_406a3d96 = $"api/v3/propertymappings/notification/{pm_uuid}/";
        var response = await client.GetAsync(url_406a3d96, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_406a3d96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_406a3d96;
    }

    /// <summary>
    /// PUT /propertymappings/notification/{pm_uuid}/
    /// </summary>
    public async Task<object?> NotificationUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bd8126ea = $"api/v3/propertymappings/notification/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_bd8126ea, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bd8126ea = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bd8126ea;
    }

    /// <summary>
    /// PATCH /propertymappings/notification/{pm_uuid}/
    /// </summary>
    public async Task<object?> NotificationPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a6c31f62 = $"api/v3/propertymappings/notification/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_a6c31f62, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a6c31f62 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a6c31f62;
    }

    /// <summary>
    /// DELETE /propertymappings/notification/{pm_uuid}/
    /// </summary>
    public async Task<object?> NotificationDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_55492057 = $"api/v3/propertymappings/notification/{pm_uuid}/";
        var response = await client.DeleteAsync(url_55492057, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_55492057 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_55492057;
    }

    /// <summary>
    /// GET /propertymappings/notification/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> NotificationUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fb09339a = $"api/v3/propertymappings/notification/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_fb09339a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fb09339a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fb09339a;
    }

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderGoogleWorkspaceListAsync(string? expression = null, string? managed = null, string? pm_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_60f3739a = $"api/v3/propertymappings/provider/google_workspace/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(expression)) queryParams.Add($"expression={expression}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (!string.IsNullOrEmpty(pm_uuid)) queryParams.Add($"pm_uuid={pm_uuid}");
        if (queryParams.Any()) url_60f3739a += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_60f3739a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_60f3739a = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_60f3739a ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/google_workspace/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspaceCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_353c76ba = $"api/v3/propertymappings/provider/google_workspace/";
        var response = await client.PostAsJsonAsync(url_353c76ba, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_353c76ba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_353c76ba;
    }

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspaceRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b2eff9a9 = $"api/v3/propertymappings/provider/google_workspace/{pm_uuid}/";
        var response = await client.GetAsync(url_b2eff9a9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b2eff9a9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b2eff9a9;
    }

    /// <summary>
    /// PUT /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspaceUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_abd1f32e = $"api/v3/propertymappings/provider/google_workspace/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_abd1f32e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_abd1f32e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_abd1f32e;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspacePartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_240f7fad = $"api/v3/propertymappings/provider/google_workspace/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_240f7fad, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_240f7fad = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_240f7fad;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/google_workspace/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspaceDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3d5f0721 = $"api/v3/propertymappings/provider/google_workspace/{pm_uuid}/";
        var response = await client.DeleteAsync(url_3d5f0721, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3d5f0721 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3d5f0721;
    }

    /// <summary>
    /// GET /propertymappings/provider/google_workspace/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderGoogleWorkspaceUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f8d65463 = $"api/v3/propertymappings/provider/google_workspace/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_f8d65463, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f8d65463 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f8d65463;
    }

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderMicrosoftEntraListAsync(string? expression = null, string? managed = null, string? pm_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a0732e1f = $"api/v3/propertymappings/provider/microsoft_entra/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(expression)) queryParams.Add($"expression={expression}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (!string.IsNullOrEmpty(pm_uuid)) queryParams.Add($"pm_uuid={pm_uuid}");
        if (queryParams.Any()) url_a0732e1f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_a0732e1f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a0732e1f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a0732e1f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/microsoft_entra/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_92be2fc0 = $"api/v3/propertymappings/provider/microsoft_entra/";
        var response = await client.PostAsJsonAsync(url_92be2fc0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_92be2fc0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_92be2fc0;
    }

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ed3d5e54 = $"api/v3/propertymappings/provider/microsoft_entra/{pm_uuid}/";
        var response = await client.GetAsync(url_ed3d5e54, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ed3d5e54 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ed3d5e54;
    }

    /// <summary>
    /// PUT /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_24d082c9 = $"api/v3/propertymappings/provider/microsoft_entra/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_24d082c9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_24d082c9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_24d082c9;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_794669e7 = $"api/v3/propertymappings/provider/microsoft_entra/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_794669e7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_794669e7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_794669e7;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/microsoft_entra/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fdfe6c37 = $"api/v3/propertymappings/provider/microsoft_entra/{pm_uuid}/";
        var response = await client.DeleteAsync(url_fdfe6c37, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fdfe6c37 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fdfe6c37;
    }

    /// <summary>
    /// GET /propertymappings/provider/microsoft_entra/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderMicrosoftEntraUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_32b54d7f = $"api/v3/propertymappings/provider/microsoft_entra/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_32b54d7f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_32b54d7f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_32b54d7f;
    }

    /// <summary>
    /// GET /propertymappings/provider/rac/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderRacListAsync(string? managed = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_361355dc = $"api/v3/propertymappings/provider/rac/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (queryParams.Any()) url_361355dc += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_361355dc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_361355dc = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_361355dc ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/rac/
    /// </summary>
    public async Task<object?> ProviderRacCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_71bbcaf8 = $"api/v3/propertymappings/provider/rac/";
        var response = await client.PostAsJsonAsync(url_71bbcaf8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_71bbcaf8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_71bbcaf8;
    }

    /// <summary>
    /// GET /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRacRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2eef20f6 = $"api/v3/propertymappings/provider/rac/{pm_uuid}/";
        var response = await client.GetAsync(url_2eef20f6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2eef20f6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2eef20f6;
    }

    /// <summary>
    /// PUT /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRacUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_18a17999 = $"api/v3/propertymappings/provider/rac/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_18a17999, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_18a17999 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_18a17999;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRacPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3ff51788 = $"api/v3/propertymappings/provider/rac/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_3ff51788, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3ff51788 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3ff51788;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/rac/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRacDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_507443f3 = $"api/v3/propertymappings/provider/rac/{pm_uuid}/";
        var response = await client.DeleteAsync(url_507443f3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_507443f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_507443f3;
    }

    /// <summary>
    /// GET /propertymappings/provider/rac/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderRacUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fdc44e0e = $"api/v3/propertymappings/provider/rac/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_fdc44e0e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fdc44e0e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fdc44e0e;
    }

    /// <summary>
    /// GET /propertymappings/provider/radius/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderRadiusListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2be5443b = $"api/v3/propertymappings/provider/radius/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_2be5443b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_2be5443b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2be5443b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2be5443b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/radius/
    /// </summary>
    public async Task<object?> ProviderRadiusCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_668bf4f7 = $"api/v3/propertymappings/provider/radius/";
        var response = await client.PostAsJsonAsync(url_668bf4f7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_668bf4f7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_668bf4f7;
    }

    /// <summary>
    /// GET /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRadiusRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a80d4742 = $"api/v3/propertymappings/provider/radius/{pm_uuid}/";
        var response = await client.GetAsync(url_a80d4742, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a80d4742 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a80d4742;
    }

    /// <summary>
    /// PUT /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRadiusUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4b6df62c = $"api/v3/propertymappings/provider/radius/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_4b6df62c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4b6df62c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4b6df62c;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRadiusPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_82f119fd = $"api/v3/propertymappings/provider/radius/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_82f119fd, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_82f119fd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_82f119fd;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/radius/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderRadiusDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1ecc892c = $"api/v3/propertymappings/provider/radius/{pm_uuid}/";
        var response = await client.DeleteAsync(url_1ecc892c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1ecc892c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1ecc892c;
    }

    /// <summary>
    /// GET /propertymappings/provider/radius/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderRadiusUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_82e69499 = $"api/v3/propertymappings/provider/radius/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_82e69499, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_82e69499 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_82e69499;
    }

    /// <summary>
    /// GET /propertymappings/provider/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderSamlListAsync(string? friendly_name = null, string? managed = null, bool? managed__isnull = null, string? saml_name = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_90d09cc4 = $"api/v3/propertymappings/provider/saml/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(saml_name)) queryParams.Add($"saml_name={saml_name}");
        if (queryParams.Any()) url_90d09cc4 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_90d09cc4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_90d09cc4 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_90d09cc4 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/saml/
    /// </summary>
    public async Task<object?> ProviderSamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3c5b39a5 = $"api/v3/propertymappings/provider/saml/";
        var response = await client.PostAsJsonAsync(url_3c5b39a5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3c5b39a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3c5b39a5;
    }

    /// <summary>
    /// GET /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderSamlRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_933ca407 = $"api/v3/propertymappings/provider/saml/{pm_uuid}/";
        var response = await client.GetAsync(url_933ca407, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_933ca407 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_933ca407;
    }

    /// <summary>
    /// PUT /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderSamlUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_39427ad7 = $"api/v3/propertymappings/provider/saml/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_39427ad7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_39427ad7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_39427ad7;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderSamlPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_804252c0 = $"api/v3/propertymappings/provider/saml/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_804252c0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_804252c0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_804252c0;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderSamlDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f6014c4a = $"api/v3/propertymappings/provider/saml/{pm_uuid}/";
        var response = await client.DeleteAsync(url_f6014c4a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f6014c4a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f6014c4a;
    }

    /// <summary>
    /// GET /propertymappings/provider/saml/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderSamlUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1195e2fb = $"api/v3/propertymappings/provider/saml/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_1195e2fb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1195e2fb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1195e2fb;
    }

    /// <summary>
    /// GET /propertymappings/provider/scim/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderScimListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d07055da = $"api/v3/propertymappings/provider/scim/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_d07055da += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d07055da, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d07055da = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d07055da ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/scim/
    /// </summary>
    public async Task<object?> ProviderScimCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a7aee1cf = $"api/v3/propertymappings/provider/scim/";
        var response = await client.PostAsJsonAsync(url_a7aee1cf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a7aee1cf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a7aee1cf;
    }

    /// <summary>
    /// GET /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScimRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f507d0e9 = $"api/v3/propertymappings/provider/scim/{pm_uuid}/";
        var response = await client.GetAsync(url_f507d0e9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f507d0e9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f507d0e9;
    }

    /// <summary>
    /// PUT /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScimUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_553a86f4 = $"api/v3/propertymappings/provider/scim/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_553a86f4, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_553a86f4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_553a86f4;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScimPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf9fb52e = $"api/v3/propertymappings/provider/scim/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_bf9fb52e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf9fb52e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf9fb52e;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScimDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_864e7a90 = $"api/v3/propertymappings/provider/scim/{pm_uuid}/";
        var response = await client.DeleteAsync(url_864e7a90, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_864e7a90 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_864e7a90;
    }

    /// <summary>
    /// GET /propertymappings/provider/scim/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderScimUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1ab8e6a7 = $"api/v3/propertymappings/provider/scim/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_1ab8e6a7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1ab8e6a7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1ab8e6a7;
    }

    /// <summary>
    /// GET /propertymappings/provider/scope/
    /// </summary>
    public async Task<PaginatedResult<object>> ProviderScopeListAsync(string? managed = null, bool? managed__isnull = null, string? scope_name = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75d8af29 = $"api/v3/propertymappings/provider/scope/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(scope_name)) queryParams.Add($"scope_name={scope_name}");
        if (queryParams.Any()) url_75d8af29 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_75d8af29, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75d8af29 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75d8af29 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/provider/scope/
    /// </summary>
    public async Task<object?> ProviderScopeCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_18ff166e = $"api/v3/propertymappings/provider/scope/";
        var response = await client.PostAsJsonAsync(url_18ff166e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_18ff166e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_18ff166e;
    }

    /// <summary>
    /// GET /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScopeRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_36e7da84 = $"api/v3/propertymappings/provider/scope/{pm_uuid}/";
        var response = await client.GetAsync(url_36e7da84, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_36e7da84 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_36e7da84;
    }

    /// <summary>
    /// PUT /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScopeUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_36e3def7 = $"api/v3/propertymappings/provider/scope/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_36e3def7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_36e3def7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_36e3def7;
    }

    /// <summary>
    /// PATCH /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScopePartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_703ee7a5 = $"api/v3/propertymappings/provider/scope/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_703ee7a5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_703ee7a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_703ee7a5;
    }

    /// <summary>
    /// DELETE /propertymappings/provider/scope/{pm_uuid}/
    /// </summary>
    public async Task<object?> ProviderScopeDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0269a411 = $"api/v3/propertymappings/provider/scope/{pm_uuid}/";
        var response = await client.DeleteAsync(url_0269a411, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0269a411 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0269a411;
    }

    /// <summary>
    /// GET /propertymappings/provider/scope/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ProviderScopeUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b76afc4 = $"api/v3/propertymappings/provider/scope/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_5b76afc4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b76afc4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b76afc4;
    }

    /// <summary>
    /// GET /propertymappings/source/kerberos/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceKerberosListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_638ee5ca = $"api/v3/propertymappings/source/kerberos/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_638ee5ca += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_638ee5ca, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_638ee5ca = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_638ee5ca ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/kerberos/
    /// </summary>
    public async Task<object?> SourceKerberosCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9d73b59c = $"api/v3/propertymappings/source/kerberos/";
        var response = await client.PostAsJsonAsync(url_9d73b59c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9d73b59c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9d73b59c;
    }

    /// <summary>
    /// GET /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceKerberosRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb1e6ffc = $"api/v3/propertymappings/source/kerberos/{pm_uuid}/";
        var response = await client.GetAsync(url_eb1e6ffc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb1e6ffc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb1e6ffc;
    }

    /// <summary>
    /// PUT /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceKerberosUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_385e6d1c = $"api/v3/propertymappings/source/kerberos/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_385e6d1c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_385e6d1c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_385e6d1c;
    }

    /// <summary>
    /// PATCH /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceKerberosPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_86f49ded = $"api/v3/propertymappings/source/kerberos/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_86f49ded, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_86f49ded = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_86f49ded;
    }

    /// <summary>
    /// DELETE /propertymappings/source/kerberos/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceKerberosDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8c008708 = $"api/v3/propertymappings/source/kerberos/{pm_uuid}/";
        var response = await client.DeleteAsync(url_8c008708, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8c008708 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8c008708;
    }

    /// <summary>
    /// GET /propertymappings/source/kerberos/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceKerberosUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_22fb2256 = $"api/v3/propertymappings/source/kerberos/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_22fb2256, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_22fb2256 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_22fb2256;
    }

    /// <summary>
    /// GET /propertymappings/source/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceLdapListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1198a60e = $"api/v3/propertymappings/source/ldap/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_1198a60e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_1198a60e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1198a60e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1198a60e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/ldap/
    /// </summary>
    public async Task<object?> SourceLdapCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_49d37db6 = $"api/v3/propertymappings/source/ldap/";
        var response = await client.PostAsJsonAsync(url_49d37db6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_49d37db6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_49d37db6;
    }

    /// <summary>
    /// GET /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceLdapRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_541ff771 = $"api/v3/propertymappings/source/ldap/{pm_uuid}/";
        var response = await client.GetAsync(url_541ff771, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_541ff771 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_541ff771;
    }

    /// <summary>
    /// PUT /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceLdapUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_43ddd777 = $"api/v3/propertymappings/source/ldap/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_43ddd777, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_43ddd777 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_43ddd777;
    }

    /// <summary>
    /// PATCH /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceLdapPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4489f8b2 = $"api/v3/propertymappings/source/ldap/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_4489f8b2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4489f8b2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4489f8b2;
    }

    /// <summary>
    /// DELETE /propertymappings/source/ldap/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceLdapDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1e3c1068 = $"api/v3/propertymappings/source/ldap/{pm_uuid}/";
        var response = await client.DeleteAsync(url_1e3c1068, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1e3c1068 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1e3c1068;
    }

    /// <summary>
    /// GET /propertymappings/source/ldap/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceLdapUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3762529a = $"api/v3/propertymappings/source/ldap/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_3762529a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3762529a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3762529a;
    }

    /// <summary>
    /// GET /propertymappings/source/oauth/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceOauthListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_00ced67d = $"api/v3/propertymappings/source/oauth/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_00ced67d += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_00ced67d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_00ced67d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_00ced67d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/oauth/
    /// </summary>
    public async Task<object?> SourceOauthCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e0685677 = $"api/v3/propertymappings/source/oauth/";
        var response = await client.PostAsJsonAsync(url_e0685677, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e0685677 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e0685677;
    }

    /// <summary>
    /// GET /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceOauthRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_05660495 = $"api/v3/propertymappings/source/oauth/{pm_uuid}/";
        var response = await client.GetAsync(url_05660495, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_05660495 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_05660495;
    }

    /// <summary>
    /// PUT /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceOauthUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9b3ab3ae = $"api/v3/propertymappings/source/oauth/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_9b3ab3ae, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9b3ab3ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9b3ab3ae;
    }

    /// <summary>
    /// PATCH /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceOauthPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a6e48f70 = $"api/v3/propertymappings/source/oauth/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_a6e48f70, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a6e48f70 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a6e48f70;
    }

    /// <summary>
    /// DELETE /propertymappings/source/oauth/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceOauthDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f3a030de = $"api/v3/propertymappings/source/oauth/{pm_uuid}/";
        var response = await client.DeleteAsync(url_f3a030de, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f3a030de = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f3a030de;
    }

    /// <summary>
    /// GET /propertymappings/source/oauth/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceOauthUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5c9f8d81 = $"api/v3/propertymappings/source/oauth/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_5c9f8d81, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5c9f8d81 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5c9f8d81;
    }

    /// <summary>
    /// GET /propertymappings/source/plex/
    /// </summary>
    public async Task<PaginatedResult<object>> SourcePlexListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_988fc946 = $"api/v3/propertymappings/source/plex/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_988fc946 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_988fc946, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_988fc946 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_988fc946 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/plex/
    /// </summary>
    public async Task<object?> SourcePlexCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_126eb5bf = $"api/v3/propertymappings/source/plex/";
        var response = await client.PostAsJsonAsync(url_126eb5bf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_126eb5bf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_126eb5bf;
    }

    /// <summary>
    /// GET /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourcePlexRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fc4e7a5c = $"api/v3/propertymappings/source/plex/{pm_uuid}/";
        var response = await client.GetAsync(url_fc4e7a5c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fc4e7a5c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fc4e7a5c;
    }

    /// <summary>
    /// PUT /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourcePlexUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c8f529b8 = $"api/v3/propertymappings/source/plex/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_c8f529b8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c8f529b8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c8f529b8;
    }

    /// <summary>
    /// PATCH /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourcePlexPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7eb8579f = $"api/v3/propertymappings/source/plex/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_7eb8579f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7eb8579f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7eb8579f;
    }

    /// <summary>
    /// DELETE /propertymappings/source/plex/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourcePlexDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6a7a83f3 = $"api/v3/propertymappings/source/plex/{pm_uuid}/";
        var response = await client.DeleteAsync(url_6a7a83f3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6a7a83f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6a7a83f3;
    }

    /// <summary>
    /// GET /propertymappings/source/plex/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourcePlexUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_39df43f3 = $"api/v3/propertymappings/source/plex/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_39df43f3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_39df43f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_39df43f3;
    }

    /// <summary>
    /// GET /propertymappings/source/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceSamlListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0aa67138 = $"api/v3/propertymappings/source/saml/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_0aa67138 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0aa67138, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0aa67138 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0aa67138 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/saml/
    /// </summary>
    public async Task<object?> SourceSamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1ba196d5 = $"api/v3/propertymappings/source/saml/";
        var response = await client.PostAsJsonAsync(url_1ba196d5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1ba196d5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1ba196d5;
    }

    /// <summary>
    /// GET /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceSamlRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_89ca21cc = $"api/v3/propertymappings/source/saml/{pm_uuid}/";
        var response = await client.GetAsync(url_89ca21cc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_89ca21cc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_89ca21cc;
    }

    /// <summary>
    /// PUT /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceSamlUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_379f0596 = $"api/v3/propertymappings/source/saml/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_379f0596, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_379f0596 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_379f0596;
    }

    /// <summary>
    /// PATCH /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceSamlPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_61bd559e = $"api/v3/propertymappings/source/saml/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_61bd559e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_61bd559e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_61bd559e;
    }

    /// <summary>
    /// DELETE /propertymappings/source/saml/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceSamlDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6106fda4 = $"api/v3/propertymappings/source/saml/{pm_uuid}/";
        var response = await client.DeleteAsync(url_6106fda4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6106fda4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6106fda4;
    }

    /// <summary>
    /// GET /propertymappings/source/saml/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceSamlUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_805024cb = $"api/v3/propertymappings/source/saml/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_805024cb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_805024cb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_805024cb;
    }

    /// <summary>
    /// GET /propertymappings/source/scim/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceScimListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_84c972f3 = $"api/v3/propertymappings/source/scim/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_84c972f3 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_84c972f3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_84c972f3 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_84c972f3 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/scim/
    /// </summary>
    public async Task<object?> SourceScimCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f4d71daa = $"api/v3/propertymappings/source/scim/";
        var response = await client.PostAsJsonAsync(url_f4d71daa, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f4d71daa = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f4d71daa;
    }

    /// <summary>
    /// GET /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceScimRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_78ad5fda = $"api/v3/propertymappings/source/scim/{pm_uuid}/";
        var response = await client.GetAsync(url_78ad5fda, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_78ad5fda = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_78ad5fda;
    }

    /// <summary>
    /// PUT /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceScimUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_48c2fc13 = $"api/v3/propertymappings/source/scim/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_48c2fc13, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_48c2fc13 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_48c2fc13;
    }

    /// <summary>
    /// PATCH /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceScimPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f3b93bd1 = $"api/v3/propertymappings/source/scim/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_f3b93bd1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f3b93bd1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f3b93bd1;
    }

    /// <summary>
    /// DELETE /propertymappings/source/scim/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceScimDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2ddb06de = $"api/v3/propertymappings/source/scim/{pm_uuid}/";
        var response = await client.DeleteAsync(url_2ddb06de, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2ddb06de = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2ddb06de;
    }

    /// <summary>
    /// GET /propertymappings/source/scim/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceScimUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2e01b0ed = $"api/v3/propertymappings/source/scim/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_2e01b0ed, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2e01b0ed = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2e01b0ed;
    }

    /// <summary>
    /// GET /propertymappings/source/telegram/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceTelegramListAsync(string? managed = null, bool? managed__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a7527374 = $"api/v3/propertymappings/source/telegram/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (managed__isnull.HasValue) queryParams.Add($"managed__isnull={managed__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_a7527374 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_a7527374, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a7527374 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a7527374 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /propertymappings/source/telegram/
    /// </summary>
    public async Task<object?> SourceTelegramCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ca57c548 = $"api/v3/propertymappings/source/telegram/";
        var response = await client.PostAsJsonAsync(url_ca57c548, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ca57c548 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ca57c548;
    }

    /// <summary>
    /// GET /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceTelegramRetrieveAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a4eafd75 = $"api/v3/propertymappings/source/telegram/{pm_uuid}/";
        var response = await client.GetAsync(url_a4eafd75, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a4eafd75 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a4eafd75;
    }

    /// <summary>
    /// PUT /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceTelegramUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95509aef = $"api/v3/propertymappings/source/telegram/{pm_uuid}/";
        var response = await client.PutAsJsonAsync(url_95509aef, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95509aef = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95509aef;
    }

    /// <summary>
    /// PATCH /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceTelegramPartialUpdateAsync(string pm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_79c7d570 = $"api/v3/propertymappings/source/telegram/{pm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_79c7d570, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_79c7d570 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_79c7d570;
    }

    /// <summary>
    /// DELETE /propertymappings/source/telegram/{pm_uuid}/
    /// </summary>
    public async Task<object?> SourceTelegramDestroyAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4d96650c = $"api/v3/propertymappings/source/telegram/{pm_uuid}/";
        var response = await client.DeleteAsync(url_4d96650c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4d96650c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4d96650c;
    }

    /// <summary>
    /// GET /propertymappings/source/telegram/{pm_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceTelegramUsedByListAsync(string pm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3061826e = $"api/v3/propertymappings/source/telegram/{pm_uuid}/used_by/";
        var response = await client.GetAsync(url_3061826e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3061826e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3061826e;
    }

}
