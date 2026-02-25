using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.CyberSecService.Infrastructure;
using Ierahkwa.CyberSecService.Domain;

namespace Ierahkwa.CyberSecService.API;

[ApiController]
[Route("api/[controller]")]
public class ThreatsController : ControllerBase
{
    private readonly CyberSecServiceDbContext _db;
    public ThreatsController(CyberSecServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<ThreatAlert>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<ThreatAlert>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] ThreatAlert entity)
    { _db.Set<ThreatAlert>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] ThreatAlert entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<ThreatAlert>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<ThreatAlert>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
