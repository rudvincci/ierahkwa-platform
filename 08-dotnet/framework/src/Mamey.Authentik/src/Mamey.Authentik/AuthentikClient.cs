using Mamey.Authentik.Services;

namespace Mamey.Authentik;

/// <summary>
/// Main client interface for accessing Authentik services.
/// </summary>
public interface IAuthentikClient
{
    /// <summary>
    /// Gets the Admin service for managing users, groups, applications, and providers.
    /// </summary>
    IAuthentikAdminService Admin { get; }

    /// <summary>
    /// Gets the Core service for core Authentik functionality.
    /// </summary>
    IAuthentikCoreService Core { get; }

    /// <summary>
    /// Gets the OAuth2 service for OAuth2 provider management.
    /// </summary>
    IAuthentikOAuth2Service OAuth2 { get; }

    /// <summary>
    /// Gets the Flows service for flow management.
    /// </summary>
    IAuthentikFlowsService Flows { get; }

    /// <summary>
    /// Gets the Policies service for policy management.
    /// </summary>
    IAuthentikPoliciesService Policies { get; }

    /// <summary>
    /// Gets the Providers service for provider management.
    /// </summary>
    IAuthentikProvidersService Providers { get; }

    /// <summary>
    /// Gets the Stages service for stage management.
    /// </summary>
    IAuthentikStagesService Stages { get; }

    /// <summary>
    /// Gets the Sources service for source management.
    /// </summary>
    IAuthentikSourcesService Sources { get; }

    /// <summary>
    /// Gets the Events service for event management.
    /// </summary>
    IAuthentikEventsService Events { get; }

    /// <summary>
    /// Gets the Authenticators service for MFA device management.
    /// </summary>
    IAuthentikAuthenticatorsService Authenticators { get; }

    /// <summary>
    /// Gets the Crypto service for cryptographic operations.
    /// </summary>
    IAuthentikCryptoService Crypto { get; }

    /// <summary>
    /// Gets the Property Mappings service for property mapping management.
    /// </summary>
    IAuthentikPropertyMappingsService PropertyMappings { get; }

    /// <summary>
    /// Gets the RAC service for remote access control.
    /// </summary>
    IAuthentikRacService Rac { get; }

    /// <summary>
    /// Gets the RBAC service for role-based access control.
    /// </summary>
    IAuthentikRbacService Rbac { get; }

    /// <summary>
    /// Gets the Tenants service for tenant management.
    /// </summary>
    IAuthentikTenantsService Tenants { get; }

    /// <summary>
    /// Gets the Tasks service for background task management.
    /// </summary>
    IAuthentikTasksService Tasks { get; }

    /// <summary>
    /// Gets the Outposts service for outpost management.
    /// </summary>
    IAuthentikOutpostsService Outposts { get; }

    /// <summary>
    /// Gets the Endpoints service for endpoint management.
    /// </summary>
    IAuthentikEndpointsService Endpoints { get; }

    /// <summary>
    /// Gets the Enterprise service for enterprise features.
    /// </summary>
    /// <remarks>
    /// Enterprise features may require a license.
    /// </remarks>
    IAuthentikEnterpriseService Enterprise { get; }

    /// <summary>
    /// Gets the Managed service for managed configurations.
    /// </summary>
    IAuthentikManagedService Managed { get; }

    /// <summary>
    /// Gets the Reports service for reporting functionality.
    /// </summary>
    IAuthentikReportsService Reports { get; }

    /// <summary>
    /// Gets the Root service for root API operations.
    /// </summary>
    IAuthentikRootService Root { get; }

    /// <summary>
    /// Gets the SSF service for Single Sign-On Federation.
    /// </summary>
    IAuthentikSsfService Ssf { get; }
}

/// <summary>
/// Main client implementation for accessing Authentik services.
/// </summary>
public class AuthentikClient : IAuthentikClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikClient"/> class.
    /// </summary>
    public AuthentikClient(
        IAuthentikAdminService admin,
        IAuthentikCoreService core,
        IAuthentikOAuth2Service oauth2,
        IAuthentikFlowsService flows,
        IAuthentikPoliciesService policies,
        IAuthentikProvidersService providers,
        IAuthentikStagesService stages,
        IAuthentikSourcesService sources,
        IAuthentikEventsService events,
        IAuthentikAuthenticatorsService authenticators,
        IAuthentikCryptoService crypto,
        IAuthentikPropertyMappingsService propertyMappings,
        IAuthentikRacService rac,
        IAuthentikRbacService rbac,
        IAuthentikTenantsService tenants,
        IAuthentikTasksService tasks,
        IAuthentikOutpostsService outposts,
        IAuthentikEndpointsService endpoints,
        IAuthentikEnterpriseService enterprise,
        IAuthentikManagedService managed,
        IAuthentikReportsService reports,
        IAuthentikRootService root,
        IAuthentikSsfService ssf)
    {
        Admin = admin;
        Core = core;
        OAuth2 = oauth2;
        Flows = flows;
        Policies = policies;
        Providers = providers;
        Stages = stages;
        Sources = sources;
        Events = events;
        Authenticators = authenticators;
        Crypto = crypto;
        PropertyMappings = propertyMappings;
        Rac = rac;
        Rbac = rbac;
        Tenants = tenants;
        Tasks = tasks;
        Outposts = outposts;
        Endpoints = endpoints;
        Enterprise = enterprise;
        Managed = managed;
        Reports = reports;
        Root = root;
        Ssf = ssf;
    }

    /// <inheritdoc />
    public IAuthentikAdminService Admin { get; }

    /// <inheritdoc />
    public IAuthentikCoreService Core { get; }

    /// <inheritdoc />
    public IAuthentikOAuth2Service OAuth2 { get; }

    /// <inheritdoc />
    public IAuthentikFlowsService Flows { get; }

    /// <inheritdoc />
    public IAuthentikPoliciesService Policies { get; }

    /// <inheritdoc />
    public IAuthentikProvidersService Providers { get; }

    /// <inheritdoc />
    public IAuthentikStagesService Stages { get; }

    /// <inheritdoc />
    public IAuthentikSourcesService Sources { get; }

    /// <inheritdoc />
    public IAuthentikEventsService Events { get; }

    /// <inheritdoc />
    public IAuthentikAuthenticatorsService Authenticators { get; }

    /// <inheritdoc />
    public IAuthentikCryptoService Crypto { get; }

    /// <inheritdoc />
    public IAuthentikPropertyMappingsService PropertyMappings { get; }

    /// <inheritdoc />
    public IAuthentikRacService Rac { get; }

    /// <inheritdoc />
    public IAuthentikRbacService Rbac { get; }

    /// <inheritdoc />
    public IAuthentikTenantsService Tenants { get; }

    /// <inheritdoc />
    public IAuthentikTasksService Tasks { get; }

    /// <inheritdoc />
    public IAuthentikOutpostsService Outposts { get; }

    /// <inheritdoc />
    public IAuthentikEndpointsService Endpoints { get; }

    /// <inheritdoc />
    public IAuthentikEnterpriseService Enterprise { get; }

    /// <inheritdoc />
    public IAuthentikManagedService Managed { get; }

    /// <inheritdoc />
    public IAuthentikReportsService Reports { get; }

    /// <inheritdoc />
    public IAuthentikRootService Root { get; }

    /// <inheritdoc />
    public IAuthentikSsfService Ssf { get; }
}
