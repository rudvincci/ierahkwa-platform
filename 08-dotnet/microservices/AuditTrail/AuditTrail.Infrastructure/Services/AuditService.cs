using AuditTrail.Core.Interfaces;
using AuditTrail.Core.Models;
using System.Text;
namespace AuditTrail.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly List<AuditEntry> _entries = new();
    private readonly List<SecurityEvent> _securityEvents = new();
    private readonly List<ComplianceReport> _reports = new();
    private readonly List<DataRetentionPolicy> _policies = new();
    private readonly List<AuditAlert> _alerts = new();

    public async Task<AuditEntry> LogAsync(AuditEntry entry) { entry.Id = Guid.NewGuid(); entry.TraceId = Guid.NewGuid().ToString("N")[..16]; entry.Timestamp = DateTime.UtcNow; _entries.Add(entry); await EvaluateAlertsAsync(entry); return entry; }
    public async Task<IEnumerable<AuditEntry>> LogBatchAsync(IEnumerable<AuditEntry> entries) { var results = new List<AuditEntry>(); foreach (var e in entries) results.Add(await LogAsync(e)); return results; }
    public Task<AuditEntry?> GetEntryByIdAsync(Guid id) => Task.FromResult(_entries.FirstOrDefault(e => e.Id == id));

    public Task<IEnumerable<AuditEntry>> SearchAsync(AuditSearchCriteria criteria)
    {
        var q = _entries.AsEnumerable();
        if (!string.IsNullOrEmpty(criteria.Service)) q = q.Where(e => e.Service == criteria.Service);
        if (!string.IsNullOrEmpty(criteria.Entity)) q = q.Where(e => e.Entity == criteria.Entity);
        if (!string.IsNullOrEmpty(criteria.EntityId)) q = q.Where(e => e.EntityId == criteria.EntityId);
        if (criteria.Action.HasValue) q = q.Where(e => e.Action == criteria.Action.Value);
        if (criteria.UserId.HasValue) q = q.Where(e => e.UserId == criteria.UserId.Value);
        if (criteria.Severity.HasValue) q = q.Where(e => e.Severity == criteria.Severity.Value);
        if (criteria.From.HasValue) q = q.Where(e => e.Timestamp >= criteria.From.Value);
        if (criteria.To.HasValue) q = q.Where(e => e.Timestamp <= criteria.To.Value);
        if (!string.IsNullOrEmpty(criteria.SearchText)) q = q.Where(e => e.Entity.Contains(criteria.SearchText) || (e.UserName?.Contains(criteria.SearchText) ?? false));
        return Task.FromResult(q.OrderByDescending(e => e.Timestamp).Skip((criteria.Page - 1) * criteria.PageSize).Take(criteria.PageSize));
    }

    public Task<IEnumerable<AuditEntry>> GetEntityHistoryAsync(string entity, string entityId) => Task.FromResult(_entries.Where(e => e.Entity == entity && e.EntityId == entityId).OrderByDescending(e => e.Timestamp));
    public Task<IEnumerable<AuditEntry>> GetUserActivityAsync(Guid userId, DateTime? from = null, DateTime? to = null) { var q = _entries.Where(e => e.UserId == userId); if (from.HasValue) q = q.Where(e => e.Timestamp >= from.Value); if (to.HasValue) q = q.Where(e => e.Timestamp <= to.Value); return Task.FromResult(q.OrderByDescending(e => e.Timestamp)); }

    public Task<SecurityEvent> LogSecurityEventAsync(SecurityEvent securityEvent) { securityEvent.Id = Guid.NewGuid(); securityEvent.Timestamp = DateTime.UtcNow; _securityEvents.Add(securityEvent); return Task.FromResult(securityEvent); }
    public Task<IEnumerable<SecurityEvent>> GetSecurityEventsAsync(SecurityEventType? type = null, AuditSeverity? severity = null, bool? unresolved = null) { var q = _securityEvents.AsEnumerable(); if (type.HasValue) q = q.Where(e => e.Type == type.Value); if (severity.HasValue) q = q.Where(e => e.Severity == severity.Value); if (unresolved == true) q = q.Where(e => !e.IsResolved); return Task.FromResult(q.OrderByDescending(e => e.Timestamp)); }
    public Task<SecurityEvent> ResolveSecurityEventAsync(Guid id, Guid resolvedBy, string resolution) { var e = _securityEvents.FirstOrDefault(e => e.Id == id); if (e != null) { e.IsResolved = true; e.ResolvedAt = DateTime.UtcNow; e.ResolvedBy = resolvedBy; e.Resolution = resolution; } return Task.FromResult(e!); }

    public Task<ComplianceReport> GenerateComplianceReportAsync(ComplianceType type, string? framework, DateTime periodStart, DateTime periodEnd, Guid generatedBy)
    {
        var report = new ComplianceReport { Id = Guid.NewGuid(), ReportNumber = $"CMP-{DateTime.UtcNow:yyyyMMdd}-{_reports.Count + 1:D4}", Name = $"{type} Compliance Report", Type = type, Framework = framework, PeriodStart = periodStart, PeriodEnd = periodEnd, Status = ReportStatus.Completed, TotalControls = 50, PassedControls = 47, FailedControls = 3, GeneratedBy = generatedBy, GeneratedAt = DateTime.UtcNow };
        _reports.Add(report); return Task.FromResult(report);
    }
    public Task<IEnumerable<ComplianceReport>> GetComplianceReportsAsync(ComplianceType? type = null) => Task.FromResult(type.HasValue ? _reports.Where(r => r.Type == type.Value) : _reports.AsEnumerable());
    public Task<byte[]> ExportComplianceReportAsync(Guid reportId, string format) => Task.FromResult(Encoding.UTF8.GetBytes("Compliance Report Export"));

    public Task<DataRetentionPolicy> CreateRetentionPolicyAsync(DataRetentionPolicy policy) { policy.Id = Guid.NewGuid(); policy.CreatedAt = DateTime.UtcNow; _policies.Add(policy); return Task.FromResult(policy); }
    public Task<IEnumerable<DataRetentionPolicy>> GetRetentionPoliciesAsync() => Task.FromResult(_policies.Where(p => p.IsActive));
    public Task<DataRetentionPolicy> UpdateRetentionPolicyAsync(DataRetentionPolicy policy) { var e = _policies.FirstOrDefault(p => p.Id == policy.Id); if (e != null) { e.RetentionDays = policy.RetentionDays; e.AnonymizeOnExpiry = policy.AnonymizeOnExpiry; } return Task.FromResult(e ?? policy); }
    public Task ExecuteRetentionPoliciesAsync() { foreach (var p in _policies.Where(p => p.IsActive)) { var cutoff = DateTime.UtcNow.AddDays(-p.RetentionDays); if (p.AnonymizeOnExpiry) foreach (var e in _entries.Where(e => e.Entity == p.EntityType && e.Timestamp < cutoff)) { e.UserName = "***"; e.UserEmail = "***"; e.IpAddress = "***"; e.IsAnonymized = true; } else if (p.DeleteOnExpiry) _entries.RemoveAll(e => e.Entity == p.EntityType && e.Timestamp < cutoff); p.LastExecuted = DateTime.UtcNow; } return Task.CompletedTask; }

    public Task<AuditAlert> CreateAlertAsync(AuditAlert alert) { alert.Id = Guid.NewGuid(); alert.CreatedAt = DateTime.UtcNow; _alerts.Add(alert); return Task.FromResult(alert); }
    public Task<IEnumerable<AuditAlert>> GetAlertsAsync() => Task.FromResult(_alerts.Where(a => a.IsActive));
    public Task<AuditAlert> UpdateAlertAsync(AuditAlert alert) { var e = _alerts.FirstOrDefault(a => a.Id == alert.Id); if (e != null) { e.Condition = alert.Condition; e.IsActive = alert.IsActive; } return Task.FromResult(e ?? alert); }
    public Task EvaluateAlertsAsync(AuditEntry entry) { foreach (var a in _alerts.Where(a => a.IsActive && entry.Severity >= a.TriggerSeverity)) { a.TriggerCount++; a.LastTriggeredAt = DateTime.UtcNow; } return Task.CompletedTask; }

    public Task<byte[]> ExportAuditLogAsync(AuditSearchCriteria criteria, string format) => Task.FromResult(Encoding.UTF8.GetBytes("Audit Log Export"));

    public Task<AuditStatistics> GetStatisticsAsync(DateTime? from = null, DateTime? to = null, string? service = null)
    {
        var entries = _entries.AsEnumerable(); if (from.HasValue) entries = entries.Where(e => e.Timestamp >= from.Value); if (to.HasValue) entries = entries.Where(e => e.Timestamp <= to.Value); if (!string.IsNullOrEmpty(service)) entries = entries.Where(e => e.Service == service);
        var list = entries.ToList();
        return Task.FromResult(new AuditStatistics { TotalEntries = list.Count, EntriesToday = list.Count(e => e.Timestamp.Date == DateTime.UtcNow.Date), UniqueUsers = list.Where(e => e.UserId.HasValue).Select(e => e.UserId).Distinct().Count(), SecurityEventsCount = _securityEvents.Count, UnresolvedSecurityEvents = _securityEvents.Count(e => !e.IsResolved), ByService = list.GroupBy(e => e.Service).ToDictionary(g => g.Key, g => g.Count()), ByAction = list.GroupBy(e => e.Action.ToString()).ToDictionary(g => g.Key, g => g.Count()), BySeverity = list.GroupBy(e => e.Severity.ToString()).ToDictionary(g => g.Key, g => g.Count()), TopUsers = list.Where(e => e.UserId.HasValue && e.UserName != null).GroupBy(e => new { e.UserId, e.UserName }).Select(g => new TopUser { UserId = g.Key.UserId!.Value, UserName = g.Key.UserName!, ActionCount = g.Count() }).OrderByDescending(u => u.ActionCount).Take(10).ToList() });
    }
}
