using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.NexusAggregationService.Infrastructure;
using Ierahkwa.NexusAggregationService.Domain;

namespace Ierahkwa.NexusAggregationService.API;

[ApiController]
[Route("api/[controller]")]
public class DomainsController : ControllerBase
{
    private readonly NexusAggregationServiceDbContext _db;
    public DomainsController(NexusAggregationServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<NexusDomain>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<NexusDomain>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] NexusDomain entity)
    { _db.Set<NexusDomain>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] NexusDomain entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<NexusDomain>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<NexusDomain>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
