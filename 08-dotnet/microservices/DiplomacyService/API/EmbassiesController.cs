using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.DiplomacyService.Infrastructure;
using Ierahkwa.DiplomacyService.Domain;

namespace Ierahkwa.DiplomacyService.API;

[ApiController]
[Route("api/[controller]")]
public class EmbassiesController : ControllerBase
{
    private readonly DiplomacyServiceDbContext _db;
    public EmbassiesController(DiplomacyServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<Embassy>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<Embassy>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] Embassy entity)
    { _db.Set<Embassy>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] Embassy entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<Embassy>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<Embassy>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
