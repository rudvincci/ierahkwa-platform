using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.BrowserService.Infrastructure;
using Ierahkwa.BrowserService.Domain;

namespace Ierahkwa.BrowserService.API;

[ApiController]
[Route("api/[controller]")]
public class ProxyController : ControllerBase
{
    private readonly BrowserServiceDbContext _db;
    public ProxyController(BrowserServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<ProxyRoute>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<ProxyRoute>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] ProxyRoute entity)
    { _db.Set<ProxyRoute>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] ProxyRoute entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<ProxyRoute>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<ProxyRoute>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
