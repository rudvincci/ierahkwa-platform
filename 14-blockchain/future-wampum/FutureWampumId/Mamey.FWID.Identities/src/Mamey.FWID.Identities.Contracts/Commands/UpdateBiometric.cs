using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to update biometric data for an identity.
/// </summary>
[Contract]
public record UpdateBiometric : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the new biometric data.
    /// </summary>
    public BiometricDataDto NewBiometric { get; init; } = null!;

    /// <summary>
    /// Gets or sets the verification biometric data (to verify identity before update).
    /// </summary>
    public BiometricDataDto VerificationBiometric { get; init; } = null!;
}

