using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AITranslation.Domain.Events;
using Pupitre.AITranslation.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Unit.Core.Entities")]
namespace Pupitre.AITranslation.Domain.Entities;


internal class TranslationRequest : AggregateRoot<TranslationRequestId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public TranslationRequest(TranslationRequestId id, string name, DateTime createdAt,
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
    /// A name for the translationrequest.
    /// </summary>
    [Description("The translationrequest's name")]
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
    /// Collection of TranslationRequest tags.
    /// </summary>
    [Description("Collection of TranslationRequest tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static TranslationRequest Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingTranslationRequestNameException();
        }

        var translationrequest = new TranslationRequest(id, name, DateTime.UtcNow, tags: tags);
        translationrequest.AddEvent(new TranslationRequestCreated(translationrequest));
        return translationrequest;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new TranslationRequestModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidTranslationRequestTagsException();
        }
    }
}

