using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to verify biometric data for an identity.
/// </summary>
[Contract]
public record VerifyBiometric : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the provided biometric data for verification.
    /// </summary>
    public BiometricDataDto ProvidedBiometric { get; init; } = null!;

    /// <summary>
    /// Gets or sets the match threshold (default: 0.95).
    /// </summary>
    public double? Threshold { get; init; }
}

