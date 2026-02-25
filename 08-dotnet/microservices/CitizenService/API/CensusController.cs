using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.CitizenService.Infrastructure;
using Ierahkwa.CitizenService.Domain;

namespace Ierahkwa.CitizenService.API;

[ApiController]
[Route("api/[controller]")]
public class CensusController : ControllerBase
{
    private readonly CitizenServiceDbContext _db;
    public CensusController(CitizenServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<CensusEntry>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<CensusEntry>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] CensusEntry entity)
    { _db.Set<CensusEntry>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] CensusEntry entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<CensusEntry>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<CensusEntry>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
