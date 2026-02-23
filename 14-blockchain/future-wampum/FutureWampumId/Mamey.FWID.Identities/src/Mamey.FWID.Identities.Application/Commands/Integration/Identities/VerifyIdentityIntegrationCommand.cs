using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Identities.Application.Commands.Integration.Identities;

/// <summary>
/// Integration command to verify an identity from another service.
/// </summary>
[Message("identities")]
internal record VerifyIdentityIntegrationCommand : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the provided biometric data for verification.
    /// </summary>
    public BiometricData ProvidedBiometric { get; init; } = null!;

    /// <summary>
    /// Gets or sets the match threshold (default: 0.95).
    /// </summary>
    public double? Threshold { get; init; }
}


