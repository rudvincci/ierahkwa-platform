using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Tasks API operations.
/// </summary>
public interface IAuthentikTasksService
{
    /// <summary>
    /// GET /tasks/schedules/
    /// </summary>
    Task<PaginatedResult<object>> SchedulesListAsync(string? actor_name = null, bool? paused = null, string? rel_obj_content_type__app_label = null, string? rel_obj_content_type__model = null, string? rel_obj_id = null, bool? rel_obj_id__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /tasks/schedules/{id}/
    /// </summary>
    Task<object?> SchedulesRetrieveAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /tasks/schedules/{id}/
    /// </summary>
    Task<object?> SchedulesUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /tasks/schedules/{id}/
    /// </summary>
    Task<object?> SchedulesPartialUpdateAsync(string id, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /tasks/schedules/{id}/send/
    /// </summary>
    Task<object?> SchedulesSendCreateAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /tasks/tasks/
    /// </summary>
    Task<PaginatedResult<object>> TasksListAsync(string? actor_name = null, string? aggregated_status = null, string? queue_name = null, string? rel_obj_content_type__app_label = null, string? rel_obj_content_type__model = null, string? rel_obj_id = null, bool? rel_obj_id__isnull = null, string? state = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /tasks/tasks/{message_id}/
    /// </summary>
    Task<object?> TasksRetrieveAsync(string message_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /tasks/tasks/{message_id}/retry/
    /// </summary>
    Task<object?> TasksRetryCreateAsync(string message_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /tasks/tasks/status/
    /// </summary>
    Task<PaginatedResult<object>> TasksStatusRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /tasks/workers
    /// </summary>
    Task<PaginatedResult<object>> WorkersListAsync(CancellationToken cancellationToken = default);

}
