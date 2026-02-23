using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AISpeech.Domain.Events;
using Pupitre.AISpeech.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.Unit.Core.Entities")]
namespace Pupitre.AISpeech.Domain.Entities;


internal class SpeechRequest : AggregateRoot<SpeechRequestId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public SpeechRequest(SpeechRequestId id, string name, DateTime createdAt,
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
    /// A name for the speechrequest.
    /// </summary>
    [Description("The speechrequest's name")]
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
    /// Collection of SpeechRequest tags.
    /// </summary>
    [Description("Collection of SpeechRequest tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static SpeechRequest Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingSpeechRequestNameException();
        }

        var speechrequest = new SpeechRequest(id, name, DateTime.UtcNow, tags: tags);
        speechrequest.AddEvent(new SpeechRequestCreated(speechrequest));
        return speechrequest;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new SpeechRequestModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidSpeechRequestTagsException();
        }
    }
}

