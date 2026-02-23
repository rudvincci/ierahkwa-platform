using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mamey.ServiceName.Domain.Events;
using Mamey.ServiceName.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.ServiceName.Tests.Unit.Core.Entities")]
namespace Mamey.ServiceName.Domain.Entities;


internal class EntityName : AggregateRoot<EntityNameId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public EntityName(EntityNameId id, string name, DateTime createdAt,
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
    /// A name for the entityname.
    /// </summary>
    [Description("The entityname's name")]
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
    /// Collection of EntityName tags.
    /// </summary>
    [Description("Collection of EntityName tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static EntityName Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingEntityNameNameException();
        }

        var entityname = new EntityName(id, name, DateTime.UtcNow, tags: tags);
        entityname.AddEvent(new EntityNameCreated(entityname));
        return entityname;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new EntityNameModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidEntityNameTagsException();
        }
    }
}

