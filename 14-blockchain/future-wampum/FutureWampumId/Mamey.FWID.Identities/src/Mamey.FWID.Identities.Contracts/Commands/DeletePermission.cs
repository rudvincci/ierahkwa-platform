using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to delete a permission.
/// </summary>
[Contract]
public record DeletePermission : ICommand
{
    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    [Required]
    public Guid PermissionId { get; init; }
}

