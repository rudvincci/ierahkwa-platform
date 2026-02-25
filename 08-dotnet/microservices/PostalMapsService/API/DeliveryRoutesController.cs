using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.PostalMapsService.Infrastructure;
using Ierahkwa.PostalMapsService.Domain;

namespace Ierahkwa.PostalMapsService.API;

[ApiController]
[Route("api/[controller]")]
public class DeliveryRoutesController : ControllerBase
{
    private readonly PostalMapsServiceDbContext _db;
    public DeliveryRoutesController(PostalMapsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<DeliveryRoute>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<DeliveryRoute>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] DeliveryRoute entity)
    { _db.Set<DeliveryRoute>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] DeliveryRoute entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<DeliveryRoute>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<DeliveryRoute>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
