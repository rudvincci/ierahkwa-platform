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
/// Service implementation for Authentik Sources API operations.
/// </summary>
public class AuthentikSourcesService : IAuthentikSourcesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikSourcesService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikSourcesService"/> class.
    /// </summary>
    public AuthentikSourcesService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikSourcesService> logger,
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
    /// GET /sources/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(string? managed = null, string? pbm_uuid = null, string? slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/sources/all/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (queryParams.Any()) url_f372512f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/all/{slug}/
    /// </summary>
    public async Task<object?> AllRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95226f05 = $"api/v3/sources/all/{slug}/";
        var response = await client.GetAsync(url_95226f05, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95226f05 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95226f05;
    }

    /// <summary>
    /// DELETE /sources/all/{slug}/
    /// </summary>
    public async Task<object?> AllDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b02df8d9 = $"api/v3/sources/all/{slug}/";
        var response = await client.DeleteAsync(url_b02df8d9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b02df8d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b02df8d9;
    }

    /// <summary>
    /// POST /sources/all/{slug}/set_icon/
    /// </summary>
    public async Task<object?> AllSetIconCreateAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a1028b96 = $"api/v3/sources/all/{slug}/set_icon/";
        var response = await client.PostAsync(url_a1028b96, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a1028b96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a1028b96;
    }

    /// <summary>
    /// POST /sources/all/{slug}/set_icon_url/
    /// </summary>
    public async Task<object?> AllSetIconUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b696db3 = $"api/v3/sources/all/{slug}/set_icon_url/";
        var response = await client.PostAsJsonAsync(url_5b696db3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b696db3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b696db3;
    }

    /// <summary>
    /// GET /sources/all/{slug}/used_by/
    /// </summary>
    public async Task<object?> AllUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a42dde8b = $"api/v3/sources/all/{slug}/used_by/";
        var response = await client.GetAsync(url_a42dde8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a42dde8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a42dde8b;
    }

    /// <summary>
    /// GET /sources/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf388636 = $"api/v3/sources/all/types/";
        var response = await client.GetAsync(url_bf388636, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf388636 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf388636 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/all/user_settings/
    /// </summary>
    public async Task<PaginatedResult<object>> AllUserSettingsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a72c654e = $"api/v3/sources/all/user_settings/";
        var response = await client.GetAsync(url_a72c654e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a72c654e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a72c654e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/group_connections/all/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsAllListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6c08b51b = $"api/v3/sources/group_connections/all/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_6c08b51b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_6c08b51b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6c08b51b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6c08b51b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/group_connections/all/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsAllRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_35492beb = $"api/v3/sources/group_connections/all/{id}/";
        var response = await client.GetAsync(url_35492beb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_35492beb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_35492beb;
    }

    /// <summary>
    /// PUT /sources/group_connections/all/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsAllUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_74cc3716 = $"api/v3/sources/group_connections/all/{id}/";
        var response = await client.PutAsJsonAsync(url_74cc3716, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_74cc3716 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_74cc3716;
    }

    /// <summary>
    /// PATCH /sources/group_connections/all/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsAllPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1e3b1b21 = $"api/v3/sources/group_connections/all/{id}/";
        var response = await client.PatchAsJsonAsync(url_1e3b1b21, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1e3b1b21 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1e3b1b21;
    }

    /// <summary>
    /// DELETE /sources/group_connections/all/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsAllDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eed117aa = $"api/v3/sources/group_connections/all/{id}/";
        var response = await client.DeleteAsync(url_eed117aa, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eed117aa = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eed117aa;
    }

    /// <summary>
    /// GET /sources/group_connections/all/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsAllUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_40ae042f = $"api/v3/sources/group_connections/all/{id}/used_by/";
        var response = await client.GetAsync(url_40ae042f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_40ae042f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_40ae042f;
    }

    /// <summary>
    /// GET /sources/group_connections/kerberos/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsKerberosListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b3d6ee1 = $"api/v3/sources/group_connections/kerberos/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_5b3d6ee1 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_5b3d6ee1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b3d6ee1 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b3d6ee1 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/kerberos/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a754bb7b = $"api/v3/sources/group_connections/kerberos/";
        var response = await client.PostAsJsonAsync(url_a754bb7b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a754bb7b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a754bb7b;
    }

    /// <summary>
    /// GET /sources/group_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5d7cdc7a = $"api/v3/sources/group_connections/kerberos/{id}/";
        var response = await client.GetAsync(url_5d7cdc7a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5d7cdc7a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5d7cdc7a;
    }

    /// <summary>
    /// PUT /sources/group_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_92616512 = $"api/v3/sources/group_connections/kerberos/{id}/";
        var response = await client.PutAsJsonAsync(url_92616512, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_92616512 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_92616512;
    }

    /// <summary>
    /// PATCH /sources/group_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_171c0ec5 = $"api/v3/sources/group_connections/kerberos/{id}/";
        var response = await client.PatchAsJsonAsync(url_171c0ec5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_171c0ec5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_171c0ec5;
    }

    /// <summary>
    /// DELETE /sources/group_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0d73f5ca = $"api/v3/sources/group_connections/kerberos/{id}/";
        var response = await client.DeleteAsync(url_0d73f5ca, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0d73f5ca = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0d73f5ca;
    }

    /// <summary>
    /// GET /sources/group_connections/kerberos/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsKerberosUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b215c701 = $"api/v3/sources/group_connections/kerberos/{id}/used_by/";
        var response = await client.GetAsync(url_b215c701, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b215c701 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b215c701;
    }

    /// <summary>
    /// GET /sources/group_connections/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsLdapListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_aa0b2d80 = $"api/v3/sources/group_connections/ldap/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_aa0b2d80 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_aa0b2d80, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_aa0b2d80 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_aa0b2d80 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/ldap/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0dbb90f1 = $"api/v3/sources/group_connections/ldap/";
        var response = await client.PostAsJsonAsync(url_0dbb90f1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0dbb90f1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0dbb90f1;
    }

    /// <summary>
    /// GET /sources/group_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c3cc4161 = $"api/v3/sources/group_connections/ldap/{id}/";
        var response = await client.GetAsync(url_c3cc4161, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c3cc4161 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c3cc4161;
    }

    /// <summary>
    /// PUT /sources/group_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_637bfec2 = $"api/v3/sources/group_connections/ldap/{id}/";
        var response = await client.PutAsJsonAsync(url_637bfec2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_637bfec2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_637bfec2;
    }

    /// <summary>
    /// PATCH /sources/group_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_09717d28 = $"api/v3/sources/group_connections/ldap/{id}/";
        var response = await client.PatchAsJsonAsync(url_09717d28, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_09717d28 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_09717d28;
    }

    /// <summary>
    /// DELETE /sources/group_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_05fa3e2d = $"api/v3/sources/group_connections/ldap/{id}/";
        var response = await client.DeleteAsync(url_05fa3e2d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_05fa3e2d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_05fa3e2d;
    }

    /// <summary>
    /// GET /sources/group_connections/ldap/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsLdapUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_43508869 = $"api/v3/sources/group_connections/ldap/{id}/used_by/";
        var response = await client.GetAsync(url_43508869, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_43508869 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_43508869;
    }

    /// <summary>
    /// GET /sources/group_connections/oauth/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsOauthListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1856133f = $"api/v3/sources/group_connections/oauth/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_1856133f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_1856133f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1856133f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1856133f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/oauth/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_622b9a4e = $"api/v3/sources/group_connections/oauth/";
        var response = await client.PostAsJsonAsync(url_622b9a4e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_622b9a4e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_622b9a4e;
    }

    /// <summary>
    /// GET /sources/group_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_99a7b338 = $"api/v3/sources/group_connections/oauth/{id}/";
        var response = await client.GetAsync(url_99a7b338, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_99a7b338 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_99a7b338;
    }

    /// <summary>
    /// PUT /sources/group_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c50cbfd1 = $"api/v3/sources/group_connections/oauth/{id}/";
        var response = await client.PutAsJsonAsync(url_c50cbfd1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c50cbfd1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c50cbfd1;
    }

    /// <summary>
    /// PATCH /sources/group_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_03ef67e9 = $"api/v3/sources/group_connections/oauth/{id}/";
        var response = await client.PatchAsJsonAsync(url_03ef67e9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_03ef67e9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_03ef67e9;
    }

    /// <summary>
    /// DELETE /sources/group_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e3967087 = $"api/v3/sources/group_connections/oauth/{id}/";
        var response = await client.DeleteAsync(url_e3967087, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e3967087 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e3967087;
    }

    /// <summary>
    /// GET /sources/group_connections/oauth/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsOauthUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b7f54876 = $"api/v3/sources/group_connections/oauth/{id}/used_by/";
        var response = await client.GetAsync(url_b7f54876, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b7f54876 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b7f54876;
    }

    /// <summary>
    /// GET /sources/group_connections/plex/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsPlexListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8ee3120e = $"api/v3/sources/group_connections/plex/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_8ee3120e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_8ee3120e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8ee3120e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8ee3120e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/plex/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95031791 = $"api/v3/sources/group_connections/plex/";
        var response = await client.PostAsJsonAsync(url_95031791, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95031791 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95031791;
    }

    /// <summary>
    /// GET /sources/group_connections/plex/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5fbeae94 = $"api/v3/sources/group_connections/plex/{id}/";
        var response = await client.GetAsync(url_5fbeae94, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5fbeae94 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5fbeae94;
    }

    /// <summary>
    /// PUT /sources/group_connections/plex/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_25bfe04f = $"api/v3/sources/group_connections/plex/{id}/";
        var response = await client.PutAsJsonAsync(url_25bfe04f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_25bfe04f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_25bfe04f;
    }

    /// <summary>
    /// PATCH /sources/group_connections/plex/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_88c6977c = $"api/v3/sources/group_connections/plex/{id}/";
        var response = await client.PatchAsJsonAsync(url_88c6977c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_88c6977c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_88c6977c;
    }

    /// <summary>
    /// DELETE /sources/group_connections/plex/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_26a415fc = $"api/v3/sources/group_connections/plex/{id}/";
        var response = await client.DeleteAsync(url_26a415fc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_26a415fc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_26a415fc;
    }

    /// <summary>
    /// GET /sources/group_connections/plex/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsPlexUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_91604938 = $"api/v3/sources/group_connections/plex/{id}/used_by/";
        var response = await client.GetAsync(url_91604938, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_91604938 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_91604938;
    }

    /// <summary>
    /// GET /sources/group_connections/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsSamlListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a4574fe9 = $"api/v3/sources/group_connections/saml/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_a4574fe9 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_a4574fe9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a4574fe9 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a4574fe9 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/saml/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3b132383 = $"api/v3/sources/group_connections/saml/";
        var response = await client.PostAsJsonAsync(url_3b132383, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3b132383 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3b132383;
    }

    /// <summary>
    /// GET /sources/group_connections/saml/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_036a04e2 = $"api/v3/sources/group_connections/saml/{id}/";
        var response = await client.GetAsync(url_036a04e2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_036a04e2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_036a04e2;
    }

    /// <summary>
    /// PUT /sources/group_connections/saml/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7ae2ffed = $"api/v3/sources/group_connections/saml/{id}/";
        var response = await client.PutAsJsonAsync(url_7ae2ffed, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7ae2ffed = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7ae2ffed;
    }

    /// <summary>
    /// PATCH /sources/group_connections/saml/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_61d82a6e = $"api/v3/sources/group_connections/saml/{id}/";
        var response = await client.PatchAsJsonAsync(url_61d82a6e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_61d82a6e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_61d82a6e;
    }

    /// <summary>
    /// DELETE /sources/group_connections/saml/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5fc76e39 = $"api/v3/sources/group_connections/saml/{id}/";
        var response = await client.DeleteAsync(url_5fc76e39, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5fc76e39 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5fc76e39;
    }

    /// <summary>
    /// GET /sources/group_connections/saml/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsSamlUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4039067c = $"api/v3/sources/group_connections/saml/{id}/used_by/";
        var response = await client.GetAsync(url_4039067c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4039067c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4039067c;
    }

    /// <summary>
    /// GET /sources/group_connections/telegram/
    /// </summary>
    public async Task<PaginatedResult<object>> GroupConnectionsTelegramListAsync(string? group = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4a94a357 = $"api/v3/sources/group_connections/telegram/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group)) queryParams.Add($"group={group}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_4a94a357 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_4a94a357, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4a94a357 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4a94a357 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/group_connections/telegram/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4c33243b = $"api/v3/sources/group_connections/telegram/";
        var response = await client.PostAsJsonAsync(url_4c33243b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4c33243b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4c33243b;
    }

    /// <summary>
    /// GET /sources/group_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f8265502 = $"api/v3/sources/group_connections/telegram/{id}/";
        var response = await client.GetAsync(url_f8265502, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f8265502 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f8265502;
    }

    /// <summary>
    /// PUT /sources/group_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fc9868e8 = $"api/v3/sources/group_connections/telegram/{id}/";
        var response = await client.PutAsJsonAsync(url_fc9868e8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fc9868e8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fc9868e8;
    }

    /// <summary>
    /// PATCH /sources/group_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7df42b43 = $"api/v3/sources/group_connections/telegram/{id}/";
        var response = await client.PatchAsJsonAsync(url_7df42b43, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7df42b43 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7df42b43;
    }

    /// <summary>
    /// DELETE /sources/group_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf42fcb6 = $"api/v3/sources/group_connections/telegram/{id}/";
        var response = await client.DeleteAsync(url_bf42fcb6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf42fcb6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf42fcb6;
    }

    /// <summary>
    /// GET /sources/group_connections/telegram/{id}/used_by/
    /// </summary>
    public async Task<object?> GroupConnectionsTelegramUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_326604c3 = $"api/v3/sources/group_connections/telegram/{id}/used_by/";
        var response = await client.GetAsync(url_326604c3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_326604c3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_326604c3;
    }

    /// <summary>
    /// GET /sources/kerberos/
    /// </summary>
    public async Task<PaginatedResult<object>> KerberosListAsync(bool? enabled = null, string? kadmin_type = null, bool? password_login_update_internal_password = null, string? pbm_uuid = null, string? realm = null, string? slug = null, string? spnego_server_name = null, string? sync_principal = null, bool? sync_users = null, bool? sync_users_password = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3ed2d5c8 = $"api/v3/sources/kerberos/";
        var queryParams = new List<string>();
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(kadmin_type)) queryParams.Add($"kadmin_type={kadmin_type}");
        if (password_login_update_internal_password.HasValue) queryParams.Add($"password_login_update_internal_password={password_login_update_internal_password.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(realm)) queryParams.Add($"realm={realm}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (!string.IsNullOrEmpty(spnego_server_name)) queryParams.Add($"spnego_server_name={spnego_server_name}");
        if (!string.IsNullOrEmpty(sync_principal)) queryParams.Add($"sync_principal={sync_principal}");
        if (sync_users.HasValue) queryParams.Add($"sync_users={sync_users.Value.ToString().ToLower()}");
        if (sync_users_password.HasValue) queryParams.Add($"sync_users_password={sync_users_password.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_3ed2d5c8 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_3ed2d5c8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3ed2d5c8 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3ed2d5c8 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/kerberos/
    /// </summary>
    public async Task<object?> KerberosCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1de785bd = $"api/v3/sources/kerberos/";
        var response = await client.PostAsJsonAsync(url_1de785bd, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1de785bd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1de785bd;
    }

    /// <summary>
    /// GET /sources/kerberos/{slug}/
    /// </summary>
    public async Task<object?> KerberosRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1e5be295 = $"api/v3/sources/kerberos/{slug}/";
        var response = await client.GetAsync(url_1e5be295, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1e5be295 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1e5be295;
    }

    /// <summary>
    /// PUT /sources/kerberos/{slug}/
    /// </summary>
    public async Task<object?> KerberosUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_80439f4e = $"api/v3/sources/kerberos/{slug}/";
        var response = await client.PutAsJsonAsync(url_80439f4e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_80439f4e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_80439f4e;
    }

    /// <summary>
    /// PATCH /sources/kerberos/{slug}/
    /// </summary>
    public async Task<object?> KerberosPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_87761ab7 = $"api/v3/sources/kerberos/{slug}/";
        var response = await client.PatchAsJsonAsync(url_87761ab7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_87761ab7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_87761ab7;
    }

    /// <summary>
    /// DELETE /sources/kerberos/{slug}/
    /// </summary>
    public async Task<object?> KerberosDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_db1393e4 = $"api/v3/sources/kerberos/{slug}/";
        var response = await client.DeleteAsync(url_db1393e4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_db1393e4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_db1393e4;
    }

    /// <summary>
    /// GET /sources/kerberos/{slug}/sync/status/
    /// </summary>
    public async Task<object?> KerberosSyncStatusRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a0678021 = $"api/v3/sources/kerberos/{slug}/sync/status/";
        var response = await client.GetAsync(url_a0678021, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a0678021 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a0678021;
    }

    /// <summary>
    /// GET /sources/kerberos/{slug}/used_by/
    /// </summary>
    public async Task<object?> KerberosUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7a7fd3c8 = $"api/v3/sources/kerberos/{slug}/used_by/";
        var response = await client.GetAsync(url_7a7fd3c8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7a7fd3c8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7a7fd3c8;
    }

    /// <summary>
    /// GET /sources/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> LdapListAsync(string? additional_group_dn = null, string? additional_user_dn = null, string? base_dn = null, string? bind_cn = null, string? client_certificate = null, bool? delete_not_found_objects = null, bool? enabled = null, string? group_membership_field = null, string? group_object_filter = null, string? group_property_mappings = null, bool? lookup_groups_from_user = null, string? object_uniqueness_field = null, bool? password_login_update_internal_password = null, string? pbm_uuid = null, string? peer_certificate = null, string? server_uri = null, string? slug = null, bool? sni = null, bool? start_tls = null, bool? sync_groups = null, string? sync_parent_group = null, bool? sync_users = null, bool? sync_users_password = null, string? user_membership_attribute = null, string? user_object_filter = null, string? user_property_mappings = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b4c9b420 = $"api/v3/sources/ldap/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(additional_group_dn)) queryParams.Add($"additional_group_dn={additional_group_dn}");
        if (!string.IsNullOrEmpty(additional_user_dn)) queryParams.Add($"additional_user_dn={additional_user_dn}");
        if (!string.IsNullOrEmpty(base_dn)) queryParams.Add($"base_dn={base_dn}");
        if (!string.IsNullOrEmpty(bind_cn)) queryParams.Add($"bind_cn={bind_cn}");
        if (!string.IsNullOrEmpty(client_certificate)) queryParams.Add($"client_certificate={client_certificate}");
        if (delete_not_found_objects.HasValue) queryParams.Add($"delete_not_found_objects={delete_not_found_objects.Value.ToString().ToLower()}");
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(group_membership_field)) queryParams.Add($"group_membership_field={group_membership_field}");
        if (!string.IsNullOrEmpty(group_object_filter)) queryParams.Add($"group_object_filter={group_object_filter}");
        if (!string.IsNullOrEmpty(group_property_mappings)) queryParams.Add($"group_property_mappings={group_property_mappings}");
        if (lookup_groups_from_user.HasValue) queryParams.Add($"lookup_groups_from_user={lookup_groups_from_user.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(object_uniqueness_field)) queryParams.Add($"object_uniqueness_field={object_uniqueness_field}");
        if (password_login_update_internal_password.HasValue) queryParams.Add($"password_login_update_internal_password={password_login_update_internal_password.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(peer_certificate)) queryParams.Add($"peer_certificate={peer_certificate}");
        if (!string.IsNullOrEmpty(server_uri)) queryParams.Add($"server_uri={server_uri}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (sni.HasValue) queryParams.Add($"sni={sni.Value.ToString().ToLower()}");
        if (start_tls.HasValue) queryParams.Add($"start_tls={start_tls.Value.ToString().ToLower()}");
        if (sync_groups.HasValue) queryParams.Add($"sync_groups={sync_groups.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(sync_parent_group)) queryParams.Add($"sync_parent_group={sync_parent_group}");
        if (sync_users.HasValue) queryParams.Add($"sync_users={sync_users.Value.ToString().ToLower()}");
        if (sync_users_password.HasValue) queryParams.Add($"sync_users_password={sync_users_password.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(user_membership_attribute)) queryParams.Add($"user_membership_attribute={user_membership_attribute}");
        if (!string.IsNullOrEmpty(user_object_filter)) queryParams.Add($"user_object_filter={user_object_filter}");
        if (!string.IsNullOrEmpty(user_property_mappings)) queryParams.Add($"user_property_mappings={user_property_mappings}");
        if (queryParams.Any()) url_b4c9b420 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b4c9b420, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b4c9b420 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b4c9b420 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/ldap/
    /// </summary>
    public async Task<object?> LdapCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_74eb0f9d = $"api/v3/sources/ldap/";
        var response = await client.PostAsJsonAsync(url_74eb0f9d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_74eb0f9d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_74eb0f9d;
    }

    /// <summary>
    /// GET /sources/ldap/{slug}/
    /// </summary>
    public async Task<object?> LdapRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fbe24456 = $"api/v3/sources/ldap/{slug}/";
        var response = await client.GetAsync(url_fbe24456, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fbe24456 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fbe24456;
    }

    /// <summary>
    /// PUT /sources/ldap/{slug}/
    /// </summary>
    public async Task<object?> LdapUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8ed55908 = $"api/v3/sources/ldap/{slug}/";
        var response = await client.PutAsJsonAsync(url_8ed55908, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8ed55908 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8ed55908;
    }

    /// <summary>
    /// PATCH /sources/ldap/{slug}/
    /// </summary>
    public async Task<object?> LdapPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5baec692 = $"api/v3/sources/ldap/{slug}/";
        var response = await client.PatchAsJsonAsync(url_5baec692, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5baec692 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5baec692;
    }

    /// <summary>
    /// DELETE /sources/ldap/{slug}/
    /// </summary>
    public async Task<object?> LdapDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4a892714 = $"api/v3/sources/ldap/{slug}/";
        var response = await client.DeleteAsync(url_4a892714, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4a892714 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4a892714;
    }

    /// <summary>
    /// GET /sources/ldap/{slug}/debug/
    /// </summary>
    public async Task<object?> LdapDebugRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7165ab5f = $"api/v3/sources/ldap/{slug}/debug/";
        var response = await client.GetAsync(url_7165ab5f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7165ab5f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7165ab5f;
    }

    /// <summary>
    /// GET /sources/ldap/{slug}/sync/status/
    /// </summary>
    public async Task<object?> LdapSyncStatusRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2f1d9bc3 = $"api/v3/sources/ldap/{slug}/sync/status/";
        var response = await client.GetAsync(url_2f1d9bc3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2f1d9bc3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2f1d9bc3;
    }

    /// <summary>
    /// GET /sources/ldap/{slug}/used_by/
    /// </summary>
    public async Task<object?> LdapUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_22142f89 = $"api/v3/sources/ldap/{slug}/used_by/";
        var response = await client.GetAsync(url_22142f89, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_22142f89 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_22142f89;
    }

    /// <summary>
    /// GET /sources/oauth/
    /// </summary>
    public async Task<PaginatedResult<object>> OauthListAsync(string? access_token_url = null, string? additional_scopes = null, string? authentication_flow = null, string? authorization_url = null, string? consumer_key = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, bool? has_jwks = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? profile_url = null, string? provider_type = null, string? request_token_url = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_225610dc = $"api/v3/sources/oauth/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(access_token_url)) queryParams.Add($"access_token_url={access_token_url}");
        if (!string.IsNullOrEmpty(additional_scopes)) queryParams.Add($"additional_scopes={additional_scopes}");
        if (!string.IsNullOrEmpty(authentication_flow)) queryParams.Add($"authentication_flow={authentication_flow}");
        if (!string.IsNullOrEmpty(authorization_url)) queryParams.Add($"authorization_url={authorization_url}");
        if (!string.IsNullOrEmpty(consumer_key)) queryParams.Add($"consumer_key={consumer_key}");
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(enrollment_flow)) queryParams.Add($"enrollment_flow={enrollment_flow}");
        if (!string.IsNullOrEmpty(group_matching_mode)) queryParams.Add($"group_matching_mode={group_matching_mode}");
        if (has_jwks.HasValue) queryParams.Add($"has_jwks={has_jwks.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(policy_engine_mode)) queryParams.Add($"policy_engine_mode={policy_engine_mode}");
        if (!string.IsNullOrEmpty(profile_url)) queryParams.Add($"profile_url={profile_url}");
        if (!string.IsNullOrEmpty(provider_type)) queryParams.Add($"provider_type={provider_type}");
        if (!string.IsNullOrEmpty(request_token_url)) queryParams.Add($"request_token_url={request_token_url}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (!string.IsNullOrEmpty(user_matching_mode)) queryParams.Add($"user_matching_mode={user_matching_mode}");
        if (queryParams.Any()) url_225610dc += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_225610dc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_225610dc = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_225610dc ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/oauth/
    /// </summary>
    public async Task<object?> OauthCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_412a3032 = $"api/v3/sources/oauth/";
        var response = await client.PostAsJsonAsync(url_412a3032, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_412a3032 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_412a3032;
    }

    /// <summary>
    /// GET /sources/oauth/{slug}/
    /// </summary>
    public async Task<object?> OauthRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_336d9f2d = $"api/v3/sources/oauth/{slug}/";
        var response = await client.GetAsync(url_336d9f2d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_336d9f2d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_336d9f2d;
    }

    /// <summary>
    /// PUT /sources/oauth/{slug}/
    /// </summary>
    public async Task<object?> OauthUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ea1ec98d = $"api/v3/sources/oauth/{slug}/";
        var response = await client.PutAsJsonAsync(url_ea1ec98d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ea1ec98d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ea1ec98d;
    }

    /// <summary>
    /// PATCH /sources/oauth/{slug}/
    /// </summary>
    public async Task<object?> OauthPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2bd8f05b = $"api/v3/sources/oauth/{slug}/";
        var response = await client.PatchAsJsonAsync(url_2bd8f05b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2bd8f05b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2bd8f05b;
    }

    /// <summary>
    /// DELETE /sources/oauth/{slug}/
    /// </summary>
    public async Task<object?> OauthDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8c05363c = $"api/v3/sources/oauth/{slug}/";
        var response = await client.DeleteAsync(url_8c05363c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8c05363c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8c05363c;
    }

    /// <summary>
    /// GET /sources/oauth/{slug}/used_by/
    /// </summary>
    public async Task<object?> OauthUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6152a157 = $"api/v3/sources/oauth/{slug}/used_by/";
        var response = await client.GetAsync(url_6152a157, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6152a157 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6152a157;
    }

    /// <summary>
    /// GET /sources/oauth/source_types/
    /// </summary>
    public async Task<PaginatedResult<object>> OauthSourceTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_17b85e17 = $"api/v3/sources/oauth/source_types/";
        var response = await client.GetAsync(url_17b85e17, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_17b85e17 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_17b85e17 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/plex/
    /// </summary>
    public async Task<PaginatedResult<object>> PlexListAsync(bool? allow_friends = null, string? authentication_flow = null, string? client_id = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8d2cd542 = $"api/v3/sources/plex/";
        var queryParams = new List<string>();
        if (allow_friends.HasValue) queryParams.Add($"allow_friends={allow_friends.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(authentication_flow)) queryParams.Add($"authentication_flow={authentication_flow}");
        if (!string.IsNullOrEmpty(client_id)) queryParams.Add($"client_id={client_id}");
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(enrollment_flow)) queryParams.Add($"enrollment_flow={enrollment_flow}");
        if (!string.IsNullOrEmpty(group_matching_mode)) queryParams.Add($"group_matching_mode={group_matching_mode}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(policy_engine_mode)) queryParams.Add($"policy_engine_mode={policy_engine_mode}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (!string.IsNullOrEmpty(user_matching_mode)) queryParams.Add($"user_matching_mode={user_matching_mode}");
        if (queryParams.Any()) url_8d2cd542 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_8d2cd542, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8d2cd542 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8d2cd542 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/plex/
    /// </summary>
    public async Task<object?> PlexCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2ab55ccb = $"api/v3/sources/plex/";
        var response = await client.PostAsJsonAsync(url_2ab55ccb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2ab55ccb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2ab55ccb;
    }

    /// <summary>
    /// GET /sources/plex/{slug}/
    /// </summary>
    public async Task<object?> PlexRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cedf6ddf = $"api/v3/sources/plex/{slug}/";
        var response = await client.GetAsync(url_cedf6ddf, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cedf6ddf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cedf6ddf;
    }

    /// <summary>
    /// PUT /sources/plex/{slug}/
    /// </summary>
    public async Task<object?> PlexUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fc981953 = $"api/v3/sources/plex/{slug}/";
        var response = await client.PutAsJsonAsync(url_fc981953, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fc981953 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fc981953;
    }

    /// <summary>
    /// PATCH /sources/plex/{slug}/
    /// </summary>
    public async Task<object?> PlexPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9cc3b2c8 = $"api/v3/sources/plex/{slug}/";
        var response = await client.PatchAsJsonAsync(url_9cc3b2c8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9cc3b2c8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9cc3b2c8;
    }

    /// <summary>
    /// DELETE /sources/plex/{slug}/
    /// </summary>
    public async Task<object?> PlexDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fb21df60 = $"api/v3/sources/plex/{slug}/";
        var response = await client.DeleteAsync(url_fb21df60, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fb21df60 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fb21df60;
    }

    /// <summary>
    /// GET /sources/plex/{slug}/used_by/
    /// </summary>
    public async Task<object?> PlexUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_179917c7 = $"api/v3/sources/plex/{slug}/used_by/";
        var response = await client.GetAsync(url_179917c7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_179917c7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_179917c7;
    }

    /// <summary>
    /// POST /sources/plex/redeem_token/
    /// </summary>
    public async Task<object?> PlexRedeemTokenCreateAsync(object request, string? slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb29f131 = $"api/v3/sources/plex/redeem_token/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (queryParams.Any()) url_eb29f131 += "?" + string.Join("&", queryParams);
        var response = await client.PostAsJsonAsync(url_eb29f131, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb29f131 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb29f131;
    }

    /// <summary>
    /// POST /sources/plex/redeem_token_authenticated/
    /// </summary>
    public async Task<object?> PlexRedeemTokenAuthenticatedCreateAsync(object request, string? slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d330137f = $"api/v3/sources/plex/redeem_token_authenticated/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (queryParams.Any()) url_d330137f += "?" + string.Join("&", queryParams);
        var response = await client.PostAsJsonAsync(url_d330137f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d330137f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d330137f;
    }

    /// <summary>
    /// GET /sources/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> SamlListAsync(bool? allow_idp_initiated = null, string? authentication_flow = null, string? binding_type = null, string? digest_algorithm = null, bool? enabled = null, string? enrollment_flow = null, string? issuer = null, string? managed = null, string? name_id_policy = null, string? pbm_uuid = null, string? policy_engine_mode = null, string? pre_authentication_flow = null, string? signature_algorithm = null, bool? signed_assertion = null, bool? signed_response = null, string? signing_kp = null, string? slo_url = null, string? slug = null, string? sso_url = null, string? temporary_user_delete_after = null, string? user_matching_mode = null, string? verification_kp = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1a5a518e = $"api/v3/sources/saml/";
        var queryParams = new List<string>();
        if (allow_idp_initiated.HasValue) queryParams.Add($"allow_idp_initiated={allow_idp_initiated.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(authentication_flow)) queryParams.Add($"authentication_flow={authentication_flow}");
        if (!string.IsNullOrEmpty(binding_type)) queryParams.Add($"binding_type={binding_type}");
        if (!string.IsNullOrEmpty(digest_algorithm)) queryParams.Add($"digest_algorithm={digest_algorithm}");
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(enrollment_flow)) queryParams.Add($"enrollment_flow={enrollment_flow}");
        if (!string.IsNullOrEmpty(issuer)) queryParams.Add($"issuer={issuer}");
        if (!string.IsNullOrEmpty(managed)) queryParams.Add($"managed={managed}");
        if (!string.IsNullOrEmpty(name_id_policy)) queryParams.Add($"name_id_policy={name_id_policy}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(policy_engine_mode)) queryParams.Add($"policy_engine_mode={policy_engine_mode}");
        if (!string.IsNullOrEmpty(pre_authentication_flow)) queryParams.Add($"pre_authentication_flow={pre_authentication_flow}");
        if (!string.IsNullOrEmpty(signature_algorithm)) queryParams.Add($"signature_algorithm={signature_algorithm}");
        if (signed_assertion.HasValue) queryParams.Add($"signed_assertion={signed_assertion.Value.ToString().ToLower()}");
        if (signed_response.HasValue) queryParams.Add($"signed_response={signed_response.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(signing_kp)) queryParams.Add($"signing_kp={signing_kp}");
        if (!string.IsNullOrEmpty(slo_url)) queryParams.Add($"slo_url={slo_url}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (!string.IsNullOrEmpty(sso_url)) queryParams.Add($"sso_url={sso_url}");
        if (!string.IsNullOrEmpty(temporary_user_delete_after)) queryParams.Add($"temporary_user_delete_after={temporary_user_delete_after}");
        if (!string.IsNullOrEmpty(user_matching_mode)) queryParams.Add($"user_matching_mode={user_matching_mode}");
        if (!string.IsNullOrEmpty(verification_kp)) queryParams.Add($"verification_kp={verification_kp}");
        if (queryParams.Any()) url_1a5a518e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_1a5a518e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1a5a518e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1a5a518e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/saml/
    /// </summary>
    public async Task<object?> SamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eeecf738 = $"api/v3/sources/saml/";
        var response = await client.PostAsJsonAsync(url_eeecf738, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eeecf738 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eeecf738;
    }

    /// <summary>
    /// GET /sources/saml/{slug}/
    /// </summary>
    public async Task<object?> SamlRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7c092329 = $"api/v3/sources/saml/{slug}/";
        var response = await client.GetAsync(url_7c092329, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7c092329 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7c092329;
    }

    /// <summary>
    /// PUT /sources/saml/{slug}/
    /// </summary>
    public async Task<object?> SamlUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5212d40b = $"api/v3/sources/saml/{slug}/";
        var response = await client.PutAsJsonAsync(url_5212d40b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5212d40b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5212d40b;
    }

    /// <summary>
    /// PATCH /sources/saml/{slug}/
    /// </summary>
    public async Task<object?> SamlPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e454155d = $"api/v3/sources/saml/{slug}/";
        var response = await client.PatchAsJsonAsync(url_e454155d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e454155d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e454155d;
    }

    /// <summary>
    /// DELETE /sources/saml/{slug}/
    /// </summary>
    public async Task<object?> SamlDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5f8ee8c7 = $"api/v3/sources/saml/{slug}/";
        var response = await client.DeleteAsync(url_5f8ee8c7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5f8ee8c7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5f8ee8c7;
    }

    /// <summary>
    /// GET /sources/saml/{slug}/metadata/
    /// </summary>
    public async Task<object?> SamlMetadataRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98033d82 = $"api/v3/sources/saml/{slug}/metadata/";
        var response = await client.GetAsync(url_98033d82, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98033d82 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98033d82;
    }

    /// <summary>
    /// GET /sources/saml/{slug}/used_by/
    /// </summary>
    public async Task<object?> SamlUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e329ec3c = $"api/v3/sources/saml/{slug}/used_by/";
        var response = await client.GetAsync(url_e329ec3c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e329ec3c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e329ec3c;
    }

    /// <summary>
    /// GET /sources/scim/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimListAsync(string? pbm_uuid = null, string? slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0c6a5c17 = $"api/v3/sources/scim/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (queryParams.Any()) url_0c6a5c17 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0c6a5c17, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0c6a5c17 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0c6a5c17 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/scim/
    /// </summary>
    public async Task<object?> ScimCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_83755a82 = $"api/v3/sources/scim/";
        var response = await client.PostAsJsonAsync(url_83755a82, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_83755a82 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_83755a82;
    }

    /// <summary>
    /// GET /sources/scim/{slug}/
    /// </summary>
    public async Task<object?> ScimRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_02c22e96 = $"api/v3/sources/scim/{slug}/";
        var response = await client.GetAsync(url_02c22e96, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_02c22e96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_02c22e96;
    }

    /// <summary>
    /// PUT /sources/scim/{slug}/
    /// </summary>
    public async Task<object?> ScimUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2b8f7669 = $"api/v3/sources/scim/{slug}/";
        var response = await client.PutAsJsonAsync(url_2b8f7669, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2b8f7669 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2b8f7669;
    }

    /// <summary>
    /// PATCH /sources/scim/{slug}/
    /// </summary>
    public async Task<object?> ScimPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fd581e19 = $"api/v3/sources/scim/{slug}/";
        var response = await client.PatchAsJsonAsync(url_fd581e19, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fd581e19 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fd581e19;
    }

    /// <summary>
    /// DELETE /sources/scim/{slug}/
    /// </summary>
    public async Task<object?> ScimDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9443ff68 = $"api/v3/sources/scim/{slug}/";
        var response = await client.DeleteAsync(url_9443ff68, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9443ff68 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9443ff68;
    }

    /// <summary>
    /// GET /sources/scim/{slug}/used_by/
    /// </summary>
    public async Task<object?> ScimUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_44b766a5 = $"api/v3/sources/scim/{slug}/used_by/";
        var response = await client.GetAsync(url_44b766a5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_44b766a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_44b766a5;
    }

    /// <summary>
    /// GET /sources/scim_groups/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimGroupsListAsync(string? group__group_uuid = null, string? group__name = null, string? source__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_21849509 = $"api/v3/sources/scim_groups/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group__group_uuid)) queryParams.Add($"group__group_uuid={group__group_uuid}");
        if (!string.IsNullOrEmpty(group__name)) queryParams.Add($"group__name={group__name}");
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (queryParams.Any()) url_21849509 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_21849509, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_21849509 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_21849509 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/scim_groups/
    /// </summary>
    public async Task<object?> ScimGroupsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_89edf239 = $"api/v3/sources/scim_groups/";
        var response = await client.PostAsJsonAsync(url_89edf239, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_89edf239 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_89edf239;
    }

    /// <summary>
    /// GET /sources/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a2d22e00 = $"api/v3/sources/scim_groups/{id}/";
        var response = await client.GetAsync(url_a2d22e00, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a2d22e00 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a2d22e00;
    }

    /// <summary>
    /// PUT /sources/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6e5e27f7 = $"api/v3/sources/scim_groups/{id}/";
        var response = await client.PutAsJsonAsync(url_6e5e27f7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6e5e27f7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6e5e27f7;
    }

    /// <summary>
    /// PATCH /sources/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_76f171d7 = $"api/v3/sources/scim_groups/{id}/";
        var response = await client.PatchAsJsonAsync(url_76f171d7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_76f171d7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_76f171d7;
    }

    /// <summary>
    /// DELETE /sources/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb23bf4f = $"api/v3/sources/scim_groups/{id}/";
        var response = await client.DeleteAsync(url_eb23bf4f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb23bf4f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb23bf4f;
    }

    /// <summary>
    /// GET /sources/scim_groups/{id}/used_by/
    /// </summary>
    public async Task<object?> ScimGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fdbb844c = $"api/v3/sources/scim_groups/{id}/used_by/";
        var response = await client.GetAsync(url_fdbb844c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fdbb844c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fdbb844c;
    }

    /// <summary>
    /// GET /sources/scim_users/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimUsersListAsync(string? source__slug = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9220ce23 = $"api/v3/sources/scim_users/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user__id.HasValue) queryParams.Add($"user__id={user__id}");
        if (!string.IsNullOrEmpty(user__username)) queryParams.Add($"user__username={user__username}");
        if (queryParams.Any()) url_9220ce23 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_9220ce23, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9220ce23 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9220ce23 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/scim_users/
    /// </summary>
    public async Task<object?> ScimUsersCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5a3ec9c9 = $"api/v3/sources/scim_users/";
        var response = await client.PostAsJsonAsync(url_5a3ec9c9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5a3ec9c9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5a3ec9c9;
    }

    /// <summary>
    /// GET /sources/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9b2a8ab9 = $"api/v3/sources/scim_users/{id}/";
        var response = await client.GetAsync(url_9b2a8ab9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9b2a8ab9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9b2a8ab9;
    }

    /// <summary>
    /// PUT /sources/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f10f385d = $"api/v3/sources/scim_users/{id}/";
        var response = await client.PutAsJsonAsync(url_f10f385d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f10f385d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f10f385d;
    }

    /// <summary>
    /// PATCH /sources/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8538bed0 = $"api/v3/sources/scim_users/{id}/";
        var response = await client.PatchAsJsonAsync(url_8538bed0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8538bed0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8538bed0;
    }

    /// <summary>
    /// DELETE /sources/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_790bd05c = $"api/v3/sources/scim_users/{id}/";
        var response = await client.DeleteAsync(url_790bd05c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_790bd05c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_790bd05c;
    }

    /// <summary>
    /// GET /sources/scim_users/{id}/used_by/
    /// </summary>
    public async Task<object?> ScimUsersUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2d30884e = $"api/v3/sources/scim_users/{id}/used_by/";
        var response = await client.GetAsync(url_2d30884e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2d30884e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2d30884e;
    }

    /// <summary>
    /// GET /sources/telegram/
    /// </summary>
    public async Task<PaginatedResult<object>> TelegramListAsync(string? authentication_flow = null, string? bot_username = null, bool? enabled = null, string? enrollment_flow = null, string? group_matching_mode = null, string? pbm_uuid = null, string? policy_engine_mode = null, bool? request_message_access = null, string? slug = null, string? user_matching_mode = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d1493c22 = $"api/v3/sources/telegram/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(authentication_flow)) queryParams.Add($"authentication_flow={authentication_flow}");
        if (!string.IsNullOrEmpty(bot_username)) queryParams.Add($"bot_username={bot_username}");
        if (enabled.HasValue) queryParams.Add($"enabled={enabled.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(enrollment_flow)) queryParams.Add($"enrollment_flow={enrollment_flow}");
        if (!string.IsNullOrEmpty(group_matching_mode)) queryParams.Add($"group_matching_mode={group_matching_mode}");
        if (!string.IsNullOrEmpty(pbm_uuid)) queryParams.Add($"pbm_uuid={pbm_uuid}");
        if (!string.IsNullOrEmpty(policy_engine_mode)) queryParams.Add($"policy_engine_mode={policy_engine_mode}");
        if (request_message_access.HasValue) queryParams.Add($"request_message_access={request_message_access.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(slug)) queryParams.Add($"slug={slug}");
        if (!string.IsNullOrEmpty(user_matching_mode)) queryParams.Add($"user_matching_mode={user_matching_mode}");
        if (queryParams.Any()) url_d1493c22 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d1493c22, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d1493c22 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d1493c22 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/telegram/
    /// </summary>
    public async Task<object?> TelegramCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b3a5d79 = $"api/v3/sources/telegram/";
        var response = await client.PostAsJsonAsync(url_5b3a5d79, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b3a5d79 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b3a5d79;
    }

    /// <summary>
    /// GET /sources/telegram/{slug}/
    /// </summary>
    public async Task<object?> TelegramRetrieveAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75e8b015 = $"api/v3/sources/telegram/{slug}/";
        var response = await client.GetAsync(url_75e8b015, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75e8b015 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75e8b015;
    }

    /// <summary>
    /// PUT /sources/telegram/{slug}/
    /// </summary>
    public async Task<object?> TelegramUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_60c741ae = $"api/v3/sources/telegram/{slug}/";
        var response = await client.PutAsJsonAsync(url_60c741ae, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_60c741ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_60c741ae;
    }

    /// <summary>
    /// PATCH /sources/telegram/{slug}/
    /// </summary>
    public async Task<object?> TelegramPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8459d1fe = $"api/v3/sources/telegram/{slug}/";
        var response = await client.PatchAsJsonAsync(url_8459d1fe, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8459d1fe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8459d1fe;
    }

    /// <summary>
    /// DELETE /sources/telegram/{slug}/
    /// </summary>
    public async Task<object?> TelegramDestroyAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d6ebec50 = $"api/v3/sources/telegram/{slug}/";
        var response = await client.DeleteAsync(url_d6ebec50, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d6ebec50 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d6ebec50;
    }

    /// <summary>
    /// GET /sources/telegram/{slug}/used_by/
    /// </summary>
    public async Task<object?> TelegramUsedByListAsync(string slug, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_02f67d4f = $"api/v3/sources/telegram/{slug}/used_by/";
        var response = await client.GetAsync(url_02f67d4f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_02f67d4f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_02f67d4f;
    }

    /// <summary>
    /// GET /sources/user_connections/all/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsAllListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c7dc5424 = $"api/v3/sources/user_connections/all/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_c7dc5424 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_c7dc5424, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c7dc5424 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c7dc5424 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /sources/user_connections/all/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsAllRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2eaf2362 = $"api/v3/sources/user_connections/all/{id}/";
        var response = await client.GetAsync(url_2eaf2362, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2eaf2362 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2eaf2362;
    }

    /// <summary>
    /// PUT /sources/user_connections/all/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsAllUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_879968b1 = $"api/v3/sources/user_connections/all/{id}/";
        var response = await client.PutAsJsonAsync(url_879968b1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_879968b1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_879968b1;
    }

    /// <summary>
    /// PATCH /sources/user_connections/all/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsAllPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b20a1478 = $"api/v3/sources/user_connections/all/{id}/";
        var response = await client.PatchAsJsonAsync(url_b20a1478, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b20a1478 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b20a1478;
    }

    /// <summary>
    /// DELETE /sources/user_connections/all/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsAllDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3f6fbf81 = $"api/v3/sources/user_connections/all/{id}/";
        var response = await client.DeleteAsync(url_3f6fbf81, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3f6fbf81 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3f6fbf81;
    }

    /// <summary>
    /// GET /sources/user_connections/all/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsAllUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e64c5a82 = $"api/v3/sources/user_connections/all/{id}/used_by/";
        var response = await client.GetAsync(url_e64c5a82, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e64c5a82 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e64c5a82;
    }

    /// <summary>
    /// GET /sources/user_connections/kerberos/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsKerberosListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9b66883b = $"api/v3/sources/user_connections/kerberos/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_9b66883b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_9b66883b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9b66883b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9b66883b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/kerberos/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1f7ee6e2 = $"api/v3/sources/user_connections/kerberos/";
        var response = await client.PostAsJsonAsync(url_1f7ee6e2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1f7ee6e2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1f7ee6e2;
    }

    /// <summary>
    /// GET /sources/user_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c78d126d = $"api/v3/sources/user_connections/kerberos/{id}/";
        var response = await client.GetAsync(url_c78d126d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c78d126d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c78d126d;
    }

    /// <summary>
    /// PUT /sources/user_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a47c8e3c = $"api/v3/sources/user_connections/kerberos/{id}/";
        var response = await client.PutAsJsonAsync(url_a47c8e3c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a47c8e3c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a47c8e3c;
    }

    /// <summary>
    /// PATCH /sources/user_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f2f713ec = $"api/v3/sources/user_connections/kerberos/{id}/";
        var response = await client.PatchAsJsonAsync(url_f2f713ec, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f2f713ec = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f2f713ec;
    }

    /// <summary>
    /// DELETE /sources/user_connections/kerberos/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_87427d2e = $"api/v3/sources/user_connections/kerberos/{id}/";
        var response = await client.DeleteAsync(url_87427d2e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_87427d2e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_87427d2e;
    }

    /// <summary>
    /// GET /sources/user_connections/kerberos/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsKerberosUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_66933efe = $"api/v3/sources/user_connections/kerberos/{id}/used_by/";
        var response = await client.GetAsync(url_66933efe, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_66933efe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_66933efe;
    }

    /// <summary>
    /// GET /sources/user_connections/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsLdapListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_da1c9ad5 = $"api/v3/sources/user_connections/ldap/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_da1c9ad5 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_da1c9ad5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_da1c9ad5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_da1c9ad5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/ldap/
    /// </summary>
    public async Task<object?> UserConnectionsLdapCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1d18c774 = $"api/v3/sources/user_connections/ldap/";
        var response = await client.PostAsJsonAsync(url_1d18c774, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1d18c774 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1d18c774;
    }

    /// <summary>
    /// GET /sources/user_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsLdapRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fbfbffce = $"api/v3/sources/user_connections/ldap/{id}/";
        var response = await client.GetAsync(url_fbfbffce, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fbfbffce = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fbfbffce;
    }

    /// <summary>
    /// PUT /sources/user_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsLdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_33fc371e = $"api/v3/sources/user_connections/ldap/{id}/";
        var response = await client.PutAsJsonAsync(url_33fc371e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_33fc371e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_33fc371e;
    }

    /// <summary>
    /// PATCH /sources/user_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsLdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2754b1eb = $"api/v3/sources/user_connections/ldap/{id}/";
        var response = await client.PatchAsJsonAsync(url_2754b1eb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2754b1eb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2754b1eb;
    }

    /// <summary>
    /// DELETE /sources/user_connections/ldap/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsLdapDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3bd30469 = $"api/v3/sources/user_connections/ldap/{id}/";
        var response = await client.DeleteAsync(url_3bd30469, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3bd30469 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3bd30469;
    }

    /// <summary>
    /// GET /sources/user_connections/ldap/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsLdapUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_23a9040b = $"api/v3/sources/user_connections/ldap/{id}/used_by/";
        var response = await client.GetAsync(url_23a9040b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_23a9040b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_23a9040b;
    }

    /// <summary>
    /// GET /sources/user_connections/oauth/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsOauthListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f4e5f5be = $"api/v3/sources/user_connections/oauth/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_f4e5f5be += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f4e5f5be, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f4e5f5be = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f4e5f5be ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/oauth/
    /// </summary>
    public async Task<object?> UserConnectionsOauthCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1db45e26 = $"api/v3/sources/user_connections/oauth/";
        var response = await client.PostAsJsonAsync(url_1db45e26, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1db45e26 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1db45e26;
    }

    /// <summary>
    /// GET /sources/user_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsOauthRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f6785d2b = $"api/v3/sources/user_connections/oauth/{id}/";
        var response = await client.GetAsync(url_f6785d2b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f6785d2b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f6785d2b;
    }

    /// <summary>
    /// PUT /sources/user_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsOauthUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_168d4a0d = $"api/v3/sources/user_connections/oauth/{id}/";
        var response = await client.PutAsJsonAsync(url_168d4a0d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_168d4a0d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_168d4a0d;
    }

    /// <summary>
    /// PATCH /sources/user_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsOauthPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0e8b9791 = $"api/v3/sources/user_connections/oauth/{id}/";
        var response = await client.PatchAsJsonAsync(url_0e8b9791, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0e8b9791 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0e8b9791;
    }

    /// <summary>
    /// DELETE /sources/user_connections/oauth/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsOauthDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_39017257 = $"api/v3/sources/user_connections/oauth/{id}/";
        var response = await client.DeleteAsync(url_39017257, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_39017257 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_39017257;
    }

    /// <summary>
    /// GET /sources/user_connections/oauth/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsOauthUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2129ef57 = $"api/v3/sources/user_connections/oauth/{id}/used_by/";
        var response = await client.GetAsync(url_2129ef57, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2129ef57 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2129ef57;
    }

    /// <summary>
    /// GET /sources/user_connections/plex/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsPlexListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_37aa18bb = $"api/v3/sources/user_connections/plex/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_37aa18bb += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_37aa18bb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_37aa18bb = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_37aa18bb ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/plex/
    /// </summary>
    public async Task<object?> UserConnectionsPlexCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e83c8613 = $"api/v3/sources/user_connections/plex/";
        var response = await client.PostAsJsonAsync(url_e83c8613, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e83c8613 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e83c8613;
    }

    /// <summary>
    /// GET /sources/user_connections/plex/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsPlexRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_533fb3f3 = $"api/v3/sources/user_connections/plex/{id}/";
        var response = await client.GetAsync(url_533fb3f3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_533fb3f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_533fb3f3;
    }

    /// <summary>
    /// PUT /sources/user_connections/plex/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsPlexUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7e5f1dc1 = $"api/v3/sources/user_connections/plex/{id}/";
        var response = await client.PutAsJsonAsync(url_7e5f1dc1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7e5f1dc1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7e5f1dc1;
    }

    /// <summary>
    /// PATCH /sources/user_connections/plex/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsPlexPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_15050e18 = $"api/v3/sources/user_connections/plex/{id}/";
        var response = await client.PatchAsJsonAsync(url_15050e18, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_15050e18 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_15050e18;
    }

    /// <summary>
    /// DELETE /sources/user_connections/plex/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsPlexDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb9fc375 = $"api/v3/sources/user_connections/plex/{id}/";
        var response = await client.DeleteAsync(url_eb9fc375, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb9fc375 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb9fc375;
    }

    /// <summary>
    /// GET /sources/user_connections/plex/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsPlexUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8dd73796 = $"api/v3/sources/user_connections/plex/{id}/used_by/";
        var response = await client.GetAsync(url_8dd73796, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8dd73796 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8dd73796;
    }

    /// <summary>
    /// GET /sources/user_connections/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsSamlListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_aa8f9bdc = $"api/v3/sources/user_connections/saml/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_aa8f9bdc += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_aa8f9bdc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_aa8f9bdc = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_aa8f9bdc ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/saml/
    /// </summary>
    public async Task<object?> UserConnectionsSamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_85bd7388 = $"api/v3/sources/user_connections/saml/";
        var response = await client.PostAsJsonAsync(url_85bd7388, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_85bd7388 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_85bd7388;
    }

    /// <summary>
    /// GET /sources/user_connections/saml/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsSamlRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f49c03e3 = $"api/v3/sources/user_connections/saml/{id}/";
        var response = await client.GetAsync(url_f49c03e3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f49c03e3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f49c03e3;
    }

    /// <summary>
    /// PUT /sources/user_connections/saml/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsSamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1b816e95 = $"api/v3/sources/user_connections/saml/{id}/";
        var response = await client.PutAsJsonAsync(url_1b816e95, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1b816e95 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1b816e95;
    }

    /// <summary>
    /// PATCH /sources/user_connections/saml/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsSamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1d467eb2 = $"api/v3/sources/user_connections/saml/{id}/";
        var response = await client.PatchAsJsonAsync(url_1d467eb2, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1d467eb2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1d467eb2;
    }

    /// <summary>
    /// DELETE /sources/user_connections/saml/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsSamlDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fe388153 = $"api/v3/sources/user_connections/saml/{id}/";
        var response = await client.DeleteAsync(url_fe388153, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fe388153 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fe388153;
    }

    /// <summary>
    /// GET /sources/user_connections/saml/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsSamlUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b13622b = $"api/v3/sources/user_connections/saml/{id}/used_by/";
        var response = await client.GetAsync(url_5b13622b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b13622b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b13622b;
    }

    /// <summary>
    /// GET /sources/user_connections/telegram/
    /// </summary>
    public async Task<PaginatedResult<object>> UserConnectionsTelegramListAsync(string? source__slug = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4f251db = $"api/v3/sources/user_connections/telegram/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(source__slug)) queryParams.Add($"source__slug={source__slug}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_c4f251db += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_c4f251db, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4f251db = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4f251db ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /sources/user_connections/telegram/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0f4d6ade = $"api/v3/sources/user_connections/telegram/";
        var response = await client.PostAsJsonAsync(url_0f4d6ade, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0f4d6ade = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0f4d6ade;
    }

    /// <summary>
    /// GET /sources/user_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f2b6d5a2 = $"api/v3/sources/user_connections/telegram/{id}/";
        var response = await client.GetAsync(url_f2b6d5a2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f2b6d5a2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f2b6d5a2;
    }

    /// <summary>
    /// PUT /sources/user_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a836934b = $"api/v3/sources/user_connections/telegram/{id}/";
        var response = await client.PutAsJsonAsync(url_a836934b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a836934b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a836934b;
    }

    /// <summary>
    /// PATCH /sources/user_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d7290419 = $"api/v3/sources/user_connections/telegram/{id}/";
        var response = await client.PatchAsJsonAsync(url_d7290419, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d7290419 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d7290419;
    }

    /// <summary>
    /// DELETE /sources/user_connections/telegram/{id}/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6895869d = $"api/v3/sources/user_connections/telegram/{id}/";
        var response = await client.DeleteAsync(url_6895869d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6895869d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6895869d;
    }

    /// <summary>
    /// GET /sources/user_connections/telegram/{id}/used_by/
    /// </summary>
    public async Task<object?> UserConnectionsTelegramUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a39dc779 = $"api/v3/sources/user_connections/telegram/{id}/used_by/";
        var response = await client.GetAsync(url_a39dc779, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a39dc779 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a39dc779;
    }

}
