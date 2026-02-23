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
/// Service implementation for Authentik Providers API operations.
/// </summary>
public class AuthentikProvidersService : IAuthentikProvidersService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikProvidersService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikProvidersService"/> class.
    /// </summary>
    public AuthentikProvidersService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikProvidersService> logger,
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
    /// GET /providers/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(bool? application__isnull = null, bool? backchannel = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/providers/all/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (backchannel.HasValue) queryParams.Add($"backchannel={backchannel.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_f372512f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /providers/all/{id}/
    /// </summary>
    public async Task<object?> AllRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95226f05 = $"api/v3/providers/all/{id}/";
        var response = await client.GetAsync(url_95226f05, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95226f05 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95226f05;
    }

    /// <summary>
    /// DELETE /providers/all/{id}/
    /// </summary>
    public async Task<object?> AllDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b02df8d9 = $"api/v3/providers/all/{id}/";
        var response = await client.DeleteAsync(url_b02df8d9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b02df8d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b02df8d9;
    }

    /// <summary>
    /// GET /providers/all/{id}/used_by/
    /// </summary>
    public async Task<object?> AllUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a42dde8b = $"api/v3/providers/all/{id}/used_by/";
        var response = await client.GetAsync(url_a42dde8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a42dde8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a42dde8b;
    }

    /// <summary>
    /// GET /providers/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf388636 = $"api/v3/providers/all/types/";
        var response = await client.GetAsync(url_bf388636, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf388636 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf388636 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /providers/google_workspace/
    /// </summary>
    public async Task<PaginatedResult<object>> GoogleWorkspaceListAsync(string? delegated_subject = null, bool? exclude_users_service_account = null, string? filter_group = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bc234555 = $"api/v3/providers/google_workspace/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(delegated_subject)) queryParams.Add($"delegated_subject={delegated_subject}");
        if (exclude_users_service_account.HasValue) queryParams.Add($"exclude_users_service_account={exclude_users_service_account.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(filter_group)) queryParams.Add($"filter_group={filter_group}");
        if (queryParams.Any()) url_bc234555 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_bc234555, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bc234555 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bc234555 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/google_workspace/
    /// </summary>
    public async Task<object?> GoogleWorkspaceCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4e41c6a3 = $"api/v3/providers/google_workspace/";
        var response = await client.PostAsJsonAsync(url_4e41c6a3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4e41c6a3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4e41c6a3;
    }

    /// <summary>
    /// GET /providers/google_workspace/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b5c445a9 = $"api/v3/providers/google_workspace/{id}/";
        var response = await client.GetAsync(url_b5c445a9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b5c445a9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b5c445a9;
    }

    /// <summary>
    /// PUT /providers/google_workspace/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5d90e18c = $"api/v3/providers/google_workspace/{id}/";
        var response = await client.PutAsJsonAsync(url_5d90e18c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5d90e18c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5d90e18c;
    }

    /// <summary>
    /// PATCH /providers/google_workspace/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspacePartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e06ea23f = $"api/v3/providers/google_workspace/{id}/";
        var response = await client.PatchAsJsonAsync(url_e06ea23f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e06ea23f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e06ea23f;
    }

    /// <summary>
    /// DELETE /providers/google_workspace/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3ad80446 = $"api/v3/providers/google_workspace/{id}/";
        var response = await client.DeleteAsync(url_3ad80446, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3ad80446 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3ad80446;
    }

    /// <summary>
    /// POST /providers/google_workspace/{id}/sync/object/
    /// </summary>
    public async Task<object?> GoogleWorkspaceSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_108c11f6 = $"api/v3/providers/google_workspace/{id}/sync/object/";
        var response = await client.PostAsJsonAsync(url_108c11f6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_108c11f6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_108c11f6;
    }

    /// <summary>
    /// GET /providers/google_workspace/{id}/sync/status/
    /// </summary>
    public async Task<object?> GoogleWorkspaceSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7bdef906 = $"api/v3/providers/google_workspace/{id}/sync/status/";
        var response = await client.GetAsync(url_7bdef906, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7bdef906 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7bdef906;
    }

    /// <summary>
    /// GET /providers/google_workspace/{id}/used_by/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e54ac348 = $"api/v3/providers/google_workspace/{id}/used_by/";
        var response = await client.GetAsync(url_e54ac348, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e54ac348 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e54ac348;
    }

    /// <summary>
    /// GET /providers/google_workspace_groups/
    /// </summary>
    public async Task<PaginatedResult<object>> GoogleWorkspaceGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d4895436 = $"api/v3/providers/google_workspace_groups/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group__group_uuid)) queryParams.Add($"group__group_uuid={group__group_uuid}");
        if (!string.IsNullOrEmpty(group__name)) queryParams.Add($"group__name={group__name}");
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
        if (queryParams.Any()) url_d4895436 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d4895436, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d4895436 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d4895436 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/google_workspace_groups/
    /// </summary>
    public async Task<object?> GoogleWorkspaceGroupsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a4deb472 = $"api/v3/providers/google_workspace_groups/";
        var response = await client.PostAsJsonAsync(url_a4deb472, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a4deb472 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a4deb472;
    }

    /// <summary>
    /// GET /providers/google_workspace_groups/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_470438ab = $"api/v3/providers/google_workspace_groups/{id}/";
        var response = await client.GetAsync(url_470438ab, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_470438ab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_470438ab;
    }

    /// <summary>
    /// DELETE /providers/google_workspace_groups/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceGroupsDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_87cdc9ae = $"api/v3/providers/google_workspace_groups/{id}/";
        var response = await client.DeleteAsync(url_87cdc9ae, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_87cdc9ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_87cdc9ae;
    }

    /// <summary>
    /// GET /providers/google_workspace_groups/{id}/used_by/
    /// </summary>
    public async Task<object?> GoogleWorkspaceGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eede2ad3 = $"api/v3/providers/google_workspace_groups/{id}/used_by/";
        var response = await client.GetAsync(url_eede2ad3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eede2ad3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eede2ad3;
    }

    /// <summary>
    /// GET /providers/google_workspace_users/
    /// </summary>
    public async Task<PaginatedResult<object>> GoogleWorkspaceUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9881615b = $"api/v3/providers/google_workspace_users/";
        var queryParams = new List<string>();
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
        if (user__id.HasValue) queryParams.Add($"user__id={user__id}");
        if (!string.IsNullOrEmpty(user__username)) queryParams.Add($"user__username={user__username}");
        if (queryParams.Any()) url_9881615b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_9881615b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9881615b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9881615b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/google_workspace_users/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUsersCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2dad1c42 = $"api/v3/providers/google_workspace_users/";
        var response = await client.PostAsJsonAsync(url_2dad1c42, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2dad1c42 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2dad1c42;
    }

    /// <summary>
    /// GET /providers/google_workspace_users/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUsersRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ea95d046 = $"api/v3/providers/google_workspace_users/{id}/";
        var response = await client.GetAsync(url_ea95d046, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ea95d046 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ea95d046;
    }

    /// <summary>
    /// DELETE /providers/google_workspace_users/{id}/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUsersDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b9a8606 = $"api/v3/providers/google_workspace_users/{id}/";
        var response = await client.DeleteAsync(url_5b9a8606, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b9a8606 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b9a8606;
    }

    /// <summary>
    /// GET /providers/google_workspace_users/{id}/used_by/
    /// </summary>
    public async Task<object?> GoogleWorkspaceUsersUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_46a3edb4 = $"api/v3/providers/google_workspace_users/{id}/used_by/";
        var response = await client.GetAsync(url_46a3edb4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_46a3edb4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_46a3edb4;
    }

    /// <summary>
    /// GET /providers/ldap/
    /// </summary>
    public async Task<PaginatedResult<object>> LdapListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, string? base_dn__iexact = null, string? certificate__kp_uuid__iexact = null, string? certificate__name__iexact = null, int? gid_start_number__iexact = null, string? name__iexact = null, string? tls_server_name__iexact = null, int? uid_start_number__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b4c9b420 = $"api/v3/providers/ldap/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(authorization_flow__slug__iexact)) queryParams.Add($"authorization_flow__slug__iexact={authorization_flow__slug__iexact}");
        if (!string.IsNullOrEmpty(base_dn__iexact)) queryParams.Add($"base_dn__iexact={base_dn__iexact}");
        if (!string.IsNullOrEmpty(certificate__kp_uuid__iexact)) queryParams.Add($"certificate__kp_uuid__iexact={certificate__kp_uuid__iexact}");
        if (!string.IsNullOrEmpty(certificate__name__iexact)) queryParams.Add($"certificate__name__iexact={certificate__name__iexact}");
        if (gid_start_number__iexact.HasValue) queryParams.Add($"gid_start_number__iexact={gid_start_number__iexact}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (!string.IsNullOrEmpty(tls_server_name__iexact)) queryParams.Add($"tls_server_name__iexact={tls_server_name__iexact}");
        if (uid_start_number__iexact.HasValue) queryParams.Add($"uid_start_number__iexact={uid_start_number__iexact}");
        if (queryParams.Any()) url_b4c9b420 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b4c9b420, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b4c9b420 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b4c9b420 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/ldap/
    /// </summary>
    public async Task<object?> LdapCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_74eb0f9d = $"api/v3/providers/ldap/";
        var response = await client.PostAsJsonAsync(url_74eb0f9d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_74eb0f9d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_74eb0f9d;
    }

    /// <summary>
    /// GET /providers/ldap/{id}/
    /// </summary>
    public async Task<object?> LdapRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fbe24456 = $"api/v3/providers/ldap/{id}/";
        var response = await client.GetAsync(url_fbe24456, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fbe24456 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fbe24456;
    }

    /// <summary>
    /// PUT /providers/ldap/{id}/
    /// </summary>
    public async Task<object?> LdapUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8ed55908 = $"api/v3/providers/ldap/{id}/";
        var response = await client.PutAsJsonAsync(url_8ed55908, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8ed55908 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8ed55908;
    }

    /// <summary>
    /// PATCH /providers/ldap/{id}/
    /// </summary>
    public async Task<object?> LdapPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5baec692 = $"api/v3/providers/ldap/{id}/";
        var response = await client.PatchAsJsonAsync(url_5baec692, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5baec692 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5baec692;
    }

    /// <summary>
    /// DELETE /providers/ldap/{id}/
    /// </summary>
    public async Task<object?> LdapDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4a892714 = $"api/v3/providers/ldap/{id}/";
        var response = await client.DeleteAsync(url_4a892714, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4a892714 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4a892714;
    }

    /// <summary>
    /// GET /providers/ldap/{id}/used_by/
    /// </summary>
    public async Task<object?> LdapUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_22142f89 = $"api/v3/providers/ldap/{id}/used_by/";
        var response = await client.GetAsync(url_22142f89, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_22142f89 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_22142f89;
    }

    /// <summary>
    /// GET /providers/microsoft_entra/
    /// </summary>
    public async Task<PaginatedResult<object>> MicrosoftEntraListAsync(bool? exclude_users_service_account = null, string? filter_group = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3295ca2b = $"api/v3/providers/microsoft_entra/";
        var queryParams = new List<string>();
        if (exclude_users_service_account.HasValue) queryParams.Add($"exclude_users_service_account={exclude_users_service_account.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(filter_group)) queryParams.Add($"filter_group={filter_group}");
        if (queryParams.Any()) url_3295ca2b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_3295ca2b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3295ca2b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3295ca2b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/microsoft_entra/
    /// </summary>
    public async Task<object?> MicrosoftEntraCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_35205ecc = $"api/v3/providers/microsoft_entra/";
        var response = await client.PostAsJsonAsync(url_35205ecc, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_35205ecc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_35205ecc;
    }

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_10e5db6a = $"api/v3/providers/microsoft_entra/{id}/";
        var response = await client.GetAsync(url_10e5db6a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_10e5db6a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_10e5db6a;
    }

    /// <summary>
    /// PUT /providers/microsoft_entra/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_728f368e = $"api/v3/providers/microsoft_entra/{id}/";
        var response = await client.PutAsJsonAsync(url_728f368e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_728f368e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_728f368e;
    }

    /// <summary>
    /// PATCH /providers/microsoft_entra/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ea3a8f56 = $"api/v3/providers/microsoft_entra/{id}/";
        var response = await client.PatchAsJsonAsync(url_ea3a8f56, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ea3a8f56 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ea3a8f56;
    }

    /// <summary>
    /// DELETE /providers/microsoft_entra/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b2633eb7 = $"api/v3/providers/microsoft_entra/{id}/";
        var response = await client.DeleteAsync(url_b2633eb7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b2633eb7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b2633eb7;
    }

    /// <summary>
    /// POST /providers/microsoft_entra/{id}/sync/object/
    /// </summary>
    public async Task<object?> MicrosoftEntraSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b13b182c = $"api/v3/providers/microsoft_entra/{id}/sync/object/";
        var response = await client.PostAsJsonAsync(url_b13b182c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b13b182c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b13b182c;
    }

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/sync/status/
    /// </summary>
    public async Task<object?> MicrosoftEntraSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_33702b50 = $"api/v3/providers/microsoft_entra/{id}/sync/status/";
        var response = await client.GetAsync(url_33702b50, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_33702b50 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_33702b50;
    }

    /// <summary>
    /// GET /providers/microsoft_entra/{id}/used_by/
    /// </summary>
    public async Task<object?> MicrosoftEntraUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0d31438c = $"api/v3/providers/microsoft_entra/{id}/used_by/";
        var response = await client.GetAsync(url_0d31438c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0d31438c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0d31438c;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_groups/
    /// </summary>
    public async Task<PaginatedResult<object>> MicrosoftEntraGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df60e994 = $"api/v3/providers/microsoft_entra_groups/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group__group_uuid)) queryParams.Add($"group__group_uuid={group__group_uuid}");
        if (!string.IsNullOrEmpty(group__name)) queryParams.Add($"group__name={group__name}");
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
        if (queryParams.Any()) url_df60e994 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_df60e994, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df60e994 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df60e994 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/microsoft_entra_groups/
    /// </summary>
    public async Task<object?> MicrosoftEntraGroupsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0129e30b = $"api/v3/providers/microsoft_entra_groups/";
        var response = await client.PostAsJsonAsync(url_0129e30b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0129e30b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0129e30b;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_groups/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a9130b5a = $"api/v3/providers/microsoft_entra_groups/{id}/";
        var response = await client.GetAsync(url_a9130b5a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a9130b5a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a9130b5a;
    }

    /// <summary>
    /// DELETE /providers/microsoft_entra_groups/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraGroupsDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f9c8f24d = $"api/v3/providers/microsoft_entra_groups/{id}/";
        var response = await client.DeleteAsync(url_f9c8f24d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f9c8f24d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f9c8f24d;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_groups/{id}/used_by/
    /// </summary>
    public async Task<object?> MicrosoftEntraGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_07f8a89f = $"api/v3/providers/microsoft_entra_groups/{id}/used_by/";
        var response = await client.GetAsync(url_07f8a89f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_07f8a89f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_07f8a89f;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_users/
    /// </summary>
    public async Task<PaginatedResult<object>> MicrosoftEntraUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_11a87ae9 = $"api/v3/providers/microsoft_entra_users/";
        var queryParams = new List<string>();
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
        if (user__id.HasValue) queryParams.Add($"user__id={user__id}");
        if (!string.IsNullOrEmpty(user__username)) queryParams.Add($"user__username={user__username}");
        if (queryParams.Any()) url_11a87ae9 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_11a87ae9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_11a87ae9 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_11a87ae9 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/microsoft_entra_users/
    /// </summary>
    public async Task<object?> MicrosoftEntraUsersCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e9ebd314 = $"api/v3/providers/microsoft_entra_users/";
        var response = await client.PostAsJsonAsync(url_e9ebd314, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e9ebd314 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e9ebd314;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_users/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraUsersRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f606c862 = $"api/v3/providers/microsoft_entra_users/{id}/";
        var response = await client.GetAsync(url_f606c862, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f606c862 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f606c862;
    }

    /// <summary>
    /// DELETE /providers/microsoft_entra_users/{id}/
    /// </summary>
    public async Task<object?> MicrosoftEntraUsersDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dae4c319 = $"api/v3/providers/microsoft_entra_users/{id}/";
        var response = await client.DeleteAsync(url_dae4c319, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dae4c319 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dae4c319;
    }

    /// <summary>
    /// GET /providers/microsoft_entra_users/{id}/used_by/
    /// </summary>
    public async Task<object?> MicrosoftEntraUsersUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ca7d598f = $"api/v3/providers/microsoft_entra_users/{id}/used_by/";
        var response = await client.GetAsync(url_ca7d598f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ca7d598f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ca7d598f;
    }

    /// <summary>
    /// GET /providers/oauth2/
    /// </summary>
    public async Task<PaginatedResult<object>> Oauth2ListAsync(string? access_code_validity = null, string? access_token_validity = null, string? application = null, string? authorization_flow = null, string? client_id = null, string? client_type = null, bool? include_claims_in_id_token = null, string? issuer_mode = null, string? property_mappings = null, string? refresh_token_validity = null, string? signing_key = null, string? sub_mode = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df711f34 = $"api/v3/providers/oauth2/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(access_code_validity)) queryParams.Add($"access_code_validity={access_code_validity}");
        if (!string.IsNullOrEmpty(access_token_validity)) queryParams.Add($"access_token_validity={access_token_validity}");
        if (!string.IsNullOrEmpty(application)) queryParams.Add($"application={application}");
        if (!string.IsNullOrEmpty(authorization_flow)) queryParams.Add($"authorization_flow={authorization_flow}");
        if (!string.IsNullOrEmpty(client_id)) queryParams.Add($"client_id={client_id}");
        if (!string.IsNullOrEmpty(client_type)) queryParams.Add($"client_type={client_type}");
        if (include_claims_in_id_token.HasValue) queryParams.Add($"include_claims_in_id_token={include_claims_in_id_token.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(issuer_mode)) queryParams.Add($"issuer_mode={issuer_mode}");
        if (!string.IsNullOrEmpty(property_mappings)) queryParams.Add($"property_mappings={property_mappings}");
        if (!string.IsNullOrEmpty(refresh_token_validity)) queryParams.Add($"refresh_token_validity={refresh_token_validity}");
        if (!string.IsNullOrEmpty(signing_key)) queryParams.Add($"signing_key={signing_key}");
        if (!string.IsNullOrEmpty(sub_mode)) queryParams.Add($"sub_mode={sub_mode}");
        if (queryParams.Any()) url_df711f34 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_df711f34, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df711f34 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df711f34 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/oauth2/
    /// </summary>
    public async Task<object?> Oauth2CreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_929b4a8a = $"api/v3/providers/oauth2/";
        var response = await client.PostAsJsonAsync(url_929b4a8a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_929b4a8a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_929b4a8a;
    }

    /// <summary>
    /// GET /providers/oauth2/{id}/
    /// </summary>
    public async Task<object?> Oauth2RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0fb678cf = $"api/v3/providers/oauth2/{id}/";
        var response = await client.GetAsync(url_0fb678cf, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0fb678cf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0fb678cf;
    }

    /// <summary>
    /// PUT /providers/oauth2/{id}/
    /// </summary>
    public async Task<object?> Oauth2UpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a2b18a5a = $"api/v3/providers/oauth2/{id}/";
        var response = await client.PutAsJsonAsync(url_a2b18a5a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a2b18a5a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a2b18a5a;
    }

    /// <summary>
    /// PATCH /providers/oauth2/{id}/
    /// </summary>
    public async Task<object?> Oauth2PartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b6e40c44 = $"api/v3/providers/oauth2/{id}/";
        var response = await client.PatchAsJsonAsync(url_b6e40c44, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b6e40c44 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b6e40c44;
    }

    /// <summary>
    /// DELETE /providers/oauth2/{id}/
    /// </summary>
    public async Task<object?> Oauth2DestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_68a349e8 = $"api/v3/providers/oauth2/{id}/";
        var response = await client.DeleteAsync(url_68a349e8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_68a349e8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_68a349e8;
    }

    /// <summary>
    /// GET /providers/oauth2/{id}/preview_user/
    /// </summary>
    public async Task<object?> Oauth2PreviewUserRetrieveAsync(int id, int? for_user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b3ba5bed = $"api/v3/providers/oauth2/{id}/preview_user/";
        var queryParams = new List<string>();
        if (for_user.HasValue) queryParams.Add($"for_user={for_user}");
        if (queryParams.Any()) url_b3ba5bed += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b3ba5bed, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b3ba5bed = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b3ba5bed;
    }

    /// <summary>
    /// GET /providers/oauth2/{id}/setup_urls/
    /// </summary>
    public async Task<object?> Oauth2SetupUrlsRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d280aa42 = $"api/v3/providers/oauth2/{id}/setup_urls/";
        var response = await client.GetAsync(url_d280aa42, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d280aa42 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d280aa42;
    }

    /// <summary>
    /// GET /providers/oauth2/{id}/used_by/
    /// </summary>
    public async Task<object?> Oauth2UsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c030a55d = $"api/v3/providers/oauth2/{id}/used_by/";
        var response = await client.GetAsync(url_c030a55d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c030a55d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c030a55d;
    }

    /// <summary>
    /// GET /providers/proxy/
    /// </summary>
    public async Task<PaginatedResult<object>> ProxyListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, bool? basic_auth_enabled__iexact = null, string? basic_auth_password_attribute__iexact = null, string? basic_auth_user_attribute__iexact = null, string? certificate__kp_uuid__iexact = null, string? certificate__name__iexact = null, string? cookie_domain__iexact = null, string? external_host__iexact = null, string? internal_host__iexact = null, bool? internal_host_ssl_validation__iexact = null, string? mode__iexact = null, string? name__iexact = null, string? property_mappings__iexact = null, string? skip_path_regex__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_93ef65ce = $"api/v3/providers/proxy/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(authorization_flow__slug__iexact)) queryParams.Add($"authorization_flow__slug__iexact={authorization_flow__slug__iexact}");
        if (basic_auth_enabled__iexact.HasValue) queryParams.Add($"basic_auth_enabled__iexact={basic_auth_enabled__iexact.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(basic_auth_password_attribute__iexact)) queryParams.Add($"basic_auth_password_attribute__iexact={basic_auth_password_attribute__iexact}");
        if (!string.IsNullOrEmpty(basic_auth_user_attribute__iexact)) queryParams.Add($"basic_auth_user_attribute__iexact={basic_auth_user_attribute__iexact}");
        if (!string.IsNullOrEmpty(certificate__kp_uuid__iexact)) queryParams.Add($"certificate__kp_uuid__iexact={certificate__kp_uuid__iexact}");
        if (!string.IsNullOrEmpty(certificate__name__iexact)) queryParams.Add($"certificate__name__iexact={certificate__name__iexact}");
        if (!string.IsNullOrEmpty(cookie_domain__iexact)) queryParams.Add($"cookie_domain__iexact={cookie_domain__iexact}");
        if (!string.IsNullOrEmpty(external_host__iexact)) queryParams.Add($"external_host__iexact={external_host__iexact}");
        if (!string.IsNullOrEmpty(internal_host__iexact)) queryParams.Add($"internal_host__iexact={internal_host__iexact}");
        if (internal_host_ssl_validation__iexact.HasValue) queryParams.Add($"internal_host_ssl_validation__iexact={internal_host_ssl_validation__iexact.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(mode__iexact)) queryParams.Add($"mode__iexact={mode__iexact}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (!string.IsNullOrEmpty(property_mappings__iexact)) queryParams.Add($"property_mappings__iexact={property_mappings__iexact}");
        if (!string.IsNullOrEmpty(skip_path_regex__iexact)) queryParams.Add($"skip_path_regex__iexact={skip_path_regex__iexact}");
        if (queryParams.Any()) url_93ef65ce += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_93ef65ce, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_93ef65ce = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_93ef65ce ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/proxy/
    /// </summary>
    public async Task<object?> ProxyCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dfba5370 = $"api/v3/providers/proxy/";
        var response = await client.PostAsJsonAsync(url_dfba5370, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dfba5370 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dfba5370;
    }

    /// <summary>
    /// GET /providers/proxy/{id}/
    /// </summary>
    public async Task<object?> ProxyRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b91e14ec = $"api/v3/providers/proxy/{id}/";
        var response = await client.GetAsync(url_b91e14ec, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b91e14ec = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b91e14ec;
    }

    /// <summary>
    /// PUT /providers/proxy/{id}/
    /// </summary>
    public async Task<object?> ProxyUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e2b187dc = $"api/v3/providers/proxy/{id}/";
        var response = await client.PutAsJsonAsync(url_e2b187dc, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e2b187dc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e2b187dc;
    }

    /// <summary>
    /// PATCH /providers/proxy/{id}/
    /// </summary>
    public async Task<object?> ProxyPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4115260 = $"api/v3/providers/proxy/{id}/";
        var response = await client.PatchAsJsonAsync(url_c4115260, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4115260 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4115260;
    }

    /// <summary>
    /// DELETE /providers/proxy/{id}/
    /// </summary>
    public async Task<object?> ProxyDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f9489d14 = $"api/v3/providers/proxy/{id}/";
        var response = await client.DeleteAsync(url_f9489d14, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f9489d14 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f9489d14;
    }

    /// <summary>
    /// GET /providers/proxy/{id}/used_by/
    /// </summary>
    public async Task<object?> ProxyUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cf577783 = $"api/v3/providers/proxy/{id}/used_by/";
        var response = await client.GetAsync(url_cf577783, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cf577783 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cf577783;
    }

    /// <summary>
    /// GET /providers/rac/
    /// </summary>
    public async Task<PaginatedResult<object>> RacListAsync(bool? application__isnull = null, string? name__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f98ba640 = $"api/v3/providers/rac/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (queryParams.Any()) url_f98ba640 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f98ba640, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f98ba640 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f98ba640 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/rac/
    /// </summary>
    public async Task<object?> RacCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_594fdb17 = $"api/v3/providers/rac/";
        var response = await client.PostAsJsonAsync(url_594fdb17, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_594fdb17 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_594fdb17;
    }

    /// <summary>
    /// GET /providers/rac/{id}/
    /// </summary>
    public async Task<object?> RacRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b92ab0e6 = $"api/v3/providers/rac/{id}/";
        var response = await client.GetAsync(url_b92ab0e6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b92ab0e6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b92ab0e6;
    }

    /// <summary>
    /// PUT /providers/rac/{id}/
    /// </summary>
    public async Task<object?> RacUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_692af601 = $"api/v3/providers/rac/{id}/";
        var response = await client.PutAsJsonAsync(url_692af601, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_692af601 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_692af601;
    }

    /// <summary>
    /// PATCH /providers/rac/{id}/
    /// </summary>
    public async Task<object?> RacPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a5fb8c6e = $"api/v3/providers/rac/{id}/";
        var response = await client.PatchAsJsonAsync(url_a5fb8c6e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a5fb8c6e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a5fb8c6e;
    }

    /// <summary>
    /// DELETE /providers/rac/{id}/
    /// </summary>
    public async Task<object?> RacDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_48174fc2 = $"api/v3/providers/rac/{id}/";
        var response = await client.DeleteAsync(url_48174fc2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_48174fc2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_48174fc2;
    }

    /// <summary>
    /// GET /providers/rac/{id}/used_by/
    /// </summary>
    public async Task<object?> RacUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_41c1f520 = $"api/v3/providers/rac/{id}/used_by/";
        var response = await client.GetAsync(url_41c1f520, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_41c1f520 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_41c1f520;
    }

    /// <summary>
    /// GET /providers/radius/
    /// </summary>
    public async Task<PaginatedResult<object>> RadiusListAsync(bool? application__isnull = null, string? authorization_flow__slug__iexact = null, string? client_networks__iexact = null, string? name__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8611aa59 = $"api/v3/providers/radius/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(authorization_flow__slug__iexact)) queryParams.Add($"authorization_flow__slug__iexact={authorization_flow__slug__iexact}");
        if (!string.IsNullOrEmpty(client_networks__iexact)) queryParams.Add($"client_networks__iexact={client_networks__iexact}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (queryParams.Any()) url_8611aa59 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_8611aa59, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8611aa59 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8611aa59 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/radius/
    /// </summary>
    public async Task<object?> RadiusCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_11acac4a = $"api/v3/providers/radius/";
        var response = await client.PostAsJsonAsync(url_11acac4a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_11acac4a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_11acac4a;
    }

    /// <summary>
    /// GET /providers/radius/{id}/
    /// </summary>
    public async Task<object?> RadiusRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7a56e4ee = $"api/v3/providers/radius/{id}/";
        var response = await client.GetAsync(url_7a56e4ee, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7a56e4ee = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7a56e4ee;
    }

    /// <summary>
    /// PUT /providers/radius/{id}/
    /// </summary>
    public async Task<object?> RadiusUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5b185d1b = $"api/v3/providers/radius/{id}/";
        var response = await client.PutAsJsonAsync(url_5b185d1b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5b185d1b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5b185d1b;
    }

    /// <summary>
    /// PATCH /providers/radius/{id}/
    /// </summary>
    public async Task<object?> RadiusPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9ba83018 = $"api/v3/providers/radius/{id}/";
        var response = await client.PatchAsJsonAsync(url_9ba83018, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9ba83018 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9ba83018;
    }

    /// <summary>
    /// DELETE /providers/radius/{id}/
    /// </summary>
    public async Task<object?> RadiusDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5585b46c = $"api/v3/providers/radius/{id}/";
        var response = await client.DeleteAsync(url_5585b46c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5585b46c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5585b46c;
    }

    /// <summary>
    /// GET /providers/radius/{id}/used_by/
    /// </summary>
    public async Task<object?> RadiusUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a9d9215a = $"api/v3/providers/radius/{id}/used_by/";
        var response = await client.GetAsync(url_a9d9215a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a9d9215a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a9d9215a;
    }

    /// <summary>
    /// GET /providers/saml/
    /// </summary>
    public async Task<PaginatedResult<object>> SamlListAsync(string? acs_url = null, string? assertion_valid_not_before = null, string? assertion_valid_not_on_or_after = null, string? audience = null, string? authentication_flow = null, string? authn_context_class_ref_mapping = null, string? authorization_flow = null, string? backchannel_application = null, string? default_name_id_policy = null, string? default_relay_state = null, string? digest_algorithm = null, string? encryption_kp = null, string? invalidation_flow = null, bool? is_backchannel = null, string? issuer = null, string? logout_method = null, string? name_id_mapping = null, string? property_mappings = null, string? session_valid_not_on_or_after = null, bool? sign_assertion = null, bool? sign_logout_request = null, bool? sign_response = null, string? signature_algorithm = null, string? signing_kp = null, string? sls_binding = null, string? sls_url = null, string? sp_binding = null, string? verification_kp = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1a5a518e = $"api/v3/providers/saml/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(acs_url)) queryParams.Add($"acs_url={acs_url}");
        if (!string.IsNullOrEmpty(assertion_valid_not_before)) queryParams.Add($"assertion_valid_not_before={assertion_valid_not_before}");
        if (!string.IsNullOrEmpty(assertion_valid_not_on_or_after)) queryParams.Add($"assertion_valid_not_on_or_after={assertion_valid_not_on_or_after}");
        if (!string.IsNullOrEmpty(audience)) queryParams.Add($"audience={audience}");
        if (!string.IsNullOrEmpty(authentication_flow)) queryParams.Add($"authentication_flow={authentication_flow}");
        if (!string.IsNullOrEmpty(authn_context_class_ref_mapping)) queryParams.Add($"authn_context_class_ref_mapping={authn_context_class_ref_mapping}");
        if (!string.IsNullOrEmpty(authorization_flow)) queryParams.Add($"authorization_flow={authorization_flow}");
        if (!string.IsNullOrEmpty(backchannel_application)) queryParams.Add($"backchannel_application={backchannel_application}");
        if (!string.IsNullOrEmpty(default_name_id_policy)) queryParams.Add($"default_name_id_policy={default_name_id_policy}");
        if (!string.IsNullOrEmpty(default_relay_state)) queryParams.Add($"default_relay_state={default_relay_state}");
        if (!string.IsNullOrEmpty(digest_algorithm)) queryParams.Add($"digest_algorithm={digest_algorithm}");
        if (!string.IsNullOrEmpty(encryption_kp)) queryParams.Add($"encryption_kp={encryption_kp}");
        if (!string.IsNullOrEmpty(invalidation_flow)) queryParams.Add($"invalidation_flow={invalidation_flow}");
        if (is_backchannel.HasValue) queryParams.Add($"is_backchannel={is_backchannel.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(issuer)) queryParams.Add($"issuer={issuer}");
        if (!string.IsNullOrEmpty(logout_method)) queryParams.Add($"logout_method={logout_method}");
        if (!string.IsNullOrEmpty(name_id_mapping)) queryParams.Add($"name_id_mapping={name_id_mapping}");
        if (!string.IsNullOrEmpty(property_mappings)) queryParams.Add($"property_mappings={property_mappings}");
        if (!string.IsNullOrEmpty(session_valid_not_on_or_after)) queryParams.Add($"session_valid_not_on_or_after={session_valid_not_on_or_after}");
        if (sign_assertion.HasValue) queryParams.Add($"sign_assertion={sign_assertion.Value.ToString().ToLower()}");
        if (sign_logout_request.HasValue) queryParams.Add($"sign_logout_request={sign_logout_request.Value.ToString().ToLower()}");
        if (sign_response.HasValue) queryParams.Add($"sign_response={sign_response.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(signature_algorithm)) queryParams.Add($"signature_algorithm={signature_algorithm}");
        if (!string.IsNullOrEmpty(signing_kp)) queryParams.Add($"signing_kp={signing_kp}");
        if (!string.IsNullOrEmpty(sls_binding)) queryParams.Add($"sls_binding={sls_binding}");
        if (!string.IsNullOrEmpty(sls_url)) queryParams.Add($"sls_url={sls_url}");
        if (!string.IsNullOrEmpty(sp_binding)) queryParams.Add($"sp_binding={sp_binding}");
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
    /// POST /providers/saml/
    /// </summary>
    public async Task<object?> SamlCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eeecf738 = $"api/v3/providers/saml/";
        var response = await client.PostAsJsonAsync(url_eeecf738, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eeecf738 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eeecf738;
    }

    /// <summary>
    /// GET /providers/saml/{id}/
    /// </summary>
    public async Task<object?> SamlRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7c092329 = $"api/v3/providers/saml/{id}/";
        var response = await client.GetAsync(url_7c092329, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7c092329 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7c092329;
    }

    /// <summary>
    /// PUT /providers/saml/{id}/
    /// </summary>
    public async Task<object?> SamlUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5212d40b = $"api/v3/providers/saml/{id}/";
        var response = await client.PutAsJsonAsync(url_5212d40b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5212d40b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5212d40b;
    }

    /// <summary>
    /// PATCH /providers/saml/{id}/
    /// </summary>
    public async Task<object?> SamlPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e454155d = $"api/v3/providers/saml/{id}/";
        var response = await client.PatchAsJsonAsync(url_e454155d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e454155d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e454155d;
    }

    /// <summary>
    /// DELETE /providers/saml/{id}/
    /// </summary>
    public async Task<object?> SamlDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5f8ee8c7 = $"api/v3/providers/saml/{id}/";
        var response = await client.DeleteAsync(url_5f8ee8c7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5f8ee8c7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5f8ee8c7;
    }

    /// <summary>
    /// GET /providers/saml/{id}/metadata/
    /// </summary>
    public async Task<object?> SamlMetadataRetrieveAsync(int id, bool? download = null, string? force_binding = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98033d82 = $"api/v3/providers/saml/{id}/metadata/";
        var queryParams = new List<string>();
        if (download.HasValue) queryParams.Add($"download={download.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(force_binding)) queryParams.Add($"force_binding={force_binding}");
        if (queryParams.Any()) url_98033d82 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_98033d82, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98033d82 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98033d82;
    }

    /// <summary>
    /// GET /providers/saml/{id}/preview_user/
    /// </summary>
    public async Task<object?> SamlPreviewUserRetrieveAsync(int id, int? for_user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_61a560fa = $"api/v3/providers/saml/{id}/preview_user/";
        var queryParams = new List<string>();
        if (for_user.HasValue) queryParams.Add($"for_user={for_user}");
        if (queryParams.Any()) url_61a560fa += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_61a560fa, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_61a560fa = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_61a560fa;
    }

    /// <summary>
    /// GET /providers/saml/{id}/used_by/
    /// </summary>
    public async Task<object?> SamlUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e329ec3c = $"api/v3/providers/saml/{id}/used_by/";
        var response = await client.GetAsync(url_e329ec3c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e329ec3c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e329ec3c;
    }

    /// <summary>
    /// POST /providers/saml/import_metadata/
    /// </summary>
    public async Task<object?> SamlImportMetadataCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1c4b17a9 = $"api/v3/providers/saml/import_metadata/";
        var response = await client.PostAsync(url_1c4b17a9, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1c4b17a9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1c4b17a9;
    }

    /// <summary>
    /// GET /providers/scim/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimListAsync(bool? exclude_users_service_account = null, string? filter_group = null, string? url = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0c6a5c17 = $"api/v3/providers/scim/";
        var queryParams = new List<string>();
        if (exclude_users_service_account.HasValue) queryParams.Add($"exclude_users_service_account={exclude_users_service_account.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(filter_group)) queryParams.Add($"filter_group={filter_group}");
        if (!string.IsNullOrEmpty(url)) queryParams.Add($"url={url}");
        if (queryParams.Any()) url_0c6a5c17 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0c6a5c17, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0c6a5c17 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0c6a5c17 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/scim/
    /// </summary>
    public async Task<object?> ScimCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_83755a82 = $"api/v3/providers/scim/";
        var response = await client.PostAsJsonAsync(url_83755a82, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_83755a82 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_83755a82;
    }

    /// <summary>
    /// GET /providers/scim/{id}/
    /// </summary>
    public async Task<object?> ScimRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_02c22e96 = $"api/v3/providers/scim/{id}/";
        var response = await client.GetAsync(url_02c22e96, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_02c22e96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_02c22e96;
    }

    /// <summary>
    /// PUT /providers/scim/{id}/
    /// </summary>
    public async Task<object?> ScimUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2b8f7669 = $"api/v3/providers/scim/{id}/";
        var response = await client.PutAsJsonAsync(url_2b8f7669, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2b8f7669 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2b8f7669;
    }

    /// <summary>
    /// PATCH /providers/scim/{id}/
    /// </summary>
    public async Task<object?> ScimPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fd581e19 = $"api/v3/providers/scim/{id}/";
        var response = await client.PatchAsJsonAsync(url_fd581e19, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fd581e19 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fd581e19;
    }

    /// <summary>
    /// DELETE /providers/scim/{id}/
    /// </summary>
    public async Task<object?> ScimDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9443ff68 = $"api/v3/providers/scim/{id}/";
        var response = await client.DeleteAsync(url_9443ff68, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9443ff68 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9443ff68;
    }

    /// <summary>
    /// POST /providers/scim/{id}/sync/object/
    /// </summary>
    public async Task<object?> ScimSyncObjectCreateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_869b6a6b = $"api/v3/providers/scim/{id}/sync/object/";
        var response = await client.PostAsJsonAsync(url_869b6a6b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_869b6a6b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_869b6a6b;
    }

    /// <summary>
    /// GET /providers/scim/{id}/sync/status/
    /// </summary>
    public async Task<object?> ScimSyncStatusRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5e79ecd9 = $"api/v3/providers/scim/{id}/sync/status/";
        var response = await client.GetAsync(url_5e79ecd9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5e79ecd9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5e79ecd9;
    }

    /// <summary>
    /// GET /providers/scim/{id}/used_by/
    /// </summary>
    public async Task<object?> ScimUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_44b766a5 = $"api/v3/providers/scim/{id}/used_by/";
        var response = await client.GetAsync(url_44b766a5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_44b766a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_44b766a5;
    }

    /// <summary>
    /// GET /providers/scim_groups/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimGroupsListAsync(string? group__group_uuid = null, string? group__name = null, int? provider__id = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_21849509 = $"api/v3/providers/scim_groups/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(group__group_uuid)) queryParams.Add($"group__group_uuid={group__group_uuid}");
        if (!string.IsNullOrEmpty(group__name)) queryParams.Add($"group__name={group__name}");
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
        if (queryParams.Any()) url_21849509 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_21849509, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_21849509 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_21849509 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/scim_groups/
    /// </summary>
    public async Task<object?> ScimGroupsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_89edf239 = $"api/v3/providers/scim_groups/";
        var response = await client.PostAsJsonAsync(url_89edf239, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_89edf239 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_89edf239;
    }

    /// <summary>
    /// GET /providers/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a2d22e00 = $"api/v3/providers/scim_groups/{id}/";
        var response = await client.GetAsync(url_a2d22e00, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a2d22e00 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a2d22e00;
    }

    /// <summary>
    /// DELETE /providers/scim_groups/{id}/
    /// </summary>
    public async Task<object?> ScimGroupsDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb23bf4f = $"api/v3/providers/scim_groups/{id}/";
        var response = await client.DeleteAsync(url_eb23bf4f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb23bf4f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb23bf4f;
    }

    /// <summary>
    /// GET /providers/scim_groups/{id}/used_by/
    /// </summary>
    public async Task<object?> ScimGroupsUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_fdbb844c = $"api/v3/providers/scim_groups/{id}/used_by/";
        var response = await client.GetAsync(url_fdbb844c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_fdbb844c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_fdbb844c;
    }

    /// <summary>
    /// GET /providers/scim_users/
    /// </summary>
    public async Task<PaginatedResult<object>> ScimUsersListAsync(int? provider__id = null, int? user__id = null, string? user__username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9220ce23 = $"api/v3/providers/scim_users/";
        var queryParams = new List<string>();
        if (provider__id.HasValue) queryParams.Add($"provider__id={provider__id}");
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
    /// POST /providers/scim_users/
    /// </summary>
    public async Task<object?> ScimUsersCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5a3ec9c9 = $"api/v3/providers/scim_users/";
        var response = await client.PostAsJsonAsync(url_5a3ec9c9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5a3ec9c9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5a3ec9c9;
    }

    /// <summary>
    /// GET /providers/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9b2a8ab9 = $"api/v3/providers/scim_users/{id}/";
        var response = await client.GetAsync(url_9b2a8ab9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9b2a8ab9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9b2a8ab9;
    }

    /// <summary>
    /// DELETE /providers/scim_users/{id}/
    /// </summary>
    public async Task<object?> ScimUsersDestroyAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_790bd05c = $"api/v3/providers/scim_users/{id}/";
        var response = await client.DeleteAsync(url_790bd05c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_790bd05c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_790bd05c;
    }

    /// <summary>
    /// GET /providers/scim_users/{id}/used_by/
    /// </summary>
    public async Task<object?> ScimUsersUsedByListAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2d30884e = $"api/v3/providers/scim_users/{id}/used_by/";
        var response = await client.GetAsync(url_2d30884e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2d30884e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2d30884e;
    }

    /// <summary>
    /// GET /providers/ssf/
    /// </summary>
    public async Task<PaginatedResult<object>> SsfListAsync(bool? application__isnull = null, string? name__iexact = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_949587ac = $"api/v3/providers/ssf/";
        var queryParams = new List<string>();
        if (application__isnull.HasValue) queryParams.Add($"application__isnull={application__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(name__iexact)) queryParams.Add($"name__iexact={name__iexact}");
        if (queryParams.Any()) url_949587ac += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_949587ac, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_949587ac = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_949587ac ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /providers/ssf/
    /// </summary>
    public async Task<object?> SsfCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b685d0fe = $"api/v3/providers/ssf/";
        var response = await client.PostAsJsonAsync(url_b685d0fe, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b685d0fe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b685d0fe;
    }

    /// <summary>
    /// GET /providers/ssf/{id}/
    /// </summary>
    public async Task<object?> SsfRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d40be2b9 = $"api/v3/providers/ssf/{id}/";
        var response = await client.GetAsync(url_d40be2b9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d40be2b9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d40be2b9;
    }

    /// <summary>
    /// PUT /providers/ssf/{id}/
    /// </summary>
    public async Task<object?> SsfUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6f7c8741 = $"api/v3/providers/ssf/{id}/";
        var response = await client.PutAsJsonAsync(url_6f7c8741, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6f7c8741 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6f7c8741;
    }

    /// <summary>
    /// PATCH /providers/ssf/{id}/
    /// </summary>
    public async Task<object?> SsfPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f69524b1 = $"api/v3/providers/ssf/{id}/";
        var response = await client.PatchAsJsonAsync(url_f69524b1, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f69524b1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f69524b1;
    }

    /// <summary>
    /// DELETE /providers/ssf/{id}/
    /// </summary>
    public async Task<object?> SsfDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4f78b23c = $"api/v3/providers/ssf/{id}/";
        var response = await client.DeleteAsync(url_4f78b23c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4f78b23c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4f78b23c;
    }

    /// <summary>
    /// GET /providers/ssf/{id}/used_by/
    /// </summary>
    public async Task<object?> SsfUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a660ff65 = $"api/v3/providers/ssf/{id}/used_by/";
        var response = await client.GetAsync(url_a660ff65, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a660ff65 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a660ff65;
    }

}
