using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.SearchService.Infrastructure;
using Ierahkwa.SearchService.Domain;

namespace Ierahkwa.SearchService.API;

[ApiController]
[Route("api/[controller]")]
public class IndicesController : ControllerBase
{
    private readonly SearchServiceDbContext _db;
    public IndicesController(SearchServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<SearchIndex>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<SearchIndex>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] SearchIndex entity)
    { _db.Set<SearchIndex>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] SearchIndex entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<SearchIndex>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<SearchIndex>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
