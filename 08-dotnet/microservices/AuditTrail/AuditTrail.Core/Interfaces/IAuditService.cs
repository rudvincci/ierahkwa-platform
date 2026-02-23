using AuditTrail.Core.Models;
namespace AuditTrail.Core.Interfaces;

public interface IAuditService
{
    Task<AuditEntry> LogAsync(AuditEntry entry);
    Task<IEnumerable<AuditEntry>> LogBatchAsync(IEnumerable<AuditEntry> entries);
    Task<AuditEntry?> GetEntryByIdAsync(Guid id);
    Task<IEnumerable<AuditEntry>> SearchAsync(AuditSearchCriteria criteria);
    Task<IEnumerable<AuditEntry>> GetEntityHistoryAsync(string entity, string entityId);
    Task<IEnumerable<AuditEntry>> GetUserActivityAsync(Guid userId, DateTime? from = null, DateTime? to = null);

    Task<SecurityEvent> LogSecurityEventAsync(SecurityEvent securityEvent);
    Task<IEnumerable<SecurityEvent>> GetSecurityEventsAsync(SecurityEventType? type = null, AuditSeverity? severity = null, bool? unresolved = null);
    Task<SecurityEvent> ResolveSecurityEventAsync(Guid id, Guid resolvedBy, string resolution);

    Task<ComplianceReport> GenerateComplianceReportAsync(ComplianceType type, string? framework, DateTime periodStart, DateTime periodEnd, Guid generatedBy);
    Task<IEnumerable<ComplianceReport>> GetComplianceReportsAsync(ComplianceType? type = null);
    Task<byte[]> ExportComplianceReportAsync(Guid reportId, string format);

    Task<DataRetentionPolicy> CreateRetentionPolicyAsync(DataRetentionPolicy policy);
    Task<IEnumerable<DataRetentionPolicy>> GetRetentionPoliciesAsync();
    Task<DataRetentionPolicy> UpdateRetentionPolicyAsync(DataRetentionPolicy policy);
    Task ExecuteRetentionPoliciesAsync();

    Task<AuditAlert> CreateAlertAsync(AuditAlert alert);
    Task<IEnumerable<AuditAlert>> GetAlertsAsync();
    Task<AuditAlert> UpdateAlertAsync(AuditAlert alert);
    Task EvaluateAlertsAsync(AuditEntry entry);

    Task<byte[]> ExportAuditLogAsync(AuditSearchCriteria criteria, string format);
    Task<AuditStatistics> GetStatisticsAsync(DateTime? from = null, DateTime? to = null, string? service = null);
}

public class AuditSearchCriteria
{
    public string? Service { get; set; }
    public string? Entity { get; set; }
    public string? EntityId { get; set; }
    public AuditAction? Action { get; set; }
    public Guid? UserId { get; set; }
    public string? IpAddress { get; set; }
    public AuditSeverity? Severity { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? SearchText { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class AuditStatistics
{
    public long TotalEntries { get; set; }
    public long EntriesToday { get; set; }
    public int UniqueUsers { get; set; }
    public int SecurityEventsCount { get; set; }
    public int UnresolvedSecurityEvents { get; set; }
    public Dictionary<string, int> ByService { get; set; } = new();
    public Dictionary<string, int> ByAction { get; set; } = new();
    public Dictionary<string, int> BySeverity { get; set; } = new();
    public List<HourlyActivity> HourlyTrend { get; set; } = new();
    public List<TopUser> TopUsers { get; set; } = new();
}

public class HourlyActivity { public int Hour { get; set; } public int Count { get; set; } }
public class TopUser { public Guid UserId { get; set; } public string UserName { get; set; } = string.Empty; public int ActionCount { get; set; } }
