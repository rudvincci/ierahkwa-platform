using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class Permission : AggregateRoot<PermissionId>
{
    public Permission(PermissionId id, string name, string description, string resource, 
        string action, DateTime createdAt, DateTime? modifiedAt = null, 
        PermissionStatus status = PermissionStatus.Active, int version = 0)
        : base(id, version)
    {
        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Status = status;
    }

    #region Properties

    /// <summary>
    /// Name of the permission.
    /// </summary>
    [Description("Name of the permission")]
    public string Name { get; private set; }

    /// <summary>
    /// Description of the permission.
    /// </summary>
    [Description("Description of the permission")]
    public string Description { get; private set; }

    /// <summary>
    /// Resource that this permission applies to.
    /// </summary>
    [Description("Resource that this permission applies to")]
    public string Resource { get; private set; }

    /// <summary>
    /// Action that this permission allows.
    /// </summary>
    [Description("Action that this permission allows")]
    public string Action { get; private set; }

    /// <summary>
    /// Current status of the permission.
    /// </summary>
    [Description("Current status of the permission")]
    public PermissionStatus Status { get; private set; }

    /// <summary>
    /// Date and time the permission was created.
    /// </summary>
    [Description("Date and time the permission was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the permission was modified.
    /// </summary>
    [Description("Date and time the permission was modified.")]
    public DateTime? ModifiedAt { get; private set; }
    #endregion

    public static Permission Create(Guid id, string name, string description, string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingPermissionNameException();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new MissingPermissionDescriptionException();
        }

        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new MissingPermissionResourceException();
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new MissingPermissionActionException();
        }

        var permission = new Permission(id, name, description, resource, action, DateTime.UtcNow);
        permission.AddEvent(new PermissionCreated(permission));
        return permission;
    }

    public void Update(string name, string description, string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingPermissionNameException();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new MissingPermissionDescriptionException();
        }

        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new MissingPermissionResourceException();
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new MissingPermissionActionException();
        }

        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
        ModifiedAt = DateTime.UtcNow;
        
        AddEvent(new PermissionModified(this));
    }

    public void Activate()
    {
        if (Status == PermissionStatus.Active)
        {
            throw new PermissionAlreadyActiveException();
        }

        Status = PermissionStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new PermissionActivated(this));
    }

    public void Deactivate()
    {
        if (Status == PermissionStatus.Inactive)
        {
            throw new PermissionAlreadyInactiveException();
        }

        Status = PermissionStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new PermissionDeactivated(this));
    }

    public bool Matches(string resource, string action)
    {
        return Resource.Equals(resource, StringComparison.OrdinalIgnoreCase) &&
               Action.Equals(action, StringComparison.OrdinalIgnoreCase);
    }
}