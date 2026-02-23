using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to update a permission.
/// </summary>
[Contract]
public record UpdatePermission : ICommand
{
    /// <summary>
    /// Gets or sets the permission identifier.
    /// </summary>
    [Required]
    public Guid PermissionId { get; init; }

    /// <summary>
    /// Gets or sets the permission name.
    /// </summary>
    [Required]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the permission description.
    /// </summary>
    public string? Description { get; init; }
}

