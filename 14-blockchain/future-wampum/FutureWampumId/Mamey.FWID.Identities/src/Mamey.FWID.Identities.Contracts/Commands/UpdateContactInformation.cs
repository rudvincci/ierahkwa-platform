using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to update the contact information of an identity.
/// </summary>
[Contract]
public record UpdateContactInformation : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the new contact information.
    /// </summary>
    [Required]
    public ContactInformationDto ContactInformation { get; init; } = null!;
}

