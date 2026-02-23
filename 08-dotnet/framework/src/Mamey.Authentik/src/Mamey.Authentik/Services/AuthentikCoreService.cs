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
/// Service implementation for Authentik Core API operations.
/// </summary>
public class AuthentikCoreService : IAuthentikCoreService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikCoreService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikCoreService"/> class.
    /// </summary>
    public AuthentikCoreService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikCoreService> logger,
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
    /// GET /core/application_entitlements/
    /// </summary>
    public async Task<PaginatedResult<object>> ApplicationEntitlementsListAsync(string? app = null, string? pbm_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eede8be3 = $"api/v3/core/application_entitlements/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(app)) queryParams.Add($"app={app}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (queryParams.Any()) url_eede8be3 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_eede8be3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eede8be3 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eede8be3 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/application_entitlements/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_373b291b = $"api/v3/core/application_entitlements/";
        var response = await client.PostAsJsonAsync(url_373b291b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_373b291b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_373b291b;
    }

    /// <summary>
    /// GET /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_da0b99ca = $"api/v3/core/application_entitlements/{pbm_uuid}/";
        var response = await client.GetAsync(url_da0b99ca, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_da0b99ca = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_da0b99ca;
    }

    /// <summary>
    /// PUT /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c6be0130 = $"api/v3/core/application_entitlements/{pbm_uuid}/";
        var response = await client.PutAsJsonAsync(url_c6be0130, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c6be0130 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c6be0130;
    }

    /// <summary>
    /// PATCH /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_092f00aa = $"api/v3/core/application_entitlements/{pbm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_092f00aa, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_092f00aa = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_092f00aa;
    }

    /// <summary>
    /// DELETE /core/application_entitlements/{pbm_uuid}/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ab7a8ff5 = $"api/v3/core/application_entitlements/{pbm_uuid}/";
        var response = await client.DeleteAsync(url_ab7a8ff5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ab7a8ff5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ab7a8ff5;
    }

    /// <summary>
    /// GET /core/application_entitlements/{pbm_uuid}/used_by/
    /// </summary>
    public async Task<object?> ApplicationEntitlementsUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_30953f56 = $"api/v3/core/application_entitlements/{pbm_uuid}/used_by/";
        var response = await client.GetAsync(url_30953f56, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_30953f56 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_30953f56;
    }

    /// <summary>
    /// GET /core/applications/
    /// </summary>
    public async Task<PaginatedResult<object>> ApplicationsListAsync(int? for_user = null, string? group = null, string? meta_description = null, string? meta_launch_url = null, string? meta_publisher = null, bool? only_with_launch_url = null, string? slug = null, bool? superuser_full_list = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_88334c3f = $"api/v3/core/applications/";
        var queryParams = new List<string>();
        if (for_user.HasValue) queryParams.Add($"for_user={for_user}");
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(meta_description)) queryParams.Add($"meta_description={meta_description}");
        if (!string.IsNullOrEmpty(meta_launch_url)) queryParams.Add($"meta_launch_url={meta_launch_url}");
        if (!string.IsNullOrEmpty(meta_publisher)) queryParams.Add($"meta_publisher={meta_publisher}");
        if (only_with_launch_url.HasValue) queryParams.Add($"only_with_launch_url={only_with_launch_url.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (superuser_full_list.HasValue) queryParams.Add($"superuser_full_list={superuser_full_list.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_88334c3f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_88334c3f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_88334c3f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_88334c3f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/applications/
    /// </summary>
    public async Task<object?> ApplicationsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9d4aff34 = $"api/v3/core/applications/";
        var response = await client.PostAsJsonAsync(url_9d4aff34, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9d4aff34 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9d4aff34;
    }

    /// <summary>
    /// GET /core/applications/{slug}/
    /// </summary>
    public async Task<object?> ApplicationsRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0ef1ed61 = $"api/v3/core/applications/{slug}/";
        var response = await client.GetAsync(url_0ef1ed61, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0ef1ed61 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0ef1ed61;
    }

    /// <summary>
    /// PUT /core/applications/{slug}/
    /// </summary>
    public async Task<object?> ApplicationsUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_70aea64a = $"api/v3/core/applications/{slug}/";
        var response = await client.PutAsJsonAsync(url_70aea64a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_70aea64a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_70aea64a;
    }

    /// <summary>
    /// PATCH /core/applications/{slug}/
    /// </summary>
    public async Task<object?> ApplicationsPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_94c06947 = $"api/v3/core/applications/{slug}/";
        var response = await client.PatchAsJsonAsync(url_94c06947, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_94c06947 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_94c06947;
    }

    /// <summary>
    /// DELETE /core/applications/{slug}/
    /// </summary>
    public async Task<object?> ApplicationsDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b1554672 = $"api/v3/core/applications/{slug}/";
        var response = await client.DeleteAsync(url_b1554672, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b1554672 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b1554672;
    }

    /// <summary>
    /// GET /core/applications/{slug}/check_access/
    /// </summary>
    public async Task<object?> ApplicationsCheckAccessRetrieveAsync(string slug, int? for_user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2087da70 = $"api/v3/core/applications/{slug}/check_access/";
        var queryParams = new List<string>();
        if (for_user.HasValue) queryParams.Add($"for_user={for_user}");
        if (queryParams.Any()) url_2087da70 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_2087da70, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2087da70 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2087da70;
    }

    /// <summary>
    /// POST /core/applications/{slug}/set_icon/
    /// </summary>
    public async Task<object?> ApplicationsSetIconCreateAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c7e48ff2 = $"api/v3/core/applications/{slug}/set_icon/";
        var response = await client.PostAsync(url_c7e48ff2, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c7e48ff2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c7e48ff2;
    }

    /// <summary>
    /// POST /core/applications/{slug}/set_icon_url/
    /// </summary>
    public async Task<object?> ApplicationsSetIconUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_088accbf = $"api/v3/core/applications/{slug}/set_icon_url/";
        var response = await client.PostAsJsonAsync(url_088accbf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_088accbf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_088accbf;
    }

    /// <summary>
    /// GET /core/applications/{slug}/used_by/
    /// </summary>
    public async Task<object?> ApplicationsUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4b416c33 = $"api/v3/core/applications/{slug}/used_by/";
        var response = await client.GetAsync(url_4b416c33, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4b416c33 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4b416c33;
    }

    /// <summary>
    /// GET /core/authenticated_sessions/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatedSessionsListAsync(string? session__last_ip = null, string? session__last_user_agent = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_930636e1 = $"api/v3/core/authenticated_sessions/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(session__last_ip)) queryParams.Add($"session__last_ip={session__last_ip}");
        if (!string.IsNullOrEmpty(session__last_user_agent)) queryParams.Add($"session__last_user_agent={session__last_user_agent}");
        if (!string.IsNullOrEmpty(user__username)) queryParams.Add($"user__username={user__username}");
        if (queryParams.Any()) url_930636e1 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_930636e1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_930636e1 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_930636e1 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /core/authenticated_sessions/{uuid}/
    /// </summary>
    public async Task<object?> AuthenticatedSessionsRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98041c0d = $"api/v3/core/authenticated_sessions/{uuid}/";
        var response = await client.GetAsync(url_98041c0d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98041c0d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98041c0d;
    }

    /// <summary>
    /// DELETE /core/authenticated_sessions/{uuid}/
    /// </summary>
    public async Task<object?> AuthenticatedSessionsDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6264a480 = $"api/v3/core/authenticated_sessions/{uuid}/";
        var response = await client.DeleteAsync(url_6264a480, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6264a480 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6264a480;
    }

    /// <summary>
    /// GET /core/authenticated_sessions/{uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatedSessionsUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_565f5214 = $"api/v3/core/authenticated_sessions/{uuid}/used_by/";
        var response = await client.GetAsync(url_565f5214, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_565f5214 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_565f5214;
    }

    /// <summary>
    /// GET /core/brands/
    /// </summary>
    public async Task<PaginatedResult<object>> BrandsListAsync(string? brand_uuid = null, string? branding_default_flow_background = null, string? branding_favicon = null, string? branding_logo = null, string? branding_title = null, string? client_certificates = null, bool? @default = null, string? domain = null, string? flow_authentication = null, string? flow_device_code = null, string? flow_invalidation = null, string? flow_recovery = null, string? flow_unenrollment = null, string? flow_user_settings = null, string? web_certificate = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f28845a8 = $"api/v3/core/brands/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(brand_uuid)) queryParams.Add($"brand_uuid={brand_uuid}");
        if (!string.IsNullOrEmpty(branding_default_flow_background)) queryParams.Add($"branding_default_flow_background={branding_default_flow_background}");
        if (!string.IsNullOrEmpty(branding_favicon)) queryParams.Add($"branding_favicon={branding_favicon}");
        if (!string.IsNullOrEmpty(branding_logo)) queryParams.Add($"branding_logo={branding_logo}");
        if (!string.IsNullOrEmpty(branding_title)) queryParams.Add($"branding_title={branding_title}");
        if (!string.IsNullOrEmpty(client_certificates)) queryParams.Add($"client_certificates={client_certificates}");
        if (@default.HasValue) queryParams.Add($"@default={@default.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(domain)) queryParams.Add($"domain={domain}");
        if (!string.IsNullOrEmpty(flow_authentication)) queryParams.Add($"flow_authentication={flow_authentication}");
        if (!string.IsNullOrEmpty(flow_device_code)) queryParams.Add($"flow_device_code={flow_device_code}");
        if (!string.IsNullOrEmpty(flow_invalidation)) queryParams.Add($"flow_invalidation={flow_invalidation}");
        if (!string.IsNullOrEmpty(flow_recovery)) queryParams.Add($"flow_recovery={flow_recovery}");
        if (!string.IsNullOrEmpty(flow_unenrollment)) queryParams.Add($"flow_unenrollment={flow_unenrollment}");
        if (!string.IsNullOrEmpty(flow_user_settings)) queryParams.Add($"flow_user_settings={flow_user_settings}");
        if (!string.IsNullOrEmpty(web_certificate)) queryParams.Add($"web_certificate={web_certificate}");
        if (queryParams.Any()) url_f28845a8 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f28845a8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f28845a8 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f28845a8 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/brands/
    /// </summary>
    public async Task<object?> BrandsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_09687a30 = $"api/v3/core/brands/";
        var response = await client.PostAsJsonAsync(url_09687a30, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_09687a30 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_09687a30;
    }

    /// <summary>
    /// GET /core/brands/{brand_uuid}/
    /// </summary>
    public async Task<object?> BrandsRetrieveAsync(string brand_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_00682610 = $"api/v3/core/brands/{brand_uuid}/";
        var response = await client.GetAsync(url_00682610, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_00682610 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_00682610;
    }

    /// <summary>
    /// PUT /core/brands/{brand_uuid}/
    /// </summary>
    public async Task<object?> BrandsUpdateAsync(string brand_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_740cd817 = $"api/v3/core/brands/{brand_uuid}/";
        var response = await client.PutAsJsonAsync(url_740cd817, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_740cd817 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_740cd817;
    }

    /// <summary>
    /// PATCH /core/brands/{brand_uuid}/
    /// </summary>
    public async Task<object?> BrandsPartialUpdateAsync(string brand_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0c025f4f = $"api/v3/core/brands/{brand_uuid}/";
        var response = await client.PatchAsJsonAsync(url_0c025f4f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0c025f4f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0c025f4f;
    }

    /// <summary>
    /// DELETE /core/brands/{brand_uuid}/
    /// </summary>
    public async Task<object?> BrandsDestroyAsync(string brand_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_419d91b2 = $"api/v3/core/brands/{brand_uuid}/";
        var response = await client.DeleteAsync(url_419d91b2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_419d91b2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_419d91b2;
    }

    /// <summary>
    /// GET /core/brands/{brand_uuid}/used_by/
    /// </summary>
    public async Task<object?> BrandsUsedByListAsync(string brand_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ff1da5b0 = $"api/v3/core/brands/{brand_uuid}/used_by/";
        var response = await client.GetAsync(url_ff1da5b0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ff1da5b0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ff1da5b0;
    }

    /// <summary>
    /// GET /core/brands/current/
    /// </summary>
    public async Task<PaginatedResult<object>> BrandsCurrentRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d69111d0 = $"api/v3/core/brands/current/";
        var response = await client.GetAsync(url_d69111d0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d69111d0 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d69111d0 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /core/groups/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupsListAsync(string? attributes = null, bool? include_children = null, bool? include_users = null, bool? is_superuser = null, string? members_by_pk = null, string? members_by_username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6612501a = $"api/v3/core/groups/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(attributes)) queryParams.Add($"attributes={attributes}");
        if (include_children.HasValue) queryParams.Add($"include_children={include_children.Value.ToString().ToLower()}");
        if (include_users.HasValue) queryParams.Add($"include_users={include_users.Value.ToString().ToLower()}");
        if (is_superuser.HasValue) queryParams.Add($"is_superuser={is_superuser.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(members_by_pk)) queryParams.Add($"members_by_pk={members_by_pk}");
        if (!string.IsNullOrEmpty(members_by_username)) queryParams.Add($"members_by_username={members_by_username}");
        if (queryParams.Any()) url_6612501a += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_6612501a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6612501a = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6612501a ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/groups/
    /// </summary>
    public async Task<object?> GroupsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9e8492d6 = $"api/v3/core/groups/";
        var response = await client.PostAsJsonAsync(url_9e8492d6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9e8492d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9e8492d6;
    }

    /// <summary>
    /// GET /core/groups/{group_uuid}/
    /// </summary>
    public async Task<object?> GroupsRetrieveAsync(string group_uuid, bool? include_children = null, bool? include_users = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4ffb3a2 = $"api/v3/core/groups/{group_uuid}/";
        var queryParams = new List<string>();
        if (include_children.HasValue) queryParams.Add($"include_children={include_children.Value.ToString().ToLower()}");
        if (include_users.HasValue) queryParams.Add($"include_users={include_users.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_c4ffb3a2 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_c4ffb3a2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4ffb3a2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4ffb3a2;
    }

    /// <summary>
    /// PUT /core/groups/{group_uuid}/
    /// </summary>
    public async Task<object?> GroupsUpdateAsync(string group_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_45d47bf9 = $"api/v3/core/groups/{group_uuid}/";
        var response = await client.PutAsJsonAsync(url_45d47bf9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_45d47bf9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_45d47bf9;
    }

    /// <summary>
    /// PATCH /core/groups/{group_uuid}/
    /// </summary>
    public async Task<object?> GroupsPartialUpdateAsync(string group_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_03a6a3db = $"api/v3/core/groups/{group_uuid}/";
        var response = await client.PatchAsJsonAsync(url_03a6a3db, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_03a6a3db = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_03a6a3db;
    }

    /// <summary>
    /// DELETE /core/groups/{group_uuid}/
    /// </summary>
    public async Task<object?> GroupsDestroyAsync(string group_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_37e61e33 = $"api/v3/core/groups/{group_uuid}/";
        var response = await client.DeleteAsync(url_37e61e33, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_37e61e33 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_37e61e33;
    }

    /// <summary>
    /// POST /core/groups/{group_uuid}/add_user/
    /// </summary>
    public async Task<object?> GroupsAddUserCreateAsync(string group_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8f66705f = $"api/v3/core/groups/{group_uuid}/add_user/";
        var response = await client.PostAsJsonAsync(url_8f66705f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8f66705f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8f66705f;
    }

    /// <summary>
    /// POST /core/groups/{group_uuid}/remove_user/
    /// </summary>
    public async Task<object?> GroupsRemoveUserCreateAsync(string group_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98a0ea55 = $"api/v3/core/groups/{group_uuid}/remove_user/";
        var response = await client.PostAsJsonAsync(url_98a0ea55, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98a0ea55 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98a0ea55;
    }

    /// <summary>
    /// GET /core/groups/{group_uuid}/used_by/
    /// </summary>
    public async Task<object?> GroupsUsedByListAsync(string group_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_10a1b7e7 = $"api/v3/core/groups/{group_uuid}/used_by/";
        var response = await client.GetAsync(url_10a1b7e7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_10a1b7e7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_10a1b7e7;
    }

    /// <summary>
    /// GET /core/tokens/
    /// </summary>
    public async Task<PaginatedResult<object>> TokensListAsync(string? description = null, string? expires = null, bool? expiring = null, string? identifier = null, string? intent = null, string? managed = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_23c85824 = $"api/v3/core/tokens/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(description)) queryParams.Add($"description={description}");
        if (!string.IsNullOrEmpty(expires)) queryParams.Add($"expires={expires}");
        if (expiring.HasValue) queryParams.Add($"expiring={expiring.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(identifier)) queryParams.Add($"identifier={identifier}");
        if (!string.IsNullOrEmpty(intent)) queryParams.Add($"intent={intent}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (!string.IsNullOrEmpty(user__username)) queryParams.Add($"user__username={user__username}");
        if (queryParams.Any()) url_23c85824 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_23c85824, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_23c85824 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_23c85824 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/tokens/
    /// </summary>
    public async Task<object?> TokensCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6c7614b7 = $"api/v3/core/tokens/";
        var response = await client.PostAsJsonAsync(url_6c7614b7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6c7614b7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6c7614b7;
    }

    /// <summary>
    /// GET /core/tokens/{identifier}/
    /// </summary>
    public async Task<object?> TokensRetrieveAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5dd20519 = $"api/v3/core/tokens/{identifier}/";
        var response = await client.GetAsync(url_5dd20519, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5dd20519 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5dd20519;
    }

    /// <summary>
    /// PUT /core/tokens/{identifier}/
    /// </summary>
    public async Task<object?> TokensUpdateAsync(string identifier, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4f805750 = $"api/v3/core/tokens/{identifier}/";
        var response = await client.PutAsJsonAsync(url_4f805750, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4f805750 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4f805750;
    }

    /// <summary>
    /// PATCH /core/tokens/{identifier}/
    /// </summary>
    public async Task<object?> TokensPartialUpdateAsync(string identifier, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8a7ad44d = $"api/v3/core/tokens/{identifier}/";
        var response = await client.PatchAsJsonAsync(url_8a7ad44d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8a7ad44d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8a7ad44d;
    }

    /// <summary>
    /// DELETE /core/tokens/{identifier}/
    /// </summary>
    public async Task<object?> TokensDestroyAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_602261fb = $"api/v3/core/tokens/{identifier}/";
        var response = await client.DeleteAsync(url_602261fb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_602261fb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_602261fb;
    }

    /// <summary>
    /// POST /core/tokens/{identifier}/set_key/
    /// </summary>
    public async Task<object?> TokensSetKeyCreateAsync(string identifier, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fc349a34 = $"api/v3/core/tokens/{identifier}/set_key/";
        var response = await client.PostAsJsonAsync(url_fc349a34, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fc349a34 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fc349a34;
    }

    /// <summary>
    /// GET /core/tokens/{identifier}/used_by/
    /// </summary>
    public async Task<object?> TokensUsedByListAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3bc184c8 = $"api/v3/core/tokens/{identifier}/used_by/";
        var response = await client.GetAsync(url_3bc184c8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3bc184c8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3bc184c8;
    }

    /// <summary>
    /// GET /core/tokens/{identifier}/view_key/
    /// </summary>
    public async Task<object?> TokensViewKeyRetrieveAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_48c00651 = $"api/v3/core/tokens/{identifier}/view_key/";
        var response = await client.GetAsync(url_48c00651, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_48c00651 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_48c00651;
    }

    /// <summary>
    /// PUT /core/transactional/applications/
    /// </summary>
    public async Task<object?> TransactionalApplicationsUpdateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c02c0228 = $"api/v3/core/transactional/applications/";
        var response = await client.PutAsJsonAsync(url_c02c0228, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c02c0228 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c02c0228;
    }

    /// <summary>
    /// GET /core/user_consent/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConsentListAsync(string? application = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f5662750 = $"api/v3/core/user_consent/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(application)) queryParams.Add($"application={application}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_f5662750 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f5662750, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f5662750 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f5662750 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /core/user_consent/{id}/
    /// </summary>
    public async Task<object?> UserConsentRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_efb89199 = $"api/v3/core/user_consent/{id}/";
        var response = await client.GetAsync(url_efb89199, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_efb89199 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_efb89199;
    }

    /// <summary>
    /// DELETE /core/user_consent/{id}/
    /// </summary>
    public async Task<object?> UserConsentDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d03c2ac5 = $"api/v3/core/user_consent/{id}/";
        var response = await client.DeleteAsync(url_d03c2ac5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d03c2ac5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d03c2ac5;
    }

    /// <summary>
    /// GET /core/user_consent/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConsentUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_82a93ab4 = $"api/v3/core/user_consent/{id}/used_by/";
        var response = await client.GetAsync(url_82a93ab4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_82a93ab4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_82a93ab4;
    }

    /// <summary>
    /// GET /core/users/
    /// </summary>
    public async Task<PaginatedResult<object>> UsersListAsync(string? attributes = null, string? date_joined = null, string? date_joined__gt = null, string? date_joined__lt = null, string? email = null, string? groups_by_name = null, string? groups_by_pk = null, bool? include_groups = null, bool? is_active = null, bool? is_superuser = null, string? last_updated = null, string? last_updated__gt = null, string? last_updated__lt = null, string? path = null, string? path_startswith = null, string? type = null, string? username = null, string? uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3cac0e0d = $"api/v3/core/users/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(attributes)) queryParams.Add($"attributes={attributes}");
        if (!string.IsNullOrEmpty(date_joined)) queryParams.Add($"date_joined={date_joined}");
        if (!string.IsNullOrEmpty(date_joined__gt)) queryParams.Add($"date_joined__gt={date_joined__gt}");
        if (!string.IsNullOrEmpty(date_joined__lt)) queryParams.Add($"date_joined__lt={date_joined__lt}");
        if (!string.IsNullOrEmpty(email)) queryParams.Add($"email={email}");
        if (!string.IsNullOrEmpty(groups_by_name)) queryParams.Add($"groups_by_name={groups_by_name}");
        if (!string.IsNullOrEmpty(groups_by_pk)) queryParams.Add($"groups_by_pk={groups_by_pk}");
        if (include_groups.HasValue) queryParams.Add($"include_groups={include_groups.Value.ToString().ToLower()}");
        if (is_active.HasValue) queryParams.Add($"is_active={is_active.Value.ToString().ToLower()}");
        if (is_superuser.HasValue) queryParams.Add($"is_superuser={is_superuser.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(last_updated)) queryParams.Add($"last_updated={last_updated}");
        if (!string.IsNullOrEmpty(last_updated__gt)) queryParams.Add($"last_updated__gt={last_updated__gt}");
        if (!string.IsNullOrEmpty(last_updated__lt)) queryParams.Add($"last_updated__lt={last_updated__lt}");
        if (!string.IsNullOrEmpty(path)) queryParams.Add($"path={path}");
        if (!string.IsNullOrEmpty(path_startswith)) queryParams.Add($"path_startswith={path_startswith}");
        if (!string.IsNullOrEmpty(type)) queryParams.Add($"type={type}");
        if (!string.IsNullOrEmpty(username)) queryParams.Add($"username={username}");
        if (!string.IsNullOrEmpty(uuid)) queryParams.Add($"uuid={uuid}");
        if (queryParams.Any()) url_3cac0e0d += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_3cac0e0d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3cac0e0d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3cac0e0d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/users/
    /// </summary>
    public async Task<object?> UsersCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e8ce84a6 = $"api/v3/core/users/";
        var response = await client.PostAsJsonAsync(url_e8ce84a6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e8ce84a6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e8ce84a6;
    }

    /// <summary>
    /// GET /core/users/{id}/
    /// </summary>
    public async Task<object?> UsersRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d41610c1 = $"api/v3/core/users/{id}/";
        var response = await client.GetAsync(url_d41610c1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d41610c1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d41610c1;
    }

    /// <summary>
    /// PUT /core/users/{id}/
    /// </summary>
    public async Task<object?> UsersUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6adbea99 = $"api/v3/core/users/{id}/";
        var response = await client.PutAsJsonAsync(url_6adbea99, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6adbea99 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6adbea99;
    }

    /// <summary>
    /// PATCH /core/users/{id}/
    /// </summary>
    public async Task<object?> UsersPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e2b92501 = $"api/v3/core/users/{id}/";
        var response = await client.PatchAsJsonAsync(url_e2b92501, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e2b92501 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e2b92501;
    }

    /// <summary>
    /// DELETE /core/users/{id}/
    /// </summary>
    public async Task<object?> UsersDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8dcb2ac3 = $"api/v3/core/users/{id}/";
        var response = await client.DeleteAsync(url_8dcb2ac3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8dcb2ac3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8dcb2ac3;
    }

    /// <summary>
    /// POST /core/users/{id}/impersonate/
    /// </summary>
    public async Task<object?> UsersImpersonateCreateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ef45a987 = $"api/v3/core/users/{id}/impersonate/";
        var response = await client.PostAsJsonAsync(url_ef45a987, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ef45a987 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ef45a987;
    }

    /// <summary>
    /// POST /core/users/{id}/recovery/
    /// </summary>
    public async Task<object?> UsersRecoveryCreateAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cb1354a3 = $"api/v3/core/users/{id}/recovery/";
        var response = await client.PostAsync(url_cb1354a3, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cb1354a3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cb1354a3;
    }

    /// <summary>
    /// POST /core/users/{id}/recovery_email/
    /// </summary>
    public async Task<object?> UsersRecoveryEmailCreateAsync(int id, string? email_stage = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1a0f11fe = $"api/v3/core/users/{id}/recovery_email/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(email_stage)) queryParams.Add($"email_stage={email_stage}");
        if (queryParams.Any()) url_1a0f11fe += "?" + string.Join("&", queryParams);
        var response = await client.PostAsync(url_1a0f11fe, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1a0f11fe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1a0f11fe;
    }

    /// <summary>
    /// POST /core/users/{id}/set_password/
    /// </summary>
    public async Task<object?> UsersSetPasswordCreateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d4c629b0 = $"api/v3/core/users/{id}/set_password/";
        var response = await client.PostAsJsonAsync(url_d4c629b0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d4c629b0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d4c629b0;
    }

    /// <summary>
    /// GET /core/users/{id}/used_by/
    /// </summary>
    public async Task<object?> UsersUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_17a0d441 = $"api/v3/core/users/{id}/used_by/";
        var response = await client.GetAsync(url_17a0d441, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_17a0d441 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_17a0d441;
    }

    /// <summary>
    /// GET /core/users/impersonate_end/
    /// </summary>
    public async Task<PaginatedResult<object>> UsersImpersonateEndRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d06e6148 = $"api/v3/core/users/impersonate_end/";
        var response = await client.GetAsync(url_d06e6148, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d06e6148 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d06e6148 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /core/users/me/
    /// </summary>
    public async Task<PaginatedResult<object>> UsersMeRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75c02202 = $"api/v3/core/users/me/";
        var response = await client.GetAsync(url_75c02202, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75c02202 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75c02202 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /core/users/paths/
    /// </summary>
    public async Task<PaginatedResult<object>> UsersPathsRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5824bf9b = $"api/v3/core/users/paths/";
        var response = await client.GetAsync(url_5824bf9b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5824bf9b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5824bf9b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /core/users/service_account/
    /// </summary>
    public async Task<object?> UsersServiceAccountCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ef161226 = $"api/v3/core/users/service_account/";
        var response = await client.PostAsJsonAsync(url_ef161226, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ef161226 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ef161226;
    }

    /// <summary>
    /// Gets a user by ID (backward compatibility wrapper).
    /// </summary>
    public async Task<object?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        // Try to parse as int (Authentik uses int IDs)
        if (int.TryParse(userId, out int userIdInt))
        {
            return await UsersRetrieveAsync(userIdInt, cancellationToken);
        }

        // Fallback: use string ID directly
        var client = GetHttpClient();
        var response = await client.GetAsync($"api/v3/core/users/{userId}/", cancellationToken);
        return await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
    }

    /// <summary>
    /// Lists users with pagination (backward compatibility wrapper).
    /// </summary>
    public async Task<PaginatedResult<object>> ListUsersAsync(int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)
    {
        // Call the generated method with minimal parameters
        // The generated method has many optional filters, but we just need pagination
        return await UsersListAsync(cancellationToken: cancellationToken);
    }

}
