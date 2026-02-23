using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Outposts API operations.
/// </summary>
public interface IAuthentikOutpostsService
{
    /// <summary>
    /// GET /outposts/instances/
    /// </summary>
    Task<PaginatedResult<object>> InstancesListAsync(string? managed__icontains = null, string? managed__iexact = null, string? name__icontains = null, string? name__iexact = null, bool? providers__isnull = null, string? providers_by_pk = null, string? service_connection__name__icontains = null, string? service_connection__name__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /outposts/instances/
    /// </summary>
    Task<object?> InstancesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/instances/{uuid}/
    /// </summary>
    Task<object?> InstancesRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /outposts/instances/{uuid}/
    /// </summary>
    Task<object?> InstancesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /outposts/instances/{uuid}/
    /// </summary>
    Task<object?> InstancesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /outposts/instances/{uuid}/
    /// </summary>
    Task<object?> InstancesDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/instances/{uuid}/health/
    /// </summary>
    Task<object?> InstancesHealthListAsync(string uuid, string? managed__icontains = null, string? managed__iexact = null, string? name__icontains = null, string? name__iexact = null, bool? providers__isnull = null, string? providers_by_pk = null, string? service_connection__name__icontains = null, string? service_connection__name__iexact = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/instances/{uuid}/used_by/
    /// </summary>
    Task<object?> InstancesUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/instances/default_settings/
    /// </summary>
    Task<PaginatedResult<object>> InstancesDefaultSettingsRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/ldap/
    /// </summary>
    Task<PaginatedResult<object>> LdapListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/ldap/{id}/check_access/
    /// </summary>
    Task<object?> LdapAccessCheckAsync(int id, string? app_slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/proxy/
    /// </summary>
    Task<PaginatedResult<object>> ProxyListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/radius/
    /// </summary>
    Task<PaginatedResult<object>> RadiusListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/radius/{id}/check_access/
    /// </summary>
    Task<object?> RadiusAccessCheckAsync(int id, string? app_slug = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/all/
    /// </summary>
    Task<PaginatedResult<object>> ServiceConnectionsAllListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsAllRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /outposts/service_connections/all/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsAllDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/state/
    /// </summary>
    Task<object?> ServiceConnectionsAllStateRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/all/{uuid}/used_by/
    /// </summary>
    Task<object?> ServiceConnectionsAllUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/all/types/
    /// </summary>
    Task<PaginatedResult<object>> ServiceConnectionsAllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/docker/
    /// </summary>
    Task<PaginatedResult<object>> ServiceConnectionsDockerListAsync(bool? local = null, string? tls_authentication = null, string? tls_verification = null, string? url = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /outposts/service_connections/docker/
    /// </summary>
    Task<object?> ServiceConnectionsDockerCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/docker/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsDockerRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /outposts/service_connections/docker/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsDockerUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /outposts/service_connections/docker/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsDockerPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /outposts/service_connections/docker/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsDockerDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/docker/{uuid}/used_by/
    /// </summary>
    Task<object?> ServiceConnectionsDockerUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/
    /// </summary>
    Task<PaginatedResult<object>> ServiceConnectionsKubernetesListAsync(bool? local = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /outposts/service_connections/kubernetes/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesRetrieveAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesPartialUpdateAsync(string uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /outposts/service_connections/kubernetes/{uuid}/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesDestroyAsync(string uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /outposts/service_connections/kubernetes/{uuid}/used_by/
    /// </summary>
    Task<object?> ServiceConnectionsKubernetesUsedByListAsync(string uuid, CancellationToken cancellationToken = default);

}
