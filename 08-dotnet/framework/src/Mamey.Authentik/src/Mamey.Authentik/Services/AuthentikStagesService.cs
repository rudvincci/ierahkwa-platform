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
/// Service implementation for Authentik Stages API operations.
/// </summary>
public class AuthentikStagesService : IAuthentikStagesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikStagesService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikStagesService"/> class.
    /// </summary>
    public AuthentikStagesService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikStagesService> logger,
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
    /// GET /stages/all/
    /// </summary>
    public async Task<PaginatedResult<object>> AllListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f372512f = $"api/v3/stages/all/";
        var response = await client.GetAsync(url_f372512f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f372512f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f372512f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /stages/all/{stage_uuid}/
    /// </summary>
    public async Task<object?> AllRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_95226f05 = $"api/v3/stages/all/{stage_uuid}/";
        var response = await client.GetAsync(url_95226f05, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_95226f05 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_95226f05;
    }

    /// <summary>
    /// DELETE /stages/all/{stage_uuid}/
    /// </summary>
    public async Task<object?> AllDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b02df8d9 = $"api/v3/stages/all/{stage_uuid}/";
        var response = await client.DeleteAsync(url_b02df8d9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b02df8d9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b02df8d9;
    }

    /// <summary>
    /// GET /stages/all/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AllUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a42dde8b = $"api/v3/stages/all/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_a42dde8b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a42dde8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a42dde8b;
    }

    /// <summary>
    /// GET /stages/all/types/
    /// </summary>
    public async Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bf388636 = $"api/v3/stages/all/types/";
        var response = await client.GetAsync(url_bf388636, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bf388636 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bf388636 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /stages/all/user_settings/
    /// </summary>
    public async Task<PaginatedResult<object>> AllUserSettingsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a72c654e = $"api/v3/stages/all/user_settings/";
        var response = await client.GetAsync(url_a72c654e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a72c654e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a72c654e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /stages/authenticator/duo/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorDuoListAsync(string? api_hostname = null, string? client_id = null, string? configure_flow = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_22a8a10e = $"api/v3/stages/authenticator/duo/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(api_hostname)) queryParams.Add($"api_hostname={api_hostname}");
        if (!string.IsNullOrEmpty(client_id)) queryParams.Add($"client_id={client_id}");
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (queryParams.Any()) url_22a8a10e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_22a8a10e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_22a8a10e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_22a8a10e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/duo/
    /// </summary>
    public async Task<object?> AuthenticatorDuoCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1f80dec6 = $"api/v3/stages/authenticator/duo/";
        var response = await client.PostAsJsonAsync(url_1f80dec6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1f80dec6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1f80dec6;
    }

    /// <summary>
    /// GET /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorDuoRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2b4c2995 = $"api/v3/stages/authenticator/duo/{stage_uuid}/";
        var response = await client.GetAsync(url_2b4c2995, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2b4c2995 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2b4c2995;
    }

    /// <summary>
    /// PUT /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorDuoUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7edb5483 = $"api/v3/stages/authenticator/duo/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_7edb5483, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7edb5483 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7edb5483;
    }

    /// <summary>
    /// PATCH /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorDuoPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7dcbf39e = $"api/v3/stages/authenticator/duo/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_7dcbf39e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7dcbf39e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7dcbf39e;
    }

    /// <summary>
    /// DELETE /stages/authenticator/duo/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorDuoDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_53c437ec = $"api/v3/stages/authenticator/duo/{stage_uuid}/";
        var response = await client.DeleteAsync(url_53c437ec, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_53c437ec = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_53c437ec;
    }

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/enrollment_status/
    /// </summary>
    public async Task<object?> AuthenticatorDuoEnrollmentStatusCreateAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f51e036a = $"api/v3/stages/authenticator/duo/{stage_uuid}/enrollment_status/";
        var response = await client.PostAsync(url_f51e036a, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f51e036a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f51e036a;
    }

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/import_device_manual/
    /// </summary>
    public async Task<object?> AuthenticatorDuoImportDeviceManualCreateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ae89aa68 = $"api/v3/stages/authenticator/duo/{stage_uuid}/import_device_manual/";
        var response = await client.PostAsJsonAsync(url_ae89aa68, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ae89aa68 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ae89aa68;
    }

    /// <summary>
    /// POST /stages/authenticator/duo/{stage_uuid}/import_devices_automatic/
    /// </summary>
    public async Task<object?> AuthenticatorDuoImportDevicesAutomaticCreateAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2105f687 = $"api/v3/stages/authenticator/duo/{stage_uuid}/import_devices_automatic/";
        var response = await client.PostAsync(url_2105f687, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2105f687 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2105f687;
    }

    /// <summary>
    /// GET /stages/authenticator/duo/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorDuoUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7a5e01e8 = $"api/v3/stages/authenticator/duo/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_7a5e01e8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7a5e01e8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7a5e01e8;
    }

    /// <summary>
    /// GET /stages/authenticator/email/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorEmailListAsync(string? configure_flow = null, string? friendly_name = null, string? from_address = null, string? host = null, string? password = null, int? port = null, string? stage_uuid = null, string? subject = null, string? template = null, int? timeout = null, string? token_expiry = null, bool? use_global_settings = null, bool? use_ssl = null, bool? use_tls = null, string? username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_27a3639a = $"api/v3/stages/authenticator/email/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (!string.IsNullOrEmpty(from_address)) queryParams.Add($"from_address={from_address}");
        if (!string.IsNullOrEmpty(host)) queryParams.Add($"host={host}");
        if (!string.IsNullOrEmpty(password)) queryParams.Add($"password={password}");
        if (port.HasValue) queryParams.Add($"port={port}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (!string.IsNullOrEmpty(subject)) queryParams.Add($"subject={subject}");
        if (!string.IsNullOrEmpty(template)) queryParams.Add($"template={template}");
        if (timeout.HasValue) queryParams.Add($"timeout={timeout}");
        if (!string.IsNullOrEmpty(token_expiry)) queryParams.Add($"token_expiry={token_expiry}");
        if (use_global_settings.HasValue) queryParams.Add($"use_global_settings={use_global_settings.Value.ToString().ToLower()}");
        if (use_ssl.HasValue) queryParams.Add($"use_ssl={use_ssl.Value.ToString().ToLower()}");
        if (use_tls.HasValue) queryParams.Add($"use_tls={use_tls.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(username)) queryParams.Add($"username={username}");
        if (queryParams.Any()) url_27a3639a += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_27a3639a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_27a3639a = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_27a3639a ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/email/
    /// </summary>
    public async Task<object?> AuthenticatorEmailCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c2f5764d = $"api/v3/stages/authenticator/email/";
        var response = await client.PostAsJsonAsync(url_c2f5764d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c2f5764d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c2f5764d;
    }

    /// <summary>
    /// GET /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEmailRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_04962f15 = $"api/v3/stages/authenticator/email/{stage_uuid}/";
        var response = await client.GetAsync(url_04962f15, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_04962f15 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_04962f15;
    }

    /// <summary>
    /// PUT /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEmailUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_58d60dba = $"api/v3/stages/authenticator/email/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_58d60dba, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_58d60dba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_58d60dba;
    }

    /// <summary>
    /// PATCH /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEmailPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4a5b0697 = $"api/v3/stages/authenticator/email/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_4a5b0697, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4a5b0697 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4a5b0697;
    }

    /// <summary>
    /// DELETE /stages/authenticator/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEmailDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8620b6a8 = $"api/v3/stages/authenticator/email/{stage_uuid}/";
        var response = await client.DeleteAsync(url_8620b6a8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8620b6a8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8620b6a8;
    }

    /// <summary>
    /// GET /stages/authenticator/email/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorEmailUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_25d65d3e = $"api/v3/stages/authenticator/email/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_25d65d3e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_25d65d3e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_25d65d3e;
    }

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorEndpointGdtcListAsync(string? configure_flow = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b71b07eb = $"api/v3/stages/authenticator/endpoint_gdtc/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (queryParams.Any()) url_b71b07eb += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b71b07eb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b71b07eb = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b71b07eb ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/endpoint_gdtc/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_78a48860 = $"api/v3/stages/authenticator/endpoint_gdtc/";
        var response = await client.PostAsJsonAsync(url_78a48860, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_78a48860 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_78a48860;
    }

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2324473e = $"api/v3/stages/authenticator/endpoint_gdtc/{stage_uuid}/";
        var response = await client.GetAsync(url_2324473e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2324473e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2324473e;
    }

    /// <summary>
    /// PUT /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3ce2cc44 = $"api/v3/stages/authenticator/endpoint_gdtc/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_3ce2cc44, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3ce2cc44 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3ce2cc44;
    }

    /// <summary>
    /// PATCH /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d99e726b = $"api/v3/stages/authenticator/endpoint_gdtc/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_d99e726b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d99e726b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d99e726b;
    }

    /// <summary>
    /// DELETE /stages/authenticator/endpoint_gdtc/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8b12cb03 = $"api/v3/stages/authenticator/endpoint_gdtc/{stage_uuid}/";
        var response = await client.DeleteAsync(url_8b12cb03, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8b12cb03 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8b12cb03;
    }

    /// <summary>
    /// GET /stages/authenticator/endpoint_gdtc/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorEndpointGdtcUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3e600e66 = $"api/v3/stages/authenticator/endpoint_gdtc/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_3e600e66, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3e600e66 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3e600e66;
    }

    /// <summary>
    /// GET /stages/authenticator/sms/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorSmsListAsync(string? account_sid = null, string? auth = null, string? auth_password = null, string? auth_type = null, string? configure_flow = null, string? friendly_name = null, string? from_number = null, string? mapping = null, string? provider = null, string? stage_uuid = null, bool? verify_only = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_61d2e64b = $"api/v3/stages/authenticator/sms/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(account_sid)) queryParams.Add($"account_sid={account_sid}");
        if (!string.IsNullOrEmpty(auth)) queryParams.Add($"auth={auth}");
        if (!string.IsNullOrEmpty(auth_password)) queryParams.Add($"auth_password={auth_password}");
        if (!string.IsNullOrEmpty(auth_type)) queryParams.Add($"auth_type={auth_type}");
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (!string.IsNullOrEmpty(from_number)) queryParams.Add($"from_number={from_number}");
        if (!string.IsNullOrEmpty(mapping)) queryParams.Add($"mapping={mapping}");
        if (!string.IsNullOrEmpty(provider)) queryParams.Add($"provider={provider}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (verify_only.HasValue) queryParams.Add($"verify_only={verify_only.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_61d2e64b += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_61d2e64b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_61d2e64b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_61d2e64b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/sms/
    /// </summary>
    public async Task<object?> AuthenticatorSmsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_425fb31d = $"api/v3/stages/authenticator/sms/";
        var response = await client.PostAsJsonAsync(url_425fb31d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_425fb31d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_425fb31d;
    }

    /// <summary>
    /// GET /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorSmsRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8d167542 = $"api/v3/stages/authenticator/sms/{stage_uuid}/";
        var response = await client.GetAsync(url_8d167542, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8d167542 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8d167542;
    }

    /// <summary>
    /// PUT /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorSmsUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df177b8b = $"api/v3/stages/authenticator/sms/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_df177b8b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df177b8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df177b8b;
    }

    /// <summary>
    /// PATCH /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorSmsPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_84d2c220 = $"api/v3/stages/authenticator/sms/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_84d2c220, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_84d2c220 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_84d2c220;
    }

    /// <summary>
    /// DELETE /stages/authenticator/sms/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorSmsDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cc83cc75 = $"api/v3/stages/authenticator/sms/{stage_uuid}/";
        var response = await client.DeleteAsync(url_cc83cc75, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cc83cc75 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cc83cc75;
    }

    /// <summary>
    /// GET /stages/authenticator/sms/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorSmsUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5f7e379b = $"api/v3/stages/authenticator/sms/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_5f7e379b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5f7e379b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5f7e379b;
    }

    /// <summary>
    /// GET /stages/authenticator/static/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorStaticListAsync(string? configure_flow = null, string? friendly_name = null, string? stage_uuid = null, int? token_count = null, int? token_length = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0c5e6aba = $"api/v3/stages/authenticator/static/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (token_count.HasValue) queryParams.Add($"token_count={token_count}");
        if (token_length.HasValue) queryParams.Add($"token_length={token_length}");
        if (queryParams.Any()) url_0c5e6aba += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_0c5e6aba, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0c5e6aba = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0c5e6aba ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/static/
    /// </summary>
    public async Task<object?> AuthenticatorStaticCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4e741aef = $"api/v3/stages/authenticator/static/";
        var response = await client.PostAsJsonAsync(url_4e741aef, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4e741aef = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4e741aef;
    }

    /// <summary>
    /// GET /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorStaticRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_89d590ab = $"api/v3/stages/authenticator/static/{stage_uuid}/";
        var response = await client.GetAsync(url_89d590ab, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_89d590ab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_89d590ab;
    }

    /// <summary>
    /// PUT /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorStaticUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4e5afc25 = $"api/v3/stages/authenticator/static/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_4e5afc25, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4e5afc25 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4e5afc25;
    }

    /// <summary>
    /// PATCH /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorStaticPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_efa82cb0 = $"api/v3/stages/authenticator/static/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_efa82cb0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_efa82cb0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_efa82cb0;
    }

    /// <summary>
    /// DELETE /stages/authenticator/static/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorStaticDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4e2b2c04 = $"api/v3/stages/authenticator/static/{stage_uuid}/";
        var response = await client.DeleteAsync(url_4e2b2c04, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4e2b2c04 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4e2b2c04;
    }

    /// <summary>
    /// GET /stages/authenticator/static/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorStaticUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f06b2b7d = $"api/v3/stages/authenticator/static/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_f06b2b7d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f06b2b7d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f06b2b7d;
    }

    /// <summary>
    /// GET /stages/authenticator/totp/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorTotpListAsync(string? configure_flow = null, string? digits = null, string? friendly_name = null, string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4ecd53e2 = $"api/v3/stages/authenticator/totp/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (!string.IsNullOrEmpty(digits)) queryParams.Add($"digits={digits}");
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_4ecd53e2 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_4ecd53e2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4ecd53e2 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4ecd53e2 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/totp/
    /// </summary>
    public async Task<object?> AuthenticatorTotpCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c95bc086 = $"api/v3/stages/authenticator/totp/";
        var response = await client.PostAsJsonAsync(url_c95bc086, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c95bc086 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c95bc086;
    }

    /// <summary>
    /// GET /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorTotpRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4be4d694 = $"api/v3/stages/authenticator/totp/{stage_uuid}/";
        var response = await client.GetAsync(url_4be4d694, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4be4d694 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4be4d694;
    }

    /// <summary>
    /// PUT /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorTotpUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0fa7fda0 = $"api/v3/stages/authenticator/totp/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_0fa7fda0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0fa7fda0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0fa7fda0;
    }

    /// <summary>
    /// PATCH /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorTotpPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ce24e5a4 = $"api/v3/stages/authenticator/totp/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_ce24e5a4, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ce24e5a4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ce24e5a4;
    }

    /// <summary>
    /// DELETE /stages/authenticator/totp/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorTotpDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c7a6053f = $"api/v3/stages/authenticator/totp/{stage_uuid}/";
        var response = await client.DeleteAsync(url_c7a6053f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c7a6053f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c7a6053f;
    }

    /// <summary>
    /// GET /stages/authenticator/totp/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorTotpUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4906e4a1 = $"api/v3/stages/authenticator/totp/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_4906e4a1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4906e4a1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4906e4a1;
    }

    /// <summary>
    /// GET /stages/authenticator/validate/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorValidateListAsync(string? configuration_stages = null, string? not_configured_action = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_908afb36 = $"api/v3/stages/authenticator/validate/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(configuration_stages)) queryParams.Add($"configuration_stages={configuration_stages}");
        if (!string.IsNullOrEmpty(not_configured_action)) queryParams.Add($"not_configured_action={not_configured_action}");
        if (queryParams.Any()) url_908afb36 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_908afb36, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_908afb36 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_908afb36 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/validate/
    /// </summary>
    public async Task<object?> AuthenticatorValidateCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_50791835 = $"api/v3/stages/authenticator/validate/";
        var response = await client.PostAsJsonAsync(url_50791835, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_50791835 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_50791835;
    }

    /// <summary>
    /// GET /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorValidateRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_30a2e08c = $"api/v3/stages/authenticator/validate/{stage_uuid}/";
        var response = await client.GetAsync(url_30a2e08c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_30a2e08c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_30a2e08c;
    }

    /// <summary>
    /// PUT /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorValidateUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b5cb8f08 = $"api/v3/stages/authenticator/validate/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_b5cb8f08, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b5cb8f08 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b5cb8f08;
    }

    /// <summary>
    /// PATCH /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorValidatePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d0d83527 = $"api/v3/stages/authenticator/validate/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_d0d83527, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d0d83527 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d0d83527;
    }

    /// <summary>
    /// DELETE /stages/authenticator/validate/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorValidateDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0f313be6 = $"api/v3/stages/authenticator/validate/{stage_uuid}/";
        var response = await client.DeleteAsync(url_0f313be6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0f313be6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0f313be6;
    }

    /// <summary>
    /// GET /stages/authenticator/validate/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorValidateUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_834758a0 = $"api/v3/stages/authenticator/validate/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_834758a0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_834758a0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_834758a0;
    }

    /// <summary>
    /// GET /stages/authenticator/webauthn/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorWebauthnListAsync(string? authenticator_attachment = null, string? configure_flow = null, string? device_type_restrictions = null, string? friendly_name = null, int? max_attempts = null, string? resident_key_requirement = null, string? stage_uuid = null, string? user_verification = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f4fd78a9 = $"api/v3/stages/authenticator/webauthn/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(authenticator_attachment)) queryParams.Add($"authenticator_attachment={authenticator_attachment}");
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (!string.IsNullOrEmpty(device_type_restrictions)) queryParams.Add($"device_type_restrictions={device_type_restrictions}");
        if (!string.IsNullOrEmpty(friendly_name)) queryParams.Add($"friendly_name={friendly_name}");
        if (max_attempts.HasValue) queryParams.Add($"max_attempts={max_attempts}");
        if (!string.IsNullOrEmpty(resident_key_requirement)) queryParams.Add($"resident_key_requirement={resident_key_requirement}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (!string.IsNullOrEmpty(user_verification)) queryParams.Add($"user_verification={user_verification}");
        if (queryParams.Any()) url_f4fd78a9 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_f4fd78a9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f4fd78a9 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f4fd78a9 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/authenticator/webauthn/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ffde52fb = $"api/v3/stages/authenticator/webauthn/";
        var response = await client.PostAsJsonAsync(url_ffde52fb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ffde52fb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ffde52fb;
    }

    /// <summary>
    /// GET /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_056425d6 = $"api/v3/stages/authenticator/webauthn/{stage_uuid}/";
        var response = await client.GetAsync(url_056425d6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_056425d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_056425d6;
    }

    /// <summary>
    /// PUT /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_312a674d = $"api/v3/stages/authenticator/webauthn/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_312a674d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_312a674d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_312a674d;
    }

    /// <summary>
    /// PATCH /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ae1b9bdd = $"api/v3/stages/authenticator/webauthn/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_ae1b9bdd, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ae1b9bdd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ae1b9bdd;
    }

    /// <summary>
    /// DELETE /stages/authenticator/webauthn/{stage_uuid}/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_46b1d1cd = $"api/v3/stages/authenticator/webauthn/{stage_uuid}/";
        var response = await client.DeleteAsync(url_46b1d1cd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_46b1d1cd = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_46b1d1cd;
    }

    /// <summary>
    /// GET /stages/authenticator/webauthn/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_429f6b0a = $"api/v3/stages/authenticator/webauthn/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_429f6b0a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_429f6b0a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_429f6b0a;
    }

    /// <summary>
    /// GET /stages/authenticator/webauthn_device_types/
    /// </summary>
    public async Task<PaginatedResult<object>> AuthenticatorWebauthnDeviceTypesListAsync(string? aaguid = null, string? description = null, string? icon = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_089ec4b5 = $"api/v3/stages/authenticator/webauthn_device_types/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(aaguid)) queryParams.Add($"aaguid={aaguid}");
        if (!string.IsNullOrEmpty(description)) queryParams.Add($"description={description}");
        if (!string.IsNullOrEmpty(icon)) queryParams.Add($"icon={icon}");
        if (queryParams.Any()) url_089ec4b5 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_089ec4b5, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_089ec4b5 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_089ec4b5 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /stages/authenticator/webauthn_device_types/{aaguid}/
    /// </summary>
    public async Task<object?> AuthenticatorWebauthnDeviceTypesRetrieveAsync(string aaguid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c3e7b892 = $"api/v3/stages/authenticator/webauthn_device_types/{aaguid}/";
        var response = await client.GetAsync(url_c3e7b892, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c3e7b892 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c3e7b892;
    }

    /// <summary>
    /// GET /stages/captcha/
    /// </summary>
    public async Task<PaginatedResult<object>> CaptchaListAsync(string? public_key = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_258095c2 = $"api/v3/stages/captcha/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(public_key)) queryParams.Add($"public_key={public_key}");
        if (queryParams.Any()) url_258095c2 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_258095c2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_258095c2 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_258095c2 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/captcha/
    /// </summary>
    public async Task<object?> CaptchaCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_54e8c34f = $"api/v3/stages/captcha/";
        var response = await client.PostAsJsonAsync(url_54e8c34f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_54e8c34f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_54e8c34f;
    }

    /// <summary>
    /// GET /stages/captcha/{stage_uuid}/
    /// </summary>
    public async Task<object?> CaptchaRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_629e3227 = $"api/v3/stages/captcha/{stage_uuid}/";
        var response = await client.GetAsync(url_629e3227, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_629e3227 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_629e3227;
    }

    /// <summary>
    /// PUT /stages/captcha/{stage_uuid}/
    /// </summary>
    public async Task<object?> CaptchaUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_98e9bbfe = $"api/v3/stages/captcha/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_98e9bbfe, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_98e9bbfe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_98e9bbfe;
    }

    /// <summary>
    /// PATCH /stages/captcha/{stage_uuid}/
    /// </summary>
    public async Task<object?> CaptchaPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_abc03c88 = $"api/v3/stages/captcha/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_abc03c88, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_abc03c88 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_abc03c88;
    }

    /// <summary>
    /// DELETE /stages/captcha/{stage_uuid}/
    /// </summary>
    public async Task<object?> CaptchaDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bb0a3efe = $"api/v3/stages/captcha/{stage_uuid}/";
        var response = await client.DeleteAsync(url_bb0a3efe, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bb0a3efe = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bb0a3efe;
    }

    /// <summary>
    /// GET /stages/captcha/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> CaptchaUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_27f7ae78 = $"api/v3/stages/captcha/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_27f7ae78, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_27f7ae78 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_27f7ae78;
    }

    /// <summary>
    /// GET /stages/consent/
    /// </summary>
    public async Task<PaginatedResult<object>> ConsentListAsync(string? consent_expire_in = null, string? mode = null, string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e5c9e445 = $"api/v3/stages/consent/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(consent_expire_in)) queryParams.Add($"consent_expire_in={consent_expire_in}");
        if (!string.IsNullOrEmpty(mode)) queryParams.Add($"mode={mode}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_e5c9e445 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_e5c9e445, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e5c9e445 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e5c9e445 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/consent/
    /// </summary>
    public async Task<object?> ConsentCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4362494 = $"api/v3/stages/consent/";
        var response = await client.PostAsJsonAsync(url_c4362494, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4362494 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4362494;
    }

    /// <summary>
    /// GET /stages/consent/{stage_uuid}/
    /// </summary>
    public async Task<object?> ConsentRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_42cc1697 = $"api/v3/stages/consent/{stage_uuid}/";
        var response = await client.GetAsync(url_42cc1697, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_42cc1697 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_42cc1697;
    }

    /// <summary>
    /// PUT /stages/consent/{stage_uuid}/
    /// </summary>
    public async Task<object?> ConsentUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9f4e7bdf = $"api/v3/stages/consent/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_9f4e7bdf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9f4e7bdf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9f4e7bdf;
    }

    /// <summary>
    /// PATCH /stages/consent/{stage_uuid}/
    /// </summary>
    public async Task<object?> ConsentPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0e72703b = $"api/v3/stages/consent/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_0e72703b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0e72703b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0e72703b;
    }

    /// <summary>
    /// DELETE /stages/consent/{stage_uuid}/
    /// </summary>
    public async Task<object?> ConsentDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3535dd5f = $"api/v3/stages/consent/{stage_uuid}/";
        var response = await client.DeleteAsync(url_3535dd5f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3535dd5f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3535dd5f;
    }

    /// <summary>
    /// GET /stages/consent/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> ConsentUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1bd97390 = $"api/v3/stages/consent/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_1bd97390, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1bd97390 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1bd97390;
    }

    /// <summary>
    /// GET /stages/deny/
    /// </summary>
    public async Task<PaginatedResult<object>> DenyListAsync(string? deny_message = null, string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a4faf549 = $"api/v3/stages/deny/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(deny_message)) queryParams.Add($"deny_message={deny_message}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_a4faf549 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_a4faf549, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a4faf549 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a4faf549 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/deny/
    /// </summary>
    public async Task<object?> DenyCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_882b5823 = $"api/v3/stages/deny/";
        var response = await client.PostAsJsonAsync(url_882b5823, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_882b5823 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_882b5823;
    }

    /// <summary>
    /// GET /stages/deny/{stage_uuid}/
    /// </summary>
    public async Task<object?> DenyRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f933594c = $"api/v3/stages/deny/{stage_uuid}/";
        var response = await client.GetAsync(url_f933594c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f933594c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f933594c;
    }

    /// <summary>
    /// PUT /stages/deny/{stage_uuid}/
    /// </summary>
    public async Task<object?> DenyUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a8bdb9bf = $"api/v3/stages/deny/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_a8bdb9bf, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a8bdb9bf = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a8bdb9bf;
    }

    /// <summary>
    /// PATCH /stages/deny/{stage_uuid}/
    /// </summary>
    public async Task<object?> DenyPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bfc7bf59 = $"api/v3/stages/deny/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_bfc7bf59, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bfc7bf59 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bfc7bf59;
    }

    /// <summary>
    /// DELETE /stages/deny/{stage_uuid}/
    /// </summary>
    public async Task<object?> DenyDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5ffa528e = $"api/v3/stages/deny/{stage_uuid}/";
        var response = await client.DeleteAsync(url_5ffa528e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5ffa528e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5ffa528e;
    }

    /// <summary>
    /// GET /stages/deny/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> DenyUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c049580f = $"api/v3/stages/deny/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_c049580f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c049580f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c049580f;
    }

    /// <summary>
    /// GET /stages/dummy/
    /// </summary>
    public async Task<PaginatedResult<object>> DummyListAsync(string? stage_uuid = null, bool? throw_error = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e6a69346 = $"api/v3/stages/dummy/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (throw_error.HasValue) queryParams.Add($"throw_error={throw_error.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_e6a69346 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_e6a69346, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e6a69346 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e6a69346 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/dummy/
    /// </summary>
    public async Task<object?> DummyCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_35825996 = $"api/v3/stages/dummy/";
        var response = await client.PostAsJsonAsync(url_35825996, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_35825996 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_35825996;
    }

    /// <summary>
    /// GET /stages/dummy/{stage_uuid}/
    /// </summary>
    public async Task<object?> DummyRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f667fc50 = $"api/v3/stages/dummy/{stage_uuid}/";
        var response = await client.GetAsync(url_f667fc50, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f667fc50 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f667fc50;
    }

    /// <summary>
    /// PUT /stages/dummy/{stage_uuid}/
    /// </summary>
    public async Task<object?> DummyUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_88f42186 = $"api/v3/stages/dummy/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_88f42186, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_88f42186 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_88f42186;
    }

    /// <summary>
    /// PATCH /stages/dummy/{stage_uuid}/
    /// </summary>
    public async Task<object?> DummyPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_241b5397 = $"api/v3/stages/dummy/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_241b5397, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_241b5397 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_241b5397;
    }

    /// <summary>
    /// DELETE /stages/dummy/{stage_uuid}/
    /// </summary>
    public async Task<object?> DummyDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_77058a71 = $"api/v3/stages/dummy/{stage_uuid}/";
        var response = await client.DeleteAsync(url_77058a71, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_77058a71 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_77058a71;
    }

    /// <summary>
    /// GET /stages/dummy/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> DummyUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ad2b17be = $"api/v3/stages/dummy/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_ad2b17be, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ad2b17be = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ad2b17be;
    }

    /// <summary>
    /// GET /stages/email/
    /// </summary>
    public async Task<PaginatedResult<object>> EmailListAsync(bool? activate_user_on_success = null, string? from_address = null, string? host = null, int? port = null, string? subject = null, string? template = null, int? timeout = null, string? token_expiry = null, bool? use_global_settings = null, bool? use_ssl = null, bool? use_tls = null, string? username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cb25ff95 = $"api/v3/stages/email/";
        var queryParams = new List<string>();
        if (activate_user_on_success.HasValue) queryParams.Add($"activate_user_on_success={activate_user_on_success.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(from_address)) queryParams.Add($"from_address={from_address}");
        if (!string.IsNullOrEmpty(host)) queryParams.Add($"host={host}");
        if (port.HasValue) queryParams.Add($"port={port}");
        if (!string.IsNullOrEmpty(subject)) queryParams.Add($"subject={subject}");
        if (!string.IsNullOrEmpty(template)) queryParams.Add($"template={template}");
        if (timeout.HasValue) queryParams.Add($"timeout={timeout}");
        if (!string.IsNullOrEmpty(token_expiry)) queryParams.Add($"token_expiry={token_expiry}");
        if (use_global_settings.HasValue) queryParams.Add($"use_global_settings={use_global_settings.Value.ToString().ToLower()}");
        if (use_ssl.HasValue) queryParams.Add($"use_ssl={use_ssl.Value.ToString().ToLower()}");
        if (use_tls.HasValue) queryParams.Add($"use_tls={use_tls.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(username)) queryParams.Add($"username={username}");
        if (queryParams.Any()) url_cb25ff95 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_cb25ff95, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cb25ff95 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cb25ff95 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/email/
    /// </summary>
    public async Task<object?> EmailCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5528168a = $"api/v3/stages/email/";
        var response = await client.PostAsJsonAsync(url_5528168a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5528168a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5528168a;
    }

    /// <summary>
    /// GET /stages/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> EmailRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dc60881d = $"api/v3/stages/email/{stage_uuid}/";
        var response = await client.GetAsync(url_dc60881d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dc60881d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dc60881d;
    }

    /// <summary>
    /// PUT /stages/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> EmailUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_36ee8c29 = $"api/v3/stages/email/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_36ee8c29, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_36ee8c29 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_36ee8c29;
    }

    /// <summary>
    /// PATCH /stages/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> EmailPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_818e2b8e = $"api/v3/stages/email/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_818e2b8e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_818e2b8e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_818e2b8e;
    }

    /// <summary>
    /// DELETE /stages/email/{stage_uuid}/
    /// </summary>
    public async Task<object?> EmailDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d89f9429 = $"api/v3/stages/email/{stage_uuid}/";
        var response = await client.DeleteAsync(url_d89f9429, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d89f9429 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d89f9429;
    }

    /// <summary>
    /// GET /stages/email/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> EmailUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4b8f8fc9 = $"api/v3/stages/email/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_4b8f8fc9, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4b8f8fc9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4b8f8fc9;
    }

    /// <summary>
    /// GET /stages/email/templates/
    /// </summary>
    public async Task<PaginatedResult<object>> EmailTemplatesListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_405728e6 = $"api/v3/stages/email/templates/";
        var response = await client.GetAsync(url_405728e6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_405728e6 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_405728e6 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /stages/identification/
    /// </summary>
    public async Task<PaginatedResult<object>> IdentificationListAsync(string? captcha_stage = null, bool? case_insensitive_matching = null, string? enrollment_flow = null, string? password_stage = null, string? passwordless_flow = null, string? recovery_flow = null, bool? show_matched_user = null, bool? show_source_labels = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_17064306 = $"api/v3/stages/identification/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(captcha_stage)) queryParams.Add($"captcha_stage={captcha_stage}");
        if (case_insensitive_matching.HasValue) queryParams.Add($"case_insensitive_matching={case_insensitive_matching.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(enrollment_flow)) queryParams.Add($"enrollment_flow={enrollment_flow}");
        if (!string.IsNullOrEmpty(password_stage)) queryParams.Add($"password_stage={password_stage}");
        if (!string.IsNullOrEmpty(passwordless_flow)) queryParams.Add($"passwordless_flow={passwordless_flow}");
        if (!string.IsNullOrEmpty(recovery_flow)) queryParams.Add($"recovery_flow={recovery_flow}");
        if (show_matched_user.HasValue) queryParams.Add($"show_matched_user={show_matched_user.Value.ToString().ToLower()}");
        if (show_source_labels.HasValue) queryParams.Add($"show_source_labels={show_source_labels.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_17064306 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_17064306, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_17064306 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_17064306 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/identification/
    /// </summary>
    public async Task<object?> IdentificationCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_42288f7d = $"api/v3/stages/identification/";
        var response = await client.PostAsJsonAsync(url_42288f7d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_42288f7d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_42288f7d;
    }

    /// <summary>
    /// GET /stages/identification/{stage_uuid}/
    /// </summary>
    public async Task<object?> IdentificationRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_33c1e6e4 = $"api/v3/stages/identification/{stage_uuid}/";
        var response = await client.GetAsync(url_33c1e6e4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_33c1e6e4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_33c1e6e4;
    }

    /// <summary>
    /// PUT /stages/identification/{stage_uuid}/
    /// </summary>
    public async Task<object?> IdentificationUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_984f47ba = $"api/v3/stages/identification/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_984f47ba, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_984f47ba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_984f47ba;
    }

    /// <summary>
    /// PATCH /stages/identification/{stage_uuid}/
    /// </summary>
    public async Task<object?> IdentificationPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6cd0daa7 = $"api/v3/stages/identification/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_6cd0daa7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6cd0daa7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6cd0daa7;
    }

    /// <summary>
    /// DELETE /stages/identification/{stage_uuid}/
    /// </summary>
    public async Task<object?> IdentificationDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bbfb3286 = $"api/v3/stages/identification/{stage_uuid}/";
        var response = await client.DeleteAsync(url_bbfb3286, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bbfb3286 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bbfb3286;
    }

    /// <summary>
    /// GET /stages/identification/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> IdentificationUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1bddbca0 = $"api/v3/stages/identification/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_1bddbca0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1bddbca0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1bddbca0;
    }

    /// <summary>
    /// GET /stages/invitation/invitations/
    /// </summary>
    public async Task<PaginatedResult<object>> InvitationInvitationsListAsync(string? created_by__username = null, string? expires = null, string? flow__slug = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_baa87ce7 = $"api/v3/stages/invitation/invitations/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(created_by__username)) queryParams.Add($"created_by__username={created_by__username}");
        if (!string.IsNullOrEmpty(expires)) queryParams.Add($"expires={expires}");
        if (!string.IsNullOrEmpty(flow__slug)) queryParams.Add($"flow__slug={flow__slug}");
        if (queryParams.Any()) url_baa87ce7 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_baa87ce7, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_baa87ce7 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_baa87ce7 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/invitation/invitations/
    /// </summary>
    public async Task<object?> InvitationInvitationsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5a89cc68 = $"api/v3/stages/invitation/invitations/";
        var response = await client.PostAsJsonAsync(url_5a89cc68, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5a89cc68 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5a89cc68;
    }

    /// <summary>
    /// GET /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    public async Task<object?> InvitationInvitationsRetrieveAsync(string invite_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d6686776 = $"api/v3/stages/invitation/invitations/{invite_uuid}/";
        var response = await client.GetAsync(url_d6686776, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d6686776 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d6686776;
    }

    /// <summary>
    /// PUT /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    public async Task<object?> InvitationInvitationsUpdateAsync(string invite_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_f7a3631c = $"api/v3/stages/invitation/invitations/{invite_uuid}/";
        var response = await client.PutAsJsonAsync(url_f7a3631c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_f7a3631c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_f7a3631c;
    }

    /// <summary>
    /// PATCH /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    public async Task<object?> InvitationInvitationsPartialUpdateAsync(string invite_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c50fb9cb = $"api/v3/stages/invitation/invitations/{invite_uuid}/";
        var response = await client.PatchAsJsonAsync(url_c50fb9cb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c50fb9cb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c50fb9cb;
    }

    /// <summary>
    /// DELETE /stages/invitation/invitations/{invite_uuid}/
    /// </summary>
    public async Task<object?> InvitationInvitationsDestroyAsync(string invite_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dbd36a59 = $"api/v3/stages/invitation/invitations/{invite_uuid}/";
        var response = await client.DeleteAsync(url_dbd36a59, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dbd36a59 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dbd36a59;
    }

    /// <summary>
    /// GET /stages/invitation/invitations/{invite_uuid}/used_by/
    /// </summary>
    public async Task<object?> InvitationInvitationsUsedByListAsync(string invite_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cb9ec2d1 = $"api/v3/stages/invitation/invitations/{invite_uuid}/used_by/";
        var response = await client.GetAsync(url_cb9ec2d1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cb9ec2d1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cb9ec2d1;
    }

    /// <summary>
    /// GET /stages/invitation/stages/
    /// </summary>
    public async Task<PaginatedResult<object>> InvitationStagesListAsync(bool? continue_flow_without_invitation = null, bool? no_flows = null, string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_32367711 = $"api/v3/stages/invitation/stages/";
        var queryParams = new List<string>();
        if (continue_flow_without_invitation.HasValue) queryParams.Add($"continue_flow_without_invitation={continue_flow_without_invitation.Value.ToString().ToLower()}");
        if (no_flows.HasValue) queryParams.Add($"no_flows={no_flows.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_32367711 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_32367711, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_32367711 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_32367711 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/invitation/stages/
    /// </summary>
    public async Task<object?> InvitationStagesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_27105ae6 = $"api/v3/stages/invitation/stages/";
        var response = await client.PostAsJsonAsync(url_27105ae6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_27105ae6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_27105ae6;
    }

    /// <summary>
    /// GET /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> InvitationStagesRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3fc3e421 = $"api/v3/stages/invitation/stages/{stage_uuid}/";
        var response = await client.GetAsync(url_3fc3e421, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3fc3e421 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3fc3e421;
    }

    /// <summary>
    /// PUT /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> InvitationStagesUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_22f22deb = $"api/v3/stages/invitation/stages/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_22f22deb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_22f22deb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_22f22deb;
    }

    /// <summary>
    /// PATCH /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> InvitationStagesPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_43082183 = $"api/v3/stages/invitation/stages/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_43082183, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_43082183 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_43082183;
    }

    /// <summary>
    /// DELETE /stages/invitation/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> InvitationStagesDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_446d1441 = $"api/v3/stages/invitation/stages/{stage_uuid}/";
        var response = await client.DeleteAsync(url_446d1441, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_446d1441 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_446d1441;
    }

    /// <summary>
    /// GET /stages/invitation/stages/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> InvitationStagesUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0e0e15c4 = $"api/v3/stages/invitation/stages/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_0e0e15c4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0e0e15c4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0e0e15c4;
    }

    /// <summary>
    /// GET /stages/mtls/
    /// </summary>
    public async Task<PaginatedResult<object>> MtlsListAsync(string? cert_attribute = null, string? certificate_authorities = null, string? mode = null, string? stage_uuid = null, string? user_attribute = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cc03c77c = $"api/v3/stages/mtls/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(cert_attribute)) queryParams.Add($"cert_attribute={cert_attribute}");
        if (!string.IsNullOrEmpty(certificate_authorities)) queryParams.Add($"certificate_authorities={certificate_authorities}");
        if (!string.IsNullOrEmpty(mode)) queryParams.Add($"mode={mode}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (!string.IsNullOrEmpty(user_attribute)) queryParams.Add($"user_attribute={user_attribute}");
        if (queryParams.Any()) url_cc03c77c += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_cc03c77c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cc03c77c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cc03c77c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/mtls/
    /// </summary>
    public async Task<object?> MtlsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_94c06820 = $"api/v3/stages/mtls/";
        var response = await client.PostAsJsonAsync(url_94c06820, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_94c06820 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_94c06820;
    }

    /// <summary>
    /// GET /stages/mtls/{stage_uuid}/
    /// </summary>
    public async Task<object?> MtlsRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a0416cba = $"api/v3/stages/mtls/{stage_uuid}/";
        var response = await client.GetAsync(url_a0416cba, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a0416cba = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a0416cba;
    }

    /// <summary>
    /// PUT /stages/mtls/{stage_uuid}/
    /// </summary>
    public async Task<object?> MtlsUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_11abacb9 = $"api/v3/stages/mtls/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_11abacb9, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_11abacb9 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_11abacb9;
    }

    /// <summary>
    /// PATCH /stages/mtls/{stage_uuid}/
    /// </summary>
    public async Task<object?> MtlsPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_78486e62 = $"api/v3/stages/mtls/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_78486e62, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_78486e62 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_78486e62;
    }

    /// <summary>
    /// DELETE /stages/mtls/{stage_uuid}/
    /// </summary>
    public async Task<object?> MtlsDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2fe04ea4 = $"api/v3/stages/mtls/{stage_uuid}/";
        var response = await client.DeleteAsync(url_2fe04ea4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2fe04ea4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2fe04ea4;
    }

    /// <summary>
    /// GET /stages/mtls/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> MtlsUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5fd1ff9e = $"api/v3/stages/mtls/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_5fd1ff9e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5fd1ff9e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5fd1ff9e;
    }

    /// <summary>
    /// GET /stages/password/
    /// </summary>
    public async Task<PaginatedResult<object>> PasswordListAsync(bool? allow_show_password = null, string? configure_flow = null, int? failed_attempts_before_cancel = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b63038fd = $"api/v3/stages/password/";
        var queryParams = new List<string>();
        if (allow_show_password.HasValue) queryParams.Add($"allow_show_password={allow_show_password.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(configure_flow)) queryParams.Add($"configure_flow={configure_flow}");
        if (failed_attempts_before_cancel.HasValue) queryParams.Add($"failed_attempts_before_cancel={failed_attempts_before_cancel}");
        if (queryParams.Any()) url_b63038fd += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b63038fd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b63038fd = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b63038fd ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/password/
    /// </summary>
    public async Task<object?> PasswordCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a5ca8702 = $"api/v3/stages/password/";
        var response = await client.PostAsJsonAsync(url_a5ca8702, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a5ca8702 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a5ca8702;
    }

    /// <summary>
    /// GET /stages/password/{stage_uuid}/
    /// </summary>
    public async Task<object?> PasswordRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2a6efff1 = $"api/v3/stages/password/{stage_uuid}/";
        var response = await client.GetAsync(url_2a6efff1, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2a6efff1 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2a6efff1;
    }

    /// <summary>
    /// PUT /stages/password/{stage_uuid}/
    /// </summary>
    public async Task<object?> PasswordUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e2bcf0a5 = $"api/v3/stages/password/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_e2bcf0a5, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e2bcf0a5 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e2bcf0a5;
    }

    /// <summary>
    /// PATCH /stages/password/{stage_uuid}/
    /// </summary>
    public async Task<object?> PasswordPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_eb2ee305 = $"api/v3/stages/password/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_eb2ee305, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_eb2ee305 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_eb2ee305;
    }

    /// <summary>
    /// DELETE /stages/password/{stage_uuid}/
    /// </summary>
    public async Task<object?> PasswordDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_efa0097b = $"api/v3/stages/password/{stage_uuid}/";
        var response = await client.DeleteAsync(url_efa0097b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_efa0097b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_efa0097b;
    }

    /// <summary>
    /// GET /stages/password/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> PasswordUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df6912b3 = $"api/v3/stages/password/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_df6912b3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df6912b3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df6912b3;
    }

    /// <summary>
    /// GET /stages/prompt/prompts/
    /// </summary>
    public async Task<PaginatedResult<object>> PromptPromptsListAsync(string? field_key = null, string? label = null, string? placeholder = null, string? type = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_83994203 = $"api/v3/stages/prompt/prompts/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(field_key)) queryParams.Add($"field_key={field_key}");
        if (!string.IsNullOrEmpty(label)) queryParams.Add($"label={label}");
        if (!string.IsNullOrEmpty(placeholder)) queryParams.Add($"placeholder={placeholder}");
        if (!string.IsNullOrEmpty(type)) queryParams.Add($"type={type}");
        if (queryParams.Any()) url_83994203 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_83994203, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_83994203 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_83994203 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/prompt/prompts/
    /// </summary>
    public async Task<object?> PromptPromptsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4145b535 = $"api/v3/stages/prompt/prompts/";
        var response = await client.PostAsJsonAsync(url_4145b535, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4145b535 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4145b535;
    }

    /// <summary>
    /// GET /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    public async Task<object?> PromptPromptsRetrieveAsync(string prompt_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_626d0d5a = $"api/v3/stages/prompt/prompts/{prompt_uuid}/";
        var response = await client.GetAsync(url_626d0d5a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_626d0d5a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_626d0d5a;
    }

    /// <summary>
    /// PUT /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    public async Task<object?> PromptPromptsUpdateAsync(string prompt_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b62b6cc7 = $"api/v3/stages/prompt/prompts/{prompt_uuid}/";
        var response = await client.PutAsJsonAsync(url_b62b6cc7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b62b6cc7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b62b6cc7;
    }

    /// <summary>
    /// PATCH /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    public async Task<object?> PromptPromptsPartialUpdateAsync(string prompt_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_56fa3e7e = $"api/v3/stages/prompt/prompts/{prompt_uuid}/";
        var response = await client.PatchAsJsonAsync(url_56fa3e7e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_56fa3e7e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_56fa3e7e;
    }

    /// <summary>
    /// DELETE /stages/prompt/prompts/{prompt_uuid}/
    /// </summary>
    public async Task<object?> PromptPromptsDestroyAsync(string prompt_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9d3c2078 = $"api/v3/stages/prompt/prompts/{prompt_uuid}/";
        var response = await client.DeleteAsync(url_9d3c2078, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9d3c2078 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9d3c2078;
    }

    /// <summary>
    /// GET /stages/prompt/prompts/{prompt_uuid}/used_by/
    /// </summary>
    public async Task<object?> PromptPromptsUsedByListAsync(string prompt_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3b7851bc = $"api/v3/stages/prompt/prompts/{prompt_uuid}/used_by/";
        var response = await client.GetAsync(url_3b7851bc, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3b7851bc = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3b7851bc;
    }

    /// <summary>
    /// POST /stages/prompt/prompts/preview/
    /// </summary>
    public async Task<object?> PromptPromptsPreviewCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0cc1b3f3 = $"api/v3/stages/prompt/prompts/preview/";
        var response = await client.PostAsJsonAsync(url_0cc1b3f3, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0cc1b3f3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0cc1b3f3;
    }

    /// <summary>
    /// GET /stages/prompt/stages/
    /// </summary>
    public async Task<PaginatedResult<object>> PromptStagesListAsync(string? fields = null, string? stage_uuid = null, string? validation_policies = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4789ddf = $"api/v3/stages/prompt/stages/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(fields)) queryParams.Add($"fields={fields}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (!string.IsNullOrEmpty(validation_policies)) queryParams.Add($"validation_policies={validation_policies}");
        if (queryParams.Any()) url_c4789ddf += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_c4789ddf, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4789ddf = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4789ddf ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/prompt/stages/
    /// </summary>
    public async Task<object?> PromptStagesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8e297908 = $"api/v3/stages/prompt/stages/";
        var response = await client.PostAsJsonAsync(url_8e297908, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8e297908 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8e297908;
    }

    /// <summary>
    /// GET /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> PromptStagesRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dcdd1097 = $"api/v3/stages/prompt/stages/{stage_uuid}/";
        var response = await client.GetAsync(url_dcdd1097, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dcdd1097 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dcdd1097;
    }

    /// <summary>
    /// PUT /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> PromptStagesUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_be00f239 = $"api/v3/stages/prompt/stages/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_be00f239, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_be00f239 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_be00f239;
    }

    /// <summary>
    /// PATCH /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> PromptStagesPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_71d90cfb = $"api/v3/stages/prompt/stages/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_71d90cfb, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_71d90cfb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_71d90cfb;
    }

    /// <summary>
    /// DELETE /stages/prompt/stages/{stage_uuid}/
    /// </summary>
    public async Task<object?> PromptStagesDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3027865c = $"api/v3/stages/prompt/stages/{stage_uuid}/";
        var response = await client.DeleteAsync(url_3027865c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3027865c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3027865c;
    }

    /// <summary>
    /// GET /stages/prompt/stages/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> PromptStagesUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_28c40311 = $"api/v3/stages/prompt/stages/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_28c40311, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_28c40311 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_28c40311;
    }

    /// <summary>
    /// GET /stages/redirect/
    /// </summary>
    public async Task<PaginatedResult<object>> RedirectListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8adfe8c2 = $"api/v3/stages/redirect/";
        var response = await client.GetAsync(url_8adfe8c2, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8adfe8c2 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8adfe8c2 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/redirect/
    /// </summary>
    public async Task<object?> RedirectCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2ad579d6 = $"api/v3/stages/redirect/";
        var response = await client.PostAsJsonAsync(url_2ad579d6, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2ad579d6 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2ad579d6;
    }

    /// <summary>
    /// GET /stages/redirect/{stage_uuid}/
    /// </summary>
    public async Task<object?> RedirectRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ce7e4054 = $"api/v3/stages/redirect/{stage_uuid}/";
        var response = await client.GetAsync(url_ce7e4054, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ce7e4054 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ce7e4054;
    }

    /// <summary>
    /// PUT /stages/redirect/{stage_uuid}/
    /// </summary>
    public async Task<object?> RedirectUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_cc24ee31 = $"api/v3/stages/redirect/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_cc24ee31, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_cc24ee31 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_cc24ee31;
    }

    /// <summary>
    /// PATCH /stages/redirect/{stage_uuid}/
    /// </summary>
    public async Task<object?> RedirectPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_21e2212c = $"api/v3/stages/redirect/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_21e2212c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_21e2212c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_21e2212c;
    }

    /// <summary>
    /// DELETE /stages/redirect/{stage_uuid}/
    /// </summary>
    public async Task<object?> RedirectDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_a58356d3 = $"api/v3/stages/redirect/{stage_uuid}/";
        var response = await client.DeleteAsync(url_a58356d3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_a58356d3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_a58356d3;
    }

    /// <summary>
    /// GET /stages/redirect/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> RedirectUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_86cdbf71 = $"api/v3/stages/redirect/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_86cdbf71, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_86cdbf71 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_86cdbf71;
    }

    /// <summary>
    /// GET /stages/source/
    /// </summary>
    public async Task<PaginatedResult<object>> SourceListAsync(string? resume_timeout = null, string? source = null, string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b68bd55f = $"api/v3/stages/source/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(resume_timeout)) queryParams.Add($"resume_timeout={resume_timeout}");
        if (!string.IsNullOrEmpty(source)) queryParams.Add($"source={source}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_b68bd55f += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b68bd55f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b68bd55f = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b68bd55f ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/source/
    /// </summary>
    public async Task<object?> SourceCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_00f4fa8f = $"api/v3/stages/source/";
        var response = await client.PostAsJsonAsync(url_00f4fa8f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_00f4fa8f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_00f4fa8f;
    }

    /// <summary>
    /// GET /stages/source/{stage_uuid}/
    /// </summary>
    public async Task<object?> SourceRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b1fb2f76 = $"api/v3/stages/source/{stage_uuid}/";
        var response = await client.GetAsync(url_b1fb2f76, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b1fb2f76 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b1fb2f76;
    }

    /// <summary>
    /// PUT /stages/source/{stage_uuid}/
    /// </summary>
    public async Task<object?> SourceUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c8e7c076 = $"api/v3/stages/source/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_c8e7c076, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c8e7c076 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c8e7c076;
    }

    /// <summary>
    /// PATCH /stages/source/{stage_uuid}/
    /// </summary>
    public async Task<object?> SourcePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c2e1840a = $"api/v3/stages/source/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_c2e1840a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c2e1840a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c2e1840a;
    }

    /// <summary>
    /// DELETE /stages/source/{stage_uuid}/
    /// </summary>
    public async Task<object?> SourceDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_dcce7d6d = $"api/v3/stages/source/{stage_uuid}/";
        var response = await client.DeleteAsync(url_dcce7d6d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_dcce7d6d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_dcce7d6d;
    }

    /// <summary>
    /// GET /stages/source/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> SourceUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_45a7c770 = $"api/v3/stages/source/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_45a7c770, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_45a7c770 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_45a7c770;
    }

    /// <summary>
    /// GET /stages/user_delete/
    /// </summary>
    public async Task<PaginatedResult<object>> UserDeleteListAsync(string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_38dd95df = $"api/v3/stages/user_delete/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_38dd95df += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_38dd95df, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_38dd95df = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_38dd95df ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/user_delete/
    /// </summary>
    public async Task<object?> UserDeleteCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7d85573a = $"api/v3/stages/user_delete/";
        var response = await client.PostAsJsonAsync(url_7d85573a, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7d85573a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7d85573a;
    }

    /// <summary>
    /// GET /stages/user_delete/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserDeleteRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_df784460 = $"api/v3/stages/user_delete/{stage_uuid}/";
        var response = await client.GetAsync(url_df784460, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_df784460 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_df784460;
    }

    /// <summary>
    /// PUT /stages/user_delete/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserDeleteUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0341517c = $"api/v3/stages/user_delete/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_0341517c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0341517c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0341517c;
    }

    /// <summary>
    /// PATCH /stages/user_delete/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserDeletePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_2f8ffa87 = $"api/v3/stages/user_delete/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_2f8ffa87, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_2f8ffa87 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_2f8ffa87;
    }

    /// <summary>
    /// DELETE /stages/user_delete/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserDeleteDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d3a817d4 = $"api/v3/stages/user_delete/{stage_uuid}/";
        var response = await client.DeleteAsync(url_d3a817d4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d3a817d4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d3a817d4;
    }

    /// <summary>
    /// GET /stages/user_delete/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> UserDeleteUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c13f5ce8 = $"api/v3/stages/user_delete/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_c13f5ce8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c13f5ce8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c13f5ce8;
    }

    /// <summary>
    /// GET /stages/user_login/
    /// </summary>
    public async Task<PaginatedResult<object>> UserLoginListAsync(string? geoip_binding = null, string? network_binding = null, string? remember_device = null, string? remember_me_offset = null, string? session_duration = null, string? stage_uuid = null, bool? terminate_other_sessions = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_52b9c3fd = $"api/v3/stages/user_login/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(geoip_binding)) queryParams.Add($"geoip_binding={geoip_binding}");
        if (!string.IsNullOrEmpty(network_binding)) queryParams.Add($"network_binding={network_binding}");
        if (!string.IsNullOrEmpty(remember_device)) queryParams.Add($"remember_device={remember_device}");
        if (!string.IsNullOrEmpty(remember_me_offset)) queryParams.Add($"remember_me_offset={remember_me_offset}");
        if (!string.IsNullOrEmpty(session_duration)) queryParams.Add($"session_duration={session_duration}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (terminate_other_sessions.HasValue) queryParams.Add($"terminate_other_sessions={terminate_other_sessions.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_52b9c3fd += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_52b9c3fd, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_52b9c3fd = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_52b9c3fd ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/user_login/
    /// </summary>
    public async Task<object?> UserLoginCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_227f7610 = $"api/v3/stages/user_login/";
        var response = await client.PostAsJsonAsync(url_227f7610, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_227f7610 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_227f7610;
    }

    /// <summary>
    /// GET /stages/user_login/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLoginRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0494a275 = $"api/v3/stages/user_login/{stage_uuid}/";
        var response = await client.GetAsync(url_0494a275, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0494a275 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0494a275;
    }

    /// <summary>
    /// PUT /stages/user_login/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLoginUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c4b96c99 = $"api/v3/stages/user_login/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_c4b96c99, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c4b96c99 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c4b96c99;
    }

    /// <summary>
    /// PATCH /stages/user_login/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLoginPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1b5be0ab = $"api/v3/stages/user_login/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_1b5be0ab, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1b5be0ab = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1b5be0ab;
    }

    /// <summary>
    /// DELETE /stages/user_login/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLoginDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c3cbf561 = $"api/v3/stages/user_login/{stage_uuid}/";
        var response = await client.DeleteAsync(url_c3cbf561, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c3cbf561 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c3cbf561;
    }

    /// <summary>
    /// GET /stages/user_login/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> UserLoginUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ba300351 = $"api/v3/stages/user_login/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_ba300351, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ba300351 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ba300351;
    }

    /// <summary>
    /// GET /stages/user_logout/
    /// </summary>
    public async Task<PaginatedResult<object>> UserLogoutListAsync(string? stage_uuid = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5d6ee02e = $"api/v3/stages/user_logout/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (queryParams.Any()) url_5d6ee02e += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_5d6ee02e, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5d6ee02e = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5d6ee02e ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/user_logout/
    /// </summary>
    public async Task<object?> UserLogoutCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_72d359f7 = $"api/v3/stages/user_logout/";
        var response = await client.PostAsJsonAsync(url_72d359f7, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_72d359f7 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_72d359f7;
    }

    /// <summary>
    /// GET /stages/user_logout/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLogoutRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6bab7a80 = $"api/v3/stages/user_logout/{stage_uuid}/";
        var response = await client.GetAsync(url_6bab7a80, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6bab7a80 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6bab7a80;
    }

    /// <summary>
    /// PUT /stages/user_logout/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLogoutUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e6ed70a4 = $"api/v3/stages/user_logout/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_e6ed70a4, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e6ed70a4 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e6ed70a4;
    }

    /// <summary>
    /// PATCH /stages/user_logout/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLogoutPartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_75eb7225 = $"api/v3/stages/user_logout/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_75eb7225, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_75eb7225 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_75eb7225;
    }

    /// <summary>
    /// DELETE /stages/user_logout/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserLogoutDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_39689044 = $"api/v3/stages/user_logout/{stage_uuid}/";
        var response = await client.DeleteAsync(url_39689044, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_39689044 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_39689044;
    }

    /// <summary>
    /// GET /stages/user_logout/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> UserLogoutUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e6e9fd96 = $"api/v3/stages/user_logout/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_e6e9fd96, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e6e9fd96 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e6e9fd96;
    }

    /// <summary>
    /// GET /stages/user_write/
    /// </summary>
    public async Task<PaginatedResult<object>> UserWriteListAsync(bool? create_users_as_inactive = null, string? create_users_group = null, string? stage_uuid = null, string? user_creation_mode = null, string? user_path_template = null, string? user_type = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d321ce75 = $"api/v3/stages/user_write/";
        var queryParams = new List<string>();
        if (create_users_as_inactive.HasValue) queryParams.Add($"create_users_as_inactive={create_users_as_inactive.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(create_users_group)) queryParams.Add($"create_users_group={create_users_group}");
        if (!string.IsNullOrEmpty(stage_uuid)) queryParams.Add($"stage_uuid={stage_uuid}");
        if (!string.IsNullOrEmpty(user_creation_mode)) queryParams.Add($"user_creation_mode={user_creation_mode}");
        if (!string.IsNullOrEmpty(user_path_template)) queryParams.Add($"user_path_template={user_path_template}");
        if (!string.IsNullOrEmpty(user_type)) queryParams.Add($"user_type={user_type}");
        if (queryParams.Any()) url_d321ce75 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_d321ce75, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d321ce75 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d321ce75 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /stages/user_write/
    /// </summary>
    public async Task<object?> UserWriteCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_ca899005 = $"api/v3/stages/user_write/";
        var response = await client.PostAsJsonAsync(url_ca899005, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_ca899005 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_ca899005;
    }

    /// <summary>
    /// GET /stages/user_write/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserWriteRetrieveAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3f440ddb = $"api/v3/stages/user_write/{stage_uuid}/";
        var response = await client.GetAsync(url_3f440ddb, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3f440ddb = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3f440ddb;
    }

    /// <summary>
    /// PUT /stages/user_write/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserWriteUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b3231e2e = $"api/v3/stages/user_write/{stage_uuid}/";
        var response = await client.PutAsJsonAsync(url_b3231e2e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b3231e2e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b3231e2e;
    }

    /// <summary>
    /// PATCH /stages/user_write/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserWritePartialUpdateAsync(string stage_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_adb2285f = $"api/v3/stages/user_write/{stage_uuid}/";
        var response = await client.PatchAsJsonAsync(url_adb2285f, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_adb2285f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_adb2285f;
    }

    /// <summary>
    /// DELETE /stages/user_write/{stage_uuid}/
    /// </summary>
    public async Task<object?> UserWriteDestroyAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_629c113d = $"api/v3/stages/user_write/{stage_uuid}/";
        var response = await client.DeleteAsync(url_629c113d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_629c113d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_629c113d;
    }

    /// <summary>
    /// GET /stages/user_write/{stage_uuid}/used_by/
    /// </summary>
    public async Task<object?> UserWriteUsedByListAsync(string stage_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e680ea0f = $"api/v3/stages/user_write/{stage_uuid}/used_by/";
        var response = await client.GetAsync(url_e680ea0f, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e680ea0f = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e680ea0f;
    }

}
