using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.IoTRoboticsService.Infrastructure;
using Ierahkwa.IoTRoboticsService.Domain;

namespace Ierahkwa.IoTRoboticsService.API;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly IoTRoboticsServiceDbContext _db;
    public SensorsController(IoTRoboticsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<SensorReading>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<SensorReading>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] SensorReading entity)
    { _db.Set<SensorReading>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] SensorReading entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<SensorReading>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<SensorReading>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
