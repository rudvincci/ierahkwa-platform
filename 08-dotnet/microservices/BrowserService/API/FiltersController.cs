using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.BrowserService.Infrastructure;
using Ierahkwa.BrowserService.Domain;

namespace Ierahkwa.BrowserService.API;

[ApiController]
[Route("api/[controller]")]
public class FiltersController : ControllerBase
{
    private readonly BrowserServiceDbContext _db;
    public FiltersController(BrowserServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<ContentFilter>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<ContentFilter>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] ContentFilter entity)
    { _db.Set<ContentFilter>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] ContentFilter entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<ContentFilter>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<ContentFilter>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
