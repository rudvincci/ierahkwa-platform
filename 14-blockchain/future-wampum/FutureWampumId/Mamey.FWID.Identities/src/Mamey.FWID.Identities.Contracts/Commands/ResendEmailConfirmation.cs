using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to resend an email confirmation.
/// </summary>
[Contract]
public record ResendEmailConfirmation : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;
}

