using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.AISafety.Domain.Events;
using Pupitre.AISafety.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.Unit.Core.Entities")]
namespace Pupitre.AISafety.Domain.Entities;


internal class SafetyCheck : AggregateRoot<SafetyCheckId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public SafetyCheck(SafetyCheckId id, string name, DateTime createdAt,
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
    /// A name for the safetycheck.
    /// </summary>
    [Description("The safetycheck's name")]
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
    /// Collection of SafetyCheck tags.
    /// </summary>
    [Description("Collection of SafetyCheck tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static SafetyCheck Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingSafetyCheckNameException();
        }

        var safetycheck = new SafetyCheck(id, name, DateTime.UtcNow, tags: tags);
        safetycheck.AddEvent(new SafetyCheckCreated(safetycheck));
        return safetycheck;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new SafetyCheckModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidSafetyCheckTagsException();
        }
    }
}

