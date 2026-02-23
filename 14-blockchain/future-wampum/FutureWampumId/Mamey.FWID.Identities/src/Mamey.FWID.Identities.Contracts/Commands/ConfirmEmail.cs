using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to confirm an email address.
/// </summary>
[Contract]
public record ConfirmEmail : ICommand
{
    /// <summary>
    /// Gets or sets the confirmation token.
    /// </summary>
    [Required]
    public string Token { get; init; } = null!;
}

