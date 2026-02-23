using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to verify a backup code.
/// </summary>
[Contract]
public record VerifyBackupCode : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the backup code.
    /// </summary>
    [Required]
    public string Code { get; init; } = null!;
}

