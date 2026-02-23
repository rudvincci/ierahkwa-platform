using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AITutors.Domain.Events;
using Pupitre.AITutors.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.Unit.Core.Entities")]
namespace Pupitre.AITutors.Domain.Entities;


internal class Tutor : AggregateRoot<TutorId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Tutor(TutorId id, string name, DateTime createdAt,
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
    /// A name for the tutor.
    /// </summary>
    [Description("The tutor's name")]
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
    /// Collection of Tutor tags.
    /// </summary>
    [Description("Collection of Tutor tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static Tutor Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingTutorNameException();
        }

        var tutor = new Tutor(id, name, DateTime.UtcNow, tags: tags);
        tutor.AddEvent(new TutorCreated(tutor));
        return tutor;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new TutorModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidTutorTagsException();
        }
    }
}

