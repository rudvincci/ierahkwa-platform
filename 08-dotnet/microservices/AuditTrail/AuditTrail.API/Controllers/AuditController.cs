using AuditTrail.Core.Interfaces;
using AuditTrail.Core.Models;
using Microsoft.AspNetCore.Mvc;
namespace AuditTrail.API.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _service;
    public AuditController(IAuditService service) => _service = service;

    [HttpPost] public async Task<ActionResult<AuditEntry>> Log([FromBody] AuditEntry entry) => await _service.LogAsync(entry);
    [HttpPost("batch")] public async Task<ActionResult<IEnumerable<AuditEntry>>> LogBatch([FromBody] IEnumerable<AuditEntry> entries) => Ok(await _service.LogBatchAsync(entries));
    [HttpGet("{id}")] public async Task<ActionResult<AuditEntry>> GetById(Guid id) { var e = await _service.GetEntryByIdAsync(id); return e == null ? NotFound() : e; }
    [HttpGet] public async Task<ActionResult<IEnumerable<AuditEntry>>> Search([FromQuery] AuditSearchCriteria criteria) => Ok(await _service.SearchAsync(criteria));
    [HttpGet("entity/{entity}/{entityId}")] public async Task<ActionResult<IEnumerable<AuditEntry>>> GetEntityHistory(string entity, string entityId) => Ok(await _service.GetEntityHistoryAsync(entity, entityId));
    [HttpGet("user/{userId}")] public async Task<ActionResult<IEnumerable<AuditEntry>>> GetUserActivity(Guid userId, [FromQuery] DateTime? from, [FromQuery] DateTime? to) => Ok(await _service.GetUserActivityAsync(userId, from, to));
    [HttpGet("export")] public async Task<ActionResult> Export([FromQuery] AuditSearchCriteria criteria, [FromQuery] string format = "csv") { var data = await _service.ExportAuditLogAsync(criteria, format); return File(data, "text/csv", "audit-log.csv"); }
    [HttpGet("statistics")] public async Task<ActionResult<AuditStatistics>> GetStatistics([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? service) => await _service.GetStatisticsAsync(from, to, service);
}

[ApiController]
[Route("api/security-events")]
public class SecurityEventsController : ControllerBase
{
    private readonly IAuditService _service;
    public SecurityEventsController(IAuditService service) => _service = service;

    [HttpPost] public async Task<ActionResult<SecurityEvent>> Log([FromBody] SecurityEvent securityEvent) => await _service.LogSecurityEventAsync(securityEvent);
    [HttpGet] public async Task<ActionResult<IEnumerable<SecurityEvent>>> GetAll([FromQuery] SecurityEventType? type, [FromQuery] AuditSeverity? severity, [FromQuery] bool? unresolved) => Ok(await _service.GetSecurityEventsAsync(type, severity, unresolved));
    [HttpPost("{id}/resolve")] public async Task<ActionResult<SecurityEvent>> Resolve(Guid id, [FromQuery] Guid resolvedBy, [FromBody] string resolution) => await _service.ResolveSecurityEventAsync(id, resolvedBy, resolution);
}

[ApiController]
[Route("api/compliance")]
public class ComplianceController : ControllerBase
{
    private readonly IAuditService _service;
    public ComplianceController(IAuditService service) => _service = service;

    [HttpPost("reports")] public async Task<ActionResult<ComplianceReport>> Generate([FromQuery] ComplianceType type, [FromQuery] string? framework, [FromQuery] DateTime periodStart, [FromQuery] DateTime periodEnd, [FromQuery] Guid generatedBy) => await _service.GenerateComplianceReportAsync(type, framework, periodStart, periodEnd, generatedBy);
    [HttpGet("reports")] public async Task<ActionResult<IEnumerable<ComplianceReport>>> GetReports([FromQuery] ComplianceType? type) => Ok(await _service.GetComplianceReportsAsync(type));
    [HttpGet("reports/{id}/export")] public async Task<ActionResult> ExportReport(Guid id, [FromQuery] string format = "pdf") { var data = await _service.ExportComplianceReportAsync(id, format); return File(data, "application/pdf", "compliance-report.pdf"); }
}

[ApiController]
[Route("api/retention-policies")]
public class RetentionPoliciesController : ControllerBase
{
    private readonly IAuditService _service;
    public RetentionPoliciesController(IAuditService service) => _service = service;
    [HttpPost] public async Task<ActionResult<DataRetentionPolicy>> Create([FromBody] DataRetentionPolicy policy) => await _service.CreateRetentionPolicyAsync(policy);
    [HttpGet] public async Task<ActionResult<IEnumerable<DataRetentionPolicy>>> GetAll() => Ok(await _service.GetRetentionPoliciesAsync());
    [HttpPut("{id}")] public async Task<ActionResult<DataRetentionPolicy>> Update(Guid id, [FromBody] DataRetentionPolicy policy) => await _service.UpdateRetentionPolicyAsync(policy);
    [HttpPost("execute")] public async Task<ActionResult> Execute() { await _service.ExecuteRetentionPoliciesAsync(); return Ok(); }
}

[ApiController]
[Route("api/audit-alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAuditService _service;
    public AlertsController(IAuditService service) => _service = service;
    [HttpPost] public async Task<ActionResult<AuditAlert>> Create([FromBody] AuditAlert alert) => await _service.CreateAlertAsync(alert);
    [HttpGet] public async Task<ActionResult<IEnumerable<AuditAlert>>> GetAll() => Ok(await _service.GetAlertsAsync());
    [HttpPut("{id}")] public async Task<ActionResult<AuditAlert>> Update(Guid id, [FromBody] AuditAlert alert) => await _service.UpdateAlertAsync(alert);
}
