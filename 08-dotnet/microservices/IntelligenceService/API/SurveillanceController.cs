using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.IntelligenceService.Infrastructure;
using Ierahkwa.IntelligenceService.Domain;

namespace Ierahkwa.IntelligenceService.API;

[ApiController]
[Route("api/[controller]")]
public class SurveillanceController : ControllerBase
{
    private readonly IntelligenceServiceDbContext _db;
    public SurveillanceController(IntelligenceServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<Surveillance>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<Surveillance>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] Surveillance entity)
    { _db.Set<Surveillance>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] Surveillance entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<Surveillance>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<Surveillance>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
