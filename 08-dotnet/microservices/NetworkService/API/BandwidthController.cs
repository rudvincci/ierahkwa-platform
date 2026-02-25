using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.NetworkService.Infrastructure;
using Ierahkwa.NetworkService.Domain;

namespace Ierahkwa.NetworkService.API;

[ApiController]
[Route("api/[controller]")]
public class BandwidthController : ControllerBase
{
    private readonly NetworkServiceDbContext _db;
    public BandwidthController(NetworkServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<BandwidthAllocation>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<BandwidthAllocation>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] BandwidthAllocation entity)
    { _db.Set<BandwidthAllocation>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] BandwidthAllocation entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<BandwidthAllocation>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<BandwidthAllocation>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
