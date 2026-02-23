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
/// Service implementation for Authentik Events API operations.
/// </summary>
public class AuthentikEventsService : IAuthentikEventsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikEventsService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikEventsService"/> class.
    /// </summary>
    public AuthentikEventsService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikEventsService> logger,
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
    /// GET /events/events/
    /// </summary>
    public async Task<PaginatedResult<object>> EventsListAsync(string? action = null, string? actions = null, string? brand_name = null, string? client_ip = null, string? context_authorized_app = null, string? context_model_app = null, string? context_model_name = null, string? context_model_pk = null, string? username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1304e053 = $"api/v3/events/events/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(action)) queryParams.Add($"action={action}");
        if (!string.IsNullOrEmpty(actions)) queryParams.Add($"actions={actions}");
        if (!string.IsNullOrEmpty(brand_name)) queryParams.Add($"brand_name={brand_name}");
        if (!string.IsNullOrEmpty(client_ip)) queryParams.Add($"client_ip={client_ip}");
        if (!string.IsNullOrEmpty(context_authorized_app)) queryParams.Add($"context_authorized_app={context_authorized_app}");
        if (!string.IsNullOrEmpty(context_model_app)) queryParams.Add($"context_model_app={context_model_app}");
        if (!string.IsNullOrEmpty(context_model_name)) queryParams.Add($"context_model_name={context_model_name}");
        if (!string.IsNullOrEmpty(context_model_pk)) queryParams.Add($"context_model_pk={context_model_pk}");
        if (!string.IsNullOrEmpty(username)) queryParams.Add($"username={username}");
        if (queryParams.Any()) url_1304e053 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_1304e053, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1304e053 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1304e053 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /events/events/
    /// </summary>
    public async Task<object?> EventsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d8fe7a66 = $"api/v3/events/events/";
        var response = await client.PostAsJsonAsync(url_d8fe7a66, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d8fe7a66 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d8fe7a66;
    }

    /// <summary>
    /// GET /events/events/{event_uuid}/
    /// </summary>
    public async Task<object?> EventsRetrieveAsync(string event_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_af1cb732 = $"api/v3/events/events/{event_uuid}/";
        var response = await client.GetAsync(url_af1cb732, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_af1cb732 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_af1cb732;
    }

    /// <summary>
    /// PUT /events/events/{event_uuid}/
    /// </summary>
    public async Task<object?> EventsUpdateAsync(string event_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_80fa8503 = $"api/v3/events/events/{event_uuid}/";
        var response = await client.PutAsJsonAsync(url_80fa8503, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_80fa8503 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_80fa8503;
    }

    /// <summary>
    /// PATCH /events/events/{event_uuid}/
    /// </summary>
    public async Task<object?> EventsPartialUpdateAsync(string event_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_170d841d = $"api/v3/events/events/{event_uuid}/";
        var response = await client.PatchAsJsonAsync(url_170d841d, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_170d841d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_170d841d;
    }

    /// <summary>
    /// DELETE /events/events/{event_uuid}/
    /// </summary>
    public async Task<object?> EventsDestroyAsync(string event_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_526a5ca0 = $"api/v3/events/events/{event_uuid}/";
        var response = await client.DeleteAsync(url_526a5ca0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_526a5ca0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_526a5ca0;
    }

    /// <summary>
    /// GET /events/events/actions/
    /// </summary>
    public async Task<PaginatedResult<object>> EventsActionsListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6106364b = $"api/v3/events/events/actions/";
        var response = await client.GetAsync(url_6106364b, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6106364b = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6106364b ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /events/events/top_per_user/
    /// </summary>
    public async Task<PaginatedResult<object>> EventsTopPerUserListAsync(string? action = null, int? top_n = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_6c4ac7b6 = $"api/v3/events/events/top_per_user/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(action)) queryParams.Add($"action={action}");
        if (top_n.HasValue) queryParams.Add($"top_n={top_n}");
        if (queryParams.Any()) url_6c4ac7b6 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_6c4ac7b6, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_6c4ac7b6 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_6c4ac7b6 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /events/events/volume/
    /// </summary>
    public async Task<PaginatedResult<object>> EventsVolumeListAsync(string? action = null, string? actions = null, string? brand_name = null, string? client_ip = null, string? context_authorized_app = null, string? context_model_app = null, string? context_model_name = null, string? context_model_pk = null, double? history_days = null, string? username = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_92718929 = $"api/v3/events/events/volume/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(action)) queryParams.Add($"action={action}");
        if (!string.IsNullOrEmpty(actions)) queryParams.Add($"actions={actions}");
        if (!string.IsNullOrEmpty(brand_name)) queryParams.Add($"brand_name={brand_name}");
        if (!string.IsNullOrEmpty(client_ip)) queryParams.Add($"client_ip={client_ip}");
        if (!string.IsNullOrEmpty(context_authorized_app)) queryParams.Add($"context_authorized_app={context_authorized_app}");
        if (!string.IsNullOrEmpty(context_model_app)) queryParams.Add($"context_model_app={context_model_app}");
        if (!string.IsNullOrEmpty(context_model_name)) queryParams.Add($"context_model_name={context_model_name}");
        if (!string.IsNullOrEmpty(context_model_pk)) queryParams.Add($"context_model_pk={context_model_pk}");
        if (history_days.HasValue) queryParams.Add($"history_days={history_days}");
        if (!string.IsNullOrEmpty(username)) queryParams.Add($"username={username}");
        if (queryParams.Any()) url_92718929 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_92718929, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_92718929 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_92718929 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /events/notifications/
    /// </summary>
    public async Task<PaginatedResult<object>> NotificationsListAsync(string? body = null, string? created = null, string? @event = null, bool? seen = null, string? severity = null, int? user = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b7a790a3 = $"api/v3/events/notifications/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(body)) queryParams.Add($"body={body}");
        if (!string.IsNullOrEmpty(created)) queryParams.Add($"created={created}");
        if (!string.IsNullOrEmpty(@event)) queryParams.Add($"@event={@event}");
        if (seen.HasValue) queryParams.Add($"seen={seen.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(severity)) queryParams.Add($"severity={severity}");
        if (user.HasValue) queryParams.Add($"user={user}");
        if (queryParams.Any()) url_b7a790a3 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b7a790a3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b7a790a3 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b7a790a3 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /events/notifications/{uuid}/
    /// </summary>
    public async Task<object?> NotificationsRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_3bebadff = $"api/v3/events/notifications/{uuid}/";
        var response = await client.GetAsync(url_3bebadff, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_3bebadff = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_3bebadff;
    }

    /// <summary>
    /// PUT /events/notifications/{uuid}/
    /// </summary>
    public async Task<object?> NotificationsUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5f2ac46b = $"api/v3/events/notifications/{uuid}/";
        var response = await client.PutAsJsonAsync(url_5f2ac46b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5f2ac46b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5f2ac46b;
    }

    /// <summary>
    /// PATCH /events/notifications/{uuid}/
    /// </summary>
    public async Task<object?> NotificationsPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_80cb6a23 = $"api/v3/events/notifications/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_80cb6a23, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_80cb6a23 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_80cb6a23;
    }

    /// <summary>
    /// DELETE /events/notifications/{uuid}/
    /// </summary>
    public async Task<object?> NotificationsDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_031d0893 = $"api/v3/events/notifications/{uuid}/";
        var response = await client.DeleteAsync(url_031d0893, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_031d0893 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_031d0893;
    }

    /// <summary>
    /// GET /events/notifications/{uuid}/used_by/
    /// </summary>
    public async Task<object?> NotificationsUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0422c3a8 = $"api/v3/events/notifications/{uuid}/used_by/";
        var response = await client.GetAsync(url_0422c3a8, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0422c3a8 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0422c3a8;
    }

    /// <summary>
    /// POST /events/notifications/mark_all_seen/
    /// </summary>
    public async Task<object?> NotificationsMarkAllSeenCreateAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_17e354ae = $"api/v3/events/notifications/mark_all_seen/";
        var response = await client.PostAsync(url_17e354ae, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_17e354ae = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_17e354ae;
    }

    /// <summary>
    /// GET /events/rules/
    /// </summary>
    public async Task<PaginatedResult<object>> RulesListAsync(string? destination_group__name = null, string? severity = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b3e0916c = $"api/v3/events/rules/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(destination_group__name)) queryParams.Add($"destination_group__name={destination_group__name}");
        if (!string.IsNullOrEmpty(severity)) queryParams.Add($"severity={severity}");
        if (queryParams.Any()) url_b3e0916c += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_b3e0916c, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b3e0916c = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b3e0916c ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /events/rules/
    /// </summary>
    public async Task<object?> RulesCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_b1ad6a8b = $"api/v3/events/rules/";
        var response = await client.PostAsJsonAsync(url_b1ad6a8b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_b1ad6a8b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_b1ad6a8b;
    }

    /// <summary>
    /// GET /events/rules/{pbm_uuid}/
    /// </summary>
    public async Task<object?> RulesRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_60aef7ac = $"api/v3/events/rules/{pbm_uuid}/";
        var response = await client.GetAsync(url_60aef7ac, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_60aef7ac = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_60aef7ac;
    }

    /// <summary>
    /// PUT /events/rules/{pbm_uuid}/
    /// </summary>
    public async Task<object?> RulesUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bfe9da10 = $"api/v3/events/rules/{pbm_uuid}/";
        var response = await client.PutAsJsonAsync(url_bfe9da10, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bfe9da10 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bfe9da10;
    }

    /// <summary>
    /// PATCH /events/rules/{pbm_uuid}/
    /// </summary>
    public async Task<object?> RulesPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_1926701b = $"api/v3/events/rules/{pbm_uuid}/";
        var response = await client.PatchAsJsonAsync(url_1926701b, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_1926701b = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_1926701b;
    }

    /// <summary>
    /// DELETE /events/rules/{pbm_uuid}/
    /// </summary>
    public async Task<object?> RulesDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_9543f2d0 = $"api/v3/events/rules/{pbm_uuid}/";
        var response = await client.DeleteAsync(url_9543f2d0, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_9543f2d0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_9543f2d0;
    }

    /// <summary>
    /// GET /events/rules/{pbm_uuid}/used_by/
    /// </summary>
    public async Task<object?> RulesUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_0fb3e521 = $"api/v3/events/rules/{pbm_uuid}/used_by/";
        var response = await client.GetAsync(url_0fb3e521, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_0fb3e521 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_0fb3e521;
    }

    /// <summary>
    /// GET /events/transports/
    /// </summary>
    public async Task<PaginatedResult<object>> TransportsListAsync(string? mode = null, bool? send_once = null, string? webhook_url = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5fbdc146 = $"api/v3/events/transports/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(mode)) queryParams.Add($"mode={mode}");
        if (send_once.HasValue) queryParams.Add($"send_once={send_once.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(webhook_url)) queryParams.Add($"webhook_url={webhook_url}");
        if (queryParams.Any()) url_5fbdc146 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_5fbdc146, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5fbdc146 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5fbdc146 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// POST /events/transports/
    /// </summary>
    public async Task<object?> TransportsCreateAsync(object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_be907fee = $"api/v3/events/transports/";
        var response = await client.PostAsJsonAsync(url_be907fee, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_be907fee = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_be907fee;
    }

    /// <summary>
    /// GET /events/transports/{uuid}/
    /// </summary>
    public async Task<object?> TransportsRetrieveAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_32bbb9d3 = $"api/v3/events/transports/{uuid}/";
        var response = await client.GetAsync(url_32bbb9d3, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_32bbb9d3 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_32bbb9d3;
    }

    /// <summary>
    /// PUT /events/transports/{uuid}/
    /// </summary>
    public async Task<object?> TransportsUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_18696952 = $"api/v3/events/transports/{uuid}/";
        var response = await client.PutAsJsonAsync(url_18696952, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_18696952 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_18696952;
    }

    /// <summary>
    /// PATCH /events/transports/{uuid}/
    /// </summary>
    public async Task<object?> TransportsPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_c5717da0 = $"api/v3/events/transports/{uuid}/";
        var response = await client.PatchAsJsonAsync(url_c5717da0, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_c5717da0 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_c5717da0;
    }

    /// <summary>
    /// DELETE /events/transports/{uuid}/
    /// </summary>
    public async Task<object?> TransportsDestroyAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_4274da7a = $"api/v3/events/transports/{uuid}/";
        var response = await client.DeleteAsync(url_4274da7a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_4274da7a = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_4274da7a;
    }

    /// <summary>
    /// POST /events/transports/{uuid}/test/
    /// </summary>
    public async Task<object?> TransportsTestCreateAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8001524d = $"api/v3/events/transports/{uuid}/test/";
        var response = await client.PostAsync(url_8001524d, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8001524d = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8001524d;
    }

    /// <summary>
    /// GET /events/transports/{uuid}/used_by/
    /// </summary>
    public async Task<object?> TransportsUsedByListAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_5855f701 = $"api/v3/events/transports/{uuid}/used_by/";
        var response = await client.GetAsync(url_5855f701, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_5855f701 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_5855f701;
    }

}
