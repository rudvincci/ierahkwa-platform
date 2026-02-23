using System.ComponentModel.DataAnnotations;
using Mamey.CQRS.Queries;

namespace Mamey.FWID.Identities.Contracts.Queries;

/// <summary>
/// Query to get all permissions for an identity.
/// </summary>
public record GetIdentityPermissions : IQuery<List<Guid>>
{
    /// <summary>
    /// Gets or sets the identity identifier.
    /// </summary>
    [Required]
    public Guid IdentityId { get; init; }
}

