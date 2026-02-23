using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Events API operations.
/// </summary>
public interface IAuthentikEventsService
{
    /// <summary>
    /// GET /events/events/
    /// </summary>
    Task<PaginatedResult<object>> EventsListAsync(string? action = null, string? actions = null, string? brand_name = null, string? client_ip = null, string? context_authorized_app = null, string? context_model_app = null, string? context_model_name = null, string? context_model_pk = null, string? username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /events/events/
    /// </summary>
    Task<object?> EventsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/events/{event_uuid}/
    /// </summary>
    Task<object?> EventsRetrieveAsync(string event_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /events/events/{event_uuid}/
    /// </summary>
    Task<object?> EventsUpdateAsync(string event_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /events/events/{event_uuid}/
    /// </summary>
    Task<object?> EventsPartialUpdateAsync(string event_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /events/events/{event_uuid}/
    /// </summary>
    Task<object?> EventsDestroyAsync(string event_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/events/actions/
    /// </summary>
    Task<PaginatedResult<object>> EventsActionsListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/events/top_per_user/
    /// </summary>
    Task<PaginatedResult<object>> EventsTopPerUserListAsync(string? action = null, int? top_n = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/events/volume/
    /// </summary>
    Task<PaginatedResult<object>> EventsVolumeListAsync(string? action = null, string? actions = null, string? brand_name = null, string? client_ip = null, string? context_authorized_app = null, string? context_model_app = null, string? context_model_name = null, string? context_model_pk = null, double? history_days = null, string? username = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/notifications/
    /// </summary>
    Task<PaginatedResult<object>> NotificationsListAsync(string? body = null, string? created = null, string? @event = null, bool? seen = null, string? severity = null, int? user = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/notifications/{uuid}/
    /// </summary>
    Task<object?> NotificationsRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /events/notifications/{uuid}/
    /// </summary>
    Task<object?> NotificationsUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /events/notifications/{uuid}/
    /// </summary>
    Task<object?> NotificationsPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /events/notifications/{uuid}/
    /// </summary>
    Task<object?> NotificationsDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/notifications/{uuid}/used_by/
    /// </summary>
    Task<object?> NotificationsUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /events/notifications/mark_all_seen/
    /// </summary>
    Task<object?> NotificationsMarkAllSeenCreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/rules/
    /// </summary>
    Task<PaginatedResult<object>> RulesListAsync(string? destination_group__name = null, string? severity = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /events/rules/
    /// </summary>
    Task<object?> RulesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/rules/{pbm_uuid}/
    /// </summary>
    Task<object?> RulesRetrieveAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /events/rules/{pbm_uuid}/
    /// </summary>
    Task<object?> RulesUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /events/rules/{pbm_uuid}/
    /// </summary>
    Task<object?> RulesPartialUpdateAsync(string pbm_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /events/rules/{pbm_uuid}/
    /// </summary>
    Task<object?> RulesDestroyAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/rules/{pbm_uuid}/used_by/
    /// </summary>
    Task<object?> RulesUsedByListAsync(string pbm_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/transports/
    /// </summary>
    Task<PaginatedResult<object>> TransportsListAsync(string? mode = null, bool? send_once = null, string? webhook_url = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /events/transports/
    /// </summary>
    Task<object?> TransportsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/transports/{uuid}/
    /// </summary>
    Task<object?> TransportsRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /events/transports/{uuid}/
    /// </summary>
    Task<object?> TransportsUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /events/transports/{uuid}/
    /// </summary>
    Task<object?> TransportsPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /events/transports/{uuid}/
    /// </summary>
    Task<object?> TransportsDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /events/transports/{uuid}/test/
    /// </summary>
    Task<object?> TransportsTestCreateAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /events/transports/{uuid}/used_by/
    /// </summary>
    Task<object?> TransportsUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

}
