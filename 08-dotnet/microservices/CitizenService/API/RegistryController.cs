using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.CitizenService.Infrastructure;
using Ierahkwa.CitizenService.Domain;

namespace Ierahkwa.CitizenService.API;

[ApiController]
[Route("api/[controller]")]
public class RegistryController : ControllerBase
{
    private readonly CitizenServiceDbContext _db;
    public RegistryController(CitizenServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<CitizenRecord>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<CitizenRecord>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] CitizenRecord entity)
    { _db.Set<CitizenRecord>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] CitizenRecord entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<CitizenRecord>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<CitizenRecord>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
