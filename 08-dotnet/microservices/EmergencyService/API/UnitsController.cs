using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.EmergencyService.Infrastructure;
using Ierahkwa.EmergencyService.Domain;

namespace Ierahkwa.EmergencyService.API;

[ApiController]
[Route("api/[controller]")]
public class UnitsController : ControllerBase
{
    private readonly EmergencyServiceDbContext _db;
    public UnitsController(EmergencyServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<EmergencyUnit>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<EmergencyUnit>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] EmergencyUnit entity)
    { _db.Set<EmergencyUnit>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] EmergencyUnit entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<EmergencyUnit>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<EmergencyUnit>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
