using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to delete a role.
/// </summary>
[Contract]
public record DeleteRole : ICommand
{
    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    [Required]
    public Guid RoleId { get; init; }
}

