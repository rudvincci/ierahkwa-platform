using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Aftercare.Domain.Events;
using Pupitre.Aftercare.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.Unit.Core.Entities")]
namespace Pupitre.Aftercare.Domain.Entities;


internal class AftercarePlan : AggregateRoot<AftercarePlanId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public AftercarePlan(AftercarePlanId id, string name, DateTime createdAt,
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
    /// A name for the aftercareplan.
    /// </summary>
    [Description("The aftercareplan's name")]
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
    /// Collection of AftercarePlan tags.
    /// </summary>
    [Description("Collection of AftercarePlan tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static AftercarePlan Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingAftercarePlanNameException();
        }

        var aftercareplan = new AftercarePlan(id, name, DateTime.UtcNow, tags: tags);
        aftercareplan.AddEvent(new AftercarePlanCreated(aftercareplan));
        return aftercareplan;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new AftercarePlanModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidAftercarePlanTagsException();
        }
    }
}

