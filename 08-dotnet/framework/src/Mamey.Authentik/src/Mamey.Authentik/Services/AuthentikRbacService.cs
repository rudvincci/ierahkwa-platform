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
/// Service implementation for Authentik Rbac API operations.
/// </summary>
public class AuthentikRbacService : IAuthentikRbacService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikRbacService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikRbacService"/> class.
    /// </summary>
    public AuthentikRbacService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikRbacService> logger,
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
    /// GET /rbac/initial_permissions/
    /// </summary>
    public async Task<PaginatedResult<object>> InitialPermissionsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_13333ffa = $"api/v3/rbac/initial_permissions/";
        var response = await client.GetAsync(url_13333ffa, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_13333ffa = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_13333ffa ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /rbac/initial_permissions/
    /// </summary>
    public async Task<object?> InitialPermissionsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_08c3572f = $"api/v3/rbac/initial_permissions/";
        var response = await client.PostAsJsonAsync(url_08c3572f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_08c3572f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_08c3572f;
    }

    /// <summary>
    /// GET /rbac/initial_permissions/{id}/
    /// </summary>
    public async Task<object?> InitialPermissionsRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d78932d2 = $"api/v3/rbac/initial_permissions/{id}/";
        var response = await client.GetAsync(url_d78932d2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d78932d2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d78932d2;
    }

    /// <summary>
    /// PUT /rbac/initial_permissions/{id}/
    /// </summary>
    public async Task<object?> InitialPermissionsUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_28eed1b6 = $"api/v3/rbac/initial_permissions/{id}/";
        var response = await client.PutAsJsonAsync(url_28eed1b6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_28eed1b6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_28eed1b6;
    }

    /// <summary>
    /// PATCH /rbac/initial_permissions/{id}/
    /// </summary>
    public async Task<object?> InitialPermissionsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_77262eef = $"api/v3/rbac/initial_permissions/{id}/";
        var response = await client.PatchAsJsonAsync(url_77262eef, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_77262eef = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_77262eef;
    }

    /// <summary>
    /// DELETE /rbac/initial_permissions/{id}/
    /// </summary>
    public async Task<object?> InitialPermissionsDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_34ca3412 = $"api/v3/rbac/initial_permissions/{id}/";
        var response = await client.DeleteAsync(url_34ca3412, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_34ca3412 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_34ca3412;
    }

    /// <summary>
    /// GET /rbac/initial_permissions/{id}/used_by/
    /// </summary>
    public async Task<object?> InitialPermissionsUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_de1fbaad = $"api/v3/rbac/initial_permissions/{id}/used_by/";
        var response = await client.GetAsync(url_de1fbaad, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_de1fbaad = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_de1fbaad;
    }

    /// <summary>
    /// GET /rbac/permissions/
    /// </summary>
    public async Task<PaginatedResult<object>> PermissionsListAsync(string? codename = null, string? content_type__app_label = null, string? content_type__model = null, string? role = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f528c843 = $"api/v3/rbac/permissions/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(codename)) queryParams.Add($"codename={codename}");
        if (!string.IsNullOrEmpty(content_type__app_label)) queryParams.Add($"content_type__app_label={content_type__app_label}");
        if (!string.IsNullOrEmpty(content_type__model)) queryParams.Add($"content_type__model={content_type__model}");
        if (!string.IsNullOrEmpty(role)) queryParams.Add($"role={role}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_f528c843 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f528c843, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f528c843 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f528c843 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /rbac/permissions/{id}/
    /// </summary>
    public async Task<object?> PermissionsRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_af898f61 = $"api/v3/rbac/permissions/{id}/";
        var response = await client.GetAsync(url_af898f61, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_af898f61 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_af898f61;
    }

    /// <summary>
    /// GET /rbac/permissions/assigned_by_roles/
    /// </summary>
    public async Task<PaginatedResult<object>> PermissionsAssignedByRolesListAsync(string? model = null, string? object_pk = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3ba0a5e6 = $"api/v3/rbac/permissions/assigned_by_roles/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(model)) queryParams.Add($"model={model}");
        if (!string.IsNullOrEmpty(object_pk)) queryParams.Add($"object_pk={object_pk}");
        if (queryParams.Any()) url_3ba0a5e6 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_3ba0a5e6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3ba0a5e6 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3ba0a5e6 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /rbac/permissions/assigned_by_roles/{uuid}/assign/
    /// </summary>
    public async Task<object?> PermissionsAssignedByRolesAssignAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3a1c1e0c = $"api/v3/rbac/permissions/assigned_by_roles/{uuid}/assign/";
        var response = await client.PostAsJsonAsync(url_3a1c1e0c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3a1c1e0c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3a1c1e0c;
    }

    /// <summary>
    /// PATCH /rbac/permissions/assigned_by_roles/{uuid}/unassign/
    /// </summary>
    public async Task<object?> PermissionsAssignedByRolesUnassignPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_30865c73 = $"api/v3/rbac/permissions/assigned_by_roles/{uuid}/unassign/";
        var response = await client.PatchAsJsonAsync(url_30865c73, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_30865c73 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_30865c73;
    }

    /// <summary>
    /// GET /rbac/permissions/assigned_by_users/
    /// </summary>
    public async Task<PaginatedResult<object>> PermissionsAssignedByUsersListAsync(string? model = null, string? object_pk = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_67075d4e = $"api/v3/rbac/permissions/assigned_by_users/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(model)) queryParams.Add($"model={model}");
        if (!string.IsNullOrEmpty(object_pk)) queryParams.Add($"object_pk={object_pk}");
        if (queryParams.Any()) url_67075d4e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_67075d4e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_67075d4e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_67075d4e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /rbac/permissions/assigned_by_users/{id}/assign/
    /// </summary>
    public async Task<object?> PermissionsAssignedByUsersAssignAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_73055601 = $"api/v3/rbac/permissions/assigned_by_users/{id}/assign/";
        var response = await client.PostAsJsonAsync(url_73055601, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_73055601 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_73055601;
    }

    /// <summary>
    /// PATCH /rbac/permissions/assigned_by_users/{id}/unassign/
    /// </summary>
    public async Task<object?> PermissionsAssignedByUsersUnassignPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_199a73f6 = $"api/v3/rbac/permissions/assigned_by_users/{id}/unassign/";
        var response = await client.PatchAsJsonAsync(url_199a73f6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_199a73f6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_199a73f6;
    }

    /// <summary>
    /// GET /rbac/permissions/roles/
    /// </summary>
    public async Task<PaginatedResult<object>> PermissionsRolesListAsync(string? uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0bfc3bdd = $"api/v3/rbac/permissions/roles/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(uuid)) queryParams.Add($"uuid={uuid}");
        if (queryParams.Any()) url_0bfc3bdd += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0bfc3bdd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0bfc3bdd = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0bfc3bdd ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /rbac/permissions/roles/{id}/
    /// </summary>
    public async Task<object?> PermissionsRolesRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_abee95fb = $"api/v3/rbac/permissions/roles/{id}/";
        var response = await client.GetAsync(url_abee95fb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_abee95fb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_abee95fb;
    }

    /// <summary>
    /// PUT /rbac/permissions/roles/{id}/
    /// </summary>
    public async Task<object?> PermissionsRolesUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_26858752 = $"api/v3/rbac/permissions/roles/{id}/";
        var response = await client.PutAsJsonAsync(url_26858752, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_26858752 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_26858752;
    }

    /// <summary>
    /// PATCH /rbac/permissions/roles/{id}/
    /// </summary>
    public async Task<object?> PermissionsRolesPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_09707993 = $"api/v3/rbac/permissions/roles/{id}/";
        var response = await client.PatchAsJsonAsync(url_09707993, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_09707993 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_09707993;
    }

    /// <summary>
    /// DELETE /rbac/permissions/roles/{id}/
    /// </summary>
    public async Task<object?> PermissionsRolesDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_935b4130 = $"api/v3/rbac/permissions/roles/{id}/";
        var response = await client.DeleteAsync(url_935b4130, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_935b4130 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_935b4130;
    }

    /// <summary>
    /// GET /rbac/permissions/users/
    /// </summary>
    public async Task<PaginatedResult<object>> PermissionsUsersListAsync(int? user_id = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b93a18ea = $"api/v3/rbac/permissions/users/";
        var queryParams = new List<string>();
        if (user_id.HasValue) queryParams.Add($"user_id={user_id}");
        if (queryParams.Any()) url_b93a18ea += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b93a18ea, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b93a18ea = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b93a18ea ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /rbac/permissions/users/{id}/
    /// </summary>
    public async Task<object?> PermissionsUsersRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d47911ab = $"api/v3/rbac/permissions/users/{id}/";
        var response = await client.GetAsync(url_d47911ab, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d47911ab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d47911ab;
    }

    /// <summary>
    /// PUT /rbac/permissions/users/{id}/
    /// </summary>
    public async Task<object?> PermissionsUsersUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b2fa92f8 = $"api/v3/rbac/permissions/users/{id}/";
        var response = await client.PutAsJsonAsync(url_b2fa92f8, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b2fa92f8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b2fa92f8;
    }

    /// <summary>
    /// PATCH /rbac/permissions/users/{id}/
    /// </summary>
    public async Task<object?> PermissionsUsersPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_935b709f = $"api/v3/rbac/permissions/users/{id}/";
        var response = await client.PatchAsJsonAsync(url_935b709f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_935b709f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_935b709f;
    }

    /// <summary>
    /// DELETE /rbac/permissions/users/{id}/
    /// </summary>
    public async Task<object?> PermissionsUsersDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a566c1ba = $"api/v3/rbac/permissions/users/{id}/";
        var response = await client.DeleteAsync(url_a566c1ba, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a566c1ba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a566c1ba;
    }

    /// <summary>
    /// GET /rbac/roles/
    /// </summary>
    public async Task<PaginatedResult<object>> RolesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e92bf44c = $"api/v3/rbac/roles/";
        var response = await client.GetAsync(url_e92bf44c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e92bf44c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e92bf44c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /rbac/roles/
    /// </summary>
    public async Task<object?> RolesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_24acfc73 = $"api/v3/rbac/roles/";
        var response = await client.PostAsJsonAsync(url_24acfc73, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_24acfc73 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_24acfc73;
    }

    /// <summary>
    /// GET /rbac/roles/{uuid}/
    /// </summary>
    public async Task<object?> RolesRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5549b66c = $"api/v3/rbac/roles/{uuid}/";
        var response = await client.GetAsync(url_5549b66c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5549b66c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5549b66c;
    }

    /// <summary>
    /// PUT /rbac/roles/{uuid}/
    /// </summary>
    public async Task<object?> RolesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_91348493 = $"api/v3/rbac/roles/{uuid}/";
        var response = await client.PutAsJsonAsync(url_91348493, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_91348493 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_91348493;
    }

    /// <summary>
    /// PATCH /rbac/roles/{uuid}/
    /// </summary>
    public async Task<object?> RolesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df72713f = $"api/v3/rbac/roles/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_df72713f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df72713f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df72713f;
    }

    /// <summary>
    /// DELETE /rbac/roles/{uuid}/
    /// </summary>
    public async Task<object?> RolesDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9d546b8f = $"api/v3/rbac/roles/{uuid}/";
        var response = await client.DeleteAsync(url_9d546b8f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9d546b8f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9d546b8f;
    }

    /// <summary>
    /// GET /rbac/roles/{uuid}/used_by/
    /// </summary>
    public async Task<object?> RolesUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_68a30e4c = $"api/v3/rbac/roles/{uuid}/used_by/";
        var response = await client.GetAsync(url_68a30e4c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_68a30e4c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_68a30e4c;
    }

}
