using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.DroneService.Infrastructure;
using Ierahkwa.DroneService.Domain;

namespace Ierahkwa.DroneService.API;

[ApiController]
[Route("api/[controller]")]
public class FlightPlansController : ControllerBase
{
    private readonly DroneServiceDbContext _db;
    public FlightPlansController(DroneServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<FlightPlan>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<FlightPlan>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] FlightPlan entity)
    { _db.Set<FlightPlan>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] FlightPlan entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<FlightPlan>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<FlightPlan>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
