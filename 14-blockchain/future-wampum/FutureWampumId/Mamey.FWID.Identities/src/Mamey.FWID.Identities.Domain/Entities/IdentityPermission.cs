using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents the relationship between an Identity and a Permission.
/// </summary>
public class IdentityPermission : AggregateRoot<Guid>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private IdentityPermission()
    {
    }

    /// <summary>
    /// Initializes a new instance of the IdentityPermission aggregate root.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    [JsonConstructor]
    public IdentityPermission(IdentityId identityId, PermissionId permissionId)
        : base(Guid.NewGuid())
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        PermissionId = permissionId ?? throw new ArgumentNullException(nameof(permissionId));
        AssignedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionAssigned(IdentityId, PermissionId.Value, AssignedAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The permission identifier.
    /// </summary>
    public PermissionId PermissionId { get; private set; } = null!;

    /// <summary>
    /// Date and time the permission was assigned.
    /// </summary>
    public DateTime AssignedAt { get; private set; }
}

