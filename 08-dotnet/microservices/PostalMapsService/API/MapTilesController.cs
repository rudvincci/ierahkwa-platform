using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.PostalMapsService.Infrastructure;
using Ierahkwa.PostalMapsService.Domain;

namespace Ierahkwa.PostalMapsService.API;

[ApiController]
[Route("api/[controller]")]
public class MapTilesController : ControllerBase
{
    private readonly PostalMapsServiceDbContext _db;
    public MapTilesController(PostalMapsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<MapTile>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<MapTile>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] MapTile entity)
    { _db.Set<MapTile>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] MapTile entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<MapTile>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<MapTile>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
