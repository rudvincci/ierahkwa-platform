using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Analytics.Domain.Events;
using Pupitre.Analytics.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.Unit.Core.Entities")]
namespace Pupitre.Analytics.Domain.Entities;


internal class Analytic : AggregateRoot<AnalyticId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Analytic(AnalyticId id, string name, DateTime createdAt,
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
    /// A name for the analytic.
    /// </summary>
    [Description("The analytic's name")]
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
    /// Collection of Analytic tags.
    /// </summary>
    [Description("Collection of Analytic tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static Analytic Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingAnalyticNameException();
        }

        var analytic = new Analytic(id, name, DateTime.UtcNow, tags: tags);
        analytic.AddEvent(new AnalyticCreated(analytic));
        return analytic;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new AnalyticModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidAnalyticTagsException();
        }
    }
}

