using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.IoTRoboticsService.Infrastructure;
using Ierahkwa.IoTRoboticsService.Domain;

namespace Ierahkwa.IoTRoboticsService.API;

[ApiController]
[Route("api/[controller]")]
public class NeuralController : ControllerBase
{
    private readonly IoTRoboticsServiceDbContext _db;
    public NeuralController(IoTRoboticsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<NeuralInterface>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<NeuralInterface>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] NeuralInterface entity)
    { _db.Set<NeuralInterface>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] NeuralInterface entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<NeuralInterface>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<NeuralInterface>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
