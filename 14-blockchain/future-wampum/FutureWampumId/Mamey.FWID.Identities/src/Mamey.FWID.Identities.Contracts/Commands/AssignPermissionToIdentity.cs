using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to assign a permission to an identity.
/// </summary>
[Contract]
public record AssignPermissionToIdentity : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    [Required]
    public Guid PermissionId { get; init; }
}

