using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to add a permission to a role.
/// </summary>
[Contract]
public record AddPermissionToRole : ICommand
{
    /// <summary>
    /// Gets or sets the role identifier.
    /// </summary>
    [Required]
    public Guid RoleId { get; init; }

    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    [Required]
    public Guid PermissionId { get; init; }
}

