using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to verify an MFA challenge.
/// </summary>
[Contract]
public record VerifyMfaChallenge : ICommand
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
    public string Code { get; init; } = null!;
}

