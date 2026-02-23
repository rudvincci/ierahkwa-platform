using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a permission aggregate root.
/// </summary>
public class Permission : AggregateRoot<PermissionId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Permission()
    {
    }

    /// <summary>
    /// Initializes a new instance of the Permission aggregate root.
    /// </summary>
    /// <param name="id">The permission identifier.</param>
    /// <param name="name">The permission name.</param>
    /// <param name="description">The permission description.</param>
    [JsonConstructor]
    public Permission(
        PermissionId id,
        string name,
        string? description = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be null or empty", nameof(name));
        
        Name = name;
        Description = description;
        Status = PermissionStatus.Active;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionCreated(Id, Name, CreatedAt));
    }

    /// <summary>
    /// The permission name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// The permission description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The status of the permission.
    /// </summary>
    public PermissionStatus Status { get; private set; }

    /// <summary>
    /// Date and time the permission was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the permission was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Updates the permission name and description.
    /// </summary>
    /// <param name="name">The new permission name.</param>
    /// <param name="description">The new permission description.</param>
    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be null or empty", nameof(name));
        
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionUpdated(Id, Name, UpdatedAt.Value));
    }

    /// <summary>
    /// Activates the permission.
    /// </summary>
    public void Activate()
    {
        if (Status == PermissionStatus.Active)
            return; // Already active
        
        Status = PermissionStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionActivated(Id, UpdatedAt.Value));
    }

    /// <summary>
    /// Deactivates the permission.
    /// </summary>
    public void Deactivate()
    {
        if (Status == PermissionStatus.Inactive)
            return; // Already inactive
        
        Status = PermissionStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionDeactivated(Id, UpdatedAt.Value));
    }
}

