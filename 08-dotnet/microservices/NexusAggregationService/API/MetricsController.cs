using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.NexusAggregationService.Infrastructure;
using Ierahkwa.NexusAggregationService.Domain;

namespace Ierahkwa.NexusAggregationService.API;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly NexusAggregationServiceDbContext _db;
    public MetricsController(NexusAggregationServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<SystemMetric>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<SystemMetric>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] SystemMetric entity)
    { _db.Set<SystemMetric>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] SystemMetric entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<SystemMetric>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<SystemMetric>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
