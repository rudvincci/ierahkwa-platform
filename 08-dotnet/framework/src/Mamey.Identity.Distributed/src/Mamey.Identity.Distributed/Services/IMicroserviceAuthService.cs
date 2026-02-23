using Mamey.Identity.Core;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for microservice-to-microservice authentication.
/// </summary>
public interface IMicroserviceAuthService
{
    /// <summary>
    /// Authenticates a microservice request.
    /// </summary>
    /// <param name="serviceId">The requesting service ID.</param>
    /// <param name="serviceSecret">The service secret.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if authentication is successful.</returns>
    Task<bool> AuthenticateServiceAsync(string serviceId, string serviceSecret, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a service-to-service token.
    /// </summary>
    /// <param name="fromServiceId">The source service ID.</param>
    /// <param name="toServiceId">The target service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The service token.</returns>
    Task<string> CreateServiceTokenAsync(string fromServiceId, string toServiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a service-to-service token.
    /// </summary>
    /// <param name="token">The service token.</param>
    /// <param name="expectedServiceId">The expected service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid.</returns>
    Task<bool> ValidateServiceTokenAsync(string token, string expectedServiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the service information from a service token.
    /// </summary>
    /// <param name="token">The service token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The service information.</returns>
    Task<ServiceInfo?> GetServiceInfoAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a new microservice.
    /// </summary>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="serviceSecret">The service secret.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if registration is successful.</returns>
    Task<bool> RegisterServiceAsync(string serviceId, string serviceSecret, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unregisters a microservice.
    /// </summary>
    /// <param name="serviceId">The service ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if unregistration is successful.</returns>
    Task<bool> UnregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about a microservice.
/// </summary>
public class ServiceInfo
{
    /// <summary>
    /// Gets or sets the service ID.
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service name.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service version.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service endpoint.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service capabilities.
    /// </summary>
    public List<string> Capabilities { get; set; } = new();

    /// <summary>
    /// Gets or sets the service registration time.
    /// </summary>
    public DateTime RegisteredAt { get; set; }

    /// <summary>
    /// Gets or sets the last heartbeat time.
    /// </summary>
    public DateTime LastHeartbeat { get; set; }
}


































