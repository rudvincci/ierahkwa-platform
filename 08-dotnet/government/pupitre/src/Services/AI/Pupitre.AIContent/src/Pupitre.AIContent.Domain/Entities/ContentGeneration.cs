using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AIContent.Domain.Events;
using Pupitre.AIContent.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Unit.Core.Entities")]
namespace Pupitre.AIContent.Domain.Entities;


internal class ContentGeneration : AggregateRoot<ContentGenerationId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public ContentGeneration(ContentGenerationId id, string name, DateTime createdAt,
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
    /// A name for the contentgeneration.
    /// </summary>
    [Description("The contentgeneration's name")]
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
    /// Collection of ContentGeneration tags.
    /// </summary>
    [Description("Collection of ContentGeneration tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static ContentGeneration Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingContentGenerationNameException();
        }

        var contentgeneration = new ContentGeneration(id, name, DateTime.UtcNow, tags: tags);
        contentgeneration.AddEvent(new ContentGenerationCreated(contentgeneration));
        return contentgeneration;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new ContentGenerationModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidContentGenerationTagsException();
        }
    }
}

