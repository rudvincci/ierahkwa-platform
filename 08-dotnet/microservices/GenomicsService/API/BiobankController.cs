using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.GenomicsService.Infrastructure;
using Ierahkwa.GenomicsService.Domain;

namespace Ierahkwa.GenomicsService.API;

[ApiController]
[Route("api/[controller]")]
public class BiobankController : ControllerBase
{
    private readonly GenomicsServiceDbContext _db;
    public BiobankController(GenomicsServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<BiobankEntry>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<BiobankEntry>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] BiobankEntry entity)
    { _db.Set<BiobankEntry>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] BiobankEntry entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<BiobankEntry>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<BiobankEntry>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
