namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a role in the identity system.
/// </summary>
public enum RoleStatus
{
    /// <summary>
    /// Role is active and can be assigned to subjects.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Role is inactive and cannot be assigned to subjects.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Role is deprecated and should not be used for new assignments.
    /// </summary>
    Deprecated = 3
}