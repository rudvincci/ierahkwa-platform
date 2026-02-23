using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Accessibility.Domain.Events;
using Pupitre.Accessibility.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.Unit.Core.Entities")]
namespace Pupitre.Accessibility.Domain.Entities;


internal class AccessProfile : AggregateRoot<AccessProfileId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public AccessProfile(AccessProfileId id, string name, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null, int version = 0)
        : base(id, version)
    {
        Name = name;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Tags = tags ?? Enumerable.Empty<string>();
    }

    #region Properties

    /// <summary>
    /// A name for the accessprofile.
    /// </summary>
    [Description("The accessprofile's name")]
    public string Name { get; private set; }

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
    /// Collection of AccessProfile tags.
    /// </summary>
    [Description("Collection of AccessProfile tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static AccessProfile Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingAccessProfileNameException();
        }

        var accessprofile = new AccessProfile(id, name, DateTime.UtcNow, tags: tags);
        accessprofile.AddEvent(new AccessProfileCreated(accessprofile));
        return accessprofile;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new AccessProfileModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidAccessProfileTagsException();
        }
    }
}

