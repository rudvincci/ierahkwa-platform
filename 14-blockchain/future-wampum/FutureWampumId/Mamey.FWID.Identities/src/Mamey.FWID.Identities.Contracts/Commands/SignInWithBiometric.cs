using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to sign in an identity using biometric data.
/// </summary>
[Contract]
public record SignInWithBiometric : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the biometric data.
    /// </summary>
    [Required]
    public BiometricDataDto BiometricData { get; init; } = null!;

    /// <summary>
    /// Gets or sets the IP address of the client.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// Gets or sets the user agent of the client.
    /// </summary>
    public string? UserAgent { get; init; }
}

