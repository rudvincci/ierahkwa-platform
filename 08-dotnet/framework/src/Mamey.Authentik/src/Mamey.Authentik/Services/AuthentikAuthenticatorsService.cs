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
/// Service implementation for Authentik Authenticators API operations.
/// </summary>
public class AuthentikAuthenticatorsService : IAuthentikAuthenticatorsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikAuthenticatorsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthenticatorsService"/> class.
    /// </summary>
    public AuthentikAuthenticatorsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikAuthenticatorsService> logger,
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
    /// GET /authenticators/admin/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminAllListAsync(int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_43bf1ccc = $"api/v3/authenticators/admin/all/";
        var queryParams = new List<string>();
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_43bf1ccc += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_43bf1ccc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_43bf1ccc = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_43bf1ccc ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/admin/duo/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminDuoListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d232274d = $"api/v3/authenticators/admin/duo/";
        var response = await client.GetAsync(url_d232274d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d232274d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d232274d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/duo/
    /// </summary>
    public async Task<object?> AdminDuoCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f6f530f9 = $"api/v3/authenticators/admin/duo/";
        var response = await client.PostAsJsonAsync(url_f6f530f9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f6f530f9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f6f530f9;
    }

    /// <summary>
    /// GET /authenticators/admin/duo/{id}/
    /// </summary>
    public async Task<object?> AdminDuoRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f34c2db9 = $"api/v3/authenticators/admin/duo/{id}/";
        var response = await client.GetAsync(url_f34c2db9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f34c2db9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f34c2db9;
    }

    /// <summary>
    /// PUT /authenticators/admin/duo/{id}/
    /// </summary>
    public async Task<object?> AdminDuoUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f4361556 = $"api/v3/authenticators/admin/duo/{id}/";
        var response = await client.PutAsJsonAsync(url_f4361556, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f4361556 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f4361556;
    }

    /// <summary>
    /// PATCH /authenticators/admin/duo/{id}/
    /// </summary>
    public async Task<object?> AdminDuoPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9f68d2a9 = $"api/v3/authenticators/admin/duo/{id}/";
        var response = await client.PatchAsJsonAsync(url_9f68d2a9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9f68d2a9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9f68d2a9;
    }

    /// <summary>
    /// DELETE /authenticators/admin/duo/{id}/
    /// </summary>
    public async Task<object?> AdminDuoDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_54d2e97e = $"api/v3/authenticators/admin/duo/{id}/";
        var response = await client.DeleteAsync(url_54d2e97e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_54d2e97e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_54d2e97e;
    }

    /// <summary>
    /// GET /authenticators/admin/email/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminEmailListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_55f680cb = $"api/v3/authenticators/admin/email/";
        var response = await client.GetAsync(url_55f680cb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_55f680cb = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_55f680cb ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/email/
    /// </summary>
    public async Task<object?> AdminEmailCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1770c2f3 = $"api/v3/authenticators/admin/email/";
        var response = await client.PostAsJsonAsync(url_1770c2f3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1770c2f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1770c2f3;
    }

    /// <summary>
    /// GET /authenticators/admin/email/{id}/
    /// </summary>
    public async Task<object?> AdminEmailRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b4a73176 = $"api/v3/authenticators/admin/email/{id}/";
        var response = await client.GetAsync(url_b4a73176, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b4a73176 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b4a73176;
    }

    /// <summary>
    /// PUT /authenticators/admin/email/{id}/
    /// </summary>
    public async Task<object?> AdminEmailUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c9669413 = $"api/v3/authenticators/admin/email/{id}/";
        var response = await client.PutAsJsonAsync(url_c9669413, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c9669413 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c9669413;
    }

    /// <summary>
    /// PATCH /authenticators/admin/email/{id}/
    /// </summary>
    public async Task<object?> AdminEmailPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_86167348 = $"api/v3/authenticators/admin/email/{id}/";
        var response = await client.PatchAsJsonAsync(url_86167348, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_86167348 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_86167348;
    }

    /// <summary>
    /// DELETE /authenticators/admin/email/{id}/
    /// </summary>
    public async Task<object?> AdminEmailDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_37393ac3 = $"api/v3/authenticators/admin/email/{id}/";
        var response = await client.DeleteAsync(url_37393ac3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_37393ac3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_37393ac3;
    }

    /// <summary>
    /// GET /authenticators/admin/endpoint/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminEndpointListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_92537f9b = $"api/v3/authenticators/admin/endpoint/";
        var response = await client.GetAsync(url_92537f9b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_92537f9b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_92537f9b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/endpoint/
    /// </summary>
    public async Task<object?> AdminEndpointCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b71afd9d = $"api/v3/authenticators/admin/endpoint/";
        var response = await client.PostAsJsonAsync(url_b71afd9d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b71afd9d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b71afd9d;
    }

    /// <summary>
    /// GET /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    public async Task<object?> AdminEndpointRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_01fe9d70 = $"api/v3/authenticators/admin/endpoint/{uuid}/";
        var response = await client.GetAsync(url_01fe9d70, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_01fe9d70 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_01fe9d70;
    }

    /// <summary>
    /// PUT /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    public async Task<object?> AdminEndpointUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f715ea8d = $"api/v3/authenticators/admin/endpoint/{uuid}/";
        var response = await client.PutAsJsonAsync(url_f715ea8d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f715ea8d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f715ea8d;
    }

    /// <summary>
    /// PATCH /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    public async Task<object?> AdminEndpointPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1222e957 = $"api/v3/authenticators/admin/endpoint/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_1222e957, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1222e957 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1222e957;
    }

    /// <summary>
    /// DELETE /authenticators/admin/endpoint/{uuid}/
    /// </summary>
    public async Task<object?> AdminEndpointDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_457ac0e0 = $"api/v3/authenticators/admin/endpoint/{uuid}/";
        var response = await client.DeleteAsync(url_457ac0e0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_457ac0e0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_457ac0e0;
    }

    /// <summary>
    /// GET /authenticators/admin/sms/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminSmsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c1a321eb = $"api/v3/authenticators/admin/sms/";
        var response = await client.GetAsync(url_c1a321eb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c1a321eb = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c1a321eb ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/sms/
    /// </summary>
    public async Task<object?> AdminSmsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6e8e61ae = $"api/v3/authenticators/admin/sms/";
        var response = await client.PostAsJsonAsync(url_6e8e61ae, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6e8e61ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6e8e61ae;
    }

    /// <summary>
    /// GET /authenticators/admin/sms/{id}/
    /// </summary>
    public async Task<object?> AdminSmsRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_51f2b746 = $"api/v3/authenticators/admin/sms/{id}/";
        var response = await client.GetAsync(url_51f2b746, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_51f2b746 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_51f2b746;
    }

    /// <summary>
    /// PUT /authenticators/admin/sms/{id}/
    /// </summary>
    public async Task<object?> AdminSmsUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f44b44cf = $"api/v3/authenticators/admin/sms/{id}/";
        var response = await client.PutAsJsonAsync(url_f44b44cf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f44b44cf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f44b44cf;
    }

    /// <summary>
    /// PATCH /authenticators/admin/sms/{id}/
    /// </summary>
    public async Task<object?> AdminSmsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95de12e0 = $"api/v3/authenticators/admin/sms/{id}/";
        var response = await client.PatchAsJsonAsync(url_95de12e0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95de12e0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95de12e0;
    }

    /// <summary>
    /// DELETE /authenticators/admin/sms/{id}/
    /// </summary>
    public async Task<object?> AdminSmsDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75b60eed = $"api/v3/authenticators/admin/sms/{id}/";
        var response = await client.DeleteAsync(url_75b60eed, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75b60eed = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75b60eed;
    }

    /// <summary>
    /// GET /authenticators/admin/static/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminStaticListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0741be1d = $"api/v3/authenticators/admin/static/";
        var response = await client.GetAsync(url_0741be1d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0741be1d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0741be1d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/static/
    /// </summary>
    public async Task<object?> AdminStaticCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_21b25f8a = $"api/v3/authenticators/admin/static/";
        var response = await client.PostAsJsonAsync(url_21b25f8a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_21b25f8a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_21b25f8a;
    }

    /// <summary>
    /// GET /authenticators/admin/static/{id}/
    /// </summary>
    public async Task<object?> AdminStaticRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_056e7bfa = $"api/v3/authenticators/admin/static/{id}/";
        var response = await client.GetAsync(url_056e7bfa, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_056e7bfa = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_056e7bfa;
    }

    /// <summary>
    /// PUT /authenticators/admin/static/{id}/
    /// </summary>
    public async Task<object?> AdminStaticUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_be400a16 = $"api/v3/authenticators/admin/static/{id}/";
        var response = await client.PutAsJsonAsync(url_be400a16, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_be400a16 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_be400a16;
    }

    /// <summary>
    /// PATCH /authenticators/admin/static/{id}/
    /// </summary>
    public async Task<object?> AdminStaticPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_90d0619e = $"api/v3/authenticators/admin/static/{id}/";
        var response = await client.PatchAsJsonAsync(url_90d0619e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_90d0619e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_90d0619e;
    }

    /// <summary>
    /// DELETE /authenticators/admin/static/{id}/
    /// </summary>
    public async Task<object?> AdminStaticDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_804d33c7 = $"api/v3/authenticators/admin/static/{id}/";
        var response = await client.DeleteAsync(url_804d33c7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_804d33c7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_804d33c7;
    }

    /// <summary>
    /// GET /authenticators/admin/totp/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminTotpListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_38c237e0 = $"api/v3/authenticators/admin/totp/";
        var response = await client.GetAsync(url_38c237e0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_38c237e0 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_38c237e0 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/totp/
    /// </summary>
    public async Task<object?> AdminTotpCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e0de765b = $"api/v3/authenticators/admin/totp/";
        var response = await client.PostAsJsonAsync(url_e0de765b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e0de765b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e0de765b;
    }

    /// <summary>
    /// GET /authenticators/admin/totp/{id}/
    /// </summary>
    public async Task<object?> AdminTotpRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_aabd4759 = $"api/v3/authenticators/admin/totp/{id}/";
        var response = await client.GetAsync(url_aabd4759, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_aabd4759 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_aabd4759;
    }

    /// <summary>
    /// PUT /authenticators/admin/totp/{id}/
    /// </summary>
    public async Task<object?> AdminTotpUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ff16509d = $"api/v3/authenticators/admin/totp/{id}/";
        var response = await client.PutAsJsonAsync(url_ff16509d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ff16509d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ff16509d;
    }

    /// <summary>
    /// PATCH /authenticators/admin/totp/{id}/
    /// </summary>
    public async Task<object?> AdminTotpPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b3d3ff42 = $"api/v3/authenticators/admin/totp/{id}/";
        var response = await client.PatchAsJsonAsync(url_b3d3ff42, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b3d3ff42 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b3d3ff42;
    }

    /// <summary>
    /// DELETE /authenticators/admin/totp/{id}/
    /// </summary>
    public async Task<object?> AdminTotpDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_447c6db0 = $"api/v3/authenticators/admin/totp/{id}/";
        var response = await client.DeleteAsync(url_447c6db0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_447c6db0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_447c6db0;
    }

    /// <summary>
    /// GET /authenticators/admin/webauthn/
    /// </summary>
    public async Task<PaginatedResult<object>> AdminWebauthnListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_26f36027 = $"api/v3/authenticators/admin/webauthn/";
        var response = await client.GetAsync(url_26f36027, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_26f36027 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_26f36027 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /authenticators/admin/webauthn/
    /// </summary>
    public async Task<object?> AdminWebauthnCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2584ad24 = $"api/v3/authenticators/admin/webauthn/";
        var response = await client.PostAsJsonAsync(url_2584ad24, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2584ad24 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2584ad24;
    }

    /// <summary>
    /// GET /authenticators/admin/webauthn/{id}/
    /// </summary>
    public async Task<object?> AdminWebauthnRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b25dc808 = $"api/v3/authenticators/admin/webauthn/{id}/";
        var response = await client.GetAsync(url_b25dc808, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b25dc808 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b25dc808;
    }

    /// <summary>
    /// PUT /authenticators/admin/webauthn/{id}/
    /// </summary>
    public async Task<object?> AdminWebauthnUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6f7fd507 = $"api/v3/authenticators/admin/webauthn/{id}/";
        var response = await client.PutAsJsonAsync(url_6f7fd507, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6f7fd507 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6f7fd507;
    }

    /// <summary>
    /// PATCH /authenticators/admin/webauthn/{id}/
    /// </summary>
    public async Task<object?> AdminWebauthnPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b2daa6d6 = $"api/v3/authenticators/admin/webauthn/{id}/";
        var response = await client.PatchAsJsonAsync(url_b2daa6d6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b2daa6d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b2daa6d6;
    }

    /// <summary>
    /// DELETE /authenticators/admin/webauthn/{id}/
    /// </summary>
    public async Task<object?> AdminWebauthnDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f81d8181 = $"api/v3/authenticators/admin/webauthn/{id}/";
        var response = await client.DeleteAsync(url_f81d8181, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f81d8181 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f81d8181;
    }

    /// <summary>
    /// GET /authenticators/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/authenticators/all/";
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/duo/
    /// </summary>
    public async Task<PaginatedResult<object>> DuoListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_72a7ac63 = $"api/v3/authenticators/duo/";
        var response = await client.GetAsync(url_72a7ac63, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_72a7ac63 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_72a7ac63 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/duo/{id}/
    /// </summary>
    public async Task<object?> DuoRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6ece42a2 = $"api/v3/authenticators/duo/{id}/";
        var response = await client.GetAsync(url_6ece42a2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6ece42a2 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6ece42a2;
    }

    /// <summary>
    /// PUT /authenticators/duo/{id}/
    /// </summary>
    public async Task<object?> DuoUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e6889a44 = $"api/v3/authenticators/duo/{id}/";
        var response = await client.PutAsJsonAsync(url_e6889a44, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e6889a44 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e6889a44;
    }

    /// <summary>
    /// PATCH /authenticators/duo/{id}/
    /// </summary>
    public async Task<object?> DuoPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1aa00cdf = $"api/v3/authenticators/duo/{id}/";
        var response = await client.PatchAsJsonAsync(url_1aa00cdf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1aa00cdf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1aa00cdf;
    }

    /// <summary>
    /// DELETE /authenticators/duo/{id}/
    /// </summary>
    public async Task<object?> DuoDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ea176973 = $"api/v3/authenticators/duo/{id}/";
        var response = await client.DeleteAsync(url_ea176973, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ea176973 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ea176973;
    }

    /// <summary>
    /// GET /authenticators/duo/{id}/used_by/
    /// </summary>
    public async Task<object?> DuoUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_80817739 = $"api/v3/authenticators/duo/{id}/used_by/";
        var response = await client.GetAsync(url_80817739, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_80817739 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_80817739;
    }

    /// <summary>
    /// GET /authenticators/email/
    /// </summary>
    public async Task<PaginatedResult<object>> EmailListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cb25ff95 = $"api/v3/authenticators/email/";
        var response = await client.GetAsync(url_cb25ff95, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cb25ff95 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cb25ff95 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/email/{id}/
    /// </summary>
    public async Task<object?> EmailRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dc60881d = $"api/v3/authenticators/email/{id}/";
        var response = await client.GetAsync(url_dc60881d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dc60881d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dc60881d;
    }

    /// <summary>
    /// PUT /authenticators/email/{id}/
    /// </summary>
    public async Task<object?> EmailUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_36ee8c29 = $"api/v3/authenticators/email/{id}/";
        var response = await client.PutAsJsonAsync(url_36ee8c29, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_36ee8c29 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_36ee8c29;
    }

    /// <summary>
    /// PATCH /authenticators/email/{id}/
    /// </summary>
    public async Task<object?> EmailPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_818e2b8e = $"api/v3/authenticators/email/{id}/";
        var response = await client.PatchAsJsonAsync(url_818e2b8e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_818e2b8e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_818e2b8e;
    }

    /// <summary>
    /// DELETE /authenticators/email/{id}/
    /// </summary>
    public async Task<object?> EmailDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d89f9429 = $"api/v3/authenticators/email/{id}/";
        var response = await client.DeleteAsync(url_d89f9429, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d89f9429 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d89f9429;
    }

    /// <summary>
    /// GET /authenticators/email/{id}/used_by/
    /// </summary>
    public async Task<object?> EmailUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4b8f8fc9 = $"api/v3/authenticators/email/{id}/used_by/";
        var response = await client.GetAsync(url_4b8f8fc9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4b8f8fc9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4b8f8fc9;
    }

    /// <summary>
    /// GET /authenticators/endpoint/
    /// </summary>
    public async Task<PaginatedResult<object>> EndpointListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_796dea6b = $"api/v3/authenticators/endpoint/";
        var response = await client.GetAsync(url_796dea6b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_796dea6b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_796dea6b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/endpoint/{uuid}/
    /// </summary>
    public async Task<object?> EndpointRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b170e9ab = $"api/v3/authenticators/endpoint/{uuid}/";
        var response = await client.GetAsync(url_b170e9ab, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b170e9ab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b170e9ab;
    }

    /// <summary>
    /// GET /authenticators/endpoint/{uuid}/used_by/
    /// </summary>
    public async Task<object?> EndpointUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_968188fc = $"api/v3/authenticators/endpoint/{uuid}/used_by/";
        var response = await client.GetAsync(url_968188fc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_968188fc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_968188fc;
    }

    /// <summary>
    /// GET /authenticators/sms/
    /// </summary>
    public async Task<PaginatedResult<object>> SmsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e51a8898 = $"api/v3/authenticators/sms/";
        var response = await client.GetAsync(url_e51a8898, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e51a8898 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e51a8898 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/sms/{id}/
    /// </summary>
    public async Task<object?> SmsRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1bfbfee1 = $"api/v3/authenticators/sms/{id}/";
        var response = await client.GetAsync(url_1bfbfee1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1bfbfee1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1bfbfee1;
    }

    /// <summary>
    /// PUT /authenticators/sms/{id}/
    /// </summary>
    public async Task<object?> SmsUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c9eb9dd3 = $"api/v3/authenticators/sms/{id}/";
        var response = await client.PutAsJsonAsync(url_c9eb9dd3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c9eb9dd3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c9eb9dd3;
    }

    /// <summary>
    /// PATCH /authenticators/sms/{id}/
    /// </summary>
    public async Task<object?> SmsPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5877d3b9 = $"api/v3/authenticators/sms/{id}/";
        var response = await client.PatchAsJsonAsync(url_5877d3b9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5877d3b9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5877d3b9;
    }

    /// <summary>
    /// DELETE /authenticators/sms/{id}/
    /// </summary>
    public async Task<object?> SmsDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_398b6522 = $"api/v3/authenticators/sms/{id}/";
        var response = await client.DeleteAsync(url_398b6522, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_398b6522 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_398b6522;
    }

    /// <summary>
    /// GET /authenticators/sms/{id}/used_by/
    /// </summary>
    public async Task<object?> SmsUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_422da567 = $"api/v3/authenticators/sms/{id}/used_by/";
        var response = await client.GetAsync(url_422da567, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_422da567 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_422da567;
    }

    /// <summary>
    /// GET /authenticators/static/
    /// </summary>
    public async Task<PaginatedResult<object>> StaticListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0ea2dfbe = $"api/v3/authenticators/static/";
        var response = await client.GetAsync(url_0ea2dfbe, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0ea2dfbe = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0ea2dfbe ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/static/{id}/
    /// </summary>
    public async Task<object?> StaticRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_abf02128 = $"api/v3/authenticators/static/{id}/";
        var response = await client.GetAsync(url_abf02128, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_abf02128 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_abf02128;
    }

    /// <summary>
    /// PUT /authenticators/static/{id}/
    /// </summary>
    public async Task<object?> StaticUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f9f8b19c = $"api/v3/authenticators/static/{id}/";
        var response = await client.PutAsJsonAsync(url_f9f8b19c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f9f8b19c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f9f8b19c;
    }

    /// <summary>
    /// PATCH /authenticators/static/{id}/
    /// </summary>
    public async Task<object?> StaticPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d53ccb37 = $"api/v3/authenticators/static/{id}/";
        var response = await client.PatchAsJsonAsync(url_d53ccb37, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d53ccb37 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d53ccb37;
    }

    /// <summary>
    /// DELETE /authenticators/static/{id}/
    /// </summary>
    public async Task<object?> StaticDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b32a64a5 = $"api/v3/authenticators/static/{id}/";
        var response = await client.DeleteAsync(url_b32a64a5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b32a64a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b32a64a5;
    }

    /// <summary>
    /// GET /authenticators/static/{id}/used_by/
    /// </summary>
    public async Task<object?> StaticUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_957ac2ae = $"api/v3/authenticators/static/{id}/used_by/";
        var response = await client.GetAsync(url_957ac2ae, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_957ac2ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_957ac2ae;
    }

    /// <summary>
    /// GET /authenticators/totp/
    /// </summary>
    public async Task<PaginatedResult<object>> TotpListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3e2c9cc5 = $"api/v3/authenticators/totp/";
        var response = await client.GetAsync(url_3e2c9cc5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3e2c9cc5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3e2c9cc5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/totp/{id}/
    /// </summary>
    public async Task<object?> TotpRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a84fa674 = $"api/v3/authenticators/totp/{id}/";
        var response = await client.GetAsync(url_a84fa674, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a84fa674 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a84fa674;
    }

    /// <summary>
    /// PUT /authenticators/totp/{id}/
    /// </summary>
    public async Task<object?> TotpUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2546ce31 = $"api/v3/authenticators/totp/{id}/";
        var response = await client.PutAsJsonAsync(url_2546ce31, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2546ce31 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2546ce31;
    }

    /// <summary>
    /// PATCH /authenticators/totp/{id}/
    /// </summary>
    public async Task<object?> TotpPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8698e974 = $"api/v3/authenticators/totp/{id}/";
        var response = await client.PatchAsJsonAsync(url_8698e974, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8698e974 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8698e974;
    }

    /// <summary>
    /// DELETE /authenticators/totp/{id}/
    /// </summary>
    public async Task<object?> TotpDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3421101c = $"api/v3/authenticators/totp/{id}/";
        var response = await client.DeleteAsync(url_3421101c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3421101c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3421101c;
    }

    /// <summary>
    /// GET /authenticators/totp/{id}/used_by/
    /// </summary>
    public async Task<object?> TotpUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2e8ca947 = $"api/v3/authenticators/totp/{id}/used_by/";
        var response = await client.GetAsync(url_2e8ca947, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2e8ca947 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2e8ca947;
    }

    /// <summary>
    /// GET /authenticators/webauthn/
    /// </summary>
    public async Task<PaginatedResult<object>> WebauthnListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0f5e2793 = $"api/v3/authenticators/webauthn/";
        var response = await client.GetAsync(url_0f5e2793, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0f5e2793 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0f5e2793 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /authenticators/webauthn/{id}/
    /// </summary>
    public async Task<object?> WebauthnRetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_82dc961c = $"api/v3/authenticators/webauthn/{id}/";
        var response = await client.GetAsync(url_82dc961c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_82dc961c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_82dc961c;
    }

    /// <summary>
    /// PUT /authenticators/webauthn/{id}/
    /// </summary>
    public async Task<object?> WebauthnUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_afd55b20 = $"api/v3/authenticators/webauthn/{id}/";
        var response = await client.PutAsJsonAsync(url_afd55b20, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_afd55b20 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_afd55b20;
    }

    /// <summary>
    /// PATCH /authenticators/webauthn/{id}/
    /// </summary>
    public async Task<object?> WebauthnPartialUpdateAsync(int id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_79e41fc5 = $"api/v3/authenticators/webauthn/{id}/";
        var response = await client.PatchAsJsonAsync(url_79e41fc5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_79e41fc5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_79e41fc5;
    }

    /// <summary>
    /// DELETE /authenticators/webauthn/{id}/
    /// </summary>
    public async Task<object?> WebauthnDestroyAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98f68dbb = $"api/v3/authenticators/webauthn/{id}/";
        var response = await client.DeleteAsync(url_98f68dbb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98f68dbb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98f68dbb;
    }

    /// <summary>
    /// GET /authenticators/webauthn/{id}/used_by/
    /// </summary>
    public async Task<object?> WebauthnUsedByListAsync(int id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_246baaab = $"api/v3/authenticators/webauthn/{id}/used_by/";
        var response = await client.GetAsync(url_246baaab, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_246baaab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_246baaab;
    }

}
