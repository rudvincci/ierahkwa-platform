using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to enable multi-factor authentication.
/// </summary>
[Contract]
public record EnableMfa : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the MFA method.
    /// </summary>
    [Required]
    public MfaMethod Method { get; init; }

    /// <summary>
    /// Gets or sets the verification code.
    /// </summary>
    [Required]
    public string VerificationCode { get; init; } = null!;
}

