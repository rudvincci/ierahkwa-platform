using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a role aggregate root.
/// </summary>
public class Role : AggregateRoot<RoleId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Role()
    {
        Permissions = new List<PermissionId>();
    }

    /// <summary>
    /// Initializes a new instance of the Role aggregate root.
    /// </summary>
    /// <param name="id">The role identifier.</param>
    /// <param name="name">The role name.</param>
    /// <param name="description">The role description.</param>
    [JsonConstructor]
    public Role(
        RoleId id,
        string name,
        string? description = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));
        
        Name = name;
        Description = description;
        Status = RoleStatus.Active;
        Permissions = new List<PermissionId>();
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new RoleCreated(Id, Name, CreatedAt));
    }

    /// <summary>
    /// The role name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// The role description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The status of the role.
    /// </summary>
    public RoleStatus Status { get; private set; }

    /// <summary>
    /// The permissions associated with this role.
    /// </summary>
    public List<PermissionId> Permissions { get; private set; } = new();

    /// <summary>
    /// Date and time the role was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the role was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Updates the role name and description.
    /// </summary>
    /// <param name="name">The new role name.</param>
    /// <param name="description">The new role description.</param>
    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));
        
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new RoleUpdated(Id, Name, UpdatedAt.Value));
    }

    /// <summary>
    /// Adds a permission to the role.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    public void AddPermission(PermissionId permissionId)
    {
        if (permissionId == null || permissionId.IsEmpty)
            throw new ArgumentException("Permission ID cannot be null or empty", nameof(permissionId));
        
        if (Permissions.Contains(permissionId))
            return; // Already added
        
        Permissions.Add(permissionId);
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionAddedToRole(Id, permissionId, UpdatedAt.Value));
    }

    /// <summary>
    /// Removes a permission from the role.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    public void RemovePermission(PermissionId permissionId)
    {
        if (permissionId == null || permissionId.IsEmpty)
            throw new ArgumentException("Permission ID cannot be null or empty", nameof(permissionId));
        
        if (!Permissions.Contains(permissionId))
            return; // Not in list
        
        Permissions.Remove(permissionId);
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionRemovedFromRole(Id, permissionId, UpdatedAt.Value));
    }

    /// <summary>
    /// Activates the role.
    /// </summary>
    public void Activate()
    {
        if (Status == RoleStatus.Active)
            return; // Already active
        
        Status = RoleStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new RoleActivated(Id, UpdatedAt.Value));
    }

    /// <summary>
    /// Deactivates the role.
    /// </summary>
    public void Deactivate()
    {
        if (Status == RoleStatus.Inactive)
            return; // Already inactive
        
        Status = RoleStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new RoleDeactivated(Id, UpdatedAt.Value));
    }
}

