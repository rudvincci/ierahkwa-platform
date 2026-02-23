using System.ComponentModel.DataAnnotations.Schema;

namespace Mamey.Types;

/// <summary>Audit log entry for request/activity tracing.</summary>
public class AuditLog : IEntity<Guid>
{
    /// <summary>EF-only.</summary>
    protected AuditLog() {}

    /// <summary>Create a new audit log record.</summary>
    public AuditLog(Guid id, string requestId, DateTime logDate,
        string activityType, string correlationId,
        string category, string status,
        UserId userId,
        Email email, Name name, AuditType type)
    {
        Id            = id;
        RequestId     = requestId;
        LogDate       = logDate;
        ActivityType  = activityType;
        CorrelationId = correlationId;
        Category      = category;
        Status        = status;
        UserId        = userId;
 
        Email         = email;
        Name          = name;
        Type          = type;
    }

    [Key]
    [Column("id")]
    public Guid Id { get; private set; }

    [Required]
    public string RequestId { get; private set; }

    [Required]
    public DateTime LogDate { get; private set; }

    [Required, MaxLength(128)]
    public string ActivityType { get; private set; }

    [Required, MaxLength(64)]
    public string CorrelationId { get; private set; }

    [Required, MaxLength(64)]
    public string Category { get; private set; }

    [Required, MaxLength(32)]
    public string Status { get; private set; }

    [Required]
    public UserId UserId { get; private set; }
    

    [Required]
    public Email Email { get; private set; }
    
    public Name Name { get; private set; }

    [Required]
    public AuditType Type { get; private set; }
}
/// <summary>
/// Classifies the kind of activity captured in an <see cref="AuditLog"/>.
/// Stored as a tiny int/byte for compactness.
/// </summary>
public enum AuditType : byte
{
    Authentication = 1,
    Authorization  = 2,
    Command        = 3,
    Query          = 4,
    Integration    = 5,
    System         = 6,
    Security       = 7,
    Error          = 8,
    Other          = 255
}