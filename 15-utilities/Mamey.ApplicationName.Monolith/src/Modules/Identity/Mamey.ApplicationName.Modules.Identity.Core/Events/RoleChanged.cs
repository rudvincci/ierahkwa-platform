using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Events;

/// <summary>
/// Published when a role’s name, claims, or metadata change.
/// </summary>
public sealed record RoleChanged(
    RoleId RoleId,
    string ChangedBy,
    DateTime OccurredAt = default)
{
    public RoleChanged(RoleId roleId, string changedBy)
        : this(roleId, changedBy,  DateTime.UtcNow) { }
}