using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Compliance.Domain.Events;
using Pupitre.Compliance.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.Unit.Core.Entities")]
namespace Pupitre.Compliance.Domain.Entities;


internal class ComplianceRecord : AggregateRoot<ComplianceRecordId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public ComplianceRecord(ComplianceRecordId id, string name, DateTime createdAt,
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
    /// A name for the compliancerecord.
    /// </summary>
    [Description("The compliancerecord's name")]
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
    /// Collection of ComplianceRecord tags.
    /// </summary>
    [Description("Collection of ComplianceRecord tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static ComplianceRecord Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingComplianceRecordNameException();
        }

        var compliancerecord = new ComplianceRecord(id, name, DateTime.UtcNow, tags: tags);
        compliancerecord.AddEvent(new ComplianceRecordCreated(compliancerecord));
        return compliancerecord;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new ComplianceRecordModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidComplianceRecordTagsException();
        }
    }
}

