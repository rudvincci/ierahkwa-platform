using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.SearchService.Infrastructure;
using Ierahkwa.SearchService.Domain;

namespace Ierahkwa.SearchService.API;

[ApiController]
[Route("api/[controller]")]
public class ResultsController : ControllerBase
{
    private readonly SearchServiceDbContext _db;
    public ResultsController(SearchServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<SearchResult>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<SearchResult>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] SearchResult entity)
    { _db.Set<SearchResult>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] SearchResult entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<SearchResult>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<SearchResult>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
