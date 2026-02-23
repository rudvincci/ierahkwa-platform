using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to refresh an access token.
/// </summary>
[Contract]
public record RefreshToken : ICommand
{
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [Required]
    public string Token { get; init; } = null!;
}

