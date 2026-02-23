using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Contracts.Commands;

/// <summary>
/// Command to revoke an identity.
/// </summary>
[Contract]
public record RevokeIdentity : ICommand
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    public Guid IdentityId { get; init; }

    /// <summary>
    /// Gets or sets the reason for revocation.
    /// </summary>
    public string Reason { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the identifier of the user who revoked the identity.
    /// </summary>
    public Guid? RevokedBy { get; init; }
}

