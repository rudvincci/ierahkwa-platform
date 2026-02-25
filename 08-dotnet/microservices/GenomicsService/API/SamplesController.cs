using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.GenomicsService.Infrastructure;
using Ierahkwa.GenomicsService.Domain;

namespace Ierahkwa.GenomicsService.API;

[ApiController]
[Route("api/[controller]")]
public class SamplesController : ControllerBase
{
    private readonly GenomicsServiceDbContext _db;
    public SamplesController(GenomicsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<GenomeSample>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<GenomeSample>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] GenomeSample entity)
    { _db.Set<GenomeSample>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] GenomeSample entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<GenomeSample>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<GenomeSample>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
