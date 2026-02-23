using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to resend an SMS confirmation.
/// </summary>
[Contract]
public record ResendSmsConfirmation : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    [Required]
    public string PhoneNumber { get; init; } = null!;
}

