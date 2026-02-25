using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ierahkwa.SocialWelfareService.Infrastructure;
using Ierahkwa.SocialWelfareService.Domain;

namespace Ierahkwa.SocialWelfareService.API;

[ApiController]
[Route("api/[controller]")]
public class WelfareController : ControllerBase
{
    private readonly SocialWelfareServiceDbContext _db;
    public WelfareController(SocialWelfareServiceDbContext db) => _db = db;

    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? tenantId) =>
        Ok(await _db.Set<WelfareCase>().Where(x => tenantId == null || x.TenantId == tenantId).ToListAsync());

    [HttpGet("{id}")] public async Task<IActionResult> GetById(Guid id) =>
        await _db.Set<WelfareCase>().FindAsync(id) is { } e ? Ok(e) : NotFound();

    [HttpPost] public async Task<IActionResult> Create([FromBody] WelfareCase entity)
    { _db.Set<WelfareCase>().Add(entity); await _db.SaveChangesAsync(); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }

    [HttpPut("{id}")] public async Task<IActionResult> Update(Guid id, [FromBody] WelfareCase entity)
    { entity.Id = id; entity.UpdatedAt = DateTime.UtcNow; _db.Set<WelfareCase>().Update(entity); await _db.SaveChangesAsync(); return Ok(entity); }

    [HttpDelete("{id}")] public async Task<IActionResult> Delete(Guid id)
    { var e = await _db.Set<WelfareCase>().FindAsync(id); if (e == null) return NotFound(); e.IsActive = false; await _db.SaveChangesAsync(); return NoContent(); }
}
