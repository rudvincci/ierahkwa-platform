using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to synchronize permissions for a service.
/// </summary>
[Contract]
public record SyncPermissions : ICommand
{
    /// <summary>
    /// Gets or sets the service name (e.g., "dids-service").
    /// </summary>
    public string ServiceName { get; init; } = null!;

    /// <summary>
    /// Gets or sets the list of permissions required by this service.
    /// </summary>
    public List<string> Permissions { get; init; } = new();

    /// <summary>
    /// Gets or sets the certificate subject (CN) for this service.
    /// </summary>
    public string? CertificateSubject { get; init; }

    /// <summary>
    /// Gets or sets the certificate issuer for this service.
    /// </summary>
    public string? CertificateIssuer { get; init; }

    /// <summary>
    /// Gets or sets the certificate thumbprint for this service.
    /// </summary>
    public string? CertificateThumbprint { get; init; }
}

