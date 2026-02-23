using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an audit trail aggregate root for compliance and regulatory tracking.
/// </summary>
internal class AuditTrail : AggregateRoot<AuditTrailId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private AuditTrail()
    {
        Tags = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the AuditTrail aggregate root.
    /// </summary>
    /// <param name="id">The audit trail identifier.</param>
    /// <param name="eventType">The type of event being audited.</param>
    /// <param name="entityType">The type of entity involved.</param>
    /// <param name="entityId">The identifier of the entity involved.</param>
    /// <param name="action">The action performed.</param>
    /// <param name="userId">The user who performed the action.</param>
    /// <param name="ipAddress">The IP address of the user.</param>
    /// <param name="userAgent">The user agent of the client.</param>
    public AuditTrail(
        AuditTrailId id,
        string eventType,
        string entityType,
        string entityId,
        string action,
        string? userId = null,
        string? ipAddress = null,
        string? userAgent = null)
        : base(id)
    {
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
        Status = AuditStatus.Active;
        Tags = new List<string>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new AuditEventRecorded(Id, EventType, EntityType, EntityId, Action, UserId, Timestamp));
    }

    #region Properties

    /// <summary>
    /// The type of event being audited.
    /// </summary>
    public string EventType { get; private set; }

    /// <summary>
    /// The type of entity involved in the event.
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// The identifier of the entity involved.
    /// </summary>
    public string EntityId { get; private set; }

    /// <summary>
    /// The action that was performed.
    /// </summary>
    public string Action { get; private set; }

    /// <summary>
    /// The user who performed the action.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// The IP address of the user.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// The user agent of the client.
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// When the event occurred.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// The status of the audit record.
    /// </summary>
    public AuditStatus Status { get; private set; }

    /// <summary>
    /// Previous value (for update operations).
    /// </summary>
    public string? PreviousValue { get; private set; }

    /// <summary>
    /// New value (for update operations).
    /// </summary>
    public string? NewValue { get; private set; }

    /// <summary>
    /// Compliance level required for this event.
    /// </summary>
    public ComplianceLevel ComplianceLevel { get; private set; }

    /// <summary>
    /// Tags for categorization and searching.
    /// </summary>
    public List<string> Tags { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Marks the audit record as archived.
    /// </summary>
    public void Archive()
    {
        if (Status == AuditStatus.Archived)
            return;

        Status = AuditStatus.Archived;
        IncrementVersion();

        AddEvent(new AuditEventArchived(Id, "default-archive", DateTime.UtcNow));
    }

    /// <summary>
    /// Adds a tag to the audit record.
    /// </summary>
    /// <param name="tag">The tag to add.</param>
    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return;

        if (!Tags.Contains(tag))
        {
            Tags.Add(tag);
            IncrementVersion();
        }
    }

    /// <summary>
    /// Sets the compliance level for this audit event.
    /// </summary>
    /// <param name="level">The compliance level.</param>
    public void SetComplianceLevel(ComplianceLevel level)
    {
        ComplianceLevel = level;
        IncrementVersion();

        // Add appropriate tags based on compliance level
        switch (level)
        {
            case ComplianceLevel.GDPR:
                AddTag("gdpr");
                break;
            case ComplianceLevel.HIPAA:
                AddTag("hipaa");
                break;
            case ComplianceLevel.PCI:
                AddTag("pci");
                break;
            case ComplianceLevel.SOX:
                AddTag("sox");
                break;
        }
    }

    /// <summary>
    /// Records the previous and new values for update operations.
    /// </summary>
    /// <param name="previousValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    public void RecordValueChange(string? previousValue, string? newValue)
    {
        PreviousValue = previousValue;
        NewValue = newValue;
        IncrementVersion();
    }

    /// <summary>
    /// Adds metadata to the audit record.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
        IncrementVersion();
    }

    /// <summary>
    /// Checks if this audit event requires retention beyond standard periods.
    /// </summary>
    /// <returns>True if special retention rules apply.</returns>
    public bool RequiresExtendedRetention()
    {
        return ComplianceLevel != ComplianceLevel.Standard ||
               Tags.Contains("security") ||
               Tags.Contains("breach") ||
               Action.Contains("delete") ||
               Action.Contains("revoke");
    }

    /// <summary>
    /// Gets the required retention period in years.
    /// </summary>
    /// <returns>The retention period in years.</returns>
    public int GetRetentionPeriodYears()
    {
        if (RequiresExtendedRetention())
        {
            switch (ComplianceLevel)
            {
                case ComplianceLevel.GDPR:
                case ComplianceLevel.HIPAA:
                    return 7;
                case ComplianceLevel.SOX:
                    return 7;
                case ComplianceLevel.PCI:
                    return 3;
                default:
                    return 5; // Extended retention for sensitive operations
            }
        }

        return 3; // Standard 3-year retention
    }

    #endregion
}

/// <summary>
/// Represents the status of an audit record.
/// </summary>
internal enum AuditStatus
{
    Active,
    Archived,
    Deleted
}

/// <summary>
/// Represents the compliance level required for an audit event.
/// </summary>
internal enum ComplianceLevel
{
    Standard,
    GDPR,
    HIPAA,
    PCI,
    SOX
}
