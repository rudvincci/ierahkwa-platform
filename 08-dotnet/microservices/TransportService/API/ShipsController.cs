using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.TransportService.Infrastructure;
using Ierahkwa.TransportService.Domain;

namespace Ierahkwa.TransportService.API;

[ApiController]
[Route("api/[controller]")]
public class ShipsController : ControllerBase
{
    private readonly TransportServiceDbContext _db;
    public ShipsController(TransportServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<ShipRecord>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<ShipRecord>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] ShipRecord entity)
    { _db.Set<ShipRecord>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] ShipRecord entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<ShipRecord>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<ShipRecord>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
