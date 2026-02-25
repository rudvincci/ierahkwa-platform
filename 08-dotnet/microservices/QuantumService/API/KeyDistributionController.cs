using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.QuantumService.Infrastructure;
using Ierahkwa.QuantumService.Domain;

namespace Ierahkwa.QuantumService.API;

[ApiController]
[Route("api/[controller]")]
public class KeyDistributionController : ControllerBase
{
    private readonly QuantumServiceDbContext _db;
    public KeyDistributionController(QuantumServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<KeyDistribution>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<KeyDistribution>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] KeyDistribution entity)
    { _db.Set<KeyDistribution>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] KeyDistribution entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<KeyDistribution>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<KeyDistribution>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
