using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to create a role.
/// </summary>
[Contract]
public record CreateRole : ICommand
{
    /// <summary>
    /// Gets or sets the role name.
    /// </summary>
    [Required]
    public string Name { get; init; } = null!;

    /// <summary>
    /// Gets or sets the role description.
    /// </summary>
    public string? Description { get; init; }
}

