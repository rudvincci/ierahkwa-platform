namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a permission in the identity system.
/// </summary>
public enum PermissionStatus
{
    /// <summary>
    /// Permission is active and can be assigned to roles.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Permission is inactive and cannot be assigned to roles.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Permission is deprecated and should not be used for new assignments.
    /// </summary>
    Deprecated = 3
}