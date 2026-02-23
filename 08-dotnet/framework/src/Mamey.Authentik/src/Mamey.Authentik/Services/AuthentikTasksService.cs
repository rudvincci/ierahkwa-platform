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
/// Service implementation for Authentik Tasks API operations.
/// </summary>
public class AuthentikTasksService : IAuthentikTasksService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikTasksService> _logger;
    private readonly IAuthentikCache? _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikTasksService"/> class.
    /// </summary>
    public AuthentikTasksService(
        IHttpClientFactory httpClientFactory,
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikTasksService> logger,
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
    /// GET /tasks/schedules/
    /// </summary>
    public async Task<PaginatedResult<object>> SchedulesListAsync(string? actor_name = null, bool? paused = null, string? rel_obj_content_type__app_label = null, string? rel_obj_content_type__model = null, string? rel_obj_id = null, bool? rel_obj_id__isnull = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_7c7c1f7d = $"api/v3/tasks/schedules/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(actor_name)) queryParams.Add($"actor_name={actor_name}");
        if (paused.HasValue) queryParams.Add($"paused={paused.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(rel_obj_content_type__app_label)) queryParams.Add($"rel_obj_content_type__app_label={rel_obj_content_type__app_label}");
        if (!string.IsNullOrEmpty(rel_obj_content_type__model)) queryParams.Add($"rel_obj_content_type__model={rel_obj_content_type__model}");
        if (!string.IsNullOrEmpty(rel_obj_id)) queryParams.Add($"rel_obj_id={rel_obj_id}");
        if (rel_obj_id__isnull.HasValue) queryParams.Add($"rel_obj_id__isnull={rel_obj_id__isnull.Value.ToString().ToLower()}");
        if (queryParams.Any()) url_7c7c1f7d += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_7c7c1f7d, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_7c7c1f7d = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_7c7c1f7d ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /tasks/schedules/{id}/
    /// </summary>
    public async Task<object?> SchedulesRetrieveAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_35372765 = $"api/v3/tasks/schedules/{id}/";
        var response = await client.GetAsync(url_35372765, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_35372765 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_35372765;
    }

    /// <summary>
    /// PUT /tasks/schedules/{id}/
    /// </summary>
    public async Task<object?> SchedulesUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_d5614f7c = $"api/v3/tasks/schedules/{id}/";
        var response = await client.PutAsJsonAsync(url_d5614f7c, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_d5614f7c = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_d5614f7c;
    }

    /// <summary>
    /// PATCH /tasks/schedules/{id}/
    /// </summary>
    public async Task<object?> SchedulesPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_e0e6146e = $"api/v3/tasks/schedules/{id}/";
        var response = await client.PatchAsJsonAsync(url_e0e6146e, request, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_e0e6146e = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_e0e6146e;
    }

    /// <summary>
    /// POST /tasks/schedules/{id}/send/
    /// </summary>
    public async Task<object?> SchedulesSendCreateAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_901a9666 = $"api/v3/tasks/schedules/{id}/send/";
        var response = await client.PostAsync(url_901a9666, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_901a9666 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_901a9666;
    }

    /// <summary>
    /// GET /tasks/tasks/
    /// </summary>
    public async Task<PaginatedResult<object>> TasksListAsync(string? actor_name = null, string? aggregated_status = null, string? queue_name = null, string? rel_obj_content_type__app_label = null, string? rel_obj_content_type__model = null, string? rel_obj_id = null, bool? rel_obj_id__isnull = null, string? state = null, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_bc4af201 = $"api/v3/tasks/tasks/";
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(actor_name)) queryParams.Add($"actor_name={actor_name}");
        if (!string.IsNullOrEmpty(aggregated_status)) queryParams.Add($"aggregated_status={aggregated_status}");
        if (!string.IsNullOrEmpty(queue_name)) queryParams.Add($"queue_name={queue_name}");
        if (!string.IsNullOrEmpty(rel_obj_content_type__app_label)) queryParams.Add($"rel_obj_content_type__app_label={rel_obj_content_type__app_label}");
        if (!string.IsNullOrEmpty(rel_obj_content_type__model)) queryParams.Add($"rel_obj_content_type__model={rel_obj_content_type__model}");
        if (!string.IsNullOrEmpty(rel_obj_id)) queryParams.Add($"rel_obj_id={rel_obj_id}");
        if (rel_obj_id__isnull.HasValue) queryParams.Add($"rel_obj_id__isnull={rel_obj_id__isnull.Value.ToString().ToLower()}");
        if (!string.IsNullOrEmpty(state)) queryParams.Add($"state={state}");
        if (queryParams.Any()) url_bc4af201 += "?" + string.Join("&", queryParams);
        var response = await client.GetAsync(url_bc4af201, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_bc4af201 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_bc4af201 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /tasks/tasks/{message_id}/
    /// </summary>
    public async Task<object?> TasksRetrieveAsync(string message_id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_669a75ff = $"api/v3/tasks/tasks/{message_id}/";
        var response = await client.GetAsync(url_669a75ff, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_669a75ff = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_669a75ff;
    }

    /// <summary>
    /// POST /tasks/tasks/{message_id}/retry/
    /// </summary>
    public async Task<object?> TasksRetryCreateAsync(string message_id, CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_48647402 = $"api/v3/tasks/tasks/{message_id}/retry/";
        var response = await client.PostAsync(url_48647402, null, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_48647402 = await response.Content.ReadFromJsonAsync<object>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_48647402;
    }

    /// <summary>
    /// GET /tasks/tasks/status/
    /// </summary>
    public async Task<PaginatedResult<object>> TasksStatusRetrieveAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_8cb17d6a = $"api/v3/tasks/tasks/status/";
        var response = await client.GetAsync(url_8cb17d6a, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_8cb17d6a = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_8cb17d6a ?? new PaginatedResult<object> { Results = new List<object>() };
    }

    /// <summary>
    /// GET /tasks/workers
    /// </summary>
    public async Task<PaginatedResult<object>> WorkersListAsync(CancellationToken cancellationToken = default)
    {
        var client = GetHttpClient();
        var url_160edda4 = $"api/v3/tasks/workers";
        var response = await client.GetAsync(url_160edda4, cancellationToken);
        
        // Error handler will throw appropriate AuthentikException if not successful
        
        var result_160edda4 = await response.Content.ReadFromJsonAsync<PaginatedResult<object>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);
        
        return result_160edda4 ?? new PaginatedResult<object> { Results = new List<object>() };
    }

}
