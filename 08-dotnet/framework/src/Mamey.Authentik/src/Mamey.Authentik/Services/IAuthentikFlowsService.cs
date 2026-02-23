using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Flows API operations.
/// </summary>
public interface IAuthentikFlowsService
{
    /// <summary>
    /// GET /flows/bindings/
    /// </summary>
    Task<PaginatedResult<object>> BindingsListAsync(bool? evaluate_on_plan = null, string? fsb_uuid = null, string? invalid_response_action = null, int? order = null, string? pbm_uuid = null, string? policies = null, string? policy_engine_mode = null, bool? re_evaluate_policies = null, string? stage = null, string? target = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/bindings/
    /// </summary>
    Task<object?> BindingsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/bindings/{fsb_uuid}/
    /// </summary>
    Task<object?> BindingsRetrieveAsync(string fsb_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /flows/bindings/{fsb_uuid}/
    /// </summary>
    Task<object?> BindingsUpdateAsync(string fsb_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /flows/bindings/{fsb_uuid}/
    /// </summary>
    Task<object?> BindingsPartialUpdateAsync(string fsb_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /flows/bindings/{fsb_uuid}/
    /// </summary>
    Task<object?> BindingsDestroyAsync(string fsb_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/bindings/{fsb_uuid}/used_by/
    /// </summary>
    Task<object?> BindingsUsedByListAsync(string fsb_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/executor/{flow_slug}/
    /// </summary>
    Task<object?> ExecutorGetAsync(string flow_slug, string? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/executor/{flow_slug}/
    /// </summary>
    Task<object?> ExecutorSolveAsync(string flow_slug, object request, string? query = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/inspector/{flow_slug}/
    /// </summary>
    Task<object?> InspectorGetAsync(string flow_slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/
    /// </summary>
    Task<PaginatedResult<object>> InstancesListAsync(string? denied_action = null, string? designation = null, string? flow_uuid = null, string? slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/instances/
    /// </summary>
    Task<object?> InstancesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/{slug}/
    /// </summary>
    Task<object?> InstancesRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /flows/instances/{slug}/
    /// </summary>
    Task<object?> InstancesUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /flows/instances/{slug}/
    /// </summary>
    Task<object?> InstancesPartialUpdateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /flows/instances/{slug}/
    /// </summary>
    Task<object?> InstancesDestroyAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/{slug}/diagram/
    /// </summary>
    Task<object?> InstancesDiagramRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/{slug}/execute/
    /// </summary>
    Task<object?> InstancesExecuteRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/{slug}/export/
    /// </summary>
    Task<object?> InstancesExportRetrieveAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/instances/{slug}/set_background/
    /// </summary>
    Task<object?> InstancesSetBackgroundCreateAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/instances/{slug}/set_background_url/
    /// </summary>
    Task<object?> InstancesSetBackgroundUrlCreateAsync(string slug, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/{slug}/used_by/
    /// </summary>
    Task<object?> InstancesUsedByListAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/instances/cache_clear/
    /// </summary>
    Task<object?> InstancesCacheClearCreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /flows/instances/cache_info/
    /// </summary>
    Task<PaginatedResult<object>> InstancesCacheInfoRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /flows/instances/import/
    /// </summary>
    Task<object?> InstancesImportCreateAsync(CancellationToken cancellationToken = default);

}
