using AssetTracker.Core.Interfaces;
using AssetTracker.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AssetTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;
    public AssetsController(IAssetService service) => _service = service;

    [HttpPost] public async Task<ActionResult<Asset>> Create([FromBody] Asset asset) => CreatedAtAction(nameof(GetById), new { id = (await _service.CreateAssetAsync(asset)).Id }, asset);
    [HttpGet("{id}")] public async Task<ActionResult<Asset>> GetById(Guid id) { var a = await _service.GetAssetByIdAsync(id); return a == null ? NotFound() : a; }
    [HttpGet("tag/{tag}")] public async Task<ActionResult<Asset>> GetByTag(string tag) { var a = await _service.GetAssetByTagAsync(tag); return a == null ? NotFound() : a; }
    [HttpGet] public async Task<ActionResult<IEnumerable<Asset>>> GetAll([FromQuery] AssetCategory? category, [FromQuery] AssetStatus? status, [FromQuery] string? department, [FromQuery] string? location) => Ok(await _service.GetAssetsAsync(category, status, department, location));
    [HttpGet("assignee/{userId}")] public async Task<ActionResult<IEnumerable<Asset>>> GetByAssignee(Guid userId) => Ok(await _service.GetAssetsByAssigneeAsync(userId));
    [HttpPut("{id}")] public async Task<ActionResult<Asset>> Update(Guid id, [FromBody] Asset asset) => await _service.UpdateAssetAsync(asset);
    [HttpPost("{id}/assign")] public async Task<ActionResult<Asset>> Assign(Guid id, [FromQuery] Guid userId, [FromQuery] string userName, [FromQuery] string? department, [FromQuery] Guid assignedBy) => await _service.AssignAssetAsync(id, userId, userName, department, assignedBy);
    [HttpPost("{id}/return")] public async Task<ActionResult<Asset>> Return(Guid id, [FromQuery] string? condition, [FromQuery] string? notes) => await _service.ReturnAssetAsync(id, condition, notes);
    [HttpPost("{id}/dispose")] public async Task<ActionResult<Asset>> Dispose(Guid id, [FromBody] string reason) => await _service.DisposeAssetAsync(id, reason);
    [HttpDelete("{id}")] public async Task<ActionResult> Delete(Guid id) { await _service.DeleteAssetAsync(id); return NoContent(); }
    [HttpGet("{id}/depreciation")] public async Task<ActionResult<decimal>> GetDepreciation(Guid id) => await _service.CalculateDepreciationAsync(id);
    [HttpPost("{id}/maintenance")] public async Task<ActionResult<AssetMaintenance>> ScheduleMaintenance(Guid id, [FromBody] AssetMaintenance m) { m.AssetId = id; return await _service.ScheduleMaintenanceAsync(m); }
    [HttpGet("{id}/maintenance")] public async Task<ActionResult<IEnumerable<AssetMaintenance>>> GetMaintenance(Guid id) => Ok(await _service.GetMaintenanceHistoryAsync(id));
    [HttpGet("maintenance/upcoming")] public async Task<ActionResult<IEnumerable<AssetMaintenance>>> GetUpcomingMaintenance([FromQuery] int days = 30) => Ok(await _service.GetUpcomingMaintenanceAsync(days));
    [HttpGet("{id}/assignments")] public async Task<ActionResult<IEnumerable<AssetAssignment>>> GetAssignments(Guid id) => Ok(await _service.GetAssignmentHistoryAsync(id));
    [HttpGet("{id}/qrcode")] public async Task<ActionResult<string>> GetQrCode(Guid id) => await _service.GenerateQrCodeAsync(id);
    [HttpGet("statistics")] public async Task<ActionResult<AssetStatistics>> GetStatistics([FromQuery] string? department) => await _service.GetStatisticsAsync(department);
}

[ApiController]
[Route("api/[controller]")]
public class AuditsController : ControllerBase
{
    private readonly IAssetService _service;
    public AuditsController(IAssetService service) => _service = service;

    [HttpPost] public async Task<ActionResult<AssetAudit>> Create([FromBody] AssetAudit audit) => await _service.CreateAuditAsync(audit);
    [HttpGet("{id}")] public async Task<ActionResult<AssetAudit>> GetById(Guid id) { var a = await _service.GetAuditByIdAsync(id); return a == null ? NotFound() : a; }
    [HttpGet] public async Task<ActionResult<IEnumerable<AssetAudit>>> GetAll([FromQuery] AuditStatus? status) => Ok(await _service.GetAuditsAsync(status));
    [HttpPost("{id}/complete")] public async Task<ActionResult<AssetAudit>> Complete(Guid id, [FromQuery] int found, [FromQuery] int missing, [FromQuery] int discrepancy, [FromQuery] string? notes) => await _service.CompleteAuditAsync(id, found, missing, discrepancy, notes);
}
