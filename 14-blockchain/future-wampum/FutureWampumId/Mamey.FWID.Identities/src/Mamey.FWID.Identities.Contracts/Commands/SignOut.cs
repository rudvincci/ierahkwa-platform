using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to sign out an identity.
/// </summary>
[Contract]
public record SignOut : ICommand
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    [Required]
    public Guid SessionId { get; init; }
}

