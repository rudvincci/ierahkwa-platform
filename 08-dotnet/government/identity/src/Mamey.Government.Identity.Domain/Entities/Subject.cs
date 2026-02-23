using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class Subject : AggregateRoot<SubjectId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    private ISet<RoleId> _roles = new HashSet<RoleId>();
    #endregion


    public Subject(SubjectId id, string name, Email email, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null, 
        IEnumerable<RoleId>? roles = null, SubjectStatus status = SubjectStatus.Active, int version = 0)
        : base(id, version)
    {
        Name = name;
        Email = email;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Status = status;
        Tags = tags ?? Enumerable.Empty<string>();
        Roles = roles ?? Enumerable.Empty<RoleId>();
    }

    #region Properties

    /// <summary>
    /// A name for the subject.
    /// </summary>
    [Description("The subject's name")]
    public string Name { get; private set; }

    /// <summary>
    /// Email address for the subject.
    /// </summary>
    [Description("The subject's email address")]
    public Email Email { get; private set; }

    /// <summary>
    /// Current status of the subject.
    /// </summary>
    [Description("Current status of the subject")]
    public SubjectStatus Status { get; private set; }

    /// <summary>
    /// Date and time the record was created.
    /// </summary>
    [Description("Date and time the record was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the record was modified.
    /// </summary>
    [Description("Date and time the record was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the subject was last authenticated.
    /// </summary>
    [Description("Date and time the subject was last authenticated.")]
    public DateTime? LastAuthenticatedAt { get; private set; }

    /// <summary>
    /// Collection of Subject tags.
    /// </summary>
    [Description("Collection of Subject tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }

    /// <summary>
    /// Collection of Role IDs assigned to the subject.
    /// </summary>
    [Description("Collection of Role IDs assigned to the subject.")]
    public IEnumerable<RoleId> Roles
    {
        get => _roles;
        private set => _roles = new HashSet<RoleId>(value);
    }
    #endregion

    public static Subject Create(Guid id, string name, string email, IEnumerable<string>? tags = null, IEnumerable<RoleId>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingSubjectNameException();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new MissingSubjectEmailException();
        }

        ValidateTags(tags);
        ValidateRoles(roles);

        var emailObj = new Email(email);
        var subject = new Subject(id, name, emailObj, DateTime.UtcNow, tags: tags, roles: roles);
        subject.AddEvent(new SubjectCreated(subject));
        return subject;
    }

    public void Update(string name, string email, IEnumerable<string>? tags = null, IEnumerable<RoleId>? roles = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingSubjectNameException();
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new MissingSubjectEmailException();
        }

        ValidateTags(tags);
        ValidateRoles(roles);

        Name = name;
        Email = new Email(email);
        ModifiedAt = DateTime.UtcNow;
        Tags = tags ?? Tags;
        Roles = roles ?? Roles;
        
        AddEvent(new SubjectModified(this));
    }

    public void Activate()
    {
        if (Status == SubjectStatus.Active)
        {
            throw new SubjectAlreadyActiveException();
        }

        Status = SubjectStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new SubjectActivated(this));
    }

    public void Deactivate()
    {
        if (Status == SubjectStatus.Inactive)
        {
            throw new SubjectAlreadyInactiveException();
        }

        Status = SubjectStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new SubjectDeactivated(this));
    }

    public void Suspend(string reason)
    {
        if (Status == SubjectStatus.Suspended)
        {
            throw new SubjectAlreadySuspendedException();
        }

        Status = SubjectStatus.Suspended;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new SubjectSuspended(this, reason));
    }

    public void RecordAuthentication()
    {
        LastAuthenticatedAt = DateTime.UtcNow;
        AddEvent(new SubjectAuthenticated(this));
    }

    public void AddRole(RoleId roleId)
    {
        if (_roles.Contains(roleId))
        {
            throw new RoleAlreadyAssignedException();
        }

        _roles.Add(roleId);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new RoleAssignedToSubject(this, roleId));
    }

    public void RemoveRole(RoleId roleId)
    {
        if (!_roles.Contains(roleId))
        {
            throw new RoleNotAssignedException();
        }

        _roles.Remove(roleId);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new RoleRemovedFromSubject(this, roleId));
    }

    public bool HasRole(RoleId roleId)
    {
        return _roles.Contains(roleId);
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            throw new InvalidSubjectTagsException();
        }

        if (_tags.Contains(tag))
        {
            return; // Tag already exists, no need to add
        }

        _tags.Add(tag);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new TagAddedToSubject(this, tag));
    }

    public void RemoveTag(string tag)
    {
        if (!_tags.Contains(tag))
        {
            return; // Tag doesn't exist, no need to remove
        }

        _tags.Remove(tag);
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new TagRemovedFromSubject(this, tag));
    }

    private static void ValidateTags(IEnumerable<string>? tags)
    {
        if (tags != null && tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidSubjectTagsException();
        }
    }

    private static void ValidateRoles(IEnumerable<RoleId>? roles)
    {
        if (roles != null && roles.Any(r => r == null))
        {
            throw new InvalidSubjectRolesException();
        }
    }

}