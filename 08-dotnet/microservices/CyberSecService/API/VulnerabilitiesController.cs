using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.CyberSecService.Infrastructure;
using Ierahkwa.CyberSecService.Domain;

namespace Ierahkwa.CyberSecService.API;

[ApiController]
[Route("api/[controller]")]
public class VulnerabilitiesController : ControllerBase
{
    private readonly CyberSecServiceDbContext _db;
    public VulnerabilitiesController(CyberSecServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<Vulnerability>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<Vulnerability>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] Vulnerability entity)
    { _db.Set<Vulnerability>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] Vulnerability entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<Vulnerability>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<Vulnerability>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
