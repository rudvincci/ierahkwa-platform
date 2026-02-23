using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Operations.Domain.Events;
using Pupitre.Operations.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Operations.Tests.Unit.Core.Entities")]
namespace Pupitre.Operations.Domain.Entities;


internal class OperationMetric : AggregateRoot<OperationMetricId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public OperationMetric(OperationMetricId id, string name, DateTime createdAt,
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
    /// A name for the operationmetric.
    /// </summary>
    [Description("The operationmetric's name")]
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
    /// Collection of OperationMetric tags.
    /// </summary>
    [Description("Collection of OperationMetric tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static OperationMetric Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingOperationMetricNameException();
        }

        var operationmetric = new OperationMetric(id, name, DateTime.UtcNow, tags: tags);
        operationmetric.AddEvent(new OperationMetricCreated(operationmetric));
        return operationmetric;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new OperationMetricModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationMetricTagsException();
        }
    }
}

