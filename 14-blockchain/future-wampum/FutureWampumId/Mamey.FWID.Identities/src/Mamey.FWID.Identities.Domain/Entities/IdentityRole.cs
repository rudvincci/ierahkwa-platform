using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents the relationship between an Identity and a Role.
/// </summary>
public class IdentityRole : AggregateRoot<Guid>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private IdentityRole()
    {
    }

    /// <summary>
    /// Initializes a new instance of the IdentityRole aggregate root.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    [JsonConstructor]
    public IdentityRole(IdentityId identityId, RoleId roleId)
        : base(Guid.NewGuid())
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        RoleId = roleId ?? throw new ArgumentNullException(nameof(roleId));
        AssignedAt = DateTime.UtcNow;
        
        AddEvent(new RoleAssigned(IdentityId, RoleId.Value, AssignedAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The role identifier.
    /// </summary>
    public RoleId RoleId { get; private set; } = null!;

    /// <summary>
    /// Date and time the role was assigned.
    /// </summary>
    public DateTime AssignedAt { get; private set; }
}

