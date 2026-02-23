namespace AuditTrail.Core.Models;

public class AuditEntry
{
    public Guid Id { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangedProperties { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? UserRole { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public int? ResponseStatus { get; set; }
    public string? CorrelationId { get; set; }
    public string? SessionId { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public AuditSeverity Severity { get; set; }
    public string? Tags { get; set; }
    public string? Metadata { get; set; }
    public bool IsAnonymized { get; set; }
    public DateTime Timestamp { get; set; }
    public long? DurationMs { get; set; }
}

public class SecurityEvent
{
    public Guid Id { get; set; }
    public SecurityEventType Type { get; set; }
    public AuditSeverity Severity { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Resource { get; set; }
    public string? Details { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
    public DateTime Timestamp { get; set; }
}

public class ComplianceReport
{
    public Guid Id { get; set; }
    public string ReportNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ComplianceType Type { get; set; }
    public string? Framework { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ReportStatus Status { get; set; }
    public int TotalControls { get; set; }
    public int PassedControls { get; set; }
    public int FailedControls { get; set; }
    public string? Findings { get; set; }
    public string? Recommendations { get; set; }
    public Guid GeneratedBy { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string? ReportUrl { get; set; }
}

public class DataRetentionPolicy
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int RetentionDays { get; set; }
    public bool AnonymizeOnExpiry { get; set; }
    public bool DeleteOnExpiry { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastExecuted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditAlert
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Condition { get; set; } = string.Empty;
    public AuditSeverity TriggerSeverity { get; set; }
    public string? NotifyUsers { get; set; }
    public string? NotifyChannels { get; set; }
    public bool IsActive { get; set; } = true;
    public int TriggerCount { get; set; }
    public DateTime? LastTriggeredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum AuditAction { Create, Read, Update, Delete, Login, Logout, Export, Import, Approve, Reject, Execute, Configure, Access, Download, Upload, Share }
public enum AuditSeverity { Debug, Info, Warning, Error, Critical }
public enum SecurityEventType { LoginSuccess, LoginFailure, PasswordChange, PermissionDenied, SuspiciousActivity, DataBreach, Intrusion, MalwareDetected, PolicyViolation, UnauthorizedAccess, BruteForce, SessionHijack }
public enum ComplianceType { GDPR, HIPAA, SOX, PCI_DSS, ISO27001, NIST, Custom }
public enum ReportStatus { Generating, Completed, Failed }
