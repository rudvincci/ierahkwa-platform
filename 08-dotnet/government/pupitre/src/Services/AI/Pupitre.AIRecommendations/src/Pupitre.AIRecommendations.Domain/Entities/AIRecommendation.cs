using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AIRecommendations.Domain.Events;
using Pupitre.AIRecommendations.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.Unit.Core.Entities")]
namespace Pupitre.AIRecommendations.Domain.Entities;


internal class AIRecommendation : AggregateRoot<AIRecommendationId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public AIRecommendation(AIRecommendationId id, string name, DateTime createdAt,
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
    /// A name for the airecommendation.
    /// </summary>
    [Description("The airecommendation's name")]
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
    /// Collection of AIRecommendation tags.
    /// </summary>
    [Description("Collection of AIRecommendation tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static AIRecommendation Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingAIRecommendationNameException();
        }

        var airecommendation = new AIRecommendation(id, name, DateTime.UtcNow, tags: tags);
        airecommendation.AddEvent(new AIRecommendationCreated(airecommendation));
        return airecommendation;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new AIRecommendationModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidAIRecommendationTagsException();
        }
    }
}

