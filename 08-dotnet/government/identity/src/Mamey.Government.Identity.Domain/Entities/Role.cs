using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class Role : AggregateRoot<RoleId>
{
    #region Fields
    private ISet<PermissionId> _permissions = new HashSet<PermissionId>();
    #endregion

    public Role(RoleId id, string name, string description, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<PermissionId>? permissions = null, 
        RoleStatus status = RoleStatus.Active, int version = 0)
        : base(id, version)
    {
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Status = status;
        Permissions = permissions ?? Enumerable.Empty<PermissionId>();
    }

    #region Properties

    /// <summary>
    /// Name of the role.
    /// </summary>
    [Description("Name of the role")]
    public string Name { get; private set; }

    /// <summary>
    /// Description of the role.
    /// </summary>
    [Description("Description of the role")]
    public string Description { get; private set; }

    /// <summary>
    /// Current status of the role.
    /// </summary>
    [Description("Current status of the role")]
    public RoleStatus Status { get; private set; }

    /// <summary>
    /// Date and time the role was created.
    /// </summary>
    [Description("Date and time the role was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the role was modified.
    /// </summary>
    [Description("Date and time the role was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Collection of Permission IDs assigned to the role.
    /// </summary>
    [Description("Collection of Permission IDs assigned to the role.")]
    public IEnumerable<PermissionId> Permissions
    {
        get => _permissions;
        private set => _permissions = new HashSet<PermissionId>(value);
    }
    #endregion

    public static Role Create(Guid id, string name, string description, IEnumerable<PermissionId>? permissions = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingRoleNameException();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new MissingRoleDescriptionException();
        }

        ValidatePermissions(permissions);

        var role = new Role(id, name, description, DateTime.UtcNow, permissions: permissions);
        role.AddEvent(new RoleCreated(role));
        return role;
    }

    public void Update(string name, string description, IEnumerable<PermissionId>? permissions = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingRoleNameException();
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new MissingRoleDescriptionException();
        }

        ValidatePermissions(permissions);

        Name = name;
        Description = description;
        ModifiedAt = DateTime.UtcNow;
        Permissions = permissions ?? Permissions;
        
        AddEvent(new RoleModified(this));
    }

    public void Activate()
    {
        if (Status == RoleStatus.Active)
        {
            throw new RoleAlreadyActiveException();
        }

        Status = RoleStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new RoleActivated(this));
    }

    public void Deactivate()
    {
        if (Status == RoleStatus.Inactive)
        {
            throw new RoleAlreadyInactiveException();
        }

        Status = RoleStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new RoleDeactivated(this));
    }

    public void AddPermission(PermissionId permissionId)
    {
        if (_permissions.Contains(permissionId))
        {
            throw new PermissionAlreadyAssignedException();
        }

        _permissions.Add(permissionId);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new PermissionAssignedToRole(this, permissionId));
    }

    public void RemovePermission(PermissionId permissionId)
    {
        if (!_permissions.Contains(permissionId))
        {
            throw new PermissionNotAssignedException();
        }

        _permissions.Remove(permissionId);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new PermissionRemovedFromRole(this, permissionId));
    }

    public bool HasPermission(PermissionId permissionId)
    {
        return _permissions.Contains(permissionId);
    }
    

    private static void ValidatePermissions(IEnumerable<PermissionId>? permissions)
    {
        if (permissions != null && permissions.Any(p => p == null))
        {
            throw new InvalidRolePermissionsException();
        }
    }
}