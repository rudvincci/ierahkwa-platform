using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AIBehavior.Domain.Events;
using Pupitre.AIBehavior.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.Unit.Core.Entities")]
namespace Pupitre.AIBehavior.Domain.Entities;


internal class Behavior : AggregateRoot<BehaviorId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Behavior(BehaviorId id, string name, DateTime createdAt,
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
    /// A name for the behavior.
    /// </summary>
    [Description("The behavior's name")]
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
    /// Collection of Behavior tags.
    /// </summary>
    [Description("Collection of Behavior tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static Behavior Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingBehaviorNameException();
        }

        var behavior = new Behavior(id, name, DateTime.UtcNow, tags: tags);
        behavior.AddEvent(new BehaviorCreated(behavior));
        return behavior;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new BehaviorModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidBehaviorTagsException();
        }
    }
}

