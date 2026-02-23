using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AIAdaptive.Domain.Events;
using Pupitre.AIAdaptive.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.Unit.Core.Entities")]
namespace Pupitre.AIAdaptive.Domain.Entities;


internal class AdaptiveLearning : AggregateRoot<AdaptiveLearningId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public AdaptiveLearning(AdaptiveLearningId id, string name, DateTime createdAt,
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
    /// A name for the adaptivelearning.
    /// </summary>
    [Description("The adaptivelearning's name")]
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
    /// Collection of AdaptiveLearning tags.
    /// </summary>
    [Description("Collection of AdaptiveLearning tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static AdaptiveLearning Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingAdaptiveLearningNameException();
        }

        var adaptivelearning = new AdaptiveLearning(id, name, DateTime.UtcNow, tags: tags);
        adaptivelearning.AddEvent(new AdaptiveLearningCreated(adaptivelearning));
        return adaptivelearning;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new AdaptiveLearningModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidAdaptiveLearningTagsException();
        }
    }
}

